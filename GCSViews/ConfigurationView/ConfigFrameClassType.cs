using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner.ArduPilot;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using Transitions;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigFrameClassType : MyUserControl, IActivate, IDeactivate
    {
        private motor_frame_class work_frame_class;
        private motor_frame_type work_frame_type;


        private const float DisabledOpacity = 0.2F;
        private const float EnabledOpacity = 1.0F;
        private bool inDoType = false;

        public ConfigFrameClassType()
        {
            InitializeComponent();

            //radioButton1.Image = new Bitmap(radioButton1.Image, 60, 60);
            radioButtonQuad.Image = new Bitmap(radioButtonQuad.Image, 60, 60);
            radioButtonHexa.Image = new Bitmap(radioButtonHexa.Image, 60, 60);
            radioButtonOcta.Image = new Bitmap(radioButtonOcta.Image, 60, 60);
            radioButtonY6.Image = new Bitmap(radioButtonY6.Image, 60, 60);
            radioButtonHeli.Image = new Bitmap(radioButtonHeli.Image, 60, 60);
            radioButtonTri.Image = new Bitmap(radioButtonTri.Image, 60, 60);
            radioButtonOctaQuad.Image = new Bitmap(radioButtonOctaQuad.Image, 60, 60);
        }

        public void Activate()
        {
            if (!MainV2.comPort.MAV.param.ContainsKey("FRAME_CLASS") || !MainV2.comPort.MAV.param.ContainsKey("FRAME_TYPE"))
            {
                Enabled = false;
                return;
            }

            // pre seed the correct values
            work_frame_class = (motor_frame_class)
                Enum.Parse(typeof (motor_frame_class), MainV2.comPort.MAV.param["FRAME_CLASS"].ToString());
            work_frame_type = (motor_frame_type)
                Enum.Parse(typeof (motor_frame_type), MainV2.comPort.MAV.param["FRAME_TYPE"].ToString());

            this.LogInfoFormat("Existing Class: {0} Type: {1}", work_frame_class, work_frame_type);

            DoClass(work_frame_class);
            DoType(work_frame_type);
        }

        public void Deactivate()
        {

        }

        private void DoClass(motor_frame_class frame_class)
        {
            if (inDoType)
                return;

            // prevent recursive calls because we modify radiobuttons
            inDoType = true;

            work_frame_class = frame_class;

            // get list of valid types
            var validtypes = ArduPilot.Common.ValidList.Where(a => { return a.Item1 == work_frame_class; });

            if (validtypes.Count() == 0 || validtypes.Count() == 1 && validtypes.First().Item2 == null)
            {
                // no valid types
                groupBox2.Enabled = false;
            }
            else
            {
                groupBox2.Enabled = true;
            }

            switch (frame_class)
            {
                case motor_frame_class.MOTOR_FRAME_UNDEFINED:
                    radioButtonUndef.Checked = true;
                    break;
                case motor_frame_class.MOTOR_FRAME_QUAD:
                    radioButtonQuad.Checked = true;
                    break;
                case motor_frame_class.MOTOR_FRAME_HEXA:
                    radioButtonHexa.Checked = true;
                    break;
                case motor_frame_class.MOTOR_FRAME_OCTA:
                    radioButtonOcta.Checked = true;
                    break;
                case motor_frame_class.MOTOR_FRAME_OCTAQUAD:
                    radioButtonOctaQuad.Checked = true;
                    break;
                case motor_frame_class.MOTOR_FRAME_Y6:
                    radioButtonY6.Checked = true;
                    break;
                case motor_frame_class.MOTOR_FRAME_HELI:
                    radioButtonHeli.Checked = true;
                    break;
                case motor_frame_class.MOTOR_FRAME_TRI:
                    radioButtonTri.Checked = true;
                    break;
            }

            // disable all options
            radioButton_VTail.Enabled = false;
            radioButton_Plus.Enabled = false;
            radioButton_V.Enabled = false;
            radioButton_X.Enabled = false;
            radioButton_H.Enabled = false;
            radioButton_Y.Enabled = false;

            // disable all images
            FadePicBoxes(pictureBoxPlus, DisabledOpacity);
            FadePicBoxes(pictureBoxX, DisabledOpacity);
            FadePicBoxes(pictureBoxV, DisabledOpacity);
            FadePicBoxes(pictureBoxH, DisabledOpacity);
            FadePicBoxes(pictureBoxY, DisabledOpacity);
            FadePicBoxes(pictureBoxVTail, DisabledOpacity);

            // display only valid types
            foreach (var validtype in validtypes)
            {
                if (validtype.Item2 == motor_frame_type.MOTOR_FRAME_TYPE_PLUS)
                {
                    FadePicBoxes(pictureBoxPlus, EnabledOpacity);
                    radioButton_Plus.Enabled = true;
                }
                if (validtype.Item2 == motor_frame_type.MOTOR_FRAME_TYPE_X)
                {
                    FadePicBoxes(pictureBoxX, EnabledOpacity);
                    radioButton_X.Enabled = true;
                }
                if (validtype.Item2 == motor_frame_type.MOTOR_FRAME_TYPE_V)
                {
                    FadePicBoxes(pictureBoxV, EnabledOpacity);
                    radioButton_V.Enabled = true;
                }
                if (validtype.Item2 == motor_frame_type.MOTOR_FRAME_TYPE_H)
                {
                    FadePicBoxes(pictureBoxH, EnabledOpacity);
                    radioButton_H.Enabled = true;
                }
                if (validtype.Item2 == motor_frame_type.MOTOR_FRAME_TYPE_Y6B)
                {
                    FadePicBoxes(pictureBoxY, EnabledOpacity);
                    radioButton_Y.Enabled = true;
                }
                if (validtype.Item2 == motor_frame_type.MOTOR_FRAME_TYPE_VTAIL)
                {
                    FadePicBoxes(pictureBoxVTail, EnabledOpacity);
                    radioButton_VTail.Enabled = true;
                }
            }

            // set our new class
            SetFrameParam(work_frame_class, work_frame_type);

            inDoType = false;
        }

        private void DoType(motor_frame_type frame_type)
        {
            if (inDoType)
                return;

            // prevent recursive calls because we modify radiobuttons
            inDoType = true;
            work_frame_type = frame_type;

            switch (frame_type)
            {
                case motor_frame_type.MOTOR_FRAME_TYPE_PLUS:
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
                    SetFrameParam(work_frame_class, frame_type);
                    break;
                case motor_frame_type.MOTOR_FRAME_TYPE_X:
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
                    SetFrameParam(work_frame_class, frame_type);
                    break;
                case motor_frame_type.MOTOR_FRAME_TYPE_V:
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
                    SetFrameParam(work_frame_class, frame_type);
                    break;
                case motor_frame_type.MOTOR_FRAME_TYPE_H:
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
                    SetFrameParam(work_frame_class, frame_type);
                    break;
                case motor_frame_type.MOTOR_FRAME_TYPE_Y6B:
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
                    SetFrameParam(work_frame_class, frame_type);
                    break;
                case motor_frame_type.MOTOR_FRAME_TYPE_VTAIL:
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
                    SetFrameParam(work_frame_class, frame_type);
                    break;
                default:
                    radioButton_Plus.Checked = false;
                    radioButton_V.Checked = false;
                    radioButton_X.Checked = false;
                    radioButton_H.Checked = false;
                    radioButton_Y.Checked = false;
                    break;
            }
            inDoType = false;
        }

        private void SetFrameParam(motor_frame_class frame_class, motor_frame_type frame_type)
        {
            try
            {
                MainV2.comPort.setParam("FRAME_CLASS", (int)frame_class);
                MainV2.comPort.setParam("FRAME_TYPE", (int)frame_type);
            }
            catch
            {
                CustomMessageBox.Show(string.Format(Strings.ErrorSetValueFailed, "FRAME_CLASS OR FRAME_TYPE"), Strings.ERROR,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void FadePicBoxes(Control picbox, float Opacity)
        {
            var fade = new Transition(new TransitionType_Linear(400));
            fade.add(picbox, "Opacity", Opacity);
            fade.run();
        }

        private void radioButtonType_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == pictureBoxPlus || sender == radioButton_Plus)
                DoType(motor_frame_type.MOTOR_FRAME_TYPE_PLUS);
            if (sender == pictureBoxX || sender == radioButton_X)
                DoType(motor_frame_type.MOTOR_FRAME_TYPE_X);
            if (sender == pictureBoxV || sender == radioButton_V)
                DoType(motor_frame_type.MOTOR_FRAME_TYPE_V);
            if (sender == pictureBoxH || sender == radioButton_H)
                DoType(motor_frame_type.MOTOR_FRAME_TYPE_H);
            if (sender == pictureBoxY || sender == radioButton_Y)
                DoType(motor_frame_type.MOTOR_FRAME_TYPE_Y6B);
            if (sender == pictureBoxVTail || sender == radioButton_VTail)
                DoType(motor_frame_type.MOTOR_FRAME_TYPE_VTAIL);
        }

        private void radioButtonClass_CheckedChanged(object sender, EventArgs e)
        {
            if (sender == radioButtonUndef)
                DoClass(motor_frame_class.MOTOR_FRAME_UNDEFINED);
            if (sender == radioButtonQuad)
                DoClass(motor_frame_class.MOTOR_FRAME_QUAD);
            if (sender == radioButtonHexa)
                DoClass(motor_frame_class.MOTOR_FRAME_HEXA);
            if (sender == radioButtonOcta)
                DoClass(motor_frame_class.MOTOR_FRAME_OCTA);
            if (sender == radioButtonOctaQuad)
                DoClass(motor_frame_class.MOTOR_FRAME_OCTAQUAD);
            if (sender == radioButtonY6)
                DoClass(motor_frame_class.MOTOR_FRAME_Y6);
            if (sender == radioButtonHeli)
                DoClass(motor_frame_class.MOTOR_FRAME_HELI);
            if (sender == radioButtonTri)
                DoClass(motor_frame_class.MOTOR_FRAME_TRI);
        }
    }
}