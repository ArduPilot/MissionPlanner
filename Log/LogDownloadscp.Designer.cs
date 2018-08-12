namespace MissionPlanner.Log
{
    partial class LogDownloadscp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogDownloadscp));
            this.TXT_seriallog = new System.Windows.Forms.TextBox();
            this.BUT_DLall = new MissionPlanner.Controls.MyButton();
            this.BUT_DLthese = new MissionPlanner.Controls.MyButton();
            this.BUT_clearlogs = new MissionPlanner.Controls.MyButton();
            this.CHK_logs = new System.Windows.Forms.CheckedListBox();
            this.BUT_redokml = new MissionPlanner.Controls.MyButton();
            this.BUT_firstperson = new MissionPlanner.Controls.MyButton();
            this.BUT_bintolog = new MissionPlanner.Controls.MyButton();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.LabelStatus = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.labelBytes = new System.Windows.Forms.Label();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // TXT_seriallog
            // 
            resources.ApplyResources(this.TXT_seriallog, "TXT_seriallog");
            this.TXT_seriallog.Name = "TXT_seriallog";
            this.tableLayoutPanel1.SetRowSpan(this.TXT_seriallog, 2);
            // 
            // BUT_DLall
            // 
            resources.ApplyResources(this.BUT_DLall, "BUT_DLall");
            this.BUT_DLall.Name = "BUT_DLall";
            this.BUT_DLall.UseVisualStyleBackColor = true;
            this.BUT_DLall.Click += new System.EventHandler(this.BUT_DLall_Click);
            // 
            // BUT_DLthese
            // 
            resources.ApplyResources(this.BUT_DLthese, "BUT_DLthese");
            this.BUT_DLthese.Name = "BUT_DLthese";
            this.toolTip1.SetToolTip(this.BUT_DLthese, resources.GetString("BUT_DLthese.ToolTip"));
            this.BUT_DLthese.UseVisualStyleBackColor = true;
            this.BUT_DLthese.Click += new System.EventHandler(this.BUT_DLthese_Click);
            // 
            // BUT_clearlogs
            // 
            resources.ApplyResources(this.BUT_clearlogs, "BUT_clearlogs");
            this.BUT_clearlogs.Name = "BUT_clearlogs";
            this.toolTip1.SetToolTip(this.BUT_clearlogs, resources.GetString("BUT_clearlogs.ToolTip"));
            this.BUT_clearlogs.UseVisualStyleBackColor = true;
            this.BUT_clearlogs.Click += new System.EventHandler(this.BUT_clearlogs_Click);
            // 
            // CHK_logs
            // 
            resources.ApplyResources(this.CHK_logs, "CHK_logs");
            this.CHK_logs.CheckOnClick = true;
            this.CHK_logs.FormattingEnabled = true;
            this.CHK_logs.Name = "CHK_logs";
            // 
            // BUT_redokml
            // 
            resources.ApplyResources(this.BUT_redokml, "BUT_redokml");
            this.BUT_redokml.Name = "BUT_redokml";
            this.toolTip1.SetToolTip(this.BUT_redokml, resources.GetString("BUT_redokml.ToolTip"));
            this.BUT_redokml.UseVisualStyleBackColor = true;
            this.BUT_redokml.Click += new System.EventHandler(this.BUT_redokml_Click);
            // 
            // BUT_firstperson
            // 
            resources.ApplyResources(this.BUT_firstperson, "BUT_firstperson");
            this.BUT_firstperson.Name = "BUT_firstperson";
            this.toolTip1.SetToolTip(this.BUT_firstperson, resources.GetString("BUT_firstperson.ToolTip"));
            this.BUT_firstperson.UseVisualStyleBackColor = true;
            this.BUT_firstperson.Click += new System.EventHandler(this.BUT_firstperson_Click);
            // 
            // BUT_bintolog
            // 
            resources.ApplyResources(this.BUT_bintolog, "BUT_bintolog");
            this.BUT_bintolog.Name = "BUT_bintolog";
            this.toolTip1.SetToolTip(this.BUT_bintolog, resources.GetString("BUT_bintolog.ToolTip"));
            this.BUT_bintolog.UseVisualStyleBackColor = true;
            this.BUT_bintolog.Click += new System.EventHandler(this.BUT_bintolog_Click);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.TXT_seriallog, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.CHK_logs, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.LabelStatus, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // LabelStatus
            // 
            resources.ApplyResources(this.LabelStatus, "LabelStatus");
            this.LabelStatus.ForeColor = System.Drawing.Color.SeaGreen;
            this.LabelStatus.Name = "LabelStatus";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // tableLayoutPanel2
            // 
            resources.ApplyResources(this.tableLayoutPanel2, "tableLayoutPanel2");
            this.tableLayoutPanel2.Controls.Add(this.progressBar1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.labelBytes, 1, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            // 
            // progressBar1
            // 
            resources.ApplyResources(this.progressBar1, "progressBar1");
            this.progressBar1.Name = "progressBar1";
            // 
            // labelBytes
            // 
            resources.ApplyResources(this.labelBytes, "labelBytes");
            this.labelBytes.Name = "labelBytes";
            // 
            // tableLayoutPanel3
            // 
            resources.ApplyResources(this.tableLayoutPanel3, "tableLayoutPanel3");
            this.tableLayoutPanel3.Controls.Add(this.BUT_DLall, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.BUT_clearlogs, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.BUT_firstperson, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.BUT_DLthese, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.BUT_bintolog, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.BUT_redokml, 1, 1);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 400;
            this.toolTip1.ShowAlways = true;
            // 
            // LogDownloadscp
            // 
            
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "LogDownloadscp";
            this.Load += new System.EventHandler(this.Log_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MyButton BUT_DLall;
        private Controls.MyButton BUT_DLthese;
        private Controls.MyButton BUT_clearlogs;
        private System.Windows.Forms.CheckedListBox CHK_logs;
        private Controls.MyButton BUT_redokml;
        private System.Windows.Forms.TextBox TXT_seriallog;
        private Controls.MyButton BUT_firstperson;
        private Controls.MyButton BUT_bintolog;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label LabelStatus;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label labelBytes;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}