namespace AltitudeAngelWings.Plugin
{
    internal partial class WaitPanel
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

        private void InitializeComponent()
        {
            this._panel = new System.Windows.Forms.Panel();
            this._btnCancel = new MissionPlanner.Controls.MyButton();
            this._lblOperation = new System.Windows.Forms.Label();
            this._picLogo = new System.Windows.Forms.PictureBox();
            this._panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._picLogo)).BeginInit();
            this.SuspendLayout();
            // 
            // _panel
            // 
            this._panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._panel.Controls.Add(this._btnCancel);
            this._panel.Controls.Add(this._lblOperation);
            this._panel.Controls.Add(this._picLogo);
            this._panel.Location = new System.Drawing.Point(3, 3);
            this._panel.MaximumSize = new System.Drawing.Size(300, 200);
            this._panel.MinimumSize = new System.Drawing.Size(100, 100);
            this._panel.Name = "_panel";
            this._panel.Size = new System.Drawing.Size(152, 187);
            this._panel.TabIndex = 6;
            // 
            // _btnCancel
            // 
            this._btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._btnCancel.Location = new System.Drawing.Point(6, 161);
            this._btnCancel.Name = "_btnCancel";
            this._btnCancel.Size = new System.Drawing.Size(143, 23);
            this._btnCancel.TabIndex = 8;
            this._btnCancel.Text = "Cancel";
            this._btnCancel.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this._btnCancel.UseVisualStyleBackColor = true;
            this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
            // 
            // _lblOperation
            // 
            this._lblOperation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._lblOperation.Location = new System.Drawing.Point(3, 131);
            this._lblOperation.Name = "_lblOperation";
            this._lblOperation.Size = new System.Drawing.Size(146, 27);
            this._lblOperation.TabIndex = 7;
            this._lblOperation.Text = "operation";
            // 
            // _picLogo
            // 
            this._picLogo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._picLogo.Image = global::AltitudeAngelWings.Plugin.Properties.Resources.AAAnimatedLogoWhite;
            this._picLogo.Location = new System.Drawing.Point(3, 3);
            this._picLogo.Name = "_picLogo";
            this._picLogo.Size = new System.Drawing.Size(146, 125);
            this._picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._picLogo.TabIndex = 6;
            this._picLogo.TabStop = false;
            // 
            // WaitPanel
            // 
            this.AutoSize = true;
            this.Controls.Add(this._panel);
            this.Name = "WaitPanel";
            this.Size = new System.Drawing.Size(159, 194);
            this._panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._picLogo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel _panel;
        private MissionPlanner.Controls.MyButton _btnCancel;
        private System.Windows.Forms.Label _lblOperation;
        private System.Windows.Forms.PictureBox _picLogo;
    }
}