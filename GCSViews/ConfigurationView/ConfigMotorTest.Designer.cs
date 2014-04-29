namespace MissionPlanner.GCSViews.ConfigurationView
{
    partial class ConfigMotorTest
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
            this.NUM_thr_percent = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.NUM_thr_percent)).BeginInit();
            this.SuspendLayout();
            // 
            // NUM_thr_percent
            // 
            this.NUM_thr_percent.Location = new System.Drawing.Point(111, 3);
            this.NUM_thr_percent.Name = "NUM_thr_percent";
            this.NUM_thr_percent.Size = new System.Drawing.Size(54, 20);
            this.NUM_thr_percent.TabIndex = 0;
            this.NUM_thr_percent.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            // 
            // ConfigMotorTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.NUM_thr_percent);
            this.Name = "ConfigMotorTest";
            this.Size = new System.Drawing.Size(563, 264);
            ((System.ComponentModel.ISupportInitialize)(this.NUM_thr_percent)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown NUM_thr_percent;
    }
}
