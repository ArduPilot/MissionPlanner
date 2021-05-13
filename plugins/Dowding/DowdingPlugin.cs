using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dowding.Model;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner;
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

        public static void Start()
        {
            Task.Run(() =>
            {
                try
                {
                    var dowd = new MissionPlanner.WebAPIs.Dowding();
                    if (Settings.Instance.ContainsKey("Dowding_username") &&
                        Settings.Instance.ContainsKey("Dowding_password") && 
                        Settings.Instance.ContainsKey("Dowding_server"))
                    {
                        dowd.Auth( Settings.Instance["Dowding_username"], Settings.Instance["Dowding_password"], Settings.Instance["Dowding_server"])
                            .Wait();
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

                    dowd.Start(Settings.Instance["Dowding_server"]).Wait();
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
                });

            return true;
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
                    if (item.Overlay == overlay && item is GMarkerGoogle)
                    {
                        if (target != null)
                        {
                            target.ToolTipMode = MarkerTooltipMode.Never;
                            target.ToolTipText = "";
                        }
                        target = (GMarkerGoogle)item;
                        target.ToolTipMode = MarkerTooltipMode.Always;
                        target.ToolTipText = "Tracking";
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

                    var time = ((int) (tick.Ts / 1000)).fromUnixTime();

                    if (time > DateTime.UtcNow.AddSeconds(-120))
                    {
                        mapMarker.IsVisible = true;
                    }
                    else
                    {
                       mapMarker.IsVisible = false;
                    }

                });

            return true;
        }

        private GMarkerGoogle target;

        public event EventHandler<PointLatLngAlt> UpdateOutput;

        private void men_Click(object sender, EventArgs e)
        {
            new DowdingUI().ShowUserControl();
        }
    }
}
