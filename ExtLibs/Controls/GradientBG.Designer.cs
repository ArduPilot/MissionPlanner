namespace MissionPlanner.Controls
{
    partial class GradientBG
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
            this._Label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._Image)).BeginInit();
            this.SuspendLayout();
            // 
            // _Image
            // 
            this._Image.BackColor = System.Drawing.Color.Transparent;
            this._Image.Location = new System.Drawing.Point(35, 30);
            this._Image.Name = "_Image";
            this._Image.Size = new System.Drawing.Size(100, 50);
            this._Image.TabIndex = 0;
            this._Image.TabStop = false;
            // 
            // _Label
            // 
            this._Label.AutoSize = true;
            this._Label.BackColor = System.Drawing.Color.Transparent;
            this._Label.Location = new System.Drawing.Point(32, 14);
            this._Label.Name = "_Label";
            this._Label.Size = new System.Drawing.Size(35, 13);
            this._Label.TabIndex = 1;
            this._Label.Text = "label1";
            // 
            // GradientBG
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            
            this.Controls.Add(this._Label);
            this.Controls.Add(this._Image);
            this.Name = "GradientBG";
            this.Size = new System.Drawing.Size(410, 110);
            ((System.ComponentModel.ISupportInitialize)(this._Image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox _Image;
        private System.Windows.Forms.Label _Label;


    }
}
