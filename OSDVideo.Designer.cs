namespace MissionPlanner
{
    partial class OSDVideo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OSDVideo));
            this.txtAviFileName = new System.Windows.Forms.TextBox();
            this.txt_tlog = new System.Windows.Forms.TextBox();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.trackBar_mediapos = new System.Windows.Forms.TrackBar();
            this.BUT_tlogfile = new MissionPlanner.Controls.MyButton();
            this.hud1 = new MissionPlanner.Controls.HUD();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.BUT_start = new MissionPlanner.Controls.MyButton();
            this.BUT_vidfile = new MissionPlanner.Controls.MyButton();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.CHK_fullres = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_mediapos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtAviFileName
            // 
            this.txtAviFileName.Location = new System.Drawing.Point(13, 13);
            this.txtAviFileName.Name = "txtAviFileName";
            this.txtAviFileName.Size = new System.Drawing.Size(314, 20);
            this.txtAviFileName.TabIndex = 1;
            // 
            // txt_tlog
            // 
            this.txt_tlog.Location = new System.Drawing.Point(13, 38);
            this.txt_tlog.Name = "txt_tlog";
            this.txt_tlog.Size = new System.Drawing.Size(314, 20);
            this.txt_tlog.TabIndex = 5;
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(416, 12);
            this.trackBar1.Maximum = 900;
            this.trackBar1.Minimum = -900;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(350, 45);
            this.trackBar1.TabIndex = 7;
            this.trackBar1.TickFrequency = 50;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // trackBar_mediapos
            // 
            this.trackBar_mediapos.Enabled = false;
            this.trackBar_mediapos.Location = new System.Drawing.Point(12, 64);
            this.trackBar_mediapos.Maximum = 900;
            this.trackBar_mediapos.Minimum = -900;
            this.trackBar_mediapos.Name = "trackBar_mediapos";
            this.trackBar_mediapos.Size = new System.Drawing.Size(835, 45);
            this.trackBar_mediapos.TabIndex = 8;
            this.trackBar_mediapos.TickFrequency = 50;
            this.trackBar_mediapos.Scroll += new System.EventHandler(this.trackBar_mediapos_Scroll);
            // 
            // BUT_tlogfile
            // 
            this.BUT_tlogfile.Location = new System.Drawing.Point(334, 38);
            this.BUT_tlogfile.Name = "BUT_tlogfile";
            this.BUT_tlogfile.Size = new System.Drawing.Size(75, 19);
            this.BUT_tlogfile.TabIndex = 6;
            this.BUT_tlogfile.Text = "Browse Tlog";
            this.BUT_tlogfile.UseVisualStyleBackColor = true;
            this.BUT_tlogfile.Click += new System.EventHandler(this.BUT_tlogfile_Click);
            // 
            // hud1
            // 
            this.hud1.airspeed = 0F;
            this.hud1.alt = 0F;
            this.hud1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hud1.BackColor = System.Drawing.Color.Black;
            this.hud1.batterylevel = 0F;
            this.hud1.batteryremaining = 0F;
            this.hud1.connected = false;
            this.hud1.current = 0F;
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("airspeed", this.bindingSource1, "airspeed", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("alt", this.bindingSource1, "alt", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("batterylevel", this.bindingSource1, "battery_voltage", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("batteryremaining", this.bindingSource1, "battery_remaining", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("current", this.bindingSource1, "current", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("datetime", this.bindingSource1, "datetime", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("disttowp", this.bindingSource1, "wp_dist", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("gpsfix", this.bindingSource1, "gpsstatus", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("gpshdop", this.bindingSource1, "gpshdop", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("groundalt", this.bindingSource1, "HomeAlt", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("groundcourse", this.bindingSource1, "groundcourse", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("groundspeed", this.bindingSource1, "groundspeed", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("heading", this.bindingSource1, "yaw", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("linkqualitygcs", this.bindingSource1, "linkqualitygcs", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("mode", this.bindingSource1, "mode", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("navpitch", this.bindingSource1, "nav_pitch", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("navroll", this.bindingSource1, "nav_roll", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("pitch", this.bindingSource1, "pitch", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("roll", this.bindingSource1, "roll", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("status", this.bindingSource1, "armed", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("targetalt", this.bindingSource1, "targetalt", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("targetheading", this.bindingSource1, "nav_bearing", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("targetspeed", this.bindingSource1, "targetairspeed", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("turnrate", this.bindingSource1, "turnrate", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("verticalspeed", this.bindingSource1, "verticalspeed", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("wpno", this.bindingSource1, "wpno", true));
            this.hud1.DataBindings.Add(new System.Windows.Forms.Binding("xtrack_error", this.bindingSource1, "xtrack_error", true));
            this.hud1.datetime = new System.DateTime(((long)(0)));
            this.hud1.disttowp = 0F;
            this.hud1.failsafe = false;
            this.hud1.gpsfix = 0F;
            this.hud1.gpshdop = 0F;
            this.hud1.groundalt = 0F;
            this.hud1.groundcourse = 0F;
            this.hud1.groundspeed = 0F;
            this.hud1.heading = 0F;
            this.hud1.hudcolor = System.Drawing.Color.White;
            this.hud1.linkqualitygcs = 100F;
            this.hud1.Location = new System.Drawing.Point(13, 115);
            this.hud1.lowairspeed = false;
            this.hud1.lowgroundspeed = false;
            this.hud1.lowvoltagealert = false;
            this.hud1.message = "";
            this.hud1.messagetime = new System.DateTime(((long)(0)));
            this.hud1.mode = "Manual";
            this.hud1.Name = "hud1";
            this.hud1.navpitch = 0F;
            this.hud1.navroll = 0F;
            this.hud1.opengl = true;
            this.hud1.pitch = 0F;
            this.hud1.roll = 0F;
            this.hud1.Russian = false;
            this.hud1.Size = new System.Drawing.Size(832, 448);
            this.hud1.status = false;
            this.hud1.streamjpg = ((System.IO.MemoryStream)(resources.GetObject("hud1.streamjpg")));
            this.hud1.TabIndex = 4;
            this.hud1.targetalt = 0F;
            this.hud1.targetheading = 0F;
            this.hud1.targetspeed = 0F;
            this.hud1.turnrate = 0F;
            this.hud1.verticalspeed = 0F;
            this.hud1.VSync = false;
            this.hud1.wpno = 0;
            this.hud1.xtrack_error = 0F;
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // BUT_start
            // 
            this.BUT_start.Location = new System.Drawing.Point(772, 29);
            this.BUT_start.Name = "BUT_start";
            this.BUT_start.Size = new System.Drawing.Size(75, 19);
            this.BUT_start.TabIndex = 3;
            this.BUT_start.Text = Strings.Start;
            this.BUT_start.UseVisualStyleBackColor = true;
            this.BUT_start.Click += new System.EventHandler(this.BUT_start_Click);
            // 
            // BUT_vidfile
            // 
            this.BUT_vidfile.Location = new System.Drawing.Point(334, 13);
            this.BUT_vidfile.Name = "BUT_vidfile";
            this.BUT_vidfile.Size = new System.Drawing.Size(75, 19);
            this.BUT_vidfile.TabIndex = 2;
            this.BUT_vidfile.Text = "Browse Video";
            this.BUT_vidfile.UseVisualStyleBackColor = true;
            this.BUT_vidfile.Click += new System.EventHandler(this.BUT_vidfile_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(854, 115);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(118, 294);
            this.textBox1.TabIndex = 9;
            this.textBox1.Text = "Please make sure you have installed\r\nffdshow\r\nand haali media splitter.";
            // 
            // CHK_fullres
            // 
            this.CHK_fullres.AutoSize = true;
            this.CHK_fullres.Location = new System.Drawing.Point(854, 64);
            this.CHK_fullres.Name = "CHK_fullres";
            this.CHK_fullres.Size = new System.Drawing.Size(64, 17);
            this.CHK_fullres.TabIndex = 10;
            this.CHK_fullres.Text = "Full Res";
            this.CHK_fullres.UseVisualStyleBackColor = true;
            this.CHK_fullres.CheckedChanged += new System.EventHandler(this.CHK_fullres_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(574, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "time offset in seconds";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(413, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "playback timeline";
            // 
            // OSDVideo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            
            this.ClientSize = new System.Drawing.Size(984, 574);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CHK_fullres);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.trackBar_mediapos);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.BUT_tlogfile);
            this.Controls.Add(this.txt_tlog);
            this.Controls.Add(this.hud1);
            this.Controls.Add(this.BUT_start);
            this.Controls.Add(this.BUT_vidfile);
            this.Controls.Add(this.txtAviFileName);
            this.Name = "OSDVideo";
            this.Text = "OSDVideo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OSDVideo_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_mediapos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtAviFileName;
        private Controls.MyButton BUT_vidfile;
        private Controls.MyButton BUT_start;
        private Controls.HUD hud1;
        private Controls.MyButton BUT_tlogfile;
        private System.Windows.Forms.TextBox txt_tlog;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.TrackBar trackBar_mediapos;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox CHK_fullres;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}