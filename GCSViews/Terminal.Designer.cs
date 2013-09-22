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
            this.BUTsetupshow = new Controls.MyButton();
            this.BUTradiosetup = new Controls.MyButton();
            this.BUTtests = new Controls.MyButton();
            this.Logs = new Controls.MyButton();
            this.BUT_logbrowse = new Controls.MyButton();
            this.BUT_ConnectAPM = new Controls.MyButton();
            this.BUT_disconnect = new Controls.MyButton();
            this.CMB_boardtype = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // TXT_terminal
            // 
            resources.ApplyResources(this.TXT_terminal, "TXT_terminal");
            this.TXT_terminal.BackColor = System.Drawing.Color.Black;
            this.TXT_terminal.ForeColor = System.Drawing.Color.White;
            this.TXT_terminal.Name = "TXT_terminal";
            this.TXT_terminal.Click += new System.EventHandler(this.TXT_terminal_Click);
            this.TXT_terminal.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TXT_terminal_KeyDown);
            this.TXT_terminal.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TXT_terminal_KeyPress);
            // 
            // BUTsetupshow
            // 
            this.BUTsetupshow.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUTsetupshow.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.BUTsetupshow, "BUTsetupshow");
            this.BUTsetupshow.Name = "BUTsetupshow";
            this.BUTsetupshow.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUTsetupshow.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUTsetupshow.UseVisualStyleBackColor = true;
            this.BUTsetupshow.Click += new System.EventHandler(this.BUTsetupshow_Click);
            // 
            // BUTradiosetup
            // 
            this.BUTradiosetup.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUTradiosetup.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.BUTradiosetup, "BUTradiosetup");
            this.BUTradiosetup.Name = "BUTradiosetup";
            this.BUTradiosetup.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUTradiosetup.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUTradiosetup.UseVisualStyleBackColor = true;
            this.BUTradiosetup.Click += new System.EventHandler(this.BUTradiosetup_Click);
            // 
            // BUTtests
            // 
            this.BUTtests.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUTtests.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.BUTtests, "BUTtests");
            this.BUTtests.Name = "BUTtests";
            this.BUTtests.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUTtests.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUTtests.UseVisualStyleBackColor = true;
            this.BUTtests.Click += new System.EventHandler(this.BUTtests_Click);
            // 
            // Logs
            // 
            this.Logs.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.Logs.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.Logs, "Logs");
            this.Logs.Name = "Logs";
            this.Logs.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.Logs.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.Logs.UseVisualStyleBackColor = true;
            this.Logs.Click += new System.EventHandler(this.Logs_Click);
            // 
            // BUT_logbrowse
            // 
            this.BUT_logbrowse.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_logbrowse.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.BUT_logbrowse, "BUT_logbrowse");
            this.BUT_logbrowse.Name = "BUT_logbrowse";
            this.BUT_logbrowse.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_logbrowse.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_logbrowse.UseVisualStyleBackColor = true;
            this.BUT_logbrowse.Click += new System.EventHandler(this.BUT_logbrowse_Click);
            // 
            // BUT_ConnectAPM
            // 
            this.BUT_ConnectAPM.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_ConnectAPM.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.BUT_ConnectAPM, "BUT_ConnectAPM");
            this.BUT_ConnectAPM.Name = "BUT_ConnectAPM";
            this.BUT_ConnectAPM.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_ConnectAPM.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_ConnectAPM.UseVisualStyleBackColor = true;
            this.BUT_ConnectAPM.Click += new System.EventHandler(this.BUT_RebootAPM_Click);
            // 
            // BUT_disconnect
            // 
            resources.ApplyResources(this.BUT_disconnect, "BUT_disconnect");
            this.BUT_disconnect.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_disconnect.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.BUT_disconnect.Name = "BUT_disconnect";
            this.BUT_disconnect.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_disconnect.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_disconnect.UseVisualStyleBackColor = true;
            this.BUT_disconnect.Click += new System.EventHandler(this.BUT_disconnect_Click);
            // 
            // CMB_boardtype
            // 
            this.CMB_boardtype.FormattingEnabled = true;
            this.CMB_boardtype.Items.AddRange(new object[] {
            resources.GetString("CMB_boardtype.Items"),
            resources.GetString("CMB_boardtype.Items1")});
            resources.ApplyResources(this.CMB_boardtype, "CMB_boardtype");
            this.CMB_boardtype.Name = "CMB_boardtype";
            // 
            // Terminal
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
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
