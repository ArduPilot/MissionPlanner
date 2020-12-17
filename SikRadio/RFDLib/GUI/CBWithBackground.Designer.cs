namespace RFDLib.GUI
{
    partial class CBWithBackground
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
            this.cmb = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // cmb
            // 
            this.cmb.FormattingEnabled = true;
            this.cmb.Location = new System.Drawing.Point(0, 0);
            this.cmb.Name = "cmb";
            this.cmb.Size = new System.Drawing.Size(467, 21);
            this.cmb.TabIndex = 0;
            // 
            // CBWithBackground
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.cmb);
            this.Name = "CBWithBackground";
            this.Size = new System.Drawing.Size(467, 76);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox cmb;
    }
}
