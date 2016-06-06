using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    public interface ICorrections
    {
        int length { get; }
        byte[] packet { get; }
    }
}
