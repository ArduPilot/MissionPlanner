namespace SimpleExample
{
    partial class simpleexample
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
            this.CMB_comport = new System.Windows.Forms.ComboBox();
            this.cmb_baudrate = new System.Windows.Forms.ComboBox();
            this.but_connect = new System.Windows.Forms.Button();
            this.but_armdisarm = new System.Windows.Forms.Button();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.but_mission = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // CMB_comport
            // 
            this.CMB_comport.FormattingEnabled = true;
            this.CMB_comport.Location = new System.Drawing.Point(13, 13);
            this.CMB_comport.Name = "CMB_comport";
            this.CMB_comport.Size = new System.Drawing.Size(121, 21);
            this.CMB_comport.TabIndex = 0;
            this.CMB_comport.Click += new System.EventHandler(this.CMB_comport_Click);
            // 
            // cmb_baudrate
            // 
            this.cmb_baudrate.FormattingEnabled = true;
            this.cmb_baudrate.Items.AddRange(new object[] {
            "9600",
            "14400",
            "19200",
            "28800",
            "38400",
            "57600",
            "115200"});
            this.cmb_baudrate.Location = new System.Drawing.Point(140, 12);
            this.cmb_baudrate.Name = "cmb_baudrate";
            this.cmb_baudrate.Size = new System.Drawing.Size(121, 21);
            this.cmb_baudrate.TabIndex = 1;
            // 
            // but_connect
            // 
            this.but_connect.Location = new System.Drawing.Point(268, 12);
            this.but_connect.Name = "but_connect";
            this.but_connect.Size = new System.Drawing.Size(75, 23);
            this.but_connect.TabIndex = 2;
            this.but_connect.Text = "Connect";
            this.but_connect.UseVisualStyleBackColor = true;
            this.but_connect.Click += new System.EventHandler(this.but_connect_Click);
            // 
            // but_armdisarm
            // 
            this.but_armdisarm.Location = new System.Drawing.Point(452, 12);
            this.but_armdisarm.Name = "but_armdisarm";
            this.but_armdisarm.Size = new System.Drawing.Size(75, 23);
            this.but_armdisarm.TabIndex = 3;
            this.but_armdisarm.Text = "Arm/Disarm";
            this.but_armdisarm.UseVisualStyleBackColor = true;
            this.but_armdisarm.Click += new System.EventHandler(this.but_armdisarm_Click);
            // 
            // but_mission
            // 
            this.but_mission.Location = new System.Drawing.Point(349, 12);
            this.but_mission.Name = "but_mission";
            this.but_mission.Size = new System.Drawing.Size(97, 23);
            this.but_mission.TabIndex = 4;
            this.but_mission.Text = "Send Mission";
            this.but_mission.UseVisualStyleBackColor = true;
            this.but_mission.Click += new System.EventHandler(this.but_mission_Click);
            // 
            // simpleexample
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(547, 62);
            this.Controls.Add(this.but_mission);
            this.Controls.Add(this.but_armdisarm);
            this.Controls.Add(this.but_connect);
            this.Controls.Add(this.cmb_baudrate);
            this.Controls.Add(this.CMB_comport);
            this.Name = "simpleexample";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_comport;
        private System.Windows.Forms.ComboBox cmb_baudrate;
        private System.Windows.Forms.Button but_connect;
        private System.Windows.Forms.Button but_armdisarm;
        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Button but_mission;
    }
}

