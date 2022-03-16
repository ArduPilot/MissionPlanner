namespace OSDConfigurator.GUI
{
    partial class BitwiseSettingControl
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
            this.listBox = new System.Windows.Forms.CheckedListBox();
            this.tbValue = new System.Windows.Forms.TextBox();
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
            this.labelDescription.Location = new System.Drawing.Point(333, 12);
            this.labelDescription.MaximumSize = new System.Drawing.Size(400, 200);
            this.labelDescription.MinimumSize = new System.Drawing.Size(400, 20);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Padding = new System.Windows.Forms.Padding(3);
            this.labelDescription.Size = new System.Drawing.Size(400, 20);
            this.labelDescription.TabIndex = 4;
            // 
            // listBox
            // 
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(152, 35);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(152, 79);
            this.listBox.TabIndex = 6;
            // 
            // tbValue
            // 
            this.tbValue.Location = new System.Drawing.Point(152, 12);
            this.tbValue.Name = "tbValue";
            this.tbValue.Size = new System.Drawing.Size(152, 20);
            this.tbValue.TabIndex = 7;
            // 
            // BitwiseSettingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tbValue);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.labelDescription);
            this.Controls.Add(this.labelName);
            this.Name = "BitwiseSettingControl";
            this.Padding = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.Size = new System.Drawing.Size(769, 125);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.CheckedListBox listBox;
        private System.Windows.Forms.TextBox tbValue;
    }
}
