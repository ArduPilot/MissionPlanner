namespace MissionPlanner.Controls
{
    partial class ThemeColors
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ThemeColors));
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.TXT_bg = new System.Windows.Forms.TextBox();
            this.TXT_ctlbg = new System.Windows.Forms.TextBox();
            this.TXT_text = new System.Windows.Forms.TextBox();
            this.TXT_butbg = new System.Windows.Forms.TextBox();
            this.TXT_butbord = new System.Windows.Forms.TextBox();
            this.BUT_butbord = new MissionPlanner.Controls.MyButton();
            this.BUT_butbg = new MissionPlanner.Controls.MyButton();
            this.BUT_text = new MissionPlanner.Controls.MyButton();
            this.BUT_ctlbg = new MissionPlanner.Controls.MyButton();
            this.BUT_bg = new MissionPlanner.Controls.MyButton();
            this.BUT_done = new MissionPlanner.Controls.MyButton();
            this.SuspendLayout();
            // 
            // TXT_bg
            // 
            resources.ApplyResources(this.TXT_bg, "TXT_bg");
            this.TXT_bg.Name = "TXT_bg";
            // 
            // TXT_ctlbg
            // 
            resources.ApplyResources(this.TXT_ctlbg, "TXT_ctlbg");
            this.TXT_ctlbg.Name = "TXT_ctlbg";
            // 
            // TXT_text
            // 
            resources.ApplyResources(this.TXT_text, "TXT_text");
            this.TXT_text.Name = "TXT_text";
            // 
            // TXT_butbg
            // 
            resources.ApplyResources(this.TXT_butbg, "TXT_butbg");
            this.TXT_butbg.Name = "TXT_butbg";
            // 
            // TXT_butbord
            // 
            resources.ApplyResources(this.TXT_butbord, "TXT_butbord");
            this.TXT_butbord.Name = "TXT_butbord";
            // 
            // BUT_butbord
            // 
            resources.ApplyResources(this.BUT_butbord, "BUT_butbord");
            this.BUT_butbord.Name = "BUT_butbord";
            this.BUT_butbord.UseVisualStyleBackColor = true;
            this.BUT_butbord.Click += new System.EventHandler(this.BUT_butbord_Click);
            // 
            // BUT_butbg
            // 
            resources.ApplyResources(this.BUT_butbg, "BUT_butbg");
            this.BUT_butbg.Name = "BUT_butbg";
            this.BUT_butbg.UseVisualStyleBackColor = true;
            this.BUT_butbg.Click += new System.EventHandler(this.BUT_butbg_Click);
            // 
            // BUT_text
            // 
            resources.ApplyResources(this.BUT_text, "BUT_text");
            this.BUT_text.Name = "BUT_text";
            this.BUT_text.UseVisualStyleBackColor = true;
            this.BUT_text.Click += new System.EventHandler(this.BUT_text_Click);
            // 
            // BUT_ctlbg
            // 
            resources.ApplyResources(this.BUT_ctlbg, "BUT_ctlbg");
            this.BUT_ctlbg.Name = "BUT_ctlbg";
            this.BUT_ctlbg.UseVisualStyleBackColor = true;
            this.BUT_ctlbg.Click += new System.EventHandler(this.BUT_ctlbg_Click);
            // 
            // BUT_bg
            // 
            resources.ApplyResources(this.BUT_bg, "BUT_bg");
            this.BUT_bg.Name = "BUT_bg";
            this.BUT_bg.UseVisualStyleBackColor = true;
            this.BUT_bg.Click += new System.EventHandler(this.BUT_bg_Click);
            // 
            // BUT_done
            // 
            resources.ApplyResources(this.BUT_done, "BUT_done");
            this.BUT_done.Name = "BUT_done";
            this.BUT_done.UseVisualStyleBackColor = true;
            this.BUT_done.Click += new System.EventHandler(this.BUT_done_Click);
            // 
            // ThemeColors
            // 
            resources.ApplyResources(this, "$this");
            
            this.Controls.Add(this.BUT_done);
            this.Controls.Add(this.TXT_butbord);
            this.Controls.Add(this.BUT_butbord);
            this.Controls.Add(this.TXT_butbg);
            this.Controls.Add(this.TXT_text);
            this.Controls.Add(this.TXT_ctlbg);
            this.Controls.Add(this.TXT_bg);
            this.Controls.Add(this.BUT_butbg);
            this.Controls.Add(this.BUT_text);
            this.Controls.Add(this.BUT_ctlbg);
            this.Controls.Add(this.BUT_bg);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ThemeColors";
            this.Load += new System.EventHandler(this.ThemeColors_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog1;
        private MyButton BUT_bg;
        private MyButton BUT_ctlbg;
        private MyButton BUT_text;
        private MyButton BUT_butbg;
        private System.Windows.Forms.TextBox TXT_bg;
        private System.Windows.Forms.TextBox TXT_ctlbg;
        private System.Windows.Forms.TextBox TXT_text;
        private System.Windows.Forms.TextBox TXT_butbg;
        private System.Windows.Forms.TextBox TXT_butbord;
        private MyButton BUT_butbord;
        private MyButton BUT_done;
    }
}