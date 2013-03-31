namespace ArdupilotMega.Controls
{
    partial class Firmware_Board
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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.imageLabelapm1 = new ArdupilotMega.Controls.ImageLabel();
            this.imageLabelapm2 = new ArdupilotMega.Controls.ImageLabel();
            this.imageLabelapm2_5 = new ArdupilotMega.Controls.ImageLabel();
            this.imageLabelpx4 = new ArdupilotMega.Controls.ImageLabel();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel1.Controls.Add(this.imageLabelapm1);
            this.flowLayoutPanel1.Controls.Add(this.imageLabelapm2);
            this.flowLayoutPanel1.Controls.Add(this.imageLabelapm2_5);
            this.flowLayoutPanel1.Controls.Add(this.imageLabelpx4);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 12);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(729, 207);
            this.flowLayoutPanel1.TabIndex = 4;
            // 
            // imageLabelapm1
            // 
            this.imageLabelapm1.Image = global::ArdupilotMega.Properties.Resources.apm1;
            this.imageLabelapm1.Location = new System.Drawing.Point(3, 3);
            this.imageLabelapm1.Name = "imageLabelapm1";
            this.imageLabelapm1.Size = new System.Drawing.Size(170, 170);
            this.imageLabelapm1.TabIndex = 4;
            this.imageLabelapm1.Click += new System.EventHandler(this.imageLabelapm1_Click);
            // 
            // imageLabelapm2
            // 
            this.imageLabelapm2.Image = global::ArdupilotMega.Properties.Resources.apm2;
            this.imageLabelapm2.Location = new System.Drawing.Point(179, 3);
            this.imageLabelapm2.Name = "imageLabelapm2";
            this.imageLabelapm2.Size = new System.Drawing.Size(170, 170);
            this.imageLabelapm2.TabIndex = 5;
            this.imageLabelapm2.Click += new System.EventHandler(this.imageLabelapm2_Click);
            // 
            // imageLabelapm2_5
            // 
            this.imageLabelapm2_5.Image = global::ArdupilotMega.Properties.Resources.apm2_5;
            this.imageLabelapm2_5.Location = new System.Drawing.Point(355, 3);
            this.imageLabelapm2_5.Name = "imageLabelapm2_5";
            this.imageLabelapm2_5.Size = new System.Drawing.Size(170, 170);
            this.imageLabelapm2_5.TabIndex = 6;
            this.imageLabelapm2_5.Click += new System.EventHandler(this.imageLabelapm2_5_Click);
            // 
            // imageLabelpx4
            // 
            this.imageLabelpx4.Image = global::ArdupilotMega.Properties.Resources.px4;
            this.imageLabelpx4.Location = new System.Drawing.Point(531, 3);
            this.imageLabelpx4.Name = "imageLabelpx4";
            this.imageLabelpx4.Size = new System.Drawing.Size(170, 170);
            this.imageLabelpx4.TabIndex = 7;
            this.imageLabelpx4.Click += new System.EventHandler(this.imageLabelpx4_Click);
            // 
            // Firmware_Board
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 232);
            this.Controls.Add(this.flowLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Firmware_Board";
            this.Text = "Board Type";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ImageLabel imageLabelapm1;
        private ImageLabel imageLabelapm2;
        private ImageLabel imageLabelapm2_5;
        private ImageLabel imageLabelpx4;
    }
}