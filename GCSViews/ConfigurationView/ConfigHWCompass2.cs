using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Color = System.Drawing.Color;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWCompass2 : MyUserControl, IActivate, IDeactivate
    {
        private List<DeviceInfo> list;

        private bool rebootrequired = false;

        private List<MAVLink.MAVLinkMessage> mprog = new List<MAVLink.MAVLinkMessage>();
        private List<MAVLink.MAVLinkMessage> mrep = new List<MAVLink.MAVLinkMessage>();

        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> packetsub1;
        private KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<MAVLink.MAVLinkMessage, bool>> packetsub2;


        public ConfigHWCompass2()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            list = MainV2.comPort.MAV.param.Where(a => a.Name.StartsWith("COMPASS_DEV_ID"))
                .Select((a, b) => new DeviceInfo(b, a.Name, (uint) a.Value))
                .OrderBy((a) => a.ParamName).ToList();

            var bs = new BindingSource();
            bs.DataSource = list;
            myDataGridView1.DataSource = bs;

            mavlinkComboBoxfitness.setup(ParameterMetaDataRepository.GetParameterOptionsInt("COMPASS_CAL_FIT",
                MainV2.comPort.MAV.cs.firmware.ToString()), "COMPASS_CAL_FIT", MainV2.comPort.MAV.param);

            mavlinkCheckBoxUseCompass1.setup(1, 0, "COMPASS_USE", MainV2.comPort.MAV.param);
            mavlinkCheckBoxUseCompass2.setup(1, 0, "COMPASS_USE2", MainV2.comPort.MAV.param);
            mavlinkCheckBoxUseCompass3.setup(1, 0, "COMPASS_USE3", MainV2.comPort.MAV.param);
        }

        public void Deactivate()
        {
            timer1.Stop();

            CheckReboot();
        }

        private bool CheckReboot()
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
                return true;

            if (rebootrequired)
            {
                if (CustomMessageBox.Show("Reboot required, reboot now?", "Reboot",
                        CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes)
                {
                    try
                    {
                        if (MainV2.comPort.doReboot())
                        {
                            CustomMessageBox.Show("Reboot failed. please manually reboot the hardware.", Strings.ERROR);
                        }
                        rebootrequired = false;
                    }
                    catch
                    {
                        CustomMessageBox.Show(Strings.ErrorCommunicating, Strings.ERROR);
                    }

                    return true;
                }
            }

            return false;
        }

        private async void myDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Up.Index)
            {
                var item = list[e.RowIndex];
                list.Remove(item);
                list.Insert(e.RowIndex - 1, item);

                await UpdateFirst3();
            }

            if (e.ColumnIndex == Down.Index)
            {
                var item = list[e.RowIndex];
                list.Remove(item);
                list.Insert(e.RowIndex + 1, item);

                await UpdateFirst3();
            }
        }

        private async Task UpdateFirst3()
        {
            if (myDataGridView1.Rows.Count >= 1)
            {
                list[0]._index = 0;
                bool p1 = await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO1_ID",
                    int.Parse(myDataGridView1.Rows[0].Cells[devIDDataGridViewTextBoxColumn.Index].Value.ToString()));

                if (!p1)
                    CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }

            if (myDataGridView1.Rows.Count >= 2)
            {
                list[1]._index = 1;
                bool p2 = await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO2_ID",
                    int.Parse(myDataGridView1.Rows[1].Cells[devIDDataGridViewTextBoxColumn.Index].Value.ToString()));

                if (!p2)
                    CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }

            if (myDataGridView1.Rows.Count >= 3)
            {
                list[2]._index = 2;
                bool p3 = await MainV2.comPort.setParamAsync((byte)MainV2.comPort.sysidcurrent,
                    (byte)MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO3_ID",
                    int.Parse(myDataGridView1.Rows[2].Cells[devIDDataGridViewTextBoxColumn.Index].Value.ToString()));

                if (!p3)
                    CustomMessageBox.Show(Strings.ErrorSettingParameter, Strings.ERROR);
            }

            rebootrequired = true;

            myDataGridView1.Invalidate();
        }

        private void BUT_OBmagcalstart_Click(object sender, EventArgs e)
        {
            if (rebootrequired && !CheckReboot())
            {
                return;
            }

            try
            {
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_START_MAG_CAL, 0, 1, 1, 0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                CustomMessageBox.Show("Failed to start MAG CAL, check the autopilot is still responding.\n" + ex.ToString(), Strings.ERROR);
                return;
            }

            mprog.Clear();
            mrep.Clear();
            horizontalProgressBar1.Value = 0;
            horizontalProgressBar2.Value = 0;
            horizontalProgressBar3.Value = 0;

            packetsub1 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS, ReceviedPacket);
            packetsub2 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT, ReceviedPacket);

            BUT_OBmagcalaccept.Enabled = true;
            BUT_OBmagcalcancel.Enabled = true;
            timer1.Start();
        }

        private bool ReceviedPacket(MAVLink.MAVLinkMessage packet)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                MainV2.comPort.DebugPacket(packet, true);

            if (packet.msgid == (byte)MAVLink.MAVLINK_MSG_ID.MAG_CAL_PROGRESS)
            {
                lock (this.mprog)
                {
                    this.mprog.Add(packet);
                }

                return true;
            }
            else if (packet.msgid == (byte)MAVLink.MAVLINK_MSG_ID.MAG_CAL_REPORT)
            {
                lock (this.mrep)
                {
                    this.mrep.Add(packet);
                }

                return true;
            }

            return true;
        }

        private void BUT_OBmagcalaccept_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_ACCEPT_MAG_CAL, 0, 0, 1, 0, 0, 0, 0);

            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR, MessageBoxButtons.OK);
            }

            MainV2.comPort.UnSubscribeToPacketType(packetsub1);
            MainV2.comPort.UnSubscribeToPacketType(packetsub2);

            timer1.Stop();
        }

        private void BUT_OBmagcalcancel_Click(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.doCommand((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, MAVLink.MAV_CMD.DO_CANCEL_MAG_CAL, 0, 0, 1, 0, 0, 0, 0);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), Strings.ERROR, MessageBoxButtons.OK);
            }

            MainV2.comPort.UnSubscribeToPacketType(packetsub1);
            MainV2.comPort.UnSubscribeToPacketType(packetsub2);

            timer1.Stop();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            lbl_obmagresult.Clear();
            int compasscount = 0;
            int completecount = 0;
            lock (mprog)
            {
                // somewhere to save our %
                Dictionary<byte, MAVLink.MAVLinkMessage> status = new Dictionary<byte, MAVLink.MAVLinkMessage>();
                foreach (var item in mprog)
                {
                    status[((MAVLink.mavlink_mag_cal_progress_t)item.data).compass_id] = item;
                }

                // message for user
                string message = "";
                foreach (var item in status)
                {
                    var obj = (MAVLink.mavlink_mag_cal_progress_t)item.Value.data;

                    try
                    {
                        if (item.Key == 0)
                            horizontalProgressBar1.Value = obj.completion_pct;
                        if (item.Key == 1)
                            horizontalProgressBar2.Value = obj.completion_pct;
                        if (item.Key == 2)
                            horizontalProgressBar3.Value = obj.completion_pct;
                    }
                    catch { }

                    message += "id:" + item.Key + " " + obj.completion_pct.ToString() + "% ";
                    compasscount++;
                }
                lbl_obmagresult.AppendText(message + "\n");
            }

            lock (mrep)
            {
                // somewhere to save our answer
                Dictionary<byte, MAVLink.MAVLinkMessage> status = new Dictionary<byte, MAVLink.MAVLinkMessage>();
                foreach (var item in mrep)
                {
                    var obj = (MAVLink.mavlink_mag_cal_report_t)item.data;

                    if (obj.compass_id == 0 && obj.ofs_x == 0)
                        continue;

                    status[obj.compass_id] = item;
                }

                // message for user
                foreach (var item in status.Values)
                {
                    var obj = (MAVLink.mavlink_mag_cal_report_t)item.data;

                    lbl_obmagresult.AppendText("id:" + obj.compass_id + " x:" + obj.ofs_x.ToString("0.0") + " y:" +
                                               obj.ofs_y.ToString("0.0") + " z:" +
                                               obj.ofs_z.ToString("0.0") + " fit:" + obj.fitness.ToString("0.0") + " " +
                                               (MAVLink.MAG_CAL_STATUS)obj.cal_status + "\n");

                    try
                    {
                        if (obj.compass_id == 0)
                        {
                            horizontalProgressBar1.Value = 100;
                            pictureBox1.BackColor = Color.Green;
                        }

                        if (obj.compass_id == 1)
                        {
                            horizontalProgressBar2.Value = 100;
                            pictureBox2.BackColor = Color.Green;
                        }

                        if (obj.compass_id == 2)
                        {
                            horizontalProgressBar3.Value = 100;
                            pictureBox3.BackColor = Color.Green;
                        }
                    }
                    catch
                    {
                    }

                    if ((MAVLink.MAG_CAL_STATUS)obj.cal_status != MAVLink.MAG_CAL_STATUS.MAG_CAL_SUCCESS)
                    {
                        //CustomMessageBox.Show(Strings.CommandFailed);
                    }

                    if (obj.autosaved == 1)
                    {
                        completecount++;
                        timer1.Interval = 1000;
                    }
                }
            }

            if (compasscount == completecount && compasscount != 0)
            {
                BUT_OBmagcalcancel.Enabled = false;
                BUT_OBmagcalaccept.Enabled = false;
                timer1.Stop();
                CustomMessageBox.Show("Please reboot the autopilot");
            }
        }

        private void myDataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
        }

        private void but_largemagcal_Click(object sender, EventArgs e)
        {
            double value = 0;
            if (InputBox.Show("MagCal Yaw", "Enter current heading in degrees\nNOTE: gps lock is required", ref value) == DialogResult.OK)
            {
                try
                {
                    if (MainV2.comPort.doCommand(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid,
                        MAVLink.MAV_CMD.FIXED_MAG_CAL_YAW, (float) value, 0, 0, 0, 0, 0, 0))
                    {

                    }
                    else
                    {
                        CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                    }
                }
                catch (Exception exception)
                {
                    CustomMessageBox.Show(Strings.CommandFailed, Strings.ERROR);
                }
            }
        }
    }
}
