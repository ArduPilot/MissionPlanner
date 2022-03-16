using System;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public class MavlinkCheckBox : CheckBox
    {
        public new event EventHandler CheckedChanged;

        private Action CallBackOnChange;

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

        public void setup(double[] OnValue, double[] OffValue, string[] paramname, MAVLink.MAVLinkParamList paramlist,
            Control enabledisable = null)
        {
            int idx = 0;
            foreach (var s in paramname)
            {
                if (paramlist.ContainsKey(s))
                {
                    setup(OnValue[idx], OffValue[idx], s, paramlist, enabledisable);
                    return;
                }
                idx++;
            }
        }

        public void setup(double OnValue, double OffValue, string[] paramname, MAVLink.MAVLinkParamList paramlist,
            Control enabledisable = null, Action callbackonchange = null)
        {
            foreach (var s in paramname)
            {
                if (paramlist.ContainsKey(s))
                {
                    setup(OnValue, OffValue, s, paramlist, enabledisable, callbackonchange);
                    return;
                }
            }
        }

        public void setup(double OnValue, double OffValue, string paramname, MAVLink.MAVLinkParamList paramlist,
            Control enabledisable = null, Action callbackonchange = null)
        {
            base.CheckedChanged -= MavlinkCheckBox_CheckedChanged;

            this.CallBackOnChange = callbackonchange;
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
                    enableBGControl(true);
                }
                else if (paramlist[paramname].Value == OffValue)
                {
                    this.Checked = false;
                    enableBGControl(false);
                }
                else
                {
                    this.CheckState = System.Windows.Forms.CheckState.Indeterminate;
                    enableBGControl(false);
                }
            }
            else
            {
                this.Enabled = false;
            }

            base.CheckedChanged += new EventHandler(MavlinkCheckBox_CheckedChanged);
        }

        void enableBGControl(bool enable)
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
                enableBGControl(true);
                try
                {
                    bool ans = MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, ParamName, OnValue);
                    if (ans == false)
                        CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                    else
                        CallBackOnChange?.Invoke();
                }
                catch
                {
                    CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }
            }
            else
            {
                enableBGControl(false);
                try
                {
                    bool ans = MainV2.comPort.setParam((byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent, ParamName, OffValue);
                    if (ans == false)
                        CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                    else
                        CallBackOnChange?.Invoke();
                }
                catch
                {
                    CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }
            }
        }
    }
}