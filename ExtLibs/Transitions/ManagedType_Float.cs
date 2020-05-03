using System;

namespace Transitions
{
    internal class ManagedType_Float : IManagedType
    {
        public Type getManagedType()
        {
            return typeof(float);
        }

        public object copy(object o)
        {
            return (float) o;
        }

        public object getIntermediateValue(object start, object end, double dPercentage)
        {
            return Utility.interpolate((float) start, (float) end, dPercentage);
        }
    }
}