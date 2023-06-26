namespace OSDConfigurator.GUI.Osd56ItemsSetup
{
    partial class ItemControl
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
            this.cbParamName = new System.Windows.Forms.ComboBox();
            this.cbType = new System.Windows.Forms.ComboBox();
            this.tbMin = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbMax = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbIncrement = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // cbParamName
            // 
            this.cbParamName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbParamName.FormattingEnabled = true;
            this.cbParamName.Location = new System.Drawing.Point(3, 3);
            this.cbParamName.Name = "cbParamName";
            this.cbParamName.Size = new System.Drawing.Size(184, 21);
            this.cbParamName.TabIndex = 0;
            // 
            // cbType
            // 
            this.cbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbType.FormattingEnabled = true;
            this.cbType.Location = new System.Drawing.Point(193, 3);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(184, 21);
            this.cbType.TabIndex = 1;
            this.cbType.SelectedIndexChanged += new System.EventHandler(this.cbType_SelectedIndexChanged);
            // 
            // tbMin
            // 
            this.tbMin.Location = new System.Drawing.Point(413, 3);
            this.tbMin.Name = "tbMin";
            this.tbMin.Size = new System.Drawing.Size(83, 20);
            this.tbMin.TabIndex = 2;
            this.tbMin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(383, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Min:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(502, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Max:";
            // 
            // tbMax
            // 
            this.tbMax.Location = new System.Drawing.Point(535, 4);
            this.tbMax.Name = "tbMax";
            this.tbMax.Size = new System.Drawing.Size(83, 20);
            this.tbMax.TabIndex = 4;
            this.tbMax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(629, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Increment:";
            // 
            // tbIncrement
            // 
            this.tbIncrement.Location = new System.Drawing.Point(689, 4);
            this.tbIncrement.Name = "tbIncrement";
            this.tbIncrement.Size = new System.Drawing.Size(83, 20);
            this.tbIncrement.TabIndex = 6;
            this.tbIncrement.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxKeyPress);
            // 
            // ItemControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbIncrement);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbMax);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbMin);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.cbParamName);
            this.Name = "ItemControl";
            this.Size = new System.Drawing.Size(775, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cbParamName;
        private System.Windows.Forms.ComboBox cbType;
        private System.Windows.Forms.TextBox tbMin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbMax;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbIncrement;
    }
}
