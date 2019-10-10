
using Flurl;
using Flurl.Http;
using GMap.NET;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using MQTTnet.Client.Receiving;
using MQTTnet.Protocol;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using MissionPlanner.Comms;
using X509Certificate = System.Security.Cryptography.X509Certificates.X509Certificate;

namespace MissionPlanner.Utilities
{
    public class TelstraUTM
    {
        private string mqtt_host = "a3eq8dm0ps3e6c-ats.iot.ap-southeast-2.amazonaws.com";
        private int mqtt_port = 8883;
        private string api_base_url = "https://np.telstra-cns.com";
        private string authorize_url = "https://laam-users-np.auth.ap-southeast-2.amazoncognito.com/authorize";
        private string token_url = "https://laam-users-np.auth.ap-southeast-2.amazoncognito.com/oauth2/token";

        private string client_id = "";
        private string client_secret = "";

        private string callback_uri = "http://localhost:3000";

        public void confighardware(string portname)
        {
            var port = new SerialPort(portname, 115200);
            port.ReadTimeout = 1000;
            port.Open();
            AT(port);
            AT(port, "ATE0"); /// echo off
            AT(port, "AT+CMEE=2"); //  Report mobile termination error - 2: +CME ERROR: <err> result code enabled and verbose <err> values used
            AT(port, "AT+CGMI"); //  Manufacturer identification 
            AT(port, "AT+CGMM"); //Model identification 
            AT(port, "AT+CGMR"); // Firmware version identification 
            AT(port, "ATI9"); //  Identification information 
            AT(port, "AT+CLCK=\"SC\",2"); // Facility lock - status
            AT(port, "AT+CPIN?"); // pin read
            AT(port, "AT+UPSV?"); //Power saving control 
            AT(port, "AT+CCLK?"); // Clock
            AT(port, "AT+CGSN"); // IMEI identification
            AT(port, "AT+COPS?"); // Operator selection 
            AT(port, "AT+CEREG=2"); // EPS network registration status - 2: network registration and location information 
            AT(port, "AT+CEREG?");
            AT(port, "AT+CEREG=0"); // 0: network registration URC disabled
            AT(port, "AT+CSQ"); // Signal quality

            //AT+UMQTT=1,1883
            //AT+UMQTT=2,"home.oborne.me",1883
            //AT+UMQTTC=1
            //AT+UMQTTC=2,0,0,"/user/ublox","Hi! This is an MQTT message."

            //AT+UMQTTER
            //https://github.com/daz/MKRGSM/blob/master/src/GSMSecurity.cpp
            //https://portal.u-blox.com/s/question/0D52p00008LRmCwCAL/how-to-debug-cme-error-usecmng-invalid-certificatekey-format
            {
                // load the certs
                var ca = File.ReadAllBytes("ca.der");
                port.WriteLine("AT+USECMNG=0,0,\"CA\"," + ca.Length);
                Thread.Sleep(100);
                port.Write(ca, 0, ca.Length);
                Thread.Sleep(1000);
                Console.Write(port.ReadExisting());

                var clientcrt = File.ReadAllBytes("client.der");
                var clientkey = File.ReadAllBytes("clientkey.der");

                port.WriteLine("AT+USECMNG=0,1,\"client\"," + clientcrt.Length);
                Thread.Sleep(100);
                port.Write(clientcrt, 0, clientcrt.Length);
                Thread.Sleep(1000);
                Console.Write(port.ReadExisting());

                port.WriteLine("AT+USECMNG=0,2,\"clientkey\"," + clientkey.Length);
                Thread.Sleep(100);
                port.Write(clientkey, 0, clientkey.Length);
                Thread.Sleep(1000);
                Console.Write(port.ReadExisting());

                // create profile
                AT(port, "AT+USECPRF=0,3,\"CA\"");
                AT(port, "AT+USECPRF=0,5,\"client\"");
                AT(port, "AT+USECPRF=0,6,\"clientkey\"");

                AT(port, "AT+USECPRF=0,0,1"); // cert validation
                AT(port, "AT+USECPRF=0,1,3"); // tls 1.2

                AT(port, "AT+UMQTT=11,1,0"); // enable tls on mqtt - doesnt work on r410m
            }

            AT(port, "AT+USECMNG=3");

            if (false)
            {
                mqtt_host = "test.oborne.me";
                mqtt_port = 1883;

                //server
                AT(port, "AT+UMQTT=2,\"" + mqtt_host + "\"," + mqtt_port + "");
                //login
                AT(port, "AT+UMQTTC=1", 35);

                AT(port,
                    "AT+UMQTTC=2,0,0,\"user\",\"Hi! This is an MQTT message. " + DateTime.Now.ToString("s") + "\"");

                // logout
                AT(port, "AT+UMQTTC=0");
            }
            // tcp echo test
            {
                var ans = AT(port, "AT+USOCR=6"); // create tcp socket                

                AT(port, "AT+USOSEC=0,1,0"); // enable ssl with profile 0

                ans = AT(port, "AT+UDNSRN=0,\""+ mqtt_host + "\"", 60); // dns resolution
                var ip = ans.reply.Replace("\tOK", "").Trim('\t').Replace("+UDNSRN:", "").Trim(new[] {' ', '"'})
                    .Split(',').FirstOrDefault().Trim('"');

                AT(port, "AT+USOCO=0,\"" + ip + "\"," + mqtt_port,120); // connect to ip

                AT(port, "AT+USORD=3,0"); // bytes to read


                //AT(port, "AT+USORD=0,32"); // get the greeting
                AT(port, "AT+USOWR=0,4,\"Test\""); // write
                //AT(port, "AT+USORD=0,4"); // read

                AT(port, "AT+USOCL=0"); // close the socket
            }

        }

        public static (string status, string reply, TimeSpan elapsed) AT(ICommsSerial port, string cmd = "AT",
            int timeout = 10, string success = "OK", string failure = "+CME ERROR")
        {
            // clear the buffer
            Console.WriteLine("Existing: " + port.ReadExisting());
            port.Write(cmd + "\r\n");
            Console.WriteLine("TX: " + cmd);
            DateTime start = DateTime.Now;
            var reply = "";
            var echo = false;
            while (true)
            {
                if (port.BytesToRead > 0)
                {
                    var line = "";
                    try
                    {
                        line = port.ReadLine();
                    }
                    catch
                    {
                        line = port.ReadExisting();
                    }

                    Console.WriteLine(line);
                    if (!echo)
                        echo = line.EndsWith(cmd);
                    if (line != "\r\n" && !echo) // blank line or echo'd command
                    {
                        reply += "\t" + line.TrimEnd();
                        if (line.StartsWith(success))
                        {
                            Console.WriteLine("RX: Success");
                            return ("Success", reply, DateTime.Now - start);
                        }

                        if (line.StartsWith(failure))
                        {
                            Console.WriteLine("RX: Error");
                            return ("Error", reply, DateTime.Now - start);
                        }
                    }
                }

                if ((DateTime.Now - start).TotalSeconds > timeout)
                {
                    Console.WriteLine("RX: Timeout");
                    return ("Timeout", reply, DateTime.Now - start);
                }

                Thread.Sleep(20);
            }
        }

        public void Auth()
        {
            var authorization_redirect_url = authorize_url + "?response_type=code&client_id=" + client_id +
                                             "&redirect_uri=" + callback_uri + "";

            var tcplistern = TcpListener.Create(3000);
            tcplistern.Start();

            System.Diagnostics.Process.Start(authorization_redirect_url);

            var client = tcplistern.AcceptTcpClient();
            client.NoDelay = true;
            var clientstream = client.GetStream();
            var sr = new StreamReader(clientstream);

            var requesturl = sr.ReadLine();

            var responce =
                "HTTP/1.1 200 OK\r\nContent-Type: text/html\r\nConnection: Closed\r\n\r\n<!DOCTYPE HTML PUBLIC \"-//IETF//DTD HTML 2.0//EN\"><html><body onload=\"window.open('','_self').close();return false;\">Please close me</body></html>"
                    .Select(a => (byte)a).ToArray();
            clientstream.Write(responce, 0, responce.Count());
            client.Client.Disconnect(true);
            client.Dispose();

            tcplistern.Stop();

            var authorization_code =
                System.Web.HttpUtility.ParseQueryString(new Uri(new Uri(callback_uri), requesturl.Split(' ')[1]).Query);

            var data = new
            {
                grant_type = "authorization_code",
                code = authorization_code["code"],
                redirect_uri = callback_uri,
                client_id = client_id,
                client_secret = client_secret
            };


            //https://docs.aws.amazon.com/cognito/latest/developerguide/token-endpoint.html

            AccessTokenResponse =
                token_url.PostUrlEncodedAsync(data).ReceiveJson<JObject>().Result;


        }

        public void SetUAV(string id)
        {
            var data = new { deviceId = id };
            UavResponse = api_base_url.AppendPathSegment("/api/uav")
                .WithOAuthBearerToken(AccessTokenResponse["access_token"].ToString())
                .PostJsonAsync(data).ReceiveJson<JObject>().Result;


        }

        public JArray GetUAVs()
        {
            return api_base_url.AppendPathSegment("/api/uav")
                .WithOAuthBearerToken(AccessTokenResponse["access_token"].ToString()).GetJsonAsync<JArray>().Result;
        }

        public void SetMission(string devid, string operatorId, List<PointLatLngAlt> mission)
        {
            var alt = 0.0;
            var list = mission.Where(a =>
            {
                alt = Math.Max(alt, a.Alt);
                return a.Lat != 0 && a.Lng != 0;
            }).ToArray();

            var corners = MinBoundingBox(list);

            var data = new
            {
                boundary = corners.Select(a => new { lon = a.Lng, lat = a.Lat }).ToArray(),
                altitude = alt,
                preflightCheck = false,
                visualLineOfSight = true,
                laancOperationData = "123",
                stop = DateTime.UtcNow.AddHours(1).ToString("O"),
                operatorId = operatorId,
                deviceId = devid,
                timezone = "Australia/Melbourne",
                start = DateTime.UtcNow.ToString("O")
            };

            var missionreq = api_base_url.AppendPathSegment("/api/mission").WithOAuthBearerToken(AccessTokenResponse["access_token"].ToString())
                .PostJsonAsync(data).ReceiveJson().Result;
        }

        public JArray GetMissions()
        {
            return api_base_url.AppendPathSegment("/api/mission").WithOAuthBearerToken(AccessTokenResponse["access_token"].ToString())
                .GetJsonAsync<JArray>().Result;
        }

        private PointLatLng[] MinBoundingBox(PointLatLngAlt[] list)
        {
            RectLatLng minBox = RectLatLng.Empty;
            var minAngle = 0d;

            //foreach edge of the convex hull
            for (var i = 0; i < list.Length; i++)
            {
                var nextIndex = i + 1;

                var current = list[i];
                var next = list[nextIndex % list.Length];

                //min / max points
                var top = double.MinValue;
                var bottom = double.MaxValue;
                var left = double.MaxValue;
                var right = double.MinValue;

                //get angle of segment to x axis
                var angle = AngleToXAxis(current, next);

                //rotate every point and get min and max values for each direction
                foreach (var p in list)
                {
                    var rotatedPoint = RotateToXAxis(p, angle);

                    top = Math.Max(top, rotatedPoint.Lat);
                    bottom = Math.Min(bottom, rotatedPoint.Lat);

                    left = Math.Min(left, rotatedPoint.Lng);
                    right = Math.Max(right, rotatedPoint.Lng);
                }

                //create axis aligned bounding box
                var box = RectLatLng.FromLTRB(left, top, right, bottom);

                if (minBox == RectLatLng.Empty || minBox.Size.HeightLat * minBox.Size.WidthLng > box.Size.HeightLat * box.Size.WidthLng)
                {
                    minBox = box;
                    minAngle = angle;
                }
            }

            return new[]
            {
                RotateToXAxis(new PointLatLng(minBox.Top,minBox.Left), -minAngle),
                RotateToXAxis(new PointLatLng(minBox.Top,minBox.Right), -minAngle),
                RotateToXAxis(new PointLatLng(minBox.Bottom,minBox.Left), -minAngle),
                RotateToXAxis(new PointLatLng(minBox.Bottom,minBox.Right), -minAngle)
            };
        }

        static double AngleToXAxis(PointLatLng A, PointLatLng B)
        {
            return -Math.Atan((A.Lat - B.Lat) / (A.Lng - B.Lng));
        }

        static PointLatLng RotateToXAxis(PointLatLng v, double angle)
        {
            var newX = v.Lng * Math.Cos(angle) - v.Lat * Math.Sin(angle);
            var newY = v.Lng * Math.Sin(angle) + v.Lat * Math.Cos(angle);

            return new PointLatLng(newY, newX);
        }

        public void SetFlightPlan(string devid, string missionid, List<MAVLink.mavlink_mission_item_int_t> mission)
        {
            var data = new
            {
                deviceId = devid,
                missionId = missionid,
                waypoints = mission.Where(a => a.x != 0 && a.y != 0).Select(a => new
                {
                    type = (MAVLink.MAV_CMD)a.command,
                    alt = a.z,
                    lon = a.x / 1e7,
                    lat = a.y / 1e7,
                    param1 = a.param1,
                    param2 = a.param2,
                    param3 = a.param3,
                    param4 = a.param4,
                }).ToArray()
            };
            var missionreq = api_base_url.AppendPathSegment("/api/flightplan").WithOAuthBearerToken(AccessTokenResponse["access_token"].ToString())
                .PostJsonAsync(data).ReceiveJson().Result;
        }

        public JArray GetFlightPlans()
        {
            return api_base_url.AppendPathSegment("/api/flightplan").WithOAuthBearerToken(AccessTokenResponse["access_token"].ToString())
                .GetJsonAsync<JArray>().Result;
        }

        public void StartMQTT(string clientid, string cafile, string clientcertfile, string clientprivate, string clientprivatepassword = "")
        {
            var ca = new X509Certificate(cafile, "");

            var reader = new PemReader(File.OpenText(clientprivate));
            var privatekey = (AsymmetricCipherKeyPair)reader.ReadObject();
            var pkinfo = (Org.BouncyCastle.Crypto.Parameters.RsaPrivateCrtKeyParameters)privatekey.Private;

            var ce1 = new X509Certificate2(File.ReadAllBytes(clientcertfile), clientprivatepassword,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet);
            CspParameters parms = new CspParameters();
            parms.Flags = CspProviderFlags.NoFlags;
            parms.KeyContainerName = Guid.NewGuid().ToString().ToUpperInvariant();
            parms.ProviderType = ((Environment.OSVersion.Version.Major > 5) || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1))) ? 0x18 : 1;

            System.Security.Cryptography.RSACryptoServiceProvider rcsp =
                new System.Security.Cryptography.RSACryptoServiceProvider(parms) { PersistKeyInCsp = true };

            rcsp.ImportParameters(DotNetUtilities.ToRSAParameters(pkinfo));
            ce1.PrivateKey = rcsp;

            //var clientcert = CertificatesToDBandBack.Certificate.GetCertificateFromPEMstring(File.ReadAllText(clientcertfile), File.ReadAllText(clientprivate), clientprivatepassword);

            MQTTClient = new MqttFactory().CreateMqttClient();
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(mqtt_host, mqtt_port).WithClientId(clientid).WithTls(new MqttClientOptionsBuilderTlsParameters
                {
                    AllowUntrustedCertificates = false,
                    IgnoreCertificateChainErrors = false,
                    IgnoreCertificateRevocationErrors = false,
                    UseTls = true,
                    SslProtocol = System.Security.Authentication.SslProtocols.Tls12,
                    Certificates = new List<byte[]>
                    {
                        ca.Export(X509ContentType.SerializedCert),
                        ce1.Export(X509ContentType.SerializedCert)
                    },
                    CertificateValidationCallback = (X509Certificate x, X509Chain y, SslPolicyErrors z, IMqttClientOptions o) =>
                    {
                        return true;
                    }
                }).Build();

            MQTTClient.ApplicationMessageReceivedHandler = new MqttApplicationMessageReceivedHandlerDelegate(e =>
            {
                Console.WriteLine("### RECEIVED APPLICATION MESSAGE ###");
                Console.WriteLine($"+ Topic = {e.ApplicationMessage.Topic}");
                Console.WriteLine($"+ Payload = {Encoding.UTF8.GetString(e.ApplicationMessage.Payload)}");
                Console.WriteLine($"+ QoS = {e.ApplicationMessage.QualityOfServiceLevel}");
                Console.WriteLine($"+ Retain = {e.ApplicationMessage.Retain}");
                Console.WriteLine();
            });
            MQTTClient.ConnectedHandler = new MqttClientConnectedHandlerDelegate(async a =>
            {
                Console.WriteLine("### CONNECTED WITH SERVER ###");
                await MQTTClient.SubscribeAsync(new TopicFilterBuilder().WithTopic("#").Build());
            });
            MQTTClient.DisconnectedHandler = new MqttClientDisconnectedHandlerDelegate(async a =>
            {
                Console.WriteLine("### DISCONNECTED FROM SERVER ###");
            });

            var connect = MQTTClient.ConnectAsync(options).Result;

            var sub = MQTTClient.SubscribeAsync(new TopicFilter
            { Topic = "test", QualityOfServiceLevel = MqttQualityOfServiceLevel.AtMostOnce });



        }

        public IMqttClient MQTTClient { get; set; }

        public void Telemetry(string devid)
        {
            //{"timestamp":"2016-05-01 12:42:54","type":"telemetry","telemetry":{"time_measured": "2016-05-01 12:42:54","rssi": "","battery": "0","armed": "false","cid": "0","altitude": "473.500","latitude": "47.199897","longitude": "9.442750","mode": "UTMBOX","speed": "0.0972222222222","heading": "36.8"}}

            var cs = MainV2.comPort.MAV.cs;

            var data = new
            {
                timestamp = DateTime.UtcNow.ToString("O"),
                type = "telemetry",
                telemetry = new
                {
                    time_measured = DateTime.UtcNow.ToString("O"),
                    latitude = cs.lat,
                    longitude = cs.lng,
                    altitude = cs.altasl,
                    relAltitude = cs.alt,
                    battery = cs.battery_remaining,
                    heading = cs.yaw,
                    speed = cs.groundspeed,
                    armed = cs.armed,
                    mode = cs.mode,
                    cid = "0"
                }
            };

            if (MQTTClient.IsConnected)
            {
                var msg = new MqttApplicationMessageBuilder().WithTopic("prod/device/telemetry/" + devid)
                    .WithPayload(data.ToJSON().Select(a => (byte) a)).WithAtLeastOnceQoS().Build();


                var pub = MQTTClient.PublishAsync(msg).Result;
            }
        }

        public JObject AccessTokenResponse { get; set; }
        public dynamic UavResponse { get; set; }
    }
}
