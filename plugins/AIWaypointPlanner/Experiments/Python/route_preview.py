"""Offline relative-route geometry experiment; output is not a flight mission."""

import argparse
import json
import math
import sys

EARTH_RADIUS_M = 6378100.0  # Mission Planner PointLatLngAlt.newpos


def number(value, name):
    """Reject ambiguous or non-finite input; this is not mission validation."""
    if isinstance(value, bool) or not isinstance(value, (int, float)):
        raise ValueError(f"{name} must be a finite number")
    try:
        result = float(value)
    except OverflowError as error:
        raise ValueError(f"{name} must be a finite number") from error
    if not math.isfinite(result):
        raise ValueError(f"{name} must be a finite number")
    return result


def destination(latitude, longitude, bearing, distance):
    """Match the host's spherical formula, including unwrapped longitude."""
    lat, lon, heading = map(math.radians, (latitude, longitude, bearing))
    arc = distance / EARTH_RADIUS_M
    next_lat = math.asin(math.sin(lat) * math.cos(arc)
                         + math.cos(lat) * math.sin(arc) * math.cos(heading))
    next_lon = lon + math.atan2(
        math.sin(heading) * math.sin(arc) * math.cos(lat),
        math.cos(arc) - math.sin(lat) * math.sin(next_lat))
    return math.degrees(next_lat), math.degrees(next_lon)


def preview(document):
    """Preview a TaskSpec-shaped relative_route with an explicit Home object."""
    if not isinstance(document, dict) or document.get("mission_type") != "relative_route":
        raise ValueError("mission_type must be relative_route")
    home, legs = document.get("home"), document.get("legs")
    if not isinstance(home, dict) or not isinstance(legs, list):
        raise ValueError("home must be an object and legs must be an array")
    latitude = number(home.get("latitude"), "home.latitude")
    longitude = number(home.get("longitude"), "home.longitude")
    if not -90 <= latitude <= 90 or not -180 <= longitude <= 180:
        raise ValueError("Home coordinates are outside geographic bounds")
    points = []
    for index, leg in enumerate(legs, 1):
        if not isinstance(leg, dict):
            raise ValueError(f"leg {index} must be an object")
        bearing = number(leg.get("bearing_deg"), f"leg {index} bearing_deg")
        distance = number(leg.get("distance_m"), f"leg {index} distance_m")
        altitude = number(leg.get("altitude_m"), f"leg {index} altitude_m")
        if distance < 0:
            raise ValueError(f"leg {index} distance_m must be non-negative")
        latitude, longitude = destination(latitude, longitude, bearing, distance)
        points.append({"leg": index, "latitude": latitude,
                       "longitude": longitude, "altitude_m": altitude})
    return {"preview_only": True, "safety_validated": False,
            "coordinate_model": "Mission Planner sphere R=6378100m; unwrapped longitude",
            "points": points}


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("input", nargs="?", default="-", help="JSON file or - for stdin")
    args = parser.parse_args()
    try:
        if args.input == "-":
            document = json.load(sys.stdin)
        else:
            with open(args.input, encoding="utf-8-sig") as source:
                document = json.load(source)
        print(json.dumps(preview(document), indent=2, allow_nan=False))
    except (OSError, ValueError) as error:
        print(f"Preview error: {error}", file=sys.stderr)
        return 2
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
