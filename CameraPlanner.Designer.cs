namespace MissionPlanner
{
    partial class CameraPlanner
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
            this.label10 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.num_overlap = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.num_sidelap = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.num_focallength = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.CMB_camera = new System.Windows.Forms.ComboBox();
            this.num_agl = new System.Windows.Forms.NumericUpDown();
            this.radio_camdir = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.fp_angle = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.CHKFill = new System.Windows.Forms.CheckBox();
            this.num_senswidth = new System.Windows.Forms.NumericUpDown();
            this.num_sensheight = new System.Windows.Forms.NumericUpDown();
            this.TXT_sensor_fplat = new System.Windows.Forms.TextBox();
            this.TXT_sensor_fplong = new System.Windows.Forms.TextBox();
            this.num_sensres = new System.Windows.Forms.NumericUpDown();
            this.label13 = new System.Windows.Forms.Label();
            this.TXT_fp_res = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.TXT_picevery = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.BUT_Ssave = new Controls.MyButton();
            this.OokButton = new Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.num_overlap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_sidelap)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_focallength)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_agl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fp_angle)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_senswidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_sensheight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_sensres)).BeginInit();
            this.SuspendLayout();
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label10.Location = new System.Drawing.Point(221, 43);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(156, 13);
            this.label10.TabIndex = 59;
            this.label10.Text = "Longitudinal sensor footprint [m]";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(86, 123);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 65;
            this.label4.Text = "Sensor width [mm]";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(246, 71);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 13);
            this.label6.TabIndex = 56;
            this.label6.Text = "Lateral sensor footprint [m]";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(86, 149);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 66;
            this.label3.Text = "Sensor height [mm]";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(86, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 55;
            this.label1.Text = "Focal length [mm]";
            // 
            // num_overlap
            // 
            this.num_overlap.DecimalPlaces = 1;
            this.num_overlap.Location = new System.Drawing.Point(13, 175);
            this.num_overlap.Name = "num_overlap";
            this.num_overlap.Size = new System.Drawing.Size(64, 20);
            this.num_overlap.TabIndex = 67;
            this.num_overlap.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.num_overlap.ValueChanged += new System.EventHandler(this.num_overlap_ValueChanged_1);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(86, 176);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(61, 13);
            this.label5.TabIndex = 68;
            this.label5.Text = "Overlap [%]";
            // 
            // num_sidelap
            // 
            this.num_sidelap.DecimalPlaces = 1;
            this.num_sidelap.Location = new System.Drawing.Point(13, 201);
            this.num_sidelap.Name = "num_sidelap";
            this.num_sidelap.Size = new System.Drawing.Size(64, 20);
            this.num_sidelap.TabIndex = 69;
            this.num_sidelap.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.num_sidelap.ValueChanged += new System.EventHandler(this.num_sidelap_ValueChanged_1);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label15.Location = new System.Drawing.Point(86, 203);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(59, 13);
            this.label15.TabIndex = 70;
            this.label15.Text = "Sidelap [%]";
            // 
            // num_focallength
            // 
            this.num_focallength.DecimalPlaces = 1;
            this.num_focallength.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.num_focallength.Location = new System.Drawing.Point(13, 68);
            this.num_focallength.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.num_focallength.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.num_focallength.Name = "num_focallength";
            this.num_focallength.Size = new System.Drawing.Size(64, 20);
            this.num_focallength.TabIndex = 47;
            this.num_focallength.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.num_focallength.ValueChanged += new System.EventHandler(this.num_focallength_ValueChanged_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(86, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 13);
            this.label2.TabIndex = 46;
            this.label2.Text = "Altitude AGL [m]";
            // 
            // CMB_camera
            // 
            this.CMB_camera.FormattingEnabled = true;
            this.CMB_camera.Location = new System.Drawing.Point(226, 227);
            this.CMB_camera.Name = "CMB_camera";
            this.CMB_camera.Size = new System.Drawing.Size(125, 21);
            this.CMB_camera.TabIndex = 76;
            this.CMB_camera.Text = "Select Configuration";
            this.CMB_camera.SelectedIndexChanged += new System.EventHandler(this.CMB_camera_SelectedIndexChanged_1);
            // 
            // num_agl
            // 
            this.num_agl.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.num_agl.Location = new System.Drawing.Point(13, 42);
            this.num_agl.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.num_agl.Name = "num_agl";
            this.num_agl.Size = new System.Drawing.Size(64, 20);
            this.num_agl.TabIndex = 45;
            this.num_agl.Value = new decimal(new int[] {
            150,
            0,
            0,
            0});
            this.num_agl.ValueChanged += new System.EventHandler(this.num_agl_ValueChanged_1);
            // 
            // radio_camdir
            // 
            this.radio_camdir.AutoSize = true;
            this.radio_camdir.Checked = true;
            this.radio_camdir.Location = new System.Drawing.Point(15, 260);
            this.radio_camdir.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radio_camdir.Name = "radio_camdir";
            this.radio_camdir.Size = new System.Drawing.Size(58, 17);
            this.radio_camdir.TabIndex = 78;
            this.radio_camdir.TabStop = true;
            this.radio_camdir.Text = "Portrait";
            this.radio_camdir.UseVisualStyleBackColor = true;
            this.radio_camdir.CheckedChanged += new System.EventHandler(this.radio_camdir_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(84, 260);
            this.radioButton2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(78, 17);
            this.radioButton2.TabIndex = 79;
            this.radioButton2.Text = "Landscape";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // fp_angle
            // 
            this.fp_angle.DecimalPlaces = 1;
            this.fp_angle.Location = new System.Drawing.Point(13, 228);
            this.fp_angle.Name = "fp_angle";
            this.fp_angle.Size = new System.Drawing.Size(64, 20);
            this.fp_angle.TabIndex = 80;
            this.fp_angle.ValueChanged += new System.EventHandler(this.fp_angle_ValueChanged);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label16.Location = new System.Drawing.Point(86, 229);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(82, 13);
            this.label16.TabIndex = 81;
            this.label16.Text = "Grid angle [deg]";
            // 
            // CHKFill
            // 
            this.CHKFill.AutoSize = true;
            this.CHKFill.Location = new System.Drawing.Point(15, 285);
            this.CHKFill.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.CHKFill.Name = "CHKFill";
            this.CHKFill.Size = new System.Drawing.Size(135, 17);
            this.CHKFill.TabIndex = 84;
            this.CHKFill.Text = "Draw interior waypoints";
            this.CHKFill.UseVisualStyleBackColor = true;
            // 
            // num_senswidth
            // 
            this.num_senswidth.DecimalPlaces = 2;
            this.num_senswidth.Location = new System.Drawing.Point(13, 122);
            this.num_senswidth.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.num_senswidth.Name = "num_senswidth";
            this.num_senswidth.Size = new System.Drawing.Size(64, 20);
            this.num_senswidth.TabIndex = 85;
            this.num_senswidth.Value = new decimal(new int[] {
            616,
            0,
            0,
            131072});
            this.num_senswidth.ValueChanged += new System.EventHandler(this.num_senswidth_ValueChanged);
            // 
            // num_sensheight
            // 
            this.num_sensheight.DecimalPlaces = 2;
            this.num_sensheight.Location = new System.Drawing.Point(13, 148);
            this.num_sensheight.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.num_sensheight.Name = "num_sensheight";
            this.num_sensheight.Size = new System.Drawing.Size(64, 20);
            this.num_sensheight.TabIndex = 86;
            this.num_sensheight.Value = new decimal(new int[] {
            462,
            0,
            0,
            131072});
            this.num_sensheight.ValueChanged += new System.EventHandler(this.num_sensheight_ValueChanged);
            // 
            // TXT_sensor_fplat
            // 
            this.TXT_sensor_fplat.Location = new System.Drawing.Point(383, 69);
            this.TXT_sensor_fplat.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TXT_sensor_fplat.Name = "TXT_sensor_fplat";
            this.TXT_sensor_fplat.Size = new System.Drawing.Size(49, 20);
            this.TXT_sensor_fplat.TabIndex = 87;
            // 
            // TXT_sensor_fplong
            // 
            this.TXT_sensor_fplong.Location = new System.Drawing.Point(383, 42);
            this.TXT_sensor_fplong.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TXT_sensor_fplong.Name = "TXT_sensor_fplong";
            this.TXT_sensor_fplong.Size = new System.Drawing.Size(49, 20);
            this.TXT_sensor_fplong.TabIndex = 88;
            // 
            // num_sensres
            // 
            this.num_sensres.DecimalPlaces = 1;
            this.num_sensres.Location = new System.Drawing.Point(13, 95);
            this.num_sensres.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.num_sensres.Name = "num_sensres";
            this.num_sensres.Size = new System.Drawing.Size(64, 20);
            this.num_sensres.TabIndex = 89;
            this.num_sensres.Value = new decimal(new int[] {
            101,
            0,
            0,
            65536});
            this.num_sensres.ValueChanged += new System.EventHandler(this.num_sensres_ValueChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(86, 96);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(113, 13);
            this.label13.TabIndex = 90;
            this.label13.Text = "Sensor resolution [MP]";
            // 
            // TXT_fp_res
            // 
            this.TXT_fp_res.Location = new System.Drawing.Point(383, 95);
            this.TXT_fp_res.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TXT_fp_res.Name = "TXT_fp_res";
            this.TXT_fp_res.Size = new System.Drawing.Size(49, 20);
            this.TXT_fp_res.TabIndex = 91;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label14.Location = new System.Drawing.Point(225, 96);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(157, 13);
            this.label14.TabIndex = 92;
            this.label14.Text = "Footprint resolution [cm^2/pixel]";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(12, 10);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(39, 17);
            this.label8.TabIndex = 93;
            this.label8.Text = "Input";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(224, 10);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(51, 17);
            this.label9.TabIndex = 94;
            this.label9.Text = "Output";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(224, 200);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(172, 17);
            this.label11.TabIndex = 96;
            this.label11.Text = "Load / Save Configuration";
            // 
            // TXT_picevery
            // 
            this.TXT_picevery.Location = new System.Drawing.Point(383, 122);
            this.TXT_picevery.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.TXT_picevery.Name = "TXT_picevery";
            this.TXT_picevery.Size = new System.Drawing.Size(49, 20);
            this.TXT_picevery.TabIndex = 97;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(261, 123);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(122, 13);
            this.label12.TabIndex = 98;
            this.label12.Text = "Take a picture every [m]";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(272, 142);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 13);
            this.label7.TabIndex = 99;
            this.label7.Text = "[CAM_TRIGG_DIST]";
            // 
            // BUT_Ssave
            // 
            this.BUT_Ssave.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_Ssave.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.BUT_Ssave.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BUT_Ssave.Location = new System.Drawing.Point(356, 227);
            this.BUT_Ssave.Name = "BUT_Ssave";
            this.BUT_Ssave.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_Ssave.Size = new System.Drawing.Size(75, 23);
            this.BUT_Ssave.TabIndex = 95;
            this.BUT_Ssave.Text = "Save";
            this.BUT_Ssave.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_Ssave.UseVisualStyleBackColor = true;
            this.BUT_Ssave.Click += new System.EventHandler(this.BUT_Ssave_Click);
            // 
            // OokButton
            // 
            this.OokButton.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.OokButton.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.OokButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.OokButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.OokButton.Location = new System.Drawing.Point(356, 256);
            this.OokButton.Name = "OokButton";
            this.OokButton.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.OokButton.Size = new System.Drawing.Size(75, 23);
            this.OokButton.TabIndex = 83;
            this.OokButton.Text = "Ok";
            this.OokButton.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.OokButton.UseVisualStyleBackColor = true;
            this.OokButton.Click += new System.EventHandler(this.OokButton_Click);
            // 
            // CameraPlanner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 313);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.TXT_picevery);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.TXT_fp_res);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.TXT_sensor_fplong);
            this.Controls.Add(this.TXT_sensor_fplat);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.BUT_Ssave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.num_overlap);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.num_sidelap);
            this.Controls.Add(this.num_sensres);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.num_sensheight);
            this.Controls.Add(this.num_focallength);
            this.Controls.Add(this.num_senswidth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CHKFill);
            this.Controls.Add(this.CMB_camera);
            this.Controls.Add(this.OokButton);
            this.Controls.Add(this.num_agl);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.radio_camdir);
            this.Controls.Add(this.fp_angle);
            this.Controls.Add(this.radioButton2);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "CameraPlanner";
            this.Load += new System.EventHandler(this.Camera_Load);
            ((System.ComponentModel.ISupportInitialize)(this.num_overlap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_sidelap)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_focallength)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_agl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fp_angle)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_senswidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_sensheight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_sensres)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown num_overlap;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown num_sidelap;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown num_focallength;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox CMB_camera;
        private System.Windows.Forms.NumericUpDown num_agl;
        private System.Windows.Forms.RadioButton radio_camdir;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.NumericUpDown fp_angle;
        private System.Windows.Forms.Label label16;
        private Controls.MyButton OokButton;
        private System.Windows.Forms.CheckBox CHKFill;
        private System.Windows.Forms.NumericUpDown num_senswidth;
        private System.Windows.Forms.NumericUpDown num_sensheight;
        private System.Windows.Forms.TextBox TXT_sensor_fplat;
        private System.Windows.Forms.TextBox TXT_sensor_fplong;
        private System.Windows.Forms.NumericUpDown num_sensres;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox TXT_fp_res;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private Controls.MyButton BUT_Ssave;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox TXT_picevery;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label7;


    }
}