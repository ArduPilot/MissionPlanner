namespace MissionPlanner.Swarm.FollowLeader
{
    partial class Control
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
            this.but_arm = new MissionPlanner.Controls.MyButton();
            this.but_takeoff = new MissionPlanner.Controls.MyButton();
            this.but_auto = new MissionPlanner.Controls.MyButton();
            this.but_master = new MissionPlanner.Controls.MyButton();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.but_start = new MissionPlanner.Controls.MyButton();
            this.but_guided = new MissionPlanner.Controls.MyButton();
            this.label3 = new System.Windows.Forms.Label();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            this.but_navguided = new MissionPlanner.Controls.MyButton();
            this.but_airmaster = new MissionPlanner.Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            this.SuspendLayout();
            // 
            // but_arm
            // 
            this.but_arm.Location = new System.Drawing.Point(204, 12);
            this.but_arm.Name = "but_arm";
            this.but_arm.Size = new System.Drawing.Size(75, 23);
            this.but_arm.TabIndex = 0;
            this.but_arm.Text = "Arm";
            this.but_arm.UseVisualStyleBackColor = true;
            this.but_arm.Click += new System.EventHandler(this.but_arm_Click);
            // 
            // but_takeoff
            // 
            this.but_takeoff.Location = new System.Drawing.Point(204, 41);
            this.but_takeoff.Name = "but_takeoff";
            this.but_takeoff.Size = new System.Drawing.Size(75, 23);
            this.but_takeoff.TabIndex = 1;
            this.but_takeoff.Text = "TakeOff";
            this.but_takeoff.UseVisualStyleBackColor = true;
            this.but_takeoff.Click += new System.EventHandler(this.but_takeoff_Click);
            // 
            // but_auto
            // 
            this.but_auto.Location = new System.Drawing.Point(204, 70);
            this.but_auto.Name = "but_auto";
            this.but_auto.Size = new System.Drawing.Size(75, 23);
            this.but_auto.TabIndex = 2;
            this.but_auto.Text = "Auto";
            this.but_auto.UseVisualStyleBackColor = true;
            this.but_auto.Click += new System.EventHandler(this.but_auto_Click);
            // 
            // but_master
            // 
            this.but_master.Location = new System.Drawing.Point(12, 12);
            this.but_master.Name = "but_master";
            this.but_master.Size = new System.Drawing.Size(75, 37);
            this.but_master.TabIndex = 3;
            this.but_master.Text = "Set Ground Master";
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
            this.numericUpDown2.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // but_start
            // 
            this.but_start.Location = new System.Drawing.Point(204, 100);
            this.but_start.Name = "but_start";
            this.but_start.Size = new System.Drawing.Size(75, 23);
            this.but_start.TabIndex = 8;
            this.but_start.Text = "Start";
            this.but_start.UseVisualStyleBackColor = true;
            this.but_start.Click += new System.EventHandler(this.but_start_Click);
            // 
            // but_guided
            // 
            this.but_guided.Location = new System.Drawing.Point(285, 70);
            this.but_guided.Name = "but_guided";
            this.but_guided.Size = new System.Drawing.Size(75, 23);
            this.but_guided.TabIndex = 9;
            this.but_guided.Text = "Guided";
            this.but_guided.UseVisualStyleBackColor = true;
            this.but_guided.Click += new System.EventHandler(this.but_guided_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Altitude";
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.DecimalPlaces = 1;
            this.numericUpDown3.Location = new System.Drawing.Point(74, 107);
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(75, 20);
            this.numericUpDown3.TabIndex = 10;
            this.numericUpDown3.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDown3.ValueChanged += new System.EventHandler(this.numericUpDown3_ValueChanged);
            // 
            // but_navguided
            // 
            this.but_navguided.Location = new System.Drawing.Point(366, 70);
            this.but_navguided.Name = "but_navguided";
            this.but_navguided.Size = new System.Drawing.Size(75, 23);
            this.but_navguided.TabIndex = 12;
            this.but_navguided.Text = "NAV Guided";
            this.but_navguided.UseVisualStyleBackColor = true;
            this.but_navguided.Click += new System.EventHandler(this.but_navguided_Click);
            // 
            // but_airmaster
            // 
            this.but_airmaster.Location = new System.Drawing.Point(93, 12);
            this.but_airmaster.Name = "but_airmaster";
            this.but_airmaster.Size = new System.Drawing.Size(75, 37);
            this.but_airmaster.TabIndex = 13;
            this.but_airmaster.Text = "Set Air Master";
            this.but_airmaster.UseVisualStyleBackColor = true;
            this.but_airmaster.Click += new System.EventHandler(this.but_airmaster_Click);
            // 
            // Control
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            
            this.ClientSize = new System.Drawing.Size(508, 292);
            this.Controls.Add(this.but_airmaster);
            this.Controls.Add(this.but_navguided);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numericUpDown3);
            this.Controls.Add(this.but_guided);
            this.Controls.Add(this.but_start);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.but_master);
            this.Controls.Add(this.but_auto);
            this.Controls.Add(this.but_takeoff);
            this.Controls.Add(this.but_arm);
            this.Name = "Control";
            this.Text = "Control";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MyButton but_arm;
        private Controls.MyButton but_takeoff;
        private Controls.MyButton but_auto;
        private Controls.MyButton but_master;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private Controls.MyButton but_start;
        private Controls.MyButton but_guided;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
        private Controls.MyButton but_navguided;
        private Controls.MyButton but_airmaster;
    }
}