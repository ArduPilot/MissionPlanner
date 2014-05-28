using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;

namespace MissionPlanner.Warnings
{
    public class WarningControl: UserControl
    {
        public WarningControl(CustomWarning item)
        {
            InitializeComponent();

            item.SetSource(MainV2.comPort.MAV.cs);
            item.SetField(item.Name);

            CMB_condition.DataSource = Enum.GetNames(typeof(CustomWarning.Conditional));

            CMB_Source.DataSource = item.GetOptions();



            custwarning = item;

            updateDisplay();
        }

        public void updateDisplay()
        {
            CMB_condition.Text = custwarning.ConditionType.ToString();
            CMB_Source.Text = custwarning.Name;
            NUM_warning.Value = (decimal)custwarning.Warning;
            NUM_repeattime.Value = custwarning.RepeatTime;
            TXT_warningtext.Text = custwarning.Text;
        }

        CustomWarning _custwarn;
        public CustomWarning custwarning 
        { 
            get 
            {
                return _custwarn; 
            }
            set 
            {
                _custwarn = value;
            }
        }

        private ComboBox CMB_condition;
        private NumericUpDown NUM_warning;
        private NumericUpDown NUM_repeattime;
        private TextBox TXT_warningtext;
        private ComboBox CMB_Source;
        // posible child
        //private CustomWarning item;
        //private CurrentState currentState;
    
        private void InitializeComponent()
        {
            this.CMB_Source = new System.Windows.Forms.ComboBox();
            this.CMB_condition = new System.Windows.Forms.ComboBox();
            this.NUM_warning = new System.Windows.Forms.NumericUpDown();
            this.NUM_repeattime = new System.Windows.Forms.NumericUpDown();
            this.TXT_warningtext = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_warning)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_repeattime)).BeginInit();
            this.SuspendLayout();
            // 
            // CMB_Source
            // 
            this.CMB_Source.FormattingEnabled = true;
            this.CMB_Source.Location = new System.Drawing.Point(3, 2);
            this.CMB_Source.Name = "CMB_Source";
            this.CMB_Source.Size = new System.Drawing.Size(121, 21);
            this.CMB_Source.TabIndex = 0;
            this.CMB_Source.Text = "hdop";
            this.CMB_Source.SelectedIndexChanged += new System.EventHandler(this.CMB_Source_SelectedIndexChanged);
            // 
            // CMB_condition
            // 
            this.CMB_condition.FormattingEnabled = true;
            this.CMB_condition.Location = new System.Drawing.Point(130, 2);
            this.CMB_condition.Name = "CMB_condition";
            this.CMB_condition.Size = new System.Drawing.Size(54, 21);
            this.CMB_condition.TabIndex = 1;
            this.CMB_condition.Text = "EQ";
            this.CMB_condition.SelectedIndexChanged += new System.EventHandler(this.CMB_condition_SelectedIndexChanged);
            // 
            // NUM_warning
            // 
            this.NUM_warning.DecimalPlaces = 2;
            this.NUM_warning.Location = new System.Drawing.Point(190, 3);
            this.NUM_warning.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.NUM_warning.Minimum = new decimal(new int[] {
            99999,
            0,
            0,
            -2147483648});
            this.NUM_warning.Name = "NUM_warning";
            this.NUM_warning.Size = new System.Drawing.Size(65, 20);
            this.NUM_warning.TabIndex = 2;
            this.NUM_warning.ValueChanged += new System.EventHandler(this.NUM_warning_ValueChanged);
            // 
            // NUM_repeattime
            // 
            this.NUM_repeattime.Location = new System.Drawing.Point(503, 3);
            this.NUM_repeattime.Name = "NUM_repeattime";
            this.NUM_repeattime.Size = new System.Drawing.Size(39, 20);
            this.NUM_repeattime.TabIndex = 3;
            this.NUM_repeattime.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUM_repeattime.ValueChanged += new System.EventHandler(this.NUM_repeattime_ValueChanged);
            // 
            // TXT_warningtext
            // 
            this.TXT_warningtext.Location = new System.Drawing.Point(261, 2);
            this.TXT_warningtext.Name = "TXT_warningtext";
            this.TXT_warningtext.Size = new System.Drawing.Size(236, 20);
            this.TXT_warningtext.TabIndex = 4;
            this.TXT_warningtext.Text = "WARNING: {name} is {value}";
            this.TXT_warningtext.TextChanged += new System.EventHandler(this.TXT_warningtext_TextChanged);
            // 
            // WarningControl
            // 
            this.Controls.Add(this.TXT_warningtext);
            this.Controls.Add(this.NUM_repeattime);
            this.Controls.Add(this.NUM_warning);
            this.Controls.Add(this.CMB_condition);
            this.Controls.Add(this.CMB_Source);
            this.Name = "WarningControl";
            this.Size = new System.Drawing.Size(548, 27);
            ((System.ComponentModel.ISupportInitialize)(this.NUM_warning)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_repeattime)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void CMB_Source_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (custwarning != null)
            custwarning.SetField(CMB_Source.Text);
        }

        private void CMB_condition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (custwarning != null)
            custwarning.ConditionType = (CustomWarning.Conditional)Enum.Parse(typeof(CustomWarning.Conditional), CMB_condition.Text);
        }

        private void NUM_warning_ValueChanged(object sender, EventArgs e)
        {
            if (custwarning != null)
            custwarning.Warning = (double)NUM_warning.Value;
        }

        private void TXT_warningtext_TextChanged(object sender, EventArgs e)
        {
            if (custwarning != null)
            custwarning.Text = TXT_warningtext.Text;
        }

        private void NUM_repeattime_ValueChanged(object sender, EventArgs e)
        {
            if (custwarning != null)
            custwarning.RepeatTime = (int)NUM_repeattime.Value;
        }
    }
}
