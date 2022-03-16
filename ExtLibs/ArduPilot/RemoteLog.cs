using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using MissionPlanner.Utilities;

namespace MissionPlanner.ArduPilot
{
    public class RemoteLog : IDisposable
    {
        private string logfilename;
        private Stream logfilestream;
        private uint seq;
        private MAVLinkInterface port;
        private bool running = false;
        private static Dictionary<string, RemoteLog> loggers = new Dictionary<string, RemoteLog>();

        public static RemoteLog StartRemoteLog(MAVLinkInterface port, byte sysid, byte compid)
        {
            var id = port.GetHashCode() + "-" + sysid + "-" + compid;

            if (loggers.ContainsKey(id))
                loggers[id].Stop(sysid, compid);

            var rem = new RemoteLog();
            rem.Start(port, sysid, compid);
            loggers[id] = rem;

            return rem;
        }

        public void Start(MAVLinkInterface port, byte sysid, byte compid)
        {
            if (port == null) throw new ArgumentNullException(nameof(port));

            if (running)
                Stop(sysid, compid);

            this.port = port;

            var dt = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            logfilename = Settings.GetDefaultLogDir() + Path.DirectorySeparatorChar + dt + ".bin";

            logfilestream =
                new BufferedStream(new FileStream(logfilename, FileMode.Create, FileAccess.ReadWrite, FileShare.Read));

            port.OnPacketReceived += Port_OnPacketReceived;

            var startpacket = new MAVLink.mavlink_remote_log_block_status_t(
                (uint) MAVLink.MAV_REMOTE_LOG_DATA_BLOCK_COMMANDS.MAV_REMOTE_LOG_DATA_BLOCK_START, sysid, compid,
                (byte) MAVLink.MAV_REMOTE_LOG_DATA_BLOCK_STATUSES.MAV_REMOTE_LOG_DATA_BLOCK_ACK);
            port.sendPacket(startpacket, sysid, compid);

            running = true;
        }

        private void Port_OnPacketReceived(object sender, MAVLink.MAVLinkMessage message)
        {
            if (message.msgid == (uint) MAVLink.MAVLINK_MSG_ID.REMOTE_LOG_DATA_BLOCK)
            {
                var data = (MAVLink.mavlink_remote_log_data_block_t) message.data;

                seq = data.seqno;

                if (logfilestream.Position != data.seqno * 200)
                    logfilestream.Seek(data.seqno * 200, SeekOrigin.Begin);

                logfilestream.Write(data.data, 0, data.data.Length);

                var resp = new MAVLink.mavlink_remote_log_block_status_t(data.seqno, message.sysid, message.compid,
                    (byte) MAVLink.MAV_REMOTE_LOG_DATA_BLOCK_STATUSES.MAV_REMOTE_LOG_DATA_BLOCK_ACK);

                port.sendPacket(resp, message.sysid, message.compid);
            }
        }

        public void Stop(byte sysid, byte compid)
        {
            if (port == null) throw new ArgumentNullException(nameof(port));

            var stoppacket = new MAVLink.mavlink_remote_log_block_status_t(
                (uint) MAVLink.MAV_REMOTE_LOG_DATA_BLOCK_COMMANDS.MAV_REMOTE_LOG_DATA_BLOCK_STOP, sysid, compid,
                (byte) MAVLink.MAV_REMOTE_LOG_DATA_BLOCK_STATUSES.MAV_REMOTE_LOG_DATA_BLOCK_ACK);
            port.sendPacket(stoppacket, sysid, compid);

            port.OnPacketReceived -= Port_OnPacketReceived;

            try
            {
                if (logfilestream != null)
                    logfilestream.Close();
            }
            catch
            {
            }

            running = false;
        }

        public void Dispose()
        {
            logfilestream?.Dispose();

            if (port != null)
                port.OnPacketReceived -= Port_OnPacketReceived;
        }
    }
}
