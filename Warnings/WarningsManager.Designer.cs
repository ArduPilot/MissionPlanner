namespace MissionPlanner.Warnings
{
    partial class WarningsManager
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.BUT_Add = new System.Windows.Forms.Button();
            this.BUT_save = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Location = new System.Drawing.Point(4, 40);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(600, 188);
            this.panel1.TabIndex = 0;
            // 
            // BUT_Add
            // 
            this.BUT_Add.Location = new System.Drawing.Point(13, 13);
            this.BUT_Add.Name = "BUT_Add";
            this.BUT_Add.Size = new System.Drawing.Size(75, 23);
            this.BUT_Add.TabIndex = 1;
            this.BUT_Add.Text = "+";
            this.BUT_Add.UseVisualStyleBackColor = true;
            this.BUT_Add.Click += new System.EventHandler(this.BUT_Add_Click);
            // 
            // BUT_save
            // 
            this.BUT_save.Location = new System.Drawing.Point(94, 13);
            this.BUT_save.Name = "BUT_save";
            this.BUT_save.Size = new System.Drawing.Size(75, 23);
            this.BUT_save.TabIndex = 2;
            this.BUT_save.Text = "Save";
            this.BUT_save.UseVisualStyleBackColor = true;
            this.BUT_save.Click += new System.EventHandler(this.BUT_save_Click);
            // 
            // WarningsManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(614, 238);
            this.Controls.Add(this.BUT_save);
            this.Controls.Add(this.BUT_Add);
            this.Controls.Add(this.panel1);
            this.Name = "WarningsManager";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button BUT_Add;
        private System.Windows.Forms.Button BUT_save;
    }
}
