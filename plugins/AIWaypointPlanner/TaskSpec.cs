using System;
using System.Collections.Generic;

namespace MissionPlanner.AIWaypointPlanner
{
    public sealed class TaskSpec
    {
        public bool requires_clarification { get; set; }
        public string clarification_question { get; set; }
        public string mission_type { get; set; }
        public string summary { get; set; }
        public string source_summary { get; set; }
        public List<string> confirmed_requirements { get; set; }
        public List<string> source_files_used { get; set; }
        public double cruise_altitude_m { get; set; }
        public double cruise_speed_mps { get; set; }
        public double lane_spacing_m { get; set; }
        public double grid_angle_deg { get; set; }
        public bool include_takeoff { get; set; }
        public double takeoff_altitude_m { get; set; }
        public string completion_action { get; set; }
        public List<RelativeLeg> legs { get; set; }
        public List<string> safety_notes { get; set; }

        public TaskSpec()
        {
            clarification_question = string.Empty;
            mission_type = "unsupported";
            summary = string.Empty;
            source_summary = string.Empty;
            confirmed_requirements = new List<string>();
            source_files_used = new List<string>();
            completion_action = "RTL";
            legs = new List<RelativeLeg>();
            safety_notes = new List<string>();
        }
    }

    public sealed class RelativeLeg
    {
        public double bearing_deg { get; set; }
        public double distance_m { get; set; }
        public double altitude_m { get; set; }
        public string purpose { get; set; }

        public RelativeLeg()
        {
            purpose = string.Empty;
        }
    }

    public sealed class MissionContext
    {
        public MissionPlanner.Utilities.PointLatLngAlt Home { get; set; }
        public IList<MissionPlanner.Utilities.PointLatLngAlt> Polygon { get; set; }

        public MissionContext()
        {
            Polygon = new List<MissionPlanner.Utilities.PointLatLngAlt>();
        }
    }

    public sealed class CandidateMissionItem
    {
        public MAVLink.MAV_CMD Command { get; set; }
        public double Param1 { get; set; }
        public double Param2 { get; set; }
        public double Param3 { get; set; }
        public double Param4 { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Altitude { get; set; }
        public string Description { get; set; }

        public CandidateMissionItem()
        {
            Description = string.Empty;
        }
    }

    public sealed class CandidateMission
    {
        public string Summary { get; set; }
        public List<CandidateMissionItem> Items { get; private set; }

        public CandidateMission()
        {
            Summary = string.Empty;
            Items = new List<CandidateMissionItem>();
        }
    }

    public sealed class ValidationResult
    {
        public List<string> Errors { get; private set; }
        public List<string> Warnings { get; private set; }
        public bool IsValid { get { return Errors.Count == 0; } }

        public ValidationResult()
        {
            Errors = new List<string>();
            Warnings = new List<string>();
        }

        public void Merge(ValidationResult other)
        {
            if (other == null)
                return;

            Errors.AddRange(other.Errors);
            Warnings.AddRange(other.Warnings);
        }
    }

    public sealed class MissionGenerationResult
    {
        public TaskSpec Spec { get; set; }
        public CandidateMission Mission { get; set; }
        public ValidationResult Validation { get; set; }

        public MissionGenerationResult()
        {
            Validation = new ValidationResult();
        }
    }
}
