using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO.Ports;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Net;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigFirmware : MyUserControl
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
        
        
        private Controls.ImageLabel pictureBoxAPM;
        private Controls.ImageLabel pictureBoxQuad;
        private Controls.ImageLabel pictureBoxHexa;
        private Controls.ImageLabel pictureBoxTri;
        private Controls.ImageLabel pictureBoxY6;
        private System.Windows.Forms.Label lbl_status;
        private System.Windows.Forms.ProgressBar progress;
        private System.Windows.Forms.Label label2;
        private Controls.ImageLabel pictureBoxHeli;
        private PictureBox pictureBoxHilimage;
        private PictureBox pictureBoxAPHil;
        private PictureBox pictureBoxACHil;
        private PictureBox pictureBoxACHHil;
        private Controls.ImageLabel pictureBoxOcta;
        private Label label1;
        private Controls.ImageLabel pictureBoxOctaQuad;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigFirmware));
            this.pictureBoxAPM = new MissionPlanner.Controls.ImageLabel();
            this.pictureBoxQuad = new MissionPlanner.Controls.ImageLabel();
            this.pictureBoxHexa = new MissionPlanner.Controls.ImageLabel();
            this.pictureBoxTri = new MissionPlanner.Controls.ImageLabel();
            this.pictureBoxY6 = new MissionPlanner.Controls.ImageLabel();
            this.lbl_status = new System.Windows.Forms.Label();
            this.progress = new System.Windows.Forms.ProgressBar();
            this.label2 = new System.Windows.Forms.Label();
            this.pictureBoxHeli = new MissionPlanner.Controls.ImageLabel();
            this.pictureBoxHilimage = new System.Windows.Forms.PictureBox();
            this.pictureBoxAPHil = new System.Windows.Forms.PictureBox();
            this.pictureBoxACHil = new System.Windows.Forms.PictureBox();
            this.pictureBoxACHHil = new System.Windows.Forms.PictureBox();
            this.pictureBoxOcta = new MissionPlanner.Controls.ImageLabel();
            this.pictureBoxOctaQuad = new MissionPlanner.Controls.ImageLabel();
            this.pictureBoxRover = new MissionPlanner.Controls.ImageLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.CMB_history = new System.Windows.Forms.ComboBox();
            this.CMB_history_label = new System.Windows.Forms.Label();
            this.lbl_Custom_firmware_label = new System.Windows.Forms.Label();
            this.lbl_devfw = new System.Windows.Forms.Label();
            this.lbl_dlfw = new System.Windows.Forms.Label();
            this.lbl_px4bl = new System.Windows.Forms.Label();
            this.pictureAntennaTracker = new MissionPlanner.Controls.ImageLabel();
            this.lbl_licence = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHilimage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAPHil)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxACHil)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxACHHil)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBoxAPM
            // 
            resources.ApplyResources(this.pictureBoxAPM, "pictureBoxAPM");
            this.pictureBoxAPM.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxAPM.Image = global::MissionPlanner.Properties.Resources.APM_airframes_001;
            this.pictureBoxAPM.Name = "pictureBoxAPM";
            this.pictureBoxAPM.TabStop = false;
            this.pictureBoxAPM.Tag = "";
            this.pictureBoxAPM.Click += new System.EventHandler(this.pictureBoxFW_Click);
            // 
            // pictureBoxQuad
            // 
            resources.ApplyResources(this.pictureBoxQuad, "pictureBoxQuad");
            this.pictureBoxQuad.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxQuad.Image = global::MissionPlanner.Properties.Resources.FW_icons_2013_logos_04;
            this.pictureBoxQuad.Name = "pictureBoxQuad";
            this.pictureBoxQuad.TabStop = false;
            this.pictureBoxQuad.Tag = "";
            this.pictureBoxQuad.Click += new System.EventHandler(this.pictureBoxFW_Click);
            // 
            // pictureBoxHexa
            // 
            resources.ApplyResources(this.pictureBoxHexa, "pictureBoxHexa");
            this.pictureBoxHexa.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxHexa.Image = global::MissionPlanner.Properties.Resources.FW_icons_2013_logos_10;
            this.pictureBoxHexa.Name = "pictureBoxHexa";
            this.pictureBoxHexa.TabStop = false;
            this.pictureBoxHexa.Tag = "";
            this.pictureBoxHexa.Click += new System.EventHandler(this.pictureBoxFW_Click);
            // 
            // pictureBoxTri
            // 
            resources.ApplyResources(this.pictureBoxTri, "pictureBoxTri");
            this.pictureBoxTri.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxTri.Image = global::MissionPlanner.Properties.Resources.FW_icons_2013_logos_08;
            this.pictureBoxTri.Name = "pictureBoxTri";
            this.pictureBoxTri.TabStop = false;
            this.pictureBoxTri.Tag = "";
            this.pictureBoxTri.Click += new System.EventHandler(this.pictureBoxFW_Click);
            // 
            // pictureBoxY6
            // 
            resources.ApplyResources(this.pictureBoxY6, "pictureBoxY6");
            this.pictureBoxY6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxY6.Image = global::MissionPlanner.Properties.Resources.y6a;
            this.pictureBoxY6.Name = "pictureBoxY6";
            this.pictureBoxY6.TabStop = false;
            this.pictureBoxY6.Tag = "";
            this.pictureBoxY6.Click += new System.EventHandler(this.pictureBoxFW_Click);
            // 
            // lbl_status
            // 
            resources.ApplyResources(this.lbl_status, "lbl_status");
            this.lbl_status.Name = "lbl_status";
            // 
            // progress
            // 
            resources.ApplyResources(this.progress, "progress");
            this.progress.Name = "progress";
            this.progress.Step = 1;
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // pictureBoxHeli
            // 
            resources.ApplyResources(this.pictureBoxHeli, "pictureBoxHeli");
            this.pictureBoxHeli.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxHeli.Image = global::MissionPlanner.Properties.Resources.APM_airframes_08;
            this.pictureBoxHeli.Name = "pictureBoxHeli";
            this.pictureBoxHeli.TabStop = false;
            this.pictureBoxHeli.Tag = "";
            this.pictureBoxHeli.Click += new System.EventHandler(this.pictureBoxFW_Click);
            // 
            // pictureBoxHilimage
            // 
            resources.ApplyResources(this.pictureBoxHilimage, "pictureBoxHilimage");
            this.pictureBoxHilimage.Image = global::MissionPlanner.Properties.Resources.hil;
            this.pictureBoxHilimage.Name = "pictureBoxHilimage";
            this.pictureBoxHilimage.TabStop = false;
            this.pictureBoxHilimage.Tag = "";
            // 
            // pictureBoxAPHil
            // 
            resources.ApplyResources(this.pictureBoxAPHil, "pictureBoxAPHil");
            this.pictureBoxAPHil.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxAPHil.Image = global::MissionPlanner.Properties.Resources.hilplane;
            this.pictureBoxAPHil.Name = "pictureBoxAPHil";
            this.pictureBoxAPHil.TabStop = false;
            this.pictureBoxAPHil.Tag = "";
            this.pictureBoxAPHil.Click += new System.EventHandler(this.pictureBoxFW_Click);
            // 
            // pictureBoxACHil
            // 
            resources.ApplyResources(this.pictureBoxACHil, "pictureBoxACHil");
            this.pictureBoxACHil.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxACHil.Image = global::MissionPlanner.Properties.Resources.hilquad;
            this.pictureBoxACHil.Name = "pictureBoxACHil";
            this.pictureBoxACHil.TabStop = false;
            this.pictureBoxACHil.Tag = "";
            this.pictureBoxACHil.Click += new System.EventHandler(this.pictureBoxFW_Click);
            // 
            // pictureBoxACHHil
            // 
            resources.ApplyResources(this.pictureBoxACHHil, "pictureBoxACHHil");
            this.pictureBoxACHHil.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxACHHil.Image = global::MissionPlanner.Properties.Resources.hilheli;
            this.pictureBoxACHHil.Name = "pictureBoxACHHil";
            this.pictureBoxACHHil.TabStop = false;
            this.pictureBoxACHHil.Tag = "";
            this.pictureBoxACHHil.Click += new System.EventHandler(this.pictureBoxFW_Click);
            // 
            // pictureBoxOcta
            // 
            resources.ApplyResources(this.pictureBoxOcta, "pictureBoxOcta");
            this.pictureBoxOcta.Image = global::MissionPlanner.Properties.Resources.FW_icons_2013_logos_12;
            this.pictureBoxOcta.Name = "pictureBoxOcta";
            this.pictureBoxOcta.TabStop = false;
            this.pictureBoxOcta.Tag = "";
            this.pictureBoxOcta.Click += new System.EventHandler(this.pictureBoxFW_Click);
            // 
            // pictureBoxOctaQuad
            // 
            resources.ApplyResources(this.pictureBoxOctaQuad, "pictureBoxOctaQuad");
            this.pictureBoxOctaQuad.Image = global::MissionPlanner.Properties.Resources.x8;
            this.pictureBoxOctaQuad.Name = "pictureBoxOctaQuad";
            this.pictureBoxOctaQuad.TabStop = false;
            this.pictureBoxOctaQuad.Tag = "";
            this.pictureBoxOctaQuad.Click += new System.EventHandler(this.pictureBoxFW_Click);
            // 
            // pictureBoxRover
            // 
            resources.ApplyResources(this.pictureBoxRover, "pictureBoxRover");
            this.pictureBoxRover.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBoxRover.Image = global::MissionPlanner.Properties.Resources.rover_11;
            this.pictureBoxRover.Name = "pictureBoxRover";
            this.pictureBoxRover.TabStop = false;
            this.pictureBoxRover.Tag = "";
            this.pictureBoxRover.Click += new System.EventHandler(this.pictureBoxFW_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // CMB_history
            // 
            resources.ApplyResources(this.CMB_history, "CMB_history");
            this.CMB_history.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_history.DropDownWidth = 160;
            this.CMB_history.FormattingEnabled = true;
            this.CMB_history.Name = "CMB_history";
            this.CMB_history.SelectedIndexChanged += new System.EventHandler(this.CMB_history_SelectedIndexChanged);
            // 
            // CMB_history_label
            // 
            resources.ApplyResources(this.CMB_history_label, "CMB_history_label");
            this.CMB_history_label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CMB_history_label.Name = "CMB_history_label";
            this.CMB_history_label.Click += new System.EventHandler(this.CMB_history_label_Click);
            // 
            // lbl_Custom_firmware_label
            // 
            resources.ApplyResources(this.lbl_Custom_firmware_label, "lbl_Custom_firmware_label");
            this.lbl_Custom_firmware_label.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_Custom_firmware_label.Name = "lbl_Custom_firmware_label";
            this.lbl_Custom_firmware_label.Click += new System.EventHandler(this.Custom_firmware_label_Click);
            // 
            // lbl_devfw
            // 
            resources.ApplyResources(this.lbl_devfw, "lbl_devfw");
            this.lbl_devfw.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_devfw.Name = "lbl_devfw";
            this.lbl_devfw.Click += new System.EventHandler(this.lbl_devfw_Click);
            // 
            // lbl_dlfw
            // 
            resources.ApplyResources(this.lbl_dlfw, "lbl_dlfw");
            this.lbl_dlfw.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_dlfw.Name = "lbl_dlfw";
            this.lbl_dlfw.Click += new System.EventHandler(this.lbl_dlfw_Click);
            // 
            // lbl_px4bl
            // 
            resources.ApplyResources(this.lbl_px4bl, "lbl_px4bl");
            this.lbl_px4bl.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_px4bl.Name = "lbl_px4bl";
            this.lbl_px4bl.Click += new System.EventHandler(this.lbl_px4bl_Click);
            // 
            // pictureAntennaTracker
            // 
            resources.ApplyResources(this.pictureAntennaTracker, "pictureAntennaTracker");
            this.pictureAntennaTracker.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureAntennaTracker.Image = global::MissionPlanner.Properties.Resources.Antenna_Tracker_01;
            this.pictureAntennaTracker.Name = "pictureAntennaTracker";
            this.pictureAntennaTracker.TabStop = false;
            this.pictureAntennaTracker.Tag = "";
            this.pictureAntennaTracker.Click += new System.EventHandler(this.pictureBoxFW_Click);
            // 
            // lbl_licence
            // 
            resources.ApplyResources(this.lbl_licence, "lbl_licence");
            this.lbl_licence.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_licence.Name = "lbl_licence";
            this.lbl_licence.Click += new System.EventHandler(this.lbl_dlfw_Click);
            // 
            // linkLabel1
            // 
            resources.ApplyResources(this.linkLabel1, "linkLabel1");
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.TabStop = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // ConfigFirmware
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.lbl_licence);
            this.Controls.Add(this.pictureAntennaTracker);
            this.Controls.Add(this.lbl_px4bl);
            this.Controls.Add(this.lbl_dlfw);
            this.Controls.Add(this.lbl_devfw);
            this.Controls.Add(this.lbl_Custom_firmware_label);
            this.Controls.Add(this.CMB_history_label);
            this.Controls.Add(this.pictureBoxRover);
            this.Controls.Add(this.CMB_history);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lbl_status);
            this.Controls.Add(this.progress);
            this.Controls.Add(this.pictureBoxACHHil);
            this.Controls.Add(this.pictureBoxACHil);
            this.Controls.Add(this.pictureBoxAPHil);
            this.Controls.Add(this.pictureBoxHilimage);
            this.Controls.Add(this.pictureBoxOctaQuad);
            this.Controls.Add(this.pictureBoxOcta);
            this.Controls.Add(this.pictureBoxHeli);
            this.Controls.Add(this.pictureBoxY6);
            this.Controls.Add(this.pictureBoxTri);
            this.Controls.Add(this.pictureBoxHexa);
            this.Controls.Add(this.pictureBoxQuad);
            this.Controls.Add(this.pictureBoxAPM);
            this.Name = "ConfigFirmware";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxHilimage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxAPHil)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxACHil)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxACHHil)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private ComboBox CMB_history;
        private Controls.ImageLabel pictureBoxRover;
        private Label CMB_history_label;
        private Label lbl_Custom_firmware_label;
        private Label lbl_devfw;
        private Label lbl_dlfw;
        private Label lbl_px4bl;
        private Controls.ImageLabel pictureAntennaTracker;
        private Label lbl_licence;
        private LinkLabel linkLabel1;

    }
}