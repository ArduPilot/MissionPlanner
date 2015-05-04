namespace MissionPlanner.Controls
{
    partial class SysidSelector
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
            this.cmb_sysid = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.myButton1 = new MissionPlanner.Controls.MyButton();
            this.SuspendLayout();
            // 
            // cmb_sysid
            // 
            this.cmb_sysid.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_sysid.FormattingEnabled = true;
            this.cmb_sysid.Location = new System.Drawing.Point(76, 38);
            this.cmb_sysid.Name = "cmb_sysid";
            this.cmb_sysid.Size = new System.Drawing.Size(161, 21);
            this.cmb_sysid.TabIndex = 0;
            this.cmb_sysid.SelectedIndexChanged += new System.EventHandler(this.cmb_sysid_SelectedIndexChanged);
            this.cmb_sysid.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.cmb_sysid_Format);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(225, 26);
            this.label1.TabIndex = 1;
            this.label1.Text = "More than one sysid has been detected,\r\nplease select the sysid you want to conne" +
    "ct to";
            // 
            // myButton1
            // 
            this.myButton1.Location = new System.Drawing.Point(117, 65);
            this.myButton1.Name = "myButton1";
            this.myButton1.Size = new System.Drawing.Size(75, 23);
            this.myButton1.TabIndex = 2;
            this.myButton1.Text = "Close";
            this.myButton1.UseVisualStyleBackColor = true;
            this.myButton1.Click += new System.EventHandler(this.myButton1_Click);
            // 
            // SysidSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(301, 94);
            this.Controls.Add(this.myButton1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmb_sysid);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SysidSelector";
            this.Text = "SysidSelector";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmb_sysid;
        private System.Windows.Forms.Label label1;
        private MyButton myButton1;
    }
}