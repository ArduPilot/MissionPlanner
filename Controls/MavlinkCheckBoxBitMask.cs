using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public class MavlinkCheckBoxBitMask : MyUserControl
    {
        public event EventValueChanged ValueChanged;

        [System.ComponentModel.Browsable(true)]
        public string ParamName { get; set; }

        [System.ComponentModel.Browsable(true)]
        public Hashtable param { get; set; }

        public float Value 
        {
            get
            {
                float answer = 0;

                for (int a = 0; a < chklist.Count; a++)
                {
                    answer += chklist[a].Value.Checked ? (1 << chklist[a].Key) : 0;
                }

                return answer;
            }
        }

        List<KeyValuePair<int, CheckBox>> chklist = new List<KeyValuePair<int, CheckBox>>();

        public MavlinkCheckBoxBitMask()
        {
            this.Enabled = false;
            this.Width = 700;
        }

        public void setup(string paramname, Hashtable paramlist)
        {
            this.ParamName = paramname;
            this.param = paramlist;

            if (paramlist.ContainsKey(paramname))
            {
                this.Enabled = true;

                var list = ParameterMetaDataRepository.GetParameterBitMaskInt(ParamName, MainV2.comPort.MAV.cs.firmware.ToString());

                int chkcount = list.Count;
                int leftside = 9;
                int top = 9;

                uint value = (uint)(float)paramlist[paramname];

                for (int a = 0; a < chkcount; a++)
                {
                    CheckBox chk = new CheckBox();
                    chk.AutoSize = true;
                    chk.Text = list[a].Value.ToString();
                    chk.Location = new System.Drawing.Point(leftside, top);

                    if ((value & ((uint)list[a].Key) << a) > 0)
                    {
                        chk.Checked = true;
                    }

                    chklist.Add(new KeyValuePair<int,CheckBox>(list[a].Key,chk));
                    this.Controls.Add(chk);

                    chk.CheckedChanged += MavlinkCheckBoxBitMask_CheckedChanged;

                    //this.Controls.Add(new Label() { Location = chk.Location, Text = "test" });

                    leftside += chk.Width + 5;
                    if (leftside > 500)
                    {
                        top += chk.Height + 5;
                        leftside = 9;
                    }
                }

                this.Height = top + 25;
            }
            else
            {
                this.Enabled = false;
            }
        }

        void MavlinkCheckBoxBitMask_CheckedChanged(object sender, EventArgs e)
        {
            if (this.ValueChanged != null)
            {
                this.ValueChanged(sender, ParamName, Value.ToString());
                return;
            }
            try
            {
                bool ans = MainV2.comPort.setParam(ParamName, Value);
                if (ans == false)
                    CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
            }
            catch 
            { 
                CustomMessageBox.Show(String.Format(Strings.ErrorSetValueFailed, ParamName), Strings.ERROR);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MavlinkCheckBoxBitMask
            // 
            this.Name = "MavlinkCheckBoxBitMask";
            this.Size = new System.Drawing.Size(217, 111);
            this.ResumeLayout(false);

        }

        
    }
}
