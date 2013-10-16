namespace MissionPlanner
{
    partial class temp
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.CMB_mavs = new System.Windows.Forms.ComboBox();
            this.but_multimav = new MissionPlanner.Controls.MyButton();
            this.BUT_xplane = new MissionPlanner.Controls.MyButton();
            this.but_osdvideo = new MissionPlanner.Controls.MyButton();
            this.BUT_outputMD = new MissionPlanner.Controls.MyButton();
            this.BUT_paramgen = new MissionPlanner.Controls.MyButton();
            this.BUT_follow_me = new MissionPlanner.Controls.MyButton();
            this.BUT_georefimage = new MissionPlanner.Controls.MyButton();
            this.BUT_lang_edit = new MissionPlanner.Controls.MyButton();
            this.BUT_clearcustommaps = new MissionPlanner.Controls.MyButton();
            this.BUT_geinjection = new MissionPlanner.Controls.MyButton();
            this.button2 = new MissionPlanner.Controls.MyButton();
            this.BUT_copyto2560 = new MissionPlanner.Controls.MyButton();
            this.BUT_copyto1280 = new MissionPlanner.Controls.MyButton();
            this.BUT_copy2560 = new MissionPlanner.Controls.MyButton();
            this.BUT_copy1280 = new MissionPlanner.Controls.MyButton();
            this.BUT_dleeprom = new MissionPlanner.Controls.MyButton();
            this.BUT_flashup = new MissionPlanner.Controls.MyButton();
            this.BUT_flashdl = new MissionPlanner.Controls.MyButton();
            this.BUT_wipeeeprom = new MissionPlanner.Controls.MyButton();
            this.button1 = new MissionPlanner.Controls.MyButton();
            this.BUT_swarm = new MissionPlanner.Controls.MyButton();
            this.BUT_outputnmea = new MissionPlanner.Controls.MyButton();
            this.BUT_outputMavlink = new MissionPlanner.Controls.MyButton();
            this.BUT_simmulti = new MissionPlanner.Controls.MyButton();
            this.BUT_followleader = new MissionPlanner.Controls.MyButton();
            this.BUT_compassmot = new MissionPlanner.Controls.MyButton();
            this.BUT_driverclean = new MissionPlanner.Controls.MyButton();
            this.but_compassrotation = new MissionPlanner.Controls.MyButton();
            this.BUT_sorttlogs = new MissionPlanner.Controls.MyButton();
            this.windDir1 = new MissionPlanner.Controls.WindDir();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(274, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Includes eeprom";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(274, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Does not include eeprom";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(95, 81);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(273, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "1280 - can do full copy\'s   2560- full read, write flash only";
            // 
            // CMB_mavs
            // 
            this.CMB_mavs.FormattingEnabled = true;
            this.CMB_mavs.Location = new System.Drawing.Point(12, 258);
            this.CMB_mavs.Name = "CMB_mavs";
            this.CMB_mavs.Size = new System.Drawing.Size(121, 21);
            this.CMB_mavs.TabIndex = 26;
            this.CMB_mavs.SelectedIndexChanged += new System.EventHandler(this.CMB_mavs_SelectedIndexChanged);
            // 
            // but_multimav
            // 
            this.but_multimav.Location = new System.Drawing.Point(37, 229);
            this.but_multimav.Name = "but_multimav";
            this.but_multimav.Size = new System.Drawing.Size(75, 23);
            this.but_multimav.TabIndex = 25;
            this.but_multimav.Text = "2nd mav";
            this.but_multimav.UseVisualStyleBackColor = true;
            this.but_multimav.Click += new System.EventHandler(this.but_multimav_Click);
            // 
            // BUT_xplane
            // 
            this.BUT_xplane.Location = new System.Drawing.Point(598, 238);
            this.BUT_xplane.Name = "BUT_xplane";
            this.BUT_xplane.Size = new System.Drawing.Size(75, 23);
            this.BUT_xplane.TabIndex = 23;
            this.BUT_xplane.Text = "live xplane output";
            this.BUT_xplane.UseVisualStyleBackColor = true;
            this.BUT_xplane.Click += new System.EventHandler(this.BUT_xplane_Click);
            // 
            // but_osdvideo
            // 
            this.but_osdvideo.Location = new System.Drawing.Point(495, 183);
            this.but_osdvideo.Name = "but_osdvideo";
            this.but_osdvideo.Size = new System.Drawing.Size(75, 23);
            this.but_osdvideo.TabIndex = 22;
            this.but_osdvideo.Text = "OSDVideo";
            this.but_osdvideo.UseVisualStyleBackColor = true;
            this.but_osdvideo.Click += new System.EventHandler(this.but_osdvideo_Click);
            // 
            // BUT_outputMD
            // 
            this.BUT_outputMD.Location = new System.Drawing.Point(308, 132);
            this.BUT_outputMD.Name = "BUT_outputMD";
            this.BUT_outputMD.Size = new System.Drawing.Size(75, 23);
            this.BUT_outputMD.TabIndex = 21;
            this.BUT_outputMD.Text = "MD";
            this.BUT_outputMD.UseVisualStyleBackColor = true;
            this.BUT_outputMD.Click += new System.EventHandler(this.myButton1_Click);
            // 
            // BUT_paramgen
            // 
            this.BUT_paramgen.Location = new System.Drawing.Point(414, 183);
            this.BUT_paramgen.Name = "BUT_paramgen";
            this.BUT_paramgen.Size = new System.Drawing.Size(75, 23);
            this.BUT_paramgen.TabIndex = 20;
            this.BUT_paramgen.Text = "Param gen";
            this.BUT_paramgen.UseVisualStyleBackColor = true;
            this.BUT_paramgen.Click += new System.EventHandler(this.BUT_paramgen_Click);
            // 
            // BUT_follow_me
            // 
            this.BUT_follow_me.Location = new System.Drawing.Point(333, 183);
            this.BUT_follow_me.Name = "BUT_follow_me";
            this.BUT_follow_me.Size = new System.Drawing.Size(75, 23);
            this.BUT_follow_me.TabIndex = 17;
            this.BUT_follow_me.Text = "Follow Me";
            this.BUT_follow_me.UseVisualStyleBackColor = true;
            this.BUT_follow_me.Click += new System.EventHandler(this.BUT_follow_me_Click);
            // 
            // BUT_georefimage
            // 
            this.BUT_georefimage.Location = new System.Drawing.Point(150, 183);
            this.BUT_georefimage.Name = "BUT_georefimage";
            this.BUT_georefimage.Size = new System.Drawing.Size(96, 23);
            this.BUT_georefimage.TabIndex = 0;
            this.BUT_georefimage.Text = "Geo ref images";
            this.BUT_georefimage.Click += new System.EventHandler(this.BUT_georefimage_Click);
            // 
            // BUT_lang_edit
            // 
            this.BUT_lang_edit.Location = new System.Drawing.Point(252, 183);
            this.BUT_lang_edit.Name = "BUT_lang_edit";
            this.BUT_lang_edit.Size = new System.Drawing.Size(75, 23);
            this.BUT_lang_edit.TabIndex = 16;
            this.BUT_lang_edit.Text = "Lang Edit";
            this.BUT_lang_edit.UseVisualStyleBackColor = true;
            this.BUT_lang_edit.Click += new System.EventHandler(this.BUT_lang_edit_Click);
            // 
            // BUT_clearcustommaps
            // 
            this.BUT_clearcustommaps.Location = new System.Drawing.Point(365, 229);
            this.BUT_clearcustommaps.Name = "BUT_clearcustommaps";
            this.BUT_clearcustommaps.Size = new System.Drawing.Size(209, 40);
            this.BUT_clearcustommaps.TabIndex = 15;
            this.BUT_clearcustommaps.Text = "Clear Custom Maps from Database";
            this.BUT_clearcustommaps.UseVisualStyleBackColor = true;
            this.BUT_clearcustommaps.Click += new System.EventHandler(this.BUT_clearcustommaps_Click);
            // 
            // BUT_geinjection
            // 
            this.BUT_geinjection.Location = new System.Drawing.Point(150, 229);
            this.BUT_geinjection.Name = "BUT_geinjection";
            this.BUT_geinjection.Size = new System.Drawing.Size(209, 40);
            this.BUT_geinjection.TabIndex = 14;
            this.BUT_geinjection.Text = "Inject GE into database (now jpgs)";
            this.BUT_geinjection.UseVisualStyleBackColor = true;
            this.BUT_geinjection.Click += new System.EventHandler(this.BUT_geinjection_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(37, 97);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(159, 23);
            this.button2.TabIndex = 9;
            this.button2.Text = "firmware.hex 2 firmware.bin";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // BUT_copyto2560
            // 
            this.BUT_copyto2560.Location = new System.Drawing.Point(143, 41);
            this.BUT_copyto2560.Name = "BUT_copyto2560";
            this.BUT_copyto2560.Size = new System.Drawing.Size(125, 23);
            this.BUT_copyto2560.TabIndex = 8;
            this.BUT_copyto2560.Text = "Copy to APM 2560";
            this.BUT_copyto2560.UseVisualStyleBackColor = true;
            // 
            // BUT_copyto1280
            // 
            this.BUT_copyto1280.Location = new System.Drawing.Point(143, 12);
            this.BUT_copyto1280.Name = "BUT_copyto1280";
            this.BUT_copyto1280.Size = new System.Drawing.Size(125, 23);
            this.BUT_copyto1280.TabIndex = 7;
            this.BUT_copyto1280.Text = "Copy to APM 1280";
            this.BUT_copyto1280.UseVisualStyleBackColor = true;
            this.BUT_copyto1280.Click += new System.EventHandler(this.BUT_copyto1280_Click);
            // 
            // BUT_copy2560
            // 
            this.BUT_copy2560.Location = new System.Drawing.Point(12, 41);
            this.BUT_copy2560.Name = "BUT_copy2560";
            this.BUT_copy2560.Size = new System.Drawing.Size(125, 23);
            this.BUT_copy2560.TabIndex = 6;
            this.BUT_copy2560.Text = "Copy APM 2560";
            this.BUT_copy2560.UseVisualStyleBackColor = true;
            this.BUT_copy2560.Click += new System.EventHandler(this.BUT_copy2560_Click);
            // 
            // BUT_copy1280
            // 
            this.BUT_copy1280.Location = new System.Drawing.Point(12, 12);
            this.BUT_copy1280.Name = "BUT_copy1280";
            this.BUT_copy1280.Size = new System.Drawing.Size(125, 23);
            this.BUT_copy1280.TabIndex = 5;
            this.BUT_copy1280.Text = "Copy APM 1280";
            this.BUT_copy1280.UseVisualStyleBackColor = true;
            this.BUT_copy1280.Click += new System.EventHandler(this.BUT_copy1280_Click);
            // 
            // BUT_dleeprom
            // 
            this.BUT_dleeprom.Location = new System.Drawing.Point(476, 46);
            this.BUT_dleeprom.Name = "BUT_dleeprom";
            this.BUT_dleeprom.Size = new System.Drawing.Size(125, 23);
            this.BUT_dleeprom.TabIndex = 4;
            this.BUT_dleeprom.Text = "download eeprom";
            this.BUT_dleeprom.UseVisualStyleBackColor = true;
            this.BUT_dleeprom.Click += new System.EventHandler(this.BUT_dleeprom_Click);
            // 
            // BUT_flashup
            // 
            this.BUT_flashup.Location = new System.Drawing.Point(542, 81);
            this.BUT_flashup.Name = "BUT_flashup";
            this.BUT_flashup.Size = new System.Drawing.Size(125, 23);
            this.BUT_flashup.TabIndex = 3;
            this.BUT_flashup.Text = "upload flash";
            this.BUT_flashup.UseVisualStyleBackColor = true;
            this.BUT_flashup.Click += new System.EventHandler(this.BUT_flashup_Click);
            // 
            // BUT_flashdl
            // 
            this.BUT_flashdl.Location = new System.Drawing.Point(411, 81);
            this.BUT_flashdl.Name = "BUT_flashdl";
            this.BUT_flashdl.Size = new System.Drawing.Size(125, 23);
            this.BUT_flashdl.TabIndex = 2;
            this.BUT_flashdl.Text = "download flash";
            this.BUT_flashdl.UseVisualStyleBackColor = true;
            this.BUT_flashdl.Click += new System.EventHandler(this.BUT_flashdl_Click);
            // 
            // BUT_wipeeeprom
            // 
            this.BUT_wipeeeprom.Location = new System.Drawing.Point(411, 12);
            this.BUT_wipeeeprom.Name = "BUT_wipeeeprom";
            this.BUT_wipeeeprom.Size = new System.Drawing.Size(125, 23);
            this.BUT_wipeeeprom.TabIndex = 1;
            this.BUT_wipeeeprom.Text = "WIPE eeprom";
            this.BUT_wipeeeprom.UseVisualStyleBackColor = true;
            this.BUT_wipeeeprom.Click += new System.EventHandler(this.BUT_wipeeeprom_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(542, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(125, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "upload eeprom";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // BUT_swarm
            // 
            this.BUT_swarm.Location = new System.Drawing.Point(307, 287);
            this.BUT_swarm.Name = "BUT_swarm";
            this.BUT_swarm.Size = new System.Drawing.Size(75, 23);
            this.BUT_swarm.TabIndex = 27;
            this.BUT_swarm.Text = "Swarm";
            this.BUT_swarm.UseVisualStyleBackColor = true;
            this.BUT_swarm.Click += new System.EventHandler(this.BUT_swarm_Click);
            // 
            // BUT_outputnmea
            // 
            this.BUT_outputnmea.Location = new System.Drawing.Point(227, 132);
            this.BUT_outputnmea.Name = "BUT_outputnmea";
            this.BUT_outputnmea.Size = new System.Drawing.Size(75, 23);
            this.BUT_outputnmea.TabIndex = 28;
            this.BUT_outputnmea.Text = "NMEA";
            this.BUT_outputnmea.UseVisualStyleBackColor = true;
            this.BUT_outputnmea.Click += new System.EventHandler(this.BUT_outputnmea_Click);
            // 
            // BUT_outputMavlink
            // 
            this.BUT_outputMavlink.Location = new System.Drawing.Point(389, 132);
            this.BUT_outputMavlink.Name = "BUT_outputMavlink";
            this.BUT_outputMavlink.Size = new System.Drawing.Size(75, 23);
            this.BUT_outputMavlink.TabIndex = 29;
            this.BUT_outputMavlink.Text = "Mavlink";
            this.BUT_outputMavlink.UseVisualStyleBackColor = true;
            this.BUT_outputMavlink.Click += new System.EventHandler(this.BUT_outputMavlink_Click);
            // 
            // BUT_simmulti
            // 
            this.BUT_simmulti.Location = new System.Drawing.Point(592, 287);
            this.BUT_simmulti.Name = "BUT_simmulti";
            this.BUT_simmulti.Size = new System.Drawing.Size(75, 23);
            this.BUT_simmulti.TabIndex = 31;
            this.BUT_simmulti.Text = "Multi Sim";
            this.BUT_simmulti.UseVisualStyleBackColor = true;
            this.BUT_simmulti.Click += new System.EventHandler(this.BUT_simmulti_Click);
            // 
            // BUT_followleader
            // 
            this.BUT_followleader.Location = new System.Drawing.Point(307, 316);
            this.BUT_followleader.Name = "BUT_followleader";
            this.BUT_followleader.Size = new System.Drawing.Size(75, 23);
            this.BUT_followleader.TabIndex = 33;
            this.BUT_followleader.Text = "Follow the leader";
            this.BUT_followleader.UseVisualStyleBackColor = true;
            this.BUT_followleader.Click += new System.EventHandler(this.BUT_followleader_Click);
            // 
            // BUT_compassmot
            // 
            this.BUT_compassmot.Location = new System.Drawing.Point(461, 338);
            this.BUT_compassmot.Name = "BUT_compassmot";
            this.BUT_compassmot.Size = new System.Drawing.Size(75, 23);
            this.BUT_compassmot.TabIndex = 35;
            this.BUT_compassmot.Text = "CompassMot";
            this.BUT_compassmot.UseVisualStyleBackColor = true;
            this.BUT_compassmot.Click += new System.EventHandler(this.BUT_compassmot_Click);
            // 
            // BUT_driverclean
            // 
            this.BUT_driverclean.Location = new System.Drawing.Point(526, 380);
            this.BUT_driverclean.Name = "BUT_driverclean";
            this.BUT_driverclean.Size = new System.Drawing.Size(75, 23);
            this.BUT_driverclean.TabIndex = 36;
            this.BUT_driverclean.Text = "Driver Clean";
            this.BUT_driverclean.UseVisualStyleBackColor = true;
            this.BUT_driverclean.Click += new System.EventHandler(this.BUT_driverclean_Click);
            // 
            // but_compassrotation
            // 
            this.but_compassrotation.Location = new System.Drawing.Point(542, 338);
            this.but_compassrotation.Name = "but_compassrotation";
            this.but_compassrotation.Size = new System.Drawing.Size(75, 23);
            this.but_compassrotation.TabIndex = 37;
            this.but_compassrotation.Text = "Compass Rotation";
            this.but_compassrotation.UseVisualStyleBackColor = true;
            this.but_compassrotation.Click += new System.EventHandler(this.but_compassrotation_Click);
            // 
            // BUT_sorttlogs
            // 
            this.BUT_sorttlogs.Location = new System.Drawing.Point(526, 409);
            this.BUT_sorttlogs.Name = "BUT_sorttlogs";
            this.BUT_sorttlogs.Size = new System.Drawing.Size(75, 23);
            this.BUT_sorttlogs.TabIndex = 38;
            this.BUT_sorttlogs.Text = "Sort TLogs";
            this.BUT_sorttlogs.UseVisualStyleBackColor = true;
            this.BUT_sorttlogs.Click += new System.EventHandler(this.BUT_sorttlogs_Click);
            // 
            // windDir1
            // 
            this.windDir1.Direction = 0D;
            this.windDir1.Location = new System.Drawing.Point(168, 338);
            this.windDir1.Name = "windDir1";
            this.windDir1.Size = new System.Drawing.Size(100, 100);
            this.windDir1.Speed = 0D;
            this.windDir1.TabIndex = 39;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // temp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 450);
            this.Controls.Add(this.windDir1);
            this.Controls.Add(this.BUT_sorttlogs);
            this.Controls.Add(this.but_compassrotation);
            this.Controls.Add(this.BUT_driverclean);
            this.Controls.Add(this.BUT_compassmot);
            this.Controls.Add(this.BUT_followleader);
            this.Controls.Add(this.BUT_simmulti);
            this.Controls.Add(this.BUT_outputMavlink);
            this.Controls.Add(this.BUT_outputnmea);
            this.Controls.Add(this.BUT_swarm);
            this.Controls.Add(this.CMB_mavs);
            this.Controls.Add(this.but_multimav);
            this.Controls.Add(this.BUT_xplane);
            this.Controls.Add(this.but_osdvideo);
            this.Controls.Add(this.BUT_outputMD);
            this.Controls.Add(this.BUT_paramgen);
            this.Controls.Add(this.BUT_follow_me);
            this.Controls.Add(this.BUT_georefimage);
            this.Controls.Add(this.BUT_lang_edit);
            this.Controls.Add(this.BUT_clearcustommaps);
            this.Controls.Add(this.BUT_geinjection);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.BUT_copyto2560);
            this.Controls.Add(this.BUT_copyto1280);
            this.Controls.Add(this.BUT_copy2560);
            this.Controls.Add(this.BUT_copy1280);
            this.Controls.Add(this.BUT_dleeprom);
            this.Controls.Add(this.BUT_flashup);
            this.Controls.Add(this.BUT_flashdl);
            this.Controls.Add(this.BUT_wipeeeprom);
            this.Controls.Add(this.button1);
            this.Name = "temp";
            this.Text = "temp";
            this.Load += new System.EventHandler(this.temp_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MyButton button1;
        private Controls.MyButton BUT_wipeeeprom;
        private Controls.MyButton BUT_flashdl;
        private Controls.MyButton BUT_flashup;
        private Controls.MyButton BUT_dleeprom;
        private Controls.MyButton BUT_copy1280;
        private Controls.MyButton BUT_copy2560;
        private Controls.MyButton BUT_copyto2560;
        private Controls.MyButton BUT_copyto1280;
        private Controls.MyButton button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private Controls.MyButton BUT_geinjection;
        private Controls.MyButton BUT_clearcustommaps;
        private Controls.MyButton BUT_lang_edit;
        private Controls.MyButton BUT_georefimage;
        private Controls.MyButton BUT_follow_me;
        private Controls.MyButton BUT_paramgen;
        private Controls.MyButton BUT_outputMD;
        private Controls.MyButton but_osdvideo;
        private Controls.MyButton BUT_xplane;
        private Controls.MyButton but_multimav;
        private System.Windows.Forms.ComboBox CMB_mavs;
        private Controls.MyButton BUT_swarm;
        private Controls.MyButton BUT_outputnmea;
        private Controls.MyButton BUT_outputMavlink;
        private Controls.MyButton BUT_simmulti;
        private Controls.MyButton BUT_followleader;
        private Controls.MyButton BUT_compassmot;
        private Controls.MyButton BUT_driverclean;
        private Controls.MyButton but_compassrotation;
        private Controls.MyButton BUT_sorttlogs;
        private Controls.WindDir windDir1;
        private System.Windows.Forms.Timer timer1;
    }
}