using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class UDPVideoShim
    {
        static UdpClient client = new UdpClient(5600, AddressFamily.InterNetwork);

        static UDPVideoShim()
        {
            client.BeginReceive(clientdata, client);
        }

        private static void clientdata(IAsyncResult ar)
        {
            if(client != null)
                client.Close();

            if (!File.Exists(GStreamer.gstlaunch))
            {
                if (CustomMessageBox.Show("A video stream has been detected, but gstreamer has not been configured/installed.\nDo you want to config it now","GStreamer", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    if (GStreamer.getGstLaunchExe())
                    {

                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            GStreamer.Start();
        }

        public static void Start()
        {

        }
    }
}