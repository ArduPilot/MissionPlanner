using MissionPlanner;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace RedundantLinkManager
{
    /// <summary>
    /// Options/Settings and MAVLinkInterface for a single link. When the settings are changed, the MAVLinkInterface
    /// is disposed so that it can be reconnected by the autoconnect thread.
    /// </summary>
    public class Link : ICloneable, IDisposable, IEquatable<Link>
    {
        private bool _enabled;
        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                Dispose();
            }
        }

        private string _type;
        public string Type
        {
            get { return _type; }
            set
            {
                if (_type == value) return;
                _type = value;
                Dispose();
            }
        }

        private string _hostOrCom;
        public string HostOrCom
        {
            get { return _hostOrCom; }
            set
            {
                if (_hostOrCom == value) return;
                _hostOrCom = value;
                Dispose();
            }
        }

        private string _portOrBaud;
        public string PortOrBaud
        {
            get { return _portOrBaud; }
            set
            {
                if (_portOrBaud == value) return;
                _portOrBaud = value;
                Dispose();
            }
        }

        public string Name { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        public MAVLinkInterface comPort = null;

        public enum Quality
        {
            Off,
            Critical,
            Marginal,
            Good,
        }

        [Newtonsoft.Json.JsonIgnore]
        public readonly Dictionary<Quality, List<ILinkCondition>> Conditions = new Dictionary<Quality, List<ILinkCondition>>()
        {
            { Quality.Critical, new List<ILinkCondition>()
            {
                new LastValidPacketCondition() { DelaySeconds = 0, ThresholdSeconds = 20 },
                new LinkQualityCondition() { DelaySeconds = 5, ThresholdPercent = 10 }
            } },
            { Quality.Marginal, new List<ILinkCondition>()
            {
                new LastValidPacketCondition() { DelaySeconds = 5, ThresholdSeconds = 5 },
                new LinkQualityCondition() { DelaySeconds = 5, ThresholdPercent = 85 }
            } },
            { Quality.Good, new List<ILinkCondition>()
            {
                new LastValidPacketCondition() { DelaySeconds = 5, ThresholdSeconds = 1 },
                new LinkQualityCondition() { DelaySeconds = 5, ThresholdPercent = 95 }
            } },
        };

        public QualityResult GetQuality()
        {
            if (!Enabled)
            {
                return new QualityResult() { CurrentQuality = Quality.Off, Reasons = "User Disabled" };
            }
            if (comPort?.MAV?.cs == null || !(comPort?.BaseStream?.IsOpen ?? false))
            {
                return new QualityResult() { CurrentQuality = Quality.Off, Reasons = "Disconnected" };
            }

            QualityResult currentQuality = new QualityResult() { CurrentQuality = Quality.Off };

            foreach (Quality quality in Enum.GetValues(typeof(Quality)))
            {
                var reasons = new List<string>();
                if (AreAllConditionsMet(quality, reasons))
                {
                    currentQuality.CurrentQuality = quality;
                }
                else
                {
                    currentQuality.Reasons = string.Join("\r\n", reasons);
                    break;
                }
            }
            return currentQuality;
        }

        private bool AreAllConditionsMet(Quality quality, List<string> reasons = null)
        {
            // If there are no conditions for this quality, then it is always met
            if (!Conditions.ContainsKey(quality))
            {
                return true;
            }

            // Loop through all conditions for this quality and return true if all are met
            // If we are collecting reasons, then do not short-circuit
            bool allMet = true;
            foreach (var condition in Conditions[quality])
            {
                if (!condition.IsMet(this, reasons))
                {
                    allMet = false;

                    // If not collecting reasons, no need to continue checking
                    if (reasons == null)
                    {
                        break;
                    }
                }
            }
            return allMet;
        }

        public void Dispose()
        {
            comPort?.Dispose();
            comPort = null;
        }

        public object Clone()
        {
            return new Link()
            {
                _enabled = Enabled,
                _type = Type,
                _hostOrCom = HostOrCom,
                _portOrBaud = PortOrBaud,
                Name = Name
            };
        }

        public bool Equals(Link other)
        {
            return Enabled == other.Enabled &&
                Type == other.Type &&
                HostOrCom == other.HostOrCom &&
                PortOrBaud == other.PortOrBaud &&
                Name == other.Name;
        }
    }

    public class QualityResult
    {
        public Link.Quality CurrentQuality;
        public string Reasons;
    }

    public interface ILinkCondition
    {
        bool IsMet(Link link, List<string> reasons);
        int DelaySeconds { get; set; }
        string Reason { get; set; }
    }

    public abstract class LinkConditionBase : ILinkCondition
    {
        public int DelaySeconds { get; set; }

        public string Reason { get; set; }

        public bool IsMet(Link link, List<string> reasons = null)
        {
            if (IsMetNow(link))
            {
                if (!timeMetFirst.HasValue)
                {
                    // Condition met for the first time, start the timer
                    timeMetFirst = DateTime.Now;
                }
                else if ((DateTime.Now - timeMetFirst.Value).TotalSeconds >= DelaySeconds)
                {
                    // Condition has been continuously met for the specified delay
                    return true;
                }
            }
            else
            {
                // Condition not met, reset the timer
                timeMetFirst = null;
            }

            if (reasons != null && Reason != null)
            {
                reasons.Add(Reason + (timeMetFirst.HasValue ? $" {(DateTime.Now - timeMetFirst.Value).TotalSeconds:0}s ago" : ""));
            }

            return false;
        }

        // Initialize to MinValue to skip the wait the first time the condition is met
        protected DateTime? timeMetFirst = DateTime.MinValue;
        protected abstract bool IsMetNow(Link link);
    }

    public class LastValidPacketCondition : LinkConditionBase
    {
        // Threshold in seconds for the last valid packet
        public int ThresholdSeconds { get; set; }

        protected override bool IsMetNow(Link link)
        {
            if (link?.comPort?.MAV?.lastvalidpacket == null)
            {
                Reason = "No valid packets";
                return false;
            }

            var lastPacketAge = (DateTime.Now - link.comPort.MAV.lastvalidpacket).TotalSeconds;
            if(lastPacketAge > ThresholdSeconds)
            {
                Reason = $"{lastPacketAge:0}s dropout";
                return false;
            }
            return true;
        }
    }

    public class LinkQualityCondition : LinkConditionBase
    {
        public int ThresholdPercent { get; set; }

        protected override bool IsMetNow(Link link)
        {
            if (link?.comPort?.MAV?.cs == null)
            {
                Reason = "No link quality";
                return false;
            }

            if(link?.comPort.MAV.cs.linkqualitygcs < ThresholdPercent)
            {
                Reason = $"Link quality {link?.comPort.MAV.cs.linkqualitygcs:0} < {ThresholdPercent:0}";
                return false;
            }
            return true;
        }
    }

    public class NamedStateCondition : LinkConditionBase
    {
        public string StateName { get; set; }
        public double MinValue { get; set; }
        public double MaxValue { get; set; }

        protected override bool IsMetNow(Link link)
        {
            if (link?.comPort?.MAV?.cs == null)
            {
                Reason = "No state object";
                return false;
            }

            // Use reflection to get the value of the named state
            PropertyInfo propertyInfo = (link?.comPort.MAV.cs.GetType().GetProperty(StateName)) ?? throw new InvalidOperationException($"State '{StateName}' does not exist.");
            try
            {
                double value = Convert.ToDouble(propertyInfo.GetValue(link?.comPort.MAV.cs));
                if (value < MinValue)
                {
                    Reason = $"{StateName} {value:0.00} < {MinValue:0.00}";
                    return false;
                }
                if (value > MaxValue)
                {
                    Reason = $"{StateName} {value:0.00} > {MaxValue:0.00}";
                    return false;
                }
                return true;
            }
            catch (InvalidCastException)
            {
                throw new InvalidOperationException($"State '{StateName}' is not a numeric type.");
            }
        }
    }
}
