namespace MissionPlanner.Controls
{
    partial class DevopsUI
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
            this.but_doit = new MissionPlanner.Controls.MyButton();
            this.num_sysid = new System.Windows.Forms.NumericUpDown();
            this.num_compid = new System.Windows.Forms.NumericUpDown();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.dom_bustype = new System.Windows.Forms.DomainUpDown();
            this.txt_spiname = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.num_busno = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.num_address = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.num_regstart = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.num_count = new System.Windows.Forms.NumericUpDown();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.but_test = new MissionPlanner.Controls.MyButton();
            ((System.ComponentModel.ISupportInitialize)(this.num_sysid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_compid)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_busno)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_address)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_regstart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_count)).BeginInit();
            this.SuspendLayout();
            // 
            // but_doit
            // 
            this.but_doit.Location = new System.Drawing.Point(526, 6);
            this.but_doit.Name = "but_doit";
            this.but_doit.Size = new System.Drawing.Size(75, 23);
            this.but_doit.TabIndex = 0;
            this.but_doit.Text = "Do It";
            this.but_doit.UseVisualStyleBackColor = true;
            this.but_doit.Click += new System.EventHandler(this.but_doit_Click);
            // 
            // num_sysid
            // 
            this.num_sysid.Location = new System.Drawing.Point(39, 3);
            this.num_sysid.Name = "num_sysid";
            this.num_sysid.Size = new System.Drawing.Size(62, 20);
            this.num_sysid.TabIndex = 1;
            this.num_sysid.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // num_compid
            // 
            this.num_compid.Location = new System.Drawing.Point(154, 3);
            this.num_compid.Name = "num_compid";
            this.num_compid.Size = new System.Drawing.Size(62, 20);
            this.num_compid.TabIndex = 2;
            this.num_compid.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Controls.Add(this.num_sysid);
            this.flowLayoutPanel1.Controls.Add(this.label2);
            this.flowLayoutPanel1.Controls.Add(this.num_compid);
            this.flowLayoutPanel1.Controls.Add(this.label3);
            this.flowLayoutPanel1.Controls.Add(this.dom_bustype);
            this.flowLayoutPanel1.Controls.Add(this.txt_spiname);
            this.flowLayoutPanel1.Controls.Add(this.label4);
            this.flowLayoutPanel1.Controls.Add(this.num_busno);
            this.flowLayoutPanel1.Controls.Add(this.label5);
            this.flowLayoutPanel1.Controls.Add(this.num_address);
            this.flowLayoutPanel1.Controls.Add(this.label6);
            this.flowLayoutPanel1.Controls.Add(this.num_regstart);
            this.flowLayoutPanel1.Controls.Add(this.label7);
            this.flowLayoutPanel1.Controls.Add(this.num_count);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(465, 58);
            this.flowLayoutPanel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "sysid";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(107, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "compid";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(222, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "bus type";
            // 
            // dom_bustype
            // 
            this.dom_bustype.Items.Add("I2C");
            this.dom_bustype.Items.Add("SPI");
            this.dom_bustype.Location = new System.Drawing.Point(275, 3);
            this.dom_bustype.Name = "dom_bustype";
            this.dom_bustype.Size = new System.Drawing.Size(62, 20);
            this.dom_bustype.TabIndex = 3;
            this.dom_bustype.Text = "SPI";
            this.dom_bustype.SelectedItemChanged += new System.EventHandler(this.dom_bustype_SelectedItemChanged);
            // 
            // txt_spiname
            // 
            this.txt_spiname.Location = new System.Drawing.Point(343, 3);
            this.txt_spiname.Name = "txt_spiname";
            this.txt_spiname.Size = new System.Drawing.Size(100, 20);
            this.txt_spiname.TabIndex = 15;
            this.txt_spiname.Text = "icm20948_ext";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "bus";
            // 
            // num_busno
            // 
            this.num_busno.Location = new System.Drawing.Point(33, 29);
            this.num_busno.Name = "num_busno";
            this.num_busno.Size = new System.Drawing.Size(62, 20);
            this.num_busno.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(101, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "address";
            // 
            // num_address
            // 
            this.num_address.Location = new System.Drawing.Point(151, 29);
            this.num_address.Name = "num_address";
            this.num_address.Size = new System.Drawing.Size(62, 20);
            this.num_address.TabIndex = 5;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(219, 26);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "regstart";
            // 
            // num_regstart
            // 
            this.num_regstart.Location = new System.Drawing.Point(267, 29);
            this.num_regstart.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.num_regstart.Name = "num_regstart";
            this.num_regstart.Size = new System.Drawing.Size(62, 20);
            this.num_regstart.TabIndex = 6;
            this.num_regstart.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(335, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "count";
            // 
            // num_count
            // 
            this.num_count.Location = new System.Drawing.Point(375, 29);
            this.num_count.Name = "num_count";
            this.num_count.Size = new System.Drawing.Size(62, 20);
            this.num_count.TabIndex = 7;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(3, 67);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(465, 144);
            this.textBox1.TabIndex = 4;
            // 
            // but_test
            // 
            this.but_test.Location = new System.Drawing.Point(513, 76);
            this.but_test.Name = "but_test";
            this.but_test.Size = new System.Drawing.Size(75, 23);
            this.but_test.TabIndex = 5;
            this.but_test.Text = "test";
            this.but_test.UseVisualStyleBackColor = true;
            this.but_test.Click += new System.EventHandler(this.but_test_Click);
            // 
            // DevopsUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.but_test);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.but_doit);
            this.Name = "DevopsUI";
            this.Size = new System.Drawing.Size(604, 265);
            ((System.ComponentModel.ISupportInitialize)(this.num_sysid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_compid)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_busno)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_address)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_regstart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_count)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MyButton but_doit;
        private System.Windows.Forms.NumericUpDown num_sysid;
        private System.Windows.Forms.NumericUpDown num_compid;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DomainUpDown dom_bustype;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown num_busno;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown num_address;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown num_regstart;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown num_count;
        private System.Windows.Forms.TextBox txt_spiname;
        private System.Windows.Forms.TextBox textBox1;
        private MyButton but_test;
    }
}
