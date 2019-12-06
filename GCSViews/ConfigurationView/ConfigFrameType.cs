using System;
using System.Reflection;
using System.Windows.Forms;
using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Controls;
using Transitions;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigFrameType : MyUserControl, IActivate, IDeactivate
    {
        private const float DisabledOpacity = 0.2F;
        private const float EnabledOpacity = 1.0F;
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private bool indochange;

        public ConfigFrameType()
        {
            InitializeComponent();

            configDefaultSettings1.OnChange += configDefaultSettings1_OnChange;
        }

        public void Activate()
        {
            if (!MainV2.comPort.MAV.param.ContainsKey("FRAME"))
            {
                Enabled = false;
                return;
            }

            DoChange((Frame) Enum.Parse(typeof (Frame), MainV2.comPort.MAV.param["FRAME"].ToString()));
        }

        public void Deactivate()
        {
            MainV2.comPort.giveComport = false;
        }

        private void configDefaultSettings1_OnChange(object sender, EventArgs e)
        {
            Activate();
        }

        private void DoChange(Frame frame)
        {
            if (indochange)
                return;

            indochange = true;

            switch (frame)
            {
                case Frame.Plus:
                    FadePicBoxes(pictureBoxPlus, EnabledOpacity);
                    FadePicBoxes(pictureBoxX, DisabledOpacity);
                    FadePicBoxes(pictureBoxV, DisabledOpacity);
                    FadePicBoxes(pictureBoxH, DisabledOpacity);
                    FadePicBoxes(pictureBoxY, DisabledOpacity);
                    FadePicBoxes(pictureBoxVTail, DisabledOpacity);
                    radioButton_VTail.Checked = false;
                    radioButton_Plus.Checked = true;
                    radioButton_V.Checked = false;
                    radioButton_X.Checked = false;
                    radioButton_H.Checked = false;
                    radioButton_Y.Checked = false;
                    SetFrameParam(frame);
                    break;
                case Frame.X:
                    FadePicBoxes(pictureBoxPlus, DisabledOpacity);
                    FadePicBoxes(pictureBoxX, EnabledOpacity);
                    FadePicBoxes(pictureBoxV, DisabledOpacity);
                    FadePicBoxes(pictureBoxH, DisabledOpacity);
                    FadePicBoxes(pictureBoxY, DisabledOpacity);
                    FadePicBoxes(pictureBoxVTail, DisabledOpacity);
                    radioButton_VTail.Checked = false;
                    radioButton_Plus.Checked = false;
                    radioButton_V.Checked = false;
                    radioButton_X.Checked = true;
                    radioButton_H.Checked = false;
                    radioButton_Y.Checked = false;
                    SetFrameParam(frame);
                    break;
                case Frame.V:
                    FadePicBoxes(pictureBoxPlus, DisabledOpacity);
                    FadePicBoxes(pictureBoxX, DisabledOpacity);
                    FadePicBoxes(pictureBoxV, EnabledOpacity);
                    FadePicBoxes(pictureBoxH, DisabledOpacity);
                    FadePicBoxes(pictureBoxY, DisabledOpacity);
                    FadePicBoxes(pictureBoxVTail, DisabledOpacity);
                    radioButton_VTail.Checked = false;
                    radioButton_Plus.Checked = false;
                    radioButton_V.Checked = true;
                    radioButton_X.Checked = false;
                    radioButton_H.Checked = false;
                    radioButton_Y.Checked = false;
                    SetFrameParam(frame);
                    break;
                case Frame.H:
                    FadePicBoxes(pictureBoxPlus, DisabledOpacity);
                    FadePicBoxes(pictureBoxX, DisabledOpacity);
                    FadePicBoxes(pictureBoxV, DisabledOpacity);
                    FadePicBoxes(pictureBoxH, EnabledOpacity);
                    FadePicBoxes(pictureBoxY, DisabledOpacity);
                    FadePicBoxes(pictureBoxVTail, DisabledOpacity);
                    radioButton_VTail.Checked = false;
                    radioButton_Plus.Checked = false;
                    radioButton_V.Checked = false;
                    radioButton_X.Checked = false;
                    radioButton_H.Checked = true;
                    radioButton_Y.Checked = false;
                    SetFrameParam(frame);
                    break;
                case Frame.Y:
                    FadePicBoxes(pictureBoxPlus, DisabledOpacity);
                    FadePicBoxes(pictureBoxX, DisabledOpacity);
                    FadePicBoxes(pictureBoxV, DisabledOpacity);
                    FadePicBoxes(pictureBoxH, DisabledOpacity);
                    FadePicBoxes(pictureBoxY, EnabledOpacity);
                    FadePicBoxes(pictureBoxVTail, DisabledOpacity);
                    radioButton_VTail.Checked = false;
                    radioButton_Plus.Checked = false;
                    radioButton_V.Checked = false;
                    radioButton_X.Checked = false;
                    radioButton_H.Checked = false;
                    radioButton_Y.Checked = true;
                    SetFrameParam(frame);
                    break;
                case Frame.VTail:
                    FadePicBoxes(pictureBoxPlus, DisabledOpacity);
                    FadePicBoxes(pictureBoxX, DisabledOpacity);
                    FadePicBoxes(pictureBoxV, DisabledOpacity);
                    FadePicBoxes(pictureBoxH, DisabledOpacity);
                    FadePicBoxes(pictureBoxY, DisabledOpacity);
                    FadePicBoxes(pictureBoxVTail, EnabledOpacity);
                    radioButton_VTail.Checked = true;
                    radioButton_Plus.Checked = false;
                    radioButton_V.Checked = false;
                    radioButton_X.Checked = false;
                    radioButton_H.Checked = false;
                    radioButton_Y.Checked = false;
                    SetFrameParam(frame);
                    break;
                default:
                    radioButton_Plus.Checked = false;
                    radioButton_V.Checked = false;
                    radioButton_X.Checked = false;
                    radioButton_H.Checked = false;
                    radioButton_Y.Checked = false;
                    break;
            }
            indochange = false;
        }

        private void SetFrameParam(Frame frame)
        {
            try
            {
                MainV2.comPort.setParam("FRAME", (int) frame);
            }
            catch
            {
                CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, "FRAME"), Strings.ERROR,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FadePicBoxes(Control picbox, float Opacity)
        {
            var fade = new Transition(new TransitionType_Linear(400));
            fade.add(picbox, "Opacity", Opacity);
            fade.run();
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

        private void pictureBoxV_Click(object sender, EventArgs e)
        {
            DoChange(Frame.V);
        }

        private void radioButton_V_CheckedChanged(object sender, EventArgs e)
        {
            DoChange(Frame.V);
        }

        private void pictureBoxH_Click(object sender, EventArgs e)
        {
            DoChange(Frame.H);
        }

        private void radioButton_H_CheckedChanged(object sender, EventArgs e)
        {
            DoChange(Frame.H);
        }

        private void pictureBoxY_Click(object sender, EventArgs e)
        {
            DoChange(Frame.Y);
        }

        private void radioButton_Y_CheckedChanged(object sender, EventArgs e)
        {
            DoChange(Frame.Y);
        }

        private void radioButton_VTail_CheckedChanged(object sender, EventArgs e)
        {
            DoChange(Frame.VTail);
        }

        private void pictureBoxVTail_Click(object sender, EventArgs e)
        {
            DoChange(Frame.VTail);
        }
    }
}