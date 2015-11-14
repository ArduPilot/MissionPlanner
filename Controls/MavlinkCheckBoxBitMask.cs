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
        private TableLayoutPanel tableLayoutPanel1;
        public Label label1;
        public MyLabel myLabel1;
        public Panel panel1;
        public event EventValueChanged ValueChanged;
        List<KeyValuePair<int, string>> list;
        int chkcount;

        [System.ComponentModel.Browsable(true)]
        public string ParamName { get; set; }


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
            set
            {
                for (int a = 0; a < chkcount; a++)
                {
                    CheckBox chk = (CheckBox) panel1.Controls[a];


                    chk.Checked = (((uint) value & (1 << list[a].Key)) > 0);
                }
            }
        }

        List<KeyValuePair<int, CheckBox>> chklist = new List<KeyValuePair<int, CheckBox>>();

        public MavlinkCheckBoxBitMask()
        {
            InitializeComponent();

            this.Enabled = false;
            this.Width = 700;
        }

        public void setup(string paramname, MAVLink.MAVLinkParamList paramlist)
        {
            this.ParamName = paramname;

            if (paramlist.ContainsKey(paramname))
            {
                this.Enabled = true;

                list = ParameterMetaDataRepository.GetParameterBitMaskInt(ParamName,
                    MainV2.comPort.MAV.cs.firmware.ToString());
                chkcount = list.Count;

                int leftside = 9;
                int top = 9;

                uint value = (uint) paramlist[paramname].Value;

                for (int a = 0; a < chkcount; a++)
                {
                    CheckBox chk = new CheckBox();
                    chk.AutoSize = true;
                    chk.Text = list[a].Value.ToString();
                    chk.Location = new System.Drawing.Point(leftside, top);

                    chk.CheckedChanged -= MavlinkCheckBoxBitMask_CheckedChanged;

                    if ((value & (1 << list[a].Key)) > 0)
                    {
                        chk.Checked = true;
                    }

                    chklist.Add(new KeyValuePair<int, CheckBox>(list[a].Key, chk));
                    panel1.Controls.Add(chk);

                    chk.CheckedChanged += MavlinkCheckBoxBitMask_CheckedChanged;

                    //this.Controls.Add(new Label() { Location = chk.Location, Text = "test" });

                    leftside += chk.Width + 5;
                    if (leftside > 500)
                    {
                        top += chk.Height + 5;
                        leftside = 9;
                    }
                }


                this.panel1.Height = top + 25;

                //this.Height = top + 25;
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.myLabel1 = new MissionPlanner.Controls.MyLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                    ((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                       | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 28);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 10);
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(337, 84);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 21);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(331, 50);
            this.panel1.TabIndex = 1;
            // 
            // myLabel1
            // 
            this.myLabel1.Anchor =
                ((System.Windows.Forms.AnchorStyles)
                    (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                      | System.Windows.Forms.AnchorStyles.Right)));
            this.myLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.myLabel1.Location = new System.Drawing.Point(3, 3);
            this.myLabel1.Name = "myLabel1";
            this.myLabel1.resize = false;
            this.myLabel1.Size = new System.Drawing.Size(337, 23);
            this.myLabel1.TabIndex = 3;
            this.myLabel1.Text = "myLabel1";
            // 
            // MavlinkCheckBoxBitMask
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.myLabel1);
            this.Name = "MavlinkCheckBoxBitMask";
            this.Size = new System.Drawing.Size(343, 115);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}