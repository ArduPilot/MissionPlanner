using System;
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
            this.MenuFlightData = new System.Windows.Forms.ToolStripButton();
            this.MenuFlightPlanner = new System.Windows.Forms.ToolStripButton();
            this.MenuHWConfig = new System.Windows.Forms.ToolStripButton();
            this.MenuSWConfig = new System.Windows.Forms.ToolStripButton();
            this.MenuSimulation = new System.Windows.Forms.ToolStripButton();
            this.MenuTerminal = new System.Windows.Forms.ToolStripButton();
            this.MenuHelp = new System.Windows.Forms.ToolStripButton();
            this.MenuConnect = new System.Windows.Forms.ToolStripButton();
            this.toolStripConnectionControl = new Controls.ToolStripConnectionControl();
            this.MenuDonate = new System.Windows.Forms.ToolStripMenuItem();
            this.menu = new Controls.MyButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.MainMenu.SuspendLayout();
            this.CTX_mainmenu.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainMenu
            // 
            this.MainMenu.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.MainMenu.ContextMenuStrip = this.CTX_mainmenu;
            this.MainMenu.GripMargin = new System.Windows.Forms.Padding(0);
            this.MainMenu.ImageScalingSize = new System.Drawing.Size(0, 0);
            this.MainMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuFlightData,
            this.MenuFlightPlanner,
            this.MenuHWConfig,
            this.MenuSWConfig,
            this.MenuSimulation,
            this.MenuTerminal,
            this.MenuHelp,
            this.MenuConnect,
            this.toolStripConnectionControl,
            this.MenuDonate});
            this.MainMenu.Location = new System.Drawing.Point(0, 0);
            this.MainMenu.Name = "MainMenu";
            this.MainMenu.Padding = new System.Windows.Forms.Padding(0, 2, 0, 4);
            this.MainMenu.Size = new System.Drawing.Size(953, 69);
            this.MainMenu.Stretch = false;
            this.MainMenu.TabIndex = 5;
            this.MainMenu.Text = "menuStrip1";
            this.MainMenu.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.MainMenu_ItemClicked);
            this.MainMenu.MouseLeave += new System.EventHandler(this.MainMenu_MouseLeave);
            // 
            // CTX_mainmenu
            // 
            this.CTX_mainmenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoHideToolStripMenuItem});
            this.CTX_mainmenu.Name = "CTX_mainmenu";
            this.CTX_mainmenu.Size = new System.Drawing.Size(126, 26);
            // 
            // autoHideToolStripMenuItem
            // 
            this.autoHideToolStripMenuItem.Checked = true;
            this.autoHideToolStripMenuItem.CheckOnClick = true;
            this.autoHideToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.autoHideToolStripMenuItem.Name = "autoHideToolStripMenuItem";
            this.autoHideToolStripMenuItem.Size = new System.Drawing.Size(125, 22);
            this.autoHideToolStripMenuItem.Text = "AutoHide";
            this.autoHideToolStripMenuItem.Click += new System.EventHandler(this.autoHideToolStripMenuItem_Click);
            // 
            // MenuFlightData
            // 
            this.MenuFlightData.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuFlightData.ForeColor = System.Drawing.Color.White;
            this.MenuFlightData.Image = ((System.Drawing.Image)(resources.GetObject("MenuFlightData.Image")));
            this.MenuFlightData.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MenuFlightData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuFlightData.Margin = new System.Windows.Forms.Padding(0);
            this.MenuFlightData.Name = "MenuFlightData";
            this.MenuFlightData.Size = new System.Drawing.Size(76, 63);
            this.MenuFlightData.Text = "FLIGHT DATA";
            this.MenuFlightData.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.MenuFlightData.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.MenuFlightData.ToolTipText = "Flight Data";
            this.MenuFlightData.Click += new System.EventHandler(this.MenuFlightData_Click);
            // 
            // MenuFlightPlanner
            // 
            this.MenuFlightPlanner.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuFlightPlanner.ForeColor = System.Drawing.Color.White;
            this.MenuFlightPlanner.Image = ((System.Drawing.Image)(resources.GetObject("MenuFlightPlanner.Image")));
            this.MenuFlightPlanner.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MenuFlightPlanner.ImageTransparentColor = System.Drawing.Color.White;
            this.MenuFlightPlanner.Margin = new System.Windows.Forms.Padding(0);
            this.MenuFlightPlanner.Name = "MenuFlightPlanner";
            this.MenuFlightPlanner.Size = new System.Drawing.Size(76, 63);
            this.MenuFlightPlanner.Text = "FLIGHT PLAN";
            this.MenuFlightPlanner.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.MenuFlightPlanner.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.MenuFlightPlanner.ToolTipText = "Flight Planner";
            this.MenuFlightPlanner.Click += new System.EventHandler(this.MenuFlightPlanner_Click);
            // 
            // MenuHWConfig
            // 
            this.MenuHWConfig.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuHWConfig.ForeColor = System.Drawing.Color.White;
            this.MenuHWConfig.Image = ((System.Drawing.Image)(resources.GetObject("MenuHWConfig.Image")));
            this.MenuHWConfig.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MenuHWConfig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuHWConfig.Margin = new System.Windows.Forms.Padding(0);
            this.MenuHWConfig.Name = "MenuHWConfig";
            this.MenuHWConfig.Size = new System.Drawing.Size(79, 63);
            this.MenuHWConfig.Text = "INITIAL SETUP";
            this.MenuHWConfig.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.MenuHWConfig.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.MenuHWConfig.ToolTipText = "Hardware Config";
            this.MenuHWConfig.Click += new System.EventHandler(this.MenuSetup_Click);
            // 
            // MenuSWConfig
            // 
            this.MenuSWConfig.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuSWConfig.ForeColor = System.Drawing.Color.White;
            this.MenuSWConfig.Image = ((System.Drawing.Image)(resources.GetObject("MenuSWConfig.Image")));
            this.MenuSWConfig.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MenuSWConfig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuSWConfig.Margin = new System.Windows.Forms.Padding(0);
            this.MenuSWConfig.Name = "MenuSWConfig";
            this.MenuSWConfig.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.MenuSWConfig.Size = new System.Drawing.Size(94, 63);
            this.MenuSWConfig.Text = "CONFIG/TUNING";
            this.MenuSWConfig.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.MenuSWConfig.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.MenuSWConfig.ToolTipText = "Software Config";
            this.MenuSWConfig.Click += new System.EventHandler(this.MenuTuning_Click);
            // 
            // MenuSimulation
            // 
            this.MenuSimulation.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuSimulation.ForeColor = System.Drawing.Color.White;
            this.MenuSimulation.Image = ((System.Drawing.Image)(resources.GetObject("MenuSimulation.Image")));
            this.MenuSimulation.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MenuSimulation.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuSimulation.Margin = new System.Windows.Forms.Padding(0);
            this.MenuSimulation.Name = "MenuSimulation";
            this.MenuSimulation.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.MenuSimulation.Size = new System.Drawing.Size(81, 63);
            this.MenuSimulation.Text = "SIMULATION";
            this.MenuSimulation.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.MenuSimulation.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.MenuSimulation.ToolTipText = "Simulation";
            this.MenuSimulation.Click += new System.EventHandler(this.MenuSimulation_Click);
            // 
            // MenuTerminal
            // 
            this.MenuTerminal.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuTerminal.ForeColor = System.Drawing.Color.White;
            this.MenuTerminal.Image = ((System.Drawing.Image)(resources.GetObject("MenuTerminal.Image")));
            this.MenuTerminal.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MenuTerminal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuTerminal.Margin = new System.Windows.Forms.Padding(0);
            this.MenuTerminal.Name = "MenuTerminal";
            this.MenuTerminal.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.MenuTerminal.Size = new System.Drawing.Size(71, 63);
            this.MenuTerminal.Text = "TERMINAL";
            this.MenuTerminal.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.MenuTerminal.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.MenuTerminal.ToolTipText = "Terminal";
            this.MenuTerminal.Click += new System.EventHandler(this.MenuTerminal_Click);
            // 
            // MenuHelp
            // 
            this.MenuHelp.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuHelp.ForeColor = System.Drawing.Color.White;
            this.MenuHelp.Image = ((System.Drawing.Image)(resources.GetObject("MenuHelp.Image")));
            this.MenuHelp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MenuHelp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuHelp.Margin = new System.Windows.Forms.Padding(0);
            this.MenuHelp.Name = "MenuHelp";
            this.MenuHelp.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.MenuHelp.Size = new System.Drawing.Size(68, 63);
            this.MenuHelp.Text = "HELP";
            this.MenuHelp.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.MenuHelp.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.MenuHelp.ToolTipText = "Terminal";
            this.MenuHelp.Click += new System.EventHandler(this.MenuHelp_Click);
            // 
            // MenuConnect
            // 
            this.MenuConnect.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.MenuConnect.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuConnect.ForeColor = System.Drawing.Color.White;
            this.MenuConnect.Image = ((System.Drawing.Image)(resources.GetObject("MenuConnect.Image")));
            this.MenuConnect.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MenuConnect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.MenuConnect.Margin = new System.Windows.Forms.Padding(0);
            this.MenuConnect.Name = "MenuConnect";
            this.MenuConnect.Size = new System.Drawing.Size(66, 63);
            this.MenuConnect.Text = "CONNECT";
            this.MenuConnect.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.MenuConnect.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.MenuConnect.Click += new System.EventHandler(this.MenuConnect_Click);
            // 
            // toolStripConnectionControl
            // 
            this.toolStripConnectionControl.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripConnectionControl.BackgroundImage = global::MissionPlanner.Properties.Resources.bgdark;
            this.toolStripConnectionControl.Font = new System.Drawing.Font("Arial Narrow", 8.25F);
            this.toolStripConnectionControl.ForeColor = System.Drawing.Color.Black;
            this.toolStripConnectionControl.Margin = new System.Windows.Forms.Padding(0);
            this.toolStripConnectionControl.Name = "toolStripConnectionControl";
            this.toolStripConnectionControl.Padding = new System.Windows.Forms.Padding(0, 0, 60, 0);
            this.toolStripConnectionControl.Size = new System.Drawing.Size(229, 63);
            this.toolStripConnectionControl.MouseLeave += new System.EventHandler(this.MainMenu_MouseLeave);
            // 
            // MenuDonate
            // 
            this.MenuDonate.Font = new System.Drawing.Font("Arial", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MenuDonate.ForeColor = System.Drawing.Color.White;
            this.MenuDonate.Image = global::MissionPlanner.Properties.Resources.donate;
            this.MenuDonate.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.MenuDonate.Name = "MenuDonate";
            this.MenuDonate.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.MenuDonate.Size = new System.Drawing.Size(64, 63);
            this.MenuDonate.Text = "DONATE";
            this.MenuDonate.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.MenuDonate.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.MenuDonate.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // menu
            // 
            this.menu.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.menu.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.menu.Dock = System.Windows.Forms.DockStyle.Top;
            this.menu.Location = new System.Drawing.Point(0, 0);
            this.menu.Margin = new System.Windows.Forms.Padding(4);
            this.menu.Name = "menu";
            this.menu.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.menu.Size = new System.Drawing.Size(1008, 28);
            this.menu.TabIndex = 6;
            this.menu.Text = "Menu";
            this.menu.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.menu.UseVisualStyleBackColor = true;
            this.menu.MouseEnter += new System.EventHandler(this.menu_MouseEnter);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.MainMenu);
            this.panel1.Location = new System.Drawing.Point(9, 57);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.MaximumSize = new System.Drawing.Size(133332, 123);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(953, 70);
            this.panel1.TabIndex = 7;
            this.panel1.Visible = false;
            this.panel1.MouseLeave += new System.EventHandler(this.MainMenu_MouseLeave);
            // 
            // MainV2
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1008, 537);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menu);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.MainMenu;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimumSize = new System.Drawing.Size(1024, 575);
            this.Name = "MainV2";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Mission Planner - By Michael Oborne";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainV2_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainV2_FormClosed);
            this.Load += new System.EventHandler(this.MainV2_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainV2_KeyDown);
            this.Resize += new System.EventHandler(this.MainV2_Resize);
            this.MainMenu.ResumeLayout(false);
            this.MainMenu.PerformLayout();
            this.CTX_mainmenu.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStripButton MenuFlightData;
        private System.Windows.Forms.ToolStripButton MenuFlightPlanner;
        private System.Windows.Forms.ToolStripButton MenuHWConfig;
        private System.Windows.Forms.ToolStripButton MenuSimulation;
        private System.Windows.Forms.ToolStripButton MenuSWConfig;
        private System.Windows.Forms.ToolStripButton MenuTerminal;
        public System.Windows.Forms.ToolStripButton MenuConnect;

        private System.Windows.Forms.ToolStripButton MenuHelp;
        private Controls.ToolStripConnectionControl toolStripConnectionControl;
        private Controls.MyButton menu;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ContextMenuStrip CTX_mainmenu;
        private System.Windows.Forms.ToolStripMenuItem autoHideToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem MenuDonate;
        public System.Windows.Forms.MenuStrip MainMenu;
    }
}