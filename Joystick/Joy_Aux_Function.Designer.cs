namespace MissionPlanner.Joystick
{
    partial class Joy_Aux_Function
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
            this.labelFunction = new System.Windows.Forms.Label();
            this.comboBoxFunction = new System.Windows.Forms.ComboBox();
            this.labelTrigger = new System.Windows.Forms.Label();
            this.comboBoxTrigger = new System.Windows.Forms.ComboBox();
            this.labelNote = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // labelFunction
            //
            this.labelFunction.AutoSize = true;
            this.labelFunction.Location = new System.Drawing.Point(12, 12);
            this.labelFunction.Name = "labelFunction";
            this.labelFunction.Size = new System.Drawing.Size(48, 13);
            this.labelFunction.TabIndex = 0;
            this.labelFunction.Text = "Function";
            //
            // comboBoxFunction
            //
            this.comboBoxFunction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFunction.FormattingEnabled = true;
            this.comboBoxFunction.Location = new System.Drawing.Point(80, 9);
            this.comboBoxFunction.Name = "comboBoxFunction";
            this.comboBoxFunction.Size = new System.Drawing.Size(280, 21);
            this.comboBoxFunction.TabIndex = 1;
            //
            // labelTrigger
            //
            this.labelTrigger.AutoSize = true;
            this.labelTrigger.Location = new System.Drawing.Point(12, 39);
            this.labelTrigger.Name = "labelTrigger";
            this.labelTrigger.Size = new System.Drawing.Size(40, 13);
            this.labelTrigger.TabIndex = 2;
            this.labelTrigger.Text = "Trigger";
            //
            // comboBoxTrigger
            //
            this.comboBoxTrigger.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTrigger.FormattingEnabled = true;
            this.comboBoxTrigger.Location = new System.Drawing.Point(80, 36);
            this.comboBoxTrigger.Name = "comboBoxTrigger";
            this.comboBoxTrigger.Size = new System.Drawing.Size(280, 21);
            this.comboBoxTrigger.TabIndex = 3;
            //
            // labelNote
            //
            this.labelNote.AutoSize = true;
            this.labelNote.Location = new System.Drawing.Point(12, 68);
            this.labelNote.Name = "labelNote";
            this.labelNote.Size = new System.Drawing.Size(340, 26);
            this.labelNote.TabIndex = 4;
            this.labelNote.Text = "Sends MAV_CMD_DO_AUX_FUNCTION to the vehicle (ArduPilot 4.1 or later).\r\n" +
                                  "No RCx_OPTION parameter needs to be assigned on the autopilot.";
            //
            // Joy_Aux_Function
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(372, 104);
            this.Controls.Add(this.labelNote);
            this.Controls.Add(this.comboBoxTrigger);
            this.Controls.Add(this.labelTrigger);
            this.Controls.Add(this.comboBoxFunction);
            this.Controls.Add(this.labelFunction);
            this.Name = "Joy_Aux_Function";
            this.Text = "Aux Function";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelFunction;
        private System.Windows.Forms.ComboBox comboBoxFunction;
        private System.Windows.Forms.Label labelTrigger;
        private System.Windows.Forms.ComboBox comboBoxTrigger;
        private System.Windows.Forms.Label labelNote;
    }
}
