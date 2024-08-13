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
            this.label4 = new System.Windows.Forms.Label();
            this.num_vtolbatid = new System.Windows.Forms.NumericUpDown();
            this.txt_payload_serial = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.cmb_payload = new System.Windows.Forms.ComboBox();
            this.txt_metar = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.cmb_location = new System.Windows.Forms.ComboBox();
            this.cmb_operation = new System.Windows.Forms.ComboBox();
            this.chk_auto_metar = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tableLayoutPanelOuter = new System.Windows.Forms.TableLayoutPanel();
            this.lineSeparator1 = new MissionPlanner.Controls.LineSeparator();
            this.lineSeparator2 = new MissionPlanner.Controls.LineSeparator();
            this.lineSeparator3 = new MissionPlanner.Controls.LineSeparator();
            this.cmb_vlos = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.num_avbatid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_vtolbatid)).BeginInit();
            this.tableLayoutPanelOuter.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(33, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "GSO:";
            this.toolTip1.SetToolTip(this.label1, "Ground Station Operator");
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(27, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "PIC:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.toolTip1.SetToolTip(this.label2, "Pilot in Command");
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 124);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Avionics Batt.:";
            this.toolTip1.SetToolTip(this.label3, "Avionics Battery ID Number");
            // 
            // cmb_gso
            // 
            this.cmb_gso.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelOuter.SetColumnSpan(this.cmb_gso, 2);
            this.cmb_gso.FormattingEnabled = true;
            this.cmb_gso.Location = new System.Drawing.Point(84, 30);
            this.cmb_gso.Name = "cmb_gso";
            this.cmb_gso.Size = new System.Drawing.Size(193, 21);
            this.cmb_gso.TabIndex = 3;
            this.toolTip1.SetToolTip(this.cmb_gso, "Ground Station Operator");
            // 
            // cmb_pic
            // 
            this.cmb_pic.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelOuter.SetColumnSpan(this.cmb_pic, 2);
            this.cmb_pic.FormattingEnabled = true;
            this.cmb_pic.Location = new System.Drawing.Point(84, 3);
            this.cmb_pic.Name = "cmb_pic";
            this.cmb_pic.Size = new System.Drawing.Size(193, 21);
            this.cmb_pic.TabIndex = 4;
            this.toolTip1.SetToolTip(this.cmb_pic, "Pilot in Command");
            // 
            // num_avbatid
            // 
            this.num_avbatid.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_avbatid.Location = new System.Drawing.Point(84, 121);
            this.num_avbatid.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.num_avbatid.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.num_avbatid.Name = "num_avbatid";
            this.num_avbatid.Size = new System.Drawing.Size(60, 20);
            this.num_avbatid.TabIndex = 5;
            this.toolTip1.SetToolTip(this.num_avbatid, "Avionics Battery ID Number");
            this.num_avbatid.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 150);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(63, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "VTOL Batt.:";
            this.toolTip1.SetToolTip(this.label4, "VTOL Battery Set ID Number");
            // 
            // num_vtolbatid
            // 
            this.num_vtolbatid.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.num_vtolbatid.Location = new System.Drawing.Point(84, 147);
            this.num_vtolbatid.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.num_vtolbatid.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            this.num_vtolbatid.Name = "num_vtolbatid";
            this.num_vtolbatid.Size = new System.Drawing.Size(60, 20);
            this.num_vtolbatid.TabIndex = 9;
            this.toolTip1.SetToolTip(this.num_vtolbatid, "VTOL Battery Set ID Number");
            this.num_vtolbatid.Value = new decimal(new int[] {
            1,
            0,
            0,
            -2147483648});
            // 
            // txt_payload_serial
            // 
            this.txt_payload_serial.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txt_payload_serial.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.txt_payload_serial.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList;
            this.txt_payload_serial.Location = new System.Drawing.Point(84, 200);
            this.txt_payload_serial.Name = "txt_payload_serial";
            this.txt_payload_serial.Size = new System.Drawing.Size(128, 20);
            this.txt_payload_serial.TabIndex = 13;
            this.toolTip1.SetToolTip(this.txt_payload_serial, "Payload Serial Number");
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 177);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Payload:";
            this.toolTip1.SetToolTip(this.label5, "Installed Payload");
            // 
            // cmb_payload
            // 
            this.cmb_payload.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_payload.FormattingEnabled = true;
            this.cmb_payload.Location = new System.Drawing.Point(84, 173);
            this.cmb_payload.Name = "cmb_payload";
            this.cmb_payload.Size = new System.Drawing.Size(128, 21);
            this.cmb_payload.TabIndex = 12;
            this.cmb_payload.Text = "None";
            this.toolTip1.SetToolTip(this.cmb_payload, "Installed Payload");
            // 
            // txt_metar
            // 
            this.txt_metar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelOuter.SetColumnSpan(this.txt_metar, 2);
            this.txt_metar.Location = new System.Drawing.Point(84, 230);
            this.txt_metar.Name = "txt_metar";
            this.txt_metar.ReadOnly = true;
            this.txt_metar.Size = new System.Drawing.Size(193, 20);
            this.txt_metar.TabIndex = 15;
            this.toolTip1.SetToolTip(this.txt_metar, "Current weather conditions in METAR format");
            // 
            // label6
            // 
            this.label6.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 233);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "METAR:";
            this.toolTip1.SetToolTip(this.label6, "Current weather conditions in METAR format");
            // 
            // label7
            // 
            this.label7.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(3, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(71, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "PIC Location:";
            this.toolTip1.SetToolTip(this.label7, "Where the PIC is located");
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(3, 88);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(54, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Op. Type:";
            this.toolTip1.SetToolTip(this.label8, "The purpose of this flight");
            // 
            // cmb_location
            // 
            this.cmb_location.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelOuter.SetColumnSpan(this.cmb_location, 2);
            this.cmb_location.FormattingEnabled = true;
            this.cmb_location.Location = new System.Drawing.Point(84, 57);
            this.cmb_location.Name = "cmb_location";
            this.cmb_location.Size = new System.Drawing.Size(193, 21);
            this.cmb_location.TabIndex = 20;
            this.toolTip1.SetToolTip(this.cmb_location, "Where the PIC is located");
            // 
            // cmb_operation
            // 
            this.cmb_operation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_operation.FormattingEnabled = true;
            this.cmb_operation.Items.AddRange(new object[] {
            "Operation",
            "Training",
            "Software Test",
            "Hardware Test",
            "Flight Cycles"});
            this.cmb_operation.Location = new System.Drawing.Point(84, 84);
            this.cmb_operation.Name = "cmb_operation";
            this.cmb_operation.Size = new System.Drawing.Size(128, 21);
            this.cmb_operation.TabIndex = 21;
            this.toolTip1.SetToolTip(this.cmb_operation, "The purpose of this flight");
            // 
            // chk_auto_metar
            // 
            this.chk_auto_metar.AutoSize = true;
            this.chk_auto_metar.Checked = true;
            this.chk_auto_metar.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chk_auto_metar.Location = new System.Drawing.Point(84, 256);
            this.chk_auto_metar.Name = "chk_auto_metar";
            this.chk_auto_metar.Size = new System.Drawing.Size(48, 17);
            this.chk_auto_metar.TabIndex = 17;
            this.chk_auto_metar.Text = "Auto";
            this.toolTip1.SetToolTip(this.chk_auto_metar, "Automatically fetch METAR from nearest airfield");
            this.chk_auto_metar.UseVisualStyleBackColor = true;
            this.chk_auto_metar.CheckedChanged += new System.EventHandler(this.chk_auto_metar_CheckedChanged);
            // 
            // label9
            // 
            this.label9.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(3, 200);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(71, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "Payload S/N:";
            this.toolTip1.SetToolTip(this.label9, "Installed Payload");
            // 
            // tableLayoutPanelOuter
            // 
            this.tableLayoutPanelOuter.AutoSize = true;
            this.tableLayoutPanelOuter.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanelOuter.ColumnCount = 3;
            this.tableLayoutPanelOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanelOuter.Controls.Add(this.cmb_pic, 1, 0);
            this.tableLayoutPanelOuter.Controls.Add(this.cmb_gso, 1, 1);
            this.tableLayoutPanelOuter.Controls.Add(this.cmb_location, 1, 2);
            this.tableLayoutPanelOuter.Controls.Add(this.cmb_operation, 1, 3);
            this.tableLayoutPanelOuter.Controls.Add(this.cmb_vlos, 2, 3);
            this.tableLayoutPanelOuter.Controls.Add(this.lineSeparator1, 0, 4);
            this.tableLayoutPanelOuter.Controls.Add(this.num_avbatid, 1, 5);
            this.tableLayoutPanelOuter.Controls.Add(this.num_vtolbatid, 1, 6);
            this.tableLayoutPanelOuter.Controls.Add(this.cmb_payload, 1, 7);
            this.tableLayoutPanelOuter.Controls.Add(this.lineSeparator2, 0, 9);
            this.tableLayoutPanelOuter.Controls.Add(this.txt_metar, 1, 10);
            this.tableLayoutPanelOuter.Controls.Add(this.chk_auto_metar, 1, 11);
            this.tableLayoutPanelOuter.Controls.Add(this.lineSeparator3, 0, 12);
            this.tableLayoutPanelOuter.Controls.Add(this.label4, 0, 6);
            this.tableLayoutPanelOuter.Controls.Add(this.label5, 0, 7);
            this.tableLayoutPanelOuter.Controls.Add(this.label3, 0, 5);
            this.tableLayoutPanelOuter.Controls.Add(this.label6, 0, 10);
            this.tableLayoutPanelOuter.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanelOuter.Controls.Add(this.label2, 0, 0);
            this.tableLayoutPanelOuter.Controls.Add(this.label7, 0, 2);
            this.tableLayoutPanelOuter.Controls.Add(this.label8, 0, 3);
            this.tableLayoutPanelOuter.Controls.Add(this.txt_payload_serial, 1, 8);
            this.tableLayoutPanelOuter.Controls.Add(this.label9, 0, 8);
            this.tableLayoutPanelOuter.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanelOuter.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelOuter.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanelOuter.MinimumSize = new System.Drawing.Size(250, 0);
            this.tableLayoutPanelOuter.Name = "tableLayoutPanelOuter";
            this.tableLayoutPanelOuter.RowCount = 13;
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelOuter.Size = new System.Drawing.Size(280, 284);
            this.tableLayoutPanelOuter.TabIndex = 79;
            // 
            // lineSeparator1
            // 
            this.lineSeparator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelOuter.SetColumnSpan(this.lineSeparator1, 3);
            this.lineSeparator1.Location = new System.Drawing.Point(3, 111);
            this.lineSeparator1.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeparator1.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator1.Name = "lineSeparator1";
            this.lineSeparator1.Size = new System.Drawing.Size(274, 2);
            this.lineSeparator1.TabIndex = 6;
            // 
            // lineSeparator2
            // 
            this.lineSeparator2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelOuter.SetColumnSpan(this.lineSeparator2, 3);
            this.lineSeparator2.Location = new System.Drawing.Point(3, 220);
            this.lineSeparator2.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeparator2.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator2.Name = "lineSeparator2";
            this.lineSeparator2.Size = new System.Drawing.Size(274, 2);
            this.lineSeparator2.TabIndex = 7;
            // 
            // lineSeparator3
            // 
            this.lineSeparator3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelOuter.SetColumnSpan(this.lineSeparator3, 3);
            this.lineSeparator3.Location = new System.Drawing.Point(3, 279);
            this.lineSeparator3.MaximumSize = new System.Drawing.Size(2000, 2);
            this.lineSeparator3.MinimumSize = new System.Drawing.Size(0, 2);
            this.lineSeparator3.Name = "lineSeparator3";
            this.lineSeparator3.Size = new System.Drawing.Size(274, 2);
            this.lineSeparator3.TabIndex = 16;
            // 
            // cmb_vlos
            // 
            this.cmb_vlos.FormattingEnabled = true;
            this.cmb_vlos.Items.AddRange(new object[] {
            "VLOS",
            "EVLOS",
            "BVLOS"});
            this.cmb_vlos.Location = new System.Drawing.Point(218, 84);
            this.cmb_vlos.Name = "cmb_vlos";
            this.cmb_vlos.Size = new System.Drawing.Size(59, 21);
            this.cmb_vlos.TabIndex = 24;
            this.cmb_vlos.Text = "VLOS";
            this.toolTip1.SetToolTip(this.cmb_vlos, "Visual line of sight");
            // 
            // RecordsTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(0, 260);
            this.Controls.Add(this.tableLayoutPanelOuter);
            this.Name = "RecordsTab";
            this.Size = new System.Drawing.Size(280, 340);
            this.VisibleChanged += new System.EventHandler(this.RecordsTab_VisibleChanged);
            ((System.ComponentModel.ISupportInitialize)(this.num_avbatid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.num_vtolbatid)).EndInit();
            this.tableLayoutPanelOuter.ResumeLayout(false);
            this.tableLayoutPanelOuter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        public System.Windows.Forms.ComboBox cmb_payload;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_payload_serial;
        private System.Windows.Forms.TextBox txt_metar;
        private MissionPlanner.Controls.LineSeparator lineSeparator3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chk_auto_metar;
        public System.Windows.Forms.ComboBox cmb_operation;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        public System.Windows.Forms.ComboBox cmb_location;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ComboBox cmb_vlos;
    }
}
