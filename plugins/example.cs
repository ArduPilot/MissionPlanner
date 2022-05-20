using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using MissionPlanner;
//loadassembly: MissionPlanner.WebAPIs

namespace test
{
    public class urlmod : MissionPlanner.Plugin.Plugin
    {
        public override string Name { get; }
        public override string Version { get; }
        public override string Author { get; }

        public override bool Init()
        {
            return false;         
        }

        public override bool Loaded()
        {
            return false;
        }

        public override bool Exit()
        {
            return false;
        }
    }
}