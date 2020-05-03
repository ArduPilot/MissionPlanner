namespace Installer
{
    partial class Installer
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
            this.but_Start = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.but_folder = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // but_Start
            // 
            this.but_Start.Location = new System.Drawing.Point(299, 39);
            this.but_Start.Name = "but_Start";
            this.but_Start.Size = new System.Drawing.Size(75, 23);
            this.but_Start.TabIndex = 0;
            this.but_Start.Text = "Start";
            this.but_Start.UseVisualStyleBackColor = true;
            this.but_Start.Click += new System.EventHandler(this.but_Start_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(281, 96);
            this.label1.TabIndex = 1;
            this.label1.Text = "Status";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(281, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // but_folder
            // 
            this.but_folder.Location = new System.Drawing.Point(299, 10);
            this.but_folder.Name = "but_folder";
            this.but_folder.Size = new System.Drawing.Size(75, 23);
            this.but_folder.TabIndex = 3;
            this.but_folder.Text = "Browse";
            this.but_folder.UseVisualStyleBackColor = true;
            this.but_folder.Click += new System.EventHandler(this.but_folder_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(300, 69);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(55, 17);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Stable";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Location = new System.Drawing.Point(299, 92);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(47, 17);
            this.radioButton2.TabIndex = 5;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Beta";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(300, 115);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(57, 17);
            this.radioButton3.TabIndex = 6;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Master";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.radioButton3_CheckedChanged);
            // 
            // Installer
            // 
   
            this.ClientSize = new System.Drawing.Size(382, 140);
            this.Controls.Add(this.radioButton3);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.but_folder);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.but_Start);
            this.Name = "Installer";
            this.Text = "Install";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button but_Start;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button but_folder;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
    }
}

