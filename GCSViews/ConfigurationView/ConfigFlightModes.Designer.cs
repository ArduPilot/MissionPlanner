﻿namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigFlightModes
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigFlightModes));
            this.CB_simple6 = new System.Windows.Forms.CheckBox();
            this.CB_simple5 = new System.Windows.Forms.CheckBox();
            this.CB_simple4 = new System.Windows.Forms.CheckBox();
            this.CB_simple3 = new System.Windows.Forms.CheckBox();
            this.CB_simple2 = new System.Windows.Forms.CheckBox();
            this.CB_simple1 = new System.Windows.Forms.CheckBox();
            this.label14 = new System.Windows.Forms.Label();
            this.LBL_flightmodepwm = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.lbl_currentmode = new System.Windows.Forms.Label();
            this.currentStateBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.labelfm6 = new System.Windows.Forms.Label();
            this.CMB_fmode6 = new System.Windows.Forms.ComboBox();
            this.labelfm5 = new System.Windows.Forms.Label();
            this.CMB_fmode5 = new System.Windows.Forms.ComboBox();
            this.labelfm4 = new System.Windows.Forms.Label();
            this.CMB_fmode4 = new System.Windows.Forms.ComboBox();
            this.labelfm3 = new System.Windows.Forms.Label();
            this.CMB_fmode3 = new System.Windows.Forms.ComboBox();
            this.labelfm2 = new System.Windows.Forms.Label();
            this.CMB_fmode2 = new System.Windows.Forms.ComboBox();
            this.labelfm1 = new System.Windows.Forms.Label();
            this.CMB_fmode1 = new System.Windows.Forms.ComboBox();
            this.BUT_SaveModes = new MissionPlanner.Controls.MyButton();
            this.chk_ss6 = new System.Windows.Forms.CheckBox();
            this.chk_ss5 = new System.Windows.Forms.CheckBox();
            this.chk_ss4 = new System.Windows.Forms.CheckBox();
            this.chk_ss3 = new System.Windows.Forms.CheckBox();
            this.chk_ss2 = new System.Windows.Forms.CheckBox();
            this.chk_ss1 = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.linkLabel1_ss = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.currentStateBindingSource)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // CB_simple6
            // 
            resources.ApplyResources(this.CB_simple6, "CB_simple6");
            this.CB_simple6.Name = "CB_simple6";
            this.CB_simple6.UseVisualStyleBackColor = true;
            // 
            // CB_simple5
            // 
            resources.ApplyResources(this.CB_simple5, "CB_simple5");
            this.CB_simple5.Name = "CB_simple5";
            this.CB_simple5.UseVisualStyleBackColor = true;
            // 
            // CB_simple4
            // 
            resources.ApplyResources(this.CB_simple4, "CB_simple4");
            this.CB_simple4.Name = "CB_simple4";
            this.CB_simple4.UseVisualStyleBackColor = true;
            // 
            // CB_simple3
            // 
            resources.ApplyResources(this.CB_simple3, "CB_simple3");
            this.CB_simple3.Name = "CB_simple3";
            this.CB_simple3.UseVisualStyleBackColor = true;
            // 
            // CB_simple2
            // 
            resources.ApplyResources(this.CB_simple2, "CB_simple2");
            this.CB_simple2.Name = "CB_simple2";
            this.CB_simple2.UseVisualStyleBackColor = true;
            // 
            // CB_simple1
            // 
            resources.ApplyResources(this.CB_simple1, "CB_simple1");
            this.CB_simple1.Name = "CB_simple1";
            this.CB_simple1.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // LBL_flightmodepwm
            // 
            resources.ApplyResources(this.LBL_flightmodepwm, "LBL_flightmodepwm");
            this.LBL_flightmodepwm.Name = "LBL_flightmodepwm";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // lbl_currentmode
            // 
            resources.ApplyResources(this.lbl_currentmode, "lbl_currentmode");
            this.lbl_currentmode.DataBindings.Add(new System.Windows.Forms.Binding("Text", this.currentStateBindingSource, "mode", true));
            this.lbl_currentmode.Name = "lbl_currentmode";
            // 
            // currentStateBindingSource
            // 
            this.currentStateBindingSource.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // labelfm6
            // 
            resources.ApplyResources(this.labelfm6, "labelfm6");
            this.labelfm6.Name = "labelfm6";
            // 
            // CMB_fmode6
            // 
            this.CMB_fmode6.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CMB_fmode6.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            resources.ApplyResources(this.CMB_fmode6, "CMB_fmode6");
            this.CMB_fmode6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_fmode6.FormattingEnabled = true;
            this.CMB_fmode6.Name = "CMB_fmode6";
            this.CMB_fmode6.SelectedIndexChanged += new System.EventHandler(this.flightmode_SelectedIndexChanged);
            // 
            // labelfm5
            // 
            resources.ApplyResources(this.labelfm5, "labelfm5");
            this.labelfm5.Name = "labelfm5";
            // 
            // CMB_fmode5
            // 
            this.CMB_fmode5.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CMB_fmode5.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            resources.ApplyResources(this.CMB_fmode5, "CMB_fmode5");
            this.CMB_fmode5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_fmode5.FormattingEnabled = true;
            this.CMB_fmode5.Name = "CMB_fmode5";
            this.CMB_fmode5.SelectedIndexChanged += new System.EventHandler(this.flightmode_SelectedIndexChanged);
            // 
            // labelfm4
            // 
            resources.ApplyResources(this.labelfm4, "labelfm4");
            this.labelfm4.Name = "labelfm4";
            // 
            // CMB_fmode4
            // 
            this.CMB_fmode4.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CMB_fmode4.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            resources.ApplyResources(this.CMB_fmode4, "CMB_fmode4");
            this.CMB_fmode4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_fmode4.FormattingEnabled = true;
            this.CMB_fmode4.Name = "CMB_fmode4";
            this.CMB_fmode4.SelectedIndexChanged += new System.EventHandler(this.flightmode_SelectedIndexChanged);
            // 
            // labelfm3
            // 
            resources.ApplyResources(this.labelfm3, "labelfm3");
            this.labelfm3.Name = "labelfm3";
            // 
            // CMB_fmode3
            // 
            this.CMB_fmode3.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CMB_fmode3.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            resources.ApplyResources(this.CMB_fmode3, "CMB_fmode3");
            this.CMB_fmode3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_fmode3.FormattingEnabled = true;
            this.CMB_fmode3.Name = "CMB_fmode3";
            this.CMB_fmode3.SelectedIndexChanged += new System.EventHandler(this.flightmode_SelectedIndexChanged);
            // 
            // labelfm2
            // 
            resources.ApplyResources(this.labelfm2, "labelfm2");
            this.labelfm2.Name = "labelfm2";
            // 
            // CMB_fmode2
            // 
            this.CMB_fmode2.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CMB_fmode2.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            resources.ApplyResources(this.CMB_fmode2, "CMB_fmode2");
            this.CMB_fmode2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_fmode2.FormattingEnabled = true;
            this.CMB_fmode2.Name = "CMB_fmode2";
            this.CMB_fmode2.SelectedIndexChanged += new System.EventHandler(this.flightmode_SelectedIndexChanged);
            // 
            // labelfm1
            // 
            resources.ApplyResources(this.labelfm1, "labelfm1");
            this.labelfm1.Name = "labelfm1";
            // 
            // CMB_fmode1
            // 
            this.CMB_fmode1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CMB_fmode1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            resources.ApplyResources(this.CMB_fmode1, "CMB_fmode1");
            this.CMB_fmode1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_fmode1.FormattingEnabled = true;
            this.CMB_fmode1.Name = "CMB_fmode1";
            this.CMB_fmode1.SelectedIndexChanged += new System.EventHandler(this.flightmode_SelectedIndexChanged);
            // 
            // BUT_SaveModes
            // 
            resources.ApplyResources(this.BUT_SaveModes, "BUT_SaveModes");
            this.BUT_SaveModes.Name = "BUT_SaveModes";
            this.BUT_SaveModes.UseVisualStyleBackColor = true;
            this.BUT_SaveModes.Click += new System.EventHandler(this.BUT_SaveModes_Click);
            // 
            // chk_ss6
            // 
            resources.ApplyResources(this.chk_ss6, "chk_ss6");
            this.chk_ss6.Name = "chk_ss6";
            this.chk_ss6.UseVisualStyleBackColor = true;
            // 
            // chk_ss5
            // 
            resources.ApplyResources(this.chk_ss5, "chk_ss5");
            this.chk_ss5.Name = "chk_ss5";
            this.chk_ss5.UseVisualStyleBackColor = true;
            // 
            // chk_ss4
            // 
            resources.ApplyResources(this.chk_ss4, "chk_ss4");
            this.chk_ss4.Name = "chk_ss4";
            this.chk_ss4.UseVisualStyleBackColor = true;
            // 
            // chk_ss3
            // 
            resources.ApplyResources(this.chk_ss3, "chk_ss3");
            this.chk_ss3.Name = "chk_ss3";
            this.chk_ss3.UseVisualStyleBackColor = true;
            // 
            // chk_ss2
            // 
            resources.ApplyResources(this.chk_ss2, "chk_ss2");
            this.chk_ss2.Name = "chk_ss2";
            this.chk_ss2.UseVisualStyleBackColor = true;
            // 
            // chk_ss1
            // 
            resources.ApplyResources(this.chk_ss1, "chk_ss1");
            this.chk_ss1.Name = "chk_ss1";
            this.chk_ss1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Controls.Add(this.labelfm1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.chk_ss6, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.CMB_fmode1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.chk_ss5, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.label11, 4, 5);
            this.tableLayoutPanel1.Controls.Add(this.chk_ss4, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.label10, 4, 4);
            this.tableLayoutPanel1.Controls.Add(this.chk_ss1, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label9, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.chk_ss3, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this.label8, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.label12, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this.label7, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.chk_ss2, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.CB_simple5, 2, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelfm2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.CB_simple4, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.CMB_fmode2, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.CB_simple3, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.CB_simple2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelfm3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.labelfm4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.labelfm5, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.labelfm6, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.CMB_fmode3, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.CMB_fmode4, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.CMB_fmode5, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.CMB_fmode6, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.BUT_SaveModes, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.CB_simple1, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.CB_simple6, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.linkLabel1_ss, 3, 6);
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // linkLabel1_ss
            // 
            resources.ApplyResources(this.linkLabel1_ss, "linkLabel1_ss");
            this.linkLabel1_ss.Name = "linkLabel1_ss";
            this.linkLabel1_ss.TabStop = true;
            this.linkLabel1_ss.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_ss_LinkClicked);
            // 
            // ConfigFlightModes
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.LBL_flightmodepwm);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.lbl_currentmode);
            this.Name = "ConfigFlightModes";
            ((System.ComponentModel.ISupportInitialize)(this.currentStateBindingSource)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox CB_simple6;
        private System.Windows.Forms.CheckBox CB_simple5;
        private System.Windows.Forms.CheckBox CB_simple4;
        private System.Windows.Forms.CheckBox CB_simple3;
        private System.Windows.Forms.CheckBox CB_simple2;
        private System.Windows.Forms.CheckBox CB_simple1;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label LBL_flightmodepwm;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label lbl_currentmode;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label labelfm6;
        private System.Windows.Forms.ComboBox CMB_fmode6;
        private System.Windows.Forms.Label labelfm5;
        private System.Windows.Forms.ComboBox CMB_fmode5;
        private System.Windows.Forms.Label labelfm4;
        private System.Windows.Forms.ComboBox CMB_fmode4;
        private System.Windows.Forms.Label labelfm3;
        private System.Windows.Forms.ComboBox CMB_fmode3;
        private System.Windows.Forms.Label labelfm2;
        private System.Windows.Forms.ComboBox CMB_fmode2;
        private System.Windows.Forms.Label labelfm1;
        private System.Windows.Forms.ComboBox CMB_fmode1;
        private Controls.MyButton BUT_SaveModes;
        private System.Windows.Forms.BindingSource currentStateBindingSource;
        private System.Windows.Forms.CheckBox chk_ss6;
        private System.Windows.Forms.CheckBox chk_ss5;
        private System.Windows.Forms.CheckBox chk_ss4;
        private System.Windows.Forms.CheckBox chk_ss3;
        private System.Windows.Forms.CheckBox chk_ss2;
        private System.Windows.Forms.CheckBox chk_ss1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.LinkLabel linkLabel1_ss;
    }
}
