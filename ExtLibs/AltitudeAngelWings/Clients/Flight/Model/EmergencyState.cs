using System;
using Newtonsoft.Json;

namespace AltitudeAngelWings.Clients.Flight.Model
{
    public class EmergencyState : IEquatable<EmergencyState>
    {
        /// <summary>
        /// The aircraft has currently declared an emergency
        /// </summary>
        [JsonProperty("inEmergency")]
        public bool IsInEmergency { get; set; } = false;

        /// <summary>
        /// Free text describing the reason for the emergency
        /// </summary>
        [JsonProperty("reason")]
        public string Reason { get; set; } = null;

        public bool Equals(EmergencyState other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return IsInEmergency == other.IsInEmergency && Reason == other.Reason;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EmergencyState)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (IsInEmergency.GetHashCode() * 397) ^ (Reason != null ? Reason.GetHashCode() : 0);
            }
        }

        public static bool operator ==(EmergencyState left, EmergencyState right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EmergencyState left, EmergencyState right)
        {
            return !Equals(left, right);
        }
    }
}