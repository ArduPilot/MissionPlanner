using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using System.Net.NetworkInformation;
using MissionPlanner;
using MissionPlanner.Utilities;
using MissionPlanner.GCSViews;
using System.Net;
using System.Net.Sockets;
using System.Web.Script.Serialization;

namespace MissionPlanner.Plugin
{
    class TcpStream : Plugin
    {

        /// 
        ///     Link to server's Github:
        ///     https://github.com/RReivax/ODM
        /// 
        ///     Description:
        ///     This class allow Mission Planner to connect into a distant server, and
        ///     send drone's data to this server. Multiple drones can connect to this server.
        ///     For more information about how the server works, consult the wiki here:
        ///     https://github.com/RReivax/ODM/wiki
        ///     
        ///     This class send a Json containing GPS posistion data, time and a username.
        ///


        public static int serverPort { get; set; }
        public static String serverIP { get; set; }
        public static String serverUsername { get; set; }
        public static bool IPLoaded { get; set; } = false;

        public static Socket client { get; set; }

        private static Timer timer;
        private static bool timerTimeout = true;
        public static DateTime timeStart { get; set; }
        private static DateTime realTime;
        private static TimeSpan timeFromBegin;

        private Object thisLock = new Object();

        public static ServerView serverWindow { get; set; } = null;

        private static dataDrone tempObject;



        /// <summary>
        /// Class who will be converted in JSON and sent to the server
        /// </summary>
        public class dataDrone
        {
            public String name { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public double Altitude { get; set; }
            public String date { get; set; }
        }

        public override string Author
        {
            get
            {
                return "Robin DADE";
            }
        }

        public override string Name
        {
            get
            {
                return "Server Integration";
            }
        }

        public override string Version
        {
            get
            {
                return "1.0";
            }
        }
        
        
        public override bool Exit()
        {
            throw new NotImplementedException();
        }

        public override bool Init()
        {
            throw new NotImplementedException();
        }

        public override bool Loaded()
        {
            tempObject = new dataDrone();
            throw new NotImplementedException();
        }


        /// <summary>
        /// Ping the server to check if it exist
        /// </summary>
        public static bool PingHost(string IPAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(IPAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Discard PingExceptions and return false;
            }
            return pingable;
        }


        /// <summary>
        /// Start a TCP connection with the distant server
        /// </summary>
        public static void changeIP(String ip, String username)
        {
            if (PingHost(ip))
            {

                try
                {
                    MissionPlanner.AsynchronousClient.StartClient(ip);
                
                    serverIP = ip;
                    IPLoaded = true;
                    if (username != null)
                        serverUsername = username;
                    else
                        serverUsername = "Username";

                    serverWindow.getConnectionIndice().Text = "Connected";
                    serverWindow.getConnectionIndice().ForeColor = System.Drawing.Color.Green;

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    IPLoaded = false;
                    serverIP = null;
                    serverPort = 0;
                }
            }
            else
            {
                #if DEBUG
                Console.Write("Unable to connect to server to:" + ip + "\n");
                #endif
            }
        }


        /// <summary>
        /// Initialyse a timer to get real time from external website
        /// </summary>
        public static void InitTimer()
        {
            timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1000; // in miliseconds
            timer.Start();
        }

        private static void timer_Tick(object sender, EventArgs e)
        {
            timerTimeout = true;
        }

        public static void setrealTime()
        {
            realTime = GetNetworkTime();
            timeStart = realTime;
        }

        /// <summary>
        /// Get the network time from a windows website
        /// </summary>
        public static DateTime GetNetworkTime()
        {
            //default Windows time server
            const string ntpServer = "time.windows.com";

            // NTP message size - 16 bytes of the digest (RFC 2030)
            var ntpData = new byte[48];

            //Setting the Leap Indicator, Version Number and Mode values
            ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

            var addresses = Dns.GetHostEntry(ntpServer).AddressList;

            //The UDP port number assigned to NTP is 123
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            //NTP uses UDP
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.Connect(ipEndPoint);

            //Stops code hang if NTP is blocked
            socket.ReceiveTimeout = 3000;

            socket.Send(ntpData);
            try
            {
                socket.Receive(ntpData);
            }
            catch
            {
                #if DEBUG
                Console.Write("Can't receive real time\n");
                #endif
            }
            socket.Close();

            //Offset to get to the "Transmit Timestamp" field (time at which the reply 
            //departed the server for the client, in 64-bit timestamp format."
            const byte serverReplyTime = 40;

            //Get the seconds part
            ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

            //Get the seconds fraction
            ulong fractPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

            //Convert From big-endian to little-endian
            intPart = SwapEndianness(intPart);
            fractPart = SwapEndianness(fractPart);

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            //**UTC** time
            var networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);

            return networkDateTime.ToLocalTime();
        }

        private static uint SwapEndianness(ulong x)
        {
            return (uint)(((x & 0x000000ff) << 24) +
                           ((x & 0x0000ff00) << 8) +
                           ((x & 0x00ff0000) >> 8) +
                           ((x & 0xff000000) >> 24));
        }
        


        /// <summary>
        /// Main loop
        /// </summary>
        public override bool Loop()
        {

            if (timerTimeout)
            {

                ///
                /// Calcute real time
                ///
                timeFromBegin = DateTime.Now.Subtract(timeStart);
                realTime = realTime.Add(timeFromBegin);
                timeStart = DateTime.Now;

                if (IPLoaded)
                {
                    timerTimeout = false;
                        ///
                        /// Create new object, which will be converted into JSON format,
                        /// and sent to the server
                        ///
                        var tempObject = new dataDrone
                        {
                            Latitude = MainV2.comPort.MAV.cs.lat,
                            Longitude = MainV2.comPort.MAV.cs.lng,
                            Altitude = MainV2.comPort.MAV.cs.alt,
                            name = serverUsername,
                            date = realTime.ToString()
                        };

                        var json = new JavaScriptSerializer().Serialize(tempObject);

                        #if DEBUG
                        Console.WriteLine("Client:\tMessage Send:" + json + "\n");
                        #endif

                        ///
                        /// Send data to server
                        /// 
                        try
                        {
                            AsynchronousClient.Send(client, json);
                        }
                        catch (Exception e)
                        {
                            #if DEBUG
                            Console.WriteLine("Send failed:" + e + "\n");
                            #endif
                            serverIP = null;
                            serverPort = 0;
                            client.Close();
                            IPLoaded = false;
                        }
                        
                }
            }
            return base.Loop();
        }
    }
}
