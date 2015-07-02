namespace MissionPlanner.Utilities.DroneApi.UI
{
    partial class VehicleSelection
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
            this.CMB_vehicle = new System.Windows.Forms.ComboBox();
            this.BUT_Select = new MissionPlanner.Controls.MyButton();
            this.BUT_new = new MissionPlanner.Controls.MyButton();
            this.SuspendLayout();
            // 
            // CMB_vehicle
            // 
            this.CMB_vehicle.FormattingEnabled = true;
            this.CMB_vehicle.Location = new System.Drawing.Point(12, 12);
            this.CMB_vehicle.Name = "CMB_vehicle";
            this.CMB_vehicle.Size = new System.Drawing.Size(121, 21);
            this.CMB_vehicle.TabIndex = 0;
            this.CMB_vehicle.Format += new System.Windows.Forms.ListControlConvertEventHandler(this.CMB_vehicle_Format);
            // 
            // BUT_Select
            // 
            this.BUT_Select.Location = new System.Drawing.Point(139, 12);
            this.BUT_Select.Name = "BUT_Select";
            this.BUT_Select.Size = new System.Drawing.Size(75, 23);
            this.BUT_Select.TabIndex = 1;
            this.BUT_Select.Text = "Select";
            this.BUT_Select.UseVisualStyleBackColor = true;
            this.BUT_Select.Click += new System.EventHandler(this.BUT_Select_Click);
            // 
            // BUT_new
            // 
            this.BUT_new.Location = new System.Drawing.Point(220, 12);
            this.BUT_new.Name = "BUT_new";
            this.BUT_new.Size = new System.Drawing.Size(75, 23);
            this.BUT_new.TabIndex = 2;
            this.BUT_new.Text = "New";
            this.BUT_new.UseVisualStyleBackColor = true;
            this.BUT_new.Click += new System.EventHandler(this.BUT_new_Click);
            // 
            // VehicleSelection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 42);
            this.Controls.Add(this.BUT_new);
            this.Controls.Add(this.BUT_Select);
            this.Controls.Add(this.CMB_vehicle);
            this.Name = "VehicleSelection";
            this.Text = "VehicleSelection";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_vehicle;
        private Controls.MyButton BUT_Select;
        private Controls.MyButton BUT_new;
    }
}