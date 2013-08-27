namespace ArdupilotMega.GCSViews.ConfigurationView
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
            this.tableLayoutPanel1 = new System.Windows.Forms.Panel();
            this.BUT_rerequestparams = new ArdupilotMega.Controls.MyButton();
            this.BUT_writePIDS = new ArdupilotMega.Controls.MyButton();
            this.BUT_Find = new ArdupilotMega.Controls.MyButton();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.AutoScroll = true;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 36);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Size = new System.Drawing.Size(647, 141);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // BUT_rerequestparams
            // 
            this.BUT_rerequestparams.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_rerequestparams.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.BUT_rerequestparams.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BUT_rerequestparams.Location = new System.Drawing.Point(121, 11);
            this.BUT_rerequestparams.Name = "BUT_rerequestparams";
            this.BUT_rerequestparams.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_rerequestparams.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.BUT_rerequestparams.Size = new System.Drawing.Size(103, 19);
            this.BUT_rerequestparams.TabIndex = 73;
            this.BUT_rerequestparams.Text = "Refresh Params";
            this.BUT_rerequestparams.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_rerequestparams.UseVisualStyleBackColor = true;
            // 
            // BUT_writePIDS
            // 
            this.BUT_writePIDS.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_writePIDS.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.BUT_writePIDS.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BUT_writePIDS.Location = new System.Drawing.Point(12, 11);
            this.BUT_writePIDS.Name = "BUT_writePIDS";
            this.BUT_writePIDS.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_writePIDS.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.BUT_writePIDS.Size = new System.Drawing.Size(103, 19);
            this.BUT_writePIDS.TabIndex = 74;
            this.BUT_writePIDS.Text = "Write Params";
            this.BUT_writePIDS.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_writePIDS.UseVisualStyleBackColor = true;
            // 
            // BUT_Find
            // 
            this.BUT_Find.BGGradBot = System.Drawing.Color.FromArgb(((int)(((byte)(205)))), ((int)(((byte)(226)))), ((int)(((byte)(150)))));
            this.BUT_Find.BGGradTop = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(193)))), ((int)(((byte)(31)))));
            this.BUT_Find.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BUT_Find.Location = new System.Drawing.Point(230, 11);
            this.BUT_Find.Name = "BUT_Find";
            this.BUT_Find.Outline = System.Drawing.Color.FromArgb(((int)(((byte)(121)))), ((int)(((byte)(148)))), ((int)(((byte)(41)))));
            this.BUT_Find.Padding = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.BUT_Find.Size = new System.Drawing.Size(103, 19);
            this.BUT_Find.TabIndex = 75;
            this.BUT_Find.Text = "Find";
            this.BUT_Find.TextColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_Find.UseVisualStyleBackColor = true;
            this.BUT_Find.Click += new System.EventHandler(this.BUT_Find_Click);
            // 
            // ConfigFriendlyParams
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.BUT_Find);
            this.Controls.Add(this.BUT_rerequestparams);
            this.Controls.Add(this.BUT_writePIDS);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ConfigFriendlyParams";
            this.Size = new System.Drawing.Size(673, 186);
            this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.Panel tableLayoutPanel1;
      private Controls.MyButton BUT_rerequestparams;
      private Controls.MyButton BUT_writePIDS;
      private Controls.MyButton BUT_Find;
   }
}
