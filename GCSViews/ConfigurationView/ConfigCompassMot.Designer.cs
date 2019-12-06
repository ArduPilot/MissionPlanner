namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigCompassMot
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
            this.components = new System.ComponentModel.Container();
            this.BUT_compassmot = new MissionPlanner.Controls.MyButton();
            this.txt_status = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lbl_start = new System.Windows.Forms.Label();
            this.lbl_finish = new System.Windows.Forms.Label();
            this.lbl_status = new System.Windows.Forms.Label();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // BUT_compassmot
            // 
            this.BUT_compassmot.Location = new System.Drawing.Point(193, 9);
            this.BUT_compassmot.Name = "BUT_compassmot";
            this.BUT_compassmot.Size = new System.Drawing.Size(75, 23);
            this.BUT_compassmot.TabIndex = 0;
            this.BUT_compassmot.Text = Strings.Start;
            this.BUT_compassmot.UseVisualStyleBackColor = true;
            this.BUT_compassmot.Click += new System.EventHandler(this.BUT_compassmot_Click);
            // 
            // txt_status
            // 
            this.txt_status.Location = new System.Drawing.Point(55, 38);
            this.txt_status.Multiline = true;
            this.txt_status.Name = "txt_status";
            this.txt_status.Size = new System.Drawing.Size(353, 111);
            this.txt_status.TabIndex = 1;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lbl_start
            // 
            this.lbl_start.AutoSize = true;
            this.lbl_start.Location = new System.Drawing.Point(274, 14);
            this.lbl_start.Name = "lbl_start";
            this.lbl_start.Size = new System.Drawing.Size(29, 13);
            this.lbl_start.TabIndex = 2;
            this.lbl_start.Text = Strings.Start;
            this.lbl_start.Visible = false;
            // 
            // lbl_finish
            // 
            this.lbl_finish.AutoSize = true;
            this.lbl_finish.Location = new System.Drawing.Point(309, 14);
            this.lbl_finish.Name = "lbl_finish";
            this.lbl_finish.Size = new System.Drawing.Size(34, 13);
            this.lbl_finish.TabIndex = 3;
            this.lbl_finish.Text = "Finish";
            this.lbl_finish.Visible = false;
            // 
            // lbl_status
            // 
            this.lbl_status.AutoSize = true;
            this.lbl_status.Location = new System.Drawing.Point(414, 41);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(132, 13);
            this.lbl_status.TabIndex = 4;
            this.lbl_status.Text = "Compass Motor Calibration";
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zedGraphControl1.Location = new System.Drawing.Point(3, 168);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(640, 229);
            this.zedGraphControl1.TabIndex = 5;
            // 
            // ConfigCompassMot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            
            this.Controls.Add(this.zedGraphControl1);
            this.Controls.Add(this.lbl_status);
            this.Controls.Add(this.lbl_finish);
            this.Controls.Add(this.lbl_start);
            this.Controls.Add(this.txt_status);
            this.Controls.Add(this.BUT_compassmot);
            this.Name = "ConfigCompassMot";
            this.Size = new System.Drawing.Size(647, 400);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.MyButton BUT_compassmot;
        private System.Windows.Forms.TextBox txt_status;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lbl_start;
        private System.Windows.Forms.Label lbl_finish;
        private System.Windows.Forms.Label lbl_status;
        private ZedGraph.ZedGraphControl zedGraphControl1;
    }
}
