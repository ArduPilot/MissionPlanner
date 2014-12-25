using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner
{
    public class Plugingstream : MissionPlanner.Plugin.Plugin
    {
        public override string Name
        {
            get { return "gstream"; }
        }

        public override string Version
        {
            get { return "0.1"; }
        }

        public override string Author
        {
            get { return "Michael Oborne"; }
        }

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()
        {
            GStreamerHud.MainForm form = new GStreamerHud.MainForm();

            form.Show();

            return true;
        }

        public override bool Exit()
        {
            return true;
        }
    }
}
