namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigEnergyProfile
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
            this.CB_EnableEnergyProfile = new System.Windows.Forms.CheckBox();
            this.Btn_SaveChanges = new MissionPlanner.Controls.MyButton();
            this.P_EnergyProfileConfiguration = new System.Windows.Forms.Panel();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.TB_Velocity = new System.Windows.Forms.TextBox();
            this.BtnAddVValue = new MissionPlanner.Controls.MyButton();
            this.TB_AngleV = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.TB_Current = new System.Windows.Forms.TextBox();
            this.BtnAddIValue = new MissionPlanner.Controls.MyButton();
            this.TB_AngleI = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.DGV_VValues = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DGV_IValues = new System.Windows.Forms.DataGridView();
            this.IAngle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ICurrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.P_EnergyProfileConfiguration.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_VValues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_IValues)).BeginInit();
            this.SuspendLayout();
            // 
            // CB_EnableEnergyProfile
            // 
            this.CB_EnableEnergyProfile.AutoSize = true;
            this.CB_EnableEnergyProfile.Location = new System.Drawing.Point(16, 11);
            this.CB_EnableEnergyProfile.Name = "CB_EnableEnergyProfile";
            this.CB_EnableEnergyProfile.Size = new System.Drawing.Size(124, 17);
            this.CB_EnableEnergyProfile.TabIndex = 38;
            this.CB_EnableEnergyProfile.Text = "Enable EnergyProfile";
            this.CB_EnableEnergyProfile.UseVisualStyleBackColor = true;
            this.CB_EnableEnergyProfile.CheckStateChanged += new System.EventHandler(this.CB_EnableEnergyProfile_CheckStateChanged);
            // 
            // Btn_SaveChanges
            // 
            this.Btn_SaveChanges.Location = new System.Drawing.Point(3, 310);
            this.Btn_SaveChanges.Name = "Btn_SaveChanges";
            this.Btn_SaveChanges.Size = new System.Drawing.Size(95, 32);
            this.Btn_SaveChanges.TabIndex = 14;
            this.Btn_SaveChanges.Text = "Save Changes";
            this.Btn_SaveChanges.UseVisualStyleBackColor = true;
            this.Btn_SaveChanges.Click += new System.EventHandler(this.Btn_SaveChanges_Click);
            // 
            // P_EnergyProfileConfiguration
            // 
            this.P_EnergyProfileConfiguration.Controls.Add(this.label10);
            this.P_EnergyProfileConfiguration.Controls.Add(this.label11);
            this.P_EnergyProfileConfiguration.Controls.Add(this.TB_Velocity);
            this.P_EnergyProfileConfiguration.Controls.Add(this.BtnAddVValue);
            this.P_EnergyProfileConfiguration.Controls.Add(this.TB_AngleV);
            this.P_EnergyProfileConfiguration.Controls.Add(this.label9);
            this.P_EnergyProfileConfiguration.Controls.Add(this.label8);
            this.P_EnergyProfileConfiguration.Controls.Add(this.TB_Current);
            this.P_EnergyProfileConfiguration.Controls.Add(this.BtnAddIValue);
            this.P_EnergyProfileConfiguration.Controls.Add(this.TB_AngleI);
            this.P_EnergyProfileConfiguration.Controls.Add(this.label6);
            this.P_EnergyProfileConfiguration.Controls.Add(this.label5);
            this.P_EnergyProfileConfiguration.Controls.Add(this.panel2);
            this.P_EnergyProfileConfiguration.Controls.Add(this.panel1);
            this.P_EnergyProfileConfiguration.Controls.Add(this.DGV_VValues);
            this.P_EnergyProfileConfiguration.Controls.Add(this.DGV_IValues);
            this.P_EnergyProfileConfiguration.Controls.Add(this.Btn_SaveChanges);
            this.P_EnergyProfileConfiguration.Enabled = false;
            this.P_EnergyProfileConfiguration.Location = new System.Drawing.Point(16, 34);
            this.P_EnergyProfileConfiguration.Name = "P_EnergyProfileConfiguration";
            this.P_EnergyProfileConfiguration.Size = new System.Drawing.Size(546, 357);
            this.P_EnergyProfileConfiguration.TabIndex = 40;
            this.P_EnergyProfileConfiguration.Paint += new System.Windows.Forms.PaintEventHandler(this.P_EnergyProfileConfiguration_Paint);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(444, 210);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 26);
            this.label10.TabIndex = 56;
            this.label10.Text = "Velocity\r\nin m/s:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(444, 190);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 55;
            this.label11.Text = "Angle:";
            // 
            // TB_Velocity
            // 
            this.TB_Velocity.Location = new System.Drawing.Point(494, 213);
            this.TB_Velocity.Name = "TB_Velocity";
            this.TB_Velocity.Size = new System.Drawing.Size(44, 20);
            this.TB_Velocity.TabIndex = 54;
            // 
            // BtnAddVValue
            // 
            this.BtnAddVValue.Location = new System.Drawing.Point(447, 239);
            this.BtnAddVValue.Name = "BtnAddVValue";
            this.BtnAddVValue.Size = new System.Drawing.Size(91, 32);
            this.BtnAddVValue.TabIndex = 53;
            this.BtnAddVValue.Text = "Add Value (V)";
            this.BtnAddVValue.UseVisualStyleBackColor = true;
            // 
            // TB_AngleV
            // 
            this.TB_AngleV.Location = new System.Drawing.Point(494, 187);
            this.TB_AngleV.Name = "TB_AngleV";
            this.TB_AngleV.Size = new System.Drawing.Size(44, 20);
            this.TB_AngleV.TabIndex = 52;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(444, 58);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 26);
            this.label9.TabIndex = 51;
            this.label9.Text = "Current \r\nin A:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(444, 38);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 50;
            this.label8.Text = "Angle:";
            // 
            // TB_Current
            // 
            this.TB_Current.Location = new System.Drawing.Point(494, 61);
            this.TB_Current.Name = "TB_Current";
            this.TB_Current.Size = new System.Drawing.Size(44, 20);
            this.TB_Current.TabIndex = 48;
            // 
            // BtnAddIValue
            // 
            this.BtnAddIValue.Location = new System.Drawing.Point(447, 87);
            this.BtnAddIValue.Name = "BtnAddIValue";
            this.BtnAddIValue.Size = new System.Drawing.Size(91, 32);
            this.BtnAddIValue.TabIndex = 46;
            this.BtnAddIValue.Text = "Add Value (I)";
            this.BtnAddIValue.UseVisualStyleBackColor = true;
            // 
            // TB_AngleI
            // 
            this.TB_AngleI.Location = new System.Drawing.Point(494, 35);
            this.TB_AngleI.Name = "TB_AngleI";
            this.TB_AngleI.Size = new System.Drawing.Size(44, 20);
            this.TB_AngleI.TabIndex = 45;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(444, 167);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 13);
            this.label6.TabIndex = 44;
            this.label6.Text = "Add parameter for V";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(444, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 43;
            this.label5.Text = "Add parameter for I";
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Info;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label4);
            this.panel2.Location = new System.Drawing.Point(209, 151);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(229, 139);
            this.panel2.TabIndex = 42;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(52, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Graph for V";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Info;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(209, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(229, 139);
            this.panel1.TabIndex = 41;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(56, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Graph for I";
            // 
            // DGV_VValues
            // 
            this.DGV_VValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_VValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.DGV_VValues.Location = new System.Drawing.Point(0, 151);
            this.DGV_VValues.Name = "DGV_VValues";
            this.DGV_VValues.ReadOnly = true;
            this.DGV_VValues.RowHeadersVisible = false;
            this.DGV_VValues.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DGV_VValues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV_VValues.Size = new System.Drawing.Size(203, 139);
            this.DGV_VValues.TabIndex = 40;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Angle";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Velocity in m/s";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // DGV_IValues
            // 
            this.DGV_IValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_IValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IAngle,
            this.ICurrent});
            this.DGV_IValues.Location = new System.Drawing.Point(0, 0);
            this.DGV_IValues.Name = "DGV_IValues";
            this.DGV_IValues.ReadOnly = true;
            this.DGV_IValues.RowHeadersVisible = false;
            this.DGV_IValues.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.DGV_IValues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.DGV_IValues.Size = new System.Drawing.Size(203, 139);
            this.DGV_IValues.TabIndex = 38;
            // 
            // IAngle
            // 
            this.IAngle.HeaderText = "Angle";
            this.IAngle.Name = "IAngle";
            this.IAngle.ReadOnly = true;
            // 
            // ICurrent
            // 
            this.ICurrent.HeaderText = "Current in A";
            this.ICurrent.Name = "ICurrent";
            this.ICurrent.ReadOnly = true;
            // 
            // ConfigEnergyProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.P_EnergyProfileConfiguration);
            this.Controls.Add(this.CB_EnableEnergyProfile);
            this.Name = "ConfigEnergyProfile";
            this.Size = new System.Drawing.Size(575, 406);
            this.P_EnergyProfileConfiguration.ResumeLayout(false);
            this.P_EnergyProfileConfiguration.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_VValues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_IValues)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Controls.MyButton Btn_SaveChanges;
        private System.Windows.Forms.CheckBox CB_EnableEnergyProfile;
        private System.Windows.Forms.Panel P_EnergyProfileConfiguration;
        private System.Windows.Forms.DataGridView DGV_IValues;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView DGV_VValues;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox TB_Current;
        private Controls.MyButton BtnAddIValue;
        private System.Windows.Forms.TextBox TB_AngleI;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox TB_Velocity;
        private Controls.MyButton BtnAddVValue;
        private System.Windows.Forms.TextBox TB_AngleV;
        private System.Windows.Forms.DataGridViewTextBoxColumn IAngle;
        private System.Windows.Forms.DataGridViewTextBoxColumn ICurrent;
    }
}
