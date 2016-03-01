namespace MissionPlanner
{
    partial class SerialInjectGPS
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SerialInjectGPS));
            this.CMB_serialport = new System.Windows.Forms.ComboBox();
            this.BUT_connect = new MissionPlanner.Controls.MyButton();
            this.CMB_baudrate = new System.Windows.Forms.ComboBox();
            this.lbl_status = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // CMB_serialport
            // 
            this.CMB_serialport.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_serialport.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_serialport, "CMB_serialport");
            this.CMB_serialport.Name = "CMB_serialport";
            this.CMB_serialport.SelectedIndexChanged += new System.EventHandler(this.CMB_serialport_SelectedIndexChanged);
            // 
            // BUT_connect
            // 
            resources.ApplyResources(this.BUT_connect, "BUT_connect");
            this.BUT_connect.Name = "BUT_connect";
            this.BUT_connect.UseVisualStyleBackColor = true;
            this.BUT_connect.Click += new System.EventHandler(this.BUT_connect_Click);
            // 
            // CMB_baudrate
            // 
            this.CMB_baudrate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_baudrate.FormattingEnabled = true;
            this.CMB_baudrate.Items.AddRange(new object[] {
            resources.GetString("CMB_baudrate.Items"),
            resources.GetString("CMB_baudrate.Items1"),
            resources.GetString("CMB_baudrate.Items2"),
            resources.GetString("CMB_baudrate.Items3"),
            resources.GetString("CMB_baudrate.Items4"),
            resources.GetString("CMB_baudrate.Items5"),
            resources.GetString("CMB_baudrate.Items6"),
            resources.GetString("CMB_baudrate.Items7"),
            resources.GetString("CMB_baudrate.Items8"),
            resources.GetString("CMB_baudrate.Items9"),
            resources.GetString("CMB_baudrate.Items10")});
            resources.ApplyResources(this.CMB_baudrate, "CMB_baudrate");
            this.CMB_baudrate.Name = "CMB_baudrate";
            // 
            // lbl_status
            // 
            resources.ApplyResources(this.lbl_status, "lbl_status");
            this.lbl_status.Name = "lbl_status";
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // SerialInjectGPS
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbl_status);
            this.Controls.Add(this.CMB_baudrate);
            this.Controls.Add(this.BUT_connect);
            this.Controls.Add(this.CMB_serialport);
            this.Name = "SerialInjectGPS";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SerialInjectGPS_FormClosing);
            this.Load += new System.EventHandler(this.SerialInjectGPS_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_serialport;
        private Controls.MyButton BUT_connect;
        private System.Windows.Forms.ComboBox CMB_baudrate;
        private System.Windows.Forms.Label lbl_status;
        private System.Windows.Forms.Timer timer1;
    }
}