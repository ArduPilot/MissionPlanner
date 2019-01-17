namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigOSD
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
            this.osdUserControl = new OSDConfigurator.GUI.OSDUserControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnWrite = new System.Windows.Forms.Button();
            this.cbAutoWriteOnLeave = new System.Windows.Forms.CheckBox();
            this.btnDiscardChanges = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // osdUserControl
            // 
            this.osdUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.osdUserControl.Location = new System.Drawing.Point(8, 8);
            this.osdUserControl.Name = "osdUserControl";
            this.osdUserControl.Size = new System.Drawing.Size(949, 807);
            this.osdUserControl.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnDiscardChanges);
            this.panel1.Controls.Add(this.cbAutoWriteOnLeave);
            this.panel1.Controls.Add(this.btnWrite);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(957, 8);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(182, 807);
            this.panel1.TabIndex = 1;
            // 
            // btnWrite
            // 
            this.btnWrite.Location = new System.Drawing.Point(20, 23);
            this.btnWrite.Name = "btnWrite";
            this.btnWrite.Size = new System.Drawing.Size(145, 23);
            this.btnWrite.TabIndex = 0;
            this.btnWrite.Text = "Write customization";
            this.btnWrite.UseVisualStyleBackColor = true;
            // 
            // cbAutoWriteOnLeave
            // 
            this.cbAutoWriteOnLeave.AutoSize = true;
            this.cbAutoWriteOnLeave.Checked = true;
            this.cbAutoWriteOnLeave.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAutoWriteOnLeave.Location = new System.Drawing.Point(24, 56);
            this.cbAutoWriteOnLeave.Name = "cbAutoWriteOnLeave";
            this.cbAutoWriteOnLeave.Size = new System.Drawing.Size(125, 17);
            this.cbAutoWriteOnLeave.TabIndex = 1;
            this.cbAutoWriteOnLeave.Text = "Auto write on leaving";
            this.cbAutoWriteOnLeave.UseVisualStyleBackColor = true;
            // 
            // btnDiscardChanges
            // 
            this.btnDiscardChanges.Location = new System.Drawing.Point(20, 108);
            this.btnDiscardChanges.Name = "btnDiscardChanges";
            this.btnDiscardChanges.Size = new System.Drawing.Size(145, 23);
            this.btnDiscardChanges.TabIndex = 2;
            this.btnDiscardChanges.Text = "Discard all changes";
            this.btnDiscardChanges.UseVisualStyleBackColor = true;
            // 
            // ConfigOSD
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.osdUserControl);
            this.Controls.Add(this.panel1);
            this.Name = "ConfigOSD";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Size = new System.Drawing.Size(1147, 823);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private OSDConfigurator.GUI.OSDUserControl osdUserControl;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnWrite;
        private System.Windows.Forms.CheckBox cbAutoWriteOnLeave;
        private System.Windows.Forms.Button btnDiscardChanges;
    }
}
