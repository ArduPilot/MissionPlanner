
namespace MissionPlanner.Controls
{
    partial class OpenDroneID_Map_Status
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
            this.LBL_ODID_reason = new System.Windows.Forms.Label();
            this.LED_ODID_Status = new Bulb.LedBulb();
            this.LBL_ODID_OK = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LBL_ODID_reason
            // 
            this.LBL_ODID_reason.AutoSize = true;
            this.LBL_ODID_reason.Location = new System.Drawing.Point(24, 23);
            this.LBL_ODID_reason.Name = "LBL_ODID_reason";
            this.LBL_ODID_reason.Size = new System.Drawing.Size(65, 13);
            this.LBL_ODID_reason.TabIndex = 1;
            this.LBL_ODID_reason.Text = "{Loading....}";
            this.LBL_ODID_reason.DoubleClick += new System.EventHandler(this.OpenDroneID_Map_Status_DoubleClick);
            // 
            // LED_ODID_Status
            // 
            this.LED_ODID_Status.Color = System.Drawing.Color.White;
            this.LED_ODID_Status.Location = new System.Drawing.Point(4, 4);
            this.LED_ODID_Status.Name = "LED_ODID_Status";
            this.LED_ODID_Status.On = true;
            this.LED_ODID_Status.Size = new System.Drawing.Size(16, 16);
            this.LED_ODID_Status.TabIndex = 2;
            this.LED_ODID_Status.Text = "ledBulb1";
            this.LED_ODID_Status.DoubleClick += new System.EventHandler(this.OpenDroneID_Map_Status_DoubleClick);
            // 
            // LBL_ODID_OK
            // 
            this.LBL_ODID_OK.AutoSize = true;
            this.LBL_ODID_OK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LBL_ODID_OK.Location = new System.Drawing.Point(24, 4);
            this.LBL_ODID_OK.Name = "LBL_ODID_OK";
            this.LBL_ODID_OK.Size = new System.Drawing.Size(71, 16);
            this.LBL_ODID_OK.TabIndex = 3;
            this.LBL_ODID_OK.Text = "Remote ID";
            this.LBL_ODID_OK.DoubleClick += new System.EventHandler(this.OpenDroneID_Map_Status_DoubleClick);
            // 
            // OpenDroneID_Map_Status
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LBL_ODID_OK);
            this.Controls.Add(this.LED_ODID_Status);
            this.Controls.Add(this.LBL_ODID_reason);
            this.Name = "OpenDroneID_Map_Status";
            this.Size = new System.Drawing.Size(155, 42);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label LBL_ODID_reason;
        private Bulb.LedBulb LED_ODID_Status;
        private System.Windows.Forms.Label LBL_ODID_OK;
    }
}
