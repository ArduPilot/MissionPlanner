namespace Carbonix
{
    partial class TakeoffTab
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
            this.but_arm = new MissionPlanner.Controls.MyButton();
            this.but_manual = new MissionPlanner.Controls.MyButton();
            this.but_calibrate = new MissionPlanner.Controls.MyButton();
            this.but_landfinal = new MissionPlanner.Controls.MyButton();
            this.tableLayoutPanelOuter = new System.Windows.Forms.TableLayoutPanel();
            this.txt_messagebox = new System.Windows.Forms.TextBox();
            this.tableLayoutActions = new System.Windows.Forms.TableLayoutPanel();
            this.table_numberViews = new System.Windows.Forms.TableLayoutPanel();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.but_safety = new MissionPlanner.Controls.MyButton();
            this.numberView4 = new Carbonix.NumberView();
            this.numberView3 = new Carbonix.NumberView();
            this.numberView2 = new Carbonix.NumberView();
            this.numberView1 = new Carbonix.NumberView();
            this.tableLayoutPanelOuter.SuspendLayout();
            this.tableLayoutActions.SuspendLayout();
            this.table_numberViews.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // but_arm
            // 
            this.but_arm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.but_arm.ColorMouseDown = System.Drawing.Color.Empty;
            this.but_arm.ColorMouseOver = System.Drawing.Color.Empty;
            this.but_arm.ColorNotEnabled = System.Drawing.Color.Empty;
            this.but_arm.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.but_arm.Location = new System.Drawing.Point(86, 62);
            this.but_arm.Margin = new System.Windows.Forms.Padding(2);
            this.but_arm.Name = "but_arm";
            this.but_arm.Size = new System.Drawing.Size(80, 26);
            this.but_arm.TabIndex = 77;
            this.but_arm.Text = "Arm / Disarm";
            this.but_arm.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.but_arm, "Arm or Disarm the aircraft");
            this.but_arm.UseVisualStyleBackColor = true;
            this.but_arm.Click += new System.EventHandler(this.but_arm_Click);
            // 
            // but_manual
            // 
            this.but_manual.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.but_manual.ColorMouseDown = System.Drawing.Color.Empty;
            this.but_manual.ColorMouseOver = System.Drawing.Color.Empty;
            this.but_manual.ColorNotEnabled = System.Drawing.Color.Empty;
            this.but_manual.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.but_manual.Location = new System.Drawing.Point(2, 32);
            this.but_manual.Margin = new System.Windows.Forms.Padding(2);
            this.but_manual.Name = "but_manual";
            this.but_manual.Size = new System.Drawing.Size(80, 26);
            this.but_manual.TabIndex = 78;
            this.but_manual.Text = "Manual";
            this.but_manual.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.but_manual, "Change mode to manual");
            this.but_manual.UseVisualStyleBackColor = true;
            this.but_manual.Click += new System.EventHandler(this.but_manual_Click);
            // 
            // but_calibrate
            // 
            this.but_calibrate.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.but_calibrate.ColorMouseDown = System.Drawing.Color.Empty;
            this.but_calibrate.ColorMouseOver = System.Drawing.Color.Empty;
            this.but_calibrate.ColorNotEnabled = System.Drawing.Color.Empty;
            this.but_calibrate.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.but_calibrate.Location = new System.Drawing.Point(2, 2);
            this.but_calibrate.Margin = new System.Windows.Forms.Padding(2);
            this.but_calibrate.Name = "but_calibrate";
            this.but_calibrate.Size = new System.Drawing.Size(80, 26);
            this.but_calibrate.TabIndex = 80;
            this.but_calibrate.Text = "Calibrate Plane";
            this.but_calibrate.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.but_calibrate, "Perform preflight calibration");
            this.but_calibrate.UseVisualStyleBackColor = true;
            this.but_calibrate.Click += new System.EventHandler(this.but_calibrate_Click);
            // 
            // but_landfinal
            // 
            this.but_landfinal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.but_landfinal.ColorMouseDown = System.Drawing.Color.Empty;
            this.but_landfinal.ColorMouseOver = System.Drawing.Color.Empty;
            this.but_landfinal.ColorNotEnabled = System.Drawing.Color.Empty;
            this.but_landfinal.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.but_landfinal.Location = new System.Drawing.Point(86, 2);
            this.but_landfinal.Margin = new System.Windows.Forms.Padding(2);
            this.but_landfinal.Name = "but_landfinal";
            this.but_landfinal.Size = new System.Drawing.Size(80, 26);
            this.but_landfinal.TabIndex = 94;
            this.but_landfinal.Text = "Cleared to Land";
            this.but_landfinal.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.but_landfinal, "Initiate Final Landing");
            this.but_landfinal.UseVisualStyleBackColor = true;
            this.but_landfinal.Click += new System.EventHandler(this.but_landfinal_Click);
            // 
            // tableLayoutPanelOuter
            // 
            this.tableLayoutPanelOuter.ColumnCount = 1;
            this.tableLayoutPanelOuter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelOuter.Controls.Add(this.txt_messagebox, 0, 2);
            this.tableLayoutPanelOuter.Controls.Add(this.tableLayoutActions, 0, 0);
            this.tableLayoutPanelOuter.Controls.Add(this.table_numberViews, 0, 1);
            this.tableLayoutPanelOuter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelOuter.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelOuter.Margin = new System.Windows.Forms.Padding(2);
            this.tableLayoutPanelOuter.Name = "tableLayoutPanelOuter";
            this.tableLayoutPanelOuter.RowCount = 3;
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 92F));
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelOuter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableLayoutPanelOuter.Size = new System.Drawing.Size(300, 367);
            this.tableLayoutPanelOuter.TabIndex = 79;
            // 
            // txt_messagebox
            // 
            this.txt_messagebox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txt_messagebox.Location = new System.Drawing.Point(2, 274);
            this.txt_messagebox.Margin = new System.Windows.Forms.Padding(2);
            this.txt_messagebox.Multiline = true;
            this.txt_messagebox.Name = "txt_messagebox";
            this.txt_messagebox.ReadOnly = true;
            this.txt_messagebox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txt_messagebox.Size = new System.Drawing.Size(296, 91);
            this.txt_messagebox.TabIndex = 2;
            this.txt_messagebox.Text = "Transition complete\r\nTransition airspeed wait\r\nAirspeed calibration complete\r\nAir" +
    "speed sensor calibration started";
            // 
            // tableLayoutActions
            // 
            this.tableLayoutActions.ColumnCount = 5;
            this.tableLayoutActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 49.99999F));
            this.tableLayoutActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.00001F));
            this.tableLayoutActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 19F));
            this.tableLayoutActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutActions.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 52F));
            this.tableLayoutActions.Controls.Add(this.but_calibrate, 0, 0);
            this.tableLayoutActions.Controls.Add(this.but_landfinal, 1, 0);
            this.tableLayoutActions.Controls.Add(this.but_arm, 1, 2);
            this.tableLayoutActions.Controls.Add(this.but_safety, 0, 2);
            this.tableLayoutActions.Controls.Add(this.but_manual, 0, 1);
            this.tableLayoutActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutActions.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutActions.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutActions.Name = "tableLayoutActions";
            this.tableLayoutActions.RowCount = 4;
            this.tableLayoutActions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutActions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutActions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutActions.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.tableLayoutActions.Size = new System.Drawing.Size(300, 92);
            this.tableLayoutActions.TabIndex = 0;
            // 
            // table_numberViews
            // 
            this.table_numberViews.ColumnCount = 4;
            this.table_numberViews.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.table_numberViews.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.table_numberViews.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.table_numberViews.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.table_numberViews.Controls.Add(this.numberView4, 3, 0);
            this.table_numberViews.Controls.Add(this.numberView3, 2, 0);
            this.table_numberViews.Controls.Add(this.numberView2, 1, 0);
            this.table_numberViews.Controls.Add(this.numberView1, 0, 0);
            this.table_numberViews.Dock = System.Windows.Forms.DockStyle.Fill;
            this.table_numberViews.Location = new System.Drawing.Point(3, 95);
            this.table_numberViews.Name = "table_numberViews";
            this.table_numberViews.RowCount = 2;
            this.table_numberViews.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table_numberViews.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.table_numberViews.Size = new System.Drawing.Size(294, 174);
            this.table_numberViews.TabIndex = 3;
            this.table_numberViews.Resize += new System.EventHandler(this.table_numberViews_Resize);
            // 
            // bindingSource1
            // 
            this.bindingSource1.DataSource = typeof(MissionPlanner.CurrentState);
            // 
            // but_safety
            // 
            this.but_safety.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.but_safety.ColorMouseDown = System.Drawing.Color.Empty;
            this.but_safety.ColorMouseOver = System.Drawing.Color.Empty;
            this.but_safety.ColorNotEnabled = System.Drawing.Color.Empty;
            this.but_safety.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.but_safety.Location = new System.Drawing.Point(2, 62);
            this.but_safety.Margin = new System.Windows.Forms.Padding(2);
            this.but_safety.Name = "but_safety";
            this.but_safety.Size = new System.Drawing.Size(80, 26);
            this.but_safety.TabIndex = 95;
            this.but_safety.Text = "Engage Safety";
            this.but_safety.TextColorNotEnabled = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(87)))), ((int)(((byte)(4)))));
            this.toolTip1.SetToolTip(this.but_safety, "Change mode to manual");
            this.but_safety.UseVisualStyleBackColor = true;
            this.but_safety.Click += new System.EventHandler(this.but_safety_Click);
            // 
            // numberView4
            // 
            this.numberView4.desc = "";
            this.numberView4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numberView4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numberView4.Location = new System.Drawing.Point(222, 3);
            this.numberView4.Name = "numberView4";
            this.numberView4.number = -9999D;
            this.numberView4.numberColor = System.Drawing.Color.Empty;
            this.numberView4.numberColorBackup = System.Drawing.Color.Empty;
            this.numberView4.numberformat = "0.00";
            this.numberView4.Size = new System.Drawing.Size(69, 81);
            this.numberView4.TabIndex = 3;
            this.numberView4.Text = "numberView4";
            // 
            // numberView3
            // 
            this.numberView3.desc = "";
            this.numberView3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numberView3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numberView3.Location = new System.Drawing.Point(149, 3);
            this.numberView3.Name = "numberView3";
            this.numberView3.number = -9999D;
            this.numberView3.numberColor = System.Drawing.Color.Empty;
            this.numberView3.numberColorBackup = System.Drawing.Color.Empty;
            this.numberView3.numberformat = "0.00";
            this.numberView3.Size = new System.Drawing.Size(67, 81);
            this.numberView3.TabIndex = 2;
            this.numberView3.Text = "numberView3";
            // 
            // numberView2
            // 
            this.numberView2.desc = "";
            this.numberView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numberView2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numberView2.Location = new System.Drawing.Point(76, 3);
            this.numberView2.Name = "numberView2";
            this.numberView2.number = -9999D;
            this.numberView2.numberColor = System.Drawing.Color.Empty;
            this.numberView2.numberColorBackup = System.Drawing.Color.Empty;
            this.numberView2.numberformat = "0.00";
            this.numberView2.Size = new System.Drawing.Size(67, 81);
            this.numberView2.TabIndex = 1;
            this.numberView2.Text = "numberView2";
            // 
            // numberView1
            // 
            this.numberView1.desc = "";
            this.numberView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numberView1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numberView1.Location = new System.Drawing.Point(3, 3);
            this.numberView1.Name = "numberView1";
            this.numberView1.number = -9999D;
            this.numberView1.numberColor = System.Drawing.Color.Empty;
            this.numberView1.numberColorBackup = System.Drawing.Color.Empty;
            this.numberView1.numberformat = "0.00";
            this.numberView1.Size = new System.Drawing.Size(67, 81);
            this.numberView1.TabIndex = 0;
            this.numberView1.Text = "numberView1";
            // 
            // TakeoffTab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelOuter);
            this.Name = "TakeoffTab";
            this.Size = new System.Drawing.Size(300, 367);
            this.VisibleChanged += new System.EventHandler(this.TakeoffTab_VisibleChanged);
            this.tableLayoutPanelOuter.ResumeLayout(false);
            this.tableLayoutPanelOuter.PerformLayout();
            this.tableLayoutActions.ResumeLayout(false);
            this.table_numberViews.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelOuter;
        private System.Windows.Forms.TableLayoutPanel tableLayoutActions;
        private MissionPlanner.Controls.MyButton but_arm;
        private MissionPlanner.Controls.MyButton but_manual;
        private MissionPlanner.Controls.MyButton but_calibrate;
        private System.Windows.Forms.TextBox txt_messagebox;
        private System.Windows.Forms.TableLayoutPanel table_numberViews;
        private NumberView numberView4;
        private NumberView numberView3;
        private NumberView numberView2;
        private MissionPlanner.Controls.MyButton but_landfinal;
        private System.Windows.Forms.BindingSource bindingSource1;
        private NumberView numberView1;
        private MissionPlanner.Controls.MyButton but_safety;
    }
}
