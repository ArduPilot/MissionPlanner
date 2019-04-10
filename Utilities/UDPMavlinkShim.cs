using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using log4net;

namespace MissionPlanner.Utilities
{
    public class UDPMavlinkShim
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static UdpClient client;

        private static Timer timer;

        static UDPMavlinkShim()
        {
            try
            {
                client = new UdpClient(14550, AddressFamily.InterNetwork);
                client.BeginReceive(clientdata, client);

                timer = new Timer(state => { Stop(); }, null, 30000, 0);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private static void clientdata(IAsyncResult ar)
        {
            timer = null;
            var client = ((UdpClient)ar.AsyncState);

            if (client == null || client.Client == null)
                return;
            try
            {
                var port = ((IPEndPoint)client.Client.LocalEndPoint).Port;

                //if (client != null)
                //client.Close();

                var udpclient = new Comms.UdpSerial(client);

                MainV2.instance.BeginInvoke((Action)delegate
               {
                   if (MainV2.comPort.BaseStream.IsOpen)
                   {
                       var mav = new MAVLinkInterface();
                       mav.BaseStream = udpclient;
                       MainV2.instance.doConnect(mav, "preset", port.ToString());

                       MainV2.Comports.Add(mav);
                   }
                   else
                   {
                       MainV2.comPort.BaseStream = udpclient;
                       MainV2.instance.doConnect(MainV2.comPort, "preset", port.ToString());
                   }
               });
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static void Start()
        {
        }

        public static void Stop()
        {
            try
            {
                if (client!= null)
                client.Close();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
    }
}
