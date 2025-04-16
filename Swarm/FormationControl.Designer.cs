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
            this.but_auto = new MissionPlanner.Controls.MyButton();
            this.savePoint = new MissionPlanner.Controls.MyButton();
            this.loadPoint = new MissionPlanner.Controls.MyButton();
            this.But_Brake = new MissionPlanner.Controls.MyButton();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.bindingSource2 = new System.Windows.Forms.BindingSource(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.bindingSource3 = new System.Windows.Forms.BindingSource(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.bindingSource4 = new System.Windows.Forms.BindingSource(this.components);
            this.label4 = new System.Windows.Forms.Label();
            this.bindingSource5 = new System.Windows.Forms.BindingSource(this.components);
            this.myButton1 = new MissionPlanner.Controls.MyButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.myButton2 = new MissionPlanner.Controls.MyButton();
            this.myButton3 = new MissionPlanner.Controls.MyButton();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.myButton4 = new MissionPlanner.Controls.MyButton();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.myButton5 = new MissionPlanner.Controls.MyButton();
            this.myButton6 = new MissionPlanner.Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource5)).BeginInit();
            this.SuspendLayout();
            // 
            // CMB_mavs
            // 
            this.CMB_mavs.DataSource = this.bindingSource1;
            this.CMB_mavs.FormattingEnabled = true;
            this.CMB_mavs.Location = new System.Drawing.Point(310, 647);
            this.CMB_mavs.Name = "CMB_mavs";
            this.CMB_mavs.Size = new System.Drawing.Size(159, 26);
            this.CMB_mavs.TabIndex = 4;
            this.CMB_mavs.SelectedIndexChanged += new System.EventHandler(this.CMB_mavs_SelectedIndexChanged);
            // 
            // grid1
            // 
            this.grid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid1.Location = new System.Drawing.Point(3, 3);
            this.grid1.Name = "grid1";
            this.grid1.Size = new System.Drawing.Size(996, 560);
            this.grid1.TabIndex = 8;
            this.grid1.Vertical = false;
            this.grid1.UpdateOffsets += new MissionPlanner.Swarm.Grid.UpdateOffsetsEvent(this.grid1_UpdateOffsets);
            // 
            // BUT_Start
            // 
            this.BUT_Start.Enabled = false;
            this.BUT_Start.Location = new System.Drawing.Point(229, 713);
            this.BUT_Start.Name = "BUT_Start";
            this.BUT_Start.Size = new System.Drawing.Size(75, 23);
            this.BUT_Start.TabIndex = 6;
            this.BUT_Start.Text = "激活编队";
            this.BUT_Start.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_Start.UseVisualStyleBackColor = true;
            this.BUT_Start.Click += new System.EventHandler(this.BUT_Start_Click);
            // 
            // BUT_leader
            // 
            this.BUT_leader.Location = new System.Drawing.Point(229, 648);
            this.BUT_leader.Name = "BUT_leader";
            this.BUT_leader.Size = new System.Drawing.Size(75, 23);
            this.BUT_leader.TabIndex = 5;
            this.BUT_leader.Text = "设置主机";
            this.BUT_leader.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_leader.UseVisualStyleBackColor = true;
            this.BUT_leader.Click += new System.EventHandler(this.BUT_leader_Click);
            // 
            // BUT_Land
            // 
            this.BUT_Land.Location = new System.Drawing.Point(89, 681);
            this.BUT_Land.Name = "BUT_Land";
            this.BUT_Land.Size = new System.Drawing.Size(75, 23);
            this.BUT_Land.TabIndex = 3;
            this.BUT_Land.Text = "降落";
            this.BUT_Land.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_Land.UseVisualStyleBackColor = true;
            this.BUT_Land.Click += new System.EventHandler(this.BUT_Land_Click);
            // 
            // BUT_Takeoff
            // 
            this.BUT_Takeoff.Location = new System.Drawing.Point(9, 681);
            this.BUT_Takeoff.Name = "BUT_Takeoff";
            this.BUT_Takeoff.Size = new System.Drawing.Size(75, 23);
            this.BUT_Takeoff.TabIndex = 2;
            this.BUT_Takeoff.Text = "起飞";
            this.BUT_Takeoff.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_Takeoff.UseVisualStyleBackColor = true;
            this.BUT_Takeoff.Click += new System.EventHandler(this.BUT_Takeoff_Click);
            // 
            // BUT_Disarm
            // 
            this.BUT_Disarm.Location = new System.Drawing.Point(89, 648);
            this.BUT_Disarm.Name = "BUT_Disarm";
            this.BUT_Disarm.Size = new System.Drawing.Size(75, 23);
            this.BUT_Disarm.TabIndex = 1;
            this.BUT_Disarm.Text = "上锁";
            this.BUT_Disarm.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_Disarm.UseVisualStyleBackColor = true;
            this.BUT_Disarm.Click += new System.EventHandler(this.BUT_Disarm_Click);
            // 
            // BUT_Arm
            // 
            this.BUT_Arm.Location = new System.Drawing.Point(8, 649);
            this.BUT_Arm.Name = "BUT_Arm";
            this.BUT_Arm.Size = new System.Drawing.Size(75, 23);
            this.BUT_Arm.TabIndex = 0;
            this.BUT_Arm.Text = "解锁";
            this.BUT_Arm.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_Arm.UseVisualStyleBackColor = true;
            this.BUT_Arm.Click += new System.EventHandler(this.BUT_Arm_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(9, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1010, 598);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.grid1);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1002, 566);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Stage 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // BUT_Updatepos
            // 
            this.BUT_Updatepos.Enabled = false;
            this.BUT_Updatepos.Location = new System.Drawing.Point(391, 678);
            this.BUT_Updatepos.Name = "BUT_Updatepos";
            this.BUT_Updatepos.Size = new System.Drawing.Size(75, 23);
            this.BUT_Updatepos.TabIndex = 10;
            this.BUT_Updatepos.Text = "更新位置";
            this.BUT_Updatepos.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_Updatepos.UseVisualStyleBackColor = true;
            this.BUT_Updatepos.Click += new System.EventHandler(this.BUT_Updatepos_Click);
            // 
            // PNL_status
            // 
            this.PNL_status.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PNL_status.AutoScroll = true;
            this.PNL_status.Location = new System.Drawing.Point(1025, 40);
            this.PNL_status.Name = "PNL_status";
            this.PNL_status.Size = new System.Drawing.Size(370, 566);
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
            this.but_guided.Location = new System.Drawing.Point(229, 679);
            this.but_guided.Name = "but_guided";
            this.but_guided.Size = new System.Drawing.Size(75, 23);
            this.but_guided.TabIndex = 12;
            this.but_guided.Text = "引导模式";
            this.but_guided.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_guided.UseVisualStyleBackColor = true;
            this.but_guided.Click += new System.EventHandler(this.but_guided_Click);
            // 
            // but_auto
            // 
            this.but_auto.Location = new System.Drawing.Point(310, 678);
            this.but_auto.Name = "but_auto";
            this.but_auto.Size = new System.Drawing.Size(75, 23);
            this.but_auto.TabIndex = 13;
            this.but_auto.Text = "自动模式";
            this.but_auto.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_auto.UseVisualStyleBackColor = true;
            this.but_auto.Click += new System.EventHandler(this.but_auto_Click);
            // 
            // savePoint
            // 
            this.savePoint.Enabled = false;
            this.savePoint.Location = new System.Drawing.Point(391, 713);
            this.savePoint.Name = "savePoint";
            this.savePoint.Size = new System.Drawing.Size(75, 23);
            this.savePoint.TabIndex = 6;
            this.savePoint.Text = "保存编队";
            this.savePoint.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.savePoint.UseVisualStyleBackColor = true;
            this.savePoint.Click += new System.EventHandler(this.BUT_SavePoint_Click);
            // 
            // loadPoint
            // 
            this.loadPoint.Enabled = false;
            this.loadPoint.Location = new System.Drawing.Point(310, 713);
            this.loadPoint.Name = "loadPoint";
            this.loadPoint.Size = new System.Drawing.Size(75, 23);
            this.loadPoint.TabIndex = 6;
            this.loadPoint.Text = "加载编队";
            this.loadPoint.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.loadPoint.UseVisualStyleBackColor = true;
            this.loadPoint.Click += new System.EventHandler(this.BUT_LoadPoint_Click);
            // 
            // But_Brake
            // 
            this.But_Brake.Location = new System.Drawing.Point(89, 713);
            this.But_Brake.Name = "But_Brake";
            this.But_Brake.Size = new System.Drawing.Size(75, 23);
            this.But_Brake.TabIndex = 3;
            this.But_Brake.Text = "刹车";
            this.But_Brake.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.But_Brake.UseVisualStyleBackColor = true;
            this.But_Brake.Click += new System.EventHandler(this.BUT_Brake_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(8, 619);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(106, 22);
            this.checkBox1.TabIndex = 14;
            this.checkBox1.Text = "包含主机";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(512, 649);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 18);
            this.label1.TabIndex = 15;
            this.label1.Text = "无人机编号";
            // 
            // comboBox1
            // 
            this.comboBox1.DataSource = this.bindingSource2;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(616, 646);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 26);
            this.comboBox1.TabIndex = 16;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(566, 681);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 18);
            this.label2.TabIndex = 15;
            this.label2.Text = "x轴：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(569, 717);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 18);
            this.label3.TabIndex = 15;
            this.label3.Text = "y轴：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(569, 751);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 18);
            this.label4.TabIndex = 15;
            this.label4.Text = "z轴：";
            // 
            // myButton1
            // 
            this.myButton1.Location = new System.Drawing.Point(616, 782);
            this.myButton1.Name = "myButton1";
            this.myButton1.Size = new System.Drawing.Size(121, 23);
            this.myButton1.TabIndex = 10;
            this.myButton1.Text = "更新选中坐标";
            this.myButton1.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton1.UseVisualStyleBackColor = true;
            this.myButton1.Click += new System.EventHandler(this.BUT_UpdateOption_pos_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(616, 678);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(121, 28);
            this.textBox1.TabIndex = 17;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(616, 714);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(121, 28);
            this.textBox2.TabIndex = 17;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(616, 748);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(121, 28);
            this.textBox3.TabIndex = 17;
            // 
            // myButton2
            // 
            this.myButton2.Location = new System.Drawing.Point(9, 715);
            this.myButton2.Name = "myButton2";
            this.myButton2.Size = new System.Drawing.Size(75, 23);
            this.myButton2.TabIndex = 3;
            this.myButton2.Text = "返航";
            this.myButton2.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton2.UseVisualStyleBackColor = true;
            this.myButton2.Click += new System.EventHandler(this.BUT_Rtl_Click);
            // 
            // myButton3
            // 
            this.myButton3.Location = new System.Drawing.Point(9, 746);
            this.myButton3.Name = "myButton3";
            this.myButton3.Size = new System.Drawing.Size(75, 23);
            this.myButton3.TabIndex = 3;
            this.myButton3.Text = "依次返航";
            this.myButton3.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton3.UseVisualStyleBackColor = true;
            this.myButton3.Click += new System.EventHandler(this.BUT_Rtl_successively_Click);
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(90, 745);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(74, 28);
            this.textBox4.TabIndex = 17;
            // 
            // myButton4
            // 
            this.myButton4.Location = new System.Drawing.Point(788, 681);
            this.myButton4.Name = "myButton4";
            this.myButton4.Size = new System.Drawing.Size(75, 23);
            this.myButton4.TabIndex = 3;
            this.myButton4.Text = "y轴差分";
            this.myButton4.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton4.UseVisualStyleBackColor = true;
            this.myButton4.Click += new System.EventHandler(this.BUT_DifferenceY_Click);
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(869, 645);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(121, 28);
            this.textBox5.TabIndex = 17;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(869, 681);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(121, 28);
            this.textBox6.TabIndex = 17;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(869, 715);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(121, 28);
            this.textBox7.TabIndex = 17;
            // 
            // myButton5
            // 
            this.myButton5.Location = new System.Drawing.Point(788, 646);
            this.myButton5.Name = "myButton5";
            this.myButton5.Size = new System.Drawing.Size(75, 23);
            this.myButton5.TabIndex = 3;
            this.myButton5.Text = "x轴差分";
            this.myButton5.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton5.UseVisualStyleBackColor = true;
            this.myButton5.Click += new System.EventHandler(this.BUT_DifferenceX_Click);
            // 
            // myButton6
            // 
            this.myButton6.Location = new System.Drawing.Point(788, 717);
            this.myButton6.Name = "myButton6";
            this.myButton6.Size = new System.Drawing.Size(75, 23);
            this.myButton6.TabIndex = 3;
            this.myButton6.Text = "z轴差分";
            this.myButton6.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.myButton6.UseVisualStyleBackColor = true;
            this.myButton6.Click += new System.EventHandler(this.BUT_DifferenceZ_Click);
            // 
            // FormationControl
            // 
            this.ClientSize = new System.Drawing.Size(1407, 877);
            this.Controls.Add(this.textBox7);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.textBox6);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.but_auto);
            this.Controls.Add(this.but_guided);
            this.Controls.Add(this.PNL_status);
            this.Controls.Add(this.myButton1);
            this.Controls.Add(this.BUT_Updatepos);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.loadPoint);
            this.Controls.Add(this.savePoint);
            this.Controls.Add(this.BUT_Start);
            this.Controls.Add(this.BUT_leader);
            this.Controls.Add(this.CMB_mavs);
            this.Controls.Add(this.myButton3);
            this.Controls.Add(this.myButton6);
            this.Controls.Add(this.myButton5);
            this.Controls.Add(this.myButton4);
            this.Controls.Add(this.myButton2);
            this.Controls.Add(this.But_Brake);
            this.Controls.Add(this.BUT_Land);
            this.Controls.Add(this.BUT_Takeoff);
            this.Controls.Add(this.BUT_Disarm);
            this.Controls.Add(this.BUT_Arm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "FormationControl";
            this.Text = "Control";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private Controls.MyButton but_auto;
        private Controls.MyButton savePoint;
        private Controls.MyButton loadPoint;
        private Controls.MyButton But_Brake;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.BindingSource bindingSource2;
        private System.Windows.Forms.BindingSource bindingSource3;
        private System.Windows.Forms.BindingSource bindingSource4;
        private System.Windows.Forms.BindingSource bindingSource5;
        private Controls.MyButton myButton1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private Controls.MyButton myButton2;
        private Controls.MyButton myButton3;
        private System.Windows.Forms.TextBox textBox4;
        private Controls.MyButton myButton4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox7;
        private Controls.MyButton myButton5;
        private Controls.MyButton myButton6;
    }
}