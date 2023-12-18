namespace RedundantLinkManager
{
    partial class LinkStatus
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
            this.tbl_links = new System.Windows.Forms.TableLayoutPanel();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.ledBulb1 = new Bulb.LedBulb();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tbl_outer = new System.Windows.Forms.TableLayoutPanel();
            this.tbl_links.SuspendLayout();
            this.tbl_outer.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbl_links
            // 
            this.tbl_links.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tbl_links.AutoSize = true;
            this.tbl_links.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tbl_links.ColumnCount = 9;
            this.tbl_links.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbl_links.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbl_links.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbl_links.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbl_links.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbl_links.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbl_links.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbl_links.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbl_links.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbl_links.Controls.Add(this.radioButton1, 0, 0);
            this.tbl_links.Controls.Add(this.ledBulb1, 1, 0);
            this.tbl_links.Controls.Add(this.label1, 2, 0);
            this.tbl_links.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.AddColumns;
            this.tbl_links.Location = new System.Drawing.Point(1, 1);
            this.tbl_links.Margin = new System.Windows.Forms.Padding(0);
            this.tbl_links.Name = "tbl_links";
            this.tbl_links.RowCount = 3;
            this.tbl_links.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tbl_links.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tbl_links.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tbl_links.Size = new System.Drawing.Size(96, 45);
            this.tbl_links.TabIndex = 0;
            // 
            // radioButton1
            // 
            this.radioButton1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.radioButton1.Location = new System.Drawing.Point(0, 0);
            this.radioButton1.Margin = new System.Windows.Forms.Padding(0);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(15, 15);
            this.radioButton1.TabIndex = 3;
            this.radioButton1.TabStop = true;
            this.radioButton1.UseVisualStyleBackColor = false;
            // 
            // ledBulb1
            // 
            this.ledBulb1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.ledBulb1.Location = new System.Drawing.Point(15, 0);
            this.ledBulb1.Margin = new System.Windows.Forms.Padding(0);
            this.ledBulb1.Name = "ledBulb1";
            this.ledBulb1.On = true;
            this.ledBulb1.Size = new System.Drawing.Size(15, 15);
            this.ledBulb1.TabIndex = 4;
            this.ledBulb1.Text = "ledBulb1";
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoEllipsis = true;
            this.label1.Location = new System.Drawing.Point(33, 1);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "label1";
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(101, 8);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(58, 30);
            this.checkBox1.TabIndex = 1;
            this.checkBox1.Text = "Auto\r\nSwitch";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // tbl_outer
            // 
            this.tbl_outer.AutoSize = true;
            this.tbl_outer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tbl_outer.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tbl_outer.ColumnCount = 2;
            this.tbl_outer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbl_outer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tbl_outer.Controls.Add(this.tbl_links, 0, 0);
            this.tbl_outer.Controls.Add(this.checkBox1, 1, 0);
            this.tbl_outer.Location = new System.Drawing.Point(0, 0);
            this.tbl_outer.Margin = new System.Windows.Forms.Padding(0);
            this.tbl_outer.Name = "tbl_outer";
            this.tbl_outer.RowCount = 1;
            this.tbl_outer.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tbl_outer.Size = new System.Drawing.Size(163, 47);
            this.tbl_outer.TabIndex = 2;
            // 
            // LinkStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.tbl_outer);
            this.MinimumSize = new System.Drawing.Size(45, 45);
            this.Name = "LinkStatus";
            this.Size = new System.Drawing.Size(163, 47);
            this.tbl_links.ResumeLayout(false);
            this.tbl_outer.ResumeLayout(false);
            this.tbl_outer.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tbl_links;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.RadioButton radioButton1;
        private Bulb.LedBulb ledBulb1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tbl_outer;
    }
}
