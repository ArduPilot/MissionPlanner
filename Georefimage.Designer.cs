using System.Windows.Forms;
namespace MissionPlanner
{
    partial class Georefimage
    {

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Georefimage));
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.TXT_logfile = new System.Windows.Forms.TextBox();
            this.TXT_jpgdir = new System.Windows.Forms.TextBox();
            this.TXT_offsetseconds = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.TXT_outputlog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BUT_Geotagimages = new MissionPlanner.Controls.MyButton();
            this.BUT_estoffset = new MissionPlanner.Controls.MyButton();
            this.BUT_doit = new MissionPlanner.Controls.MyButton();
            this.BUT_browsedir = new MissionPlanner.Controls.MyButton();
            this.BUT_browselog = new MissionPlanner.Controls.MyButton();
            this.NUM_latpos = new System.Windows.Forms.NumericUpDown();
            this.NUM_lngpos = new System.Windows.Forms.NumericUpDown();
            this.NUM_altpos = new System.Windows.Forms.NumericUpDown();
            this.NUM_ATT_Heading = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.BUT_networklinkgeoref = new MissionPlanner.Controls.MyButton();
            this.NUM_time = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.RDIO_TimeOffset = new System.Windows.Forms.RadioButton();
            this.RDIO_CAMMsgSynchro = new System.Windows.Forms.RadioButton();
            this.PANEL_TIME_OFFSET = new System.Windows.Forms.Panel();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.NUM_CAM_Alt = new System.Windows.Forms.NumericUpDown();
            this.label15 = new System.Windows.Forms.Label();
            this.NUM_CAM_Lon = new System.Windows.Forms.NumericUpDown();
            this.NUM_CAM_Lat = new System.Windows.Forms.NumericUpDown();
            this.label16 = new System.Windows.Forms.Label();
            this.PANEL_CAM = new System.Windows.Forms.Panel();
            this.NUM_CAM_Week = new System.Windows.Forms.NumericUpDown();
            this.label26 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.NUM_ATT_Roll = new System.Windows.Forms.NumericUpDown();
            this.label25 = new System.Windows.Forms.Label();
            this.NUM_ATT_Pitch = new System.Windows.Forms.NumericUpDown();
            this.label24 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.NUM_GPS_Week = new System.Windows.Forms.NumericUpDown();
            this.panel1 = new System.Windows.Forms.Panel();
            this.NUM_CAM_Time = new System.Windows.Forms.NumericUpDown();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.NUM_GPS_AMSL_Alt = new System.Windows.Forms.NumericUpDown();
            this.label20 = new System.Windows.Forms.Label();
            this.NUM_CAM_Pitch = new System.Windows.Forms.NumericUpDown();
            this.label19 = new System.Windows.Forms.Label();
            this.NUM_CAM_Roll = new System.Windows.Forms.NumericUpDown();
            this.label18 = new System.Windows.Forms.Label();
            this.NUM_CAM_Heading = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.num_camerarotation = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.num_hfov = new System.Windows.Forms.NumericUpDown();
            this.num_vfov = new System.Windows.Forms.NumericUpDown();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label28 = new System.Windows.Forms.Label();
            this.txt_basealt = new System.Windows.Forms.TextBox();
            this.CHECK_AMSLAlt_Use = new System.Windows.Forms.CheckBox();
            this.PANEL_SHUTTER_LAG = new System.Windows.Forms.Panel();
            this.TXT_shutterLag = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.chk_cammsg = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_latpos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_lngpos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_altpos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_ATT_Heading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_time)).BeginInit();
            this.PANEL_TIME_OFFSET.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Alt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Lon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Lat)).BeginInit();
            this.PANEL_CAM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Week)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_ATT_Roll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_ATT_Pitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_GPS_Week)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Time)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_GPS_AMSL_Alt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Pitch)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Roll)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Heading)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_camerarotation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_hfov)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_vfov)).BeginInit();
            this.panel3.SuspendLayout();
            this.PANEL_SHUTTER_LAG.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // TXT_logfile
            // 
            resources.ApplyResources(this.TXT_logfile, "TXT_logfile");
            this.TXT_logfile.Name = "TXT_logfile";
            this.TXT_logfile.TextChanged += new System.EventHandler(this.TXT_logfile_TextChanged);
            // 
            // TXT_jpgdir
            // 
            resources.ApplyResources(this.TXT_jpgdir, "TXT_jpgdir");
            this.TXT_jpgdir.Name = "TXT_jpgdir";
            // 
            // TXT_offsetseconds
            // 
            resources.ApplyResources(this.TXT_offsetseconds, "TXT_offsetseconds");
            this.TXT_offsetseconds.Name = "TXT_offsetseconds";
            // 
            // TXT_outputlog
            // 
            resources.ApplyResources(this.TXT_outputlog, "TXT_outputlog");
            this.TXT_outputlog.Name = "TXT_outputlog";
            this.TXT_outputlog.ReadOnly = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // BUT_Geotagimages
            // 
            resources.ApplyResources(this.BUT_Geotagimages, "BUT_Geotagimages");
            this.BUT_Geotagimages.Name = "BUT_Geotagimages";
            this.BUT_Geotagimages.UseVisualStyleBackColor = true;
            this.BUT_Geotagimages.Click += new System.EventHandler(this.BUT_Geotagimages_Click);
            // 
            // BUT_estoffset
            // 
            resources.ApplyResources(this.BUT_estoffset, "BUT_estoffset");
            this.BUT_estoffset.Name = "BUT_estoffset";
            this.BUT_estoffset.UseVisualStyleBackColor = true;
            this.BUT_estoffset.Click += new System.EventHandler(this.BUT_estoffset_Click);
            // 
            // BUT_doit
            // 
            resources.ApplyResources(this.BUT_doit, "BUT_doit");
            this.BUT_doit.Name = "BUT_doit";
            this.BUT_doit.UseVisualStyleBackColor = true;
            this.BUT_doit.Click += new System.EventHandler(this.BUT_doit_Click);
            // 
            // BUT_browsedir
            // 
            resources.ApplyResources(this.BUT_browsedir, "BUT_browsedir");
            this.BUT_browsedir.Name = "BUT_browsedir";
            this.BUT_browsedir.UseVisualStyleBackColor = true;
            this.BUT_browsedir.Click += new System.EventHandler(this.BUT_browsedir_Click);
            // 
            // BUT_browselog
            // 
            resources.ApplyResources(this.BUT_browselog, "BUT_browselog");
            this.BUT_browselog.Name = "BUT_browselog";
            this.BUT_browselog.UseVisualStyleBackColor = true;
            this.BUT_browselog.Click += new System.EventHandler(this.BUT_browselog_Click);
            // 
            // NUM_latpos
            // 
            resources.ApplyResources(this.NUM_latpos, "NUM_latpos");
            this.NUM_latpos.Name = "NUM_latpos";
            this.NUM_latpos.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.NUM_latpos.ValueChanged += new System.EventHandler(this.NUM_latpos_ValueChanged);
            // 
            // NUM_lngpos
            // 
            resources.ApplyResources(this.NUM_lngpos, "NUM_lngpos");
            this.NUM_lngpos.Name = "NUM_lngpos";
            this.NUM_lngpos.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NUM_lngpos.ValueChanged += new System.EventHandler(this.NUM_lngpos_ValueChanged);
            // 
            // NUM_altpos
            // 
            resources.ApplyResources(this.NUM_altpos, "NUM_altpos");
            this.NUM_altpos.Name = "NUM_altpos";
            this.NUM_altpos.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.NUM_altpos.ValueChanged += new System.EventHandler(this.NUM_altpos_ValueChanged);
            // 
            // NUM_ATT_Heading
            // 
            resources.ApplyResources(this.NUM_ATT_Heading, "NUM_ATT_Heading");
            this.NUM_ATT_Heading.Name = "NUM_ATT_Heading";
            this.NUM_ATT_Heading.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUM_ATT_Heading.ValueChanged += new System.EventHandler(this.NUM_ATT_Heading_ValueChanged);
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
            // BUT_networklinkgeoref
            // 
            resources.ApplyResources(this.BUT_networklinkgeoref, "BUT_networklinkgeoref");
            this.BUT_networklinkgeoref.Name = "BUT_networklinkgeoref";
            this.BUT_networklinkgeoref.UseVisualStyleBackColor = true;
            this.BUT_networklinkgeoref.Click += new System.EventHandler(this.BUT_networklinkgeoref_Click);
            // 
            // NUM_time
            // 
            resources.ApplyResources(this.NUM_time, "NUM_time");
            this.NUM_time.Name = "NUM_time";
            this.NUM_time.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NUM_time.ValueChanged += new System.EventHandler(this.NUM_time_ValueChanged);
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
            // RDIO_TimeOffset
            // 
            resources.ApplyResources(this.RDIO_TimeOffset, "RDIO_TimeOffset");
            this.RDIO_TimeOffset.Name = "RDIO_TimeOffset";
            this.RDIO_TimeOffset.UseVisualStyleBackColor = true;
            this.RDIO_TimeOffset.CheckedChanged += new System.EventHandler(this.ProcessType_CheckedChanged);
            // 
            // RDIO_CAMMsgSynchro
            // 
            resources.ApplyResources(this.RDIO_CAMMsgSynchro, "RDIO_CAMMsgSynchro");
            this.RDIO_CAMMsgSynchro.Checked = true;
            this.RDIO_CAMMsgSynchro.Name = "RDIO_CAMMsgSynchro";
            this.RDIO_CAMMsgSynchro.TabStop = true;
            this.RDIO_CAMMsgSynchro.UseVisualStyleBackColor = true;
            this.RDIO_CAMMsgSynchro.CheckedChanged += new System.EventHandler(this.ProcessType_CheckedChanged);
            // 
            // PANEL_TIME_OFFSET
            // 
            this.PANEL_TIME_OFFSET.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PANEL_TIME_OFFSET.Controls.Add(this.BUT_estoffset);
            this.PANEL_TIME_OFFSET.Controls.Add(this.TXT_offsetseconds);
            this.PANEL_TIME_OFFSET.Controls.Add(this.label1);
            resources.ApplyResources(this.PANEL_TIME_OFFSET, "PANEL_TIME_OFFSET");
            this.PANEL_TIME_OFFSET.Name = "PANEL_TIME_OFFSET";
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
            // NUM_CAM_Alt
            // 
            resources.ApplyResources(this.NUM_CAM_Alt, "NUM_CAM_Alt");
            this.NUM_CAM_Alt.Name = "NUM_CAM_Alt";
            this.NUM_CAM_Alt.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.NUM_CAM_Alt.ValueChanged += new System.EventHandler(this.NUM_CAM_Alt_ValueChanged);
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // NUM_CAM_Lon
            // 
            resources.ApplyResources(this.NUM_CAM_Lon, "NUM_CAM_Lon");
            this.NUM_CAM_Lon.Name = "NUM_CAM_Lon";
            this.NUM_CAM_Lon.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.NUM_CAM_Lon.ValueChanged += new System.EventHandler(this.NUM_CAM_Lon_ValueChanged);
            // 
            // NUM_CAM_Lat
            // 
            resources.ApplyResources(this.NUM_CAM_Lat, "NUM_CAM_Lat");
            this.NUM_CAM_Lat.Name = "NUM_CAM_Lat";
            this.NUM_CAM_Lat.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NUM_CAM_Lat.ValueChanged += new System.EventHandler(this.NUM_CAM_Lat_ValueChanged);
            // 
            // label16
            // 
            resources.ApplyResources(this.label16, "label16");
            this.label16.Name = "label16";
            // 
            // PANEL_CAM
            // 
            this.PANEL_CAM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Week);
            this.PANEL_CAM.Controls.Add(this.label26);
            this.PANEL_CAM.Controls.Add(this.panel2);
            this.PANEL_CAM.Controls.Add(this.NUM_ATT_Roll);
            this.PANEL_CAM.Controls.Add(this.label25);
            this.PANEL_CAM.Controls.Add(this.NUM_ATT_Pitch);
            this.PANEL_CAM.Controls.Add(this.label24);
            this.PANEL_CAM.Controls.Add(this.label6);
            this.PANEL_CAM.Controls.Add(this.label23);
            this.PANEL_CAM.Controls.Add(this.NUM_GPS_Week);
            this.PANEL_CAM.Controls.Add(this.panel1);
            this.PANEL_CAM.Controls.Add(this.NUM_latpos);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Time);
            this.PANEL_CAM.Controls.Add(this.NUM_lngpos);
            this.PANEL_CAM.Controls.Add(this.label22);
            this.PANEL_CAM.Controls.Add(this.label10);
            this.PANEL_CAM.Controls.Add(this.label21);
            this.PANEL_CAM.Controls.Add(this.NUM_altpos);
            this.PANEL_CAM.Controls.Add(this.NUM_GPS_AMSL_Alt);
            this.PANEL_CAM.Controls.Add(this.NUM_time);
            this.PANEL_CAM.Controls.Add(this.label20);
            this.PANEL_CAM.Controls.Add(this.NUM_ATT_Heading);
            this.PANEL_CAM.Controls.Add(this.label2);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Pitch);
            this.PANEL_CAM.Controls.Add(this.label3);
            this.PANEL_CAM.Controls.Add(this.label19);
            this.PANEL_CAM.Controls.Add(this.label4);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Roll);
            this.PANEL_CAM.Controls.Add(this.label5);
            this.PANEL_CAM.Controls.Add(this.label18);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Heading);
            this.PANEL_CAM.Controls.Add(this.label17);
            this.PANEL_CAM.Controls.Add(this.label16);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Lat);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Lon);
            this.PANEL_CAM.Controls.Add(this.label15);
            this.PANEL_CAM.Controls.Add(this.NUM_CAM_Alt);
            this.PANEL_CAM.Controls.Add(this.label14);
            this.PANEL_CAM.Controls.Add(this.label13);
            resources.ApplyResources(this.PANEL_CAM, "PANEL_CAM");
            this.PANEL_CAM.Name = "PANEL_CAM";
            // 
            // NUM_CAM_Week
            // 
            resources.ApplyResources(this.NUM_CAM_Week, "NUM_CAM_Week");
            this.NUM_CAM_Week.Name = "NUM_CAM_Week";
            this.NUM_CAM_Week.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NUM_CAM_Week.ValueChanged += new System.EventHandler(this.NUM_CAM_Week_ValueChanged);
            // 
            // label26
            // 
            resources.ApplyResources(this.label26, "label26");
            this.label26.Name = "label26";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // NUM_ATT_Roll
            // 
            resources.ApplyResources(this.NUM_ATT_Roll, "NUM_ATT_Roll");
            this.NUM_ATT_Roll.Name = "NUM_ATT_Roll";
            this.NUM_ATT_Roll.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUM_ATT_Roll.ValueChanged += new System.EventHandler(this.NUM_ATT_Roll_ValueChanged);
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // NUM_ATT_Pitch
            // 
            resources.ApplyResources(this.NUM_ATT_Pitch, "NUM_ATT_Pitch");
            this.NUM_ATT_Pitch.Name = "NUM_ATT_Pitch";
            this.NUM_ATT_Pitch.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.NUM_ATT_Pitch.ValueChanged += new System.EventHandler(this.NUM_ATT_Pitch_ValueChanged);
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // NUM_GPS_Week
            // 
            resources.ApplyResources(this.NUM_GPS_Week, "NUM_GPS_Week");
            this.NUM_GPS_Week.Name = "NUM_GPS_Week";
            this.NUM_GPS_Week.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            this.NUM_GPS_Week.ValueChanged += new System.EventHandler(this.NUM_GPS_Week_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // NUM_CAM_Time
            // 
            resources.ApplyResources(this.NUM_CAM_Time, "NUM_CAM_Time");
            this.NUM_CAM_Time.Name = "NUM_CAM_Time";
            this.NUM_CAM_Time.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUM_CAM_Time.ValueChanged += new System.EventHandler(this.NUM_CAM_Time_ValueChanged);
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // NUM_GPS_AMSL_Alt
            // 
            resources.ApplyResources(this.NUM_GPS_AMSL_Alt, "NUM_GPS_AMSL_Alt");
            this.NUM_GPS_AMSL_Alt.Name = "NUM_GPS_AMSL_Alt";
            this.NUM_GPS_AMSL_Alt.Value = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.NUM_GPS_AMSL_Alt.ValueChanged += new System.EventHandler(this.NUM_GPS_AMSL_Alt_ValueChanged);
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // NUM_CAM_Pitch
            // 
            resources.ApplyResources(this.NUM_CAM_Pitch, "NUM_CAM_Pitch");
            this.NUM_CAM_Pitch.Name = "NUM_CAM_Pitch";
            this.NUM_CAM_Pitch.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            this.NUM_CAM_Pitch.ValueChanged += new System.EventHandler(this.NUM_CAM_Pitch_ValueChanged);
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // NUM_CAM_Roll
            // 
            resources.ApplyResources(this.NUM_CAM_Roll, "NUM_CAM_Roll");
            this.NUM_CAM_Roll.Name = "NUM_CAM_Roll";
            this.NUM_CAM_Roll.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NUM_CAM_Roll.ValueChanged += new System.EventHandler(this.NUM_CAM_Roll_ValueChanged);
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // NUM_CAM_Heading
            // 
            resources.ApplyResources(this.NUM_CAM_Heading, "NUM_CAM_Heading");
            this.NUM_CAM_Heading.Name = "NUM_CAM_Heading";
            this.NUM_CAM_Heading.Value = new decimal(new int[] {
            6,
            0,
            0,
            0});
            this.NUM_CAM_Heading.ValueChanged += new System.EventHandler(this.NUM_CAM_Heading_ValueChanged);
            // 
            // label17
            // 
            resources.ApplyResources(this.label17, "label17");
            this.label17.Name = "label17";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // num_camerarotation
            // 
            resources.ApplyResources(this.num_camerarotation, "num_camerarotation");
            this.num_camerarotation.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.num_camerarotation.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
            this.num_camerarotation.Name = "num_camerarotation";
            this.num_camerarotation.Value = new decimal(new int[] {
            90,
            0,
            0,
            0});
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
            // num_hfov
            // 
            resources.ApplyResources(this.num_hfov, "num_hfov");
            this.num_hfov.Maximum = new decimal(new int[] {
            900,
            0,
            0,
            0});
            this.num_hfov.Name = "num_hfov";
            this.num_hfov.Value = new decimal(new int[] {
            200,
            0,
            0,
            0});
            // 
            // num_vfov
            // 
            resources.ApplyResources(this.num_vfov, "num_vfov");
            this.num_vfov.Maximum = new decimal(new int[] {
            900,
            0,
            0,
            0});
            this.num_vfov.Name = "num_vfov";
            this.num_vfov.Value = new decimal(new int[] {
            130,
            0,
            0,
            0});
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.label28);
            this.panel3.Controls.Add(this.txt_basealt);
            this.panel3.Controls.Add(this.CHECK_AMSLAlt_Use);
            this.panel3.Controls.Add(this.label8);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Controls.Add(this.num_vfov);
            this.panel3.Controls.Add(this.num_camerarotation);
            this.panel3.Controls.Add(this.num_hfov);
            this.panel3.Controls.Add(this.label7);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // label28
            // 
            resources.ApplyResources(this.label28, "label28");
            this.label28.Name = "label28";
            // 
            // txt_basealt
            // 
            resources.ApplyResources(this.txt_basealt, "txt_basealt");
            this.txt_basealt.Name = "txt_basealt";
            // 
            // CHECK_AMSLAlt_Use
            // 
            resources.ApplyResources(this.CHECK_AMSLAlt_Use, "CHECK_AMSLAlt_Use");
            this.CHECK_AMSLAlt_Use.Checked = true;
            this.CHECK_AMSLAlt_Use.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CHECK_AMSLAlt_Use.Name = "CHECK_AMSLAlt_Use";
            this.CHECK_AMSLAlt_Use.UseVisualStyleBackColor = true;
            this.CHECK_AMSLAlt_Use.CheckedChanged += new System.EventHandler(this.CHECK_AMSLAlt_Use_CheckedChanged);
            // 
            // PANEL_SHUTTER_LAG
            // 
            this.PANEL_SHUTTER_LAG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PANEL_SHUTTER_LAG.Controls.Add(this.TXT_shutterLag);
            this.PANEL_SHUTTER_LAG.Controls.Add(this.label27);
            resources.ApplyResources(this.PANEL_SHUTTER_LAG, "PANEL_SHUTTER_LAG");
            this.PANEL_SHUTTER_LAG.Name = "PANEL_SHUTTER_LAG";
            // 
            // TXT_shutterLag
            // 
            resources.ApplyResources(this.TXT_shutterLag, "TXT_shutterLag");
            this.TXT_shutterLag.Name = "TXT_shutterLag";
            this.TXT_shutterLag.TextChanged += new System.EventHandler(this.TXT_shutterLag_TextChanged);
            // 
            // label27
            // 
            resources.ApplyResources(this.label27, "label27");
            this.label27.Name = "label27";
            // 
            // chk_cammsg
            // 
            resources.ApplyResources(this.chk_cammsg, "chk_cammsg");
            this.chk_cammsg.Name = "chk_cammsg";
            this.chk_cammsg.UseVisualStyleBackColor = true;
            this.chk_cammsg.CheckedChanged += new System.EventHandler(this.chk_cammsg_CheckedChanged);
            // 
            // Georefimage
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.chk_cammsg);
            this.Controls.Add(this.PANEL_SHUTTER_LAG);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.PANEL_CAM);
            this.Controls.Add(this.PANEL_TIME_OFFSET);
            this.Controls.Add(this.RDIO_CAMMsgSynchro);
            this.Controls.Add(this.RDIO_TimeOffset);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.BUT_networklinkgeoref);
            this.Controls.Add(this.BUT_Geotagimages);
            this.Controls.Add(this.TXT_outputlog);
            this.Controls.Add(this.BUT_doit);
            this.Controls.Add(this.TXT_jpgdir);
            this.Controls.Add(this.TXT_logfile);
            this.Controls.Add(this.BUT_browsedir);
            this.Controls.Add(this.BUT_browselog);
            this.Name = "Georefimage";
            ((System.ComponentModel.ISupportInitialize)(this.NUM_latpos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_lngpos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_altpos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_ATT_Heading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_time)).EndInit();
            this.PANEL_TIME_OFFSET.ResumeLayout(false);
            this.PANEL_TIME_OFFSET.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Alt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Lon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Lat)).EndInit();
            this.PANEL_CAM.ResumeLayout(false);
            this.PANEL_CAM.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Week)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_ATT_Roll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_ATT_Pitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_GPS_Week)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Time)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_GPS_AMSL_Alt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Pitch)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Roll)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_CAM_Heading)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_camerarotation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_hfov)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_vfov)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.PANEL_SHUTTER_LAG.ResumeLayout(false);
            this.PANEL_SHUTTER_LAG.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        private NumericUpDown NUM_CAM_Week;
        private Label label26;
        private Panel PANEL_SHUTTER_LAG;
        private TextBox TXT_shutterLag;
        private Label label27;
        private Label label6;
        private Panel panel2;
        private NumericUpDown NUM_ATT_Roll;
        private Label label25;
        private NumericUpDown NUM_ATT_Pitch;
        private Label label24;
        private CheckBox CHECK_AMSLAlt_Use;
        private Label label23;
        private NumericUpDown NUM_GPS_Week;
        private OpenFileDialog openFileDialog1;
        private Controls.MyButton BUT_browselog;
        private Controls.MyButton BUT_browsedir;
        private TextBox TXT_logfile;
        private TextBox TXT_jpgdir;
        private TextBox TXT_offsetseconds;
        private Controls.MyButton BUT_doit;
        private FolderBrowserDialog folderBrowserDialog1;
        private Label label1;
        private TextBox TXT_outputlog;
        private Controls.MyButton BUT_estoffset;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private Label label16;
        private Label label9;
        private Label label8;
        private Label label7;
        private Label label20;
        private Label label19;
        private Label label18;
        private Label label17;
        private Label label21;
        private Label label22;
        private NumericUpDown NUM_latpos;
        private NumericUpDown NUM_lngpos;
        private NumericUpDown NUM_altpos;
        private NumericUpDown NUM_ATT_Heading;
        private Controls.MyButton BUT_networklinkgeoref;
        private NumericUpDown NUM_time;
        private RadioButton RDIO_TimeOffset;
        private RadioButton RDIO_CAMMsgSynchro;
        private Panel PANEL_TIME_OFFSET;
        private NumericUpDown NUM_CAM_Alt;
        private NumericUpDown NUM_CAM_Lon;
        private NumericUpDown NUM_CAM_Lat;
        private Panel PANEL_CAM;
        private NumericUpDown num_camerarotation;
        private NumericUpDown num_hfov;
        private NumericUpDown num_vfov;
        private Panel panel3;
        private Controls.MyButton BUT_Geotagimages;
        private PROCESSING_MODE selectedProcessingMode;
        private NumericUpDown NUM_CAM_Pitch;
        private NumericUpDown NUM_CAM_Roll;
        private NumericUpDown NUM_CAM_Heading;
        private NumericUpDown NUM_GPS_AMSL_Alt;
        private NumericUpDown NUM_CAM_Time;
        private Panel panel1;
    }
}