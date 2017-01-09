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
            this.label6 = new System.Windows.Forms.Label();
            this.TXT_IM90Deg = new System.Windows.Forms.TextBox();
            this.TXT_I0Deg = new System.Windows.Forms.TextBox();
            this.TXT_IM45Deg = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.TXT_IP90Deg = new System.Windows.Forms.TextBox();
            this.TXT_IP45Deg = new System.Windows.Forms.TextBox();
            this.Btn_SaveChanges = new MissionPlanner.Controls.MyButton();
            this.myLabel2 = new MissionPlanner.Controls.MyLabel();
            this.myLabel3 = new MissionPlanner.Controls.MyLabel();
            this.myLabel4 = new MissionPlanner.Controls.MyLabel();
            this.myLabel5 = new MissionPlanner.Controls.MyLabel();
            this.myLabel6 = new MissionPlanner.Controls.MyLabel();
            this.TXT_VP90Deg = new System.Windows.Forms.TextBox();
            this.TXT_V0Deg = new System.Windows.Forms.TextBox();
            this.TXT_VP45Deg = new System.Windows.Forms.TextBox();
            this.TXT_VM45Deg = new System.Windows.Forms.TextBox();
            this.myLabel1 = new MissionPlanner.Controls.MyLabel();
            this.myLabel8 = new MissionPlanner.Controls.MyLabel();
            this.TXT_VM90Deg = new System.Windows.Forms.TextBox();
            this.myLabel9 = new MissionPlanner.Controls.MyLabel();
            this.myLabel10 = new MissionPlanner.Controls.MyLabel();
            this.myLabel11 = new MissionPlanner.Controls.MyLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(10, 88);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 26);
            this.label6.TabIndex = 6;
            this.label6.Text = "Velocity (km/h) in \r\nrelation to angle";
            // 
            // TXT_IM90Deg
            // 
            this.TXT_IM90Deg.Location = new System.Drawing.Point(102, 53);
            this.TXT_IM90Deg.Name = "TXT_IM90Deg";
            this.TXT_IM90Deg.Size = new System.Drawing.Size(29, 20);
            this.TXT_IM90Deg.TabIndex = 8;
            this.TXT_IM90Deg.Text = "10";
            this.TXT_IM90Deg.TextChanged += new System.EventHandler(this.TXT_ParamA_TextChanged);
            // 
            // TXT_I0Deg
            // 
            this.TXT_I0Deg.Location = new System.Drawing.Point(172, 53);
            this.TXT_I0Deg.Name = "TXT_I0Deg";
            this.TXT_I0Deg.Size = new System.Drawing.Size(29, 20);
            this.TXT_I0Deg.TabIndex = 9;
            this.TXT_I0Deg.Text = "10";
            // 
            // TXT_IM45Deg
            // 
            this.TXT_IM45Deg.Location = new System.Drawing.Point(137, 53);
            this.TXT_IM45Deg.Name = "TXT_IM45Deg";
            this.TXT_IM45Deg.Size = new System.Drawing.Size(29, 20);
            this.TXT_IM45Deg.TabIndex = 17;
            this.TXT_IM45Deg.Text = "10";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.79365F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.20635F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(13, 62);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(90, 0);
            this.tableLayoutPanel1.TabIndex = 20;
            // 
            // TXT_IP90Deg
            // 
            this.TXT_IP90Deg.Location = new System.Drawing.Point(242, 53);
            this.TXT_IP90Deg.Name = "TXT_IP90Deg";
            this.TXT_IP90Deg.Size = new System.Drawing.Size(30, 20);
            this.TXT_IP90Deg.TabIndex = 21;
            this.TXT_IP90Deg.Text = "10";
            // 
            // TXT_IP45Deg
            // 
            this.TXT_IP45Deg.Location = new System.Drawing.Point(207, 53);
            this.TXT_IP45Deg.Name = "TXT_IP45Deg";
            this.TXT_IP45Deg.Size = new System.Drawing.Size(29, 20);
            this.TXT_IP45Deg.TabIndex = 19;
            this.TXT_IP45Deg.Text = "10";
            // 
            // Btn_SaveChanges
            // 
            this.Btn_SaveChanges.Location = new System.Drawing.Point(13, 128);
            this.Btn_SaveChanges.Name = "Btn_SaveChanges";
            this.Btn_SaveChanges.Size = new System.Drawing.Size(68, 32);
            this.Btn_SaveChanges.TabIndex = 14;
            this.Btn_SaveChanges.Text = "Save Changes";
            this.Btn_SaveChanges.UseVisualStyleBackColor = true;
            this.Btn_SaveChanges.Click += new System.EventHandler(this.Btn_SaveChanges_Click);
            // 
            // myLabel2
            // 
            this.myLabel2.Location = new System.Drawing.Point(213, 36);
            this.myLabel2.Name = "myLabel2";
            this.myLabel2.resize = false;
            this.myLabel2.Size = new System.Drawing.Size(18, 19);
            this.myLabel2.TabIndex = 21;
            this.myLabel2.Text = "45";
            // 
            // myLabel3
            // 
            this.myLabel3.Location = new System.Drawing.Point(181, 36);
            this.myLabel3.Name = "myLabel3";
            this.myLabel3.resize = false;
            this.myLabel3.Size = new System.Drawing.Size(14, 19);
            this.myLabel3.TabIndex = 22;
            this.myLabel3.Text = "0";
            // 
            // myLabel4
            // 
            this.myLabel4.Location = new System.Drawing.Point(142, 36);
            this.myLabel4.Name = "myLabel4";
            this.myLabel4.resize = false;
            this.myLabel4.Size = new System.Drawing.Size(20, 19);
            this.myLabel4.TabIndex = 23;
            this.myLabel4.Text = "-45";
            // 
            // myLabel5
            // 
            this.myLabel5.Location = new System.Drawing.Point(105, 36);
            this.myLabel5.Name = "myLabel5";
            this.myLabel5.resize = false;
            this.myLabel5.Size = new System.Drawing.Size(20, 19);
            this.myLabel5.TabIndex = 24;
            this.myLabel5.Text = "-90";
            // 
            // myLabel6
            // 
            this.myLabel6.Location = new System.Drawing.Point(248, 36);
            this.myLabel6.Name = "myLabel6";
            this.myLabel6.resize = false;
            this.myLabel6.Size = new System.Drawing.Size(18, 19);
            this.myLabel6.TabIndex = 22;
            this.myLabel6.Text = "90";
            // 
            // TXT_VP90Deg
            // 
            this.TXT_VP90Deg.Location = new System.Drawing.Point(242, 96);
            this.TXT_VP90Deg.Name = "TXT_VP90Deg";
            this.TXT_VP90Deg.Size = new System.Drawing.Size(30, 20);
            this.TXT_VP90Deg.TabIndex = 30;
            this.TXT_VP90Deg.Text = "10";
            // 
            // TXT_V0Deg
            // 
            this.TXT_V0Deg.Location = new System.Drawing.Point(172, 96);
            this.TXT_V0Deg.Name = "TXT_V0Deg";
            this.TXT_V0Deg.Size = new System.Drawing.Size(29, 20);
            this.TXT_V0Deg.TabIndex = 27;
            this.TXT_V0Deg.Text = "10";
            // 
            // TXT_VP45Deg
            // 
            this.TXT_VP45Deg.Location = new System.Drawing.Point(207, 96);
            this.TXT_VP45Deg.Name = "TXT_VP45Deg";
            this.TXT_VP45Deg.Size = new System.Drawing.Size(29, 20);
            this.TXT_VP45Deg.TabIndex = 29;
            this.TXT_VP45Deg.Text = "10";
            // 
            // TXT_VM45Deg
            // 
            this.TXT_VM45Deg.Location = new System.Drawing.Point(137, 96);
            this.TXT_VM45Deg.Name = "TXT_VM45Deg";
            this.TXT_VM45Deg.Size = new System.Drawing.Size(29, 20);
            this.TXT_VM45Deg.TabIndex = 28;
            this.TXT_VM45Deg.Text = "10";
            // 
            // myLabel1
            // 
            this.myLabel1.Location = new System.Drawing.Point(248, 78);
            this.myLabel1.Name = "myLabel1";
            this.myLabel1.resize = false;
            this.myLabel1.Size = new System.Drawing.Size(18, 19);
            this.myLabel1.TabIndex = 32;
            this.myLabel1.Text = "90";
            // 
            // myLabel8
            // 
            this.myLabel8.Location = new System.Drawing.Point(181, 78);
            this.myLabel8.Name = "myLabel8";
            this.myLabel8.resize = false;
            this.myLabel8.Size = new System.Drawing.Size(14, 19);
            this.myLabel8.TabIndex = 33;
            this.myLabel8.Text = "0";
            // 
            // TXT_VM90Deg
            // 
            this.TXT_VM90Deg.Location = new System.Drawing.Point(102, 96);
            this.TXT_VM90Deg.Name = "TXT_VM90Deg";
            this.TXT_VM90Deg.Size = new System.Drawing.Size(29, 20);
            this.TXT_VM90Deg.TabIndex = 26;
            this.TXT_VM90Deg.Text = "10";
            // 
            // myLabel9
            // 
            this.myLabel9.Location = new System.Drawing.Point(213, 78);
            this.myLabel9.Name = "myLabel9";
            this.myLabel9.resize = false;
            this.myLabel9.Size = new System.Drawing.Size(18, 19);
            this.myLabel9.TabIndex = 31;
            this.myLabel9.Text = "45";
            // 
            // myLabel10
            // 
            this.myLabel10.Location = new System.Drawing.Point(142, 78);
            this.myLabel10.Name = "myLabel10";
            this.myLabel10.resize = false;
            this.myLabel10.Size = new System.Drawing.Size(20, 19);
            this.myLabel10.TabIndex = 34;
            this.myLabel10.Text = "-45";
            // 
            // myLabel11
            // 
            this.myLabel11.Location = new System.Drawing.Point(105, 78);
            this.myLabel11.Name = "myLabel11";
            this.myLabel11.resize = false;
            this.myLabel11.Size = new System.Drawing.Size(20, 19);
            this.myLabel11.TabIndex = 35;
            this.myLabel11.Text = "-90";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 26);
            this.label1.TabIndex = 36;
            this.label1.Text = "Current (mA) in \r\nrelation to angle";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(224, 13);
            this.label2.TabIndex = 37;
            this.label2.Text = "Approximation of energyconsumption function:";
            // 
            // ConfigEnergyProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TXT_VP90Deg);
            this.Controls.Add(this.TXT_V0Deg);
            this.Controls.Add(this.TXT_VP45Deg);
            this.Controls.Add(this.TXT_VM45Deg);
            this.Controls.Add(this.myLabel1);
            this.Controls.Add(this.myLabel8);
            this.Controls.Add(this.TXT_VM90Deg);
            this.Controls.Add(this.myLabel9);
            this.Controls.Add(this.myLabel10);
            this.Controls.Add(this.myLabel11);
            this.Controls.Add(this.TXT_IP90Deg);
            this.Controls.Add(this.TXT_I0Deg);
            this.Controls.Add(this.TXT_IP45Deg);
            this.Controls.Add(this.TXT_IM45Deg);
            this.Controls.Add(this.myLabel6);
            this.Controls.Add(this.myLabel3);
            this.Controls.Add(this.TXT_IM90Deg);
            this.Controls.Add(this.myLabel2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.myLabel4);
            this.Controls.Add(this.Btn_SaveChanges);
            this.Controls.Add(this.myLabel5);
            this.Controls.Add(this.label6);
            this.Name = "ConfigEnergyProfile";
            this.Size = new System.Drawing.Size(304, 171);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TXT_IM90Deg;
        private System.Windows.Forms.TextBox TXT_I0Deg;
        private Controls.MyButton Btn_SaveChanges;
        private System.Windows.Forms.TextBox TXT_IM45Deg;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.TextBox TXT_IP90Deg;
        private System.Windows.Forms.TextBox TXT_IP45Deg;
        private Controls.MyLabel myLabel6;
        private Controls.MyLabel myLabel3;
        private Controls.MyLabel myLabel2;
        private Controls.MyLabel myLabel4;
        private Controls.MyLabel myLabel5;
        private System.Windows.Forms.TextBox TXT_VP90Deg;
        private System.Windows.Forms.TextBox TXT_V0Deg;
        private System.Windows.Forms.TextBox TXT_VP45Deg;
        private System.Windows.Forms.TextBox TXT_VM45Deg;
        private Controls.MyLabel myLabel1;
        private Controls.MyLabel myLabel8;
        private System.Windows.Forms.TextBox TXT_VM90Deg;
        private Controls.MyLabel myLabel9;
        private Controls.MyLabel myLabel10;
        private Controls.MyLabel myLabel11;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
