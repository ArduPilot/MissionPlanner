using MissionPlanner.Controls;
using System.Windows.Forms;

namespace MissionPlanner.Joystick
{
    partial class JoystickSetup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(JoystickSetup));
            this.CMB_joysticks = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.CHK_elevons = new System.Windows.Forms.CheckBox();
            this.BUT_enable = new MissionPlanner.Controls.MyButton();
            this.BUT_save = new MissionPlanner.Controls.MyButton();
            this.label14 = new System.Windows.Forms.Label();
            this.chk_manualcontrol = new System.Windows.Forms.CheckBox();
            this.but_export = new MissionPlanner.Controls.MyButton();
            this.but_import = new MissionPlanner.Controls.MyButton();
            this.lbl_profile = new System.Windows.Forms.Label();
            this.CMB_profile = new System.Windows.Forms.ComboBox();
            this.BUT_profileCreate = new MissionPlanner.Controls.MyButton();
            this.BUT_profileDelete = new MissionPlanner.Controls.MyButton();
            this.SuspendLayout();
            // 
            // CMB_joysticks
            // 
            this.CMB_joysticks.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_joysticks, "CMB_joysticks");
            this.CMB_joysticks.Name = "CMB_joysticks";
            this.CMB_joysticks.SelectedIndexChanged += new System.EventHandler(this.CMB_joysticks_SelectedIndexChanged);
            this.CMB_joysticks.Click += new System.EventHandler(this.CMB_joysticks_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // CHK_elevons
            // 
            resources.ApplyResources(this.CHK_elevons, "CHK_elevons");
            this.CHK_elevons.Name = "CHK_elevons";
            this.CHK_elevons.UseVisualStyleBackColor = true;
            this.CHK_elevons.CheckedChanged += new System.EventHandler(this.CHK_elevons_CheckedChanged);
            // 
            // BUT_enable
            // 
            resources.ApplyResources(this.BUT_enable, "BUT_enable");
            this.BUT_enable.Name = "BUT_enable";
            this.BUT_enable.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_enable.UseVisualStyleBackColor = true;
            this.BUT_enable.Click += new System.EventHandler(this.BUT_enable_Click);
            // 
            // BUT_save
            // 
            resources.ApplyResources(this.BUT_save, "BUT_save");
            this.BUT_save.Name = "BUT_save";
            this.BUT_save.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_save.UseVisualStyleBackColor = true;
            this.BUT_save.Click += new System.EventHandler(this.BUT_save_Click);
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // chk_manualcontrol
            // 
            resources.ApplyResources(this.chk_manualcontrol, "chk_manualcontrol");
            this.chk_manualcontrol.Name = "chk_manualcontrol";
            this.chk_manualcontrol.UseVisualStyleBackColor = true;
            this.chk_manualcontrol.CheckedChanged += new System.EventHandler(this.chk_manualcontrol_CheckedChanged);
            // 
            // but_export
            // 
            resources.ApplyResources(this.but_export, "but_export");
            this.but_export.Name = "but_export";
            this.but_export.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_export.UseVisualStyleBackColor = true;
            this.but_export.Click += new System.EventHandler(this.but_export_Click);
            // 
            // but_import
            // 
            resources.ApplyResources(this.but_import, "but_import");
            this.but_import.Name = "but_import";
            this.but_import.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.but_import.UseVisualStyleBackColor = true;
            this.but_import.Click += new System.EventHandler(this.but_import_Click);
            //
            // lbl_profile
            //
            this.lbl_profile.AutoSize = true;
            this.lbl_profile.Name = "lbl_profile";
            this.lbl_profile.Text = "Profile";
            //
            // CMB_profile
            //
            this.CMB_profile.FormattingEnabled = true;
            this.CMB_profile.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_profile.Name = "CMB_profile";
            this.CMB_profile.SelectedIndexChanged += new System.EventHandler(this.CMB_profile_SelectedIndexChanged);
            //
            // BUT_profileCreate
            //
            this.BUT_profileCreate.Name = "BUT_profileCreate";
            this.BUT_profileCreate.Text = "Create";
            this.BUT_profileCreate.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_profileCreate.UseVisualStyleBackColor = true;
            this.BUT_profileCreate.Click += new System.EventHandler(this.BUT_profileCreate_Click);
            //
            // BUT_profileDelete
            //
            this.BUT_profileDelete.Name = "BUT_profileDelete";
            this.BUT_profileDelete.Text = "Delete";
            this.BUT_profileDelete.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_profileDelete.UseVisualStyleBackColor = true;
            this.BUT_profileDelete.Click += new System.EventHandler(this.BUT_profileDelete_Click);
            //
            // JoystickSetup
            //
            this.Controls.Add(this.but_import);
            this.Controls.Add(this.but_export);
            this.Controls.Add(this.chk_manualcontrol);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.CHK_elevons);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.BUT_enable);
            this.Controls.Add(this.BUT_save);
            this.Controls.Add(this.CMB_joysticks);
            this.Controls.Add(this.lbl_profile);
            this.Controls.Add(this.CMB_profile);
            this.Controls.Add(this.BUT_profileCreate);
            this.Controls.Add(this.BUT_profileDelete);
            resources.ApplyResources(this, "$this");
            this.Name = "JoystickSetup";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.JoystickSetup_FormClosed);
            this.Load += new System.EventHandler(this.Joystick_Load);

            this.lbl_profile.Location = new System.Drawing.Point(19, 50);
            this.CMB_profile.Location = new System.Drawing.Point(72, 47);
            this.CMB_profile.Size = new System.Drawing.Size(170, 21);
            this.BUT_profileCreate.Location = new System.Drawing.Point(252, 47);
            this.BUT_profileCreate.Size = new System.Drawing.Size(70, 23);
            this.BUT_profileDelete.Location = new System.Drawing.Point(328, 47);
            this.BUT_profileDelete.Size = new System.Drawing.Size(60, 23);

            this.label6.Location = new System.Drawing.Point(this.label6.Location.X, this.label6.Location.Y + 32);
            this.label7.Location = new System.Drawing.Point(this.label7.Location.X, this.label7.Location.Y + 32);
            this.label8.Location = new System.Drawing.Point(this.label8.Location.X, this.label8.Location.Y + 32);
            this.label9.Location = new System.Drawing.Point(this.label9.Location.X, this.label9.Location.Y + 32);
            this.chk_manualcontrol.Location = new System.Drawing.Point(this.chk_manualcontrol.Location.X, this.chk_manualcontrol.Location.Y + 32);
            this.CHK_elevons.Location = new System.Drawing.Point(this.CHK_elevons.Location.X, this.CHK_elevons.Location.Y + 32);

            this.MinimumSize = new System.Drawing.Size(this.MinimumSize.Width, this.MinimumSize.Height + 32);

            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_joysticks;
        private Controls.MyButton BUT_save;
        private Controls.MyButton BUT_enable;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox CHK_elevons;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.CheckBox chk_manualcontrol;
        private MyButton but_export;
        private MyButton but_import;
        private System.Windows.Forms.Label lbl_profile;
        private System.Windows.Forms.ComboBox CMB_profile;
        private MyButton BUT_profileCreate;
        private MyButton BUT_profileDelete;
    }
}