
namespace MissionPlanner.Controls
{
    partial class QuickViewOptions
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
            this.CMB_Source = new System.Windows.Forms.ComboBox();
            this.NUM_precision = new System.Windows.Forms.NumericUpDown();
            this.LBL_precision = new System.Windows.Forms.Label();
            this.CHK_customcolor = new System.Windows.Forms.CheckBox();
            this.TXT_color = new System.Windows.Forms.TextBox();
            this.BUT_colorpicker = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.CHK_customwidth = new System.Windows.Forms.CheckBox();
            this.NUM_customwidth = new System.Windows.Forms.NumericUpDown();
            this.CHK_customlabel = new System.Windows.Forms.CheckBox();
            this.TXT_customformat = new System.Windows.Forms.TextBox();
            this.TXT_customlabel = new System.Windows.Forms.TextBox();
            this.TXT_offset = new System.Windows.Forms.TextBox();
            this.TXT_scale = new System.Windows.Forms.TextBox();
            this.CHK_customformat = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.NUM_precision)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_customwidth)).BeginInit();
            this.SuspendLayout();
            // 
            // CMB_Source
            // 
            this.CMB_Source.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CMB_Source.FormattingEnabled = true;
            this.CMB_Source.Location = new System.Drawing.Point(12, 12);
            this.CMB_Source.Name = "CMB_Source";
            this.CMB_Source.Size = new System.Drawing.Size(153, 21);
            this.CMB_Source.TabIndex = 1;
            this.CMB_Source.TextUpdate += new System.EventHandler(this.CMB_Source_TextUpdate);
            this.CMB_Source.DropDownClosed += new System.EventHandler(this.CMB_Source_DropDownClosed);
            // 
            // NUM_precision
            // 
            this.NUM_precision.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.NUM_precision.Location = new System.Drawing.Point(229, 12);
            this.NUM_precision.Maximum = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.NUM_precision.Name = "NUM_precision";
            this.NUM_precision.Size = new System.Drawing.Size(40, 20);
            this.NUM_precision.TabIndex = 3;
            this.toolTip1.SetToolTip(this.NUM_precision, "How many decimal places to show");
            this.NUM_precision.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.NUM_precision.ValueChanged += new System.EventHandler(this.NUM_precision_ValueChanged);
            // 
            // LBL_precision
            // 
            this.LBL_precision.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LBL_precision.AutoSize = true;
            this.LBL_precision.Location = new System.Drawing.Point(171, 15);
            this.LBL_precision.Name = "LBL_precision";
            this.LBL_precision.Size = new System.Drawing.Size(53, 13);
            this.LBL_precision.TabIndex = 4;
            this.LBL_precision.Text = "Precision:";
            this.toolTip1.SetToolTip(this.LBL_precision, "How many decimal places to show");
            // 
            // CHK_customcolor
            // 
            this.CHK_customcolor.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CHK_customcolor.AutoSize = true;
            this.CHK_customcolor.Location = new System.Drawing.Point(3, 36);
            this.CHK_customcolor.Name = "CHK_customcolor";
            this.CHK_customcolor.Size = new System.Drawing.Size(88, 17);
            this.CHK_customcolor.TabIndex = 5;
            this.CHK_customcolor.Text = "Custom Color";
            this.toolTip1.SetToolTip(this.CHK_customcolor, "Apply custom text color");
            this.CHK_customcolor.UseVisualStyleBackColor = true;
            this.CHK_customcolor.CheckedChanged += new System.EventHandler(this.CHK_customcolor_CheckedChanged);
            // 
            // TXT_color
            // 
            this.TXT_color.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_color.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TXT_color.Location = new System.Drawing.Point(105, 38);
            this.TXT_color.Name = "TXT_color";
            this.TXT_color.Size = new System.Drawing.Size(118, 13);
            this.TXT_color.TabIndex = 6;
            this.TXT_color.Text = "FFFFFF";
            this.TXT_color.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.TXT_color, "Apply custom text color");
            this.TXT_color.TextChanged += new System.EventHandler(this.TXT_color_TextChanged);
            // 
            // BUT_colorpicker
            // 
            this.BUT_colorpicker.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.BUT_colorpicker.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.BUT_colorpicker.Location = new System.Drawing.Point(229, 35);
            this.BUT_colorpicker.Name = "BUT_colorpicker";
            this.BUT_colorpicker.Size = new System.Drawing.Size(19, 20);
            this.BUT_colorpicker.TabIndex = 7;
            this.toolTip1.SetToolTip(this.BUT_colorpicker, "Apply custom text color");
            this.BUT_colorpicker.UseVisualStyleBackColor = false;
            this.BUT_colorpicker.Click += new System.EventHandler(this.BUT_colorpicker_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(12, 39);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(257, 200);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Advanced";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this.CHK_customwidth, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.NUM_customwidth, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.CHK_customcolor, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.TXT_color, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.BUT_colorpicker, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.CHK_customlabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.TXT_customformat, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.TXT_customlabel, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.TXT_offset, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this.TXT_scale, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this.CHK_customformat, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(251, 181);
            this.tableLayoutPanel1.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 128);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Scale";
            this.toolTip1.SetToolTip(this.label1, "Apply scale and offset to convert units.");
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 158);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Offset";
            this.toolTip1.SetToolTip(this.label2, "Apply scale and offset to convert units.");
            // 
            // CHK_customwidth
            // 
            this.CHK_customwidth.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CHK_customwidth.AutoSize = true;
            this.CHK_customwidth.Location = new System.Drawing.Point(3, 96);
            this.CHK_customwidth.Name = "CHK_customwidth";
            this.CHK_customwidth.Size = new System.Drawing.Size(92, 17);
            this.CHK_customwidth.TabIndex = 9;
            this.CHK_customwidth.Text = "Custom Width";
            this.toolTip1.SetToolTip(this.CHK_customwidth, "Specify the maximum number of digits wide you expect the number to be. This provi" +
        "des more consistent text size.");
            this.CHK_customwidth.UseVisualStyleBackColor = true;
            this.CHK_customwidth.CheckedChanged += new System.EventHandler(this.CHK_customwidth_CheckedChanged);
            // 
            // NUM_customwidth
            // 
            this.NUM_customwidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.NUM_customwidth.Location = new System.Drawing.Point(105, 95);
            this.NUM_customwidth.Maximum = new decimal(new int[] {
            9,
            0,
            0,
            0});
            this.NUM_customwidth.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.NUM_customwidth.Name = "NUM_customwidth";
            this.NUM_customwidth.Size = new System.Drawing.Size(118, 20);
            this.NUM_customwidth.TabIndex = 9;
            this.NUM_customwidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.NUM_customwidth, "Specify the maximum number of digits wide you expect the number to be. This provi" +
        "des more consistent text size.");
            this.NUM_customwidth.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            this.NUM_customwidth.ValueChanged += new System.EventHandler(this.NUM_customwidth_ValueChanged);
            // 
            // CHK_customlabel
            // 
            this.CHK_customlabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CHK_customlabel.AutoSize = true;
            this.CHK_customlabel.Location = new System.Drawing.Point(3, 6);
            this.CHK_customlabel.Name = "CHK_customlabel";
            this.CHK_customlabel.Size = new System.Drawing.Size(90, 17);
            this.CHK_customlabel.TabIndex = 15;
            this.CHK_customlabel.Text = "Custom Label";
            this.toolTip1.SetToolTip(this.CHK_customlabel, "Override the description at the top");
            this.CHK_customlabel.UseVisualStyleBackColor = true;
            this.CHK_customlabel.CheckedChanged += new System.EventHandler(this.CHK_customlabel_CheckedChanged);
            // 
            // TXT_customformat
            // 
            this.TXT_customformat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_customformat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TXT_customformat.Location = new System.Drawing.Point(105, 68);
            this.TXT_customformat.Name = "TXT_customformat";
            this.TXT_customformat.Size = new System.Drawing.Size(118, 13);
            this.TXT_customformat.TabIndex = 10;
            this.TXT_customformat.Text = "0.00";
            this.TXT_customformat.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.TXT_customformat, "Custom format specifier, e.g. \"0.0\" or \"m:ss\"");
            this.TXT_customformat.TextChanged += new System.EventHandler(this.TXT_customformat_TextChanged);
            // 
            // TXT_customlabel
            // 
            this.TXT_customlabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_customlabel.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TXT_customlabel.Location = new System.Drawing.Point(105, 8);
            this.TXT_customlabel.Name = "TXT_customlabel";
            this.TXT_customlabel.Size = new System.Drawing.Size(118, 13);
            this.TXT_customlabel.TabIndex = 16;
            this.TXT_customlabel.Text = "EFI Fuel Pressure (kPa)";
            this.TXT_customlabel.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.TXT_customlabel, "Override the description at the top");
            this.TXT_customlabel.TextChanged += new System.EventHandler(this.TXT_customlabel_TextChanged);
            // 
            // TXT_offset
            // 
            this.TXT_offset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_offset.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TXT_offset.Location = new System.Drawing.Point(105, 158);
            this.TXT_offset.Name = "TXT_offset";
            this.TXT_offset.Size = new System.Drawing.Size(118, 13);
            this.TXT_offset.TabIndex = 18;
            this.TXT_offset.Text = "0.0";
            this.TXT_offset.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.TXT_offset, "Apply scale and offset to convert units.");
            this.TXT_offset.TextChanged += new System.EventHandler(this.TXT_offset_TextChanged);
            this.TXT_offset.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TXT_scale_offset_KeyPress);
            // 
            // TXT_scale
            // 
            this.TXT_scale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TXT_scale.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TXT_scale.Location = new System.Drawing.Point(105, 128);
            this.TXT_scale.Name = "TXT_scale";
            this.TXT_scale.Size = new System.Drawing.Size(118, 13);
            this.TXT_scale.TabIndex = 17;
            this.TXT_scale.Text = "1.0";
            this.TXT_scale.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.toolTip1.SetToolTip(this.TXT_scale, "Apply scale and offset to convert units.");
            this.TXT_scale.TextChanged += new System.EventHandler(this.TXT_scale_TextChanged);
            this.TXT_scale.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TXT_scale_offset_KeyPress);
            // 
            // CHK_customformat
            // 
            this.CHK_customformat.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.CHK_customformat.AutoSize = true;
            this.CHK_customformat.Location = new System.Drawing.Point(3, 66);
            this.CHK_customformat.Name = "CHK_customformat";
            this.CHK_customformat.Size = new System.Drawing.Size(96, 17);
            this.CHK_customformat.TabIndex = 8;
            this.CHK_customformat.Text = "Custom Format";
            this.toolTip1.SetToolTip(this.CHK_customformat, "Custom format specifier, e.g. \"0.0\" or \"m:ss\"");
            this.CHK_customformat.UseVisualStyleBackColor = true;
            this.CHK_customformat.CheckedChanged += new System.EventHandler(this.CHK_customformat_CheckedChanged);
            // 
            // QuickViewOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(281, 251);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.LBL_precision);
            this.Controls.Add(this.NUM_precision);
            this.Controls.Add(this.CMB_Source);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(5000, 290);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(16, 290);
            this.Name = "QuickViewOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "QuickViewOptions";
            this.Shown += new System.EventHandler(this.QuickViewOptions_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.NUM_precision)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_customwidth)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_Source;
        private System.Windows.Forms.NumericUpDown NUM_precision;
        private System.Windows.Forms.Label LBL_precision;
        private System.Windows.Forms.CheckBox CHK_customcolor;
        private System.Windows.Forms.TextBox TXT_color;
        private System.Windows.Forms.Button BUT_colorpicker;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox CHK_customformat;
        private System.Windows.Forms.CheckBox CHK_customwidth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown NUM_customwidth;
        private System.Windows.Forms.CheckBox CHK_customlabel;
        private System.Windows.Forms.TextBox TXT_customformat;
        private System.Windows.Forms.TextBox TXT_customlabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox TXT_offset;
        private System.Windows.Forms.TextBox TXT_scale;
    }
}