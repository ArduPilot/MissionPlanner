using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    partial class ProgressReporterDialogue
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressReporterDialogue));
            this.progressBar1 = new MissionPlanner.Controls.MyProgressBar();
            this.lblProgressMessage = new System.Windows.Forms.Label();
            this.btnCancel = new MissionPlanner.Controls.MyButton();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.btnClose = new MissionPlanner.Controls.MyButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.imgWarning = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.imgWarning)).BeginInit();
            this.SuspendLayout();
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.BackColor = System.Drawing.Color.Transparent;
            this.progressBar1.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(127)))), ((int)(((byte)(167)))), ((int)(((byte)(42)))));
            this.progressBar1.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(139)))), ((int)(((byte)(26)))));
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(171)))), ((int)(((byte)(112)))));
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar1.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(54)))), ((int)(((byte)(8)))));
            // 
            // lblProgressMessage
            // 
            resources.ApplyResources(this.lblProgressMessage, "lblProgressMessage");
            this.lblProgressMessage.Name = "lblProgressMessage";
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // btnClose
            // 
            resources.ApplyResources(this.btnClose, "btnClose");
            this.btnClose.Name = "btnClose";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btn_Close_Click);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // imgWarning
            // 
            resources.ApplyResources(this.imgWarning, "imgWarning");
            this.imgWarning.Image = global::MissionPlanner.Controls.Properties.Resources.iconWarning48;
            this.imgWarning.Name = "imgWarning";
            this.imgWarning.TabStop = false;
            // 
            // ProgressReporterDialogue
            // 
            resources.ApplyResources(this, "$this");
            
            this.ControlBox = false;
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.imgWarning);
            this.Controls.Add(this.lblProgressMessage);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProgressReporterDialogue";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.ProgressReporterDialogue_Load);
            ((System.ComponentModel.ISupportInitialize)(this.imgWarning)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyProgressBar progressBar1;
        private System.Windows.Forms.Label lblProgressMessage;
        public MyButton btnCancel;
        private PictureBox imgWarning;
        private LinkLabel linkLabel1;
        private MyButton btnClose;
        private Timer timer1;
    }
}