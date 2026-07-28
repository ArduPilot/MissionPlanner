using log4net;
using MissionPlanner.Comms;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities
{
    /// <summary>
    /// Class to interact with Septentrio receivers.
    /// </summary>
    public class Septentrio
    {
        /// <summary>
        /// An exception representing a missing acknowledgement.
        /// </summary>
        public class FailedAckException : Exception { }

        /// <summary>
        /// Cache of the last detected receiver port name. 
        /// </summary>
        public static string LastDetectedPort { get; set; } = "USB1+USB2+COM1+COM2";

        /// <summary>
        /// Selection of messages from what MSM level to output.
        /// </summary>
        public enum RTCMLevel
        {
            /// <summary>
            /// MSM3 messages.
            /// </summary>
            Lite = 3,

            /// <summary>
            /// MSM4 messages.
            /// </summary>
            Basic = 4,

            /// <summary>
            /// MSM7 messages.
            /// </summary>
            Full = 7,
        }

        /// <summary>
        /// Flags to choose what constellations to output RTCM messages for.
        /// </summary>
        [Flags]
        public enum RTCMSignals
        {
            /// <summary>
            /// Output RTCM messages for no constellation.
            /// </summary>
            None =    0b_0000_0000,

            /// <summary>
            /// Output RTCM messages for the GPS constellation.
            /// </summary>
            Gps =     0b_0000_0001,

            /// <summary>
            /// Output RTCM messages for the GLONASS constellation.
            /// </summary>
            Glonass = 0b_0000_0010,

            /// <summary>
            /// Output RTCM messages for the BeiDou constellation.
            /// </summary>
            Beidou =  0b_0000_0100,

            /// <summary>
            /// Output RTCM messages for the Galileo constellation.
            /// </summary>
            Galileo = 0b_0000_1000,
        }

        /// <summary>
        /// Configure the receiver connected on `receiverPort` as a base station.
        /// </summary>
        /// <exception cref="FailedAckException" />
        /// <exception cref="IOException" />
        public static async Task ConfigureBaseReceiver(ICommsSerial receiverPort)
        {
            await receiverPort.BaseStream.FlushAsync();

            receiverPort.BaudRate = 115200;
            receiverPort.ReadTimeout = 200;
            receiverPort.WriteTimeout = 200;
 
            string activePort = await ConfigureBaudAndDetectPort(receiverPort);
            log.Info($"Detected active port: {activePort}");

            await SendAck(receiverPort, "setPVTMode,Static,All,Auto\n");
            // Append +RTCMv3 output to preserve any existing streams already configured on this port
            await SendAck(receiverPort, $"setDataInOut,{activePort},Auto,+RTCMv3\n");
        }

        /// <summary>
        /// Set the fixed base position for the receiver.
        /// </summary>
        /// <exception cref="FailedAckException" />
        public static async Task SetBasePosition(ICommsSerial receiverPort, float latitude, float longitude, float altitude)
        {
            await receiverPort.BaseStream.FlushAsync();

            await SendAck(receiverPort, "setStaticPosGeodetic,Geodetic1," + latitude.ToString("0.000000000", System.Globalization.CultureInfo.InvariantCulture) + "," + longitude.ToString("0.000000000", System.Globalization.CultureInfo.InvariantCulture) + "," + altitude.ToString("0.0000", System.Globalization.CultureInfo.InvariantCulture) + ",WGS84" + "\n");
            await SendAck(receiverPort, "setPVTMode,Static,,Geodetic1\n");
        }

        /// <summary>
        /// Set the base position for the receiver to be automatically calculated.
        /// </summary>
        /// <exception cref="FailedAckException" />
        public static async Task SetAutoBasePosition(ICommsSerial receiverPort)
        {
            await receiverPort.BaseStream.FlushAsync();

            await SendAck(receiverPort, "setPVTMode,Static,,auto\n");
        }

        /// <summary>
        /// Configure the baud rate of the serial port and detect the active port on the receiver side. In case the receiver is connected over serial, this automatically sets the correct baud rate.
        /// </summary>
        /// <returns>The detected active port name or the default port string if auto-detection falls back.</returns>
        /// <exception cref="FailedAckException" />
        /// <exception cref="IOException" />
        private static async Task<string> ConfigureBaudAndDetectPort(ICommsSerial receiverPort)
        {
            bool receiverAcknowledged = false;

            // Default fallback ports we expect the receiver to be connected to
            string detectedPort = LastDetectedPort;

            // All the baud rates we expect the receiver could be running at
            var bauds = new[] { receiverPort.BaudRate, 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800 };

            foreach (var baud in bauds)
            {
                receiverPort.BaudRate = baud;

                // Try to set the port settings on a best effort basis
                try
                {
                    // Detect active port directly on established connection
                    detectedPort = await DetectPort(receiverPort);

                    // Skip setCOMSettings command for USB connections
                    if (!detectedPort.Contains("USB"))
                    {
                        await SendAck(receiverPort, $"setCOMSettings,{detectedPort},baud{DefaultBaudrate},bits8,No,bit1,none\n");
                    }

                    receiverAcknowledged = true;
                    break;
                } catch { }
            }

            if (!receiverAcknowledged)
                throw new FailedAckException();

            receiverPort.BaudRate = DefaultBaudrate;
            return detectedPort;
        }

        /// <summary>
        /// Set the level of generated RTCM messages.
        /// </summary>
        /// <exception cref="FailedAckException" />
        public static Task SetEnabledRTCM(ICommsSerial receiverPort, RTCMLevel level, RTCMSignals signals)
        {
            string activePort = LastDetectedPort;

            int messageLevel;
            string messages = "RTCM1006+RTCM1033+RTCM1230";
            
            switch (level)
            {
                case RTCMLevel.Lite:
                    messageLevel = 3;
                    break;
                case RTCMLevel.Basic:
                    messageLevel = 4;
                    break;
                case RTCMLevel.Full:
                default:
                    messageLevel = 7;
                    break;
            }

            if ((signals & RTCMSignals.Gps) == RTCMSignals.Gps)
                messages += "+RTCM107" + messageLevel;
            if ((signals & RTCMSignals.Glonass) == RTCMSignals.Glonass)
                messages += "+RTCM108" + messageLevel;
            if ((signals & RTCMSignals.Galileo) == RTCMSignals.Galileo)
                messages += "+RTCM109" + messageLevel;
            if ((signals & RTCMSignals.Beidou) == RTCMSignals.Beidou)
                messages += "+RTCM112" + messageLevel;

            return SendAck(receiverPort, $"setRTCMv3Output,{activePort},{messages}\n");
        }

        /// <summary>
        /// Detect the active port on the receiver side. 
        /// </summary>
        /// <returns>The detected active port name or the default port string if auto-detection falls back.</returns>
        /// <exception cref="IOException" />
        public static async Task<string> DetectPort(ICommsSerial receiverPort)
        {
            // Clear any stale unread incoming bytes
            if (receiverPort.BytesToRead > 0)
            {
                receiverPort.DiscardInBuffer();
            }

            await receiverPort.BaseStream.FlushAsync();

            // Send ping command (gecm / getEchoMessage)
            byte[] pingBytes = Encoding.ASCII.GetBytes("gecm\n");
            await receiverPort.BaseStream.WriteAsync(pingBytes, 0, pingBytes.Length);

            Stopwatch sw = Stopwatch.StartNew();
            StringBuilder buffer = new StringBuilder();
            // 256 bytes should be enough to read the prompt and the active port name
            byte[] readBuf = new byte[256];

            while (sw.ElapsedMilliseconds < AckTimeout)
            {
                if (receiverPort.BytesToRead > 0)
                {
                    int bytesRead = await receiverPort.BaseStream.ReadAsync(readBuf, 0, readBuf.Length);
                    if (bytesRead > 0)
                    {
                        // Clean up non-printable characters or null bytes if needed
                        string incoming = Encoding.ASCII.GetString(readBuf, 0, bytesRead);
                        buffer.Append(incoming);

                        string currentText = buffer.ToString();
                        int promptIdx = currentText.IndexOf('>');

                        if (promptIdx >= 3)
                        {
                            // Look back 4 characters before '>'
                            int start = Math.Max(0, promptIdx - 4);
                            string candidate = currentText.Substring(start, promptIdx - start);

                            // Limit the candidate to serial and USB port names (COMx or USBx)
                            if (candidate.Contains("COM"))
                            {
                                int idx = candidate.IndexOf("COM");
                                LastDetectedPort = candidate.Substring(idx);
                                return LastDetectedPort;
                            }
                            else if (candidate.Contains("USB"))
                            {
                                int idx = candidate.IndexOf("USB");
                                LastDetectedPort = candidate.Substring(idx);
                                return LastDetectedPort;
                            }
                        }
                    }
                }
                // Prevent busy-waiting and wait for serial data 
                await Task.Delay(20);
            }

            // Default fallback if no active port is detected within the timeout
            return LastDetectedPort;
        }

        /// <summary>
        /// Set the interval of generated RTCM messages.
        /// </summary>
        /// <exception cref="FailedAckException" />
        public static Task SetRTCMInterval(ICommsSerial receiverPort, float interval)
        {
            return SendAck(receiverPort, "setRTCMv3Interval,MSM3+MSM4+MSM7+RTCM1005|6+RTCM1033+RTCM1230," + interval.ToString("0.0", System.Globalization.CultureInfo.InvariantCulture) + "\n");
        }

        /// <summary>
        /// Send a command to the receiver on `receiverPort` and confirm its acknowledgement.
        /// </summary>
        /// <exception cref="FailedAckException" />
        private static async Task SendAck(ICommsSerial receiverPort, String command)
        {
            Stopwatch sw = new Stopwatch();
            string line;
            StreamReader reader = new StreamReader(receiverPort.BaseStream, System.Text.Encoding.ASCII);

            await receiverPort.BaseStream.FlushAsync();
            await receiverPort.BaseStream.WriteAsync(System.Text.Encoding.ASCII.GetBytes(command), 0, command.Length);

            // From https://stackoverflow.com/questions/45756279/how-to-set-a-timeout-for-a-streamreader-operation-that-reads-a-file
            sw.Start();
            while (((line = await reader.ReadLineAsync()) != null))
            {
                if (line.Contains(command.Remove(command.Length - 1)))
                {
                    return;
                }

                // If the receiver never properly acknowledges the command, we need to manually time out
                if (sw.ElapsedMilliseconds > AckTimeout)
                {
                    log.Error("Waiting for command acknowledgement timed out");
                    break;
                }
            }

            throw new FailedAckException();
        }

        private static readonly ILog log = LogManager.GetLogger(typeof(Septentrio));

        /// <summary>
        /// The maximum time to wait for the receiver to acknowledge a message.
        /// If the receiver didn't acknowledge a message in this time, we assume it wasn't received correctly.
        /// </summary>
        private const int AckTimeout = 1000;

        /// <summary>
        /// The default baud rate for Septentrio receivers.
        /// </summary>
        public const int DefaultBaudrate = 115200;
    }
}
