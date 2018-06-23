namespace MissionPlanner
{
    partial class ConnectionConfigForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionConfigForm));
            this.connectionControl = new MissionPlanner.Controls.ConnectionControl();
            this.SuspendLayout();
            // 
            // connectionControl
            // 
            this.connectionControl.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("connectionControl.BackgroundImage")));
            this.connectionControl.Location = new System.Drawing.Point(138, 69);
            this.connectionControl.MinimumSize = new System.Drawing.Size(230, 54);
            this.connectionControl.Name = "connectionControl";
            this.connectionControl.Size = new System.Drawing.Size(230, 57);
            this.connectionControl.TabIndex = 0;
            // 
            // ConnectionConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::MissionPlanner.ConnectForm.connectionControl1_BackgroundImage;
            this.ClientSize = new System.Drawing.Size(484, 361);
            this.ControlBox = false;
            this.Controls.Add(this.connectionControl);
            this.Name = "ConnectionConfigForm";
            this.Text = "ConnectionConfigForm";
            this.ResumeLayout(false);

        }

        #endregion

        private Controls.ConnectionControl connectionControl;
    }
}