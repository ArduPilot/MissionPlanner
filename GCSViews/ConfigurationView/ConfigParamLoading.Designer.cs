namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigParamLoading
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
            this.label1 = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.but_forceparams = new MissionPlanner.Controls.MyButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(140, 131);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(218, 60);
            this.label1.TabIndex = 1;
            this.label1.Text = "Paramaters are still loading. Many screens will not work untill all Parameters ar" +
    "e loaded. ";
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // but_forceparams
            // 
            this.but_forceparams.Location = new System.Drawing.Point(209, 194);
            this.but_forceparams.Name = "but_forceparams";
            this.but_forceparams.Size = new System.Drawing.Size(75, 23);
            this.but_forceparams.TabIndex = 2;
            this.but_forceparams.Text = "Retry Now";
            this.but_forceparams.UseVisualStyleBackColor = true;
            this.but_forceparams.Click += new System.EventHandler(this.but_forceparams_Click);
            // 
            // ConfigParamLoading
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.but_forceparams);
            this.Controls.Add(this.label1);
            this.Name = "ConfigParamLoading";
            this.Size = new System.Drawing.Size(495, 323);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer timer1;
        private Controls.MyButton but_forceparams;
    }
}
