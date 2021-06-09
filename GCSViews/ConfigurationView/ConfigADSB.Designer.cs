namespace MissionPlanner.GCSViews.ConfigurationView
{
   partial class ConfigADSB
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigADSB));
            this.tableLayoutPanel1 = new System.Windows.Forms.Panel();
            this.BUT_rerequestparams = new MissionPlanner.Controls.MyButton();
            this.BUT_writePIDS = new MissionPlanner.Controls.MyButton();
            this.BUT_Find = new MissionPlanner.Controls.MyButton();
            this.txt_acreg = new System.Windows.Forms.TextBox();
            this.txt_flid = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.but_saveacreg = new MissionPlanner.Controls.MyButton();
            this.but_saveflid = new MissionPlanner.Controls.MyButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // BUT_rerequestparams
            // 
            resources.ApplyResources(this.BUT_rerequestparams, "BUT_rerequestparams");
            this.BUT_rerequestparams.Name = "BUT_rerequestparams";
            this.BUT_rerequestparams.UseVisualStyleBackColor = true;
            this.BUT_rerequestparams.Click += new System.EventHandler(this.BUT_rerequestparams_Click);
            // 
            // BUT_writePIDS
            // 
            resources.ApplyResources(this.BUT_writePIDS, "BUT_writePIDS");
            this.BUT_writePIDS.Name = "BUT_writePIDS";
            this.BUT_writePIDS.UseVisualStyleBackColor = true;
            this.BUT_writePIDS.Click += new System.EventHandler(this.BUT_writePIDS_Click);
            // 
            // BUT_Find
            // 
            resources.ApplyResources(this.BUT_Find, "BUT_Find");
            this.BUT_Find.Name = "BUT_Find";
            this.BUT_Find.UseVisualStyleBackColor = true;
            this.BUT_Find.Click += new System.EventHandler(this.BUT_Find_Click);
            // 
            // txt_acreg
            // 
            resources.ApplyResources(this.txt_acreg, "txt_acreg");
            this.txt_acreg.Name = "txt_acreg";
            // 
            // txt_flid
            // 
            resources.ApplyResources(this.txt_flid, "txt_flid");
            this.txt_flid.Name = "txt_flid";
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
            // but_saveacreg
            // 
            resources.ApplyResources(this.but_saveacreg, "but_saveacreg");
            this.but_saveacreg.Name = "but_saveacreg";
            this.but_saveacreg.UseVisualStyleBackColor = true;
            this.but_saveacreg.Click += new System.EventHandler(this.but_saveacreg_Click);
            // 
            // but_saveflid
            // 
            resources.ApplyResources(this.but_saveflid, "but_saveflid");
            this.but_saveflid.Name = "but_saveflid";
            this.but_saveflid.UseVisualStyleBackColor = true;
            this.but_saveflid.Click += new System.EventHandler(this.but_saveflid_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.but_saveflid);
            this.panel1.Controls.Add(this.txt_acreg);
            this.panel1.Controls.Add(this.but_saveacreg);
            this.panel1.Controls.Add(this.txt_flid);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // ConfigADSB
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.BUT_Find);
            this.Controls.Add(this.BUT_rerequestparams);
            this.Controls.Add(this.BUT_writePIDS);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ConfigADSB";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Panel tableLayoutPanel1;
      private Controls.MyButton BUT_rerequestparams;
      private Controls.MyButton BUT_writePIDS;
      private Controls.MyButton BUT_Find;
        private System.Windows.Forms.TextBox txt_acreg;
        private System.Windows.Forms.TextBox txt_flid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Controls.MyButton but_saveacreg;
        private Controls.MyButton but_saveflid;
        private System.Windows.Forms.Panel panel1;
    }
}
