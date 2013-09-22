namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigSimplePidsV2
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
            this.TXT_info = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.TRK_Gain = new Controls.MyTrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.myLabel1 = new Controls.MyLabel();
            this.TRK_damp = new Controls.MyTrackBar();
            this.myLabel2 = new Controls.MyLabel();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TRK_Gain)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TRK_damp)).BeginInit();
            this.SuspendLayout();
            // 
            // TXT_info
            // 
            this.TXT_info.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_info.Location = new System.Drawing.Point(4, 226);
            this.TXT_info.Multiline = true;
            this.TXT_info.Name = "TXT_info";
            this.TXT_info.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.TXT_info.Size = new System.Drawing.Size(556, 82);
            this.TXT_info.TabIndex = 1;
            this.TXT_info.Text = "NOTE: using this interface may reset some off your custom pids.";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.myLabel1);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Location = new System.Drawing.Point(4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(556, 216);
            this.panel1.TabIndex = 2;
            // 
            // TRK_Gain
            // 
            this.TRK_Gain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TRK_Gain.LargeChange = 0.005F;
            this.TRK_Gain.Location = new System.Drawing.Point(6, 19);
            this.TRK_Gain.Maximum = 0.4F;
            this.TRK_Gain.Minimum = 0.08F;
            this.TRK_Gain.Name = "TRK_Gain";
            this.TRK_Gain.Size = new System.Drawing.Size(328, 45);
            this.TRK_Gain.SmallChange = 0.01F;
            this.TRK_Gain.TabIndex = 0;
            this.TRK_Gain.TickFrequency = 0.01F;
            this.TRK_Gain.Value = 0.15F;
            this.TRK_Gain.Scroll += new System.EventHandler(this.TRK_Gain_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.myLabel2);
            this.groupBox1.Controls.Add(this.TRK_Gain);
            this.groupBox1.Controls.Add(this.TRK_damp);
            this.groupBox1.Location = new System.Drawing.Point(28, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(338, 150);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Roll/Pitch Gain";
            // 
            // myLabel1
            // 
            this.myLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.myLabel1.Location = new System.Drawing.Point(28, 159);
            this.myLabel1.Name = "myLabel1";
            this.myLabel1.resize = false;
            this.myLabel1.Size = new System.Drawing.Size(510, 45);
            this.myLabel1.TabIndex = 5;
            this.myLabel1.Text = "In Development (use with caution)";
            // 
            // TRK_damp
            // 
            this.TRK_damp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TRK_damp.LargeChange = 0.001F;
            this.TRK_damp.Location = new System.Drawing.Point(6, 99);
            this.TRK_damp.Maximum = 0.01F;
            this.TRK_damp.Minimum = 0.001F;
            this.TRK_damp.Name = "TRK_damp";
            this.TRK_damp.Size = new System.Drawing.Size(328, 45);
            this.TRK_damp.SmallChange = 0.001F;
            this.TRK_damp.TabIndex = 1;
            this.TRK_damp.TickFrequency = 0.001F;
            this.TRK_damp.Value = 0.001F;
            this.TRK_damp.Scroll += new System.EventHandler(this.TRK_damp_Scroll);
            // 
            // myLabel2
            // 
            this.myLabel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.myLabel2.Location = new System.Drawing.Point(6, 70);
            this.myLabel2.Name = "myLabel2";
            this.myLabel2.resize = false;
            this.myLabel2.Size = new System.Drawing.Size(328, 23);
            this.myLabel2.TabIndex = 3;
            this.myLabel2.Text = "Roll/Pitch Damping";
            // 
            // ConfigSimplePidsV2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.TXT_info);
            this.Name = "ConfigSimplePidsV2";
            this.Size = new System.Drawing.Size(563, 311);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TRK_Gain)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TRK_damp)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TXT_info;
        private System.Windows.Forms.Panel panel1;
        private Controls.MyTrackBar TRK_Gain;
        private System.Windows.Forms.GroupBox groupBox1;
        private Controls.MyLabel myLabel1;
        private Controls.MyLabel myLabel2;
        private Controls.MyTrackBar TRK_damp;


    }
}
