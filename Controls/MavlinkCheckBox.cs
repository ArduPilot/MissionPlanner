using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace MissionPlanner.Controls
{
    public class MavlinkCheckBox : CheckBox
    {
        public new event EventHandler CheckedChanged;

        [System.ComponentModel.Browsable(true)]
        public double OnValue { get; set; }

        [System.ComponentModel.Browsable(true)]
        public double OffValue { get; set; }

        [System.ComponentModel.Browsable(true)]
        public string ParamName { get; set; }


        Control _control;

        public MavlinkCheckBox()
        {
            OnValue = 1;
            OffValue = 0;

            this.Enabled = false;
        }

        public void setup(double OnValue, double OffValue, string paramname, MAVLink.MAVLinkParamList paramlist,
            Control enabledisable = null)
        {
            base.CheckedChanged -= MavlinkCheckBox_CheckedChanged;

            this.OnValue = OnValue;
            this.OffValue = OffValue;
            this.ParamName = paramname;
            this._control = enabledisable;

            if (paramlist.ContainsKey(paramname))
            {
                this.Enabled = true;
                this.Visible = true;

                if (paramlist[paramname].Value == OnValue)
                {
                    this.Checked = true;
                    enableControl(true);
                }
                else if (paramlist[paramname].Value == OffValue)
                {
                    this.Checked = false;
                    enableControl(false);
                }
                else
                {
                    this.CheckState = System.Windows.Forms.CheckState.Indeterminate;
                    enableControl(false);
                }
            }
            else
            {
                this.Enabled = false;
            }

            base.CheckedChanged += new EventHandler(MavlinkCheckBox_CheckedChanged);
        }

        void enableControl(bool enable)
        {
            if (_control != null)
                _control.Enabled = enable;
        }

        void MavlinkCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (this.CheckedChanged != null)
                this.CheckedChanged(sender, e);

            if (this.Checked)
            {
                enableControl(true);
                try
                {
                    bool ans = MainV2.comPort.setParam(ParamName, OnValue);
                    if (ans == false)
                        CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }
                catch
                {
                    CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }
            }
            else
            {
                enableControl(false);
                try
                {
                    bool ans = MainV2.comPort.setParam(ParamName, OffValue);
                    if (ans == false)
                        CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }
                catch
                {
                    CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }
            }
        }
    }
}