namespace MissionPlanner.Controls
{
    partial class Coords
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
            this.CMB_coordsystem = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // CMB_coordsystem
            // 
            this.CMB_coordsystem.FormattingEnabled = true;
            this.CMB_coordsystem.Items.AddRange(new object[] {
            "GEO",
            "UTM",
            "MGRS"});
            this.CMB_coordsystem.Location = new System.Drawing.Point(0, 0);
            this.CMB_coordsystem.Name = "CMB_coordsystem";
            this.CMB_coordsystem.Size = new System.Drawing.Size(54, 21);
            this.CMB_coordsystem.TabIndex = 0;
            this.CMB_coordsystem.Text = "GEO";
            this.CMB_coordsystem.SelectedIndexChanged += new System.EventHandler(this.CMB_coordsystem_SelectedIndexChanged);
            // 
            // Coords
            // 
            this.Controls.Add(this.CMB_coordsystem);
            this.Name = "Coords";
            this.Size = new System.Drawing.Size(200, 21);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox CMB_coordsystem;
    }
}
