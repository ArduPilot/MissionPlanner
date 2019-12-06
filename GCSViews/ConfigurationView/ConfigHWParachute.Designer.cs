namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigHWParachute
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigHWParachute));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.mavlinkComboBoxServoNum = new System.Windows.Forms.ComboBox();
            this.mavlinkNumericUpDownMinAlt = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.mavlinkNumericUpDownDeploy = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.mavlinkNumericUpDownResting = new MissionPlanner.Controls.MavlinkNumericUpDown();
            this.mavlinkComboBoxType = new MissionPlanner.Controls.MavlinkComboBox();
            this.mavlinkCheckBoxEnable = new MissionPlanner.Controls.MavlinkCheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownMinAlt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownDeploy)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownResting)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // pictureBox3
            // 
            resources.ApplyResources(this.pictureBox3, "pictureBox3");
            this.pictureBox3.BackColor = System.Drawing.Color.White;
            this.pictureBox3.BackgroundImage = global::MissionPlanner.Properties.Resources.sonar;
            this.pictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pictureBox3.Image = global::MissionPlanner.Properties.Resources.Parachute;
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.TabStop = false;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // mavlinkComboBoxServoNum
            // 
            resources.ApplyResources(this.mavlinkComboBoxServoNum, "mavlinkComboBoxServoNum");
            this.mavlinkComboBoxServoNum.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mavlinkComboBoxServoNum.FormattingEnabled = true;
            this.mavlinkComboBoxServoNum.Items.AddRange(new object[] {
            resources.GetString("mavlinkComboBoxServoNum.Items"),
            resources.GetString("mavlinkComboBoxServoNum.Items1"),
            resources.GetString("mavlinkComboBoxServoNum.Items2"),
            resources.GetString("mavlinkComboBoxServoNum.Items3"),
            resources.GetString("mavlinkComboBoxServoNum.Items4"),
            resources.GetString("mavlinkComboBoxServoNum.Items5")});
            this.mavlinkComboBoxServoNum.Name = "mavlinkComboBoxServoNum";
            this.mavlinkComboBoxServoNum.SelectedIndexChanged += new System.EventHandler(this.mavlinkComboBoxServoNum_SelectedIndexChanged);
            // 
            // mavlinkNumericUpDownMinAlt
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownMinAlt, "mavlinkNumericUpDownMinAlt");
            this.mavlinkNumericUpDownMinAlt.Max = 1F;
            this.mavlinkNumericUpDownMinAlt.Min = 0F;
            this.mavlinkNumericUpDownMinAlt.Name = "mavlinkNumericUpDownMinAlt";
            this.mavlinkNumericUpDownMinAlt.ParamName = null;
            // 
            // mavlinkNumericUpDownDeploy
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownDeploy, "mavlinkNumericUpDownDeploy");
            this.mavlinkNumericUpDownDeploy.Max = 1F;
            this.mavlinkNumericUpDownDeploy.Min = 0F;
            this.mavlinkNumericUpDownDeploy.Name = "mavlinkNumericUpDownDeploy";
            this.mavlinkNumericUpDownDeploy.ParamName = null;
            // 
            // mavlinkNumericUpDownResting
            // 
            resources.ApplyResources(this.mavlinkNumericUpDownResting, "mavlinkNumericUpDownResting");
            this.mavlinkNumericUpDownResting.Max = 1F;
            this.mavlinkNumericUpDownResting.Min = 0F;
            this.mavlinkNumericUpDownResting.Name = "mavlinkNumericUpDownResting";
            this.mavlinkNumericUpDownResting.ParamName = null;
            // 
            // mavlinkComboBoxType
            // 
            resources.ApplyResources(this.mavlinkComboBoxType, "mavlinkComboBoxType");
            this.mavlinkComboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.mavlinkComboBoxType.FormattingEnabled = true;
            this.mavlinkComboBoxType.Name = "mavlinkComboBoxType";
            this.mavlinkComboBoxType.ParamName = null;
            this.mavlinkComboBoxType.SubControl = null;
            // 
            // mavlinkCheckBoxEnable
            // 
            resources.ApplyResources(this.mavlinkCheckBoxEnable, "mavlinkCheckBoxEnable");
            this.mavlinkCheckBoxEnable.ForeColor = System.Drawing.SystemColors.ControlText;
            this.mavlinkCheckBoxEnable.Name = "mavlinkCheckBoxEnable";
            this.mavlinkCheckBoxEnable.OffValue = 0D;
            this.mavlinkCheckBoxEnable.OnValue = 1D;
            this.mavlinkCheckBoxEnable.ParamName = null;
            this.mavlinkCheckBoxEnable.UseVisualStyleBackColor = true;
            // 
            // ConfigHWParachute
            // 
            resources.ApplyResources(this, "$this");
            
            this.Controls.Add(this.mavlinkComboBoxServoNum);
            this.Controls.Add(this.mavlinkNumericUpDownMinAlt);
            this.Controls.Add(this.mavlinkNumericUpDownDeploy);
            this.Controls.Add(this.mavlinkNumericUpDownResting);
            this.Controls.Add(this.mavlinkComboBoxType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.mavlinkCheckBoxEnable);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Name = "ConfigHWParachute";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownMinAlt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownDeploy)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.mavlinkNumericUpDownResting)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox3;
        private Controls.MavlinkCheckBox mavlinkCheckBoxEnable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private Controls.MavlinkComboBox mavlinkComboBoxType;
        private Controls.MavlinkNumericUpDown mavlinkNumericUpDownResting;
        private Controls.MavlinkNumericUpDown mavlinkNumericUpDownDeploy;
        private Controls.MavlinkNumericUpDown mavlinkNumericUpDownMinAlt;
        private System.Windows.Forms.ComboBox mavlinkComboBoxServoNum;
    }
}
