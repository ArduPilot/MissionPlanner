"""Standard-library tests for the geometry preview and its JSON contract."""

import copy
import json
import math
from pathlib import Path
import subprocess
import sys
import unittest

from route_preview import EARTH_RADIUS_M, destination, preview

SAMPLE = json.loads(Path(__file__).with_name("sample.json").read_text(encoding="utf-8"))


class PreviewTests(unittest.TestCase):
    def test_zero_distance(self):
        lat, lon = destination(31, 121, 90, 0)
        self.assertAlmostEqual(lat, 31, places=12)
        self.assertAlmostEqual(lon, 121, places=12)

    def test_equatorial_east(self):
        lat, lon = destination(0, 0, 90, 1000)
        self.assertAlmostEqual(lat, 0, places=12)
        self.assertAlmostEqual(lon, math.degrees(1000 / EARTH_RADIUS_M), places=12)

    def test_sequential_legs_and_altitude(self):
        result = preview(SAMPLE)
        first = destination(31, 121, 90, 100)
        second = destination(*first, 0, 100)
        self.assertEqual(result["points"][1]["latitude"], second[0])
        self.assertEqual(result["points"][1]["longitude"], second[1])
        self.assertEqual([point["altitude_m"] for point in result["points"]], [50, 60])

    def test_preview_contract(self):
        result = preview(SAMPLE)
        self.assertTrue(result["preview_only"])
        self.assertFalse(result["safety_validated"])
        self.assertNotIn("command", result["points"][0])

    def test_antimeridian_matches_host_unwrapped_longitude(self):
        self.assertGreater(destination(0, 179.999, 90, 1000)[1], 180)

    def test_input_is_not_modified(self):
        before = copy.deepcopy(SAMPLE)
        preview(SAMPLE)
        self.assertEqual(before, SAMPLE)

    def test_empty_route_has_no_points(self):
        document = dict(SAMPLE, legs=[])
        self.assertEqual(preview(document)["points"], [])

    def test_nonfinite_and_ambiguous_numbers(self):
        for value in (float("nan"), float("inf"), True, "100", None, 10 ** 400):
            with self.subTest(value=repr(value)):
                document = copy.deepcopy(SAMPLE)
                document["legs"][0]["distance_m"] = value
                with self.assertRaises(ValueError):
                    preview(document)

    def test_negative_distance(self):
        document = copy.deepcopy(SAMPLE)
        document["legs"][0]["distance_m"] = -1
        with self.assertRaises(ValueError):
            preview(document)

    def test_invalid_shape_and_home(self):
        for document in ([], {}, dict(SAMPLE, mission_type="survey_polygon"),
                         dict(SAMPLE, home={"latitude": 91, "longitude": 0}),
                         dict(SAMPLE, legs=[None])):
            with self.subTest(document=document), self.assertRaises(ValueError):
                preview(document)

    def test_cli_emits_json(self):
        result = self.run_cli(json.dumps(SAMPLE))
        self.assertEqual(result.returncode, 0, result.stderr)
        self.assertEqual(json.loads(result.stdout), preview(SAMPLE))

    def test_cli_rejects_malformed_json(self):
        result = self.run_cli("{")
        self.assertEqual(result.returncode, 2)
        self.assertEqual(result.stdout, "")
        self.assertIn("Preview error:", result.stderr)

    @staticmethod
    def run_cli(payload):
        return subprocess.run([sys.executable, str(Path(__file__).with_name("route_preview.py"))],
                              input=payload, text=True, capture_output=True, timeout=10)


if __name__ == "__main__":
    unittest.main()
