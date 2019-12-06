using System;
using System.Collections.Generic;
using System.Text;

namespace Zeroconf
{
    public struct DomainService : IEquatable<DomainService>
    {
        public bool Equals(DomainService other)
        {
            return string.Equals(Domain, other.Domain) && string.Equals(Service, other.Service);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is DomainService && Equals((DomainService)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Domain != null ? Domain.GetHashCode() : 0)*397) ^ (Service != null ? Service.GetHashCode() : 0);
            }
        }

        public static bool operator ==(DomainService left, DomainService right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DomainService left, DomainService right)
        {
            return !left.Equals(right);
        }

        public DomainService(string domain, string service)
        {
            Domain = domain;
            Service = service;
        }
        public string Domain { get; }
        public string Service { get; }
    }
}
