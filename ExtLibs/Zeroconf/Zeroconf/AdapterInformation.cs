using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zeroconf
{
    public struct AdapterInformation : IEquatable<AdapterInformation>
    {
        public bool Equals(AdapterInformation other)
        {
            return string.Equals(Address, other.Address) && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AdapterInformation && Equals((AdapterInformation)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Address != null ? Address.GetHashCode() : 0)*397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        public static bool operator ==(AdapterInformation left, AdapterInformation right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AdapterInformation left, AdapterInformation right)
        {
            return !left.Equals(right);
        }

        public AdapterInformation(string address, string name)
        {
            Address = address;
            Name = name;
        }

        public string Address { get; }

        public string Name { get; }

        public override string ToString()
        {
            return $"{Name}: {Address}";
        }
    }
}