using System;
using System.Collections.Generic;
using System.Text;

namespace Zeroconf
{
    public struct ServiceAnnouncement : IEquatable<ServiceAnnouncement>
    {
        public bool Equals(ServiceAnnouncement other)
        {
            return AdapterInformation.Equals(other.AdapterInformation) && Equals(Host, other.Host);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ServiceAnnouncement && Equals((ServiceAnnouncement)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (AdapterInformation.GetHashCode()*397) ^ (Host != null ? Host.GetHashCode() : 0);
            }
        }

        public static bool operator ==(ServiceAnnouncement left, ServiceAnnouncement right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ServiceAnnouncement left, ServiceAnnouncement right)
        {
            return !left.Equals(right);
        }

        public AdapterInformation AdapterInformation { get; }
        public IZeroconfHost Host { get; }

        public ServiceAnnouncement(AdapterInformation adapterInformation, IZeroconfHost host)
        {
            if (host == null) throw new ArgumentNullException(nameof(host));
            AdapterInformation = adapterInformation;
            Host = host;
        }
    }
}
