namespace MissionPlanner
{
    partial class NMEA_Viewer
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
            this.TXT_Data = new System.Windows.Forms.RichTextBox();
            this.LBL_port_txt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TXT_Data
            // 
            this.TXT_Data.Location = new System.Drawing.Point(12, 44);
            this.TXT_Data.Name = "TXT_Data";
            this.TXT_Data.Size = new System.Drawing.Size(939, 575);
            this.TXT_Data.TabIndex = 0;
            this.TXT_Data.Text = "";
            // 
            // LBL_port_txt
            // 
            this.LBL_port_txt.AutoSize = true;
            this.LBL_port_txt.Location = new System.Drawing.Point(13, 13);
            this.LBL_port_txt.Name = "LBL_port_txt";
            this.LBL_port_txt.Size = new System.Drawing.Size(103, 13);
            this.LBL_port_txt.TabIndex = 1;
            this.LBL_port_txt.Text = "NMEA String Viewer";
            // 
            // NMEA_Viewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(963, 631);
            this.Controls.Add(this.LBL_port_txt);
            this.Controls.Add(this.TXT_Data);
            this.Name = "NMEA_Viewer";
            this.Text = "NMEA_Viewer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox TXT_Data;
        private System.Windows.Forms.Label LBL_port_txt;
    }
}