namespace MissionPlanner.Controls
{
    partial class Vibration
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Vibration));
            this.VibBarX = new MissionPlanner.Controls.VerticalProgressBar2();
            this.VibBarZ = new MissionPlanner.Controls.VerticalProgressBar2();
            this.VibBarY = new MissionPlanner.Controls.VerticalProgressBar2();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txt_clip0 = new System.Windows.Forms.TextBox();
            this.txt_clip1 = new System.Windows.Forms.TextBox();
            this.txt_clip2 = new System.Windows.Forms.TextBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // VibBarX
            // 
            this.VibBarX.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.VibBarX.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.VibBarX.DisplayScale = 1F;
            resources.ApplyResources(this.VibBarX, "VibBarX");
            this.VibBarX.DrawLabel = false;
            this.VibBarX.Label = null;
            this.VibBarX.Maximum = 90;
            this.VibBarX.maxline = 60;
            this.VibBarX.Minimum = 0;
            this.VibBarX.minline = 30;
            this.VibBarX.Name = "VibBarX";
            this.tableLayoutPanel1.SetRowSpan(this.VibBarX, 4);
            this.VibBarX.Value = 10;
            this.VibBarX.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // VibBarZ
            // 
            this.VibBarZ.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.VibBarZ.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.VibBarZ.DisplayScale = 1F;
            resources.ApplyResources(this.VibBarZ, "VibBarZ");
            this.VibBarZ.DrawLabel = false;
            this.VibBarZ.Label = null;
            this.VibBarZ.Maximum = 90;
            this.VibBarZ.maxline = 60;
            this.VibBarZ.Minimum = 0;
            this.VibBarZ.minline = 30;
            this.VibBarZ.Name = "VibBarZ";
            this.tableLayoutPanel1.SetRowSpan(this.VibBarZ, 4);
            this.VibBarZ.Value = 10;
            this.VibBarZ.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // VibBarY
            // 
            this.VibBarY.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.VibBarY.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.VibBarY.DisplayScale = 1F;
            resources.ApplyResources(this.VibBarY, "VibBarY");
            this.VibBarY.DrawLabel = false;
            this.VibBarY.Label = null;
            this.VibBarY.Maximum = 90;
            this.VibBarY.maxline = 60;
            this.VibBarY.Minimum = 0;
            this.VibBarY.minline = 30;
            this.VibBarY.Name = "VibBarY";
            this.tableLayoutPanel1.SetRowSpan(this.VibBarY, 4);
            this.VibBarY.Value = 10;
            this.VibBarY.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.VibBarZ, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.VibBarY, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.VibBarX, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.label5, 3, 5);
            this.tableLayoutPanel1.Controls.Add(this.label4, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.label3, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.label7, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.label2, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.label6, 4, 4);
            this.tableLayoutPanel1.Controls.Add(this.label8, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.txt_clip0, 5, 2);
            this.tableLayoutPanel1.Controls.Add(this.txt_clip1, 5, 3);
            this.tableLayoutPanel1.Controls.Add(this.txt_clip2, 5, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Name = "label1";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.tableLayoutPanel1.SetColumnSpan(this.label8, 2);
            this.label8.Name = "label8";
            // 
            // txt_clip0
            // 
            resources.ApplyResources(this.txt_clip0, "txt_clip0");
            this.txt_clip0.Name = "txt_clip0";
            // 
            // txt_clip1
            // 
            resources.ApplyResources(this.txt_clip1, "txt_clip1");
            this.txt_clip1.Name = "txt_clip1";
            // 
            // txt_clip2
            // 
            resources.ApplyResources(this.txt_clip2, "txt_clip2");
            this.txt_clip2.Name = "txt_clip2";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Vibration
            // 
            
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Vibration";
            this.ShowIcon = false;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private VerticalProgressBar2 VibBarX;
        private VerticalProgressBar2 VibBarZ;
        private VerticalProgressBar2 VibBarY;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt_clip0;
        private System.Windows.Forms.TextBox txt_clip1;
        private System.Windows.Forms.TextBox txt_clip2;
        private System.Windows.Forms.Timer timer1;
    }
}