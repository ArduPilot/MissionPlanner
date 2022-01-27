using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using MissionPlanner.Controls;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using MissionPlanner;
using System.Drawing;
using System.Threading.Tasks;

namespace Shortcuts
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {

        ToolStripMenuItem but;
        MissionPlanner.Controls.MyDataGridView commands;

        public override string Name
        {
            get { return "Graphique"; }
        }

        public override string Version
        {
            get { return "0.10"; }
        }

        public override string Author
        {
            get { return "Ali Hassoun"; }
        }

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()
        {
            but = new ToolStripMenuItem("Fix mission top/bottom");
            but.Click += but_Click;
            ToolStripItemCollection col = Host.FPMenuMap.Items;
            col.Add(but);
            commands =
                Host.MainForm.FlightPlanner.Controls.Find("Commands", true).FirstOrDefault() as
                    MissionPlanner.Controls.MyDataGridView;

            Console.WriteLine("Test : Loaded .cs file");
            return true;
        }

        public override bool Loop()
        {
            Console.WriteLine("Test : Loop .cs file");
            return true;
        }


        void but_Click(object sender, EventArgs e)
        {
            CustomMessageBox.Show("This is a sample plugin\nSee the source in the plugins folder");

            string angle = "0";
            InputBox.Show("Enter Angle", "This will be the heading", ref angle);
            int angle_in_number = Int32.Parse(angle);

            Host.InsertWP(0, MAVLink.MAV_CMD.DO_SET_SERVO, 9, angle_in_number, 0, 0, 0, 0, 0);
            Host.InsertWP(1, MAVLink.MAV_CMD.DO_SET_SERVO, 10, 1000, 0, 0, 0, 0, 0);

            Host.AddWPtoList(MAVLink.MAV_CMD.DO_SET_SERVO, 9, 1000, 0, 0, 0, 0, 0);
            Host.AddWPtoList(MAVLink.MAV_CMD.DO_SET_SERVO, 10, 1000, 0, 0, 0, 0, 0);

            commands.Rows.RemoveAt(1);
        }

        public override bool Exit()
        {
            return true;
        }

    }
}
