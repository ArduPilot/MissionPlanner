using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArdupilotMega.Plugin;
using ArdupilotMega;
using System.Windows.Forms;

    public class PluginTestExample : Plugin
    {
        string _Name = "Plugin Test"; 
        string _Version = "0.1";
        string _Author = "Michael Oborne";

        public override string Name { get { return _Name; } }
        public override string Version { get { return _Version; } }
        public override string Author { get { return _Author; } }


        public override bool Init() { loopratehz = 0.1f; return true; }

        public override bool Loaded() 
        {
            ToolStripLabel item = new ToolStripLabel("Test Plugin");
            item.Click += item_Click;

            Host.FDMenuMap.Items.Add(item);

            return true; 
        }

        void item_Click(object sender, EventArgs e)
        {
            SetupUI(0);
        }

        public bool SetupUI(int gui = 0) 
        {
            if (gui == 0)
            {
                PluginTest.TestForm form = new PluginTest.TestForm();
                form.Show();
            }

            return true; 
        }

        public override bool Loop()
        { 
            Console.WriteLine("Plugin Loop - {0}", NextRun);

            Console.WriteLine("Currrent Pos {0} {1} {2}", Host.cs.lat, Host.cs.lng, Host.cs.altasl);

            return true; 
        }


        public override bool Exit() { return true; }
    }

