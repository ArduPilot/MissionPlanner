using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using log4net;

namespace MissionPlanner.Utilities
{
    public class UDPMavlinkShim
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static UdpClient client;

        static UDPMavlinkShim()
        {
            try
            {
                client = new UdpClient(14550, AddressFamily.InterNetwork);
                client.BeginReceive(clientdata, client);
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private static void clientdata(IAsyncResult ar)
        {
            var client = ((UdpClient) ar.AsyncState);

            if (client == null || client.Client == null)
                return;

            var port = ((IPEndPoint) client.Client.LocalEndPoint).Port;

            //if (client != null)
            //client.Close();

            try
            {
                var udpclient = new Comms.UdpSerial(client);

                MainV2.instance.BeginInvoke((Action) delegate
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
