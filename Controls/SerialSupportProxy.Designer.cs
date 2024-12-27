namespace MissionPlanner.Controls
{
    partial class SerialSupportProxy
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TXT_host = new System.Windows.Forms.TextBox();
            this.NUM_port = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.BUT_connect = new MissionPlanner.Controls.MyButton();
            this.label3 = new System.Windows.Forms.Label();
            this.rad_udp = new System.Windows.Forms.RadioButton();
            this.rad_tcp = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_port)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Support ID";
            // 
            // TXT_host
            // 
            this.TXT_host.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this.TXT_host, 2);
            this.TXT_host.Location = new System.Drawing.Point(71, 7);
            this.TXT_host.Name = "TXT_host";
            this.TXT_host.Size = new System.Drawing.Size(158, 20);
            this.TXT_host.TabIndex = 2;
            this.TXT_host.Text = "support.ardupilot.org";
            // 
            // NUM_port
            // 
            this.NUM_port.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel1.SetColumnSpan(this.NUM_port, 2);
            this.NUM_port.InterceptArrowKeys = false;
            this.NUM_port.Location = new System.Drawing.Point(71, 33);
            this.NUM_port.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.NUM_port.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUM_port.Name = "NUM_port";
            this.NUM_port.Size = new System.Drawing.Size(62, 20);
            this.NUM_port.TabIndex = 3;
            this.NUM_port.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.rad_tcp, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.TXT_host, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.NUM_port, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.BUT_connect, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.rad_udp, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(317, 83);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // BUT_connect
            // 
            this.BUT_connect.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BUT_connect.Location = new System.Drawing.Point(235, 18);
            this.BUT_connect.Name = "BUT_connect";
            this.tableLayoutPanel1.SetRowSpan(this.BUT_connect, 2);
            this.BUT_connect.Size = new System.Drawing.Size(75, 23);
            this.BUT_connect.TabIndex = 4;
            this.BUT_connect.Text = "Connect";
            this.BUT_connect.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_connect.UseVisualStyleBackColor = true;
            this.BUT_connect.Click += new System.EventHandler(this.BUT_connect_Click);
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Protocol";
            // 
            // rad_udp
            // 
            this.rad_udp.AutoSize = true;
            this.rad_udp.Checked = true;
            this.rad_udp.Location = new System.Drawing.Point(71, 59);
            this.rad_udp.Name = "rad_udp";
            this.rad_udp.Size = new System.Drawing.Size(48, 17);
            this.rad_udp.TabIndex = 6;
            this.rad_udp.TabStop = true;
            this.rad_udp.Text = "UDP";
            this.rad_udp.UseVisualStyleBackColor = true;
            // 
            // rad_tcp
            // 
            this.rad_tcp.AutoSize = true;
            this.rad_tcp.Location = new System.Drawing.Point(153, 59);
            this.rad_tcp.Name = "rad_tcp";
            this.rad_tcp.Size = new System.Drawing.Size(46, 17);
            this.rad_tcp.TabIndex = 7;
            this.rad_tcp.Text = "TCP";
            this.rad_tcp.UseVisualStyleBackColor = true;
            // 
            // SerialSupportProxy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(317, 83);
            this.Controls.Add(this.tableLayoutPanel1);
            this.MaximizeBox = false;
            this.Name = "SerialSupportProxy";
            this.ShowIcon = false;
            this.Text = "Support Proxy Connection";
            ((System.ComponentModel.ISupportInitialize)(this.NUM_port)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox TXT_host;
        private System.Windows.Forms.NumericUpDown NUM_port;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private MyButton BUT_connect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton rad_udp;
        private System.Windows.Forms.RadioButton rad_tcp;
    }
}