namespace MissionPlanner.GCSViews.ConfigurationView
{
   partial class ConfigFriendlyParams
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfigFriendlyParams));
            this.tableLayoutPanel1 = new System.Windows.Forms.Panel();
            this.BUT_rerequestparams = new MissionPlanner.Controls.MyButton();
            this.BUT_writePIDS = new MissionPlanner.Controls.MyButton();
            this.BUT_Find = new MissionPlanner.Controls.MyButton();
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
            // 
            // BUT_writePIDS
            // 
            resources.ApplyResources(this.BUT_writePIDS, "BUT_writePIDS");
            this.BUT_writePIDS.Name = "BUT_writePIDS";
            this.BUT_writePIDS.UseVisualStyleBackColor = true;
            // 
            // BUT_Find
            // 
            resources.ApplyResources(this.BUT_Find, "BUT_Find");
            this.BUT_Find.Name = "BUT_Find";
            this.BUT_Find.UseVisualStyleBackColor = true;
            this.BUT_Find.Click += new System.EventHandler(this.BUT_Find_Click);
            // 
            // ConfigFriendlyParams
            // 
            resources.ApplyResources(this, "$this");
            
            this.Controls.Add(this.BUT_Find);
            this.Controls.Add(this.BUT_rerequestparams);
            this.Controls.Add(this.BUT_writePIDS);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ConfigFriendlyParams";
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Panel tableLayoutPanel1;
      private Controls.MyButton BUT_rerequestparams;
      private Controls.MyButton BUT_writePIDS;
      private Controls.MyButton BUT_Find;
   }
}
