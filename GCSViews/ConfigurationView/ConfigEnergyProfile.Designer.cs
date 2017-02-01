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
            this.label2 = new System.Windows.Forms.Label();
            this.CB_EnableEnergyProfile = new System.Windows.Forms.CheckBox();
            this.Btn_SaveChanges = new MissionPlanner.Controls.MyButton();
            this.P_EnergyProfileConfiguration = new System.Windows.Forms.Panel();
            this.DGV_IValues = new System.Windows.Forms.DataGridView();
            this.IAngle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ICurrent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.myButton1 = new MissionPlanner.Controls.MyButton();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.myButton3 = new MissionPlanner.Controls.MyButton();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.P_EnergyProfileConfiguration.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_IValues)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 13);
            this.label2.TabIndex = 37;
            this.label2.Text = "Values of approximated Polynom for I";
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
            this.Btn_SaveChanges.Location = new System.Drawing.Point(20, 355);
            this.Btn_SaveChanges.Name = "Btn_SaveChanges";
            this.Btn_SaveChanges.Size = new System.Drawing.Size(68, 32);
            this.Btn_SaveChanges.TabIndex = 14;
            this.Btn_SaveChanges.Text = "Save Changes";
            this.Btn_SaveChanges.UseVisualStyleBackColor = true;
            this.Btn_SaveChanges.Click += new System.EventHandler(this.Btn_SaveChanges_Click);
            // 
            // P_EnergyProfileConfiguration
            // 
            this.P_EnergyProfileConfiguration.Controls.Add(this.label10);
            this.P_EnergyProfileConfiguration.Controls.Add(this.label11);
            this.P_EnergyProfileConfiguration.Controls.Add(this.textBox3);
            this.P_EnergyProfileConfiguration.Controls.Add(this.myButton3);
            this.P_EnergyProfileConfiguration.Controls.Add(this.textBox4);
            this.P_EnergyProfileConfiguration.Controls.Add(this.label9);
            this.P_EnergyProfileConfiguration.Controls.Add(this.label8);
            this.P_EnergyProfileConfiguration.Controls.Add(this.textBox2);
            this.P_EnergyProfileConfiguration.Controls.Add(this.myButton1);
            this.P_EnergyProfileConfiguration.Controls.Add(this.textBox1);
            this.P_EnergyProfileConfiguration.Controls.Add(this.label6);
            this.P_EnergyProfileConfiguration.Controls.Add(this.label5);
            this.P_EnergyProfileConfiguration.Controls.Add(this.panel2);
            this.P_EnergyProfileConfiguration.Controls.Add(this.panel1);
            this.P_EnergyProfileConfiguration.Controls.Add(this.dataGridView1);
            this.P_EnergyProfileConfiguration.Controls.Add(this.label1);
            this.P_EnergyProfileConfiguration.Controls.Add(this.DGV_IValues);
            this.P_EnergyProfileConfiguration.Controls.Add(this.Btn_SaveChanges);
            this.P_EnergyProfileConfiguration.Controls.Add(this.label2);
            this.P_EnergyProfileConfiguration.Enabled = false;
            this.P_EnergyProfileConfiguration.Location = new System.Drawing.Point(16, 34);
            this.P_EnergyProfileConfiguration.Name = "P_EnergyProfileConfiguration";
            this.P_EnergyProfileConfiguration.Size = new System.Drawing.Size(626, 390);
            this.P_EnergyProfileConfiguration.TabIndex = 40;
            // 
            // DGV_IValues
            // 
            this.DGV_IValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DGV_IValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.IAngle,
            this.ICurrent});
            this.DGV_IValues.Location = new System.Drawing.Point(20, 28);
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
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            this.dataGridView1.Location = new System.Drawing.Point(20, 196);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(203, 139);
            this.dataGridView1.TabIndex = 40;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 180);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 13);
            this.label1.TabIndex = 39;
            this.label1.Text = "Values of approximated Polynom for V";
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
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Info;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.label3);
            this.panel1.Location = new System.Drawing.Point(245, 28);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(229, 139);
            this.panel1.TabIndex = 41;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Info;
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label4);
            this.panel2.Location = new System.Drawing.Point(245, 196);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(229, 139);
            this.panel2.TabIndex = 42;
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
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(52, 62);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Graph for V";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(495, 45);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(97, 13);
            this.label5.TabIndex = 43;
            this.label5.Text = "Add parameter for I";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(495, 211);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 13);
            this.label6.TabIndex = 44;
            this.label6.Text = "Add parameter for V";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(545, 64);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(44, 20);
            this.textBox1.TabIndex = 45;
            // 
            // myButton1
            // 
            this.myButton1.Location = new System.Drawing.Point(498, 116);
            this.myButton1.Name = "myButton1";
            this.myButton1.Size = new System.Drawing.Size(91, 32);
            this.myButton1.TabIndex = 46;
            this.myButton1.Text = "Add Value (I)";
            this.myButton1.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(545, 90);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(44, 20);
            this.textBox2.TabIndex = 48;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(495, 67);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 13);
            this.label8.TabIndex = 50;
            this.label8.Text = "Angle:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(495, 87);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 26);
            this.label9.TabIndex = 51;
            this.label9.Text = "Current \r\nin A:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(495, 254);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(44, 26);
            this.label10.TabIndex = 56;
            this.label10.Text = "Velocity\r\nin m/s:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(495, 234);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(37, 13);
            this.label11.TabIndex = 55;
            this.label11.Text = "Angle:";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(545, 257);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(44, 20);
            this.textBox3.TabIndex = 54;
            // 
            // myButton3
            // 
            this.myButton3.Location = new System.Drawing.Point(498, 283);
            this.myButton3.Name = "myButton3";
            this.myButton3.Size = new System.Drawing.Size(91, 32);
            this.myButton3.TabIndex = 53;
            this.myButton3.Text = "Add Value (V)";
            this.myButton3.UseVisualStyleBackColor = true;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(545, 231);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(44, 20);
            this.textBox4.TabIndex = 52;
            // 
            // ConfigEnergyProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.P_EnergyProfileConfiguration);
            this.Controls.Add(this.CB_EnableEnergyProfile);
            this.Name = "ConfigEnergyProfile";
            this.Size = new System.Drawing.Size(657, 458);
            this.P_EnergyProfileConfiguration.ResumeLayout(false);
            this.P_EnergyProfileConfiguration.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DGV_IValues)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Controls.MyButton Btn_SaveChanges;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox CB_EnableEnergyProfile;
        private System.Windows.Forms.Panel P_EnergyProfileConfiguration;
        private System.Windows.Forms.DataGridView DGV_IValues;
        private System.Windows.Forms.DataGridViewTextBoxColumn IAngle;
        private System.Windows.Forms.DataGridViewTextBoxColumn ICurrent;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox2;
        private Controls.MyButton myButton1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBox3;
        private Controls.MyButton myButton3;
        private System.Windows.Forms.TextBox textBox4;
    }
}
