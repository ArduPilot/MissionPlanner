using System;
using System.Collections.Generic;
using System.Linq;
using MissionPlanner.Utilities;

namespace MissionPlanner.AIWaypointPlanner
{
    public sealed class MissionValidator
    {
        public const int MaximumMissionItems = 200;
        public const int MaximumRelativeLegs = 50;
        public const int MaximumPolygonVertices = 250;
        public const double MaximumDistanceFromHomeM = 20000.0;
        public const double MaximumTotalRouteM = 50000.0;

        private string languageCode;

        public MissionValidator()
            : this(UiStrings.DefaultLanguageCode)
        {
        }

        public MissionValidator(string languageCode)
        {
            LanguageCode = languageCode;
        }

        public string LanguageCode
        {
            get { return languageCode; }
            set { languageCode = UiStrings.NormalizeLanguageCode(value); }
        }

        private string L(string key)
        {
            return UiStrings.Get(languageCode, key);
        }

        private string F(string key, params object[] arguments)
        {
            return UiStrings.Format(languageCode, key, arguments);
        }

        public ValidationResult ValidateSpec(TaskSpec spec, MissionContext context)
        {
            var result = new ValidationResult();

            if (spec == null)
            {
                result.Errors.Add(L("Validation.NoSpec"));
                return result;
            }

            if (spec.requires_clarification)
            {
                result.Errors.Add(string.IsNullOrWhiteSpace(spec.clarification_question)
                    ? L("Validation.NeedsClarification")
                    : F("Validation.NeedsClarificationFormat", spec.clarification_question.Trim()));
            }

            if (string.IsNullOrWhiteSpace(spec.source_summary))
                result.Errors.Add(L("Validation.SourceSummaryMissing"));
            if (spec.confirmed_requirements == null ||
                !spec.confirmed_requirements.Any(requirement => !string.IsNullOrWhiteSpace(requirement)))
            {
                result.Errors.Add(L("Validation.RequirementsMissing"));
            }

            if (context == null || !IsValidHome(context.Home))
                result.Errors.Add(L("Validation.HomeInvalid"));

            if (spec.mission_type != "survey_polygon" &&
                spec.mission_type != "relative_route" &&
                spec.mission_type != "unsupported")
            {
                result.Errors.Add(L("Validation.MissionTypeInvalid"));
            }

            if (spec.mission_type == "unsupported")
                result.Errors.Add(L("Validation.UnsupportedTemplate"));

            ValidateNumber(result, spec.cruise_altitude_m, 30.0, 500.0, L("Validation.FieldCruiseAltitude"));
            ValidateNumber(result, spec.cruise_speed_mps, 8.0, 45.0, L("Validation.FieldCruiseSpeed"));
            if (spec.include_takeoff)
                ValidateNumber(result, spec.takeoff_altitude_m, 20.0, 200.0, L("Validation.FieldTakeoffAltitude"));

            if (!string.Equals(spec.completion_action, "RTL", StringComparison.Ordinal))
                result.Errors.Add(L("Validation.CompletionMustRtl"));

            if (spec.mission_type == "survey_polygon")
                ValidateSurveySpec(result, spec, context);
            else if (spec.mission_type == "relative_route")
                ValidateRelativeSpec(result, spec);

            if (spec.safety_notes != null)
            {
                foreach (string note in spec.safety_notes.Where(n => !string.IsNullOrWhiteSpace(n)))
                    result.Warnings.Add(F("Validation.AiNoteFormat", note.Trim()));
            }

            result.Warnings.Add(L("Validation.LocalOnlyWarning"));
            return result;
        }

        public ValidationResult ValidateMission(CandidateMission mission, PointLatLngAlt home)
        {
            var result = new ValidationResult();
            if (mission == null || mission.Items == null || mission.Items.Count == 0)
            {
                result.Errors.Add(L("Validation.NoMissionItems"));
                return result;
            }

            if (mission.Items.Count > MaximumMissionItems)
                result.Errors.Add(F("Validation.TooManyItemsFormat", MaximumMissionItems));

            if (!IsValidHome(home))
            {
                result.Errors.Add(L("Validation.MissionHomeInvalid"));
                return result;
            }

            PointLatLngAlt previous = home;
            double totalRoute = 0.0;
            int waypointCount = 0;

            foreach (CandidateMissionItem item in mission.Items)
            {
                if (item.Command != MAVLink.MAV_CMD.WAYPOINT)
                    continue;

                var point = new PointLatLngAlt(item.Latitude, item.Longitude, item.Altitude);
                if (!IsValidCoordinate(point))
                {
                    result.Errors.Add(L("Validation.InvalidCoordinate"));
                    continue;
                }

                double distanceFromHome = home.GetDistance(point);
                if (!IsFinite(distanceFromHome) || distanceFromHome > MaximumDistanceFromHomeM)
                    result.Errors.Add(F("Validation.WaypointTooFarFormat", MaximumDistanceFromHomeM));

                double segment = previous.GetDistance(point);
                if (!IsFinite(segment))
                    result.Errors.Add(L("Validation.SegmentDistanceInvalid"));
                else
                    totalRoute += segment;

                previous = point;
                waypointCount++;
            }

            if (waypointCount == 0)
                result.Errors.Add(L("Validation.NoWaypoints"));

            if (totalRoute > MaximumTotalRouteM)
                result.Errors.Add(F("Validation.RouteTooLongFormat", MaximumTotalRouteM));

            CandidateMissionItem last = mission.Items[mission.Items.Count - 1];
            if (last.Command != MAVLink.MAV_CMD.RETURN_TO_LAUNCH)
                result.Errors.Add(L("Validation.MustEndRtl"));

            return result;
        }

        public static bool IsValidHome(PointLatLngAlt home)
        {
            return home != null && IsValidCoordinate(home) &&
                   !(Math.Abs(home.Lat) < 0.000001 && Math.Abs(home.Lng) < 0.000001);
        }

        private void ValidateSurveySpec(ValidationResult result, TaskSpec spec, MissionContext context)
        {
            ValidateNumber(result, spec.lane_spacing_m, 20.0, 500.0, L("Validation.FieldLaneSpacing"));
            ValidateNumber(result, spec.grid_angle_deg, 0.0, 359.999, L("Validation.FieldGridAngle"));

            IList<PointLatLngAlt> polygon = context == null ? null : context.Polygon;
            if (polygon == null || polygon.Count < 3)
            {
                result.Errors.Add(L("Validation.SurveyPolygonMissing"));
                return;
            }

            if (polygon.Count > MaximumPolygonVertices)
                result.Errors.Add(F("Validation.TooManyVerticesFormat", MaximumPolygonVertices));

            if (context != null && IsValidHome(context.Home))
            {
                foreach (PointLatLngAlt vertex in polygon)
                {
                    if (!IsValidCoordinate(vertex))
                    {
                        result.Errors.Add(L("Validation.PolygonInvalid"));
                        break;
                    }

                    if (context.Home.GetDistance(vertex) > MaximumDistanceFromHomeM)
                    {
                        result.Errors.Add(F("Validation.PolygonTooFarFormat", MaximumDistanceFromHomeM));
                        break;
                    }
                }
            }
        }

        private void ValidateRelativeSpec(ValidationResult result, TaskSpec spec)
        {
            if (spec.legs == null || spec.legs.Count == 0)
            {
                result.Errors.Add(L("Validation.NoRelativeLegs"));
                return;
            }

            if (spec.legs.Count > MaximumRelativeLegs)
                result.Errors.Add(F("Validation.TooManyLegsFormat", MaximumRelativeLegs));

            double total = 0.0;
            for (int i = 0; i < spec.legs.Count; i++)
            {
                RelativeLeg leg = spec.legs[i];
                if (leg == null)
                {
                    result.Errors.Add(F("Validation.EmptyLegFormat", i + 1));
                    continue;
                }

                ValidateNumber(result, leg.bearing_deg, 0.0, 359.999,
                    F("Validation.FieldLegBearingFormat", i + 1));
                ValidateNumber(result, leg.distance_m, 50.0, 10000.0,
                    F("Validation.FieldLegDistanceFormat", i + 1));
                ValidateNumber(result, leg.altitude_m, 30.0, 500.0,
                    F("Validation.FieldLegAltitudeFormat", i + 1));
                if (IsFinite(leg.distance_m))
                    total += leg.distance_m;
            }

            if (total > MaximumTotalRouteM)
                result.Errors.Add(F("Validation.RelativeRouteTooLongFormat", MaximumTotalRouteM));
        }

        private void ValidateNumber(ValidationResult result, double value, double minimum, double maximum, string name)
        {
            if (!IsFinite(value) || value < minimum || value > maximum)
                result.Errors.Add(F("Validation.NumberRangeFormat", name, minimum, maximum));
        }

        private static bool IsValidCoordinate(PointLatLngAlt point)
        {
            return point != null && IsFinite(point.Lat) && IsFinite(point.Lng) && IsFinite(point.Alt) &&
                   point.Lat >= -90.0 && point.Lat <= 90.0 && point.Lng >= -180.0 && point.Lng <= 180.0;
        }

        private static bool IsFinite(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}
