
namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigInitialParams
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigInitialParams));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cb_tmotor = new System.Windows.Forms.CheckBox();
            this.t_prop = new System.Windows.Forms.TextBox();
            this.t_cellcount = new System.Windows.Forms.TextBox();
            this.t_cellmax = new System.Windows.Forms.TextBox();
            this.t_cellmin = new System.Windows.Forms.TextBox();
            this.btn_docalc = new MissionPlanner.Controls.MyButton();
            this.cmb_batterytype = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cb_suggested = new System.Windows.Forms.CheckBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(37, 32);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(553, 137);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = resources.GetString("textBox1.Text");
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(99, 186);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Airscrew size in inch:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 217);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Battery cellcount:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 248);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(163, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Battery cell fully charged voltage:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 279);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(176, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Battery cell fully discharged voltage:";
            // 
            // cb_tmotor
            // 
            this.cb_tmotor.AutoSize = true;
            this.cb_tmotor.Location = new System.Drawing.Point(210, 314);
            this.cb_tmotor.Name = "cb_tmotor";
            this.cb_tmotor.Size = new System.Drawing.Size(154, 17);
            this.cb_tmotor.TabIndex = 5;
            this.cb_tmotor.Text = "Using T-Motor Flame ESC?";
            this.cb_tmotor.UseVisualStyleBackColor = true;
            // 
            // t_prop
            // 
            this.t_prop.Location = new System.Drawing.Point(210, 186);
            this.t_prop.Name = "t_prop";
            this.t_prop.Size = new System.Drawing.Size(100, 20);
            this.t_prop.TabIndex = 6;
            this.t_prop.Text = "12";
            // 
            // t_cellcount
            // 
            this.t_cellcount.Location = new System.Drawing.Point(210, 218);
            this.t_cellcount.Name = "t_cellcount";
            this.t_cellcount.Size = new System.Drawing.Size(100, 20);
            this.t_cellcount.TabIndex = 7;
            this.t_cellcount.Text = "4";
            // 
            // t_cellmax
            // 
            this.t_cellmax.Location = new System.Drawing.Point(210, 248);
            this.t_cellmax.Name = "t_cellmax";
            this.t_cellmax.Size = new System.Drawing.Size(100, 20);
            this.t_cellmax.TabIndex = 8;
            this.t_cellmax.Text = "4.2";
            // 
            // t_cellmin
            // 
            this.t_cellmin.Location = new System.Drawing.Point(210, 279);
            this.t_cellmin.Name = "t_cellmin";
            this.t_cellmin.Size = new System.Drawing.Size(100, 20);
            this.t_cellmin.TabIndex = 9;
            this.t_cellmin.Text = "3.3";
            // 
            // btn_docalc
            // 
            this.btn_docalc.Location = new System.Drawing.Point(185, 388);
            this.btn_docalc.Name = "btn_docalc";
            this.btn_docalc.Size = new System.Drawing.Size(236, 23);
            this.btn_docalc.TabIndex = 12;
            this.btn_docalc.Text = "Calculate Initial Parameters";
            this.btn_docalc.UseVisualStyleBackColor = true;
            this.btn_docalc.Click += new System.EventHandler(this.btn_docalc_Click);
            // 
            // cmb_batterytype
            // 
            this.cmb_batterytype.FormattingEnabled = true;
            this.cmb_batterytype.Items.AddRange(new object[] {
            "LiPo",
            "LiPoHV",
            "LiIon"});
            this.cmb_batterytype.Location = new System.Drawing.Point(389, 244);
            this.cmb_batterytype.Name = "cmb_batterytype";
            this.cmb_batterytype.Size = new System.Drawing.Size(121, 21);
            this.cmb_batterytype.TabIndex = 13;
            this.cmb_batterytype.SelectedIndexChanged += new System.EventHandler(this.cmb_batterytype_SelectedIndexChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(386, 221);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(88, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Battery Chemistry";
            // 
            // cb_suggested
            // 
            this.cb_suggested.AutoSize = true;
            this.cb_suggested.Location = new System.Drawing.Point(210, 337);
            this.cb_suggested.Name = "cb_suggested";
            this.cb_suggested.Size = new System.Drawing.Size(346, 17);
            this.cb_suggested.TabIndex = 15;
            this.cb_suggested.Text = "Add suggested settings for 4.0 and up (Battery failsafe and Fence) ?";
            this.cb_suggested.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(145, 469);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(316, 13);
            this.linkLabel1.TabIndex = 16;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://ardupilot.org/copter/docs/tuning-process-instructions.html";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(115, 447);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(377, 13);
            this.label6.TabIndex = 17;
            this.label6.Text = "You can find a detailed description of initial parameter settings and tuning here" +
    ".";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(253, 492);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(100, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "PLEASE READ IT !";
            // 
            // ConfigInitialParams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.cb_suggested);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.cmb_batterytype);
            this.Controls.Add(this.btn_docalc);
            this.Controls.Add(this.t_cellmin);
            this.Controls.Add(this.t_cellmax);
            this.Controls.Add(this.t_cellcount);
            this.Controls.Add(this.t_prop);
            this.Controls.Add(this.cb_tmotor);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Name = "ConfigInitialParams";
            this.Size = new System.Drawing.Size(612, 523);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox cb_tmotor;
        private System.Windows.Forms.TextBox t_prop;
        private System.Windows.Forms.TextBox t_cellcount;
        private System.Windows.Forms.TextBox t_cellmax;
        private System.Windows.Forms.TextBox t_cellmin;
        private Controls.MyButton btn_docalc;
        private System.Windows.Forms.ComboBox cmb_batterytype;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox cb_suggested;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
    }
}
