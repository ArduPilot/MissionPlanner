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
            ((System.ComponentModel.ISupportInitialize)(this.NUM_port)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(32, 12);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Server";
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 42);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Support ID";
            // 
            // TXT_host
            // 
            this.TXT_host.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_host.Location = new System.Drawing.Point(87, 9);
            this.TXT_host.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.TXT_host.Name = "TXT_host";
            this.TXT_host.Size = new System.Drawing.Size(219, 22);
            this.TXT_host.TabIndex = 2;
            this.TXT_host.Text = "support.ardupilot.org";
            // 
            // NUM_port
            // 
            this.NUM_port.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.NUM_port.InterceptArrowKeys = false;
            this.NUM_port.Location = new System.Drawing.Point(87, 39);
            this.NUM_port.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
            this.NUM_port.Size = new System.Drawing.Size(83, 22);
            this.NUM_port.TabIndex = 3;
            this.NUM_port.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.TXT_host, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.NUM_port, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.BUT_connect, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(423, 73);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // BUT_connect
            // 
            this.BUT_connect.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.BUT_connect.Location = new System.Drawing.Point(314, 21);
            this.BUT_connect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.BUT_connect.Name = "BUT_connect";
            this.tableLayoutPanel1.SetRowSpan(this.BUT_connect, 2);
            this.BUT_connect.Size = new System.Drawing.Size(100, 28);
            this.BUT_connect.TabIndex = 4;
            this.BUT_connect.Text = "Connect";
            this.BUT_connect.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.BUT_connect.UseVisualStyleBackColor = true;
            this.BUT_connect.Click += new System.EventHandler(this.BUT_connect_Click);
            // 
            // SerialSupportProxy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(423, 73);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
    }
}