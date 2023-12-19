using MissionPlanner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static IronPython.Modules._ast;

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

        public Quality GetQuality()
        {
            if (comPort?.MAV?.cs == null || !(comPort?.BaseStream?.IsOpen ?? false))
            {
                return Quality.Off;
            }

            Quality currentQuality = Quality.Off;
            foreach (Quality quality in Enum.GetValues(typeof(Quality)))
            {
                if (AreAllConditionsMet(quality))
                {
                    currentQuality = quality;
                }
                else
                {
                    break;
                }
            }
            return currentQuality;
        }

        private bool AreAllConditionsMet(Quality quality)
        {
            if (!Conditions.ContainsKey(quality))
            {
                return true;
            }

            return Conditions[quality].All(condition => condition.IsMet(this));
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

    public interface ILinkCondition
    {
        bool IsMet(Link link);
        int DelaySeconds { get; set; }        
    }

    public abstract class LinkConditionBase : ILinkCondition
    {
        public int DelaySeconds { get; set; }

        public bool IsMet(Link link)
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

            return false;
        }

        protected DateTime? timeMetFirst = null;
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
                return false;
            }

            var lastPacketAge = (DateTime.Now - link.comPort.MAV.lastvalidpacket).TotalSeconds;
            return lastPacketAge <= ThresholdSeconds;
        }
    }

    public class LinkQualityCondition : LinkConditionBase
    {
        public int ThresholdPercent { get; set; }

        protected override bool IsMetNow(Link link)
        {
            if (link?.comPort?.MAV?.cs == null)
            {
                return false;
            }

            return link?.comPort.MAV.cs.linkqualitygcs >= ThresholdPercent;
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
                return false;
            }

            // Use reflection to get the value of the named state
            PropertyInfo propertyInfo = (link?.comPort.MAV.cs.GetType().GetProperty(StateName)) ?? throw new InvalidOperationException($"State '{StateName}' does not exist.");
            try
            {
                double value = Convert.ToDouble(propertyInfo.GetValue(link?.comPort.MAV.cs));
                return value >= MinValue && value <= MaxValue;
            }
            catch (InvalidCastException)
            {
                throw new InvalidOperationException($"State '{StateName}' is not a numeric type.");
            }
        }
    }
}
