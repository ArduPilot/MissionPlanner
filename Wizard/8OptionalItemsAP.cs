using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega.Controls;
using ArdupilotMega.Controls.BackstageView;

namespace ArdupilotMega.Wizard
{
    public partial class _8OptionalItemsAP : MyUserControl, IWizard, IActivate, IDeactivate
    {
        public _8OptionalItemsAP()
        {
            InitializeComponent();
        }

        void updateosd()
        {

            MainV2.comPort.setParam("SR0_EXT_STAT", 2);
            MainV2.comPort.setParam("SR0_EXTRA1", 2);
            MainV2.comPort.setParam("SR0_EXTRA2", 2);
            MainV2.comPort.setParam("SR0_EXTRA3", 2);
            MainV2.comPort.setParam("SR0_POSITION", 2);
            MainV2.comPort.setParam("SR0_RAW_CTRL", 2);
            MainV2.comPort.setParam("SR0_RAW_SENS", 2);
            MainV2.comPort.setParam("SR0_RC_CHAN", 2);

            MainV2.comPort.setParam("SR3_EXT_STAT", 2);
            MainV2.comPort.setParam("SR3_EXTRA1", 2);
            MainV2.comPort.setParam("SR3_EXTRA2", 2);
            MainV2.comPort.setParam("SR3_EXTRA3", 2);
            MainV2.comPort.setParam("SR3_POSITION", 2);
            MainV2.comPort.setParam("SR3_RAW_CTRL", 2);
            MainV2.comPort.setParam("SR3_RAW_SENS", 2);
            MainV2.comPort.setParam("SR3_RC_CHAN", 2);
        }

        public void Activate()
        {

            if (MainV2.comPort.MAV.param.ContainsKey("ARSPD_ENABLE"))
            {
                CHK_airspeeduse.setup(1, 0, "ARSPD_USE", MainV2.comPort.MAV.param);
                CHK_enableairspeed.setup(1, 0, "ARSPD_ENABLE", MainV2.comPort.MAV.param);

                timer1.Start();
            }
            else
            {
                // no airspeed - keep going
                updateosd();
                Wizard.instance.BeginInvoke((MethodInvoker)delegate
                {
                    Wizard.instance.GoNext(1,false);
                });
            } 
        }

        public void Deactivate()
        {
            timer1.Stop();
        }

        public int WizardValidate()
        {
            timer1.Stop();

            updateosd();

            return 1;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            LBL_airspeed.Text = MainV2.comPort.MAV.cs.airspeed.ToString("0.00") + " m/s";
        }
    }
}
