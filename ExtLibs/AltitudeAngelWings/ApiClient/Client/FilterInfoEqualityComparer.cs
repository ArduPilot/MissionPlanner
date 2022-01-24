using System.Collections.Generic;

namespace AltitudeAngelWings.ApiClient.Client
{
    public class FilterInfoDisplayEqualityComparer : IEqualityComparer<FilterInfoDisplay>
    {
        public bool Equals(FilterInfoDisplay x, FilterInfoDisplay y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (ReferenceEquals(x, null)) return false;
            if (ReferenceEquals(y, null)) return false;
            if (x.GetType() != y.GetType()) return false;
            return x.Name == y.Name;
        }

        public int GetHashCode(FilterInfoDisplay obj)
        {
            return obj.Name != null ? obj.Name.GetHashCode() : 0;
        }
    }
}