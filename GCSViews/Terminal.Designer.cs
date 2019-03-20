namespace MissionPlanner.GCSViews
{
    partial class Terminal
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Terminal));
            this.TXT_terminal = new System.Windows.Forms.RichTextBox();
            this.BUTsetupshow = new MissionPlanner.Controls.MyButton();
            this.BUTradiosetup = new MissionPlanner.Controls.MyButton();
            this.BUTtests = new MissionPlanner.Controls.MyButton();
            this.Logs = new MissionPlanner.Controls.MyButton();
            this.BUT_logbrowse = new MissionPlanner.Controls.MyButton();
            this.BUT_ConnectAPM = new MissionPlanner.Controls.MyButton();
            this.BUT_disconnect = new MissionPlanner.Controls.MyButton();
            this.CMB_boardtype = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // TXT_terminal
            // 
            resources.ApplyResources(this.TXT_terminal, "TXT_terminal");
            this.TXT_terminal.AutoWordSelection = true;
            this.TXT_terminal.BackColor = System.Drawing.Color.Black;
            this.TXT_terminal.ForeColor = System.Drawing.Color.White;
            this.TXT_terminal.Name = "TXT_terminal";
            this.TXT_terminal.Click += new System.EventHandler(this.TXT_terminal_Click);
            this.TXT_terminal.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TXT_terminal_KeyDown);
            this.TXT_terminal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TXT_terminal_KeyPress);
            // 
            // BUTsetupshow
            // 
            resources.ApplyResources(this.BUTsetupshow, "BUTsetupshow");
            this.BUTsetupshow.Name = "BUTsetupshow";
            this.BUTsetupshow.UseVisualStyleBackColor = true;
            this.BUTsetupshow.Click += new System.EventHandler(this.BUTsetupshow_Click);
            // 
            // BUTradiosetup
            // 
            resources.ApplyResources(this.BUTradiosetup, "BUTradiosetup");
            this.BUTradiosetup.Name = "BUTradiosetup";
            this.BUTradiosetup.UseVisualStyleBackColor = true;
            this.BUTradiosetup.Click += new System.EventHandler(this.BUTradiosetup_Click);
            // 
            // BUTtests
            // 
            resources.ApplyResources(this.BUTtests, "BUTtests");
            this.BUTtests.Name = "BUTtests";
            this.BUTtests.UseVisualStyleBackColor = true;
            this.BUTtests.Click += new System.EventHandler(this.BUTtests_Click);
            // 
            // Logs
            // 
            resources.ApplyResources(this.Logs, "Logs");
            this.Logs.Name = "Logs";
            this.Logs.UseVisualStyleBackColor = true;
            this.Logs.Click += new System.EventHandler(this.Logs_Click);
            // 
            // BUT_logbrowse
            // 
            resources.ApplyResources(this.BUT_logbrowse, "BUT_logbrowse");
            this.BUT_logbrowse.Name = "BUT_logbrowse";
            this.BUT_logbrowse.UseVisualStyleBackColor = true;
            this.BUT_logbrowse.Click += new System.EventHandler(this.BUT_logbrowse_Click);
            // 
            // BUT_ConnectAPM
            // 
            resources.ApplyResources(this.BUT_ConnectAPM, "BUT_ConnectAPM");
            this.BUT_ConnectAPM.Name = "BUT_ConnectAPM";
            this.BUT_ConnectAPM.UseVisualStyleBackColor = true;
            this.BUT_ConnectAPM.Click += new System.EventHandler(this.BUT_RebootAPM_Click);
            // 
            // BUT_disconnect
            // 
            resources.ApplyResources(this.BUT_disconnect, "BUT_disconnect");
            this.BUT_disconnect.Name = "BUT_disconnect";
            this.BUT_disconnect.UseVisualStyleBackColor = true;
            this.BUT_disconnect.Click += new System.EventHandler(this.BUT_disconnect_Click);
            // 
            // CMB_boardtype
            // 
            this.CMB_boardtype.FormattingEnabled = true;
            this.CMB_boardtype.Items.AddRange(new object[] {
            resources.GetString("CMB_boardtype.Items"),
            resources.GetString("CMB_boardtype.Items1"),
            resources.GetString("CMB_boardtype.Items2"),
            resources.GetString("CMB_boardtype.Items3"),
            resources.GetString("CMB_boardtype.Items4")});
            resources.ApplyResources(this.CMB_boardtype, "CMB_boardtype");
            this.CMB_boardtype.Name = "CMB_boardtype";
            // 
            // Terminal
            // 
            
            this.Controls.Add(this.CMB_boardtype);
            this.Controls.Add(this.BUT_disconnect);
            this.Controls.Add(this.BUT_ConnectAPM);
            this.Controls.Add(this.BUT_logbrowse);
            this.Controls.Add(this.Logs);
            this.Controls.Add(this.BUTtests);
            this.Controls.Add(this.BUTradiosetup);
            this.Controls.Add(this.BUTsetupshow);
            this.Controls.Add(this.TXT_terminal);
            this.Name = "Terminal";
            resources.ApplyResources(this, "$this");
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Terminal_FormClosing);
            this.Load += new System.EventHandler(this.Terminal_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox TXT_terminal;
        private Controls.MyButton BUTsetupshow;
        private Controls.MyButton BUTradiosetup;
        private Controls.MyButton BUTtests;
        private Controls.MyButton Logs;
        private Controls.MyButton BUT_logbrowse;
        private Controls.MyButton BUT_ConnectAPM;
        private Controls.MyButton BUT_disconnect;
        private System.Windows.Forms.ComboBox CMB_boardtype;
    }
}
