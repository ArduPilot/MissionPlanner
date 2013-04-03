using System;
using System.Reflection;
using System.Windows.Forms;
using ArdupilotMega.Controls.BackstageView;
using log4net;
using Transitions;

namespace ArdupilotMega.GCSViews.ConfigurationView
{
    public partial class ConfigAccelerometerCalibrationQuad : UserControl, IActivate, IDeactivate
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private const float DisabledOpacity = 0.2F;
        private const float EnabledOpacity = 1.0F;

        public enum Frame
        {
            Plus = 0,
            X = 1,
            V = 2,
            Trapezoid = 3
        }

        public ConfigAccelerometerCalibrationQuad()
        {
            InitializeComponent();
        }

        private void BUT_levelac2_Click(object sender, EventArgs e)
        {
            try
            {
                Log.Info("Sending level command (mavlink 1.0)");                
                MainV2.comPort.doCommand(MAVLink.MAV_CMD.PREFLIGHT_CALIBRATION,1,0,0,0,0,0,0);

                BUT_levelac2.Text = "Complete";
            }
            catch(Exception ex)
            {
                Log.Error("Exception on level", ex);
                CustomMessageBox.Show("Failed to level : ac2 2.0.37+ is required");
            }
        }

        bool indochange = false;

        void DoChange(Frame frame)
        {
            if (indochange)
                return;

            indochange = true;

            switch (frame)
            {
                case Frame.Plus:
                    FadePicBoxes(pictureBoxPlus, EnabledOpacity);
                    FadePicBoxes(pictureBoxX, DisabledOpacity);
                    FadePicBoxes(pictureBoxTrap, DisabledOpacity);
                    radioButton_Plus.Checked = true;
                    radioButton_Trap.Checked = false;
                    radioButton_X.Checked = false;
                    SetFrameParam(frame);
                    break;
                case Frame.X:
                    FadePicBoxes(pictureBoxPlus, DisabledOpacity);
                    FadePicBoxes(pictureBoxX, EnabledOpacity);
                    FadePicBoxes(pictureBoxTrap, DisabledOpacity);
                    radioButton_Plus.Checked = false;
                    radioButton_Trap.Checked = false;
                    radioButton_X.Checked = true;
                    SetFrameParam(frame);
                    break;
                case Frame.V:
                    FadePicBoxes(pictureBoxPlus, DisabledOpacity);
                    FadePicBoxes(pictureBoxX, DisabledOpacity);
                    FadePicBoxes(pictureBoxTrap, DisabledOpacity);
                    radioButton_Plus.Checked = false;
                    radioButton_Trap.Checked = false;
                    radioButton_X.Checked = false;
                    SetFrameParam(frame);
                    break;
                case Frame.Trapezoid:
                    FadePicBoxes(pictureBoxPlus, DisabledOpacity);
                    FadePicBoxes(pictureBoxX, DisabledOpacity);
                    FadePicBoxes(pictureBoxTrap, EnabledOpacity);
                    radioButton_Plus.Checked = false;
                    radioButton_Trap.Checked = true;
                    radioButton_X.Checked = false;
                    SetFrameParam(frame);
                    break;
            }
            indochange = false;
        }

        private void SetFrameParam(Frame frame)
        {
            try
            {
                MainV2.comPort.setParam("FRAME", (int)frame);
            }
            catch
            {
                CustomMessageBox.Show("Set frame failed", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FadePicBoxes(Control picbox, float Opacity)
        {
            var fade = new Transition(new TransitionType_Linear(400));
            fade.add(picbox, "Opacity", Opacity);
            fade.run();
        }

        public void Activate()
        {
            if (!MainV2.comPort.MAV.param.ContainsKey("FRAME"))
            {
                this.Enabled = false;
                return;
            }

            DoChange((Frame)Enum.Parse(typeof(Frame), MainV2.comPort.MAV.param["FRAME"].ToString()));

            BUT_calib_accell.Enabled = true;
        }

        public void Deactivate()
        {
            MainV2.comPort.giveComport = false;

        }

        private void BUT_calib_accell_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.giveComport == true)
            {
                MainV2.comPort.BaseStream.WriteLine("");
                return;
            }

            try
            {
                Log.Info("Sending accel command (mavlink 1.0)");
                MainV2.comPort.giveComport = true;

                MainV2.comPort.Write("\n\n\n\n\n\n\n\n\n\n\n");
                System.Threading.Thread.Sleep(200);

                MainV2.comPort.doCommand(MAVLink.MAV_CMD.PREFLIGHT_CALIBRATION, 0, 0, 0, 0, 1, 0, 0);
                MainV2.comPort.giveComport = true;

                System.Threading.ThreadPool.QueueUserWorkItem(readmessage,this);

                BUT_calib_accell.Text = "Click When Done";
            }
            catch (Exception ex)
            {
                MainV2.comPort.giveComport = false;
                Log.Error("Exception on level", ex);
                CustomMessageBox.Show("Failed to level : ac2 2.0.37+ is required");
            }
        }

        static void readmessage(object item)
        {
            ConfigAccelerometerCalibrationQuad local = (ConfigAccelerometerCalibrationQuad)item;

            // clean up history
            MainV2.comPort.MAV.cs.messages.Clear();

            while (!(MainV2.comPort.MAV.cs.message.Contains("Calibration successful") || MainV2.comPort.MAV.cs.message.Contains("Calibration failed")))
            {
                try
                {
                    System.Threading.Thread.Sleep(10);
                    // read the message
                    MainV2.comPort.readPacket();
                    // update cs with the message
                    MainV2.comPort.MAV.cs.UpdateCurrentSettings(null);
                    // update user display
                    local.UpdateUserMessage();


                }
                catch { break; }
            }

            MainV2.comPort.giveComport = false;

            try
            {
                local.Invoke((MethodInvoker)delegate()
            {
                local.BUT_calib_accell.Text = "Done";
                local.BUT_calib_accell.Enabled = false;
            });
            }
            catch { }
        }

        public void UpdateUserMessage()
        {
            this.Invoke((MethodInvoker)delegate()
            {
                 lbl_Accel_user.Text = MainV2.comPort.MAV.cs.message;
            });
        }

        private void radioButton_Plus_CheckedChanged(object sender, EventArgs e)
        {
            DoChange(Frame.Plus);
        }

        private void radioButton_X_CheckedChanged(object sender, EventArgs e)
        {
            DoChange(Frame.X);
        }

        private void pictureBoxPlus_Click(object sender, EventArgs e)
        {
            DoChange(Frame.Plus);
        }

        private void pictureBoxX_Click(object sender, EventArgs e)
        {
            DoChange(Frame.X);
        }

        private void pictureBoxTrap_Click(object sender, EventArgs e)
        {
            DoChange(Frame.Trapezoid);
        }

        private void radioButton_Trap_CheckedChanged(object sender, EventArgs e)
        {
            DoChange(Frame.Trapezoid);
        }

    }
}
