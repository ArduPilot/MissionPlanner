namespace GMap.NET
{
    partial class TilePrefetcherMenu
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.trackBarMaxZoom = new System.Windows.Forms.TrackBar();
            this.trackBarMinZoom = new System.Windows.Forms.TrackBar();
            this.labelMaxZoom = new System.Windows.Forms.Label();
            this.labelMinZoom = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.textBoxTile = new System.Windows.Forms.TextBox();
            this.labelTotal = new System.Windows.Forms.Label();
            this.numericUpDownMinZoom = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownMaxZoom = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxZoom)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(174, 360);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // trackBarMaxZoom
            // 
            this.trackBarMaxZoom.Location = new System.Drawing.Point(74, 46);
            this.trackBarMaxZoom.Maximum = 20;
            this.trackBarMaxZoom.Minimum = 1;
            this.trackBarMaxZoom.Name = "trackBarMaxZoom";
            this.trackBarMaxZoom.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarMaxZoom.Size = new System.Drawing.Size(45, 295);
            this.trackBarMaxZoom.TabIndex = 1;
            this.trackBarMaxZoom.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            this.trackBarMaxZoom.Value = 20;
            this.trackBarMaxZoom.ValueChanged += new System.EventHandler(this.trackBarMaxZoom_ValueChanged);
            // 
            // trackBarMinZoom
            // 
            this.trackBarMinZoom.Location = new System.Drawing.Point(7, 46);
            this.trackBarMinZoom.Maximum = 20;
            this.trackBarMinZoom.Minimum = 1;
            this.trackBarMinZoom.Name = "trackBarMinZoom";
            this.trackBarMinZoom.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarMinZoom.Size = new System.Drawing.Size(45, 295);
            this.trackBarMinZoom.TabIndex = 1;
            this.trackBarMinZoom.Value = 1;
            this.trackBarMinZoom.ValueChanged += new System.EventHandler(this.trackBarMinZoom_ValueChanged);
            // 
            // labelMaxZoom
            // 
            this.labelMaxZoom.AutoSize = true;
            this.labelMaxZoom.Location = new System.Drawing.Point(74, 4);
            this.labelMaxZoom.Name = "labelMaxZoom";
            this.labelMaxZoom.Size = new System.Drawing.Size(64, 13);
            this.labelMaxZoom.TabIndex = 2;
            this.labelMaxZoom.Text = "Max zoom : ";
            // 
            // labelMinZoom
            // 
            this.labelMinZoom.AutoSize = true;
            this.labelMinZoom.Location = new System.Drawing.Point(7, 4);
            this.labelMinZoom.Name = "labelMinZoom";
            this.labelMinZoom.Size = new System.Drawing.Size(61, 13);
            this.labelMinZoom.TabIndex = 2;
            this.labelMinZoom.Text = "Min zoom : ";
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(7, 360);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // textBoxTile
            // 
            this.textBoxTile.Location = new System.Drawing.Point(144, 7);
            this.textBoxTile.Multiline = true;
            this.textBoxTile.Name = "textBoxTile";
            this.textBoxTile.ReadOnly = true;
            this.textBoxTile.Size = new System.Drawing.Size(105, 334);
            this.textBoxTile.TabIndex = 3;
            // 
            // labelTotal
            // 
            this.labelTotal.AutoSize = true;
            this.labelTotal.Location = new System.Drawing.Point(7, 344);
            this.labelTotal.Name = "labelTotal";
            this.labelTotal.Size = new System.Drawing.Size(132, 13);
            this.labelTotal.TabIndex = 4;
            this.labelTotal.Text = "Estimated : 0 tiles for 0 MB";
            // 
            // numericUpDownMinZoom
            // 
            this.numericUpDownMinZoom.Location = new System.Drawing.Point(7, 20);
            this.numericUpDownMinZoom.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownMinZoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMinZoom.Name = "numericUpDownMinZoom";
            this.numericUpDownMinZoom.Size = new System.Drawing.Size(61, 20);
            this.numericUpDownMinZoom.TabIndex = 6;
            this.numericUpDownMinZoom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownMinZoom.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMinZoom.ValueChanged += new System.EventHandler(this.numericUpDownMinZoom_ValueChanged);
            // 
            // numericUpDownMaxZoom
            // 
            this.numericUpDownMaxZoom.Location = new System.Drawing.Point(74, 20);
            this.numericUpDownMaxZoom.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownMaxZoom.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownMaxZoom.Name = "numericUpDownMaxZoom";
            this.numericUpDownMaxZoom.Size = new System.Drawing.Size(61, 20);
            this.numericUpDownMaxZoom.TabIndex = 6;
            this.numericUpDownMaxZoom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.numericUpDownMaxZoom.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.numericUpDownMaxZoom.ValueChanged += new System.EventHandler(this.numericUpDownMaxZoom_ValueChanged);
            // 
            // TilePrefetcherMenu
            // 
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(256, 388);
            this.Controls.Add(this.numericUpDownMaxZoom);
            this.Controls.Add(this.numericUpDownMinZoom);
            this.Controls.Add(this.labelTotal);
            this.Controls.Add(this.textBoxTile);
            this.Controls.Add(this.labelMinZoom);
            this.Controls.Add(this.labelMaxZoom);
            this.Controls.Add(this.trackBarMinZoom);
            this.Controls.Add(this.trackBarMaxZoom);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TilePrefetcherMenu";
            this.Padding = new System.Windows.Forms.Padding(4);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Tile Prefetcher Menu";
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMaxZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarMinZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMinZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownMaxZoom)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TrackBar trackBarMaxZoom;
        private System.Windows.Forms.TrackBar trackBarMinZoom;
        private System.Windows.Forms.Label labelMaxZoom;
        private System.Windows.Forms.Label labelMinZoom;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.TextBox textBoxTile;
        private System.Windows.Forms.Label labelTotal;
        private System.Windows.Forms.NumericUpDown numericUpDownMinZoom;
        private System.Windows.Forms.NumericUpDown numericUpDownMaxZoom;
    }
}