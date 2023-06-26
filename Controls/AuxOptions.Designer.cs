namespace MissionPlanner.Controls
{
    partial class AuxOptions
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AuxOptions));
            this.BUT_Low = new MissionPlanner.Controls.MyButton();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.BUT_High = new MissionPlanner.Controls.MyButton();
            this.TXT_highvalue = new System.Windows.Forms.TextBox();
            this.TXT_rcchannel = new System.Windows.Forms.Label();
            this.but_mid = new MissionPlanner.Controls.MyButton();
            this.txt_midvalue = new System.Windows.Forms.TextBox();
            this.TXT_low_value = new System.Windows.Forms.TextBox();
            this.mavlinkComboBox1 = new MissionPlanner.Controls.MavlinkComboBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BUT_Low
            // 
            this.BUT_Low.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.BUT_Low, "BUT_Low");
            this.BUT_Low.Name = "BUT_Low";
            this.BUT_Low.UseVisualStyleBackColor = true;
            this.BUT_Low.Click += new System.EventHandler(this.BUT_Low_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            resources.ApplyResources(this.contextMenuStrip1, "contextMenuStrip1");
            // 
            // renameToolStripMenuItem
            // 
            // 
            // BUT_High
            // 
            this.BUT_High.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.BUT_High, "BUT_High");
            this.BUT_High.Name = "BUT_High";
            this.BUT_High.UseVisualStyleBackColor = true;
            this.BUT_High.Click += new System.EventHandler(this.BUT_High_Click);
            // 
            // TXT_highvalue
            // 
            resources.ApplyResources(this.TXT_highvalue, "TXT_highvalue");
            this.TXT_highvalue.Name = "TXT_highvalue";
            this.TXT_highvalue.TextChanged += new System.EventHandler(this.TXT_pwm_high_TextChanged);
            // 
            // TXT_rcchannel
            // 
            this.TXT_rcchannel.BackColor = System.Drawing.Color.Red;
            this.TXT_rcchannel.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.TXT_rcchannel, "TXT_rcchannel");
            this.TXT_rcchannel.Name = "TXT_rcchannel";
            // 
            // but_mid
            // 
            this.but_mid.ContextMenuStrip = this.contextMenuStrip1;
            resources.ApplyResources(this.but_mid, "but_mid");
            this.but_mid.Name = "but_mid";
            this.but_mid.UseVisualStyleBackColor = true;
            this.but_mid.Click += new System.EventHandler(this.But_mid_Click);
            // 
            // txt_midvalue
            // 
            resources.ApplyResources(this.txt_midvalue, "txt_midvalue");
            this.txt_midvalue.Name = "txt_midvalue";
            this.txt_midvalue.TextChanged += new System.EventHandler(this.txt_midvalue_TextChanged);
            // 
            // TXT_low_value
            // 
            resources.ApplyResources(this.TXT_low_value, "TXT_low_value");
            this.TXT_low_value.Name = "TXT_low_value";
            this.TXT_low_value.TextChanged += new System.EventHandler(this.TXT_pwm_low_TextChanged);
            // 
            // mavlinkComboBox1
            // 
            this.mavlinkComboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            resources.ApplyResources(this.mavlinkComboBox1, "mavlinkComboBox1");
            this.mavlinkComboBox1.FormattingEnabled = true;
            this.mavlinkComboBox1.Name = "mavlinkComboBox1";
            this.mavlinkComboBox1.ParamName = null;
            this.mavlinkComboBox1.SubControl = null;
            // 
            // AuxOptions
            // 
            this.Controls.Add(this.mavlinkComboBox1);
            this.Controls.Add(this.txt_midvalue);
            this.Controls.Add(this.but_mid);
            this.Controls.Add(this.TXT_rcchannel);
            this.Controls.Add(this.TXT_highvalue);
            this.Controls.Add(this.TXT_low_value);
            this.Controls.Add(this.BUT_High);
            this.Controls.Add(this.BUT_Low);
            this.Name = "AuxOptions";
            resources.ApplyResources(this, "$this");
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyButton BUT_Low;
        private MyButton BUT_High;
        private System.Windows.Forms.TextBox TXT_highvalue;
        private System.Windows.Forms.Label TXT_rcchannel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private MyButton but_mid;
        private System.Windows.Forms.TextBox txt_midvalue;
        private MavlinkComboBox mavlinkComboBox1;
        private System.Windows.Forms.TextBox TXT_low_value;
    }
}
