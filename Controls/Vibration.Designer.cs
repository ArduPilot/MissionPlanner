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
            this.VibBarX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VibBarX.DrawLabel = false;
            this.VibBarX.Label = null;
            this.VibBarX.Location = new System.Drawing.Point(33, 48);
            this.VibBarX.Maximum = 90;
            this.VibBarX.maxline = 60;
            this.VibBarX.Minimum = 0;
            this.VibBarX.minline = 30;
            this.VibBarX.Name = "VibBarX";
            this.tableLayoutPanel1.SetRowSpan(this.VibBarX, 4);
            this.VibBarX.Size = new System.Drawing.Size(62, 174);
            this.VibBarX.TabIndex = 0;
            this.VibBarX.Value = 10;
            this.VibBarX.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // VibBarZ
            // 
            this.VibBarZ.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.VibBarZ.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.VibBarZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VibBarZ.DrawLabel = false;
            this.VibBarZ.Label = null;
            this.VibBarZ.Location = new System.Drawing.Point(169, 48);
            this.VibBarZ.Maximum = 90;
            this.VibBarZ.maxline = 60;
            this.VibBarZ.Minimum = 0;
            this.VibBarZ.minline = 30;
            this.VibBarZ.Name = "VibBarZ";
            this.tableLayoutPanel1.SetRowSpan(this.VibBarZ, 4);
            this.VibBarZ.Size = new System.Drawing.Size(62, 174);
            this.VibBarZ.TabIndex = 1;
            this.VibBarZ.Value = 10;
            this.VibBarZ.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // VibBarY
            // 
            this.VibBarY.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(255)))));
            this.VibBarY.BorderColor = System.Drawing.SystemColors.ActiveBorder;
            this.VibBarY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.VibBarY.DrawLabel = false;
            this.VibBarY.Label = null;
            this.VibBarY.Location = new System.Drawing.Point(101, 48);
            this.VibBarY.Maximum = 90;
            this.VibBarY.maxline = 60;
            this.VibBarY.Minimum = 0;
            this.VibBarY.minline = 30;
            this.VibBarY.Name = "VibBarY";
            this.tableLayoutPanel1.SetRowSpan(this.VibBarY, 4);
            this.VibBarY.Size = new System.Drawing.Size(62, 174);
            this.VibBarY.TabIndex = 2;
            this.VibBarY.Value = 10;
            this.VibBarY.ValueColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.05264F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.05263F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.05263F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.05263F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 15.78947F));
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
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(355, 274);
            this.tableLayoutPanel1.TabIndex = 3;
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label1, 3);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(33, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(198, 45);
            this.label1.TabIndex = 0;
            this.label1.Text = "Vibration";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(169, 225);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 49);
            this.label5.TabIndex = 5;
            this.label5.Text = "Z";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.Location = new System.Drawing.Point(101, 225);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(62, 49);
            this.label4.TabIndex = 4;
            this.label4.Text = "Y";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(33, 225);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 49);
            this.label3.TabIndex = 6;
            this.label3.Text = "X";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.Location = new System.Drawing.Point(237, 135);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 45);
            this.label7.TabIndex = 8;
            this.label7.Text = "Secondary";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(237, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 45);
            this.label2.TabIndex = 9;
            this.label2.Text = "Primary";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(237, 180);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(62, 45);
            this.label6.TabIndex = 10;
            this.label6.Text = "Tertiary";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label8, 2);
            this.label8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(237, 45);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(115, 45);
            this.label8.TabIndex = 11;
            this.label8.Text = "Clipping";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txt_clip0
            // 
            this.txt_clip0.Location = new System.Drawing.Point(305, 93);
            this.txt_clip0.Name = "txt_clip0";
            this.txt_clip0.Size = new System.Drawing.Size(47, 20);
            this.txt_clip0.TabIndex = 12;
            // 
            // txt_clip1
            // 
            this.txt_clip1.Location = new System.Drawing.Point(305, 138);
            this.txt_clip1.Name = "txt_clip1";
            this.txt_clip1.Size = new System.Drawing.Size(47, 20);
            this.txt_clip1.TabIndex = 13;
            // 
            // txt_clip2
            // 
            this.txt_clip2.Location = new System.Drawing.Point(305, 183);
            this.txt_clip2.Name = "txt_clip2";
            this.txt_clip2.Size = new System.Drawing.Size(47, 20);
            this.txt_clip2.TabIndex = 14;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // Vibration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(355, 274);
            this.Controls.Add(this.tableLayoutPanel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "Vibration";
            this.ShowIcon = false;
            this.Text = "Vibration";
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