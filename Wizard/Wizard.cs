using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.Wizard
{
    public partial class Wizard : Form
    {
        MainSwitcher wiz_main = null;

        List<string> history = new List<string>();

        internal static Hashtable config = new Hashtable();

        internal static Wizard instance;

        public Wizard()
        {
            instance = this;

            InitializeComponent();

            Utilities.ThemeManager.ApplyThemeTo(this);

            config.Clear();

            wiz_main = new MainSwitcher(this.panel1);

            wiz_main.AddScreen(new MainSwitcher.Screen("Intro", new _1Intro(), true));
            wiz_main.AddScreen(new MainSwitcher.Screen("FrameFW", new _2FrameFW(), true));
            wiz_main.AddScreen(new MainSwitcher.Screen("Connect", new _3ConnectAP(), true));
            wiz_main.AddScreen(new MainSwitcher.Screen("FrameType", new _4FrameType(), true));
            wiz_main.AddScreen(new MainSwitcher.Screen("AccelCalib", new _5AccelCalib(), true));
            wiz_main.AddScreen(new MainSwitcher.Screen("CompassCalib", new _6CompassCalib(), true));
            wiz_main.AddScreen(new MainSwitcher.Screen("BatteryMonitor", new _7BatteryMonitor(), true));
            wiz_main.AddScreen(new MainSwitcher.Screen("OptionalAC", new _8OptionalItemsAC(), true));
            wiz_main.AddScreen(new MainSwitcher.Screen("OptionalAP", new _8OptionalItemsAP(), true));
            wiz_main.AddScreen(new MainSwitcher.Screen("Radio Calib", new _9RadioCalibration(), true));
            wiz_main.AddScreen(new MainSwitcher.Screen("Flight Modes", new _10FlightModes(), true));
            wiz_main.AddScreen(new MainSwitcher.Screen("Verify", new _11Verify(), true));
            wiz_main.AddScreen(new MainSwitcher.Screen("Failsafe", new _12FailSafe(), true));
            wiz_main.AddScreen(new MainSwitcher.Screen("GeoFence", new _13GeoFence(), true));

            wiz_main.AddScreen(new MainSwitcher.Screen("DontForget", new _98DontForget(), true));

            if (MainV2.comPort.BaseStream.IsOpen)
            {
                if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.FIXED_WING)
                {
                    wiz_main.ShowScreen("AccelCalib");
                }
                else if (MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.HELICOPTER ||
                         MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.HEXAROTOR ||
                         MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.OCTOROTOR ||
                         MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.QUADROTOR ||
                         MainV2.comPort.MAV.aptype == MAVLink.MAV_TYPE.TRICOPTER)
                {
                    wiz_main.ShowScreen("FrameType");
                }
                else
                {
                    wiz_main.ShowScreen("Intro");
                }
            }
            else
            {
                wiz_main.ShowScreen("Intro");
            }

            history.Add(wiz_main.current.Name);

            progressStep1.Maximum = wiz_main.screens.Count + 1;
            progressStep1.Step = 1;
        }

        public void GoNext(int progresspages, bool saveinhistory = true)
        {
            // do the current page busy check
            if (wiz_main.current.Control is IWizard)
            {
                bool busy = ((IWizard) (wiz_main.current.Control)).WizardBusy();
                if (busy)
                {
                    return;
                }
            }

            if (wiz_main.screens.IndexOf(wiz_main.current) == (wiz_main.screens.Count - 1))
            {
                this.Close();
                return;
            }

            // show the next screen
            wiz_main.ShowScreen(wiz_main.screens[wiz_main.screens.IndexOf(wiz_main.current) + progresspages].Name);

            // display index 0 as 1
            progressStep1.Step = wiz_main.screens.IndexOf(wiz_main.current) + 1;

            // add a history line
            if (saveinhistory)
                history.Add(wiz_main.current.Name);

            // enable the back button if we leave the start point
            if (wiz_main.screens.IndexOf(wiz_main.current) >= 1)
            {
                BUT_Back.Enabled = true;
            }

            if (wiz_main.current == wiz_main.screens.Last())
            {
                BUT_Next.Text = "Finish";
            }
            else
            {
                BUT_Next.Text = "Next >>";
            }
        }

        public void GoBack()
        {
            if (history.Count == 1)
            {
                BUT_Back.Enabled = false;
                return;
            }

            // show the last page in history
            wiz_main.ShowScreen(history[history.Count - 2]);

            // remove that entry
            history.RemoveAt(history.Count - 1);

            // display index 0 as 1
            progressStep1.Step = wiz_main.screens.IndexOf(wiz_main.current) + 1;

            // disable the back button if we go back to start
            if (wiz_main.screens.IndexOf(wiz_main.current) == 0)
            {
                BUT_Back.Enabled = false;
            }
        }

        private void BUT_Back_Click(object sender, EventArgs e)
        {
            // do the current page busy check
            if (wiz_main.current.Control is IWizard)
            {
                bool busy = ((IWizard) (wiz_main.current.Control)).WizardBusy();
                if (busy)
                {
                    return;
                }
            }

            GoBack();
        }

        private void BUT_Next_Click(object sender, EventArgs e)
        {
            int progresspages = 1;

            // do the current page validation.
            if (wiz_main.current.Control is IWizard)
            {
                progresspages = ((IWizard) (wiz_main.current.Control)).WizardValidate();
                if (progresspages == 0)
                {
                    return;
                }
            }

            GoNext(progresspages);
        }

        private void Wizard_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                wiz_main.ShowScreen("");
            }
            catch
            {
            }
        }
    }
}