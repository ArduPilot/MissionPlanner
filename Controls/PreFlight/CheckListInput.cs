using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.Drawing;

namespace MissionPlanner.Controls.PreFlight
{
    public class CheckListInput : UserControl
    {
        public event EventHandler ReloadList;

        CheckListControl _parent;

        public CheckListInput(CheckListControl parent)
        {
            _parent = parent;

            InitializeComponent();
        }

        public CheckListInput(CheckListControl parent, CheckListItem item)
        {
            _parent = parent;

            InitializeComponent();

            CheckListItem.defaultsrc = MainV2.comPort.MAV.cs;
            item.SetField(item.Name);

            CMB_condition.DataSource = Enum.GetNames(typeof(CheckListItem.Conditional));

            CMB_Source.DataSource = item.GetOptions();

            CMB_colour1.DataSource = Enum.GetNames(typeof(KnownColor));
            CMB_colour2.DataSource = Enum.GetNames(typeof(KnownColor));

            CheckListItem = item;

            updateDisplay();
        }

        public void updateDisplay()
        {
            CMB_condition.Text = CheckListItem.ConditionType.ToString();
            CMB_Source.Text = CheckListItem.Name;
            NUM_trigger.Value = (decimal) CheckListItem.TriggerValue;
            TXT_text.Text = CheckListItem.Text;
            TXT_desc.Text = CheckListItem.Description;
            CMB_colour1.SelectedItem = CheckListItem.TrueColor;
            CMB_colour2.SelectedItem = CheckListItem.FalseColor;
        }

        CheckListItem _checklistitem;

        public CheckListItem CheckListItem
        {
            get { return _checklistitem; }
            set { _checklistitem = value; }
        }

        private ComboBox CMB_condition;
        private NumericUpDown NUM_trigger;
        public TextBox TXT_text;
        private Controls.MyButton but_addchild;
        private Controls.MyButton but_remove;
        public ComboBox CMB_colour1;
        public ComboBox CMB_colour2;
        public TextBox TXT_desc;
        private ComboBox CMB_Source;

        private void InitializeComponent()
        {
            this.CMB_Source = new System.Windows.Forms.ComboBox();
            this.CMB_condition = new System.Windows.Forms.ComboBox();
            this.NUM_trigger = new System.Windows.Forms.NumericUpDown();
            this.TXT_text = new System.Windows.Forms.TextBox();
            this.but_addchild = new MissionPlanner.Controls.MyButton();
            this.but_remove = new MissionPlanner.Controls.MyButton();
            this.CMB_colour1 = new System.Windows.Forms.ComboBox();
            this.CMB_colour2 = new System.Windows.Forms.ComboBox();
            this.TXT_desc = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_trigger)).BeginInit();
            this.SuspendLayout();
            // 
            // CMB_Source
            // 
            this.CMB_Source.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_Source.FormattingEnabled = true;
            this.CMB_Source.Location = new System.Drawing.Point(3, 2);
            this.CMB_Source.Name = "CMB_Source";
            this.CMB_Source.Size = new System.Drawing.Size(121, 21);
            this.CMB_Source.TabIndex = 0;
            this.CMB_Source.SelectedIndexChanged += new System.EventHandler(this.CMB_Source_SelectedIndexChanged);
            // 
            // CMB_condition
            // 
            this.CMB_condition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_condition.FormattingEnabled = true;
            this.CMB_condition.Location = new System.Drawing.Point(130, 2);
            this.CMB_condition.Name = "CMB_condition";
            this.CMB_condition.Size = new System.Drawing.Size(54, 21);
            this.CMB_condition.TabIndex = 1;
            this.CMB_condition.SelectedIndexChanged += new System.EventHandler(this.CMB_condition_SelectedIndexChanged);
            // 
            // NUM_trigger
            // 
            this.NUM_trigger.DecimalPlaces = 2;
            this.NUM_trigger.Location = new System.Drawing.Point(190, 3);
            this.NUM_trigger.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.NUM_trigger.Minimum = new decimal(new int[] {
            99999,
            0,
            0,
            -2147483648});
            this.NUM_trigger.Name = "NUM_trigger";
            this.NUM_trigger.Size = new System.Drawing.Size(65, 20);
            this.NUM_trigger.TabIndex = 2;
            this.NUM_trigger.ValueChanged += new System.EventHandler(this.NUM_warning_ValueChanged);
            // 
            // TXT_text
            // 
            this.TXT_text.Location = new System.Drawing.Point(261, 29);
            this.TXT_text.Name = "TXT_text";
            this.TXT_text.Size = new System.Drawing.Size(236, 20);
            this.TXT_text.TabIndex = 4;
            this.TXT_text.Text = "{name} is {value}";
            this.TXT_text.TextChanged += new System.EventHandler(this.TXT_warningtext_TextChanged);
            // 
            // but_addchild
            // 
            this.but_addchild.Location = new System.Drawing.Point(631, 1);
            this.but_addchild.Name = "but_addchild";
            this.but_addchild.Size = new System.Drawing.Size(25, 20);
            this.but_addchild.TabIndex = 7;
            this.but_addchild.Text = "+";
            this.but_addchild.UseVisualStyleBackColor = true;
            this.but_addchild.Click += new System.EventHandler(this.but_addchild_Click);
            // 
            // but_remove
            // 
            this.but_remove.Location = new System.Drawing.Point(662, 1);
            this.but_remove.Name = "but_remove";
            this.but_remove.Size = new System.Drawing.Size(25, 20);
            this.but_remove.TabIndex = 8;
            this.but_remove.Text = "-";
            this.but_remove.UseVisualStyleBackColor = true;
            this.but_remove.Click += new System.EventHandler(this.but_remove_Click);
            // 
            // CMB_colour1
            // 
            this.CMB_colour1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_colour1.DropDownWidth = 100;
            this.CMB_colour1.FormattingEnabled = true;
            this.CMB_colour1.Location = new System.Drawing.Point(503, 1);
            this.CMB_colour1.Name = "CMB_colour1";
            this.CMB_colour1.Size = new System.Drawing.Size(50, 21);
            this.CMB_colour1.TabIndex = 5;
            this.CMB_colour1.SelectedIndexChanged += new System.EventHandler(this.CMB_colour1_SelectedIndexChanged);
            // 
            // CMB_colour2
            // 
            this.CMB_colour2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_colour2.DropDownWidth = 100;
            this.CMB_colour2.FormattingEnabled = true;
            this.CMB_colour2.Location = new System.Drawing.Point(559, 1);
            this.CMB_colour2.Name = "CMB_colour2";
            this.CMB_colour2.Size = new System.Drawing.Size(50, 21);
            this.CMB_colour2.TabIndex = 6;
            this.CMB_colour2.SelectedIndexChanged += new System.EventHandler(this.CMB_colour2_SelectedIndexChanged);
            // 
            // TXT_desc
            // 
            this.TXT_desc.Location = new System.Drawing.Point(261, 3);
            this.TXT_desc.Name = "TXT_desc";
            this.TXT_desc.Size = new System.Drawing.Size(236, 20);
            this.TXT_desc.TabIndex = 3;
            this.TXT_desc.Text = "GPS Hdop";
            this.TXT_desc.TextChanged += new System.EventHandler(this.TXT_desc_TextChanged);
            // 
            // CheckListInput
            // 
            this.Controls.Add(this.TXT_desc);
            this.Controls.Add(this.CMB_colour2);
            this.Controls.Add(this.CMB_colour1);
            this.Controls.Add(this.but_remove);
            this.Controls.Add(this.but_addchild);
            this.Controls.Add(this.TXT_text);
            this.Controls.Add(this.NUM_trigger);
            this.Controls.Add(this.CMB_condition);
            this.Controls.Add(this.CMB_Source);
            this.Name = "CheckListInput";
            this.Size = new System.Drawing.Size(695, 51);
            ((System.ComponentModel.ISupportInitialize)(this.NUM_trigger)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void CMB_Source_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.SetField(CMB_Source.Text);
        }

        private void CMB_condition_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.ConditionType =
                    (CheckListItem.Conditional)Enum.Parse(typeof(CheckListItem.Conditional), CMB_condition.Text);
        }

        private void NUM_warning_ValueChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.TriggerValue = (double) NUM_trigger.Value;
        }

        private void TXT_warningtext_TextChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.Text = TXT_text.Text;
        }

        private void but_addchild_Click(object sender, EventArgs e)
        {
            CheckListItem.Child = new CheckListItem();

            if (ReloadList != null)
                ReloadList(this, null);
        }

        private void but_remove_Click(object sender, EventArgs e)
        {
            lock (_parent.CheckListItems)
            {
                _parent.CheckListItems.Remove(CheckListItem);

                foreach (var item in _parent.CheckListItems)
                {
                    removewarning(item, CheckListItem);
                }
            }

            if (ReloadList != null)
                ReloadList(this, null);
        }

        void removewarning(CheckListItem lookin, CheckListItem removeme)
        {
            // depth first check children
            if (lookin.Child != null)
                removewarning(lookin.Child, removeme);

            if (lookin.Child == removeme)
            {
                if (lookin.Child.Child != null)
                {
                    lookin.Child = lookin.Child.Child;
                }
                else
                {
                    lookin.Child = null;
                }
                return;
            }
        }

        private void TXT_desc_TextChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.Description = TXT_desc.Text;
        }

        private void CMB_colour1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.TrueColor = CMB_colour1.SelectedValue.ToString();
        }

        private void CMB_colour2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CheckListItem != null)
                CheckListItem.FalseColor = CMB_colour2.SelectedValue.ToString();
        }
    }
}