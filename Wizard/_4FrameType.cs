using System;
using System.Windows.Forms;
using MissionPlanner.ArduPilot;
using MissionPlanner.Controls;

namespace MissionPlanner.Wizard
{
    public partial class _4FrameType : MyUserControl, IWizard, IActivate
    {
        bool selected = false;

        enum frame_class
        {
            none = motor_frame_class.MOTOR_FRAME_UNDEFINED,
            tri = motor_frame_class.MOTOR_FRAME_TRI,
            quad= motor_frame_class.MOTOR_FRAME_QUAD,
            trap= motor_frame_class.MOTOR_FRAME_QUAD,
            hexa= motor_frame_class.MOTOR_FRAME_HEXA,
            x8= motor_frame_class.MOTOR_FRAME_OCTAQUAD,
            octa= motor_frame_class.MOTOR_FRAME_OCTA,
            y6= motor_frame_class.MOTOR_FRAME_Y6
        }

        public _4FrameType()
        {
            InitializeComponent();
        }

        public int WizardValidate()
        {
            if (selected)
                return 1;

            return 0;
        }

        public bool WizardBusy()
        {
            return false;
        }

        void setframeType(object sender)
        {
            string option = (sender as PictureBoxMouseOver).Tag.ToString();

            selected = true;

            switch (option)
            {
                case "x":
                    MainV2.comPort.setParam("FRAME", (int) Frame.X);
                    MainV2.comPort.setParam("FRAME_CLASS", (int) (frame_class) Enum.Parse(typeof (frame_class), Wizard.config["fwframe"].ToString()));
                    MainV2.comPort.setParam("FRAME_TYPE", (int)Frame.X);
                    break;
                case "+":
                    MainV2.comPort.setParam("FRAME", (int) Frame.Plus);
                    MainV2.comPort.setParam("FRAME_CLASS", (int)(frame_class)Enum.Parse(typeof(frame_class), Wizard.config["fwframe"].ToString()));
                    MainV2.comPort.setParam("FRAME_TYPE", (int)Frame.Plus);
                    break;
                case "trap":
                    MainV2.comPort.setParam("FRAME", (int) Frame.V);
                    MainV2.comPort.setParam("FRAME_CLASS", (int)(frame_class)Enum.Parse(typeof(frame_class), Wizard.config["fwframe"].ToString()));
                    MainV2.comPort.setParam("FRAME_TYPE", (int)Frame.V);
                    break;
                case "h":
                    MainV2.comPort.setParam("FRAME", (int) Frame.H);
                    MainV2.comPort.setParam("FRAME_CLASS", (int)(frame_class)Enum.Parse(typeof(frame_class), Wizard.config["fwframe"].ToString()));
                    MainV2.comPort.setParam("FRAME_TYPE", (int)Frame.H);
                    break;
                case "y6b":
                    MainV2.comPort.setParam("FRAME", (int) Frame.Y);
                    MainV2.comPort.setParam("FRAME_CLASS", (int)(frame_class)Enum.Parse(typeof(frame_class), Wizard.config["fwframe"].ToString()));
                    MainV2.comPort.setParam("FRAME_TYPE", (int)Frame.Y);
                    break;
            }
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            DeselectAll();
            (sender as PictureBoxMouseOver).selected = true;
            try
            {
                setframeType(sender);
            }
            catch
            {
                CustomMessageBox.Show(Strings.ErrorNotConnected, Strings.ERROR);
                Wizard.instance.Close();
            }
        }

        void DeselectAll()
        {
            foreach (var ctl in this.panel1.Controls)
            {
                if (ctl.GetType() == typeof (PictureBoxMouseOver))
                {
                    (ctl as PictureBoxMouseOver).selected = false;
                }
            }
        }

        public void Activate()
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                CustomMessageBox.Show(Strings.ErrorNotConnected, Strings.ERROR);
                Wizard.instance.Close();
                return;
            }

            if (MainV2.comPort.MAV.param.ContainsKey("FRAME"))
            {
                Frame frame = (Frame) (int) (float) MainV2.comPort.MAV.param["FRAME"];

                switch (frame)
                {
                    case Frame.X:
                        pictureBox_Click(pictureBoxMouseOverX, EventArgs.Empty);
                        break;
                    case Frame.Plus:
                        pictureBox_Click(pictureBoxMouseOverplus, EventArgs.Empty);
                        break;
                    case Frame.V:
                        pictureBox_Click(pictureBoxMouseOvertrap, EventArgs.Empty);
                        break;
                    case Frame.H:
                        pictureBox_Click(pictureBoxMouseOverH, EventArgs.Empty);
                        break;
                    case Frame.Y:
                        pictureBox_Click(pictureBoxMouseOverY, EventArgs.Empty);
                        break;
                }
            }
        }
    }
}