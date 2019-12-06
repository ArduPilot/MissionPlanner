using System;
using System.Windows.Forms;
using MissionPlanner.Utilities;
using log4net;
using System.Reflection;

namespace MissionPlanner.Controls
{
    public class MavlinkNumericUpDown : NumericUpDown
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        [System.ComponentModel.Browsable(true)]
        public float Min { get; set; }

        [System.ComponentModel.Browsable(true)]
        public float Max { get; set; }

        [System.ComponentModel.Browsable(true)]
        public string ParamName { get; set; }


        Control _control;
        float _scale = 1;

        Timer timer = new Timer();

        [System.ComponentModel.Browsable(true)]
        public event EventHandler ValueUpdated;

        public MavlinkNumericUpDown()
        {
            Min = 0;
            Max = 1;

            this.Name = "MavlinkNumericUpDown";

            timer.Tick += Timer_Tick;

            this.Enabled = false;
        }

        public void setup(float Min, float Max, float Scale, float Increment, string paramname,
            MAVLink.MAVLinkParamList paramlist, Control enabledisable = null)
        {
            setup(Min, Max, Scale, Increment, new string[] {paramname}, paramlist, enabledisable);
        }

        public void setup(float Min, float Max, float Scale, float Increment, string[] paramname,
            MAVLink.MAVLinkParamList paramlist, Control enabledisable = null)
        {
            this.ValueChanged -= MavlinkNumericUpDown_ValueChanged;

            // default to first item
            this.ParamName = paramname[0];
            // set a new item is first item doesnt exist
            foreach (var paramn in paramname)
            {
                if (paramlist.ContainsKey(paramn))
                {
                    this.ParamName = paramn;
                    break;
                }
            }

            // update local name
            Name = ParamName;
            // set min and max of both are equal
            if (Min == Max)
            {
                double mint = Min, maxt = Max;
                ParameterMetaDataRepository.GetParameterRange(ParamName, ref mint, ref maxt,
                    MainV2.comPort.MAV.cs.firmware.ToString());
                Min = (float) mint;
                Max = (float) maxt;
                if (Min == Max)
                    log.InfoFormat("{0} {1} = {2}", ParamName, Min, Max);
            }

            if (Increment == 0)
            {
                double Inc = 0;
                if (ParameterMetaDataRepository.GetParameterIncrement(ParamName, ref Inc,
                    MainV2.comPort.MAV.cs.firmware.ToString()))
                    Increment = (float) Inc;
            }

            _scale = Scale;
            this.Minimum = (decimal) (Min);
            this.Maximum = (decimal) (Max);
            this.Increment = (decimal) (Increment);
            this.DecimalPlaces = BitConverter.GetBytes(decimal.GetBits((decimal) Increment)[3])[2];

            this._control = enabledisable;

            if (paramlist.ContainsKey(ParamName))
            {
                this.Enabled = true;
                this.Visible = true;

                enableControl(true);

                decimal value = (decimal) ((float) paramlist[ParamName]/_scale);

                int dec = BitConverter.GetBytes(decimal.GetBits((decimal) value)[3])[2];

                if (dec > this.DecimalPlaces)
                    this.DecimalPlaces = dec;

                if (value < this.Minimum)
                    this.Minimum = value;
                if (value > this.Maximum)
                    this.Maximum = value;

                base.Value = value;
            }
            else
            {
                this.Enabled = false;
                enableControl(false);
            }

            this.ValueChanged += new EventHandler(MavlinkNumericUpDown_ValueChanged);
        }

        void enableControl(bool enable)
        {
            if (_control != null)
            {
                _control.Enabled = enable;
                _control.Visible = true;
            }
        }

        void MavlinkNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            string value = base.Text;
            if (decimal.Parse(value) > base.Maximum)
            {
                if (
                    CustomMessageBox.Show(ParamName + " Value out of range\nDo you want to accept the new value?",
                        "Out of range", MessageBoxButtons.YesNo) == (int)DialogResult.Yes)
                {
                    base.Maximum = decimal.Parse(value);
                    base.Value = decimal.Parse(value);
                }
            }

            if (ValueUpdated != null)
            {
                this.UpdateEditText();
                ValueUpdated(this, new MAVLinkParamChanged(ParamName, (float) base.Value*(float) _scale));
                return;
            }

            lock (timer)
            {
                timer.Interval = 300;

                if (!timer.Enabled)
                    timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            lock (timer)
            {
                try
                {
                    bool ans = MainV2.comPort.setParam(ParamName, (float) base.Value * (float) _scale);
                    if (ans == false)
                        CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }
                catch
                {
                    CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }

                timer.Stop();
            }
        }
    }
}