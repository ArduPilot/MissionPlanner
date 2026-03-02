using log4net;
using MissionPlanner.Comms;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace MissionPlanner.Utilities
{
    public class Unicore
    {

        public class FailedAckException : Exception { }



        public static async Task ConfigureBaseReceiver(ICommsSerial receiverPort)
        {
            await receiverPort.BaseStream.FlushAsync();

            receiverPort.BaudRate = 115200;
            receiverPort.ReadTimeout = 200;
            receiverPort.WriteTimeout = 200;


            await SendAck(receiverPort, "GPGGA COM3 1\r\n");
            await SendAck(receiverPort, "mode base time 60 2 2.5\r\n");
            await SendAck(receiverPort, "rtcm1006 com3 1\r\n");
            await SendAck(receiverPort, "rtcm1033 com3 1\r\n");
            await SendAck(receiverPort, "rtcm1074 com3 1\r\n");
            await SendAck(receiverPort, "rtcm1124 com3 1\r\n");
            await SendAck(receiverPort, "rtcm1084 com3 1\r\n");
            await SendAck(receiverPort, "rtcm1094 com3 1\r\n");
            await SendAck(receiverPort, "saveconfig\r\n");

        }


        private static async Task SendAck(ICommsSerial receiverPort, String command)
        {
            StreamReader reader = new StreamReader(receiverPort.BaseStream, System.Text.Encoding.ASCII);

            await receiverPort.BaseStream.FlushAsync();
            await receiverPort.BaseStream.WriteAsync(System.Text.Encoding.ASCII.GetBytes(command), 0, command.Length);

        }


        private static readonly ILog log = LogManager.GetLogger(typeof(Septentrio));


    }
}
