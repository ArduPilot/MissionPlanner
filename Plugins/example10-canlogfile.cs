using MissionPlanner;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;
using Newtonsoft.Json;
using UAVCAN;
//loadassembly: UAVCAN

namespace CANLogExtract
{
    public class Plugin : MissionPlanner.Plugin.Plugin
    {
        public override string Name
        {
            get { return "CAN Log Extract"; }
        }

        public override string Version
        {
            get { return "0.10"; }
        }

        public override string Author
        {
            get { return "Michael Oborne"; }
        }

        public override bool Init()
        {
            var but = new ToolStripMenuItem("CAN Log Extract");
            but.Click += but_Click;
            ToolStripItemCollection col = Host.FDMenuMap.Items;
            
            // uncomment to enable
            col.Add(but);

            return true;
        }

        private void but_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "can file|*.can";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = "txt";
                sfd.Filter = "txt|*.txt";
                sfd.FileName = Path.GetFileNameWithoutExtension(ofd.FileName) + ".txt";
                sfd.InitialDirectory = Path.GetDirectoryName(ofd.FileName);

                if (sfd.ShowDialog() == DialogResult.OK)
                { 
                    UAVCAN.uavcan can = new uavcan();

                    using (var stream = sfd.OpenFile())
                    {
                        var data = File.ReadAllBytes(ofd.FileName);

                        can.MessageReceived += (frame, msg, id) =>
                        {
                            var str = ASCIIEncoding.ASCII.GetBytes("\n" + msg.GetType().Name + "=" + msg.ToJSON());
                            stream
                                .Write(str, 0, str.Length);
                        };

                        can.FrameReceived += (frame, payload) =>
                        {
                            var str = ASCIIEncoding.ASCII.GetBytes("\nFrame " + frame.ToJSON(Formatting.None) + payload.ToJSON(Formatting.None));
                            stream
                                .Write(str, 0, str.Length);
                        };

                        can.FrameError += (frame, payload) =>
                        {
                            var str = ASCIIEncoding.ASCII.GetBytes("\nError");
                            stream
                                .Write(str, 0, str.Length);
                        };

                        data.ForEach(b =>
                        {
                            try
                            {
                                can.Read((byte) b);
                                stream.WriteByte((byte) b);
                            }
                            catch (Exception ex)
                            {
                                var str = ASCIIEncoding.ASCII.GetBytes(ex.ToString());
                                stream
                                    .Write(str, 0, str.Length);
                            }
                        });
                    }
                }
            }
        }

        public override bool Loaded()
        {
            return true;
        }

        public override bool Loop()
        {
            return true;
        }

        public override bool Exit()
        {
            return true;
        }
    }
}