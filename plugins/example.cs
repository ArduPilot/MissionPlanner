using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using MissionPlanner;

namespace test
{
    public class urlmod : MissionPlanner.Plugin.Plugin
    {
        public override string Name { get; }
        public override string Version { get; }
        public override string Author { get; }

        public override bool Init()
        {
            srtm.baseurl = "https://firmware.oborne.me/SRTM/";
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