namespace MissionPlanner.Swarm.WaypointLeader
{
    partial class WPControl
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WPControl));
            this.but_master = new MissionPlanner.Controls.MyButton();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.but_start = new MissionPlanner.Controls.MyButton();
            this.but_airmaster = new MissionPlanner.Controls.MyButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.PNL_status = new System.Windows.Forms.FlowLayoutPanel();
            this.txt_mode = new System.Windows.Forms.Label();
            this.but_resetmode = new MissionPlanner.Controls.MyButton();
            this.but_rth = new MissionPlanner.Controls.MyButton();
            this.label3 = new System.Windows.Forms.Label();
            this.num_useroffline = new System.Windows.Forms.NumericUpDown();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chk_V = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_useroffline)).BeginInit();
            this.SuspendLayout();
            // 
            // but_master
            // 
            this.but_master.Location = new System.Drawing.Point(12, 12);
            this.but_master.Name = "but_master";
            this.but_master.Size = new System.Drawing.Size(75, 37);
            this.but_master.TabIndex = 3;
            this.but_master.Text = "Set Ground Master";
            this.toolTip1.SetToolTip(this.but_master, "set the ground master drone");
            this.but_master.UseVisualStyleBackColor = true;
            this.but_master.Click += new System.EventHandler(this.but_master_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DecimalPlaces = 1;
            this.numericUpDown1.Location = new System.Drawing.Point(74, 55);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(75, 20);
            this.numericUpDown1.TabIndex = 4;
            this.toolTip1.SetToolTip(this.numericUpDown1, "the seperation between the drones in the air");
            this.numericUpDown1.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Seperation";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 83);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Lead";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.DecimalPlaces = 1;
            this.numericUpDown2.Location = new System.Drawing.Point(74, 81);
            this.numericUpDown2.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(75, 20);
            this.numericUpDown2.TabIndex = 6;
            this.toolTip1.SetToolTip(this.numericUpDown2, "the amount of lead the master air drone will be infront of the ground master");
            this.numericUpDown2.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // but_start
            // 
            this.but_start.Location = new System.Drawing.Point(218, 99);
            this.but_start.Name = "but_start";
            this.but_start.Size = new System.Drawing.Size(75, 23);
            this.but_start.TabIndex = 8;
            this.but_start.Text = "Start";
            this.toolTip1.SetToolTip(this.but_start, "start/stop sending commands to the drones");
            this.but_start.UseVisualStyleBackColor = true;
            this.but_start.Click += new System.EventHandler(this.but_start_Click);
            // 
            // but_airmaster
            // 
            this.but_airmaster.Location = new System.Drawing.Point(93, 12);
            this.but_airmaster.Name = "but_airmaster";
            this.but_airmaster.Size = new System.Drawing.Size(75, 37);
            this.but_airmaster.TabIndex = 13;
            this.but_airmaster.Text = "Set Air Master";
            this.toolTip1.SetToolTip(this.but_airmaster, "set the air master drone");
            this.but_airmaster.UseVisualStyleBackColor = true;
            this.but_airmaster.Click += new System.EventHandler(this.but_airmaster_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // PNL_status
            // 
            this.PNL_status.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PNL_status.Location = new System.Drawing.Point(13, 148);
            this.PNL_status.Name = "PNL_status";
            this.PNL_status.Size = new System.Drawing.Size(758, 136);
            this.PNL_status.TabIndex = 14;
            this.toolTip1.SetToolTip(this.PNL_status, "status of all the connected devices");
            // 
            // txt_mode
            // 
            this.txt_mode.AutoSize = true;
            this.txt_mode.Location = new System.Drawing.Point(203, 12);
            this.txt_mode.Name = "txt_mode";
            this.txt_mode.Size = new System.Drawing.Size(58, 13);
            this.txt_mode.TabIndex = 15;
            this.txt_mode.Text = "Seperation";
            // 
            // but_resetmode
            // 
            this.but_resetmode.Location = new System.Drawing.Point(282, 7);
            this.but_resetmode.Name = "but_resetmode";
            this.but_resetmode.Size = new System.Drawing.Size(75, 23);
            this.but_resetmode.TabIndex = 16;
            this.but_resetmode.Text = "Reset Mode";
            this.toolTip1.SetToolTip(this.but_resetmode, "Reset the internal state back so you can begin again");
            this.but_resetmode.UseVisualStyleBackColor = true;
            this.but_resetmode.Click += new System.EventHandler(this.but_resetmode_Click);
            // 
            // but_rth
            // 
            this.but_rth.Location = new System.Drawing.Point(282, 36);
            this.but_rth.Name = "but_rth";
            this.but_rth.Size = new System.Drawing.Size(75, 23);
            this.but_rth.TabIndex = 17;
            this.but_rth.Text = "set mode rth";
            this.toolTip1.SetToolTip(this.but_rth, "set the mode to return to home");
            this.but_rth.UseVisualStyleBackColor = true;
            this.but_rth.Click += new System.EventHandler(this.but_rth_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 19;
            this.label3.Text = "User Offline";
            // 
            // num_useroffline
            // 
            this.num_useroffline.DecimalPlaces = 1;
            this.num_useroffline.Location = new System.Drawing.Point(74, 107);
            this.num_useroffline.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            -2147483648});
            this.num_useroffline.Name = "num_useroffline";
            this.num_useroffline.Size = new System.Drawing.Size(75, 20);
            this.num_useroffline.TabIndex = 18;
            this.toolTip1.SetToolTip(this.num_useroffline, "the distance the groundmaster can go over to trigger the drones to switch to retu" +
        "rn to home");
            this.num_useroffline.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.num_useroffline.ValueChanged += new System.EventHandler(this.num_useroffline_ValueChanged);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(363, 4);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(408, 138);
            this.textBox1.TabIndex = 20;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // chk_V
            // 
            this.chk_V.AutoSize = true;
            this.chk_V.Location = new System.Drawing.Point(218, 76);
            this.chk_V.Name = "chk_V";
            this.chk_V.Size = new System.Drawing.Size(33, 17);
            this.chk_V.TabIndex = 21;
            this.chk_V.Text = "V";
            this.chk_V.UseVisualStyleBackColor = true;
            this.chk_V.CheckedChanged += new System.EventHandler(this.chk_V_CheckedChanged);
            // 
            // WPControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(783, 286);
            this.Controls.Add(this.chk_V);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.num_useroffline);
            this.Controls.Add(this.but_rth);
            this.Controls.Add(this.but_resetmode);
            this.Controls.Add(this.txt_mode);
            this.Controls.Add(this.PNL_status);
            this.Controls.Add(this.but_airmaster);
            this.Controls.Add(this.but_start);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.but_master);
            this.Name = "WPControl";
            this.Text = "WPControl";
            this.TopMost = true;
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_useroffline)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Controls.MyButton but_master;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private Controls.MyButton but_start;
        private Controls.MyButton but_airmaster;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.FlowLayoutPanel PNL_status;
        private System.Windows.Forms.Label txt_mode;
        private Controls.MyButton but_resetmode;
        private Controls.MyButton but_rth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown num_useroffline;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox chk_V;
    }
}