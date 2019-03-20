namespace MissionPlanner.Swarm
{
    partial class FormationControl
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
            this.CMB_mavs = new System.Windows.Forms.ComboBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.grid1 = new MissionPlanner.Swarm.Grid();
            this.BUT_Start = new MissionPlanner.Controls.MyButton();
            this.BUT_leader = new MissionPlanner.Controls.MyButton();
            this.BUT_Land = new MissionPlanner.Controls.MyButton();
            this.BUT_Takeoff = new MissionPlanner.Controls.MyButton();
            this.BUT_Disarm = new MissionPlanner.Controls.MyButton();
            this.BUT_Arm = new MissionPlanner.Controls.MyButton();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.BUT_Updatepos = new MissionPlanner.Controls.MyButton();
            this.PNL_status = new System.Windows.Forms.FlowLayoutPanel();
            this.timer_status = new System.Windows.Forms.Timer(this.components);
            this.but_guided = new MissionPlanner.Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CMB_mavs
            // 
            this.CMB_mavs.DataSource = this.bindingSource1;
            this.CMB_mavs.FormattingEnabled = true;
            this.CMB_mavs.Location = new System.Drawing.Point(336, 12);
            this.CMB_mavs.Name = "CMB_mavs";
            this.CMB_mavs.Size = new System.Drawing.Size(121, 21);
            this.CMB_mavs.TabIndex = 4;
            this.CMB_mavs.SelectedIndexChanged += new System.EventHandler(this.CMB_mavs_SelectedIndexChanged);
            // 
            // grid1
            // 
            this.grid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid1.Location = new System.Drawing.Point(3, 3);
            this.grid1.Name = "grid1";
            this.grid1.Size = new System.Drawing.Size(755, 388);
            this.grid1.TabIndex = 8;
            this.grid1.Vertical = false;
            this.grid1.UpdateOffsets += new MissionPlanner.Swarm.Grid.UpdateOffsetsEvent(this.grid1_UpdateOffsets);
            // 
            // BUT_Start
            // 
            this.BUT_Start.Enabled = false;
            this.BUT_Start.Location = new System.Drawing.Point(706, 12);
            this.BUT_Start.Name = "BUT_Start";
            this.BUT_Start.Size = new System.Drawing.Size(75, 23);
            this.BUT_Start.TabIndex = 6;
            this.BUT_Start.Text = global::MissionPlanner.Strings.Start;
            this.BUT_Start.UseVisualStyleBackColor = true;
            this.BUT_Start.Click += new System.EventHandler(this.BUT_Start_Click);
            // 
            // BUT_leader
            // 
            this.BUT_leader.Location = new System.Drawing.Point(463, 12);
            this.BUT_leader.Name = "BUT_leader";
            this.BUT_leader.Size = new System.Drawing.Size(75, 23);
            this.BUT_leader.TabIndex = 5;
            this.BUT_leader.Text = "Set Leader";
            this.BUT_leader.UseVisualStyleBackColor = true;
            this.BUT_leader.Click += new System.EventHandler(this.BUT_leader_Click);
            // 
            // BUT_Land
            // 
            this.BUT_Land.Location = new System.Drawing.Point(255, 12);
            this.BUT_Land.Name = "BUT_Land";
            this.BUT_Land.Size = new System.Drawing.Size(75, 23);
            this.BUT_Land.TabIndex = 3;
            this.BUT_Land.Text = "Land (all)";
            this.BUT_Land.UseVisualStyleBackColor = true;
            this.BUT_Land.Click += new System.EventHandler(this.BUT_Land_Click);
            // 
            // BUT_Takeoff
            // 
            this.BUT_Takeoff.Location = new System.Drawing.Point(174, 12);
            this.BUT_Takeoff.Name = "BUT_Takeoff";
            this.BUT_Takeoff.Size = new System.Drawing.Size(75, 23);
            this.BUT_Takeoff.TabIndex = 2;
            this.BUT_Takeoff.Text = "Takeoff";
            this.BUT_Takeoff.UseVisualStyleBackColor = true;
            this.BUT_Takeoff.Click += new System.EventHandler(this.BUT_Takeoff_Click);
            // 
            // BUT_Disarm
            // 
            this.BUT_Disarm.Location = new System.Drawing.Point(93, 12);
            this.BUT_Disarm.Name = "BUT_Disarm";
            this.BUT_Disarm.Size = new System.Drawing.Size(75, 23);
            this.BUT_Disarm.TabIndex = 1;
            this.BUT_Disarm.Text = "Disarm (exl leader)";
            this.BUT_Disarm.UseVisualStyleBackColor = true;
            this.BUT_Disarm.Click += new System.EventHandler(this.BUT_Disarm_Click);
            // 
            // BUT_Arm
            // 
            this.BUT_Arm.Location = new System.Drawing.Point(12, 12);
            this.BUT_Arm.Name = "BUT_Arm";
            this.BUT_Arm.Size = new System.Drawing.Size(75, 23);
            this.BUT_Arm.TabIndex = 0;
            this.BUT_Arm.Text = "Arm (exl leader)";
            this.BUT_Arm.UseVisualStyleBackColor = true;
            this.BUT_Arm.Click += new System.EventHandler(this.BUT_Arm_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 39);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(769, 420);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.grid1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(761, 394);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Stage 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // BUT_Updatepos
            // 
            this.BUT_Updatepos.Enabled = false;
            this.BUT_Updatepos.Location = new System.Drawing.Point(625, 12);
            this.BUT_Updatepos.Name = "BUT_Updatepos";
            this.BUT_Updatepos.Size = new System.Drawing.Size(75, 23);
            this.BUT_Updatepos.TabIndex = 10;
            this.BUT_Updatepos.Text = "Update Pos";
            this.BUT_Updatepos.UseVisualStyleBackColor = true;
            this.BUT_Updatepos.Click += new System.EventHandler(this.BUT_Updatepos_Click);
            // 
            // PNL_status
            // 
            this.PNL_status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PNL_status.AutoScroll = true;
            this.PNL_status.Location = new System.Drawing.Point(783, 61);
            this.PNL_status.Name = "PNL_status";
            this.PNL_status.Size = new System.Drawing.Size(147, 398);
            this.PNL_status.TabIndex = 11;
            // 
            // timer_status
            // 
            this.timer_status.Enabled = true;
            this.timer_status.Interval = 200;
            this.timer_status.Tick += new System.EventHandler(this.timer_status_Tick);
            // 
            // but_guided
            // 
            this.but_guided.Location = new System.Drawing.Point(544, 12);
            this.but_guided.Name = "but_guided";
            this.but_guided.Size = new System.Drawing.Size(75, 23);
            this.but_guided.TabIndex = 12;
            this.but_guided.Text = "Guided Mode";
            this.but_guided.UseVisualStyleBackColor = true;
            this.but_guided.Click += new System.EventHandler(this.but_guided_Click);
            // 
            // FormationControl
            // 
            
            this.ClientSize = new System.Drawing.Size(931, 471);
            this.Controls.Add(this.but_guided);
            this.Controls.Add(this.PNL_status);
            this.Controls.Add(this.BUT_Updatepos);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.BUT_Start);
            this.Controls.Add(this.BUT_leader);
            this.Controls.Add(this.CMB_mavs);
            this.Controls.Add(this.BUT_Land);
            this.Controls.Add(this.BUT_Takeoff);
            this.Controls.Add(this.BUT_Disarm);
            this.Controls.Add(this.BUT_Arm);
            this.Name = "FormationControl";
            this.Text = "Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Control_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.MyButton BUT_Arm;
        private Controls.MyButton BUT_Disarm;
        private Controls.MyButton BUT_Takeoff;
        private Controls.MyButton BUT_Land;
        private System.Windows.Forms.ComboBox CMB_mavs;
        private Controls.MyButton BUT_leader;
        private Controls.MyButton BUT_Start;
        private Grid grid1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private Controls.MyButton BUT_Updatepos;
        private System.Windows.Forms.FlowLayoutPanel PNL_status;
        private System.Windows.Forms.Timer timer_status;
        private Controls.MyButton but_guided;
    }
}