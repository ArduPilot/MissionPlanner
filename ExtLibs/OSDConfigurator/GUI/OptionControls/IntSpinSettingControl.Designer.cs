namespace OSDConfigurator.GUI
{
    partial class IntSpinSettingControl
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
            this.labelName = new System.Windows.Forms.Label();
            this.labelDescription = new System.Windows.Forms.Label();
            this.numBox = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numBox)).BeginInit();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Location = new System.Drawing.Point(16, 15);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(88, 13);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "SETTING NAME";
            // 
            // labelDescription
            // 
            this.labelDescription.AutoSize = true;
            this.labelDescription.Location = new System.Drawing.Point(333, 14);
            this.labelDescription.MaximumSize = new System.Drawing.Size(400, 200);
            this.labelDescription.MinimumSize = new System.Drawing.Size(400, 20);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Padding = new System.Windows.Forms.Padding(3);
            this.labelDescription.Size = new System.Drawing.Size(400, 20);
            this.labelDescription.TabIndex = 4;
            this.labelDescription.Text = "No description";
            // 
            // numBox
            // 
            this.numBox.Location = new System.Drawing.Point(153, 13);
            this.numBox.Name = "numBox";
            this.numBox.Size = new System.Drawing.Size(152, 20);
            this.numBox.TabIndex = 5;
            // 
            // IntSpinSettingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.numBox);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.labelName);
            this.Name = "IntSpinSettingControl";
            this.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.Size = new System.Drawing.Size(769, 42);
            ((System.ComponentModel.ISupportInitialize)(this.numBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.NumericUpDown numBox;
    }
}
