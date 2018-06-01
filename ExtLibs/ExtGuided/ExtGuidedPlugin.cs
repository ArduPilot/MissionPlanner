using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Plugin;
using MissionPlanner;
using System.Windows.Forms;
using System.IO;
using MissionPlanner.Utilities;

    public class ExtGuidedPlugin : Plugin
    {
        string _Name = "External Guided"; 
        string _Version = "0.1";
        string _Author = "Michael Oborne";

        public override string Name { get { return _Name; } }
        public override string Version { get { return _Version; } }
        public override string Author { get { return _Author; } }

        public static string file = "";

        public override bool Init() { loopratehz = 1f; return true; }

        public override bool Loaded() 
        {
            ToolStripLabel item = new ToolStripLabel("External Guided");
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
                ExtGuided.FilePick form = new ExtGuided.FilePick();
                form.Show();
            }

            return true; 
        }

        public override bool Loop()
        {
            if (File.Exists(file))
            {
                try
                {
                    string location = File.ReadAllText(file);

                    string[] loc = location.Split(',');

                    Host.comPort.setGuidedModeWP(
                        new Locationwp()
                        {
                            alt = float.Parse(loc[2]),
                            lat = double.Parse(loc[0]),
                            lng = double.Parse(loc[1]),
                        }
                    );

                    //File.Delete(file);
                }
                catch (Exception ex) 
                { 
                    Console.WriteLine(ex.ToString()); 
                }
            }

            return true; 
        }


        public override bool Exit() { return true; }
    }

