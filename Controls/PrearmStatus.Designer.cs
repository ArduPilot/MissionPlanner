namespace MissionPlanner.Controls
{
    partial class PrearmStatus
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
            this.components = new System.ComponentModel.Container();
            this.TXT_PrearmErrors = new System.Windows.Forms.TextBox();
            this.updatetexttimer = new System.Windows.Forms.Timer(this.components);
            this.requestchecktimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // TXT_PrearmErrors
            // 
            this.TXT_PrearmErrors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TXT_PrearmErrors.Location = new System.Drawing.Point(10, 10);
            this.TXT_PrearmErrors.Multiline = true;
            this.TXT_PrearmErrors.Name = "TXT_PrearmErrors";
            this.TXT_PrearmErrors.Size = new System.Drawing.Size(364, 241);
            this.TXT_PrearmErrors.TabIndex = 0;
            // 
            // updatetexttimer
            // 
            this.updatetexttimer.Tick += new System.EventHandler(this.updatetexttimer_Tick);
            // 
            // requestchecktimer
            // 
            this.requestchecktimer.Interval = 5000;
            this.requestchecktimer.Tick += new System.EventHandler(this.requestchecktimer_Tick);
            // 
            // PrearmStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(384, 261);
            this.Controls.Add(this.TXT_PrearmErrors);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "PrearmStatus";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.Text = "Prearm Checks";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TXT_PrearmErrors;
        private System.Windows.Forms.Timer updatetexttimer;
        private System.Windows.Forms.Timer requestchecktimer;
    }
}