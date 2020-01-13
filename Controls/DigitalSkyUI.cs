using Flurl;
using Flurl.Http;
using GMap.NET;
using GMap.NET.WindowsForms;
using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.Maps;
using MissionPlanner.Utilities;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class DigitalSkyUI : UserControl, IActivate
    {
        GMapOverlay _markeroverlay;
        private DigitalSky digitalSky;

        public DigitalSkyUI()
        {
            InitializeComponent();
        }

        public async void test()
        {
            ServicePointManager
                    .ServerCertificateValidationCallback +=
                (sender, cert, chain, sslPolicyErrors) => true;

            var login = await "https://digitalsky-uat.centralindia.cloudapp.azure.com:8080"
                .AppendPathSegment("api/auth/token")
                .PostJsonAsync(new { email = "", password = "" }).ReceiveJson<JObject>().ConfigureAwait(false);

            var login2 = login as IDictionary<String, Object>;


            var accessToken = login["accessToken"].Value<string>();

            var myuserid = login["id"].Value<Int64>();

            //https://github.com/iSPIRT/digital-sky-api/blob/develop/src/main/java/com/ispirit/digitalsky/controller/UserController.java#L74
            var user = await "https://digitalsky-uat.centralindia.cloudapp.azure.com:8080"
                .AppendPathSegment("api/user/" + myuserid)
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync<JObject>().ConfigureAwait(false);

            //https://github.com/iSPIRT/digital-sky-api/blob/master/src/main/java/com/ispirit/digitalsky/controller/UserController.java#L89
            var apps = await "https://digitalsky-uat.centralindia.cloudapp.azure.com:8080"
                .AppendPathSegment("api/user/applications")
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync<JArray>().ConfigureAwait(false);

            //https://github.com/iSPIRT/digital-sky-api/blob/master/src/main/java/com/ispirit/digitalsky/controller/OperatorDroneController.java#L36
            var drones = await "https://digitalsky-uat.centralindia.cloudapp.azure.com:8080"
                .AppendPathSegment("api/operatorDrone")
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync<JArray>().ConfigureAwait(false);

            //https://github.com/iSPIRT/digital-sky-api/blob/develop/src/main/java/com/ispirit/digitalsky/controller/OperatorDroneController.java#L47
            var droneid = await "https://digitalsky-uat.centralindia.cloudapp.azure.com:8080"
                .AppendPathSegment("api/operatorDrone/7")
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync<JObject>().ConfigureAwait(false);

            //https://github.com/iSPIRT/digital-sky-api/blob/a0144df5e558db27e15ecda25e1e4836408bc8d9/src/main/java/com/ispirit/digitalsky/controller/FlyDronePermissionApplicationController.java#L136
            var flyDronePermissionApplication_list = await "https://digitalsky-uat.centralindia.cloudapp.azure.com:8080"
                .AppendPathSegment("/api/applicationForm/flyDronePermissionApplication/list")
                .SetQueryParam("droneId", "7")
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync<JArray>().ConfigureAwait(false);

            var flyDronePermissionApplication_application = await "https://digitalsky-uat.centralindia.cloudapp.azure.com:8080"
                .AppendPathSegment(
                    "api/applicationForm/flyDronePermissionApplication/5d175f1c4cedfd0005d76bea/document/permissionArtifact")
                .WithOAuthBearerToken(accessToken).GetStringAsync().ConfigureAwait(false);
            /*
                //https://github.com/iSPIRT/digital-sky-api/blob/develop/src/main/java/com/ispirit/digitalsky/controller/AirspaceCategoryController.java#L63
                        var getAllAirspaceCategory = "https://digitalsky-uat.centralindia.cloudapp.azure.com:8080"
                    .AppendPathSegment("/api/airspaceCategory/list")
                    .WithHeader("Authorization", "Bearer " + accessToken)
                    .GetJsonAsync<List<JObject>>().Result;
                    */


            var uploadlog = await "https://digitalsky-uat.centralindia.cloudapp.azure.com:8080"
                .AppendPathSegment(
                    "/api/applicationForm/flyDronePermissionApplication/5d175f1c4cedfd0005d76bea/document/flightLog")
                .AllowAnyHttpStatus()
                .WithOAuthBearerToken(accessToken)
                .PostMultipartAsync(mp => mp.AddFile("flightLogDocument", @"C:\Users\mich1\Downloads\permissionArtifact")).ReceiveJson<JObject>().ConfigureAwait(false);




            var manufacturer = await "https://digitalsky-uat.centralindia.cloudapp.azure.com:8080"
                .AppendPathSegment("/api/manufacturer/0")
                .AllowAnyHttpStatus()
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync<JObject>().ConfigureAwait(false);


            var occurance = await "https://digitalsky-uat.centralindia.cloudapp.azure.com:8080"
                .AppendPathSegment("/api/occurrenceReport/drone/7/list")
                .AllowAnyHttpStatus()
                .WithOAuthBearerToken(accessToken)
                .GetJsonAsync<JArray>().ConfigureAwait(false);




            var flightcreate = await "https://digitalsky-uat.centralindia.cloudapp.azure.com:8080"
                .AppendPathSegment("/api/applicationForm/flyDronePermissionApplication")
                .AllowAnyHttpStatus()
                .WithOAuthBearerToken(accessToken)
                .PostJsonAsync(new
                {
                    droneId = 7,
                    id = "0",
                    pilotBusinessIdentifier = "9646771ef9e148228a6d56f305df6489",
                    //pilotId =18,
                    flyArea = new[]
                    {
                        new {latitude = 12.9346483, longitude = 77.6091678},
                        new {latitude = 12.9342143, longitude = 77.6098088},
                        new
                        {
                            latitude = 12.9334793, longitude = 77.6092478
                        },
                        // close it
                        new {latitude = 12.9346483, longitude = 77.6091678}
                    },

                    //operatorId = 0,
                    payloadWeightInKg = 1,
                    payloadDetails = "camera",
                    flightPurpose = "hobby",
                    maxAltitude = 400,
                    startDateTime = DateTime.Now.AddDays(2).ToString("dd-MM-yyyy HH:mm:ss"),
                    endDateTime = DateTime.Now.AddDays(2).AddHours(2).ToString("dd-MM-yyyy HH:mm:ss"),
                    //  status= "SUBMITTED",
                    /* recurringTimeExpression = "",
                     recurringTimeDurationInMinutes = 0,
                     recurringPatternType = "",
                    */
                    /* fir = "",
                    adcNumber = "",
                    ficNumber = "",
                    maxEndurance = 0,
                    droneType = "",
                    uin = ""*/
                }).ReceiveJson<JObject>().ConfigureAwait(false);
        }

        public void Activate()
        {
            myGMAP1.MapProvider = GCSViews.FlightData.mymap.MapProvider;
            myGMAP1.MaxZoom = 20;
            myGMAP1.Zoom = 5;
            myGMAP1.DisableFocusOnMouseEnter = true;
            myGMAP1.DragButton = MouseButtons.Left;
            myGMAP1.Position = new PointLatLng(17.8758086, 77.7369485);
            myGMAP1.FillEmptyTiles = true;

            _markeroverlay = new GMapOverlay("markers");
            myGMAP1.Overlays.Add(_markeroverlay);

            myGMAP1.Invalidate();

            digitalSky = new DigitalSky();
        }

        private async void But_login_Click(object sender, EventArgs e)
        {
            var un = Settings.Instance["DigitalSky_Username", ""];
            if (InputBox.Show("Username", "Username", ref un) == DialogResult.OK)
            {
                Settings.Instance["DigitalSky_Username"] = un;
                // secures against copy of config to another pc, but not locally
                var pw = "";
                if (Settings.Instance["DigitalSky_Password", ""] != "")
                    pw = new Crypto().DecryptString(Settings.Instance["DigitalSky_Password", ""]);
                if (InputBox.Show("Password", "Password", ref pw, true) == DialogResult.OK)
                {
                    Settings.Instance["DigitalSky_Password"] = new Crypto().EncryptString(pw);

                    if (await digitalSky.Login(un, pw).ConfigureAwait(false))
                    {
                        var dronelist = (await digitalSky.GetDrones().ConfigureAwait(false));

                        var displaylist = dronelist.Select(a => new KeyValuePair<int, string>(
                            a.Value["id"].Value<int>(),
                            a.Value["droneType"]["modelName"].Value<string>()));

                        cmb_drones.DisplayMember = "Value";
                        cmb_drones.ValueMember = "Key";
                        cmb_drones.DataSource = displaylist.ToList();

                        CustomMessageBox.Show("Login Successful");
                    }
                    else
                    {
                        CustomMessageBox.Show("Login Failed");
                    }
                }
            }
        }

        private async void Cmb_drones_SelectedIndexChanged(object sender, EventArgs e)
        {
            var applist = await digitalSky
                .ListFlyPermission((int)cmb_drones.SelectedValue).ConfigureAwait(false);

            var displaylist =
                applist.Where(a => a.Value["status"].Value<string>().Contains("APPROVED")).Select(a => new KeyValuePair<string, JToken>(a.Key, a.Value));

            cmb_applications.DisplayMember = "Key";
            cmb_applications.ValueMember = "Value";
            cmb_applications.DataSource = displaylist.ToList();
        }

        private void Cmb_applications_SelectedIndexChanged(object sender, EventArgs e)
        {
            var perm = (JToken)cmb_applications.SelectedValue;

            var flyArea = perm["flyArea"].Children().Select(a =>
                new PointLatLngAlt(a["latitude"].Value<double>(), a["longitude"].Value<double>(), perm["maxAltitude"].Value<double>() / 3.281, perm["id"].Value<string>()));

            lbl_approvedstatus.Text = perm["status"].Value<string>();

            _markeroverlay.Markers.Clear();
            _markeroverlay.Polygons.Clear();

            _markeroverlay.Polygons.Add(new GMapPolygon(flyArea.ToList().Select(a => (PointLatLng)a).ToList(), ""));

            foreach (var pointLatLngAlt in flyArea)
            {
                _markeroverlay.Markers.Add(new GMapMarkerWP(pointLatLngAlt, ""));
            }

            var rect = myGMAP1.GetRectOfAllMarkers(null);
            if (rect.HasValue)
            {
                // 10% padding
                rect.Value.Inflate(rect.Value.HeightLat * 0.1, rect.Value.WidthLng * 0.1);
                myGMAP1.SetZoomToFitRect(rect.Value);
            }
        }

        private async void But_dlartifact_Click(object sender, EventArgs e)
        {
            var perm = (JToken)cmb_applications.SelectedValue;

            var id = perm["id"].Value<string>();

            var xmlfile = await digitalSky.GetPermissionArtifact(id).ConfigureAwait(false);

            var artifactdir = Settings.GetDataDirectory() + Path.DirectorySeparatorChar + "DigitalSkyArtifact";

            if (!Directory.Exists(artifactdir))
                Directory.CreateDirectory(artifactdir);

            var destlocalfile = artifactdir + Path.DirectorySeparatorChar + id + ".xml";

            File.WriteAllText(destlocalfile, xmlfile);

            MAVFtp ftp = new MAVFtp(MainV2.comPort, (byte)MainV2.comPort.sysidcurrent,
                (byte)MainV2.comPort.compidcurrent);

            ftp.UploadFile(id + ".xml", destlocalfile, null);

            ftp = null;
        }

        private async void But_uploadflightlog_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "DigitalSky Flight Log (*.json)|*.json";
            ofd.ShowDialog();
            if (File.Exists(ofd.FileName))
            {
                var perm = (JToken)cmb_applications.SelectedValue;
                var id = perm["id"].Value<string>();

                var ans = await digitalSky.UploadFlightLog(id, ofd.FileName).ConfigureAwait(false);

                CustomMessageBox.Show(ans.ToString());
            }
        }
    }
}
