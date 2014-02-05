namespace MissionPlanner.Controls
{
    partial class MyProgressBar
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
            this.marquee = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // marquee
            // 
            this.marquee.Interval = 50;
            this.marquee.Tick += new System.EventHandler(this.marquee_Tick);
            // 
            // MyProgressBar
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Name = "MyProgressBar";
            this.Size = new System.Drawing.Size(208, 29);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer marquee;
    }
}
