using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace MissionPlanner.Controls
{
    public class MavlinkComboBox : ComboBox
    {
        public new event EventHandler SelectedIndexChanged;

        [System.ComponentModel.Browsable(true)]
        public string ParamName { get; set; }


        Control _control;
        Type _source;
        List<KeyValuePair<int, string>> _source2;
        string paramname2 = "";

        public Control SubControl
        {
            get { return _control; }
            set { _control = value; }
        }

        [System.ComponentModel.Browsable(true)]
        public event EventHandler ValueUpdated;

        public MavlinkComboBox()
        {
            this.Enabled = false;
            this.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        public void setup(List<KeyValuePair<int, string>> source, string paramname, MAVLink.MAVLinkParamList paramlist)
            //, string paramname2 = "", Control enabledisable = null)
        {
            base.SelectedIndexChanged -= MavlinkComboBox_SelectedIndexChanged;

            _source2 = source;

            this.DisplayMember = "Value";
            this.ValueMember = "Key";
            this.DataSource = source;

            this.ParamName = paramname;


            if (paramlist.ContainsKey(paramname))
            {
                this.Enabled = true;
                this.Visible = true;

                enableControl(true);

                var item = paramlist[paramname];

                this.SelectedValue = (int) paramlist[paramname].Value;
            }

            base.SelectedIndexChanged += new EventHandler(MavlinkComboBox_SelectedIndexChanged);
        }


        public void setup(Type source, string paramname, MAVLink.MAVLinkParamList paramlist)
            //, string paramname2 = "", Control enabledisable = null)
        {
            base.SelectedIndexChanged -= MavlinkComboBox_SelectedIndexChanged;

            _source = source;

            this.DataSource = Enum.GetNames(source);

            this.ParamName = paramname;

            if (paramlist.ContainsKey(paramname))
            {
                this.Enabled = true;
                this.Visible = true;

                enableControl(true);

                this.Text = Enum.GetName(source, (Int32) paramlist[paramname].Value);
            }

            base.SelectedIndexChanged += new EventHandler(MavlinkComboBox_SelectedIndexChanged);
        }

        void enableControl(bool enable)
        {
            if (_control != null)
                _control.Enabled = enable;
        }

        void MavlinkComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.SelectedIndexChanged != null)
                this.SelectedIndexChanged(sender, e);

            if (_source != null)
            {
                try
                {
                    if (ValueUpdated != null)
                    {
                        ValueUpdated(this,
                            new MAVLinkParamChanged(ParamName, (float) (Int32) Enum.Parse(_source, this.Text)));
                        return;
                    }

                    if (!MainV2.comPort.setParam(ParamName, (float) (Int32) Enum.Parse(_source, this.Text)))
                    {
                        CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                    }

                    if (paramname2 != "")
                    {
                        if (
                            !MainV2.comPort.setParam(paramname2,
                                (float) (Int32) Enum.Parse(_source, this.Text) > 0 ? 1 : 0))
                        {
                            CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, paramname2), Strings.ERROR);
                        }
                    }
                }
                catch
                {
                    CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
                }
            }
            else if (_source2 != null)
            {
                try
                {
                    if (ValueUpdated != null)
                    {
                        ValueUpdated(this,
                            new MAVLinkParamChanged(ParamName, (float) (int) ((MavlinkComboBox) sender).SelectedValue));
                        return;
                    }

                    if (!MainV2.comPort.setParam(ParamName, (float) (int) ((MavlinkComboBox) sender).SelectedValue))
                    {
                        CustomMessageBox.Show("Set " + ParamName + " Failed!", Strings.ERROR);
                    }

                    if (paramname2 != "")
                    {
                        if (
                            !MainV2.comPort.setParam(paramname2,
                                (float) (int) ((MavlinkComboBox) sender).SelectedValue > 0 ? 1 : 0))
                        {
                            CustomMessageBox.Show("Set " + paramname2 + " Failed!", Strings.ERROR);
                        }
                    }
                }
                catch
                {
                    CustomMessageBox.Show("Set " + ParamName + " Failed!", Strings.ERROR);
                }
            }
        }
    }
}