using FormsVideoLibrary;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MissionPlanner;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Xamarin.GCSViews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Video : ContentPage
    {
        public Video()
        {
            InitializeComponent();

            // false so the bg is transparent
            hud1.bgon = false;

            // 10hz hud
            Forms.Device.StartTimer(TimeSpan.FromMilliseconds(100), () =>
            {
                Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    var start = DateTime.Now;

                    hud1.HoldInvalidation = true;
                    hud1.airspeed = MainV2.comPort.MAV.cs.airspeed;
                    hud1.alt = MainV2.comPort.MAV.cs.alt;
                    hud1.batterylevel = (float)MainV2.comPort.MAV.cs.battery_voltage;
                    hud1.batteryremaining = MainV2.comPort.MAV.cs.battery_remaining;
                    hud1.connected = MainV2.comPort.MAV.cs.connected;
                    hud1.current = (float)MainV2.comPort.MAV.cs.current;
                    hud1.datetime = MainV2.comPort.MAV.cs.datetime;
                    hud1.disttowp = MainV2.comPort.MAV.cs.wp_dist;
                    hud1.ekfstatus = MainV2.comPort.MAV.cs.ekfstatus;
                    hud1.failsafe = MainV2.comPort.MAV.cs.failsafe;
                    hud1.gpsfix = MainV2.comPort.MAV.cs.gpsstatus;
                    hud1.gpsfix2 = MainV2.comPort.MAV.cs.gpsstatus2;
                    hud1.gpshdop = MainV2.comPort.MAV.cs.gpshdop;
                    hud1.gpshdop2 = MainV2.comPort.MAV.cs.gpshdop2;
                    hud1.groundalt = (float)MainV2.comPort.MAV.cs.HomeAlt;
                    hud1.groundcourse = MainV2.comPort.MAV.cs.groundcourse;
                    hud1.groundspeed = MainV2.comPort.MAV.cs.groundspeed;
                    hud1.heading = MainV2.comPort.MAV.cs.yaw;
                    hud1.linkqualitygcs = MainV2.comPort.MAV.cs.linkqualitygcs;
                    hud1.message = MainV2.comPort.MAV.cs.messageHigh;
                    hud1.mode = MainV2.comPort.MAV.cs.mode;
                    hud1.navpitch = MainV2.comPort.MAV.cs.nav_pitch;
                    hud1.navroll = MainV2.comPort.MAV.cs.nav_roll;
                    hud1.pitch = MainV2.comPort.MAV.cs.pitch;
                    hud1.roll = MainV2.comPort.MAV.cs.roll;
                    hud1.status = MainV2.comPort.MAV.cs.armed;
                    hud1.targetalt = MainV2.comPort.MAV.cs.targetalt;
                    hud1.targetheading = MainV2.comPort.MAV.cs.nav_bearing;
                    hud1.targetspeed = MainV2.comPort.MAV.cs.targetairspeed;
                    hud1.turnrate = MainV2.comPort.MAV.cs.turnrate;
                    hud1.verticalspeed = MainV2.comPort.MAV.cs.verticalspeed;
                    hud1.vibex = MainV2.comPort.MAV.cs.vibex;
                    hud1.vibey = MainV2.comPort.MAV.cs.vibey;
                    hud1.vibez = MainV2.comPort.MAV.cs.vibez;
                    hud1.wpno = (int)MainV2.comPort.MAV.cs.wpno;
                    hud1.xtrack_error = MainV2.comPort.MAV.cs.xtrack_error;
                    hud1.AOA = MainV2.comPort.MAV.cs.AOA;
                    hud1.SSA = MainV2.comPort.MAV.cs.SSA;
                    hud1.critAOA = MainV2.comPort.MAV.cs.crit_AOA;
                    hud1.HoldInvalidation = false;
                    hud1.Invalidate();

                    hud1.Refresh();
                });
                return true;
            });            
        }

        private void hud1_Touch(object sender, SkiaSharp.Views.Forms.SKTouchEventArgs e)
        {
            
        }
    }
}