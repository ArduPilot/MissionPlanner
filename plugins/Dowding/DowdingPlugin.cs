using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Dowding.Model;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner;
using MissionPlanner.Controls;
using MissionPlanner.GCSViews;
using MissionPlanner.Maps;
using MissionPlanner.Plugin;
using MissionPlanner.Utilities;
using MissionPlanner.WebAPIs;

namespace Dowding
{
    public class DowdingPlugin : Plugin
    {
        string _Name = "Dowding";
        string _Version = "0.1";
        string _Author = "Michael Oborne";

        public override string Name
        {
            get { return _Name; }
        }

        public override string Version
        {
            get { return _Version; }
        }

        public override string Author
        {
            get { return _Author; }
        }

        public static bool IsAlive
        {
            get { return DowdingPlugin.ws != null && DowdingPlugin.ws.State == WebSocket4Net.WebSocketState.Open; }
        }

        public override bool Exit()
        {
            return true;
        }

        public override bool Init()
        {
            if (Settings.Instance.ContainsKey("Dowding_enabled"))
            {
                if (Settings.Instance.GetBoolean("Dowding_enabled"))
                {
                    Start();
                    loopratehz = 1;
                }
            }

            loopratehz = 1;
            
            return true;
        }

        public static async Task Start()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var dowd = new MissionPlanner.WebAPIs.Dowding();
                    if (Settings.Instance.ContainsKey("Dowding_username") &&
                        Settings.Instance.ContainsKey("Dowding_password") &&
                        Settings.Instance.ContainsKey("Dowding_server"))
                    {
                        await dowd.Auth(Settings.Instance["Dowding_username"],
                            new Crypto().DecryptString(Settings.Instance["Dowding_password"]),
                            Settings.Instance["Dowding_server"]);
                    }
                    else if (Settings.Instance.ContainsKey("Dowding_token") &&
                             Settings.Instance.ContainsKey("Dowding_server"))
                    {
                        dowd.SetToken(Settings.Instance["Dowding_token"], Settings.Instance["Dowding_server"]);
                    }
                    else
                    {
                        CustomMessageBox.Show("Dowding invalid settings");
                    }

                    ws = await dowd.Start(Settings.Instance["Dowding_server"]);
                }
                catch
                {
                    CustomMessageBox.Show("Failed to start Dowding");
                }
            });
        }

        public override bool Loaded()
        {
            MainV2.instance.Invoke((Action)
                delegate
                {
                    System.Windows.Forms.ToolStripMenuItem men = new System.Windows.Forms.ToolStripMenuItem() { Text = "Dowding" };
                    men.Click += men_Click;
                    Host.FDMenuMap.Items.Add(men);

                    System.Windows.Forms.ToolStripMenuItem men2 = new System.Windows.Forms.ToolStripMenuItem() { Text = "Dowding Point At" };
                    men2.Click += men2_Click;
                    Host.FDMenuMap.Items.Add(men2);
                });

            return true;
        }

        private void men2_Click(object sender, EventArgs e)
        {
            double alt = 0;
            InputBox.Show("Altitude", "Enter HAE altitude", ref alt);
            UpdateOutput?.Invoke(this, this.Host.FDMenuMapPosition.ToPLLA(alt));
        }

        public override bool Loop()
        {
            GMapOverlay overlay;

            if (Host.FDGMapControl.Overlays.Any(a=>a.Id == "dowding"))
            {
                overlay = Host.FDGMapControl.Overlays.First(a => a.Id == "dowding");
            } 
            else
            {
                overlay = new GMap.NET.WindowsForms.GMapOverlay("dowding");
                Host.FDGMapControl.Overlays.Add(overlay);
                Host.FDGMapControl.OnMarkerClick += (item, args) =>
                {
                    if (item.Overlay == overlay && item is GMapMarker && item.Tag is VehicleTick)
                    {
                        // unselect
                        if (target == item)
                        {
                            target.ToolTipMode = MarkerTooltipMode.Never;
                            target.ToolTipText = "";
                            target = null;
                            return;
                        }

                        //clear old
                        if (target != null)
                        {
                            target.ToolTipMode = MarkerTooltipMode.Never;
                            target.ToolTipText = "";
                        }
                        target = item;
                        target.ToolTipMode = MarkerTooltipMode.Always;
                        var vt = (VehicleTick) item.Tag;
                        target.ToolTipText = "Tracking\r\nVendor: " + vt.Vendor + "\r\nModel: " + vt.Model + "\r\nSerial: " + vt.Serial;
                        UpdateOutput?.Invoke(this,
                            new PointLatLngAlt((double)vt.Lat, (double)vt.Lon, (double)vt.Hae));
                    }
                    else if (item is GMapMarker && item.Tag is MAVState)
                    {
                        // unselect
                        if (target == item)
                        {
                            target.ToolTipMode = MarkerTooltipMode.Never;
                            target.ToolTipText = "";
                            target = null;
                            timer?.Dispose();
                            timer = null;
                            return;
                        }

                        //clear old
                        if (target != null)
                        {
                            target.ToolTipMode = MarkerTooltipMode.Never;
                            target.ToolTipText = "";
                            timer?.Dispose();
                            timer = null;
                        }

                        target = item;
                        timer = new Timer(a => {
                            if (target is GMapMarker && target.Tag is MAVState)
                                UpdateOutput?.Invoke(this, ((MAVState)item.Tag).cs.Location);
                        }, this, 1, 2000);
                    }
                };
            }

            FlightData.instance.updateMarkersAsNeeded<VehicleTick, GMarkerGoogle>(MissionPlanner.WebAPIs.Dowding.Vehicles.Values,
                overlay, tick =>
                {
                    return tick.Serial ?? tick.Id;
                },
                mapMarker =>
                {
                    return ((VehicleTick) mapMarker.Tag).Serial ?? ((VehicleTick) mapMarker.Tag).Id;
                },
                tick =>
                {
                    return new GMarkerGoogle(new PointLatLng((double)tick.Lat, (double)tick.Lon), GMarkerGoogleType.blue_dot) {Tag = tick};
                },
                (tick, mapMarker) =>
                {
                    mapMarker.Position = new PointLatLng((double) tick.Lat, (double) tick.Lon);
                    mapMarker.Tag = tick;

                    if (mapMarker == target)
                    {
                        UpdateOutput?.Invoke(this,
                            new PointLatLngAlt((double) tick.Lat, (double) tick.Lon, (double) tick.Hae));
                    }

                    // hide if older than 120 seconds
                    var time = ((int) (tick.Ts / 1000)).fromUnixTime();

                    if (time > DateTime.UtcNow.AddSeconds(-120))
                    {
                        mapMarker.IsVisible = true;
                    }
                    else
                    {
                        mapMarker.IsVisible = false;
                    }

                    // hide if more than 100km from our map center
                    if (new PointLatLngAlt(FlightData.instance.gMapControl1.Position).GetDistance(mapMarker.Position) > 1000*100)
                        mapMarker.IsVisible = false;
                });

            return true;
        }

        private GMapMarker target;
        private Timer timer;
        internal static WebSocket4Net.WebSocket ws;

        public static event EventHandler<PointLatLngAlt> UpdateOutput;

        private void men_Click(object sender, EventArgs e)
        {
            new DowdingUI().ShowUserControl();
        }

        public static void Stop()
        {
            try
            {
                if (ws != null)
                    ws.Close();
            }
            catch
            {

            }
        }
    }
}
