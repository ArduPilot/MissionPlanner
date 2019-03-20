using System;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.Wizard
{
    public partial class _8OptionalItemsAP : MyUserControl, IWizard, IActivate, IDeactivate
    {
        public _8OptionalItemsAP()
        {
            InitializeComponent();
        }

        void updateosd()
        {
            try
            {
                MainV2.comPort.setParam("SR0_EXT_STAT", 2);
                MainV2.comPort.setParam("SR0_EXTRA1", 2);
                MainV2.comPort.setParam("SR0_EXTRA2", 2);
                MainV2.comPort.setParam("SR0_EXTRA3", 2);
                MainV2.comPort.setParam("SR0_POSITION", 2);
                MainV2.comPort.setParam("SR0_RAW_CTRL", 2);
                MainV2.comPort.setParam("SR0_RAW_SENS", 2);
                MainV2.comPort.setParam("SR0_RC_CHAN", 2);

                MainV2.comPort.setParam("SR1_EXT_STAT", 2);
                MainV2.comPort.setParam("SR1_EXTRA1", 2);
                MainV2.comPort.setParam("SR1_EXTRA2", 2);
                MainV2.comPort.setParam("SR1_EXTRA3", 2);
                MainV2.comPort.setParam("SR1_POSITION", 2);
                MainV2.comPort.setParam("SR1_RAW_CTRL", 2);
                MainV2.comPort.setParam("SR1_RAW_SENS", 2);
                MainV2.comPort.setParam("SR1_RC_CHAN", 2);

                MainV2.comPort.setParam("SR3_EXT_STAT", 2);
                MainV2.comPort.setParam("SR3_EXTRA1", 2);
                MainV2.comPort.setParam("SR3_EXTRA2", 2);
                MainV2.comPort.setParam("SR3_EXTRA3", 2);
                MainV2.comPort.setParam("SR3_POSITION", 2);
                MainV2.comPort.setParam("SR3_RAW_CTRL", 2);
                MainV2.comPort.setParam("SR3_RAW_SENS", 2);
                MainV2.comPort.setParam("SR3_RC_CHAN", 2);
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.ErrorSettingParameter + ex.ToString(), Strings.ERROR);
            }

            connected();
        }

        void connected()
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                CustomMessageBox.Show(Strings.ErrorNotConnected, Strings.ERROR);
                Wizard.instance.Close();
            }
        }

        public void Activate()
        {
            connected();

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
                if (Wizard.instance.IsHandleCreated)
                    Wizard.instance.BeginInvoke((MethodInvoker) delegate { Wizard.instance.GoNext(1, false); });
            }
        }

        public void Deactivate()
        {
            connected();

            timer1.Stop();
        }

        public int WizardValidate()
        {
            connected();

            timer1.Stop();

            updateosd();

            return 1;
        }

        public bool WizardBusy()
        {
            return false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            LBL_airspeed.Text = MainV2.comPort.MAV.cs.airspeed.ToString("0.00") + " m/s";
        }
    }
}