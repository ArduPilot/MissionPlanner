
namespace MissionPlanner.Controls
{
    partial class QuickView
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
          
            this.SuspendLayout();

     
            this.toolTip1.SetToolTip(this, "Double click to change");
            // 
            // QuickView
            // 
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "QuickView";
            this.Size = new System.Drawing.Size(122, 54);

            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
    }
}
