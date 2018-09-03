using System;

namespace Transitions
{
    internal class ManagedType_Int : IManagedType
    {
        public Type getManagedType()
        {
            return typeof(int);
        }

        public object copy(object o)
        {
            return (int) o;
        }

        public object getIntermediateValue(object start, object end, double dPercentage)
        {
            return Utility.interpolate((int) start, (int) end, dPercentage);
        }
    }
}