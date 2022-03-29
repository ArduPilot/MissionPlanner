using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ClipperLib;
using log4net;
using MissionPlanner.Comms;

namespace MissionPlanner.Utilities
{
    public class AutoConnect
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static List<ConnectionInfo> connectionInfos = new List<ConnectionInfo>()
        {
            new ConnectionInfo("Mavlink default port", true, 14550, ProtocolType.Udp, ConnectionFormat.MAVLink,
                Direction.Inbound, ""),
            new ConnectionInfo("Mavlink alt port", true, 14551, ProtocolType.Udp, ConnectionFormat.MAVLink,
                Direction.Inbound, ""),

            new ConnectionInfo("Mavlink sitl port", false, 5760, ProtocolType.Tcp, ConnectionFormat.MAVLink,
                Direction.Outbound, "127.0.0.1"),

            new ConnectionInfo("Video udp 5000 h264", true, 5000, ProtocolType.Udp, ConnectionFormat.Video,
                Direction.Inbound,
                "udpsrc port=5000 buffer-size=90000 ! application/x-rtp ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false"),
            new ConnectionInfo("Video udp 5100 h264", true, 5100, ProtocolType.Udp, ConnectionFormat.Video,
                Direction.Inbound,
                "udpsrc port=5100 buffer-size=90000 ! application/x-rtp ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false"),
            new ConnectionInfo("Video udp 5600 h264", true, 5600, ProtocolType.Udp, ConnectionFormat.Video,
                Direction.Inbound,
                "udpsrc port=5600 buffer-size=90000 ! application/x-rtp ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false"),

            new ConnectionInfo("Video udp 5601 h265", true, 5601, ProtocolType.Udp, ConnectionFormat.Video,
                Direction.Inbound,
                "udpsrc port=5601 buffer-size=90000 ! application/x-rtp ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false"),

            new ConnectionInfo("SkyViper", false, 554, ProtocolType.Tcp, ConnectionFormat.Video, Direction.Outbound,
                "rtspsrc location=rtsp://192.168.99.1/media/stream2 debug=false buffer-mode=1 latency=100 ntp-time-source=3 ! application/x-rtp ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false"),
            new ConnectionInfo("HereLink Wifi", false, 8554, ProtocolType.Tcp, ConnectionFormat.Video, Direction.Outbound,
                "rtspsrc location=rtsp://192.168.43.1:8554/fpv_stream latency=41 udp-reconnect=1 timeout=0 do-retransmission=false ! application/x-rtp ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false"),
            new ConnectionInfo("HereLink GCS", false, 8554, ProtocolType.Tcp, ConnectionFormat.Video, Direction.Outbound,
                "rtspsrc location=rtsp://192.168.0.10:8554/H264Video latency=41 udp-reconnect=1 timeout=0 do-retransmission=false ! application/x-rtp ! decodebin3 ! queue max-size-buffers=1 leaky=2 ! videoconvert ! video/x-raw,format=BGRA ! appsink name=outsink sync=false"),

            new ConnectionInfo("Serial", false, 57600, ProtocolType.Serial, ConnectionFormat.MAVLink,
                Direction.Outbound, ""),

        };

        private static string SettingsName = "AutoConnect";

        public static void Start()
        {
            var config = Settings.Instance[SettingsName];

            if (config == null)
            {
                Settings.Instance[SettingsName] = connectionInfos.ToJSON();
                config = Settings.Instance[SettingsName];
            }

            connectionInfos = config.FromJSON<List<ConnectionInfo>>();

            foreach (var connectionInfo in connectionInfos)
            {
                if (connectionInfo.Enabled == false)
                    continue;

                log.Info(connectionInfo.ToJSON());

                if (connectionInfo.Format == ConnectionFormat.MAVLink)
                {
                    if (connectionInfo.Protocol == ProtocolType.Udp && connectionInfo.Direction == Direction.Inbound)
                    {
                        try
                        {
                            var client = new UdpClient(connectionInfo.Port);
                            client.BeginReceive(clientdataMAVLink, client);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }

                        continue;
                    }

                    if (connectionInfo.Protocol == ProtocolType.Udp && connectionInfo.Direction == Direction.Outbound)
                    {
                        try
                        {
                            // create and set default dest
                            var client = new UdpClient(connectionInfo.ConfigString, connectionInfo.Port);
                            client.SendAsync(new byte[] {0}, 1);
                            client.BeginReceive(clientdataMAVLink, client);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }

                        continue;
                    }

                    if (connectionInfo.Protocol == ProtocolType.Tcp &&
                       connectionInfo.Direction == Direction.Outbound)
                    {
                        try
                        {
                            // try anything already connected
                            Task.Run(() =>
                            {

                                try
                                {
                                    var serial = new TcpSerial();
                                    serial.Host = connectionInfo.ConfigString;
                                    serial.Port = connectionInfo.Port.ToString();
                                    serial.ReadBufferSize = 1024 * 300;
                                    serial.Open();
                                    // sample 1.2seconds
                                    Thread.Sleep(1200);
                                    var btr = serial.BytesToRead;
                                    var buffer = new byte[btr];
                                    serial.Read(buffer, 0, buffer.Length);
                                    //serial.Close();
                                    var parse = new MAVLink.MavlinkParse();
                                    var st = buffer.ToMemoryStream();
                                    while (st.Position < st.Length)
                                    {
                                        var packet = parse.ReadPacket(st);
                                        if (packet != null)
                                        {
                                            if (packet.msgid == (int) MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
                                            {
                                                NewMavlinkConnection?.BeginInvoke(null, serial, null, null);
                                                return;
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }

                            });


                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }

                        continue;
                    }

                    if (connectionInfo.Protocol == ProtocolType.Tcp &&
                       connectionInfo.Direction == Direction.Inbound)
                    {
                        try
                        {
                            // try anything already connected
                            Task.Run(() =>
                            {

                                try
                                {
                                    TcpListener listener = new TcpListener(IPAddress.Any, connectionInfo.Port);
                                    listener.Start();
                                    var client = listener.AcceptTcpClient();
                                    var serial = new TcpSerial();
                                    serial.client = client;
                                    serial.ReadBufferSize = 1024 * 300;
                                    serial.Open();
                                    // sample 1.2seconds
                                    Thread.Sleep(1200);
                                    var btr = serial.BytesToRead;
                                    var buffer = new byte[btr];
                                    serial.Read(buffer, 0, buffer.Length);
                                    //serial.Close();
                                    var parse = new MAVLink.MavlinkParse();
                                    var st = buffer.ToMemoryStream();
                                    while (st.Position < st.Length)
                                    {
                                        var packet = parse.ReadPacket(st);
                                        if (packet != null)
                                        {
                                            if (packet.msgid == (int)MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
                                            {
                                                NewMavlinkConnection?.BeginInvoke(null, serial, null, null);
                                                return;
                                            }
                                        }
                                    }
                                }
                                catch
                                {
                                }

                            });


                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }

                        continue;
                    }

                    if (connectionInfo.Protocol == ProtocolType.Serial &&
                        connectionInfo.Direction == Direction.Outbound)
                    {
                        try
                        {
                            // try anything already connected
                            Task.Run(() =>
                            {
                                Parallel.ForEach(SerialPort.GetPortNames(), port =>
                                {
                                    try
                                    {
                                        var serial = new SerialPort(port, connectionInfo.Port);
                                        serial.ReadBufferSize = 1024 * 300;
                                        serial.Open();
                                        // sample 1.2seconds
                                        Thread.Sleep(1200);
                                        var btr = serial.BytesToRead;
                                        var buffer = new byte[btr];
                                        serial.Read(buffer, 0, buffer.Length);
                                        serial.Close();
                                        var parse = new MAVLink.MavlinkParse();
                                        var st = buffer.ToMemoryStream();
                                        while (st.Position < st.Length)
                                        {
                                            var packet = parse.ReadPacket(st);
                                            if (packet != null)
                                            {
                                                if (packet.msgid == (int) MAVLink.MAVLINK_MSG_ID.HEARTBEAT)
                                                {
                                                    NewMavlinkConnection?.BeginInvoke(null, serial, null, null);
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                });
                            });


                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }

                        continue;
                    }
                }
                else if (connectionInfo.Format == ConnectionFormat.Video)
                {
                    if (connectionInfo.Protocol == ProtocolType.Udp && connectionInfo.Direction == Direction.Inbound)
                    {
                        try
                        {
                            var client = new UdpClient(connectionInfo.Port, AddressFamily.InterNetwork);
                            client.BeginReceive(clientdataVideo, client);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }

                        continue;
                    }

                    if (connectionInfo.Protocol == ProtocolType.Tcp && connectionInfo.Direction == Direction.Inbound)
                    {
                        try
                        {
                            var client = new TcpListener(IPAddress.Any, connectionInfo.Port);
                            client.Start();
                            client.BeginAcceptTcpClient(clientdatatcpvideo, client);
                        }
                        catch (Exception ex)
                        {
                            log.Error(ex);
                        }

                        continue;
                    }

                    if (connectionInfo.Direction == Direction.Outbound)
                    {
                        NewVideoStream?.BeginInvoke(null,
                            connectionInfos.First(a => a == connectionInfo).ConfigString, null, null);
                        continue;
                    }
                }
            }
        }

        private static void clientdatatcpvideo(IAsyncResult ar)
        {
            var client = ((TcpClient)ar.AsyncState);

            if (client == null || client.Client == null)
                return;

            try
            {
                var port = ((IPEndPoint)client.Client.LocalEndPoint).Port;

                if (client != null)
                    client.Close();

                NewVideoStream?.BeginInvoke(null,
                    connectionInfos.First(a => a.Enabled && a.Port == port && a.Protocol == ProtocolType.Tcp && a.Direction == Direction.Inbound)
                        .ConfigString, null, null);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private static void clientdataVideo(IAsyncResult ar)
        {
            var client = ((UdpClient) ar.AsyncState);

            if (client == null || client.Client == null)
                return;
            try
            {
                var port = ((IPEndPoint) client.Client.LocalEndPoint).Port;

                if (client != null)
                    client.Close();

                NewVideoStream?.BeginInvoke(null,
                    connectionInfos.First(a => a.Enabled && a.Port == port && a.Protocol == ProtocolType.Udp)
                        .ConfigString, null, null);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static event EventHandler<string> NewVideoStream;
        public static event EventHandler<ICommsSerial> NewMavlinkConnection;

        public static void RaiseNewVideoStream(object E, string s) => NewVideoStream?.Invoke(E, s);
        public static void RaiseNewMavlinkConnection(object E, ICommsSerial s) => NewMavlinkConnection?.Invoke(E, s);

        private static void clientdataMAVLink(IAsyncResult ar)
        {
            var client = ((UdpClient) ar.AsyncState);

            if (client == null || client.Client == null)
                return;
            try
            {
                var port = ((IPEndPoint) client.Client.LocalEndPoint).Port;

                var udpclient = new Comms.UdpSerial(client);

                udpclient.Port = port.ToString();

                NewMavlinkConnection?.BeginInvoke(null, udpclient, null, null);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void Stop()
        {

        }

        public class ConnectionInfo
        {
            public string Label;
            public bool Enabled = true;
            public int Port;
            public ProtocolType Protocol;
            public ConnectionFormat Format;
            public Direction Direction;
            public string ConfigString;

            public ConnectionInfo(string label, bool enabled, int port, ProtocolType protocol, ConnectionFormat format,
                Direction direction, string configstring)
            {
                Label = label;
                Enabled = enabled;
                Port = port;
                Protocol = protocol;
                Format = format;
                Direction = direction;
                ConfigString = configstring;
            }
        }

        public enum ProtocolType
        {
            Tcp,
            Udp,
            Serial
        }

        public enum Direction
        {
            Inbound,
            Outbound
        }

        public enum ConnectionFormat
        {
            MAVLink,
            Video,
        }
    }
}