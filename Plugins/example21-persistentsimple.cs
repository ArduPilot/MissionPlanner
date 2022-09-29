using System;
using System.Windows.Forms;
using MissionPlanner;
using MissionPlanner.Plugin;
using MissionPlanner.Controls;

namespace PersistentSimpleActions
{
    public class PersistentSimpleActions : Plugin
    {
        private string _Name = "Persistent Simple Actions";
        private string _Version = "0.1";
        private string _Author = "Bob Long";

        public override string Name { get { return _Name; } }
        public override string Version { get { return _Version; } }
        public override string Author { get { return _Author; } }

        // CHANGE THIS TO TRUE TO USE THIS PLUGIN
        public override bool Init() { return false; }

        public override bool Loaded() 
        {
            MyButton button1 = new MyButton();
            MyButton button2 = new MyButton();
            MyButton button3 = new MyButton();
            ToolTip toolTip1 = new ToolTip();

            // Rename these .Text fields to any valid mode and the code will automatically work
            button1.Text = "Auto";
            button2.Text = "Loiter";
            button3.Text = "RTL";

            // 
            // button1
            // 
            button1.Location = new System.Drawing.Point(4, 4);
            button1.Size = new System.Drawing.Size(75, 23);
            toolTip1.SetToolTip(button1, "Change mode to " + button1.Text);
            button1.Click += new EventHandler(but_mode_Click);
            // 
            // button2
            // 
            button2.Location = new System.Drawing.Point(85, 4);
            button2.Size = new System.Drawing.Size(75, 23);
            toolTip1.SetToolTip(button2, "Change mode to " + button2.Text);
            button2.Click += new EventHandler(but_mode_Click);
            // 
            // button3
            // 
            button3.Location = new System.Drawing.Point(166, 4);
            button3.Size = new System.Drawing.Size(75, 23);
            toolTip1.SetToolTip(button3, "Change mode to " + button3.Text);
            button3.Click += new EventHandler(but_mode_Click);

            // Increase the minimum size of the persistent panel. Not necessary, but adds a little
            // more gap between the buttons and the tabs.
            MainV2.instance.FlightData.panel_persistent.MinimumSize = new System.Drawing.Size(0, 35);

            // Add the buttons
            MainV2.instance.FlightData.panel_persistent.Controls.Add(button1);
            MainV2.instance.FlightData.panel_persistent.Controls.Add(button2);
            MainV2.instance.FlightData.panel_persistent.Controls.Add(button3); 
            
            return true;
        }

        public override bool Exit() { return true; }

        private void but_mode_Click(object sender, EventArgs e)
        {
            try
            {
                ((Control)sender).Enabled = false;
                MainV2.comPort.setMode(((Control)sender).Text);
            }
            catch
            {
                CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
            }

            ((Control)sender).Enabled = true;
        }

    }
}