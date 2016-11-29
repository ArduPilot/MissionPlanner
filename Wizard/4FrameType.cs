using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.GCSViews.ConfigurationView;

namespace MissionPlanner.Wizard
{
    public partial class _4FrameType : MyUserControl, IWizard, IActivate
    {
        bool selected = false;

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
                    MainV2.comPort.setParam("FRAME", (int) ConfigFrameType.Frame.X);
                    break;
                case "+":
                    MainV2.comPort.setParam("FRAME", (int) ConfigFrameType.Frame.Plus);
                    break;
                case "trap":
                    MainV2.comPort.setParam("FRAME", (int) ConfigFrameType.Frame.V);
                    break;
                case "h":
                    MainV2.comPort.setParam("FRAME", (int) ConfigFrameType.Frame.H);
                    break;
                case "y6b":
                    MainV2.comPort.setParam("FRAME", (int) ConfigFrameType.Frame.Y);
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
                ConfigFrameType.Frame frame = (ConfigFrameType.Frame) (int) (float) MainV2.comPort.MAV.param["FRAME"];

                switch (frame)
                {
                    case ConfigFrameType.Frame.X:
                        pictureBox_Click(pictureBoxMouseOverX, EventArgs.Empty);
                        break;
                    case ConfigFrameType.Frame.Plus:
                        pictureBox_Click(pictureBoxMouseOverplus, EventArgs.Empty);
                        break;
                    case ConfigFrameType.Frame.V:
                        pictureBox_Click(pictureBoxMouseOvertrap, EventArgs.Empty);
                        break;
                    case ConfigFrameType.Frame.H:
                        pictureBox_Click(pictureBoxMouseOverH, EventArgs.Empty);
                        break;
                    case ConfigFrameType.Frame.Y:
                        pictureBox_Click(pictureBoxMouseOverY, EventArgs.Empty);
                        break;
                }
            }
        }
    }
}