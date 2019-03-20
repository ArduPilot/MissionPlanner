using System;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.Wizard
{
    public partial class _2FrameFW : MyUserControl, IWizard
    {
        public _2FrameFW()
        {
            InitializeComponent();
        }

        public int WizardValidate()
        {
            if (Wizard.config.ContainsKey("fwframe"))
            {
                if (MainV2.comPort.BaseStream.IsOpen)
                    return 2;

                return 1;
            }
            return 0;
        }

        public bool WizardBusy()
        {
            return false;
        }

        void setfwframe(object sender)
        {
            // only remembers the last selected item
            Wizard.config["fwframe"] = ((Control) sender).Tag.ToString();
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            DeselectAll();
            (sender as PictureBoxMouseOver).selected = true;
            setfwframe(sender);
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
    }
}