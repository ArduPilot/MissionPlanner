using Flurl;
using Flurl.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities
{
    public class DigitalSky
    {
        public string URL = "https://digitalsky-uat.centralindia.cloudapp.azure.com:8080";
        private JObject login;

        private string accessToken
        {
            get { return login["accessToken"].Value<string>(); }
        }

        static DigitalSky()
        {
            ServicePointManager
                    .ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;
        }

        public void test()
        {
            if (Login("", "").Result)
            {
                var userdrones = GetDrones();

                foreach (var userdrone in userdrones.Result)
                {
                    var permlist = ListFlyPermission(userdrone.Key);

                    foreach (var perm in permlist.Result)
                    {
                        if (perm.Value["status"].Value<string>().Contains("APPROVED"))
                        {
                            var flyArea = perm.Value["flyArea"].Children().Select(a =>
                                new PointLatLngAlt(a["latitude"].Value<double>(),
                                    a["longitude"].Value<double>(),
                                    perm.Value["maxAltitude"].Value<double>() / 3.281,
                                    perm.Value["id"].Value<string>()));
                        }
                        else
                        {

                        }
                    }
                }
            }
        }

        public async Task<bool> Login(string username, string password)
        {
            login = await URL
                .AppendPathSegment("api/auth/token")
                .PostJsonAsync(new {email = username, password = password}).ReceiveJson<JObject>();

            return !String.IsNullOrEmpty(accessToken);
        }

        public async Task<Dictionary<int, JToken>> GetDrones()
        {
            var drones = await URL
                .AppendPathSegment("api/operatorDrone")
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync<JArray>();

            return drones.Where(a => a["operatorDroneStatus"].Value<string>() == "UIN_APPROVED")
                .ToDictionary(a => a["id"].Value<int>());
        }

        public async Task<Dictionary<string, JToken>> ListFlyPermission(int drone)
        {
            var flyDronePermissionApplication_list = await URL
                .AppendPathSegment("/api/applicationForm/flyDronePermissionApplication/list")
                .SetQueryParam("droneId", drone)
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync<JArray>();

            return flyDronePermissionApplication_list.ToDictionary(a => a["id"].Value<string>());
        }

        public async Task<string> GetPermissionArtifact(string applicationid)
        {
            var flyDronePermissionApplication_application = await
                URL
                    .AppendPathSegment(
                        "api/applicationForm/flyDronePermissionApplication/" + applicationid +
                        "/document/permissionArtifact")
                    .WithOAuthBearerToken(accessToken).GetStringAsync();

            return flyDronePermissionApplication_application;
        }

        public async Task<JObject> UploadFlightLog(string applicationid, string flightlogfile)
        {
            var uploadlog = await URL
                .AppendPathSegment(
                    "/api/applicationForm/flyDronePermissionApplication/" + applicationid + "/document/flightLog")
                .WithOAuthBearerToken(accessToken)
                .PostMultipartAsync(mp =>
                    mp.AddFile("flightLogDocument", flightlogfile))
                .ReceiveJson<JObject>();

            return uploadlog;
        }
    }
}