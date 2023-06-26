using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MissionPlanner.Utilities;
using Onvif.Core.Client;
using Onvif.Core.Client.Common;
using Onvif.Core.Client.Device;
using Onvif.Core.Client.Media;
using Onvif.Core.Client.Ptz;

namespace Onvif
{
    public class OnvifDevice
    {
        private DeviceClient device;
        private MediaClient media;
        private PTZClient ptz;
        private FloatRange xrange;
        private FloatRange yrange;
        private FloatRange zrange;
        private string token;

        public OnvifDevice()
        {

        }

        public OnvifDevice(string host,int port,string username,string password)
        {
            Host = host;
            Port = port;
            UserName = username;
            Password = password;
        }
        
        public async Task SetTrack(PointLatLngAlt myposition, PointLatLngAlt target)
        {
            var bearing = myposition.GetBearing(target);

            var distance = myposition.GetDistance(target);

            // prevent div/0 and also point it flat
            if (distance == 0)
                distance = 1000;

            var curve = getCurve(myposition.Alt, distance/1000.0);

            if (curve > 0)
                Console.Write("Target below horizon");

            var heightdelta = target.Alt - myposition.Alt;

            // soh cah toa
            // in degrees FOV  
            var fov = Math.Atan(40 / distance) * (180 / Math.PI) ;
            // in zoom X
            var z = (float) MathHelper.mapConstrained(fov, FOVMax, FOVMin, ZoomMin, ZoomMax);

            bearing += YawOffset;
            bearing = bearing % 360;

            await SetRPYAsync(0, Math.Atan(heightdelta / distance) * rad2deg, bearing, z);
        }


        const double rad2deg = (180 / Math.PI);
        const double deg2rad = (1.0 / rad2deg);
        const double EARTHRADIUSKM = 6371;
        /// <summary>
        /// https://dizzib.github.io/earth/curve-calc
        /// </summary>
        /// <param name="mypositionAlt"></param>
        /// <param name="distance"></param>
        /// <returns></returns>
        private double getCurve(double mypositionAlt, double distance)
        {
            var h0 = mypositionAlt;
            var d0 = distance;
            var h0_km = h0 * 0.001;
            var d0_km = d0;
            var d1_km = getHorizonDistance_km(h0_km);
            var h1_m = getTargetHiddenHeight_km(d0_km - d1_km) * 1000;
            var d1 = d1_km ;
            var h1 = h1_m ;

            return h1;
        }
        private double getHorizonDistance_km(double h0_km)
        {
            return Math.Sqrt(Math.Pow(h0_km, 2) + 2 * EARTHRADIUSKM * h0_km);
        }
        private double getTargetHiddenHeight_km(double d2_km)
        {
            if (d2_km < 0)
            {
                return 0;
            }
            return Math.Sqrt(Math.Pow(d2_km, 2) + Math.Pow(EARTHRADIUSKM, 2)) - EARTHRADIUSKM;
        }

        public async Task<OnvifDevice> Setup()
        {
            device = await OnvifClientFactory.CreateDeviceClientAsync(Host+":"+Port, UserName, Password);

            var devicecapabilities = await device.GetCapabilitiesAsync(new CapabilityCategory[] { CapabilityCategory.All});

            var info = await device.GetDeviceInformationAsync(new GetDeviceInformationRequest() { });

            media = await OnvifClientFactory.CreateMediaClientAsync(Host+":"+Port, UserName, Password);

            var profiles = await media.GetProfilesAsync();
             
            ptz = await Onvif.Core.Client.OnvifClientFactory.CreatePTZClientAsync(Host+":"+Port, UserName, Password);

            var capabilities = await ptz.GetServiceCapabilitiesAsync();

            var config = await ptz.GetConfigurationsAsync();

            var space = config.PTZConfiguration[0].DefaultAbsolutePantTiltPositionSpace;

            token = profiles.Profiles[0].token;

            xrange = config.PTZConfiguration[0].PanTiltLimits.Range.XRange;
            yrange = config.PTZConfiguration[0].PanTiltLimits.Range.YRange;
            zrange = config.PTZConfiguration[0].ZoomLimits.Range.XRange;

            
            var status = await ptz.GetStatusAsync(token);
            //var presets = await ptz.GetPresetsAsync(token);

            //Console.WriteLine("preset ");
            //  await ptz.GotoPresetAsync(token, presets.Preset[0].token, null);

            //Thread.Sleep(4000);

            var nodes = await ptz.GetNodesAsync();

            //Console.WriteLine("home ");
            //await ptz.GotoHomePositionAsync(token, null);

            //Thread.Sleep(2000);
            //Console.WriteLine("ContinuousMoveAsync ");
            /*
            await ptz.ContinuousMoveAsync(token, new PTZSpeed() {PanTilt = new Vector2D() {x = -1f, y = -1}, Zoom = new Vector1D() { x=0f}}, "2");
            Thread.Sleep(2000);
            await ptz.StopAsync(token, true, true);
            */
            return this;
        }

        public async Task SetRPYAsync(double roll, double pitch, double yaw, float zoom = 1)
        {
            var y = (float) MathHelper.mapConstrained(pitch, PitchMin, PitchMax, yrange.Min, yrange.Max);

            var x = (float) MathHelper.mapConstrained(yaw, YawMin, YawMax, yrange.Min, yrange.Max);

            var z = 0.294f * (float) Math.Log(zoom);

            Console.WriteLine("PTZ: x {0} yaw {1} y {2} pitch {3} z {4} zoom {5}", x, yaw, y, pitch, z, zoom);

            await ptz.AbsoluteMoveAsync(token,
                new PTZVector() {PanTilt = new Vector2D() {x = x, y = y}, Zoom = new Vector1D() {x = z}}, new PTZSpeed(){ PanTilt = new Vector2D(){ x = 0.2f, y = 0.1f}});
        }

        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 80;

        public double YawMin { get; set; } = 0;

        public double YawMax { get; set; } = 360;

        public double PitchMin { get; set; } = -56;

        public double PitchMax { get; set; } = 90;

        public double ZoomMin { get; set; } = 1;

        public double ZoomMax { get; set; } = 30;

        public double FOVMin { get; set; } = 2.3;

        public double FOVMax { get; set; } = 63.7;

        public string Password { get; set; } = "";

        public string UserName { get; set; } = "admin";

        public double YawOffset { get; set; } = 0;
    }
}
