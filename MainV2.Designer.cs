using System;
using System.IO;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner
{
    partial class MainV2
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
            Console.WriteLine("mainv2_Dispose");
            if (PluginThreadrunner != null)
                PluginThreadrunner.Dispose();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainV2));
            this.MainMenu = new System.Windows.Forms.MenuStrip();
            this.CTX_mainmenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.autoHideToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fullScreenToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.readonlyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionOptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectionListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuFlightData = new System.Windows.Forms.ToolStripButton();
            this.MenuFlightPlanner = new System.Windows.Forms.ToolStripButton();
            this.MenuInitConfig = new System.Windows.Forms.ToolStripButton();
            this.MenuConfigTune = new System.Windows.Forms.ToolStripButton();
            this.MenuSimulation = new System.Windows.Forms.ToolStripButton();
            this.MenuHelp = new System.Windows.Forms.ToolStripButton();
            this.MenuConnect = new System.Windows.Forms.ToolStripButton();
            this.MenuArduPilot = new System.Windows.Forms.ToolStripButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.p1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.p2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.p3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.p4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton3 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton4 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton5 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton6 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton7 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton8 = new System.Windows.Forms.ToolStripButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.verticalProgressBar2 = new MissionPlanner.Controls.VerticalProgressBar();
            this.verticalProgressBar1 = new MissionPlanner.Controls.VerticalProgressBar();
            this.menu = new MissionPlanner.Controls.MyButton();
            this.verticalProgressBar3 = new MissionPlanner.Controls.VerticalProgressBar();
            this.verticalProgressBar4 = new MissionPlanner.Controls.VerticalProgressBar();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.verticalProgressBar5 = new MissionPlanner.Controls.VerticalProgressBar();
            this.verticalProgressBar6 = new MissionPlanner.Controls.VerticalProgressBar();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.myButton1 = new MissionPlanner.Controls.MyButton();
            this.myButton2 = new MissionPlanner.Controls.MyButton();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.myButton3 = new MissionPlanner.Controls.MyButton();
            this.status1 = new MissionPlanner.Controls.Status();
            this.toolStripConnectionControl = new MissionPlanner.Controls.ToolStripConnectionControl();
            this.toolStripConnectionControl1 = new MissionPlanner.Controls.ToolStripConnectionControl();
            this.MainMenu.SuspendLayout();
            this.CTX_mainmenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            resources.ApplyResources(this.MainMenu, "MainMenu");
            this.MainMenu.ContextMenuStrip = this.CTX_mainmenu;
            this.MainMenu.GripMargin = new System.Windows.Forms.Padding(0);
            this.MainMenu.ImageScalingSize = new System.Drawing.Size(45, 39);
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFlightData,
            this.MenuFlightPlanner,
            this.MenuInitConfig,
            this.MenuConfigTune,
            this.MenuSimulation,
            this.MenuHelp,
            this.MenuConnect,
            this.toolStripConnectionControl,
            this.MenuArduPilot});
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.ShowItemToolTips = true;
            this.MainMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MainMenu_ItemClicked);
            this.MainMenu.MouseLeave += new System.EventHandler(this.MainMenu_MouseLeave);
            // 
            // CTX_mainmenu
            // 
            this.CTX_mainmenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoHideToolStripMenuItem,
            this.fullScreenToolStripMenuItem,
            this.readonlyToolStripMenuItem,
            this.connectionOptionsToolStripMenuItem,
            this.connectionListToolStripMenuItem});
            this.CTX_mainmenu.Name = "CTX_mainmenu";
            resources.ApplyResources(this.CTX_mainmenu, "CTX_mainmenu");
            // 
            // autoHideToolStripMenuItem
            // 
            this.autoHideToolStripMenuItem.CheckOnClick = true;
            this.autoHideToolStripMenuItem.Name = "autoHideToolStripMenuItem";
            resources.ApplyResources(this.autoHideToolStripMenuItem, "autoHideToolStripMenuItem");
            this.autoHideToolStripMenuItem.Click += new System.EventHandler(this.autoHideToolStripMenuItem_Click);
            // 
            // fullScreenToolStripMenuItem
            // 
            this.fullScreenToolStripMenuItem.CheckOnClick = true;
            this.fullScreenToolStripMenuItem.Name = "fullScreenToolStripMenuItem";
            resources.ApplyResources(this.fullScreenToolStripMenuItem, "fullScreenToolStripMenuItem");
            this.fullScreenToolStripMenuItem.Click += new System.EventHandler(this.fullScreenToolStripMenuItem_Click);
            // 
            // readonlyToolStripMenuItem
            // 
            this.readonlyToolStripMenuItem.CheckOnClick = true;
            this.readonlyToolStripMenuItem.Name = "readonlyToolStripMenuItem";
            resources.ApplyResources(this.readonlyToolStripMenuItem, "readonlyToolStripMenuItem");
            this.readonlyToolStripMenuItem.Click += new System.EventHandler(this.readonlyToolStripMenuItem_Click);
            // 
            // connectionOptionsToolStripMenuItem
            // 
            this.connectionOptionsToolStripMenuItem.Name = "connectionOptionsToolStripMenuItem";
            resources.ApplyResources(this.connectionOptionsToolStripMenuItem, "connectionOptionsToolStripMenuItem");
            this.connectionOptionsToolStripMenuItem.Click += new System.EventHandler(this.connectionOptionsToolStripMenuItem_Click);
            // 
            // connectionListToolStripMenuItem
            // 
            this.connectionListToolStripMenuItem.Name = "connectionListToolStripMenuItem";
            resources.ApplyResources(this.connectionListToolStripMenuItem, "connectionListToolStripMenuItem");
            this.connectionListToolStripMenuItem.Click += new System.EventHandler(this.connectionListToolStripMenuItem_Click);
            // 
            // MenuFlightData
            // 
            this.MenuFlightData.ForeColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.MenuFlightData, "MenuFlightData");
            this.MenuFlightData.Margin = new System.Windows.Forms.Padding(0);
            this.MenuFlightData.Name = "MenuFlightData";
            this.MenuFlightData.Click += new System.EventHandler(this.MenuFlightData_Click);
            // 
            // MenuFlightPlanner
            // 
            this.MenuFlightPlanner.ForeColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.MenuFlightPlanner, "MenuFlightPlanner");
            this.MenuFlightPlanner.Margin = new System.Windows.Forms.Padding(0);
            this.MenuFlightPlanner.Name = "MenuFlightPlanner";
            this.MenuFlightPlanner.Click += new System.EventHandler(this.MenuFlightPlanner_Click);
            // 
            // MenuInitConfig
            // 
            this.MenuInitConfig.ForeColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.MenuInitConfig, "MenuInitConfig");
            this.MenuInitConfig.Margin = new System.Windows.Forms.Padding(0);
            this.MenuInitConfig.Name = "MenuInitConfig";
            this.MenuInitConfig.Click += new System.EventHandler(this.MenuSetup_Click);
            // 
            // MenuConfigTune
            // 
            this.MenuConfigTune.ForeColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.MenuConfigTune, "MenuConfigTune");
            this.MenuConfigTune.Margin = new System.Windows.Forms.Padding(0);
            this.MenuConfigTune.Name = "MenuConfigTune";
            this.MenuConfigTune.Click += new System.EventHandler(this.MenuTuning_Click);
            // 
            // MenuSimulation
            // 
            this.MenuSimulation.ForeColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.MenuSimulation, "MenuSimulation");
            this.MenuSimulation.Margin = new System.Windows.Forms.Padding(0);
            this.MenuSimulation.Name = "MenuSimulation";
            this.MenuSimulation.Click += new System.EventHandler(this.MenuSimulation_Click);
            // 
            // MenuHelp
            // 
            this.MenuHelp.ForeColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.MenuHelp, "MenuHelp");
            this.MenuHelp.Margin = new System.Windows.Forms.Padding(0);
            this.MenuHelp.Name = "MenuHelp";
            this.MenuHelp.Click += new System.EventHandler(this.MenuHelp_Click);
            // 
            // MenuConnect
            // 
            this.MenuConnect.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.MenuConnect.ForeColor = System.Drawing.SystemColors.ControlLight;
            resources.ApplyResources(this.MenuConnect, "MenuConnect");
            this.MenuConnect.Margin = new System.Windows.Forms.Padding(0);
            this.MenuConnect.Name = "MenuConnect";
            this.MenuConnect.Click += new System.EventHandler(this.MenuConnect_Click);
            // 
            // MenuArduPilot
            // 
            this.MenuArduPilot.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.MenuArduPilot, "MenuArduPilot");
            this.MenuArduPilot.BackColor = System.Drawing.Color.Transparent;
            this.MenuArduPilot.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.MenuArduPilot.ForeColor = System.Drawing.Color.White;
            this.MenuArduPilot.Margin = new System.Windows.Forms.Padding(0);
            this.MenuArduPilot.Name = "MenuArduPilot";
            this.MenuArduPilot.Click += new System.EventHandler(this.MenuArduPilot_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.myButton3);
            this.panel1.Controls.Add(this.myButton2);
            this.panel1.Controls.Add(this.myButton1);
            this.panel1.Controls.Add(this.label12);
            this.panel1.Controls.Add(this.label11);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.verticalProgressBar6);
            this.panel1.Controls.Add(this.verticalProgressBar5);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.verticalProgressBar4);
            this.panel1.Controls.Add(this.verticalProgressBar3);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.verticalProgressBar2);
            this.panel1.Controls.Add(this.verticalProgressBar1);
            this.panel1.Controls.Add(this.menuStrip1);
            this.panel1.Controls.Add(this.status1);
            this.panel1.Controls.Add(this.MainMenu);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            this.panel1.MouseLeave += new System.EventHandler(this.MainMenu_MouseLeave);
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.ContextMenuStrip = this.CTX_mainmenu;
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(45, 39);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.p1ToolStripMenuItem,
            this.p2ToolStripMenuItem,
            this.p3ToolStripMenuItem,
            this.p4ToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.ShowItemToolTips = true;
            // 
            // p1ToolStripMenuItem
            // 
            this.p1ToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.p1ToolStripMenuItem.Name = "p1ToolStripMenuItem";
            resources.ApplyResources(this.p1ToolStripMenuItem, "p1ToolStripMenuItem");
            // 
            // p2ToolStripMenuItem
            // 
            this.p2ToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.p2ToolStripMenuItem.Name = "p2ToolStripMenuItem";
            resources.ApplyResources(this.p2ToolStripMenuItem, "p2ToolStripMenuItem");
            // 
            // p3ToolStripMenuItem
            // 
            this.p3ToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.p3ToolStripMenuItem.Name = "p3ToolStripMenuItem";
            resources.ApplyResources(this.p3ToolStripMenuItem, "p3ToolStripMenuItem");
            // 
            // p4ToolStripMenuItem
            // 
            this.p4ToolStripMenuItem.ForeColor = System.Drawing.SystemColors.ControlLightLight;
            this.p4ToolStripMenuItem.Name = "p4ToolStripMenuItem";
            resources.ApplyResources(this.p4ToolStripMenuItem, "p4ToolStripMenuItem");
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Name = "toolStripButton1";
            resources.ApplyResources(this.toolStripButton1, "toolStripButton1");
            // 
            // toolStripButton3
            // 
            this.toolStripButton3.Name = "toolStripButton3";
            resources.ApplyResources(this.toolStripButton3, "toolStripButton3");
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.Name = "toolStripButton2";
            resources.ApplyResources(this.toolStripButton2, "toolStripButton2");
            // 
            // toolStripButton4
            // 
            this.toolStripButton4.Name = "toolStripButton4";
            resources.ApplyResources(this.toolStripButton4, "toolStripButton4");
            // 
            // toolStripButton5
            // 
            this.toolStripButton5.Name = "toolStripButton5";
            resources.ApplyResources(this.toolStripButton5, "toolStripButton5");
            // 
            // toolStripButton6
            // 
            this.toolStripButton6.Name = "toolStripButton6";
            resources.ApplyResources(this.toolStripButton6, "toolStripButton6");
            // 
            // toolStripButton7
            // 
            this.toolStripButton7.Name = "toolStripButton7";
            resources.ApplyResources(this.toolStripButton7, "toolStripButton7");
            // 
            // toolStripButton8
            // 
            this.toolStripButton8.Name = "toolStripButton8";
            resources.ApplyResources(this.toolStripButton8, "toolStripButton8");
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // verticalProgressBar2
            // 
            this.verticalProgressBar2.DrawLabel = true;
            resources.ApplyResources(this.verticalProgressBar2, "verticalProgressBar2");
            this.verticalProgressBar2.Label = null;
            this.verticalProgressBar2.maxline = 0;
            this.verticalProgressBar2.minline = 0;
            this.verticalProgressBar2.Name = "verticalProgressBar2";
            this.verticalProgressBar2.Value = 20;
            // 
            // verticalProgressBar1
            // 
            this.verticalProgressBar1.DrawLabel = true;
            resources.ApplyResources(this.verticalProgressBar1, "verticalProgressBar1");
            this.verticalProgressBar1.Label = null;
            this.verticalProgressBar1.maxline = 0;
            this.verticalProgressBar1.minline = 0;
            this.verticalProgressBar1.Name = "verticalProgressBar1";
            this.verticalProgressBar1.Value = 20;
            // 
            // menu
            // 
            resources.ApplyResources(this.menu, "menu");
            this.menu.Name = "menu";
            this.menu.UseVisualStyleBackColor = true;
            this.menu.MouseEnter += new System.EventHandler(this.menu_MouseEnter);
            // 
            // verticalProgressBar3
            // 
            this.verticalProgressBar3.DrawLabel = true;
            resources.ApplyResources(this.verticalProgressBar3, "verticalProgressBar3");
            this.verticalProgressBar3.Label = null;
            this.verticalProgressBar3.maxline = 0;
            this.verticalProgressBar3.minline = 0;
            this.verticalProgressBar3.Name = "verticalProgressBar3";
            this.verticalProgressBar3.Value = 20;
            // 
            // verticalProgressBar4
            // 
            this.verticalProgressBar4.DrawLabel = true;
            resources.ApplyResources(this.verticalProgressBar4, "verticalProgressBar4");
            this.verticalProgressBar4.Label = null;
            this.verticalProgressBar4.maxline = 0;
            this.verticalProgressBar4.minline = 0;
            this.verticalProgressBar4.Name = "verticalProgressBar4";
            this.verticalProgressBar4.Value = 20;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
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
            // verticalProgressBar5
            // 
            this.verticalProgressBar5.DrawLabel = true;
            resources.ApplyResources(this.verticalProgressBar5, "verticalProgressBar5");
            this.verticalProgressBar5.Label = null;
            this.verticalProgressBar5.maxline = 0;
            this.verticalProgressBar5.minline = 0;
            this.verticalProgressBar5.Name = "verticalProgressBar5";
            this.verticalProgressBar5.Value = 20;
            // 
            // verticalProgressBar6
            // 
            this.verticalProgressBar6.DrawLabel = true;
            resources.ApplyResources(this.verticalProgressBar6, "verticalProgressBar6");
            this.verticalProgressBar6.Label = null;
            this.verticalProgressBar6.maxline = 0;
            this.verticalProgressBar6.minline = 0;
            this.verticalProgressBar6.Name = "verticalProgressBar6";
            this.verticalProgressBar6.Value = 20;
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
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // myButton1
            // 
            resources.ApplyResources(this.myButton1, "myButton1");
            this.myButton1.Name = "myButton1";
            this.myButton1.UseVisualStyleBackColor = true;
            // 
            // myButton2
            // 
            resources.ApplyResources(this.myButton2, "myButton2");
            this.myButton2.Name = "myButton2";
            this.myButton2.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // myButton3
            // 
            resources.ApplyResources(this.myButton3, "myButton3");
            this.myButton3.Name = "myButton3";
            this.myButton3.UseVisualStyleBackColor = true;
            this.myButton3.Click += new System.EventHandler(this.myButton3_Click);
            // 
            // status1
            // 
            resources.ApplyResources(this.status1, "status1");
            this.status1.Name = "status1";
            this.status1.Percent = 0D;
            // 
            // toolStripConnectionControl
            // 
            this.toolStripConnectionControl.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.toolStripConnectionControl, "toolStripConnectionControl");
            this.toolStripConnectionControl.ForeColor = System.Drawing.Color.Black;
            this.toolStripConnectionControl.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripConnectionControl.Name = "toolStripConnectionControl";
            this.toolStripConnectionControl.MouseLeave += new System.EventHandler(this.MainMenu_MouseLeave);
            // 
            // toolStripConnectionControl1
            // 
            resources.ApplyResources(this.toolStripConnectionControl1, "toolStripConnectionControl1");
            this.toolStripConnectionControl1.Name = "toolStripConnectionControl1";
            // 
            // MainV2
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menu);
            this.KeyPreview = true;
            this.MainMenuStrip = this.MainMenu;
            this.Name = "MainV2";
            this.Load += new System.EventHandler(this.MainV2_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainV2_KeyDown);
            this.Resize += new System.EventHandler(this.MainV2_Resize);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.CTX_mainmenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.ToolStripMenuItem autoHideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionListToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectionOptionsToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip CTX_mainmenu;
        private System.Windows.Forms.ToolStripMenuItem fullScreenToolStripMenuItem;
        public System.Windows.Forms.MenuStrip MainMenu;
        private MissionPlanner.Controls.MyButton menu;
        public System.Windows.Forms.ToolStripButton MenuArduPilot;
        public System.Windows.Forms.ToolStripButton MenuConfigTune;
        public System.Windows.Forms.ToolStripButton MenuConnect;
        public System.Windows.Forms.ToolStripButton MenuFlightData;
        public System.Windows.Forms.ToolStripButton MenuFlightPlanner;
        public System.Windows.Forms.ToolStripButton MenuHelp;
        public System.Windows.Forms.ToolStripButton MenuInitConfig;
        public System.Windows.Forms.ToolStripButton MenuSimulation;
        public System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem p1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem p2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem p3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem p4ToolStripMenuItem;
        public System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem readonlyToolStripMenuItem;
        public MissionPlanner.Controls.Status status1;
        public System.Windows.Forms.ToolStripButton toolStripButton1;
        public System.Windows.Forms.ToolStripButton toolStripButton2;
        public System.Windows.Forms.ToolStripButton toolStripButton3;
        public System.Windows.Forms.ToolStripButton toolStripButton4;
        public System.Windows.Forms.ToolStripButton toolStripButton5;
        public System.Windows.Forms.ToolStripButton toolStripButton6;
        public System.Windows.Forms.ToolStripButton toolStripButton7;
        public System.Windows.Forms.ToolStripButton toolStripButton8;
        private MissionPlanner.Controls.ToolStripConnectionControl toolStripConnectionControl;
        private MissionPlanner.Controls.ToolStripConnectionControl toolStripConnectionControl1;

        #endregion

        private Label label3;
        private Label label2;
        private Label label1;
        private VerticalProgressBar verticalProgressBar2;
        private VerticalProgressBar verticalProgressBar1;
        private Label label12;
        private Label label11;
        private Label label10;
        private Label label9;
        private Label label8;
        private VerticalProgressBar verticalProgressBar6;
        private VerticalProgressBar verticalProgressBar5;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private VerticalProgressBar verticalProgressBar4;
        private VerticalProgressBar verticalProgressBar3;
        private MyButton myButton2;
        private MyButton myButton1;
        private Label label13;
        private Label label14;
        private MyButton myButton3;
    }
}