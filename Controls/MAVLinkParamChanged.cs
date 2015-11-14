using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Controls
{
    public class MAVLinkParamChanged : EventArgs
    {
        public string name;
        public float value;

        public MAVLinkParamChanged(string Name, float Value)
        {
            this.name = Name;
            this.value = Value;
        }
    }
}