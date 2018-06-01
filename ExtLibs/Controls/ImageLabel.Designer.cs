namespace MissionPlanner.Controls
{
    partial class ImageLabel
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
            this.PictureBox = new System.Windows.Forms.PictureBox();
            this.Label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // PictureBox
            // 
            this.PictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PictureBox.Location = new System.Drawing.Point(0, 0);
            this.PictureBox.Name = "PictureBox";
            this.PictureBox.Size = new System.Drawing.Size(170, 157);
            this.PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.PictureBox.TabIndex = 0;
            this.PictureBox.TabStop = false;
            this.PictureBox.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // Label
            // 
            this.Label.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.Label.Location = new System.Drawing.Point(0, 157);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(170, 13);
            this.Label.TabIndex = 1;
            this.Label.Text = "";
            this.Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ImageLabel
            // 
            this.Controls.Add(this.PictureBox);
            this.Controls.Add(this.Label);
            this.Name = "ImageLabel";
            this.Size = new System.Drawing.Size(170, 170);
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.PictureBox PictureBox;
        public System.Windows.Forms.Label Label;

    }
}
