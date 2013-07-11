﻿namespace ArdupilotMega.Log
{
    partial class Log
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Log));
            this.TXT_seriallog = new System.Windows.Forms.TextBox();
            this.BUT_DLall = new ArdupilotMega.Controls.MyButton();
            this.BUT_DLthese = new ArdupilotMega.Controls.MyButton();
            this.BUT_clearlogs = new ArdupilotMega.Controls.MyButton();
            this.CHK_logs = new System.Windows.Forms.CheckedListBox();
            this.TXT_status = new System.Windows.Forms.TextBox();
            this.BUT_redokml = new ArdupilotMega.Controls.MyButton();
            this.BUT_firstperson = new ArdupilotMega.Controls.MyButton();
            this.BUT_dumpdf = new ArdupilotMega.Controls.MyButton();
            this.BUT_bintolog = new ArdupilotMega.Controls.MyButton();
            this.CHK_arducopter = new System.Windows.Forms.RadioButton();
            this.CHK_arduplane = new System.Windows.Forms.RadioButton();
            this.CHK_ardurover = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TXT_seriallog
            // 
            resources.ApplyResources(this.TXT_seriallog, "TXT_seriallog");
            this.TXT_seriallog.Name = "TXT_seriallog";
            // 
            // BUT_DLall
            // 
            this.BUT_DLall.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_DLall.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.BUT_DLall, "BUT_DLall");
            this.BUT_DLall.Name = "BUT_DLall";
            this.BUT_DLall.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_DLall.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_DLall.UseVisualStyleBackColor = true;
            this.BUT_DLall.Click += new System.EventHandler(this.BUT_DLall_Click);
            // 
            // BUT_DLthese
            // 
            this.BUT_DLthese.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_DLthese.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.BUT_DLthese, "BUT_DLthese");
            this.BUT_DLthese.Name = "BUT_DLthese";
            this.BUT_DLthese.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_DLthese.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_DLthese.UseVisualStyleBackColor = true;
            this.BUT_DLthese.Click += new System.EventHandler(this.BUT_DLthese_Click);
            // 
            // BUT_clearlogs
            // 
            this.BUT_clearlogs.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_clearlogs.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.BUT_clearlogs, "BUT_clearlogs");
            this.BUT_clearlogs.Name = "BUT_clearlogs";
            this.BUT_clearlogs.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_clearlogs.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_clearlogs.UseVisualStyleBackColor = true;
            this.BUT_clearlogs.Click += new System.EventHandler(this.BUT_clearlogs_Click);
            // 
            // CHK_logs
            // 
            this.CHK_logs.CheckOnClick = true;
            this.CHK_logs.FormattingEnabled = true;
            resources.ApplyResources(this.CHK_logs, "CHK_logs");
            this.CHK_logs.Name = "CHK_logs";
            this.CHK_logs.Click += new System.EventHandler(this.CHK_logs_Click);
            // 
            // TXT_status
            // 
            resources.ApplyResources(this.TXT_status, "TXT_status");
            this.TXT_status.ForeColor = System.Drawing.Color.Red;
            this.TXT_status.Name = "TXT_status";
            // 
            // BUT_redokml
            // 
            this.BUT_redokml.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_redokml.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.BUT_redokml, "BUT_redokml");
            this.BUT_redokml.Name = "BUT_redokml";
            this.BUT_redokml.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_redokml.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_redokml.UseVisualStyleBackColor = true;
            this.BUT_redokml.Click += new System.EventHandler(this.BUT_redokml_Click);
            // 
            // BUT_firstperson
            // 
            this.BUT_firstperson.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_firstperson.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.BUT_firstperson, "BUT_firstperson");
            this.BUT_firstperson.Name = "BUT_firstperson";
            this.BUT_firstperson.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_firstperson.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_firstperson.UseVisualStyleBackColor = true;
            this.BUT_firstperson.Click += new System.EventHandler(this.BUT_firstperson_Click);
            // 
            // BUT_dumpdf
            // 
            this.BUT_dumpdf.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_dumpdf.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.BUT_dumpdf, "BUT_dumpdf");
            this.BUT_dumpdf.Name = "BUT_dumpdf";
            this.BUT_dumpdf.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_dumpdf.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_dumpdf.UseVisualStyleBackColor = true;
            this.BUT_dumpdf.Click += new System.EventHandler(this.BUT_dumpdf_Click);
            // 
            // BUT_bintolog
            // 
            this.BUT_bintolog.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_bintolog.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            resources.ApplyResources(this.BUT_bintolog, "BUT_bintolog");
            this.BUT_bintolog.Name = "BUT_bintolog";
            this.BUT_bintolog.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_bintolog.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_bintolog.UseVisualStyleBackColor = true;
            this.BUT_bintolog.Click += new System.EventHandler(this.BUT_bintolog_Click);
            // 
            // CHK_arducopter
            // 
            resources.ApplyResources(this.CHK_arducopter, "CHK_arducopter");
            this.CHK_arducopter.Checked = true;
            this.CHK_arducopter.Name = "CHK_arducopter";
            this.CHK_arducopter.TabStop = true;
            this.CHK_arducopter.UseVisualStyleBackColor = true;
            // 
            // CHK_arduplane
            // 
            resources.ApplyResources(this.CHK_arduplane, "CHK_arduplane");
            this.CHK_arduplane.Name = "CHK_arduplane";
            this.CHK_arduplane.UseVisualStyleBackColor = true;
            // 
            // CHK_ardurover
            // 
            resources.ApplyResources(this.CHK_ardurover, "CHK_ardurover");
            this.CHK_ardurover.Name = "CHK_ardurover";
            this.CHK_ardurover.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // Log
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CHK_ardurover);
            this.Controls.Add(this.CHK_arduplane);
            this.Controls.Add(this.CHK_arducopter);
            this.Controls.Add(this.BUT_bintolog);
            this.Controls.Add(this.BUT_dumpdf);
            this.Controls.Add(this.BUT_firstperson);
            this.Controls.Add(this.BUT_redokml);
            this.Controls.Add(this.TXT_status);
            this.Controls.Add(this.CHK_logs);
            this.Controls.Add(this.BUT_clearlogs);
            this.Controls.Add(this.BUT_DLthese);
            this.Controls.Add(this.BUT_DLall);
            this.Controls.Add(this.TXT_seriallog);
            this.Name = "Log";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Log_FormClosing);
            this.Load += new System.EventHandler(this.Log_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ArdupilotMega.Controls.MyButton BUT_DLall;
        private ArdupilotMega.Controls.MyButton BUT_DLthese;
        private ArdupilotMega.Controls.MyButton BUT_clearlogs;
        private System.Windows.Forms.CheckedListBox CHK_logs;
        private System.Windows.Forms.TextBox TXT_status;
        private ArdupilotMega.Controls.MyButton BUT_redokml;
        private System.Windows.Forms.TextBox TXT_seriallog;
        private ArdupilotMega.Controls.MyButton BUT_firstperson;
        private Controls.MyButton BUT_dumpdf;
        private Controls.MyButton BUT_bintolog;
        private System.Windows.Forms.RadioButton CHK_arducopter;
        private System.Windows.Forms.RadioButton CHK_arduplane;
        private System.Windows.Forms.RadioButton CHK_ardurover;
        private System.Windows.Forms.Label label1;
    }
}