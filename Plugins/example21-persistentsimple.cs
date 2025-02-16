using System;
using System.Windows.Forms;
using System.Drawing;
using MissionPlanner;
using MissionPlanner.Plugin;
using MissionPlanner.Controls;

namespace PersistentSimpleActions
{
    public class PersistentSimpleActions : Plugin
    {
        private string _Name = "Persistent Simple Actions";
        private string _Version = "0.2";
        private string _Author = "Bob Long";

        public override string Name { get { return _Name; } }
        public override string Version { get { return _Version; } }
        public override string Author { get { return _Author; } }

        MyButton button1 = new MyButton();
        MyButton button2 = new MyButton();
        MyButton button3 = new MyButton();
        ToolTip toolTip1 = new ToolTip();

        // CHANGE THIS TO TRUE TO USE THIS PLUGIN
        public override bool Init() { return false; }

        public override bool Loaded()
        {
            // Rename these .Text fields to any valid mode and the code will automatically work
            button1.Text = "Auto";
            button2.Text = "Loiter";
            button3.Text = "RTL";

            //Make a variable for positioning the Controls on the Panel Persistant Panel
            var panelPosition = 3;

            //Varaible for the button one location X
            var buttonOneLocationX = panelPosition + 3;
            //Set the button's width
            var buttonOneWidth = button1.Width;

            // Increase the minimum size of the persistent panel. Not necessary, but adds a little
            // more gap between the buttons and the tabs.
            MainV2.instance.FlightData.panel_persistent.MinimumSize = new System.Drawing.Size(0, 35);

            //
            // button1
            //
            //Location of the button
            button1.Location = new Point((int)(buttonOneLocationX), (int)(3));
            button1.Size = new Size((int)(MainV2.instance.FlightData.panel_persistent.Width * 0.14), (int)(MainV2.instance.FlightData.panel_persistent.Height * 0.6));
            toolTip1.SetToolTip(button1, "Change mode to " + button1.Text);
            button1.Click += new EventHandler(but_mode_Click);
            //
            // button2
            //
            //Variable for location X of Button 2
            var buttonTwoLocationX = button1.Right + 7;
            button2.Location = new Point((int)(buttonTwoLocationX), (int)(3));
            button2.Size = new Size((int)(MainV2.instance.FlightData.panel_persistent.Width * 0.14), (int)(MainV2.instance.FlightData.panel_persistent.Height * 0.6));
            toolTip1.SetToolTip(button2, "Change mode to " + button2.Text);
            button2.Click += new EventHandler(but_mode_Click);
            // 
            // button3
            //
            //Variable for location X of Button 3
            var buttonThreeLocationX = button2.Right + 7;
            button3.Location = new System.Drawing.Point(buttonThreeLocationX, (int)(3));
            button3.Size = new Size((int)(MainV2.instance.FlightData.panel_persistent.Width * 0.14), (int)(MainV2.instance.FlightData.panel_persistent.Height * 0.6));
            toolTip1.SetToolTip(button3, "Change mode to " + button3.Text);
            button3.Click += new EventHandler(but_mode_Click);

            // Add the buttons
            MainV2.instance.FlightData.panel_persistent.Controls.Add(button1);
            MainV2.instance.FlightData.panel_persistent.Controls.Add(button2);
            MainV2.instance.FlightData.panel_persistent.Controls.Add(button3);

            //Resizing Controls on Persistent Panel
            MainV2.instance.FlightData.panel_persistent.Resize += new EventHandler(Controls_Resize);

            return true;
        }

        private void Controls_Resize(object sender, EventArgs e)
        {
            //Make a variable for positioning the Controls on the Panel Persistant Panel
            var panelPosition = 3;

            //Varaible for the button location X
            var buttonOneLocationX = panelPosition + 3;
            //Set the button's width
            var buttonOneWidth = button1.Width;

            // Increase the minimum size of the persistent panel. Not necessary, but adds a little
            // more gap between the buttons and the tabs.
            MainV2.instance.FlightData.panel_persistent.MinimumSize = new System.Drawing.Size(0, 35);

            //
            // button1
            //
            //Location of the button
            button1.Location = new Point((int)(buttonOneLocationX), (int)(3));
            button1.Size = new Size((int)(MainV2.instance.FlightData.panel_persistent.Width * 0.14), (int)(MainV2.instance.FlightData.panel_persistent.Height * 0.6));
            //
            // button2
            //
            //Variable for location X of Button 2
            var buttonTwoLocationX = button1.Right + 7;
            button2.Location = new Point((int)(buttonTwoLocationX), (int)(3));
            button2.Size = new Size((int)(MainV2.instance.FlightData.panel_persistent.Width * 0.14), (int)(MainV2.instance.FlightData.panel_persistent.Height * 0.6));
            // 
            // button3
            //
            //Variable for location X of Button 3
            var buttonThreeLocationX = button2.Right + 7;
            button3.Location = new System.Drawing.Point(buttonThreeLocationX, (int)(3));
            button3.Size = new Size((int)(MainV2.instance.FlightData.panel_persistent.Width * 0.14), (int)(MainV2.instance.FlightData.panel_persistent.Height * 0.6));
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