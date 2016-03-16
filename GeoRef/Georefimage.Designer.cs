using System.Windows.Forms;
namespace MissionPlanner.GeoRef
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
            this.BUT_networklinkgeoref = new MissionPlanner.Controls.MyButton();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.RDIO_TimeOffset = new System.Windows.Forms.RadioButton();
            this.RDIO_CAMMsgSynchro = new System.Windows.Forms.RadioButton();
            this.PANEL_TIME_OFFSET = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.num_camerarotation = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.num_hfov = new System.Windows.Forms.NumericUpDown();
            this.num_vfov = new System.Windows.Forms.NumericUpDown();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chk_usegps2 = new System.Windows.Forms.CheckBox();
            this.label28 = new System.Windows.Forms.Label();
            this.txt_basealt = new System.Windows.Forms.TextBox();
            this.CHECK_AMSLAlt_Use = new System.Windows.Forms.CheckBox();
            this.PANEL_SHUTTER_LAG = new System.Windows.Forms.Panel();
            this.TXT_shutterLag = new System.Windows.Forms.TextBox();
            this.label27 = new System.Windows.Forms.Label();
            this.chk_cammsg = new System.Windows.Forms.CheckBox();
            this.PANEL_TIME_OFFSET.SuspendLayout();
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
            // BUT_networklinkgeoref
            // 
            resources.ApplyResources(this.BUT_networklinkgeoref, "BUT_networklinkgeoref");
            this.BUT_networklinkgeoref.Name = "BUT_networklinkgeoref";
            this.BUT_networklinkgeoref.UseVisualStyleBackColor = true;
            this.BUT_networklinkgeoref.Click += new System.EventHandler(this.BUT_networklinkgeoref_Click);
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
            this.panel3.Controls.Add(this.chk_usegps2);
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
            // chk_usegps2
            // 
            resources.ApplyResources(this.chk_usegps2, "chk_usegps2");
            this.chk_usegps2.Name = "chk_usegps2";
            this.chk_usegps2.UseVisualStyleBackColor = true;
            this.chk_usegps2.CheckedChanged += new System.EventHandler(this.chk_usegps2_CheckedChanged);
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
            this.PANEL_TIME_OFFSET.ResumeLayout(false);
            this.PANEL_TIME_OFFSET.PerformLayout();
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
        private Panel PANEL_SHUTTER_LAG;
        private TextBox TXT_shutterLag;
        private Label label27;
        private CheckBox CHECK_AMSLAlt_Use;
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
        private Label label11;
        private Label label12;
        private Label label9;
        private Label label8;
        private Label label7;
        private Controls.MyButton BUT_networklinkgeoref;
        private RadioButton RDIO_TimeOffset;
        private RadioButton RDIO_CAMMsgSynchro;
        private Panel PANEL_TIME_OFFSET;
        private NumericUpDown num_camerarotation;
        private NumericUpDown num_hfov;
        private NumericUpDown num_vfov;
        private Panel panel3;
        private Controls.MyButton BUT_Geotagimages;
        private PROCESSING_MODE selectedProcessingMode;
        private CheckBox chk_usegps2;
    }
}