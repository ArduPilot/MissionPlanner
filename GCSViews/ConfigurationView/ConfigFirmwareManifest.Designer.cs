namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigFirmwareManifest
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
            this.lbl_status = new System.Windows.Forms.Label();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.lbl_devfw = new System.Windows.Forms.Label();
            this.lbl_Custom_firmware_label = new System.Windows.Forms.Label();
            this.lbl_px4bl = new System.Windows.Forms.Label();
            this.lbl_bootloaderupdate = new System.Windows.Forms.Label();
            this.imageLabel1 = new MissionPlanner.Controls.ImageLabel();
            this.pictureBoxSub = new MissionPlanner.Controls.ImageLabel();
            this.pictureAntennaTracker = new MissionPlanner.Controls.ImageLabel();
            this.pictureBoxRover = new MissionPlanner.Controls.ImageLabel();
            this.pictureBoxHeli = new MissionPlanner.Controls.ImageLabel();
            this.pictureBoxQuad = new MissionPlanner.Controls.ImageLabel();
            this.pictureBoxPlane = new MissionPlanner.Controls.ImageLabel();
            this.lbl_alloptions = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbl_status
            // 
            this.lbl_status.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_status.Location = new System.Drawing.Point(3, 341);
            this.lbl_status.Name = "lbl_status";
            this.lbl_status.Size = new System.Drawing.Size(450, 34);
            this.lbl_status.TabIndex = 51;
            this.lbl_status.Text = "Status";
            // 
            // progress
            // 
            this.progress.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.progress.Location = new System.Drawing.Point(3, 315);
            this.progress.Name = "progress";
            this.progress.Size = new System.Drawing.Size(940, 23);
            this.progress.Step = 1;
            this.progress.TabIndex = 50;
            // 
            // lbl_devfw
            // 
            this.lbl_devfw.AutoSize = true;
            this.lbl_devfw.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_devfw.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_devfw.Location = new System.Drawing.Point(867, 341);
            this.lbl_devfw.Name = "lbl_devfw";
            this.lbl_devfw.Size = new System.Drawing.Size(76, 13);
            this.lbl_devfw.TabIndex = 52;
            this.lbl_devfw.Text = "Beta firmwares";
            this.lbl_devfw.Click += new System.EventHandler(this.Lbl_devfw_Click);
            // 
            // lbl_Custom_firmware_label
            // 
            this.lbl_Custom_firmware_label.AutoSize = true;
            this.lbl_Custom_firmware_label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_Custom_firmware_label.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_Custom_firmware_label.Location = new System.Drawing.Point(736, 341);
            this.lbl_Custom_firmware_label.Name = "lbl_Custom_firmware_label";
            this.lbl_Custom_firmware_label.Size = new System.Drawing.Size(110, 13);
            this.lbl_Custom_firmware_label.TabIndex = 53;
            this.lbl_Custom_firmware_label.Text = "Load custom firmware";
            this.lbl_Custom_firmware_label.Click += new System.EventHandler(this.Lbl_Custom_firmware_label_Click);
            // 
            // lbl_px4bl
            // 
            this.lbl_px4bl.AutoSize = true;
            this.lbl_px4bl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_px4bl.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.lbl_px4bl.Location = new System.Drawing.Point(624, 341);
            this.lbl_px4bl.Name = "lbl_px4bl";
            this.lbl_px4bl.Size = new System.Drawing.Size(88, 13);
            this.lbl_px4bl.TabIndex = 54;
            this.lbl_px4bl.Text = "Force Bootloader";
            this.lbl_px4bl.Click += new System.EventHandler(this.Lbl_px4bl_Click);
            // 
            // lbl_bootloaderupdate
            // 
            this.lbl_bootloaderupdate.AutoSize = true;
            this.lbl_bootloaderupdate.Location = new System.Drawing.Point(522, 341);
            this.lbl_bootloaderupdate.Name = "lbl_bootloaderupdate";
            this.lbl_bootloaderupdate.Size = new System.Drawing.Size(96, 13);
            this.lbl_bootloaderupdate.TabIndex = 55;
            this.lbl_bootloaderupdate.Text = "Bootloader Update";
            this.lbl_bootloaderupdate.Click += new System.EventHandler(this.Lbl_bootloaderupdate_Click);
            // 
            // imageLabel1
            // 
            this.imageLabel1.Image = global::MissionPlanner.Properties.Resources.pixhawk2cube;
            this.imageLabel1.Location = new System.Drawing.Point(783, 159);
            this.imageLabel1.Name = "imageLabel1";
            this.imageLabel1.Size = new System.Drawing.Size(150, 150);
            this.imageLabel1.TabIndex = 49;
            this.imageLabel1.TabStop = false;
            this.imageLabel1.Tag = "";
            // 
            // pictureBoxSub
            // 
            this.pictureBoxSub.Image = global::MissionPlanner.Properties.Resources.sub;
            this.pictureBoxSub.Location = new System.Drawing.Point(783, 3);
            this.pictureBoxSub.Name = "pictureBoxSub";
            this.pictureBoxSub.Size = new System.Drawing.Size(150, 150);
            this.pictureBoxSub.TabIndex = 48;
            this.pictureBoxSub.TabStop = false;
            this.pictureBoxSub.Tag = "";
            this.pictureBoxSub.Click += new System.EventHandler(this.PictureBox_Click);
            // 
            // pictureAntennaTracker
            // 
            this.pictureAntennaTracker.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureAntennaTracker.Image = global::MissionPlanner.Properties.Resources.Antenna_Tracker_01;
            this.pictureAntennaTracker.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureAntennaTracker.Location = new System.Drawing.Point(634, 3);
            this.pictureAntennaTracker.Name = "pictureAntennaTracker";
            this.pictureAntennaTracker.Size = new System.Drawing.Size(150, 150);
            this.pictureAntennaTracker.TabIndex = 47;
            this.pictureAntennaTracker.TabStop = false;
            this.pictureAntennaTracker.Tag = "";
            this.pictureAntennaTracker.Click += new System.EventHandler(this.PictureBox_Click);
            // 
            // pictureBoxRover
            // 
            this.pictureBoxRover.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxRover.Image = global::MissionPlanner.Properties.Resources.rover_11;
            this.pictureBoxRover.Location = new System.Drawing.Point(3, 3);
            this.pictureBoxRover.Name = "pictureBoxRover";
            this.pictureBoxRover.Size = new System.Drawing.Size(150, 150);
            this.pictureBoxRover.TabIndex = 38;
            this.pictureBoxRover.TabStop = false;
            this.pictureBoxRover.Tag = "";
            this.pictureBoxRover.Click += new System.EventHandler(this.PictureBox_Click);
            // 
            // pictureBoxHeli
            // 
            this.pictureBoxHeli.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxHeli.Image = global::MissionPlanner.Properties.Resources.APM_airframes_08;
            this.pictureBoxHeli.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxHeli.Location = new System.Drawing.Point(471, 3);
            this.pictureBoxHeli.Name = "pictureBoxHeli";
            this.pictureBoxHeli.Size = new System.Drawing.Size(150, 150);
            this.pictureBoxHeli.TabIndex = 44;
            this.pictureBoxHeli.TabStop = false;
            this.pictureBoxHeli.Tag = "";
            this.pictureBoxHeli.Click += new System.EventHandler(this.PictureBox_Click);
            // 
            // pictureBoxQuad
            // 
            this.pictureBoxQuad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxQuad.Image = global::MissionPlanner.Properties.Resources.FW_icons_2013_logos_04;
            this.pictureBoxQuad.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxQuad.Location = new System.Drawing.Point(315, 3);
            this.pictureBoxQuad.Name = "pictureBoxQuad";
            this.pictureBoxQuad.Size = new System.Drawing.Size(150, 150);
            this.pictureBoxQuad.TabIndex = 40;
            this.pictureBoxQuad.TabStop = false;
            this.pictureBoxQuad.Tag = "";
            this.pictureBoxQuad.Click += new System.EventHandler(this.PictureBox_Click);
            // 
            // pictureBoxPlane
            // 
            this.pictureBoxPlane.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxPlane.Image = global::MissionPlanner.Properties.Resources.APM_airframes_001;
            this.pictureBoxPlane.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBoxPlane.Location = new System.Drawing.Point(159, 3);
            this.pictureBoxPlane.Name = "pictureBoxPlane";
            this.pictureBoxPlane.Size = new System.Drawing.Size(150, 150);
            this.pictureBoxPlane.TabIndex = 39;
            this.pictureBoxPlane.TabStop = false;
            this.pictureBoxPlane.Tag = "";
            this.pictureBoxPlane.Click += new System.EventHandler(this.PictureBox_Click);
            // 
            // lbl_alloptions
            // 
            this.lbl_alloptions.AutoSize = true;
            this.lbl_alloptions.Location = new System.Drawing.Point(459, 341);
            this.lbl_alloptions.Name = "lbl_alloptions";
            this.lbl_alloptions.Size = new System.Drawing.Size(57, 13);
            this.lbl_alloptions.TabIndex = 56;
            this.lbl_alloptions.Text = "All Options";
            this.lbl_alloptions.Click += new System.EventHandler(this.lbl_alloptions_Click);
            // 
            // ConfigFirmwareManifest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbl_alloptions);
            this.Controls.Add(this.lbl_bootloaderupdate);
            this.Controls.Add(this.lbl_px4bl);
            this.Controls.Add(this.lbl_Custom_firmware_label);
            this.Controls.Add(this.lbl_devfw);
            this.Controls.Add(this.lbl_status);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.imageLabel1);
            this.Controls.Add(this.pictureBoxSub);
            this.Controls.Add(this.pictureAntennaTracker);
            this.Controls.Add(this.pictureBoxRover);
            this.Controls.Add(this.pictureBoxHeli);
            this.Controls.Add(this.pictureBoxQuad);
            this.Controls.Add(this.pictureBoxPlane);
            this.Name = "ConfigFirmwareManifest";
            this.Size = new System.Drawing.Size(946, 375);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Controls.ImageLabel pictureBoxSub;
        private Controls.ImageLabel pictureAntennaTracker;
        private Controls.ImageLabel pictureBoxRover;
        private Controls.ImageLabel pictureBoxHeli;
        private Controls.ImageLabel pictureBoxQuad;
        private Controls.ImageLabel pictureBoxPlane;
        private Controls.ImageLabel imageLabel1;
        private System.Windows.Forms.Label lbl_status;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label lbl_devfw;
        private System.Windows.Forms.Label lbl_Custom_firmware_label;
        private System.Windows.Forms.Label lbl_px4bl;
        private System.Windows.Forms.Label lbl_bootloaderupdate;
        private System.Windows.Forms.Label lbl_alloptions;
    }
}
