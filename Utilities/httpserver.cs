using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using log4net;
using SharpKml.Base;
using SharpKml.Dom;
using Newtonsoft.Json;

namespace MissionPlanner.Utilities
{
    public class httpserver
    {
        /// <summary>
        /// used for mini http server for websockets/mjpeg video stream, and network link kmls
        /// </summary>
        private TcpListener listener;

        // Thread signal. 
        public static ManualResetEvent tcpClientConnected =
            new ManualResetEvent(false);


        /// <summary>
        /// used to feed in a network link kml to the http server
        /// </summary>
        public static string georefkml = "";

        public static string mavelous_web = Settings.GetRunningDirectory() + @"mavelous_web\";
        public static string georefimagepath = "";

        public static bool run = true;

        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        ~httpserver()
        {
            run = false;
            tcpClientConnected.Set();
        }

        public static void Stop()
        {
            run = false;
            tcpClientConnected.Set();
        }

        /// <summary>          
        /// little web server for sending network link kml's          
        /// </summary>          
        public void listernforclients()
        {
            try
            {
                listener = new TcpListener(IPAddress.Any, 56781);

                listener.Start();
            }
            catch (Exception e)
            {
                log.Error("Exception starting listener. Possible multiple instances of planner?", e);
                return;
            } // in use
            // Enter the listening loop.               
            while (run)
            {
                // Perform a blocking call to accept requests.           
                // You could also user server.AcceptSocket() here.               
                try
                {
                    log.Info("Listening for client");
                    //TcpClient client = listener.AcceptTcpClient();

                    // Set the event to nonsignaled state.
                    tcpClientConnected.Reset();

                    listener.BeginAcceptTcpClient(new AsyncCallback(DoAcceptTcpClientCallback), listener);

                    // Wait until a connection is made and processed before  
                    // continuing.
                    tcpClientConnected.WaitOne();

                    //System.Threading.Thread.Sleep(50);
                }
                catch (ThreadAbortException ex)
                {
                    log.Info(ex);
                    return;
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        public void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener) ar.AsyncState;

            // End the operation and display the received data on  
            // the console.

            TcpClient client = listener.EndAcceptTcpClient(ar);

            var th = new Thread(ProcessClient) { IsBackground = true };
            th.Start(client);

            // Signal the calling thread to continue.
            tcpClientConnected.Set();
        }

        public void ProcessClient(object clientobj)
        {
            var client = clientobj as TcpClient;
            using(client)
            {
                try
                {
                    // Get a stream object for reading and writing          
                    log.Info("Accepted Client " + client.Client.RemoteEndPoint.ToString());
                    //client.SendBufferSize = 100 * 1024; // 100kb
                    //client.LingerState.Enabled = true;
                    //client.NoDelay = true;

                    // makesure we have valid image
                    GCSViews.FlightData.myhud.streamjpgenable = true;

                    NetworkStream stream = client.GetStream();

                    // 5 seconds - default for httpd 2.2+
                    stream.ReadTimeout = 5000;

                    goto skipagain;

                    again:
                    log.Info("doing Again");

                    skipagain:

                    var asciiEncoding = new ASCIIEncoding();

                    var request = new byte[1024*4];
                    int len = 0;

                    // handle header
                    try
                    {
                        len = stream.Read(request, 0, request.Length);
                    }
                    catch
                    {
                        return;
                    }

                    string head = System.Text.Encoding.ASCII.GetString(request, 0, len);
                    log.Info(head);

                    int index = head.IndexOf('\n');

                    if (index == -1)
                        return;

                    string url = head.Substring(0, index - 1);
                    //url = url.Replace("\r", "");
                    //url = url.Replace("GET ","");
                    //url = url.Replace(" HTTP/1.0", "");
                    //url = url.Replace(" HTTP/1.1", "");

                    //Tracking.AddEvent("HTTPServer", "Get", url, "");
/////////////////////////////////////////////////////////////////
                    if (url.Contains(" /websocket/server"))
                    {
                        using (var writer = new StreamWriter(stream, Encoding.Default))
                        {
                            writer.WriteLine("HTTP/1.1 101 WebSocket Protocol Handshake");
                            writer.WriteLine("Upgrade: WebSocket");
                            writer.WriteLine("Connection: Upgrade");
                            writer.WriteLine("WebSocket-Location: ws://localhost:56781/websocket/server");

                            int start = head.IndexOf("Sec-WebSocket-Key:") + 19;
                            int end = head.IndexOf('\r', start);
                            if (end == -1)
                                end = head.IndexOf('\n', start);
                            string accept = ComputeWebSocketHandshakeSecurityHash09(head.Substring(start, end - start));

                            writer.WriteLine("Sec-WebSocket-Accept: " + accept);
                            writer.WriteLine("Server: Mission Planner");
                            writer.WriteLine("");
                            writer.Flush();

                            while (client.Connected)
                            {
                                while (client.Available > 0)
                                {
                                    var bydata = stream.ReadByte();
                                    Console.Write(bydata.ToString("X2"));

                                    if (bydata == 0x88)
                                        return;
                                }

                                byte[] packet = new byte[1024 * 32];

                                var cs = JsonConvert.SerializeObject(MainV2.comPort.MAV.cs);
                                var wps = JsonConvert.SerializeObject(MainV2.comPort.MAV.wps);

                                foreach (var sendme in new[] { cs,wps })
                                {
                                    int i = 0;
                                    var tosend = sendme.Length;
                                    packet[i++] = 0x81; // fin - utf
                                    
                                    if (tosend <= 125)
                                    {
                                        packet[i++] = (byte)(tosend);
                                    }
                                    else
                                    {
                                        packet[i++] = 126; // nomask -  2 byte length
                                        packet[i++] = (byte)(tosend >> 8);
                                        packet[i++] = (byte)(tosend & 0xff);
                                    }
                                    
                                    foreach (char ch in sendme)
                                    {
                                        packet[i++] = (byte)ch;
                                    }

                                    stream.Write(packet, 0, i);
                                    stream.Flush();
                                }

                                Thread.Sleep(200);
                            }
                        }
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.Contains(" /websocket/raw"))
                    {
                        using (var writer = new StreamWriter(stream, Encoding.Default))
                        {
                            writer.WriteLine("HTTP/1.1 101 WebSocket Protocol Handshake");
                            writer.WriteLine("Upgrade: WebSocket");
                            writer.WriteLine("Connection: Upgrade");
                            writer.WriteLine("WebSocket-Location: ws://localhost:56781/websocket/raw");

                            int start = head.IndexOf("Sec-WebSocket-Key:") + 19;
                            int end = head.IndexOf('\r', start);
                            if (end == -1)
                                end = head.IndexOf('\n', start);
                            string accept = ComputeWebSocketHandshakeSecurityHash09(head.Substring(start, end - start));

                            writer.WriteLine("Sec-WebSocket-Accept: " + accept);
                            if(head.Contains("Sec-WebSocket-Protocol:"))
                                writer.WriteLine("Sec-WebSocket-Protocol: binary");
                            writer.WriteLine("Server: Mission Planner");
                            writer.WriteLine("");
                            writer.Flush();

                            EventHandler<MAVLink.MAVLinkMessage> action = null;
                            action = (sender, message) =>
                            {
                                var sendme = message.buffer;
                                try
                                {
                                    byte[] packet = new byte[1024 * 32];

                                    int i = 0;
                                    var tosend = sendme.Length;
                                    packet[i++] = 0x82; // fin - data

                                    if (tosend <= 125)
                                    {
                                        packet[i++] = (byte) (tosend);
                                    }
                                    else
                                    {
                                        packet[i++] = 126; // nomask -  2 byte length
                                        packet[i++] = (byte) (tosend >> 8);
                                        packet[i++] = (byte) (tosend & 0xff);
                                    }

                                    foreach (char ch in sendme)
                                    {
                                        packet[i++] = (byte) ch;
                                    }

                                    stream.WriteAsync(packet, 0, i);
                                    stream.FlushAsync();
                                }
                                catch
                                {
                                    ((MAVLinkInterface)sender).OnPacketReceived -= action;
                                    stream.Close();
                                    client.Close();
                                }
                            };

                            MainV2.comPort.OnPacketReceived += action;

                            while (client.Connected)
                            {
                                while (client.Available > 0)
                                {
                                    var bydata = stream.ReadByte();
                                    Console.Write(bydata.ToString("X2"));

                                    if (bydata == 0x88)
                                        return;
                                }

                                Thread.Sleep(200);
                            }

                            MainV2.comPort.OnPacketReceived -= action;
                        }
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.Contains(" /georefnetwork.kml"))
                    {
                        byte[] buffer = Encoding.ASCII.GetBytes(georefkml);

                        string header =
                            "HTTP/1.1 200 OK\r\nServer: here\r\nKeep-Alive: timeout=15, max=100\r\nConnection: Keep-Alive\r\nCache-Control: no-cache\r\nContent-Type: application/vnd.google-earth.kml+xml\r\nX-Pad: avoid browser bug\r\nContent-Length: " +
                            buffer.Length + "\r\n\r\n";
                        byte[] temp = asciiEncoding.GetBytes(header);
                        stream.Write(temp, 0, temp.Length);

                        stream.Write(buffer, 0, buffer.Length);

                        stream.Flush();

                        goto again;

                        //stream.Close();
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.Contains(" /location.kml"))
                    {
                        SharpKml.Dom.Document kml = new SharpKml.Dom.Document();

                        foreach (var mavLinkInterface in MainV2.Comports)
                        {
                            foreach (var MAV in mavLinkInterface.MAVlist)
                            {
                                SharpKml.Dom.Placemark pmplane = new SharpKml.Dom.Placemark();
                                pmplane.Name = "P/Q " + MAV.cs.altasl;

                                pmplane.Visibility = true;

                                SharpKml.Dom.Location loc = new SharpKml.Dom.Location();
                                loc.Latitude = MAV.cs.lat;
                                loc.Longitude = MAV.cs.lng;
                                loc.Altitude = MAV.cs.altasl;

                                if (loc.Altitude < 0)
                                    loc.Altitude = 0.01;

                                SharpKml.Dom.Orientation ori = new SharpKml.Dom.Orientation();
                                ori.Heading = MAV.cs.yaw;
                                ori.Roll = -MAV.cs.roll;
                                ori.Tilt = -MAV.cs.pitch;

                                SharpKml.Dom.Scale sca = new SharpKml.Dom.Scale();

                                sca.X = 2;
                                sca.Y = 2;
                                sca.Z = 2;

                                SharpKml.Dom.Model model = new SharpKml.Dom.Model();
                                model.Location = loc;
                                model.Orientation = ori;
                                model.AltitudeMode = SharpKml.Dom.AltitudeMode.Absolute;
                                model.Scale = sca;

                                SharpKml.Dom.Link link = new SharpKml.Dom.Link();
                                link.Href = new Uri("block_plane_0.dae", UriKind.Relative);

                                model.Link = link;

                                pmplane.Geometry = model;

                                kml.AddFeature(pmplane);
                            }
                        }

                        SharpKml.Dom.LookAt la = new SharpKml.Dom.LookAt()
                        {
                            Altitude = MainV2.comPort.MAV.cs.altasl,
                            Latitude = MainV2.comPort.MAV.cs.lat,
                            Longitude = MainV2.comPort.MAV.cs.lng,
                            Tilt = 80,
                            Heading = MainV2.comPort.MAV.cs.yaw,
                            AltitudeMode = SharpKml.Dom.AltitudeMode.Absolute,
                            Range = 50
                        };

                        if (la.Latitude.Value != 0 && la.Longitude.Value != 0)
                        {
                            kml.Viewpoint = la;
                        }

                        SharpKml.Base.Serializer serializer = new SharpKml.Base.Serializer();
                        serializer.Serialize(kml);

                        byte[] buffer = Encoding.ASCII.GetBytes(serializer.Xml);

                        string header =
                            "HTTP/1.1 200 OK\r\nContent-Type: application/vnd.google-earth.kml+xml\r\nContent-Length: " +
                            buffer.Length + "\r\n\r\n";
                        byte[] temp = asciiEncoding.GetBytes(header);
                        stream.Write(temp, 0, temp.Length);

                        stream.Write(buffer, 0, buffer.Length);

                        goto again;
                    }
                    else if (url.Contains(" /network.kml"))
                    {
                        byte[] buffer = Encoding.ASCII.GetBytes(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<kml xmlns=""http://www.opengis.net/kml/2.2"" xmlns:gx=""http://www.google.com/kml/ext/2.2"" xmlns:kml=""http://www.opengis.net/kml/2.2"" xmlns:atom=""http://www.w3.org/2005/Atom"">
    <Folder>
        <name> Network Links </name>
        <open> 1 </open>
        <NetworkLink>
            <name> View Centered Placemark</name> 
            <open> 1 </open>
            <refreshVisibility> 0 </refreshVisibility>
            <flyToView> 1 </flyToView>
            <Link>
                <href> http://127.0.0.1:56781/location.kml</href>
                <refreshMode> onInterval </refreshMode>
                <refreshInterval> 1 </refreshInterval>
                <viewRefreshTime> 1 </viewRefreshTime>
            </Link>
        </NetworkLink>
        <NetworkLink>
            <name> View Centered Placemark</name> 
            <open> 1 </open>
            <refreshVisibility> 0 </refreshVisibility>
            <flyToView> 0 </flyToView>
            <Link>
                <href> http://127.0.0.1:56781/wps.kml</href>
            </Link>
        </NetworkLink>
    </Folder>
</kml>");

     string header =
                            "HTTP/1.1 200 OK\r\nContent-Type: application/vnd.google-earth.kml+xml\r\nContent-Length: " +
                            buffer.Length + "\r\n\r\n";
                        byte[] temp = asciiEncoding.GetBytes(header);
                        stream.Write(temp, 0, temp.Length);

                        stream.Write(buffer, 0, buffer.Length);

                        stream.Flush();

                        goto again;
                    }
                    else if (url.Contains(" /wps.kml"))
                    {
                        SharpKml.Dom.Document kml = new SharpKml.Dom.Document();

                        SharpKml.Dom.CoordinateCollection coords = new SharpKml.Dom.CoordinateCollection();

                        PointLatLngAlt home = null;
                        // draw track
                        try
                        {
                            foreach (var point in GCSViews.FlightPlanner.instance.pointlist)
                            {
                                if (point == null)
                                    continue;

                                if (point.Tag.ToLower().Contains("home"))
                                    home = point;

                                coords.Add(new SharpKml.Base.Vector(point.Lat, point.Lng, point.Alt));
                            }
                        }
                        catch
                        {
                        }

                        var altmode = SharpKml.Dom.AltitudeMode.Absolute;

                        foreach (var point in GCSViews.FlightPlanner.instance.pointlist)
                        {
                            if (point == null)
                                continue;

                            SharpKml.Dom.Placemark wp = new SharpKml.Dom.Placemark();
                            wp.Name = "WP " + point.Tag + " Alt: " + point.Alt;
                            SharpKml.Dom.Point wppoint = new SharpKml.Dom.Point();
                            wppoint.AltitudeMode = altmode;
                            wppoint.Coordinate = new Vector()
                            {
                                Latitude = point.Lat,
                                Longitude = point.Lng,
                                Altitude = point.Alt
                            };
                            wp.Geometry = wppoint;
                            kml.AddFeature(wp);
                        }

                        SharpKml.Dom.LineString ls = new SharpKml.Dom.LineString();
                        ls.AltitudeMode = altmode;
                        ls.Coordinates = coords;
                        ls.Extrude = false;
                        ls.Tessellate = true;

                        Style style = new Style();
                        style.Id = "yellowLineGreenPoly";
                        style.Line = new LineStyle(new Color32(HexStringToColor("ff00ffff")), 4);

                        Style style2 = new Style();
                        style2.Id = "yellowLineGreenPoly";
                        style2.Line = new LineStyle(new Color32(HexStringToColor("7f00ffff")), 4);

                        // above ground
                        SharpKml.Dom.Placemark pm = new SharpKml.Dom.Placemark()
                        {
                            Geometry = ls,
                            Name = "WPs",
                            StyleSelector = style
                        };

                        kml.AddFeature(pm);

                        // on ground
                        SharpKml.Dom.LineString ls2 = new SharpKml.Dom.LineString();
                        ls2.Coordinates = coords;
                        ls2.Extrude = false;
                        ls2.Tessellate = true;
                        ls2.AltitudeMode = SharpKml.Dom.AltitudeMode.ClampToGround;

                        SharpKml.Dom.Placemark pm2 = new SharpKml.Dom.Placemark()
                        {
                            Geometry = ls2,
                            Name = "onground",
                            StyleSelector = style2
                        };

                        kml.AddFeature(pm2);

                        SharpKml.Base.Serializer serializer = new SharpKml.Base.Serializer();
                        serializer.Serialize(kml);

                        byte[] buffer = Encoding.ASCII.GetBytes(serializer.Xml);

                        string header =
                            "HTTP/1.1 200 OK\r\nContent-Type: application/vnd.google-earth.kml+xml\r\nContent-Length: " +
                            buffer.Length + "\r\n\r\n";
                        byte[] temp = asciiEncoding.GetBytes(header);
                        stream.Write(temp, 0, temp.Length);

                        stream.Write(buffer, 0, buffer.Length);

                        stream.Flush();

                        goto again;

                        //stream.Close();
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.Contains(" /block_plane_0.dae"))
                    {
                        string header = "HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\n\r\n";
                        byte[] temp = asciiEncoding.GetBytes(header);
                        stream.Write(temp, 0, temp.Length);

                        BinaryReader file =
                            new BinaryReader(File.Open("block_plane_0.dae", FileMode.Open, FileAccess.Read,
                                FileShare.Read));
                        byte[] buffer = new byte[1024];
                        while (file.PeekChar() != -1)
                        {
                            int leng = file.Read(buffer, 0, buffer.Length);

                            stream.Write(buffer, 0, leng);
                        }
                        file.Close();
                        stream.Close();
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.Contains(" /hud.html"))
                    {
                        var fi = new FileInfo("hud.html");
                        string header = "HTTP/1.1 200 OK\r\nConnection: close\r\nContent-Type: text/html\r\nContent-Length: " + fi.Length + "\r\n\r\n";
                        byte[] temp = asciiEncoding.GetBytes(header);
                        stream.Write(temp, 0, temp.Length);

                        BinaryReader file =
                            new BinaryReader(File.Open("hud.html", FileMode.Open, FileAccess.Read, FileShare.Read));
                        byte[] buffer = new byte[1024];
                        while (file.PeekChar() != -1)
                        {
                            int leng = file.Read(buffer, 0, buffer.Length);

                            stream.Write(buffer, 0, leng);
                        }
                        file.Close();
                        stream.Close();
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.ToLower().Contains(" /hud.jpg") || url.ToLower().Contains(" /map.jpg") ||
                             url.ToLower().Contains(" /both.jpg"))
                    {
                        refreshmap();

                        string header =
                            "HTTP/1.1 200 OK\r\nContent-Type: multipart/x-mixed-replace;boundary=PLANNER\r\n\r\n--PLANNER\r\n";
                        byte[] temp = asciiEncoding.GetBytes(header);
                        stream.Write(temp, 0, temp.Length);

                        while (client.Connected)
                        {
                            System.Threading.Thread.Sleep(200); // 5hz
                            byte[] data = null;

                            if (url.ToLower().Contains("hud"))
                            {
                                GCSViews.FlightData.myhud.streamjpgenable = true;
                                data = GCSViews.FlightData.myhud.streamjpg.ToArray();
                            }
                            else if (url.ToLower().Contains("map"))
                            {
                                data = GetControlJpegRaw(GCSViews.FlightData.mymap);
                            }
                            else
                            {
                                GCSViews.FlightData.myhud.streamjpgenable = true;
                                Image img1 = Image.FromStream(GCSViews.FlightData.myhud.streamjpg);
                                Image img2 = GetControlJpeg(GCSViews.FlightData.mymap);
                                int bigger = img1.Height > img2.Height ? img1.Height : img2.Height;
                                Image imgout = new Bitmap(img1.Width + img2.Width, bigger);

                                Graphics grap = Graphics.FromImage(imgout);

                                grap.DrawImageUnscaled(img1, 0, 0);
                                grap.DrawImageUnscaled(img2, img1.Width, 0);

                                MemoryStream streamjpg = new MemoryStream();
                                imgout.Save(streamjpg, System.Drawing.Imaging.ImageFormat.Jpeg);
                                data = streamjpg.ToArray();
                            }

                            header = "Content-Type: image/jpeg\r\nContent-Length: " + data.Length + "\r\n\r\n";
                            temp = asciiEncoding.GetBytes(header);
                            stream.Write(temp, 0, temp.Length);

                            stream.Write(data, 0, data.Length);

                            header = "\r\n--PLANNER\r\n";
                            temp = asciiEncoding.GetBytes(header);
                            stream.Write(temp, 0, temp.Length);
                        }
                        GCSViews.FlightData.myhud.streamjpgenable = false;
                        stream.Close();
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.Contains(" /guided?"))
                    {
                        //http://127.0.0.1:56781/guided?lat=-34&lng=117.8&alt=30

                        Regex rex = new Regex(@"lat=([\-\.0-9]+)&lng=([\-\.0-9]+)&alt=([\.0-9]+)",
                            RegexOptions.IgnoreCase);

                        Match match = rex.Match(url);

                        if (match.Success)
                        {
                            Locationwp gwp = new Locationwp()
                            {
                                lat = double.Parse(match.Groups[1].Value),
                                lng = double.Parse(match.Groups[2].Value),
                                alt = float.Parse(match.Groups[3].Value)
                            };
                            try
                            {
                                MainV2.comPort.setGuidedModeWP(gwp);
                            }
                            catch
                            {
                            }

                            string header = "HTTP/1.1 200 OK\r\n\r\nSent Guide Mode Wp";
                            byte[] temp = asciiEncoding.GetBytes(header);
                            stream.Write(temp, 0, temp.Length);
                        }
                        else
                        {
                            string header = "HTTP/1.1 200 OK\r\n\r\nFailed Guide Mode Wp";
                            byte[] temp = asciiEncoding.GetBytes(header);
                            stream.Write(temp, 0, temp.Length);
                        }
                        stream.Close();
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.ToLower().Contains("post /guide"))
                    {
                        Regex rex = new Regex(@"lat"":([\-\.0-9]+),""lon"":([\-\.0-9]+),""alt"":([\.0-9]+)",
                            RegexOptions.IgnoreCase);

                        Match match = rex.Match(head);

                        if (match.Success)
                        {
                            Locationwp gwp = new Locationwp()
                            {
                                lat = double.Parse(match.Groups[1].Value),
                                lng = double.Parse(match.Groups[2].Value),
                                alt = float.Parse(match.Groups[3].Value)
                            };
                            try
                            {
                                MainV2.comPort.setGuidedModeWP(gwp);
                            }
                            catch
                            {
                            }

                            string header = "HTTP/1.1 200 OK\r\n\r\nSent Guide Mode Wp";
                            byte[] temp = asciiEncoding.GetBytes(header);
                            stream.Write(temp, 0, temp.Length);
                        }
                        else
                        {
                            string header = "HTTP/1.1 200 OK\r\n\r\nFailed Guide Mode Wp";
                            byte[] temp = asciiEncoding.GetBytes(header);
                            stream.Write(temp, 0, temp.Length);
                        }
                        stream.Close();
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.ToLower().Contains(" /command_long"))
                    {
                        string header = "HTTP/1.1 404 not found\r\nContent-Type: image/jpg\r\n\r\n";
                        byte[] temp = asciiEncoding.GetBytes(header);
                        stream.Write(temp, 0, temp.Length);

                        stream.Close();
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.ToLower().Contains(" /rcoverride"))
                    {
                        string header = "HTTP/1.1 404 not found\r\nContent-Type: image/jpg\r\n\r\n";
                        byte[] temp = asciiEncoding.GetBytes(header);
                        stream.Write(temp, 0, temp.Length);

                        stream.Close();
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.ToLower().Contains(" /get_mission"))
                    {
                        string header = "HTTP/1.1 404 not found\r\nContent-Type: image/jpg\r\n\r\n";
                        byte[] temp = asciiEncoding.GetBytes(header);
                        stream.Write(temp, 0, temp.Length);

                        stream.Close();
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.ToLower().Contains(" /mavlink/"))
                    {
                        /*
        GET /mavlink/ATTITUDE+VFR_HUD+NAV_CONTROLLER_OUTPUT+META_WAYPOINT+GPS_RAW_INT+HEARTBEAT+META_LINKQUALITY+GPS_STATUS+STATUSTEXT+SYS_STATUS?_=1355828718540 HTTP/1.1
        Host: ubuntu:9999
        Connection: keep-alive
        X-Requested-With: XMLHttpRequest
        User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.97 Safari/537.11
        Accept: 
        Referer: http://ubuntu:9999/index.html
        Accept-Encoding: gzip,deflate,sdch
        Accept-Language: en-GB,en-US;q=0.8,en;q=0.6
        Accept-Charset: ISO-8859-1,utf-8;q=0.7,*;q=0.3

        HTTP/1.1 200 OK
        Content-Type: application/json
        Content-Length: 2121
        Date: Thu, 29 Nov 2012 12:13:38 GMT
        Server: ubuntu

        {
        "VFR_HUD": {"msg": {"throttle": 0, "groundspeed": 0.0, "airspeed": 0.0, "climb": 0.0, "mavpackettype": "VFR_HUD", "alt": -0.47999998927116394, "heading": 108}, "index": 687, "time_usec": 0},
        "STATUSTEXT": {"msg": {"mavpackettype": "STATUSTEXT", "severity": 1, "text": "Initialising APM..."}, "index": 2, "time_usec": 0}, 
        "SYS_STATUS": {"msg": {"onboard_control_sensors_present": 4294966287, "load": 0, "battery_remaining": -1, "errors_count4": 0, "drop_rate_comm": 0, "errors_count2": 0, "errors_count3": 0, "errors_comm": 0, "current_battery": -1, "errors_count1": 0, "onboard_control_sensors_health": 4294966287, "mavpackettype": "SYS_STATUS", "onboard_control_sensors_enabled": 4294945807, "voltage_battery": 10080}, "index": 693, "time_usec": 0}, 
        "META_LINKQUALITY": {"msg": {"master_in": 11110, "mav_loss": 0, "mavpackettype": "META_LINKQUALITY", "master_out": 194, "packet_loss": 0.0}, "index": 194, "time_usec": 0},
        "ATTITUDE": {"msg": {"pitchspeed": -0.000976863200776279, "yaw": 1.8878594636917114, "rollspeed": -0.0030046366155147552, "time_boot_ms": 194676, "pitch": -0.09986469894647598, "mavpackettype": "ATTITUDE", "yawspeed": -0.0015030358918011189, "roll": -0.029391441494226456}, "index": 687, "time_usec": 0}, 
        "GPS_RAW_INT": {"msg": {"fix_type": 1, "cog": 0, "epv": 65535, "lon": 0, "time_usec": 0, "eph": 9999, "satellites_visible": 0, "lat": 0, "mavpackettype": "GPS_RAW_INT", "alt": 137000, "vel": 0}, "index": 687, "time_usec": 0}, 
        "HEARTBEAT": {"msg": {"custom_mode": 0, "system_status": 4, "base_mode": 81, "autopilot": 3, "mavpackettype": "HEARTBEAT", "type": 2, "mavlink_version": 3}, "index": 190, "time_usec": 0},
        "GPS_STATUS": {"msg": {"satellite_snr": "", "satellite_azimuth": "", "satellite_prn": "", "satellite_elevation": "", "satellites_visible": 0, "satellite_used": "", "mavpackettype": "GPS_STATUS"}, "index": 2, "time_usec": 0}, 
        "NAV_CONTROLLER_OUTPUT": {"msg": {"wp_dist": 0, "nav_pitch": 0.0, "target_bearing": 0, "nav_roll": 0.0, "aspd_error": 0.0, "alt_error": 0.0, "mavpackettype": "NAV_CONTROLLER_OUTPUT", "xtrack_error": 0.0, "nav_bearing": 0}, "index": 687, "time_usec": 0}}
                      */

                        object[] data = new object[20];

                        if (MainV2.comPort.MAV.getPacket((byte) MAVLink.MAVLINK_MSG_ID.ATTITUDE) != null)
                        {
                            var tmsg = MainV2.comPort.MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.ATTITUDE)
                                .ToStructure<MAVLink.mavlink_attitude_t>();

                            var json = JsonConvert.SerializeObject(tmsg);

                            var name = MAVLink.MAVLINK_MESSAGE_INFOS.GetMessageInfo((uint) MAVLink.MAVLINK_MSG_ID.ATTITUDE).name;


                        }

                        Messagejson message = new Messagejson();

                        if (MainV2.comPort.MAV.getPacket((byte) MAVLink.MAVLINK_MSG_ID.ATTITUDE) != null)
                            message.ATTITUDE = new Message2()
                            {
                                index = 1,
                                msg =
                                    MainV2.comPort.MAV.getPacket((byte)MAVLink.MAVLINK_MSG_ID.ATTITUDE)
                                        .ToStructure<MAVLink.mavlink_attitude_t>()
                            };
                        if (MainV2.comPort.MAV.getPacket((byte)MAVLink.MAVLINK_MSG_ID.VFR_HUD) != null)
                            message.VFR_HUD = new Message2()
                            {
                                index = 1,
                                msg =
                                    MainV2.comPort.MAV.getPacket((byte)MAVLink.MAVLINK_MSG_ID.VFR_HUD)
                                        .ToStructure<MAVLink.mavlink_vfr_hud_t>()
                            };
                        if (MainV2.comPort.MAV.getPacket((byte)MAVLink.MAVLINK_MSG_ID.NAV_CONTROLLER_OUTPUT) != null)
                            message.NAV_CONTROLLER_OUTPUT = new Message2()
                            {
                                index = 1,
                                msg =
                                    MainV2.comPort.MAV.getPacket((byte)MAVLink.MAVLINK_MSG_ID.NAV_CONTROLLER_OUTPUT)
                                        .ToStructure<MAVLink.mavlink_nav_controller_output_t>()
                            };
                        if (MainV2.comPort.MAV.getPacket((byte)MAVLink.MAVLINK_MSG_ID.GPS_RAW_INT) != null)
                            message.GPS_RAW_INT = new Message2()
                            {
                                index = 1,
                                msg =
                                    MainV2.comPort.MAV.getPacket((byte)MAVLink.MAVLINK_MSG_ID.GPS_RAW_INT)
                                        .ToStructure<MAVLink.mavlink_gps_raw_int_t>()
                            };
                        if (MainV2.comPort.MAV.getPacket((byte) MAVLink.MAVLINK_MSG_ID.HEARTBEAT) != null)
                            message.HEARTBEAT = new Message2()
                            {
                                index = 1,
                                msg =
                                    MainV2.comPort.MAV.getPacket((byte) MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
                                        .ToStructure<MAVLink.mavlink_heartbeat_t>()
                            };
                        if (MainV2.comPort.MAV.getPacket((byte) MAVLink.MAVLINK_MSG_ID.GPS_STATUS) != null)
                            message.GPS_STATUS = new Message2()
                            {
                                index = 1,
                                msg =
                                    MainV2.comPort.MAV.getPacket((byte) MAVLink.MAVLINK_MSG_ID.GPS_STATUS)
                                        .ToStructure<MAVLink.mavlink_gps_status_t>()
                            };
                        if (MainV2.comPort.MAV.getPacket((byte) MAVLink.MAVLINK_MSG_ID.STATUSTEXT) != null)
                            message.STATUSTEXT = new Message2()
                            {
                                index = 1,
                                msg =
                                    MainV2.comPort.MAV.getPacket((byte) MAVLink.MAVLINK_MSG_ID.STATUSTEXT)
                                        .ToStructure<MAVLink.mavlink_statustext_t>()
                            };
                        if (MainV2.comPort.MAV.getPacket((byte) MAVLink.MAVLINK_MSG_ID.SYS_STATUS) != null)
                            message.SYS_STATUS = new Message2()
                            {
                                index = 1,
                                msg =
                                    MainV2.comPort.MAV.getPacket((byte)MAVLink.MAVLINK_MSG_ID.SYS_STATUS)
                                        .ToStructure<MAVLink.mavlink_sys_status_t>()
                            };

                        message.META_LINKQUALITY =
                            message.SYS_STATUS =
                                new Message2()
                                {
                                    index = packetindex,
                                    time_usec = 0,
                                    msg =
                                        new META_LINKQUALITY()
                                        {
                                            master_in = (int) MainV2.comPort.MAV.packetsnotlost,
                                            mavpackettype = "META_LINKQUALITY",
                                            master_out = MainV2.comPort.packetcount,
                                            packet_loss = 100 - MainV2.comPort.MAV.cs.linkqualitygcs,
                                            mav_loss = 0
                                        }
                                };

                        packetindex++;

                        string output = JsonConvert.SerializeObject(message);

                        string header = "HTTP/1.1 200 OK\r\nContent-Type: application/json\r\nContent-Length: " +
                                        output.Length + "\r\n\r\n";
                        byte[] temp = asciiEncoding.GetBytes(header);
                        stream.Write(temp, 0, temp.Length);

                        temp = asciiEncoding.GetBytes(output);
                        stream.Write(temp, 0, temp.Length);

                        stream.Flush();

                        goto again;

                        //stream.Close();
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.ToLower().Contains(" /mav/"))
                    {
                        //C:\Users\hog\Desktop\DIYDrones\mavelous\modules\lib\mavelous_web


                        Regex rex = new Regex(@"([^\s]+)\s(.+)\sHTTP/1", RegexOptions.IgnoreCase);

                        Match match = rex.Match(url);

                        if (match.Success)
                        {
                            string fileurl = WebUtility.UrlDecode(match.Groups[2].Value);

                            fileurl = fileurl.Replace("/mav/", "");

                            if (fileurl == "" || fileurl == "/")
                                fileurl = "index.html";

                            if (File.Exists(mavelous_web + fileurl))
                            {
                                string header = "HTTP/1.1 200 OK\r\n";
                                if (fileurl.Contains(".htm"))
                                    header += "Content-Type: text/html\r\n";
                                else if (fileurl.Contains(".js"))
                                    header += "Content-Type: application/x-javascript\r\n";
                                else if (fileurl.Contains(".css"))
                                    header += "Content-Type: text/css\r\n";
                                else
                                    header += "Content-Type: text/plain\r\n";

                                var fileinfo = new FileInfo(mavelous_web + fileurl);

                                var filetime = fileinfo.LastWriteTimeUtc.ToString("ddd, dd MMM yyyy HH:mm:ss") + " GMT";
                                var modified = Regex.Match(head, "If-Modified-Since:(.*)", RegexOptions.IgnoreCase);
                                if (modified.Success && modified.Groups[1].Value.Trim().ToLower() == filetime.ToLower())
                                {
                                    string header2 = "HTTP/1.1 304 not modified\r\nConnection: Keep-Alive\r\n\r\n";
                                    byte[] temp2 = asciiEncoding.GetBytes(header2);
                                    stream.Write(temp2, 0, temp2.Length);

                                    stream.Flush();

                                    goto again;
                                }

                                header += "Connection: keep-alive\r\nLast-Modified: " + filetime + "\r\nContent-Length: " + fileinfo.Length + "\r\n\r\n";


                                byte[] temp = asciiEncoding.GetBytes(header);
                                stream.Write(temp, 0, temp.Length);

                                BinaryReader file =
                                    new BinaryReader(File.Open(mavelous_web + fileurl, FileMode.Open, FileAccess.Read,
                                        FileShare.Read));
                                byte[] buffer = new byte[1024];
                                while (file.BaseStream.Position < file.BaseStream.Length)
                                {
                                    int leng = file.Read(buffer, 0, buffer.Length);

                                    stream.Write(buffer, 0, leng);
                                }
                                file.Close();

                                stream.Flush();

                                goto again;
                            }
                        }

                        /////////////////////////////////////////////////////////////////
                        {
                            string header = "HTTP/1.1 404 not found\r\nConnection: Keep-Alive\r\nContent-Length: 0\r\nContent -Type: text/plain\r\n\r\n";
                            byte[] temp = asciiEncoding.GetBytes(header);
                            stream.Write(temp, 0, temp.Length);

                            stream.Flush();

                            goto again;
                        }
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.ToLower().Contains(".jpg"))
                    {
                        Regex rex = new Regex(@"([^\s]+)\s(.+)\sHTTP/1", RegexOptions.IgnoreCase);

                        Match match = rex.Match(url);

                        if (match.Success)
                        {
                            string fileurl = match.Groups[2].Value;

                            using (Image orig = Image.FromFile(georefimagepath + fileurl))
                            using (Image resi = ResizeImage(orig, new Size(640, 480)))
                            using (MemoryStream memstream = new MemoryStream())
                            {
                                resi.Save(memstream, System.Drawing.Imaging.ImageFormat.Jpeg);

                                memstream.Position = 0;
                                string header =
                                    "HTTP/1.1 200 OK\r\nServer: here\r\nKeep-Alive: timeout=15, max=100\r\nConnection: Keep-Alive\r\nContent-Type: image/jpg\r\nX-Pad: avoid browser bug\r\nContent-Length: " +
                                    memstream.Length + "\r\n\r\n";
                                byte[] temp = asciiEncoding.GetBytes(header);
                                stream.Write(temp, 0, temp.Length);

                                using (BinaryReader file = new BinaryReader(memstream))
                                {
                                    byte[] buffer = new byte[1024];
                                    while (file.BaseStream.Position < file.BaseStream.Length)
                                    {
                                        int leng = file.Read(buffer, 0, buffer.Length);

                                        stream.Write(buffer, 0, leng);
                                    }
                                }
                            }

                            stream.Flush();

                            goto again;

                            //stream.Close();
                        }
                        /////////////////////////////////////////////////////////////////
                        else
                        {
                            string header = "HTTP/1.1 404 not found\r\nContent-Type: image/jpg\r\n\r\n";
                            byte[] temp = asciiEncoding.GetBytes(header);
                            stream.Write(temp, 0, temp.Length);
                        }
                        stream.Close();
                    }
                    /////////////////////////////////////////////////////////////////
                    else if (url.ToLower().Contains(" / "))
                    {
                        Console.WriteLine(url);
                        string header = "HTTP/1.1 200 OK\r\nConnection: close\r\nContent-Type: text/html\r\n\r\n";
                        byte[] temp = asciiEncoding.GetBytes(header);
                        stream.Write(temp, 0, temp.Length);

                        string content = @"
                <a href=/mav/>Mavelous</a>
<a href=/mavlink/>Mavelous traffic</a>
<a href=/hud.jpg>Hud image</a>
<a href=/map.jpg>Map image </a>
<a href=/both.jpg>Map & hud image</a>
<a href=/hud.html>hud html5</a>
<a href=/network.kml>network kml</a>
<a href=/georefnetwork.kml>georef kml</a>

";
                        temp = asciiEncoding.GetBytes(content);
                        stream.Write(temp, 0, temp.Length);
                    }
                    /////////////////////////////////////////////////////////////////
                    else
                    {
                        string header = "HTTP/1.1 404 not found\r\nContent-Type: text/plain\r\n\r\n";
                        byte[] temp = asciiEncoding.GetBytes(header);
                        stream.Write(temp, 0, temp.Length);
                    }

                    stream.Close();
                    log.Info("Close http " + url);
                    client.Close();
                }
                catch (Exception ee)
                {
                    log.Error("Failed http ", ee);
                }
            }
        }


        public static Color HexStringToColor(string hexColor)
        {
            string hc = (hexColor);
            if (hc.Length != 8)
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("hexColor is not exactly 6 digits.");
                return Color.Empty;
            }
            string a = hc.Substring(0, 2);
            string r = hc.Substring(6, 2);
            string g = hc.Substring(4, 2);
            string b = hc.Substring(2, 2);
            Color color = Color.Empty;
            try
            {
                int ai
                    = Int32.Parse(a, System.Globalization.NumberStyles.HexNumber);
                int ri
                    = Int32.Parse(r, System.Globalization.NumberStyles.HexNumber);
                int gi
                    = Int32.Parse(g, System.Globalization.NumberStyles.HexNumber);
                int bi
                    = Int32.Parse(b, System.Globalization.NumberStyles.HexNumber);
                color = Color.FromArgb(ai, ri, gi, bi);
            }
            catch
            {
                // you can choose whether to throw an exception
                //throw new ArgumentException("Conversion failed.");
                return Color.Empty;
            }
            return color;
        }

        public Image GetControlJpeg(Control ctl)
        {
            //var g = ctl.CreateGraphics();

            Bitmap bmp = new Bitmap(ctl.Width, ctl.Height);

            MainV2.instance.Invoke(
                (Action) delegate() { ctl.DrawToBitmap(bmp, new Rectangle(0, 0, ctl.Width, ctl.Height)); });

            return bmp;
        }

        public byte[] GetControlJpegRaw(Control ctl)
        {
            using (Image img = GetControlJpeg(ctl))
            using (MemoryStream streamjpg = new MemoryStream())
            {
                img.Save(streamjpg, System.Drawing.Imaging.ImageFormat.Jpeg);

                byte[] data = streamjpg.ToArray();

                return data;
            }
        }

        public Image ResizeImage(Image image, Size size,
            bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float) size.Width/(float) originalWidth;
                float percentHeight = (float) size.Height/(float) originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int) (originalWidth*percent);
                newHeight = (int) (originalHeight*percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }


        public static String ComputeWebSocketHandshakeSecurityHash09(String secWebSocketKey)
        {
            const String MagicKEY = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            String secWebSocketAccept = String.Empty;

            // 1. Combine the request Sec-WebSocket-Key with magic key.
            String ret = secWebSocketKey + MagicKEY;

            // 2. Compute the SHA1 hash
            using (System.Security.Cryptography.SHA1 sha = new System.Security.Cryptography.SHA1CryptoServiceProvider())
            {
                byte[] sha1Hash = sha.ComputeHash(Encoding.UTF8.GetBytes(ret));

                // 3. Base64 encode the hash
                secWebSocketAccept = Convert.ToBase64String(sha1Hash);
            }
            return secWebSocketAccept;
        }

        void refreshmap()
        {
            Action m = delegate() { GCSViews.FlightData.mymap.Refresh(); };
            MainV2.instance.Invoke(m);
        }


        int packetindex = 0;
        //{"master_in": 11110, "mav_loss": 0, "mavpackettype": "META_LINKQUALITY", "master_out": 194, "packet_loss": 0.0}

        public struct META_LINKQUALITY
        {
            public int master_in;
            public int mav_loss;
            public string mavpackettype;
            public int master_out;
            public double packet_loss;
        }

        public struct Messagejson
        {
            public Message2 VFR_HUD;
            public Message2 STATUSTEXT;
            public Message2 SYS_STATUS;
            public Message2 ATTITUDE;
            public Message2 GPS_RAW_INT;
            public Message2 HEARTBEAT;
            public Message2 GPS_STATUS;
            public Message2 NAV_CONTROLLER_OUTPUT;
            public Message2 META_LINKQUALITY;
        }

        public struct Message2
        {
            public object msg;
            public int index;
            public long time_usec;
        }
    }
}