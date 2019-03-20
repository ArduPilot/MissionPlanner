namespace MissionPlanner.Controls
{
    partial class EKFStatus
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EKFStatus));
            this.ekfvel = new MissionPlanner.Controls.VerticalProgressBar2();
            this.ekfposv = new MissionPlanner.Controls.VerticalProgressBar2();
            this.ekfposh = new MissionPlanner.Controls.VerticalProgressBar2();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.ekfcompass = new MissionPlanner.Controls.VerticalProgressBar2();
            this.ekfterrain = new MissionPlanner.Controls.VerticalProgressBar2();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ekfvel
            // 
            this.ekfvel.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.ekfvel.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.ekfvel.DisplayScale = 0.01F;
            resources.ApplyResources(this.ekfvel, "ekfvel");
            this.ekfvel.DrawLabel = false;
            this.ekfvel.Label = null;
            this.ekfvel.Maximum = 100;
            this.ekfvel.maxline = 80;
            this.ekfvel.Minimum = 0;
            this.ekfvel.minline = 50;
            this.ekfvel.Name = "ekfvel";
            this.tableLayoutPanel1.SetRowSpan(this.ekfvel, 4);
            this.ekfvel.Value = 10;
            this.ekfvel.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // ekfposv
            // 
            this.ekfposv.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.ekfposv.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.ekfposv.DisplayScale = 0.01F;
            resources.ApplyResources(this.ekfposv, "ekfposv");
            this.ekfposv.DrawLabel = false;
            this.ekfposv.Label = null;
            this.ekfposv.Maximum = 100;
            this.ekfposv.maxline = 80;
            this.ekfposv.Minimum = 0;
            this.ekfposv.minline = 50;
            this.ekfposv.Name = "ekfposv";
            this.tableLayoutPanel1.SetRowSpan(this.ekfposv, 4);
            this.ekfposv.Value = 10;
            this.ekfposv.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // ekfposh
            // 
            this.ekfposh.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.ekfposh.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.ekfposh.DisplayScale = 0.01F;
            resources.ApplyResources(this.ekfposh, "ekfposh");
            this.ekfposh.DrawLabel = false;
            this.ekfposh.Label = null;
            this.ekfposh.Maximum = 100;
            this.ekfposh.maxline = 80;
            this.ekfposh.Minimum = 0;
            this.ekfposh.minline = 50;
            this.ekfposh.Name = "ekfposh";
            this.tableLayoutPanel1.SetRowSpan(this.ekfposh, 4);
            this.ekfposh.Value = 10;
            this.ekfposh.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.ekfposv, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.ekfposh, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.ekfvel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label8, 6, 1);
            this.tableLayoutPanel1.Controls.Add(this.ekfcompass, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.ekfterrain, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label3, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.label4, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.label5, 4, 5);
            this.tableLayoutPanel1.Controls.Add(this.label6, 5, 5);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 6, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 5);
            this.label1.Name = "label1";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.tableLayoutPanel1.SetColumnSpan(this.label8, 2);
            this.label8.Name = "label8";
            // 
            // ekfcompass
            // 
            this.ekfcompass.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.ekfcompass.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.ekfcompass.DisplayScale = 0.01F;
            resources.ApplyResources(this.ekfcompass, "ekfcompass");
            this.ekfcompass.DrawLabel = false;
            this.ekfcompass.Label = null;
            this.ekfcompass.Maximum = 100;
            this.ekfcompass.maxline = 80;
            this.ekfcompass.Minimum = 0;
            this.ekfcompass.minline = 50;
            this.ekfcompass.Name = "ekfcompass";
            this.tableLayoutPanel1.SetRowSpan(this.ekfcompass, 4);
            this.ekfcompass.Value = 10;
            this.ekfcompass.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // ekfterrain
            // 
            this.ekfterrain.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.ekfterrain.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.ekfterrain.DisplayScale = 0.01F;
            resources.ApplyResources(this.ekfterrain, "ekfterrain");
            this.ekfterrain.DrawLabel = false;
            this.ekfterrain.Label = null;
            this.ekfterrain.Maximum = 100;
            this.ekfterrain.maxline = 80;
            this.ekfterrain.Minimum = 0;
            this.ekfterrain.minline = 50;
            this.ekfterrain.Name = "ekfterrain";
            this.tableLayoutPanel1.SetRowSpan(this.ekfterrain, 4);
            this.ekfterrain.Value = 10;
            this.ekfterrain.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
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
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.tableLayoutPanel1.SetRowSpan(this.flowLayoutPanel1, 3);
            // 
            // EKFStatus
            // 
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EKFStatus";
            this.ShowIcon = false;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private VerticalProgressBar2 ekfvel;
        private VerticalProgressBar2 ekfposv;
        private VerticalProgressBar2 ekfposh;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Timer timer1;
        private VerticalProgressBar2 ekfcompass;
        private VerticalProgressBar2 ekfterrain;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    }
}