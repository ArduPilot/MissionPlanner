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
            this.label7 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ekfvel
            // 
            this.ekfvel.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.ekfvel.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.ekfvel.DisplayScale = 0.01F;
            this.ekfvel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ekfvel.DrawLabel = false;
            this.ekfvel.Label = null;
            this.ekfvel.Location = new System.Drawing.Point(33, 48);
            this.ekfvel.Maximum = 100;
            this.ekfvel.maxline = 80;
            this.ekfvel.Minimum = 0;
            this.ekfvel.minline = 50;
            this.ekfvel.Name = "ekfvel";
            this.tableLayoutPanel1.SetRowSpan(this.ekfvel, 4);
            this.ekfvel.Size = new System.Drawing.Size(58, 174);
            this.ekfvel.TabIndex = 0;
            this.ekfvel.Value = 10;
            this.ekfvel.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // ekfposv
            // 
            this.ekfposv.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.ekfposv.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.ekfposv.DisplayScale = 0.01F;
            this.ekfposv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ekfposv.DrawLabel = false;
            this.ekfposv.Label = null;
            this.ekfposv.Location = new System.Drawing.Point(161, 48);
            this.ekfposv.Maximum = 100;
            this.ekfposv.maxline = 80;
            this.ekfposv.Minimum = 0;
            this.ekfposv.minline = 50;
            this.ekfposv.Name = "ekfposv";
            this.tableLayoutPanel1.SetRowSpan(this.ekfposv, 4);
            this.ekfposv.Size = new System.Drawing.Size(58, 174);
            this.ekfposv.TabIndex = 1;
            this.ekfposv.Value = 10;
            this.ekfposv.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // ekfposh
            // 
            this.ekfposh.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.ekfposh.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.ekfposh.DisplayScale = 0.01F;
            this.ekfposh.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ekfposh.DrawLabel = false;
            this.ekfposh.Label = null;
            this.ekfposh.Location = new System.Drawing.Point(97, 48);
            this.ekfposh.Maximum = 100;
            this.ekfposh.maxline = 80;
            this.ekfposh.Minimum = 0;
            this.ekfposh.minline = 50;
            this.ekfposh.Name = "ekfposh";
            this.tableLayoutPanel1.SetRowSpan(this.ekfposh, 4);
            this.ekfposh.Size = new System.Drawing.Size(58, 174);
            this.ekfposh.TabIndex = 2;
            this.ekfposh.Value = 10;
            this.ekfposh.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.81215F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.81215F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.81215F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.81215F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.81215F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.46962F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.46962F));
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
            this.tableLayoutPanel1.Controls.Add(this.label7, 6, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(498, 274);
            this.tableLayoutPanel1.TabIndex = 3;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 5);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(33, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(314, 45);
            this.label1.TabIndex = 0;
            this.label1.Text = "EKF Status";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label8, 2);
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(353, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(142, 45);
            this.label8.TabIndex = 11;
            this.label8.Text = "Flags";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ekfcompass
            // 
            this.ekfcompass.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.ekfcompass.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.ekfcompass.DisplayScale = 0.01F;
            this.ekfcompass.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ekfcompass.DrawLabel = false;
            this.ekfcompass.Label = null;
            this.ekfcompass.Location = new System.Drawing.Point(225, 48);
            this.ekfcompass.Maximum = 100;
            this.ekfcompass.maxline = 80;
            this.ekfcompass.Minimum = 0;
            this.ekfcompass.minline = 50;
            this.ekfcompass.Name = "ekfcompass";
            this.tableLayoutPanel1.SetRowSpan(this.ekfcompass, 4);
            this.ekfcompass.Size = new System.Drawing.Size(58, 174);
            this.ekfcompass.TabIndex = 17;
            this.ekfcompass.Value = 10;
            this.ekfcompass.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // ekfterrain
            // 
            this.ekfterrain.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.ekfterrain.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.ekfterrain.DisplayScale = 0.01F;
            this.ekfterrain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ekfterrain.DrawLabel = false;
            this.ekfterrain.Label = null;
            this.ekfterrain.Location = new System.Drawing.Point(289, 48);
            this.ekfterrain.Maximum = 100;
            this.ekfterrain.maxline = 80;
            this.ekfterrain.Minimum = 0;
            this.ekfterrain.minline = 50;
            this.ekfterrain.Name = "ekfterrain";
            this.tableLayoutPanel1.SetRowSpan(this.ekfterrain, 4);
            this.ekfterrain.Size = new System.Drawing.Size(58, 174);
            this.ekfterrain.TabIndex = 18;
            this.ekfterrain.Value = 10;
            this.ekfterrain.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(33, 225);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 49);
            this.label2.TabIndex = 19;
            this.label2.Text = "Velocity";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(97, 225);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 49);
            this.label3.TabIndex = 20;
            this.label3.Text = "Position (Horiz)";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(161, 225);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 49);
            this.label4.TabIndex = 21;
            this.label4.Text = "Position (Vert)";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(225, 225);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 49);
            this.label5.TabIndex = 22;
            this.label5.Text = "Compass";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(289, 225);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 49);
            this.label6.TabIndex = 23;
            this.label6.Text = "Terrain";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label7, 2);
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(353, 90);
            this.label7.Name = "label7";
            this.tableLayoutPanel1.SetRowSpan(this.label7, 3);
            this.label7.Size = new System.Drawing.Size(142, 135);
            this.label7.TabIndex = 24;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // EKFStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(498, 274);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "EKFStatus";
            this.ShowIcon = false;
            this.Text = "EKF Status";
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
        private System.Windows.Forms.Label label7;
    }
}