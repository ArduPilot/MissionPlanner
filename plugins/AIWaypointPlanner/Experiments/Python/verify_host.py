"""Compare Python output with the installed C# PointLatLngAlt.newpos method."""

import argparse
import json
import math
import os
from pathlib import Path
import random
import subprocess

from route_preview import preview


def make_cases():
    cases = []
    for lat, lon, bearing, distance in (
        (0, 0, 90, 0), (31, 121, 90, 100), (0, 179.999, 90, 1000),
        (89.9, -45, 0, 25000), (-89.9, 20, 180, 25000),
    ):
        cases.append({"mission_type": "relative_route",
                      "home": {"latitude": lat, "longitude": lon},
                      "legs": [{"bearing_deg": bearing, "distance_m": distance, "altitude_m": 50}]})
    generator = random.Random(3100)
    for _ in range(64):
        cases.append({"mission_type": "relative_route",
                      "home": {"latitude": generator.uniform(-85, 85),
                               "longitude": generator.uniform(-180, 180)},
                      "legs": [{"bearing_deg": generator.uniform(-720, 720),
                                "distance_m": generator.uniform(0, 50000),
                                "altitude_m": generator.uniform(-100, 1000)}
                               for _ in range(generator.randint(1, 12))]})
    return cases


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    default_host = Path(os.environ.get("ProgramFiles(x86)", r"C:\Program Files (x86)")) / "Mission Planner"
    parser.add_argument("--host-dir", type=Path, default=default_host)
    args = parser.parse_args()
    cases = make_cases()
    result = subprocess.run(
        ["powershell.exe", "-NoProfile", "-NonInteractive", "-ExecutionPolicy", "Bypass", "-File",
         str(Path(__file__).with_name("host_reference.ps1")), "-HostDirectory", str(args.host_dir)],
        input=json.dumps(cases), text=True, encoding="utf-8", errors="replace",
        capture_output=True, timeout=60)
    if result.returncode:
        raise RuntimeError(f"Host reference failed:\n{result.stderr or result.stdout}")
    reference = json.loads(result.stdout)
    if len(reference["cases"]) != len(cases):
        raise AssertionError("Host returned the wrong number of cases")
    largest_error, largest_altitude_error, checked = 0.0, 0.0, 0
    for case, expected in zip(cases, reference["cases"]):
        actual = preview(case)["points"]
        if len(actual) != len(expected["points"]):
            raise AssertionError("Host returned the wrong number of points")
        for left, right in zip(actual, expected["points"]):
            for field in ("latitude", "longitude"):
                error = abs(left[field] - right[field])
                if not all(map(math.isfinite, (left[field], right[field], error))):
                    raise AssertionError(f"{field}: non-finite comparison result")
                largest_error = max(largest_error, error)
                if error > 1e-9:
                    raise AssertionError(f"{field}: {left[field]} != {right[field]}")
            altitude_error = abs(left["altitude_m"] - right["altitude_m"])
            if not all(map(math.isfinite, (left["altitude_m"], right["altitude_m"], altitude_error))):
                raise AssertionError("Non-finite altitude comparison result")
            largest_altitude_error = max(largest_altitude_error, altitude_error)
            if altitude_error > 1e-9:
                raise AssertionError("Altitude was changed")
            checked += 1
    print(f"PASS: {len(cases)} routes, {checked} points; maximum coordinate error {largest_error:.3g} degrees")
    print(f"Maximum altitude error after JSON round trip: {largest_altitude_error:.3g} metres")
    print(f"Reference assembly version: {reference['assembly_version']}")
    print(f"Mission Planner version: {reference['host_version']}")
    print(f"Mission Planner assembly version: {reference['host_assembly_version']}")
    print(f"Reference assembly SHA-256: {reference['assembly_sha256']}")


if __name__ == "__main__":
    main()
