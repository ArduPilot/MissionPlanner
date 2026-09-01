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

        public ValidationResult ValidateSpec(TaskSpec spec, MissionContext context)
        {
            var result = new ValidationResult();

            if (spec == null)
            {
                result.Errors.Add("GPT 未返回可解析的任务参数。");
                return result;
            }

            if (spec.requires_clarification)
            {
                result.Errors.Add(string.IsNullOrWhiteSpace(spec.clarification_question)
                    ? "任务信息不足，需要补充说明。"
                    : "需要补充说明：" + spec.clarification_question.Trim());
            }

            if (string.IsNullOrWhiteSpace(spec.source_summary))
                result.Errors.Add("AI 未提供对任务资料的理解摘要，不能进入应用流程。");
            if (spec.confirmed_requirements == null ||
                !spec.confirmed_requirements.Any(requirement => !string.IsNullOrWhiteSpace(requirement)))
            {
                result.Errors.Add("AI 未列出需要操作员确认的任务要求。");
            }

            if (context == null || !IsValidHome(context.Home))
                result.Errors.Add("Mission Planner 的计划 Home 无效，请先设置正确的 Home 位置。");

            if (spec.mission_type != "survey_polygon" &&
                spec.mission_type != "relative_route" &&
                spec.mission_type != "unsupported")
            {
                result.Errors.Add("模型返回了不受支持的任务类型。");
            }

            if (spec.mission_type == "unsupported")
                result.Errors.Add("当前版本无法把该目标转换为受支持的确定性任务模板。");

            ValidateNumber(result, spec.cruise_altitude_m, 30.0, 500.0, "巡航高度");
            ValidateNumber(result, spec.cruise_speed_mps, 8.0, 45.0, "巡航速度");
            if (spec.include_takeoff)
                ValidateNumber(result, spec.takeoff_altitude_m, 20.0, 200.0, "起飞高度");

            if (!string.Equals(spec.completion_action, "RTL", StringComparison.Ordinal))
                result.Errors.Add("当前版本只允许 RTL 作为任务结束动作。");

            if (spec.mission_type == "survey_polygon")
                ValidateSurveySpec(result, spec, context);
            else if (spec.mission_type == "relative_route")
                ValidateRelativeSpec(result, spec);

            if (spec.safety_notes != null)
            {
                foreach (string note in spec.safety_notes.Where(n => !string.IsNullOrWhiteSpace(n)))
                    result.Warnings.Add("GPT 提示：" + note.Trim());
            }

            result.Warnings.Add("候选任务不会自动上传、解锁、起飞或改变飞行模式；应用后仍需人工复核并手动写入飞控。");
            return result;
        }

        public ValidationResult ValidateMission(CandidateMission mission, PointLatLngAlt home)
        {
            var result = new ValidationResult();
            if (mission == null || mission.Items == null || mission.Items.Count == 0)
            {
                result.Errors.Add("本地任务编译器没有生成任何任务项。");
                return result;
            }

            if (mission.Items.Count > MaximumMissionItems)
                result.Errors.Add("任务项数量超过上限 " + MaximumMissionItems + "。");

            if (!IsValidHome(home))
            {
                result.Errors.Add("无法依据无效 Home 校验任务范围。");
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
                    result.Errors.Add("候选任务包含无效经纬度或高度。");
                    continue;
                }

                double distanceFromHome = home.GetDistance(point);
                if (!IsFinite(distanceFromHome) || distanceFromHome > MaximumDistanceFromHomeM)
                    result.Errors.Add("候选航点超出 Home 周围 " + MaximumDistanceFromHomeM.ToString("0") + " 米限制。");

                double segment = previous.GetDistance(point);
                if (!IsFinite(segment))
                    result.Errors.Add("无法计算候选航段距离。");
                else
                    totalRoute += segment;

                previous = point;
                waypointCount++;
            }

            if (waypointCount == 0)
                result.Errors.Add("候选任务不包含有效航点。");

            if (totalRoute > MaximumTotalRouteM)
                result.Errors.Add("候选航线总长度超过 " + MaximumTotalRouteM.ToString("0") + " 米限制。");

            CandidateMissionItem last = mission.Items[mission.Items.Count - 1];
            if (last.Command != MAVLink.MAV_CMD.RETURN_TO_LAUNCH)
                result.Errors.Add("候选任务必须以 RETURN_TO_LAUNCH 结束。");

            return result;
        }

        public static bool IsValidHome(PointLatLngAlt home)
        {
            return home != null && IsValidCoordinate(home) &&
                   !(Math.Abs(home.Lat) < 0.000001 && Math.Abs(home.Lng) < 0.000001);
        }

        private static void ValidateSurveySpec(ValidationResult result, TaskSpec spec, MissionContext context)
        {
            ValidateNumber(result, spec.lane_spacing_m, 20.0, 500.0, "航线间距");
            ValidateNumber(result, spec.grid_angle_deg, 0.0, 359.999, "网格角度");

            IList<PointLatLngAlt> polygon = context == null ? null : context.Polygon;
            if (polygon == null || polygon.Count < 3)
            {
                result.Errors.Add("区域巡视需要先在 Flight Planner 地图上绘制至少三个顶点的多边形。");
                return;
            }

            if (polygon.Count > MaximumPolygonVertices)
                result.Errors.Add("多边形顶点数量超过上限 " + MaximumPolygonVertices + "。");

            if (context != null && IsValidHome(context.Home))
            {
                foreach (PointLatLngAlt vertex in polygon)
                {
                    if (!IsValidCoordinate(vertex))
                    {
                        result.Errors.Add("绘制的多边形包含无效坐标。");
                        break;
                    }

                    if (context.Home.GetDistance(vertex) > MaximumDistanceFromHomeM)
                    {
                        result.Errors.Add("绘制区域超出 Home 周围 " + MaximumDistanceFromHomeM.ToString("0") + " 米限制。");
                        break;
                    }
                }
            }
        }

        private static void ValidateRelativeSpec(ValidationResult result, TaskSpec spec)
        {
            if (spec.legs == null || spec.legs.Count == 0)
            {
                result.Errors.Add("相对航线至少需要一个航段。");
                return;
            }

            if (spec.legs.Count > MaximumRelativeLegs)
                result.Errors.Add("相对航段数量超过上限 " + MaximumRelativeLegs + "。");

            double total = 0.0;
            for (int i = 0; i < spec.legs.Count; i++)
            {
                RelativeLeg leg = spec.legs[i];
                if (leg == null)
                {
                    result.Errors.Add("第 " + (i + 1) + " 个航段为空。");
                    continue;
                }

                ValidateNumber(result, leg.bearing_deg, 0.0, 359.999, "第 " + (i + 1) + " 航段方位角");
                ValidateNumber(result, leg.distance_m, 50.0, 10000.0, "第 " + (i + 1) + " 航段距离");
                ValidateNumber(result, leg.altitude_m, 30.0, 500.0, "第 " + (i + 1) + " 航段高度");
                if (IsFinite(leg.distance_m))
                    total += leg.distance_m;
            }

            if (total > MaximumTotalRouteM)
                result.Errors.Add("模型给出的相对航段总长超过 " + MaximumTotalRouteM.ToString("0") + " 米限制。");
        }

        private static void ValidateNumber(ValidationResult result, double value, double minimum, double maximum, string name)
        {
            if (!IsFinite(value) || value < minimum || value > maximum)
                result.Errors.Add(name + "必须在 " + minimum.ToString("0.###") + " 至 " + maximum.ToString("0.###") + " 之间。");
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
