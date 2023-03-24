using MissionPlanner.ArduPilot.Mavlink;
using MissionPlanner.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigCubeID : MyUserControl, IActivate
    {
        public ConfigCubeID()
        {
            InitializeComponent();
        }

        bool done = false;
        double progress = 0.0;
        uint offset = 0;
        private string file;

        private void but_upfw_Click(object sender, EventArgs e)
        {
            done = false;
            progress = 0.0;
            offset = 0;

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "*.bin|*.bin";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                file = ofd.FileName;

                ProgressReporterDialogue prd = new ProgressReporterDialogue();

                prd.DoWork += Prd_DoWork;

                prd.RunBackgroundOperationAsync();
            }
        }

        private void Prd_DoWork(Utilities.IProgressReporterDialogue sender)
        {
            var firmware_data = File.ReadAllBytes(file);

            var firmware_size = (uint)firmware_data.Length;

            var crc32 = (uint)MAVFtp.crc_crc32(0, firmware_data);

            bool seenresp = false;

            MainV2.comPort.BaseStream.BaudRate = 57600;

            var subid = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.CUBEPILOT_FIRMWARE_UPDATE_RESP, msg =>
            {
                seenresp = true;
                var firmware_span = new ReadOnlySpan<byte>(firmware_data);

                var resp = (MAVLink.mavlink_cubepilot_firmware_update_resp_t)msg.data;

                offset = resp.offset;
                progress = resp.offset / (double)firmware_size;

                if (resp.offset > firmware_size)
                {
                    done = true;
                    return true;
                }

                var len = (int)Math.Min(252, firmware_size - offset);
                if (len == 0)
                {
                    done = true;
                    return true;
                }
                MainV2.comPort.generatePacket(MAVLink.MAVLINK_MSG_ID.ENCAPSULATED_DATA,
                    new MAVLink.mavlink_encapsulated_data_t((ushort)(offset / 252), firmware_span.Slice((int)offset, len).ToArray()),
                    MainV2.comPort.sysidcurrent, MainV2.comPort.compidcurrent);

                return true;

            }, (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, true);

            while (!done && !sender.doWorkArgs.CancelRequested)
            {
                // send start
                MainV2.comPort.generatePacket(MAVLink.MAVLINK_MSG_ID.CUBEPILOT_FIRMWARE_UPDATE_START,
      new MAVLink.mavlink_cubepilot_firmware_update_start_t(firmware_size, crc32, (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent),
      (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);

                Thread.Sleep(1000);
                sender.UpdateProgressAndStatus((int)(progress * 100), "Updating " + offset + " " + seenresp);

                if (!seenresp)
                    continue;

                if (offset > firmware_size)
                {
                    done = true;
                    continue;
                }

                // send current chunk
                var len = (int)Math.Min(252, firmware_size - offset);
                if (len == 0)
                {
                    done = true;
                    continue;
                }
                MainV2.comPort.generatePacket(MAVLink.MAVLINK_MSG_ID.ENCAPSULATED_DATA,
                    new MAVLink.mavlink_encapsulated_data_t((ushort)(offset / 252), new ReadOnlySpan<byte>(firmware_data).Slice((int)offset, len).ToArray()),
                    MainV2.comPort.sysidcurrent, MainV2.comPort.compidcurrent);
            }

            MainV2.comPort.UnSubscribeToPacketType(subid);
        }

        public void Activate()
        {
            if (MainV2.comPort.compidcurrent != (int)MAVLink.MAV_COMPONENT.MAV_COMP_ID_ODID_TXRX_1)
            {
                if (MainV2.comPort.MAVlist.Contains(1, 1))
                {
                    mavpasscombo.setup(new[] { "SERIAL_PASS2" }, MainV2.comPort.MAVlist[1, 1].param);
                    mavnumtimeout.setup(0, 60, 1, 1, "SERIAL_PASSTIMO", MainV2.comPort.MAVlist[1, 1].param);
                }
                but_upfw.Enabled = false;
            }
            else 
            {
                this.Enabled = true;
                but_upfw.Enabled = true;
            }
            //SERIAL_PASS2
            //SERIAL_PASSTIMO
        }
    }
}
