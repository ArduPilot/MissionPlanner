
namespace MissionPlanner.Controls
{
    partial class SpectrogramUI
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
            this.but_loadlog = new MissionPlanner.Controls.MyButton();
            this.cmb_sensor = new System.Windows.Forms.ComboBox();
            this.zedGraphControl1 = new ZedGraph.ZedGraphControl();
            this.SuspendLayout();
            // 
            // but_loadlog
            // 
            this.but_loadlog.Location = new System.Drawing.Point(15, 6);
            this.but_loadlog.Name = "but_loadlog";
            this.but_loadlog.Size = new System.Drawing.Size(75, 23);
            this.but_loadlog.TabIndex = 5;
            this.but_loadlog.Text = "Load Log";
            this.but_loadlog.UseVisualStyleBackColor = true;
            this.but_loadlog.Click += new System.EventHandler(this.but_loadlog_Click);
            // 
            // cmb_sensor
            // 
            this.cmb_sensor.FormattingEnabled = true;
            this.cmb_sensor.Items.AddRange(new object[] {
            "ACC1",
            "ACC2",
            "ACC3",
            "GYR1",
            "GYR2",
            "GYR3"});
            this.cmb_sensor.Location = new System.Drawing.Point(96, 6);
            this.cmb_sensor.Name = "cmb_sensor";
            this.cmb_sensor.Size = new System.Drawing.Size(93, 21);
            this.cmb_sensor.TabIndex = 6;
            this.cmb_sensor.Text = "ACC1";
            this.cmb_sensor.SelectedIndexChanged += new System.EventHandler(this.cmb_sensor_SelectedIndexChanged);
            // 
            // zedGraphControl1
            // 
            this.zedGraphControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zedGraphControl1.Location = new System.Drawing.Point(12, 35);
            this.zedGraphControl1.Name = "zedGraphControl1";
            this.zedGraphControl1.ScrollGrace = 0D;
            this.zedGraphControl1.ScrollMaxX = 0D;
            this.zedGraphControl1.ScrollMaxY = 0D;
            this.zedGraphControl1.ScrollMaxY2 = 0D;
            this.zedGraphControl1.ScrollMinX = 0D;
            this.zedGraphControl1.ScrollMinY = 0D;
            this.zedGraphControl1.ScrollMinY2 = 0D;
            this.zedGraphControl1.Size = new System.Drawing.Size(776, 403);
            this.zedGraphControl1.TabIndex = 4;
            // 
            // SpectrogramUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.zedGraphControl1);
            this.Controls.Add(this.cmb_sensor);
            this.Controls.Add(this.but_loadlog);
            this.Name = "SpectrogramUI";
            this.Text = "SpectrogramUI";
            this.Resize += new System.EventHandler(this.SpectrogramUI_Resize);
            this.ResumeLayout(false);

        }

        #endregion
        private MyButton but_loadlog;
        private System.Windows.Forms.ComboBox cmb_sensor;
        private ZedGraph.ZedGraphControl zedGraphControl1;
    }
}