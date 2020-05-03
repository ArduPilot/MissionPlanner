namespace ExtGuided
{
    partial class FilePick
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
            this.txt_File = new System.Windows.Forms.TextBox();
            this.but_browse = new MissionPlanner.Controls.MyButton();
            this.SuspendLayout();
            // 
            // txt_File
            // 
            this.txt_File.Location = new System.Drawing.Point(12, 12);
            this.txt_File.Name = "txt_File";
            this.txt_File.Size = new System.Drawing.Size(191, 20);
            this.txt_File.TabIndex = 0;
            // 
            // but_browse
            // 
            this.but_browse.Location = new System.Drawing.Point(209, 10);
            this.but_browse.Name = "but_browse";
            this.but_browse.Size = new System.Drawing.Size(75, 23);
            this.but_browse.TabIndex = 1;
            this.but_browse.Text = "Browse";
            this.but_browse.UseVisualStyleBackColor = true;
            this.but_browse.Click += new System.EventHandler(this.but_browse_Click);
            // 
            // TestForm
            // 
            this.ClientSize = new System.Drawing.Size(297, 44);
            this.Controls.Add(this.but_browse);
            this.Controls.Add(this.txt_File);
            this.Name = "TestForm";
            this.Text = "TestForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_File;
        private MissionPlanner.Controls.MyButton but_browse;
    }
}