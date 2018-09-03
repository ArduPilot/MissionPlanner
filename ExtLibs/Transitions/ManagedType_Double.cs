using System;

namespace Transitions
{
    internal class ManagedType_Double : IManagedType
    {
        public Type getManagedType()
        {
            return typeof(double);
        }

        public object copy(object o)
        {
            return (double) o;
        }

        public object getIntermediateValue(object start, object end, double dPercentage)
        {
            return Utility.interpolate((double) start, (double) end, dPercentage);
        }
    }
}