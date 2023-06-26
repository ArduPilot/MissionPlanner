using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace MissionPlanner
{
    public static class Common
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static GMapMarker getMAVMarker(MAVState MAV, GMapOverlay overlay = null)
        {
            PointLatLng portlocation = MAV.cs.Location;

            if(overlay!= null)
            {
                var existing = overlay.Markers.Where((a)=>a.Tag == MAV).ToArray();                
                if (existing.Count() > 1)
                {
                    existing.Skip(1).ToArray().ForEach((a) => { overlay.Markers.Remove(a);});
                }
                if(existing.Count() > 0) {
                    var item = existing.First();
                    if (item is GMapMarkerPlane)
                    {
                        var itemp = (GMapMarkerPlane)item;
                        itemp.Position = portlocation;
                        itemp.Heading = MAV.cs.yaw;
                        itemp.Cog = MAV.cs.groundcourse;
                        itemp.Target = MAV.cs.target_bearing;
                        itemp.Nav_bearing = MAV.cs.nav_bearing;
                        itemp.Radius = MAV.cs.radius * CurrentState.multiplierdist;
                        return null;
                    }
                    else if (item is GMapMarkerQuad)
                    {
                        var itemq = (GMapMarkerQuad)item;
                        itemq.Position = portlocation;
                        itemq.Heading = MAV.cs.yaw;
                        itemq.Cog = MAV.cs.groundcourse;
                        itemq.Target = MAV.cs.nav_bearing;
                        itemq.Sysid = MAV.sysid;
                        return null;
                    }
                    else if (item is GMapMarkerRover)
                    {
                        var itemr = (GMapMarkerRover)item;
                        itemr.Position = portlocation;
                        itemr.Heading = MAV.cs.yaw;
                        itemr.Cog = MAV.cs.groundcourse;
                        itemr.Target = MAV.cs.target_bearing;
                        itemr.Nav_bearing = MAV.cs.nav_bearing;
                        return null;
                    }
                    else
                    {
                        existing.ForEach((a)=> overlay.Markers.Remove(a));
                    }
                }
            }

            if (MAV.aptype == MAVLink.MAV_TYPE.FIXED_WING || MAV.aptype >= MAVLink.MAV_TYPE.VTOL_DUOROTOR && MAV.aptype <= MAVLink.MAV_TYPE.VTOL_RESERVED5)
            {
                return (new GMapMarkerPlane(MAV.sysid - 1, portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing,
                    MAV.cs.radius * CurrentState.multiplierdist)
                {
                    ToolTipText = ArduPilot.Common.speechConversion(MAV, "" + Settings.Instance["mapicondesc"]),
                    ToolTipMode = String.IsNullOrEmpty(Settings.Instance["mapicondesc"]) ? MarkerTooltipMode.Never : MarkerTooltipMode.Always,
                    Tag = MAV
                });
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.GROUND_ROVER)
            {
                return (new GMapMarkerRover(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing)
                {
                    ToolTipText = MAV.cs.alt.ToString("0") + "\n" + MAV.sysid.ToString("sysid: 0"),
                    ToolTipMode = MarkerTooltipMode.Always,
                    Tag = MAV
                });
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.SURFACE_BOAT)
            {
                return (new GMapMarkerBoat(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing){  Tag = MAV});
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.SUBMARINE)
            {
                return (new GMapMarkerSub(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.cs.target_bearing){ Tag = MAV});
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.HELICOPTER)
            {
                return (new GMapMarkerHeli(portlocation, MAV.cs.yaw,
                    MAV.cs.groundcourse, MAV.cs.nav_bearing){ Tag = MAV});
            }
            else if (MAV.cs.firmware == Firmwares.ArduTracker)
            {
                return (new GMapMarkerAntennaTracker(portlocation, MAV.cs.yaw,
                    MAV.cs.target_bearing){ Tag = MAV});
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.COAXIAL)
            {
                return (new GMapMarkerSingle(portlocation, MAV.cs.yaw,
                   MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.sysid)
                { Tag = MAV });
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
                        warn = (int)w,
                        Tag = MAV
                    });
                }

                return (new GMapMarkerQuad(portlocation, MAV.cs.yaw,
                        MAV.cs.groundcourse, MAV.cs.nav_bearing, MAV.sysid)
                {
                    ToolTipText = ArduPilot.Common.speechConversion(MAV, "" + Settings.Instance["mapicondesc"]),
                    ToolTipMode = String.IsNullOrEmpty(Settings.Instance["mapicondesc"]) ? MarkerTooltipMode.Never : MarkerTooltipMode.Always,
                    Tag = MAV
                });
            }
            else
            {
                // unknown type
                return (new GMarkerGoogle(portlocation, GMarkerGoogleType.green_dot) { Tag = MAV });
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

        public static DialogResult MessageShowAgain(string title, string promptText, bool show_cancel = false)
        {
            Form form = new Form();
            System.Windows.Forms.Label label = new System.Windows.Forms.Label();
            CheckBox chk = new CheckBox();
            Controls.MyButton buttonOk = new Controls.MyButton();
            Controls.MyButton buttonCancel = new Controls.MyButton();
            System.ComponentModel.ComponentResourceManager resources =
                new System.ComponentModel.ComponentResourceManager(typeof(MainV2));
            try
            {
                form.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            } catch {}

            string link = "";
            string linktext = "";


            Regex linkregex = new Regex(@"(\[link;([^\]]+);([^\]]+)\])", RegexOptions.IgnoreCase);
            Match match = linkregex.Match(promptText);
            if (match.Success)
            {
                link = match.Groups[2].Value;
                linktext = match.Groups[3].Value;
                promptText = promptText.Replace(match.Groups[1].Value, "");
            }

            form.Text = title;
            label.Text = promptText;

            chk.Tag = ($"SHOWAGAIN_{title.Replace(" ", "_").Replace('+', '_').Replace('-', '_').Replace('.', '_')}");
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
                buttonCancel.Dispose();
                label.Dispose();
                return DialogResult.OK;
            }

            chk.CheckStateChanged += new EventHandler(chk_CheckStateChanged);

            buttonOk.Text = Strings.OK;
            buttonOk.DialogResult = DialogResult.OK;
            buttonOk.Location = new Point(form.Right - (show_cancel ? 180 : 100), 80);

            buttonCancel.Text = Strings.Cancel;
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(form.Right - 90, 80);
            buttonCancel.Visible = show_cancel;

            label.SetBounds(9, 9, 372, 13);

            label.AutoSize = true;

            form.Controls.AddRange(new Control[] { label, chk, buttonOk, buttonCancel });

            if (link != "" && linktext != "")
            {
                Size textSize2 = TextRenderer.MeasureText(linktext, SystemFonts.DefaultFont);
                var linklbl = new LinkLabel
                {
                    Left = 9,
                    Top = label.Bottom,
                    Width = textSize2.Width,
                    Height = textSize2.Height,
                    Text = linktext,
                    Tag = link,
                    AutoSize = true
                };
                linklbl.Click += (sender, args) =>
                {
                    try
                    {
                        System.Diagnostics.Process.Start(((LinkLabel)sender).Tag.ToString());
                    }
                    catch (Exception)
                    {
                        CustomMessageBox.Show($"Failed to open link {((LinkLabel)sender).Tag}");
                    }
                };

                form.Controls.Add(linklbl);

                form.Width = Math.Max(form.Width, linklbl.Right + 16);
            }

            form.ClientSize = new Size(396, 107);

            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;

            ThemeManager.ApplyThemeTo(form);

            DialogResult dialogResult = DialogResult.Cancel;
            if (Application.OpenForms.Count > 0)
            {
                if (Application.OpenForms[0].InvokeRequired)
                    Application.OpenForms[0].Invoke(new Action(() => { dialogResult = form.ShowDialog(); }));
                else
                    dialogResult = form.ShowDialog();
            }
            else
            {
                dialogResult = form.ShowDialog();
            }

            form.Dispose();

            form = null;

            return dialogResult;
        }

        static void chk_CheckStateChanged(object sender, EventArgs e)
        {
            Settings.Instance[(string)((CheckBox)(sender)).Tag] = ((CheckBox)(sender)).Checked.ToString();
        }

        /// <summary>
        /// https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
        /// </summary>
        /// <param name="url"></param>
        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
