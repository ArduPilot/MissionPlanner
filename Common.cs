using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.ArduPilot;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;

namespace MissionPlanner
{
    public class Common
    {
        public static GMapMarker getMAVMarker(MAVState MAV)
        {
            PointLatLng portlocation = new PointLatLng(MAV.cs.lat, MAV.cs.lng);

            if (MAV.aptype == MAVLink.MAV_TYPE.FIXED_WING)
            {
                // colorise map marker/s based on their sysid, for common sysid/s used 1-6, 11-16, and 101-106
                // its rare for ArduPilot to be used to fly more than 6 planes at a time from one console.
                int which = 0; // default 0=red for other sysids
                if ((MAV.sysid >= 1) && (MAV.sysid <= 6)) { which = MAV.sysid - 1; }  //1=black, 2=blue, 3=green,4=yellow,5=orange,6=red
                if ((MAV.sysid >= 11) && (MAV.sysid <= 16)) { which = MAV.sysid - 11; }  //1=black, 2=blue, 3=green,4=yellow,5=orange,6=red
                if ((MAV.sysid >= 101) && (MAV.sysid <= 106)) { which = MAV.sysid - 101; }  //1=black, 2=blue, 3=green,4=yellow,5=orange,6=red

                return (new GMapMarkerPlane(which, portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing,
                    MAV.cs.radius * CurrentState.multiplierdist)
                {
                    ToolTipText = MAV.cs.alt.ToString("0") + CurrentState.AltUnit + " | " + (int)MAV.cs.airspeed +
                                  CurrentState.SpeedUnit + " | id:" + (int)MAV.sysid,
                    ToolTipMode = MarkerTooltipMode.Always
                });
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.GROUND_ROVER)
            {
                return (new GMapMarkerRover(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing)
                {
                    ToolTipText = MAV.cs.alt.ToString("0") + "\n" + MAV.sysid.ToString("sysid: 0"),
                    ToolTipMode = MarkerTooltipMode.Always
                });
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.SURFACE_BOAT)
            {
                return (new GMapMarkerBoat(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing));
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.SUBMARINE)
            {
                return (new GMapMarkerSub(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing));
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.HELICOPTER)
            {
                return (new GMapMarkerHeli(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing));
            }
            else if (MAV.cs.firmware == Firmwares.ArduTracker)
            {
                return (new GMapMarkerAntennaTracker(portlocation, MAV.cs.yaw,
                    MAV.cs.target_bearing));
            }
            else if (MAV.cs.firmware == Firmwares.ArduCopter2 || MAV.aptype == MAVLink.MAV_TYPE.QUADROTOR)
            {
                if (MAV.param.ContainsKey("AVD_W_DIST_XY") && MAV.param.ContainsKey("AVD_F_DIST_XY"))
                {
                    var w = MAV.param["AVD_W_DIST_XY"].Value;
                    var f = MAV.param["AVD_F_DIST_XY"].Value;
                    return (new GMapMarkerQuad(portlocation, MAV.cs.yaw,
                        MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.sysid)
                    {
                        danger = (int)f,
                        warn = (int)w
                    });
                }

                return (new GMapMarkerQuad(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.sysid));
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.COAXIAL)
            {
                return (new GMapMarkerSingle(portlocation, MAV.cs.yaw,
                   MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.sysid));
            }
            else
            {
                // unknown type
                return (new GMarkerGoogle(portlocation, GMarkerGoogleType.green_dot));
            }
        }
        public static Form LoadingBox(string title, string promptText)
        {
            Form form = new Form();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(MainV2));
            form.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));

            form.Text = title;
            label.Text = promptText;

            label.SetBounds(9, 50, 372, 13);

            label.AutoSize = true;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            ThemeManager.ApplyThemeTo(form);

            form.Show();
            form.Refresh();
            label.Refresh();
            Application.DoEvents();
            return form;
        }

        public static DialogResult MessageShowAgain(string title, string promptText)
        {
            Form form = new Form();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            CheckBox chk = new CheckBox();
            Controls.MyButton buttonOk = new Controls.MyButton();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(MainV2));
            form.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));

            form.Text = title;
            label.Text = promptText;

            chk.Tag = ("SHOWAGAIN_" + title.Replace(" ", "_").Replace('+', '_'));
            chk.AutoSize = true;
            chk.Text = Strings.ShowMeAgain;
            chk.Checked = true;
            chk.Location = new Point(9, 80);

            if (Settings.Instance.ContainsKey((string)chk.Tag) && Settings.Instance.GetBoolean((string)chk.Tag) == false)
            // skip it
            {
                form.Dispose();
                chk.Dispose();
                buttonOk.Dispose();
                label.Dispose();
                return DialogResult.OK;
            }

            chk.CheckStateChanged += new EventHandler(chk_CheckStateChanged);

            buttonOk.Text = Strings.OK;
            buttonOk.DialogResult = DialogResult.OK;
            buttonOk.Location = new Point(form.Right - 100, 80);

            label.SetBounds(9, 40, 372, 13);

            label.AutoSize = true;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, chk, buttonOk });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            ThemeManager.ApplyThemeTo(form);

            DialogResult dialogResult = form.ShowDialog();

            form.Dispose();

            form = null;

            return dialogResult;
        }

        static void chk_CheckStateChanged(object sender, EventArgs e)
        {
            Settings.Instance[(string)((CheckBox)(sender)).Tag] = ((CheckBox)(sender)).Checked.ToString();
        }

        public static List<KeyValuePair<int, string>> getModesList(Firmwares csFirmware)
        {
            return ArduPilot.Common.getModesList(csFirmware);
        }
    }
}
