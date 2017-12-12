using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities
{
    public interface ICorrections
    {
        /// <summary>
        /// overall packet length (use for sending forward)
        /// </summary>
        int length { get; }
        /// <summary>
        /// raw packet data 
        /// </summary>
        byte[] packet { get; }
        /// <summary>
        /// reset the parser to the initial state
        /// </summary>
        /// <returns></returns>
        bool resetParser();
    }
}
