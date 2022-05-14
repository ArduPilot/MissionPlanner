using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using Org.BouncyCastle.Crypto.Tls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.plugins
{
    public class example19_linkforward : Plugin.Plugin
    {
        public override string Name { get; } = "Link Forward";

        public override string Version { get; }

        public override string Author { get; }

        public override bool Exit()
        {
            return true;
        }

        public override bool Init()
        {
            var rootbut = new ToolStripMenuItem("Forward between links");
            rootbut.Click += but_Click;
            ToolStripItemCollection col = Host.FDMenuMap.Items;
            col.Add(rootbut);

            return true;
        }

        private void but_Click(object sender2, EventArgs e)
        {
            loopratehz = 1;
        }

        public override bool Loaded()
        {            
            return true;
        }

        List<MAVLinkInterface> mAVLinkInterfaces = new List<MAVLinkInterface>();

        public override bool Loop()
        {
            try
            {
                MainV2.Comports.ForEach<MAVLinkInterface>(comport =>
                {
                    // add any new interface to the forwarding
                    if (mAVLinkInterfaces.Contains(comport))
                        return;
                    mAVLinkInterfaces.Add(comport);

                    comport.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT, (m) =>
                    {
                        try
                        {
                            MainV2.Comports.ForEach<MAVLinkInterface>(comport1 =>
                            {
                                if (comport != comport1)
                                    comport1.Write(m.buffer);
                            });
                            return true;
                        }
                        catch (Exception e)
                        {
                            return true;
                        }
                    });

                    comport.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.ATTITUDE, (m) =>
                    {
                        try
                        {
                            MainV2.Comports.ForEach<MAVLinkInterface>(comport1 =>
                            {
                                if (comport != comport1)
                                    comport1.Write(m.buffer);
                            });
                            return true;
                        }
                        catch (Exception e)
                        {
                            return true;
                        }
                    });
                });
            }
            catch (Exception e)
            {

            }

            return true;
        }
    }
}