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
                        itemp.Radius = (float)CurrentState.fromDistDisplayUnit(MAV.cs.radius);
                        itemp.IsActive = MAV == MainV2.comPort?.MAV;
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
                        itemq.IsActive = MAV == MainV2.comPort?.MAV;
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
                        itemr.IsActive = MAV == MainV2.comPort?.MAV;
                        return null;
                    }
                    else
                    {
                        existing.ForEach((a)=> overlay.Markers.Remove(a));
                    }
                }
            }
            if (MAV.aptype == MAVLink.MAV_TYPE.FIXED_WING ||
                MAV.aptype >= MAVLink.MAV_TYPE.VTOL_DUOROTOR && MAV.aptype <= MAVLink.MAV_TYPE.VTOL_RESERVED5)
            {
                return new GMapMarkerPlane(
                    MAV.sysid - 1,
                    portlocation,
                    MAV.cs.yaw,
                    MAV.cs.groundcourse,
                    MAV.cs.nav_bearing,
                    MAV.cs.target_bearing,
                    (float)CurrentState.fromDistDisplayUnit(MAV.cs.radius))
                {
                    IsActive = MAV == MainV2.comPort?.MAV,
                    ToolTipText = ArduPilot.Common.speechConversion(MAV, "" + Settings.Instance["mapicondesc"]),
                    ToolTipMode = String.IsNullOrEmpty(Settings.Instance["mapicondesc"]) ?
                                  MarkerTooltipMode.Never :
                                  MarkerTooltipMode.Always,
                    Tag = MAV
                };
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.GROUND_ROVER)
            {
                return new GMapMarkerRover(
                    portlocation,
                    MAV.cs.yaw,
                    MAV.cs.groundcourse,
                    MAV.cs.nav_bearing,
                    MAV.cs.target_bearing)
                {
                    IsActive = MAV == MainV2.comPort?.MAV,
                    ToolTipText = ArduPilot.Common.speechConversion(MAV, "" + Settings.Instance["mapicondesc"]),
                    ToolTipMode = String.IsNullOrEmpty(Settings.Instance["mapicondesc"]) ?
                                  MarkerTooltipMode.Never :
                                  MarkerTooltipMode.Always,
                    Tag = MAV
                };
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.SURFACE_BOAT)
            {
                return new GMapMarkerBoat(
                    portlocation,
                    MAV.cs.yaw,
                    MAV.cs.groundcourse,
                    MAV.cs.nav_bearing,
                    MAV.cs.target_bearing)
                {
                    IsActive = MAV == MainV2.comPort?.MAV,
                    ToolTipText = ArduPilot.Common.speechConversion(MAV, "" + Settings.Instance["mapicondesc"]),
                    ToolTipMode = String.IsNullOrEmpty(Settings.Instance["mapicondesc"]) ?
                                  MarkerTooltipMode.Never :
                                  MarkerTooltipMode.Always,
                    Tag = MAV
                };
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.SUBMARINE)
            {
                return new GMapMarkerSub(
                    portlocation,
                    MAV.cs.yaw,
                    MAV.cs.groundcourse,
                    MAV.cs.nav_bearing,
                    MAV.cs.target_bearing)
                {
                    IsActive = MAV == MainV2.comPort?.MAV,
                    ToolTipText = ArduPilot.Common.speechConversion(MAV, "" + Settings.Instance["mapicondesc"]),
                    ToolTipMode = String.IsNullOrEmpty(Settings.Instance["mapicondesc"]) ?
                                  MarkerTooltipMode.Never :
                                  MarkerTooltipMode.Always,
                    Tag = MAV
                };
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.HELICOPTER)
            {
                return new GMapMarkerHeli(
                    portlocation,
                    MAV.cs.yaw,
                    MAV.cs.groundcourse,
                    MAV.cs.nav_bearing)
                {
                    IsActive = MAV == MainV2.comPort?.MAV,
                    ToolTipText = ArduPilot.Common.speechConversion(MAV, "" + Settings.Instance["mapicondesc"]),
                    ToolTipMode = String.IsNullOrEmpty(Settings.Instance["mapicondesc"]) ?
                                  MarkerTooltipMode.Never :
                                  MarkerTooltipMode.Always,
                    Tag = MAV
                };
            }
            else if (MAV.cs.firmware == Firmwares.ArduTracker)
            {
                return new GMapMarkerAntennaTracker(portlocation, MAV.cs.yaw, MAV.cs.target_bearing)
                {
                    Tag = MAV
                };
            }
            else if (MAV.aptype == MAVLink.MAV_TYPE.COAXIAL)
            {
                return new GMapMarkerSingle(
                    portlocation,
                    MAV.cs.yaw,
                    MAV.cs.groundcourse,
                    MAV.cs.nav_bearing,
                    MAV.sysid)
                {
                    IsActive = MAV == MainV2.comPort?.MAV,
                    Tag = MAV
                };
            }
            else if (MAV.cs.firmware == Firmwares.ArduCopter2 || MAV.aptype == MAVLink.MAV_TYPE.QUADROTOR)
            {
                if (MAV.param.ContainsKey("AVD_W_DIST_XY") && MAV.param.ContainsKey("AVD_F_DIST_XY"))
                {
                    var w = MAV.param["AVD_W_DIST_XY"].Value;
                    var f = MAV.param["AVD_F_DIST_XY"].Value;
                    return new GMapMarkerQuad(
                        portlocation,
                        MAV.cs.yaw,
                        MAV.cs.groundcourse,
                        MAV.cs.nav_bearing,
                        MAV.sysid)
                    {
                        IsActive = MAV == MainV2.comPort?.MAV,
                        danger = (int)f,
                        warn = (int)w,
                        Tag = MAV
                    };
                }

                return new GMapMarkerQuad(
                    portlocation,
                    MAV.cs.yaw,
                    MAV.cs.groundcourse,
                    MAV.cs.nav_bearing,
                    MAV.sysid)
                {
                    IsActive = MAV == MainV2.comPort?.MAV,
                    ToolTipText = ArduPilot.Common.speechConversion(MAV, "" + Settings.Instance["mapicondesc"]),
                    ToolTipMode = String.IsNullOrEmpty(Settings.Instance["mapicondesc"]) ?
                                  MarkerTooltipMode.Never :
                                  MarkerTooltipMode.Always,
                    Tag = MAV
                };
            }
            else
            {
                // unknown type
                return new GMarkerGoogle(portlocation, GMarkerGoogleType.green_dot)
                {
                    Tag = MAV
                };
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

        public static DialogResult MessageShowAgain(string title, string promptText, bool show_cancel = false, string tag = "")
        {
            // `tag` is the unique ID for this prompt, used to remember "show me again" state.
            // If not provided, we use the title as the tag.
            if (string.IsNullOrEmpty(tag))
                tag = title;

            // Early return if user has disabled this prompt
            string showAgainKey = $"SHOWAGAIN_{tag.Replace(" ", "_").Replace('+', '_').Replace('-', '_').Replace('.', '_')}";
            if (Settings.Instance.ContainsKey(showAgainKey) && Settings.Instance.GetBoolean(showAgainKey) == false)
                return DialogResult.OK;

            // Check if this was called on a non-UI thread, and marshal to the main UI thread if so
            DialogResult dialogResult = DialogResult.Cancel;
            Func<DialogResult> showFunc = () =>
            {
                using (var form = CreateMessageShowAgainForm(title, promptText, show_cancel, showAgainKey))
                {
                    return form.ShowDialog();
                }
            };
            if (MainV2.instance != null && !MainV2.instance.IsDisposed && MainV2.instance.InvokeRequired)
            {
                MainV2.instance.Invoke(new Action(() => dialogResult = showFunc()));
            }
            else if (Application.OpenForms.Count > 0 && Application.OpenForms[0].InvokeRequired)
            {
                Application.OpenForms[0].Invoke(new Action(() => dialogResult = showFunc()) );
            }
            else
            {
                dialogResult = showFunc();
            }
            return dialogResult;
        }

        private static Form CreateMessageShowAgainForm(string title, string promptText, bool show_cancel, string showAgainKey)
        {
            var form = new Form()
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                Padding = new Padding(0),
                StartPosition = FormStartPosition.CenterParent,
                Text = title,
            };
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(MainV2));
            try { form.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon"); } catch { /* ignore */ }

            // Layout constants
            int margin = 16;              // outer padding
            int rowspacing = 8;           // space below each row
            int buttonspacing = 6;        // space between buttons
            int minContentWidth = 300;    // minimum dialog width
            int maxContentWidth = 600;    // wrap width for long text

            // Root table: 1 column, 3 rows (content, optional link, footer)
            var table = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                Dock = DockStyle.Fill,
                Padding = new Padding(margin),
                RowCount = 3,
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));  // message
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));  // link (optional)
            table.RowStyles.Add(new RowStyle(SizeType.AutoSize));  // footer

            form.Controls.Add(table);

            // Main message label
            var label = new Label
            {
                AutoSize = true,
                Margin = new Padding(0, 0, 0, rowspacing),
                MaximumSize = new Size(maxContentWidth, 0),
                MinimumSize = new Size(minContentWidth, 0),
                Text = promptText,
                TextAlign = ContentAlignment.TopLeft,
            };
            table.Controls.Add(label, 0, 0);

            // Optional inline link parsing: [link;https://...;Link Text]
            string link = "";
            string linktext = "";
            var linkregex = new Regex(@"(\[link;([^\]]+);([^\]]+)\])", RegexOptions.IgnoreCase);
            var match = linkregex.Match(promptText);
            if (match.Success)
            {
                link = match.Groups[2].Value;
                linktext = match.Groups[3].Value;

                // Show label text without the token, link sits on its own line beneath.
                label.Text = promptText.Replace(match.Groups[1].Value, "");

                var linklbl = new LinkLabel
                {
                    AutoSize = true,
                    Margin = new Padding(0, 0, 0, rowspacing),
                    Text = linktext,
                };
                linklbl.LinkClicked += (sender, args) =>
                {
                    OpenUrl(link);
                };

                table.Controls.Add(linklbl, 0, 1);
            }

            // Footer: checkbox left, buttons right
            var footer = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 2,
                Dock = DockStyle.Fill,
                Margin = new Padding(0),
                RowCount = 1,
            };
            footer.ColumnCount = 2;
            footer.ColumnStyles.Clear();
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            footer.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            // "Show me again" checkbox
            var chk = new CheckBox
            {
                Anchor = AnchorStyles.Left,
                AutoSize = true,
                Checked = true,
                Margin = new Padding(0),
                Tag = showAgainKey,
                Text = Strings.ShowMeAgain,
            };
            chk.CheckStateChanged += chk_CheckStateChanged;
            footer.Controls.Add(chk, 0, 0);

            // Buttons, right-aligned via FlowLayout RightToLeft
            var buttonPanel = new FlowLayoutPanel
            {
                Anchor = AnchorStyles.Right,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                Margin = new Padding(0),
                WrapContents = false,
            };

            var buttonOk = new Controls.MyButton
            {
                DialogResult = DialogResult.OK,
                Margin = new Padding(buttonspacing, 0, 0, 0),
                Text = Strings.OK,
            };
            var buttonCancel = new Controls.MyButton
            {
                DialogResult = DialogResult.Cancel,
                Margin = new Padding(buttonspacing, 0, 0, 0),
                Text = Strings.Cancel,
                Visible = show_cancel,
            };

            // Add in order that keeps Cancel at right edge when visible
            if (show_cancel)
                buttonPanel.Controls.Add(buttonCancel);
            buttonPanel.Controls.Add(buttonOk);

            footer.Controls.Add(buttonPanel, 2, 0);
            table.Controls.Add(footer, 0, 2);

            // Dialog keyboard behavior
            form.AcceptButton = buttonOk;
            form.CancelButton = show_cancel ? buttonCancel : null;

            ThemeManager.ApplyThemeTo(form);

            return form;
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
