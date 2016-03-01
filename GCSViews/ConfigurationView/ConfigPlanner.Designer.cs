namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigPlanner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigPlanner));
            this.label33 = new System.Windows.Forms.Label();
            this.CMB_ratesensors = new System.Windows.Forms.ComboBox();
            this.label26 = new System.Windows.Forms.Label();
            this.CMB_videoresolutions = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.CHK_GDIPlus = new System.Windows.Forms.CheckBox();
            this.label24 = new System.Windows.Forms.Label();
            this.CHK_loadwponconnect = new System.Windows.Forms.CheckBox();
            this.label23 = new System.Windows.Forms.Label();
            this.NUM_tracklength = new System.Windows.Forms.NumericUpDown();
            this.CHK_speechaltwarning = new System.Windows.Forms.CheckBox();
            this.label108 = new System.Windows.Forms.Label();
            this.CHK_resetapmonconnect = new System.Windows.Forms.CheckBox();
            this.CHK_mavdebug = new System.Windows.Forms.CheckBox();
            this.label107 = new System.Windows.Forms.Label();
            this.CMB_raterc = new System.Windows.Forms.ComboBox();
            this.label104 = new System.Windows.Forms.Label();
            this.label103 = new System.Windows.Forms.Label();
            this.label102 = new System.Windows.Forms.Label();
            this.label101 = new System.Windows.Forms.Label();
            this.CMB_ratestatus = new System.Windows.Forms.ComboBox();
            this.CMB_rateposition = new System.Windows.Forms.ComboBox();
            this.CMB_rateattitude = new System.Windows.Forms.ComboBox();
            this.label99 = new System.Windows.Forms.Label();
            this.label98 = new System.Windows.Forms.Label();
            this.label97 = new System.Windows.Forms.Label();
            this.CMB_speedunits = new System.Windows.Forms.ComboBox();
            this.CMB_distunits = new System.Windows.Forms.ComboBox();
            this.label96 = new System.Windows.Forms.Label();
            this.label95 = new System.Windows.Forms.Label();
            this.CHK_speechbattery = new System.Windows.Forms.CheckBox();
            this.CHK_speechcustom = new System.Windows.Forms.CheckBox();
            this.CHK_speechmode = new System.Windows.Forms.CheckBox();
            this.CHK_speechwaypoint = new System.Windows.Forms.CheckBox();
            this.label94 = new System.Windows.Forms.Label();
            this.CMB_osdcolor = new System.Windows.Forms.ComboBox();
            this.CMB_language = new System.Windows.Forms.ComboBox();
            this.label93 = new System.Windows.Forms.Label();
            this.CHK_enablespeech = new System.Windows.Forms.CheckBox();
            this.CHK_hudshow = new System.Windows.Forms.CheckBox();
            this.label92 = new System.Windows.Forms.Label();
            this.CMB_videosources = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.CHK_maprotation = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.CHK_disttohomeflightdata = new System.Windows.Forms.CheckBox();
            this.BUT_Joystick = new MissionPlanner.Controls.MyButton();
            this.BUT_videostop = new MissionPlanner.Controls.MyButton();
            this.BUT_videostart = new MissionPlanner.Controls.MyButton();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_log_dir = new System.Windows.Forms.TextBox();
            this.BUT_logdirbrowse = new MissionPlanner.Controls.MyButton();
            this.label4 = new System.Windows.Forms.Label();
            this.CMB_theme = new System.Windows.Forms.ComboBox();
            this.BUT_themecustom = new MissionPlanner.Controls.MyButton();
            this.CHK_speecharmdisarm = new System.Windows.Forms.CheckBox();
            this.BUT_Vario = new MissionPlanner.Controls.MyButton();
            this.label5 = new System.Windows.Forms.Label();
            this.chk_analytics = new System.Windows.Forms.CheckBox();
            this.CHK_beta = new System.Windows.Forms.CheckBox();
            this.CHK_Password = new System.Windows.Forms.CheckBox();
            this.CHK_speechlowspeed = new System.Windows.Forms.CheckBox();
            this.CHK_advancedview = new System.Windows.Forms.CheckBox();
            this.CHK_showairports = new System.Windows.Forms.CheckBox();
            this.chk_ADSB = new System.Windows.Forms.CheckBox();
            this.chk_tfr = new System.Windows.Forms.CheckBox();
            this.chk_temp = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_tracklength)).BeginInit();
            this.SuspendLayout();
            // 
            // label33
            // 
            resources.ApplyResources(this.label33, "label33");
            this.label33.Name = "label33";
            // 
            // CMB_ratesensors
            // 
            this.CMB_ratesensors.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_ratesensors.FormattingEnabled = true;
            this.CMB_ratesensors.Items.AddRange(new object[] {
            resources.GetString("CMB_ratesensors.Items"),
            resources.GetString("CMB_ratesensors.Items1"),
            resources.GetString("CMB_ratesensors.Items2"),
            resources.GetString("CMB_ratesensors.Items3"),
            resources.GetString("CMB_ratesensors.Items4"),
            resources.GetString("CMB_ratesensors.Items5"),
            resources.GetString("CMB_ratesensors.Items6"),
            resources.GetString("CMB_ratesensors.Items7"),
            resources.GetString("CMB_ratesensors.Items8"),
            resources.GetString("CMB_ratesensors.Items9"),
            resources.GetString("CMB_ratesensors.Items10"),
            resources.GetString("CMB_ratesensors.Items11")});
            resources.ApplyResources(this.CMB_ratesensors, "CMB_ratesensors");
            this.CMB_ratesensors.Name = "CMB_ratesensors";
            this.CMB_ratesensors.SelectedIndexChanged += new System.EventHandler(this.CMB_ratesensors_SelectedIndexChanged);
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // CMB_videoresolutions
            // 
            this.CMB_videoresolutions.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_videoresolutions.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_videoresolutions, "CMB_videoresolutions");
            this.CMB_videoresolutions.Name = "CMB_videoresolutions";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // CHK_GDIPlus
            // 
            resources.ApplyResources(this.CHK_GDIPlus, "CHK_GDIPlus");
            this.CHK_GDIPlus.Name = "CHK_GDIPlus";
            this.CHK_GDIPlus.UseVisualStyleBackColor = true;
            this.CHK_GDIPlus.CheckedChanged += new System.EventHandler(this.CHK_GDIPlus_CheckedChanged);
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // CHK_loadwponconnect
            // 
            resources.ApplyResources(this.CHK_loadwponconnect, "CHK_loadwponconnect");
            this.CHK_loadwponconnect.Name = "CHK_loadwponconnect";
            this.CHK_loadwponconnect.UseVisualStyleBackColor = true;
            this.CHK_loadwponconnect.CheckedChanged += new System.EventHandler(this.CHK_loadwponconnect_CheckedChanged);
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // NUM_tracklength
            // 
            this.NUM_tracklength.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            resources.ApplyResources(this.NUM_tracklength, "NUM_tracklength");
            this.NUM_tracklength.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.NUM_tracklength.Minimum = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.NUM_tracklength.Name = "NUM_tracklength";
            this.NUM_tracklength.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.NUM_tracklength.ValueChanged += new System.EventHandler(this.NUM_tracklength_ValueChanged);
            // 
            // CHK_speechaltwarning
            // 
            resources.ApplyResources(this.CHK_speechaltwarning, "CHK_speechaltwarning");
            this.CHK_speechaltwarning.Name = "CHK_speechaltwarning";
            this.CHK_speechaltwarning.UseVisualStyleBackColor = true;
            this.CHK_speechaltwarning.CheckedChanged += new System.EventHandler(this.CHK_speechaltwarning_CheckedChanged);
            // 
            // label108
            // 
            resources.ApplyResources(this.label108, "label108");
            this.label108.Name = "label108";
            // 
            // CHK_resetapmonconnect
            // 
            this.CHK_resetapmonconnect.Checked = true;
            this.CHK_resetapmonconnect.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.CHK_resetapmonconnect, "CHK_resetapmonconnect");
            this.CHK_resetapmonconnect.Name = "CHK_resetapmonconnect";
            this.CHK_resetapmonconnect.UseVisualStyleBackColor = true;
            this.CHK_resetapmonconnect.CheckedChanged += new System.EventHandler(this.CHK_resetapmonconnect_CheckedChanged);
            // 
            // CHK_mavdebug
            // 
            resources.ApplyResources(this.CHK_mavdebug, "CHK_mavdebug");
            this.CHK_mavdebug.Name = "CHK_mavdebug";
            this.CHK_mavdebug.UseVisualStyleBackColor = true;
            this.CHK_mavdebug.CheckedChanged += new System.EventHandler(this.CHK_mavdebug_CheckedChanged);
            // 
            // label107
            // 
            resources.ApplyResources(this.label107, "label107");
            this.label107.Name = "label107";
            // 
            // CMB_raterc
            // 
            this.CMB_raterc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_raterc.FormattingEnabled = true;
            this.CMB_raterc.Items.AddRange(new object[] {
            resources.GetString("CMB_raterc.Items"),
            resources.GetString("CMB_raterc.Items1"),
            resources.GetString("CMB_raterc.Items2"),
            resources.GetString("CMB_raterc.Items3"),
            resources.GetString("CMB_raterc.Items4"),
            resources.GetString("CMB_raterc.Items5"),
            resources.GetString("CMB_raterc.Items6"),
            resources.GetString("CMB_raterc.Items7"),
            resources.GetString("CMB_raterc.Items8"),
            resources.GetString("CMB_raterc.Items9"),
            resources.GetString("CMB_raterc.Items10")});
            resources.ApplyResources(this.CMB_raterc, "CMB_raterc");
            this.CMB_raterc.Name = "CMB_raterc";
            this.CMB_raterc.SelectedIndexChanged += new System.EventHandler(this.CMB_raterc_SelectedIndexChanged);
            // 
            // label104
            // 
            resources.ApplyResources(this.label104, "label104");
            this.label104.Name = "label104";
            // 
            // label103
            // 
            resources.ApplyResources(this.label103, "label103");
            this.label103.Name = "label103";
            // 
            // label102
            // 
            resources.ApplyResources(this.label102, "label102");
            this.label102.Name = "label102";
            // 
            // label101
            // 
            resources.ApplyResources(this.label101, "label101");
            this.label101.Name = "label101";
            // 
            // CMB_ratestatus
            // 
            this.CMB_ratestatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_ratestatus.FormattingEnabled = true;
            this.CMB_ratestatus.Items.AddRange(new object[] {
            resources.GetString("CMB_ratestatus.Items"),
            resources.GetString("CMB_ratestatus.Items1"),
            resources.GetString("CMB_ratestatus.Items2"),
            resources.GetString("CMB_ratestatus.Items3"),
            resources.GetString("CMB_ratestatus.Items4"),
            resources.GetString("CMB_ratestatus.Items5"),
            resources.GetString("CMB_ratestatus.Items6"),
            resources.GetString("CMB_ratestatus.Items7"),
            resources.GetString("CMB_ratestatus.Items8"),
            resources.GetString("CMB_ratestatus.Items9"),
            resources.GetString("CMB_ratestatus.Items10")});
            resources.ApplyResources(this.CMB_ratestatus, "CMB_ratestatus");
            this.CMB_ratestatus.Name = "CMB_ratestatus";
            this.CMB_ratestatus.SelectedIndexChanged += new System.EventHandler(this.CMB_ratestatus_SelectedIndexChanged);
            // 
            // CMB_rateposition
            // 
            this.CMB_rateposition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_rateposition.FormattingEnabled = true;
            this.CMB_rateposition.Items.AddRange(new object[] {
            resources.GetString("CMB_rateposition.Items"),
            resources.GetString("CMB_rateposition.Items1"),
            resources.GetString("CMB_rateposition.Items2"),
            resources.GetString("CMB_rateposition.Items3"),
            resources.GetString("CMB_rateposition.Items4"),
            resources.GetString("CMB_rateposition.Items5"),
            resources.GetString("CMB_rateposition.Items6"),
            resources.GetString("CMB_rateposition.Items7"),
            resources.GetString("CMB_rateposition.Items8"),
            resources.GetString("CMB_rateposition.Items9"),
            resources.GetString("CMB_rateposition.Items10")});
            resources.ApplyResources(this.CMB_rateposition, "CMB_rateposition");
            this.CMB_rateposition.Name = "CMB_rateposition";
            this.CMB_rateposition.SelectedIndexChanged += new System.EventHandler(this.CMB_rateposition_SelectedIndexChanged);
            // 
            // CMB_rateattitude
            // 
            this.CMB_rateattitude.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_rateattitude.FormattingEnabled = true;
            this.CMB_rateattitude.Items.AddRange(new object[] {
            resources.GetString("CMB_rateattitude.Items"),
            resources.GetString("CMB_rateattitude.Items1"),
            resources.GetString("CMB_rateattitude.Items2"),
            resources.GetString("CMB_rateattitude.Items3"),
            resources.GetString("CMB_rateattitude.Items4"),
            resources.GetString("CMB_rateattitude.Items5"),
            resources.GetString("CMB_rateattitude.Items6"),
            resources.GetString("CMB_rateattitude.Items7"),
            resources.GetString("CMB_rateattitude.Items8"),
            resources.GetString("CMB_rateattitude.Items9"),
            resources.GetString("CMB_rateattitude.Items10")});
            resources.ApplyResources(this.CMB_rateattitude, "CMB_rateattitude");
            this.CMB_rateattitude.Name = "CMB_rateattitude";
            this.CMB_rateattitude.SelectedIndexChanged += new System.EventHandler(this.CMB_rateattitude_SelectedIndexChanged);
            // 
            // label99
            // 
            resources.ApplyResources(this.label99, "label99");
            this.label99.Name = "label99";
            // 
            // label98
            // 
            resources.ApplyResources(this.label98, "label98");
            this.label98.Name = "label98";
            // 
            // label97
            // 
            resources.ApplyResources(this.label97, "label97");
            this.label97.Name = "label97";
            // 
            // CMB_speedunits
            // 
            this.CMB_speedunits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_speedunits.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_speedunits, "CMB_speedunits");
            this.CMB_speedunits.Name = "CMB_speedunits";
            this.CMB_speedunits.SelectedIndexChanged += new System.EventHandler(this.CMB_speedunits_SelectedIndexChanged);
            // 
            // CMB_distunits
            // 
            this.CMB_distunits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_distunits.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_distunits, "CMB_distunits");
            this.CMB_distunits.Name = "CMB_distunits";
            this.CMB_distunits.SelectedIndexChanged += new System.EventHandler(this.CMB_distunits_SelectedIndexChanged);
            // 
            // label96
            // 
            resources.ApplyResources(this.label96, "label96");
            this.label96.Name = "label96";
            // 
            // label95
            // 
            resources.ApplyResources(this.label95, "label95");
            this.label95.Name = "label95";
            // 
            // CHK_speechbattery
            // 
            resources.ApplyResources(this.CHK_speechbattery, "CHK_speechbattery");
            this.CHK_speechbattery.Name = "CHK_speechbattery";
            this.CHK_speechbattery.UseVisualStyleBackColor = true;
            this.CHK_speechbattery.CheckedChanged += new System.EventHandler(this.CHK_speechbattery_CheckedChanged);
            // 
            // CHK_speechcustom
            // 
            resources.ApplyResources(this.CHK_speechcustom, "CHK_speechcustom");
            this.CHK_speechcustom.Name = "CHK_speechcustom";
            this.CHK_speechcustom.UseVisualStyleBackColor = true;
            this.CHK_speechcustom.CheckedChanged += new System.EventHandler(this.CHK_speechcustom_CheckedChanged);
            // 
            // CHK_speechmode
            // 
            resources.ApplyResources(this.CHK_speechmode, "CHK_speechmode");
            this.CHK_speechmode.Name = "CHK_speechmode";
            this.CHK_speechmode.UseVisualStyleBackColor = true;
            this.CHK_speechmode.CheckedChanged += new System.EventHandler(this.CHK_speechmode_CheckedChanged);
            // 
            // CHK_speechwaypoint
            // 
            resources.ApplyResources(this.CHK_speechwaypoint, "CHK_speechwaypoint");
            this.CHK_speechwaypoint.Name = "CHK_speechwaypoint";
            this.CHK_speechwaypoint.UseVisualStyleBackColor = true;
            this.CHK_speechwaypoint.CheckedChanged += new System.EventHandler(this.CHK_speechwaypoint_CheckedChanged);
            // 
            // label94
            // 
            resources.ApplyResources(this.label94, "label94");
            this.label94.Name = "label94";
            // 
            // CMB_osdcolor
            // 
            this.CMB_osdcolor.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.CMB_osdcolor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_osdcolor.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_osdcolor, "CMB_osdcolor");
            this.CMB_osdcolor.Name = "CMB_osdcolor";
            this.CMB_osdcolor.DrawItem += new System.Windows.Forms.DrawItemEventHandler(this.CMB_osdcolor_DrawItem);
            this.CMB_osdcolor.SelectedIndexChanged += new System.EventHandler(this.CMB_osdcolor_SelectedIndexChanged);
            // 
            // CMB_language
            // 
            this.CMB_language.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_language.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_language, "CMB_language");
            this.CMB_language.Name = "CMB_language";
            this.CMB_language.SelectedIndexChanged += new System.EventHandler(this.CMB_language_SelectedIndexChanged);
            // 
            // label93
            // 
            resources.ApplyResources(this.label93, "label93");
            this.label93.Name = "label93";
            // 
            // CHK_enablespeech
            // 
            resources.ApplyResources(this.CHK_enablespeech, "CHK_enablespeech");
            this.CHK_enablespeech.Name = "CHK_enablespeech";
            this.CHK_enablespeech.UseVisualStyleBackColor = true;
            this.CHK_enablespeech.CheckedChanged += new System.EventHandler(this.CHK_enablespeech_CheckedChanged);
            // 
            // CHK_hudshow
            // 
            this.CHK_hudshow.Checked = true;
            this.CHK_hudshow.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.CHK_hudshow, "CHK_hudshow");
            this.CHK_hudshow.Name = "CHK_hudshow";
            this.CHK_hudshow.UseVisualStyleBackColor = true;
            this.CHK_hudshow.CheckedChanged += new System.EventHandler(this.CHK_hudshow_CheckedChanged);
            // 
            // label92
            // 
            resources.ApplyResources(this.label92, "label92");
            this.label92.Name = "label92";
            // 
            // CMB_videosources
            // 
            this.CMB_videosources.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_videosources.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_videosources, "CMB_videosources");
            this.CMB_videosources.Name = "CMB_videosources";
            this.CMB_videosources.SelectedIndexChanged += new System.EventHandler(this.CMB_videosources_SelectedIndexChanged);
            this.CMB_videosources.Click += new System.EventHandler(this.CMB_videosources_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // CHK_maprotation
            // 
            resources.ApplyResources(this.CHK_maprotation, "CHK_maprotation");
            this.CHK_maprotation.Name = "CHK_maprotation";
            this.CHK_maprotation.UseVisualStyleBackColor = true;
            this.CHK_maprotation.CheckedChanged += new System.EventHandler(this.CHK_maprotation_CheckedChanged);
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // CHK_disttohomeflightdata
            // 
            this.CHK_disttohomeflightdata.Checked = true;
            this.CHK_disttohomeflightdata.CheckState = System.Windows.Forms.CheckState.Checked;
            resources.ApplyResources(this.CHK_disttohomeflightdata, "CHK_disttohomeflightdata");
            this.CHK_disttohomeflightdata.Name = "CHK_disttohomeflightdata";
            this.CHK_disttohomeflightdata.UseVisualStyleBackColor = true;
            this.CHK_disttohomeflightdata.CheckedChanged += new System.EventHandler(this.CHK_disttohomeflightdata_CheckedChanged);
            // 
            // BUT_Joystick
            // 
            resources.ApplyResources(this.BUT_Joystick, "BUT_Joystick");
            this.BUT_Joystick.Name = "BUT_Joystick";
            this.BUT_Joystick.UseVisualStyleBackColor = true;
            this.BUT_Joystick.Click += new System.EventHandler(this.BUT_Joystick_Click);
            // 
            // BUT_videostop
            // 
            resources.ApplyResources(this.BUT_videostop, "BUT_videostop");
            this.BUT_videostop.Name = "BUT_videostop";
            this.BUT_videostop.UseVisualStyleBackColor = true;
            this.BUT_videostop.Click += new System.EventHandler(this.BUT_videostop_Click);
            // 
            // BUT_videostart
            // 
            resources.ApplyResources(this.BUT_videostart, "BUT_videostart");
            this.BUT_videostart.Name = "BUT_videostart";
            this.BUT_videostart.UseVisualStyleBackColor = true;
            this.BUT_videostart.Click += new System.EventHandler(this.BUT_videostart_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // txt_log_dir
            // 
            resources.ApplyResources(this.txt_log_dir, "txt_log_dir");
            this.txt_log_dir.Name = "txt_log_dir";
            // 
            // BUT_logdirbrowse
            // 
            resources.ApplyResources(this.BUT_logdirbrowse, "BUT_logdirbrowse");
            this.BUT_logdirbrowse.Name = "BUT_logdirbrowse";
            this.BUT_logdirbrowse.UseVisualStyleBackColor = true;
            this.BUT_logdirbrowse.Click += new System.EventHandler(this.BUT_logdirbrowse_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // CMB_theme
            // 
            this.CMB_theme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CMB_theme.FormattingEnabled = true;
            resources.ApplyResources(this.CMB_theme, "CMB_theme");
            this.CMB_theme.Name = "CMB_theme";
            this.CMB_theme.SelectedIndexChanged += new System.EventHandler(this.CMB_theme_SelectedIndexChanged);
            // 
            // BUT_themecustom
            // 
            resources.ApplyResources(this.BUT_themecustom, "BUT_themecustom");
            this.BUT_themecustom.Name = "BUT_themecustom";
            this.BUT_themecustom.UseVisualStyleBackColor = true;
            this.BUT_themecustom.Click += new System.EventHandler(this.BUT_themecustom_Click);
            // 
            // CHK_speecharmdisarm
            // 
            resources.ApplyResources(this.CHK_speecharmdisarm, "CHK_speecharmdisarm");
            this.CHK_speecharmdisarm.Name = "CHK_speecharmdisarm";
            this.CHK_speecharmdisarm.UseVisualStyleBackColor = true;
            this.CHK_speecharmdisarm.CheckedChanged += new System.EventHandler(this.CHK_speecharmdisarm_CheckedChanged);
            // 
            // BUT_Vario
            // 
            resources.ApplyResources(this.BUT_Vario, "BUT_Vario");
            this.BUT_Vario.Name = "BUT_Vario";
            this.BUT_Vario.UseVisualStyleBackColor = true;
            this.BUT_Vario.Click += new System.EventHandler(this.BUT_Vario_Click);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // chk_analytics
            // 
            resources.ApplyResources(this.chk_analytics, "chk_analytics");
            this.chk_analytics.Name = "chk_analytics";
            this.chk_analytics.UseVisualStyleBackColor = true;
            this.chk_analytics.CheckedChanged += new System.EventHandler(this.chk_analytics_CheckedChanged);
            // 
            // CHK_beta
            // 
            resources.ApplyResources(this.CHK_beta, "CHK_beta");
            this.CHK_beta.Name = "CHK_beta";
            this.CHK_beta.UseVisualStyleBackColor = true;
            this.CHK_beta.CheckedChanged += new System.EventHandler(this.CHK_beta_CheckedChanged);
            // 
            // CHK_Password
            // 
            resources.ApplyResources(this.CHK_Password, "CHK_Password");
            this.CHK_Password.Name = "CHK_Password";
            this.CHK_Password.UseVisualStyleBackColor = true;
            this.CHK_Password.CheckedChanged += new System.EventHandler(this.CHK_Password_CheckedChanged);
            // 
            // CHK_speechlowspeed
            // 
            resources.ApplyResources(this.CHK_speechlowspeed, "CHK_speechlowspeed");
            this.CHK_speechlowspeed.Name = "CHK_speechlowspeed";
            this.CHK_speechlowspeed.UseVisualStyleBackColor = true;
            this.CHK_speechlowspeed.CheckedChanged += new System.EventHandler(this.CHK_speechlowspeed_CheckedChanged);
            // 
            // CHK_advancedview
            // 
            resources.ApplyResources(this.CHK_advancedview, "CHK_advancedview");
            this.CHK_advancedview.Name = "CHK_advancedview";
            this.CHK_advancedview.UseVisualStyleBackColor = true;
            this.CHK_advancedview.CheckedChanged += new System.EventHandler(this.CHK_advancedview_CheckedChanged);
            // 
            // CHK_showairports
            // 
            resources.ApplyResources(this.CHK_showairports, "CHK_showairports");
            this.CHK_showairports.Checked = true;
            this.CHK_showairports.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHK_showairports.Name = "CHK_showairports";
            this.CHK_showairports.UseVisualStyleBackColor = true;
            this.CHK_showairports.CheckedChanged += new System.EventHandler(this.CHK_showairports_CheckedChanged);
            // 
            // chk_ADSB
            // 
            resources.ApplyResources(this.chk_ADSB, "chk_ADSB");
            this.chk_ADSB.Name = "chk_ADSB";
            this.chk_ADSB.UseVisualStyleBackColor = true;
            this.chk_ADSB.CheckedChanged += new System.EventHandler(this.chk_ADSB_CheckedChanged);
            // 
            // chk_tfr
            // 
            resources.ApplyResources(this.chk_tfr, "chk_tfr");
            this.chk_tfr.Checked = true;
            this.chk_tfr.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_tfr.Name = "chk_tfr";
            this.chk_tfr.UseVisualStyleBackColor = true;
            this.chk_tfr.CheckedChanged += new System.EventHandler(this.chk_tfr_CheckedChanged);
            // 
            // chk_temp
            // 
            resources.ApplyResources(this.chk_temp, "chk_temp");
            this.chk_temp.Name = "chk_temp";
            this.chk_temp.UseVisualStyleBackColor = true;
            this.chk_temp.CheckedChanged += new System.EventHandler(this.chk_temp_CheckedChanged);
            // 
            // ConfigPlanner
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chk_temp);
            this.Controls.Add(this.chk_tfr);
            this.Controls.Add(this.chk_ADSB);
            this.Controls.Add(this.CHK_showairports);
            this.Controls.Add(this.CHK_advancedview);
            this.Controls.Add(this.CHK_speechlowspeed);
            this.Controls.Add(this.CHK_Password);
            this.Controls.Add(this.CHK_beta);
            this.Controls.Add(this.chk_analytics);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.BUT_Vario);
            this.Controls.Add(this.CHK_speecharmdisarm);
            this.Controls.Add(this.BUT_themecustom);
            this.Controls.Add(this.CMB_theme);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.BUT_logdirbrowse);
            this.Controls.Add(this.txt_log_dir);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CHK_disttohomeflightdata);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CHK_maprotation);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.CMB_ratesensors);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.CMB_videoresolutions);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.CHK_GDIPlus);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.CHK_loadwponconnect);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.NUM_tracklength);
            this.Controls.Add(this.CHK_speechaltwarning);
            this.Controls.Add(this.label108);
            this.Controls.Add(this.CHK_resetapmonconnect);
            this.Controls.Add(this.CHK_mavdebug);
            this.Controls.Add(this.label107);
            this.Controls.Add(this.CMB_raterc);
            this.Controls.Add(this.label104);
            this.Controls.Add(this.label103);
            this.Controls.Add(this.label102);
            this.Controls.Add(this.label101);
            this.Controls.Add(this.CMB_ratestatus);
            this.Controls.Add(this.CMB_rateposition);
            this.Controls.Add(this.CMB_rateattitude);
            this.Controls.Add(this.label99);
            this.Controls.Add(this.label98);
            this.Controls.Add(this.label97);
            this.Controls.Add(this.CMB_speedunits);
            this.Controls.Add(this.CMB_distunits);
            this.Controls.Add(this.label96);
            this.Controls.Add(this.label95);
            this.Controls.Add(this.CHK_speechbattery);
            this.Controls.Add(this.CHK_speechcustom);
            this.Controls.Add(this.CHK_speechmode);
            this.Controls.Add(this.CHK_speechwaypoint);
            this.Controls.Add(this.label94);
            this.Controls.Add(this.CMB_osdcolor);
            this.Controls.Add(this.CMB_language);
            this.Controls.Add(this.label93);
            this.Controls.Add(this.CHK_enablespeech);
            this.Controls.Add(this.CHK_hudshow);
            this.Controls.Add(this.label92);
            this.Controls.Add(this.CMB_videosources);
            this.Controls.Add(this.BUT_Joystick);
            this.Controls.Add(this.BUT_videostop);
            this.Controls.Add(this.BUT_videostart);
            this.Name = "ConfigPlanner";
            this.Load += new System.EventHandler(this.ConfigPlanner_Load);
            ((System.ComponentModel.ISupportInitialize)(this.NUM_tracklength)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.ComboBox CMB_ratesensors;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.ComboBox CMB_videoresolutions;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox CHK_GDIPlus;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.CheckBox CHK_loadwponconnect;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.NumericUpDown NUM_tracklength;
        private System.Windows.Forms.CheckBox CHK_speechaltwarning;
        private System.Windows.Forms.Label label108;
        private System.Windows.Forms.CheckBox CHK_resetapmonconnect;
        private System.Windows.Forms.CheckBox CHK_mavdebug;
        private System.Windows.Forms.Label label107;
        private System.Windows.Forms.ComboBox CMB_raterc;
        private System.Windows.Forms.Label label104;
        private System.Windows.Forms.Label label103;
        private System.Windows.Forms.Label label102;
        private System.Windows.Forms.Label label101;
        private System.Windows.Forms.ComboBox CMB_ratestatus;
        private System.Windows.Forms.ComboBox CMB_rateposition;
        private System.Windows.Forms.ComboBox CMB_rateattitude;
        private System.Windows.Forms.Label label99;
        private System.Windows.Forms.Label label98;
        private System.Windows.Forms.Label label97;
        private System.Windows.Forms.ComboBox CMB_speedunits;
        private System.Windows.Forms.ComboBox CMB_distunits;
        private System.Windows.Forms.Label label96;
        private System.Windows.Forms.Label label95;
        private System.Windows.Forms.CheckBox CHK_speechbattery;
        private System.Windows.Forms.CheckBox CHK_speechcustom;
        private System.Windows.Forms.CheckBox CHK_speechmode;
        private System.Windows.Forms.CheckBox CHK_speechwaypoint;
        private System.Windows.Forms.Label label94;
        private System.Windows.Forms.ComboBox CMB_osdcolor;
        private System.Windows.Forms.ComboBox CMB_language;
        private System.Windows.Forms.Label label93;
        private System.Windows.Forms.CheckBox CHK_enablespeech;
        private System.Windows.Forms.CheckBox CHK_hudshow;
        private System.Windows.Forms.Label label92;
        private System.Windows.Forms.ComboBox CMB_videosources;
        private Controls.MyButton BUT_Joystick;
        private Controls.MyButton BUT_videostop;
        private Controls.MyButton BUT_videostart;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox CHK_maprotation;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox CHK_disttohomeflightdata;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_log_dir;
        private Controls.MyButton BUT_logdirbrowse;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox CMB_theme;
        private Controls.MyButton BUT_themecustom;
        private System.Windows.Forms.CheckBox CHK_speecharmdisarm;
        private Controls.MyButton BUT_Vario;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox chk_analytics;
        private System.Windows.Forms.CheckBox CHK_beta;
        private System.Windows.Forms.CheckBox CHK_Password;
        private System.Windows.Forms.CheckBox CHK_speechlowspeed;
        private System.Windows.Forms.CheckBox CHK_advancedview;
        private System.Windows.Forms.CheckBox CHK_showairports;
        private System.Windows.Forms.CheckBox chk_ADSB;
        private System.Windows.Forms.CheckBox chk_tfr;
        private System.Windows.Forms.CheckBox chk_temp;
    }
}
