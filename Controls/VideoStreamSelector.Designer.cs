namespace MissionPlanner.Controls
{
    partial class VideoStreamSelector
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
            this.cmb_detectedstreams = new System.Windows.Forms.ComboBox();
            this.but_launch = new MissionPlanner.Controls.MyButton();
            this.txt_gstreamraw = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cmb_detectedstreams
            // 
            this.cmb_detectedstreams.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_detectedstreams.FormattingEnabled = true;
            this.cmb_detectedstreams.Location = new System.Drawing.Point(115, 6);
            this.cmb_detectedstreams.Name = "cmb_detectedstreams";
            this.cmb_detectedstreams.Size = new System.Drawing.Size(303, 21);
            this.cmb_detectedstreams.TabIndex = 0;
            this.cmb_detectedstreams.SelectedIndexChanged += new System.EventHandler(this.cmb_detectedstreams_SelectedIndexChanged);
            // 
            // but_launch
            // 
            this.but_launch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.but_launch.Location = new System.Drawing.Point(343, 59);
            this.but_launch.Name = "but_launch";
            this.but_launch.Size = new System.Drawing.Size(75, 21);
            this.but_launch.TabIndex = 1;
            this.but_launch.Text = "Connect";
            this.but_launch.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_launch.UseVisualStyleBackColor = true;
            this.but_launch.Click += new System.EventHandler(this.but_launch_Click);
            // 
            // txt_gstreamraw
            // 
            this.txt_gstreamraw.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_gstreamraw.Location = new System.Drawing.Point(115, 33);
            this.txt_gstreamraw.Name = "txt_gstreamraw";
            this.txt_gstreamraw.Size = new System.Drawing.Size(303, 20);
            this.txt_gstreamraw.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Detected Streams";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(97, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "GStreamer Pipeline";
            // 
            // VideoStreamSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(430, 93);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_gstreamraw);
            this.Controls.Add(this.but_launch);
            this.Controls.Add(this.cmb_detectedstreams);
            this.Name = "VideoStreamSelector";
            this.Text = "VideoStreamSelector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmb_detectedstreams;
        private MyButton but_launch;
        private System.Windows.Forms.TextBox txt_gstreamraw;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}