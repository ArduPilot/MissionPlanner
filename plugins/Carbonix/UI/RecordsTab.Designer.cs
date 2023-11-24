using System.Drawing;
namespace Carbonix
{
    partial class RecordsTab
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
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cmb_gso = new System.Windows.Forms.ComboBox();
            this.cmb_pic = new System.Windows.Forms.ComboBox();
            this.num_avbatid = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanelOuter = new System.Windows.Forms.TableLayoutPanel();
            this.lineSeparator2 = new MissionPlanner.Controls.LineSeparator();
            this.lineSeparator1 = new MissionPlanner.Controls.LineSeparator();
            this.label4 = new System.Windows.Forms.Label();
            this.num_vtolbatid = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.num_avbatid)).BeginInit();
            this.tableLayoutPanelOuter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_vtolbatid)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 40);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "GSO:";
            this.toolTip1.SetToolTip(this.label1, "Ground Station Operator");
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 8);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "PIC:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.label2, "Pilot in Command");
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 83);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(90, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Avionics Batt.:";
            this.toolTip1.SetToolTip(this.label3, "Battery Set ID Number");
            // 
            // cmb_gso
            // 
            this.cmb_gso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_gso.FormattingEnabled = true;
            this.cmb_gso.Location = new System.Drawing.Point(102, 36);
            this.cmb_gso.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmb_gso.Name = "cmb_gso";
            this.cmb_gso.Size = new System.Drawing.Size(294, 24);
            this.cmb_gso.TabIndex = 3;
            this.toolTip1.SetToolTip(this.cmb_gso, "Ground Station Operator");
            // 
            // cmb_pic
            // 
            this.cmb_pic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_pic.FormattingEnabled = true;
            this.cmb_pic.Location = new System.Drawing.Point(102, 4);
            this.cmb_pic.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cmb_pic.Name = "cmb_pic";
            this.cmb_pic.Size = new System.Drawing.Size(294, 24);
            this.cmb_pic.TabIndex = 4;
            this.toolTip1.SetToolTip(this.cmb_pic, "Pilot in Command");
            // 
            // num_avbatid
            // 
            this.num_avbatid.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_avbatid.Location = new System.Drawing.Point(102, 80);
            this.num_avbatid.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.num_avbatid.Name = "num_avbatid";
            this.num_avbatid.Size = new System.Drawing.Size(61, 22);
            this.num_avbatid.TabIndex = 5;
            this.toolTip1.SetToolTip(this.num_avbatid, "Avionics Battery ID Number");
            // 
            // tableLayoutPanelOuter
            // 
            this.tableLayoutPanelOuter.ColumnCount = 2;
            this.tableLayoutPanelOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelOuter.Controls.Add(this.lineSeparator2, 0, 5);
            this.tableLayoutPanelOuter.Controls.Add(this.label3, 0, 3);
            this.tableLayoutPanelOuter.Controls.Add(this.num_avbatid, 1, 3);
            this.tableLayoutPanelOuter.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanelOuter.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanelOuter.Controls.Add(this.cmb_pic, 1, 0);
            this.tableLayoutPanelOuter.Controls.Add(this.cmb_gso, 1, 1);
            this.tableLayoutPanelOuter.Controls.Add(this.lineSeparator1, 0, 2);
            this.tableLayoutPanelOuter.Controls.Add(this.label4, 0, 4);
            this.tableLayoutPanelOuter.Controls.Add(this.num_vtolbatid, 1, 4);
            this.tableLayoutPanelOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelOuter.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelOuter.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tableLayoutPanelOuter.Name = "tableLayoutPanelOuter";
            this.tableLayoutPanelOuter.RowCount = 7;
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 12F));
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelOuter.Size = new System.Drawing.Size(400, 452);
            this.tableLayoutPanelOuter.TabIndex = 79;
            // 
            // lineSeparator2
            // 
            this.lineSeparator2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelOuter.SetColumnSpan(this.lineSeparator2, 2);
            this.lineSeparator2.Location = new System.Drawing.Point(4, 140);
            this.lineSeparator2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lineSeparator2.MaximumSize = new System.Drawing.Size(2667, 2);
            this.lineSeparator2.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator2.Name = "lineSeparator2";
            this.lineSeparator2.Size = new System.Drawing.Size(392, 2);
            this.lineSeparator2.TabIndex = 7;
            // 
            // lineSeparator1
            // 
            this.lineSeparator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelOuter.SetColumnSpan(this.lineSeparator1, 2);
            this.lineSeparator1.Location = new System.Drawing.Point(4, 68);
            this.lineSeparator1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.lineSeparator1.MaximumSize = new System.Drawing.Size(2667, 2);
            this.lineSeparator1.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator1.Name = "lineSeparator1";
            this.lineSeparator1.Size = new System.Drawing.Size(392, 2);
            this.lineSeparator1.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(4, 113);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 16);
            this.label4.TabIndex = 8;
            this.label4.Text = "VTOL Batt.:";
            this.toolTip1.SetToolTip(this.label4, "Battery Set ID Number");
            // 
            // num_vtolbatid
            // 
            this.num_vtolbatid.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_vtolbatid.Location = new System.Drawing.Point(102, 110);
            this.num_vtolbatid.Margin = new System.Windows.Forms.Padding(4);
            this.num_vtolbatid.Name = "num_vtolbatid";
            this.num_vtolbatid.Size = new System.Drawing.Size(61, 22);
            this.num_vtolbatid.TabIndex = 9;
            this.toolTip1.SetToolTip(this.num_vtolbatid, "VTOL Battery Set ID Number");
            // 
            // RecordsTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelOuter);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "RecordsTab";
            this.Size = new System.Drawing.Size(400, 452);
            this.VisibleChanged += new System.EventHandler(this.RecordsTab_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.num_avbatid)).EndInit();
            this.tableLayoutPanelOuter.ResumeLayout(false);
            this.tableLayoutPanelOuter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.num_vtolbatid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelOuter;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox cmb_pic;
        public System.Windows.Forms.ComboBox cmb_gso;
        public System.Windows.Forms.NumericUpDown num_avbatid;
        public System.Windows.Forms.NumericUpDown num_vtolbatid;
        private MissionPlanner.Controls.LineSeparator lineSeparator1;
        private MissionPlanner.Controls.LineSeparator lineSeparator2;
        private System.Windows.Forms.Label label4;
    }
}
