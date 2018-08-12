namespace MissionPlanner.Controls
{
    partial class RadialGradientBG
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
            this._Image = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._Image)).BeginInit();
            this.SuspendLayout();
            // 
            // _Image
            // 
            this._Image.Location = new System.Drawing.Point(35, 30);
            this._Image.Name = "_Image";
            this._Image.Size = new System.Drawing.Size(100, 50);
            this._Image.TabIndex = 0;
            this._Image.TabStop = false;
            // 
            // RadialGradientBG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            
            this.Controls.Add(this._Image);
            this.Name = "RadialGradientBG";
            this.Size = new System.Drawing.Size(410, 110);
            ((System.ComponentModel.ISupportInitialize)(this._Image)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox _Image;


    }
}
