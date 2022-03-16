using System;

namespace Transitions
{
    internal interface IManagedType
    {
        Type getManagedType();
        object copy(object o);
        object getIntermediateValue(object start, object end, double dPercentage);
    }
}