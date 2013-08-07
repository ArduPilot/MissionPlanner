using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArdupilotMega.Plugin;
using ArdupilotMega;
using System.Windows.Forms;

    public class PluginTestExample : IPlugin
    {
        public PluginHost Host { get; set; }

        string _Name = "Plugin Test"; 
        string _Version = "0.1";
        string _Author = "Michael Oborne";

        public string Name { get { return _Name; } }
        public string Version { get { return _Version; } }
        public string Author { get { return _Author; } }

        public DateTime NextRun { get; set; }

        public bool Init() { loopratehz = 1; return true; }

        public bool Loaded() 
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

        public bool Loop() { 
            Console.WriteLine("Plugin Loop - {0}", NextRun);

            Console.WriteLine("Currrent Pos {0} {1} {2}", Host.cs.lat, Host.cs.lng, Host.cs.altasl);

            return true; 
        }

        public int loopratehz { get; set; }

        public bool Exit() { return true; }
    }

