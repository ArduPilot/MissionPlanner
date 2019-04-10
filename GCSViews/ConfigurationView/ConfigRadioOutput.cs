using System;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigRadioOutput : MyUserControl, IActivate, IDeactivate
    {
        public ConfigRadioOutput()
        {
            InitializeComponent();

            bindingSource1.DataSource = typeof(CurrentState);

            SuspendLayout();
            setup(BAR1, rev1, func1, min1, trim1, max1, 1);
            setup(BAR2, rev2, func2, min2, trim2, max2, 2);
            setup(BAR3, rev3, func3, min3, trim3, max3, 3);
            setup(BAR4, rev4, func4, min4, trim4, max4, 4);
            setup(BAR5, rev5, func5, min5, trim5, max5, 5);
            setup(BAR6, rev6, func6, min6, trim6, max6, 6);
            setup(BAR7, rev7, func7, min7, trim7, max7, 7);
            setup(BAR8, rev8, func8, min8, trim8, max8, 8);
            ResumeLayout(true);
        }

        private void setup(HorizontalProgressBar2 bAR1, MavlinkCheckBox rev1, MavlinkComboBox func1,
            MavlinkNumericUpDown min1, MavlinkNumericUpDown trim1, MavlinkNumericUpDown max1, int servono)
        {
            var servo = String.Format("SERVO{0}", servono);

            bAR1.DataBindings.Add("Value", bindingSource1, "ch" + servono + "out");
            rev1.setup(1, 0, servo + "_REVERSED", MainV2.comPort.MAV.param);
            func1.setup(ParameterMetaDataRepository.GetParameterOptionsInt(servo + "_FUNCTION",
                    MainV2.comPort.MAV.cs.firmware.ToString()), servo + "_FUNCTION", MainV2.comPort.MAV.param);
            min1.setup(800, 2200, 1, 1, servo + "_MIN", MainV2.comPort.MAV.param);
            trim1.setup(800, 2200, 1, 1, servo + "_TRIM", MainV2.comPort.MAV.param);
            max1.setup(800, 2200, 1, 1, servo + "_MAX", MainV2.comPort.MAV.param);
        }

        public void Activate()
        {
            timer1.Start();
        }

        public void Deactivate()
        {
            timer1.Stop();
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                MainV2.comPort.MAV.cs.UpdateCurrentSettings(bindingSource1.UpdateDataSource(MainV2.comPort.MAV.cs));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}