using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MissionPlanner.Utilities;
using Onvif.Core.Client.Common;

namespace Onvif
{
    public class OnvifDevice
    {
        public void SetTrack(PointLatLngAlt myposition, PointLatLngAlt target)
        {
            var bearing = myposition.GetBearing(target);

            var distance = myposition.GetDistance(target);

            var curve = getCurve(myposition.Alt, distance);

            if (curve > 0)
                Console.Write("Target below horizon");

            var heightdelta = target.Alt - myposition.Alt;

            SetRPYAsync(0, Math.Tan(heightdelta / distance) * rad2deg, bearing);
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

        public async Task SetRPYAsync(double r, double p, double y)
        {
            var host = "192.165.1.59:8000";

            var ptz = await Onvif.Core.Client.OnvifClientFactory.CreatePTZClientAsync(host, "", "");

            var config = await ptz.GetConfigurationsAsync();

            var token = config.PTZConfiguration[0].token;

            await ptz.AbsoluteMoveAsync(token, new PTZVector() { PanTilt = new Vector2D() { x = (float)y, y = (float)p } },
                new PTZSpeed() {PanTilt = new Vector2D() {x = 120, y = 90}});

            /*
        //Specify the binding to be used for the client.
        BasicHttpBinding binding = new BasicHttpBinding();

        //Specify the address to be used for the client.
        EndpointAddress address =
            new EndpointAddress(new Uri("http://" + host + "/onvif/device_service"));

        var device = new Device.DeviceClient(binding, address);

        var capa = await device.GetCapabilitiesAsync(new CapabilityCategory[] {CapabilityCategory.PTZ});

        var uri = capa.Capabilities.PTZ.XAddr;

        // create the media object to use the service
        var mediaService = new Media.MediaClient(binding, address);// getMediaService(MyMediaServiceAddress);
        // The client gets and chooses the profile.
        var ProfileList = await mediaService.GetProfilesAsync();
        // use the first profile (the device has at least one media profile).
        var targetProfileToken = ProfileList.Profiles[0].token;


        var ptz = new PTZ.PTZClient(binding, address);

        var ptzconfig = await ptz.GetConfigurationsAsync();

        var ptztoken = ptzconfig.PTZConfiguration[0].NodeToken;

        //ptzconfig.Spaces.ContinuousPanTiltVelocitySpace[0].XRange.Min;

        //This section describes three operations to move the PTZ unit absolutely, relatively or
        //continuously.All operations require a ProfileToken referencing a Media Profile including a
        //PTZConfiguration.

        ptz.AbsoluteMoveAsync(ptztoken, new PTZVector() {PanTilt = new Vector2D() {x = (float)y, y = (float)p}},
            new PTZSpeed() {PanTilt = new Vector2D() {x = 120, y = 90}});
            */
        }
    }
}
