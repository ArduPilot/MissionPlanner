namespace MissionPlanner.Controls
{
    partial class ConnectionControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionControl));
            this.cmb_Baud = new System.Windows.Forms.ComboBox();
            this.cmb_ConnectionType = new System.Windows.Forms.ComboBox();
            this.cmb_Connection = new System.Windows.Forms.ComboBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.cmb_sysid = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cmb_Baud
            // 
            this.cmb_Baud.BackColor = System.Drawing.Color.Black;
            this.cmb_Baud.DropDownWidth = 150;
            this.cmb_Baud.ForeColor = System.Drawing.Color.White;
            this.cmb_Baud.FormattingEnabled = true;
            this.cmb_Baud.Items.AddRange(new object[] {
            resources.GetString("cmb_Baud.Items"),
            resources.GetString("cmb_Baud.Items1"),
            resources.GetString("cmb_Baud.Items2"),
            resources.GetString("cmb_Baud.Items3"),
            resources.GetString("cmb_Baud.Items4"),
            resources.GetString("cmb_Baud.Items5"),
            resources.GetString("cmb_Baud.Items6"),
            resources.GetString("cmb_Baud.Items7"),
            resources.GetString("cmb_Baud.Items8"),
            resources.GetString("cmb_Baud.Items9"),
            resources.GetString("cmb_Baud.Items10"),
            resources.GetString("cmb_Baud.Items11"),
            resources.GetString("cmb_Baud.Items12")});
            resources.ApplyResources(this.cmb_Baud, "cmb_Baud");
            this.cmb_Baud.Name = "cmb_Baud";
            // 
            // cmb_ConnectionType
            // 
            this.cmb_ConnectionType.BackColor = System.Drawing.Color.Black;
            this.cmb_ConnectionType.DropDownWidth = 122;
            this.cmb_ConnectionType.ForeColor = System.Drawing.Color.White;
            this.cmb_ConnectionType.FormattingEnabled = true;
            resources.ApplyResources(this.cmb_ConnectionType, "cmb_ConnectionType");
            this.cmb_ConnectionType.Name = "cmb_ConnectionType";
            // 
            // cmb_Connection
            // 
            this.cmb_Connection.BackColor = System.Drawing.Color.Black;
            this.cmb_Connection.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmb_Connection.DropDownWidth = 200;
            this.cmb_Connection.ForeColor = System.Drawing.Color.White;
            this.cmb_Connection.FormattingEnabled = true;
            resources.ApplyResources(this.cmb_Connection, "cmb_Connection");
            this.cmb_Connection.Name = "cmb_Connection";
            this.cmb_Connection.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.cmb_Connection_DrawItem);
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.BackColor = System.Drawing.Color.Transparent;
            this.linkLabel1.Image = global::MissionPlanner.Properties.Resources.bgdark;
            this.linkLabel1.LinkColor = System.Drawing.Color.White;
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            // 
            // cmb_sysid
            // 
            this.cmb_sysid.BackColor = System.Drawing.Color.Black;
            this.cmb_sysid.DropDownWidth = 160;
            this.cmb_sysid.ForeColor = System.Drawing.Color.White;
            this.cmb_sysid.FormattingEnabled = true;
            resources.ApplyResources(this.cmb_sysid, "cmb_sysid");
            this.cmb_sysid.Name = "cmb_sysid";
            this.cmb_sysid.SelectedIndexChanged += new System.EventHandler(this.CMB_sysid_SelectedIndexChanged);
            this.cmb_sysid.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.cmb_sysid_Format);
            // 
            // ConnectionControl
            // 
            
            this.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.Controls.Add(this.cmb_sysid);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.cmb_Connection);
            this.Controls.Add(this.cmb_ConnectionType);
            this.Controls.Add(this.cmb_Baud);
            resources.ApplyResources(this, "$this");
            this.Name = "ConnectionControl";
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.ConnectionControl_MouseClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmb_Baud;
        private System.Windows.Forms.ComboBox cmb_ConnectionType;
        private System.Windows.Forms.ComboBox cmb_Connection;
        private System.Windows.Forms.LinkLabel linkLabel1;
        public System.Windows.Forms.ComboBox cmb_sysid;
    }
}
