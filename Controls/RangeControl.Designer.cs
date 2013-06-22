namespace ArdupilotMega.Controls
{
   partial class RangeControl
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
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.trackBar1 = new ArdupilotMega.Controls.MyTrackBar();
            this.myLabel1 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.LBL_max = new System.Windows.Forms.Label();
            this.LBL_min = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.DecimalPlaces = 3;
            this.numericUpDown1.Location = new System.Drawing.Point(3, 32);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(57, 20);
            this.numericUpDown1.TabIndex = 4;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.AutoSize = true;
            this.tableLayoutPanel3.ColumnCount = 4;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.Controls.Add(this.trackBar1, 1, 2);
            this.tableLayoutPanel3.Controls.Add(this.myLabel1, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.LBL_max, 2, 3);
            this.tableLayoutPanel3.Controls.Add(this.LBL_min, 1, 3);
            this.tableLayoutPanel3.Controls.Add(this.numericUpDown1, 0, 2);
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 4;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(487, 98);
            this.tableLayoutPanel3.TabIndex = 6;
            // 
            // trackBar1
            // 
            this.tableLayoutPanel3.SetColumnSpan(this.trackBar1, 2);
            this.trackBar1.LargeChange = 0.004F;
            this.trackBar1.Location = new System.Drawing.Point(66, 32);
            this.trackBar1.Maximum = 100F;
            this.trackBar1.Minimum = 0F;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(314, 45);
            this.trackBar1.SmallChange = 1F;
            this.trackBar1.TabIndex = 3;
            this.trackBar1.TickFrequency = 10F;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackBar1.Value = 0F;
            // 
            // myLabel1
            // 
            this.myLabel1.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this.myLabel1, 4);
            this.myLabel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.myLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.myLabel1.Location = new System.Drawing.Point(3, 0);
            this.myLabel1.Name = "myLabel1";
            this.myLabel1.Size = new System.Drawing.Size(481, 16);
            this.myLabel1.TabIndex = 1;
            this.myLabel1.Text = "myLabel1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.tableLayoutPanel3.SetColumnSpan(this.label1, 4);
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(481, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "label1";
            // 
            // LBL_max
            // 
            this.LBL_max.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.LBL_max.AutoSize = true;
            this.LBL_max.Location = new System.Drawing.Point(367, 80);
            this.LBL_max.Name = "LBL_max";
            this.LBL_max.Size = new System.Drawing.Size(13, 13);
            this.LBL_max.TabIndex = 6;
            this.LBL_max.Text = "1";
            // 
            // LBL_min
            // 
            this.LBL_min.AutoSize = true;
            this.LBL_min.Location = new System.Drawing.Point(66, 80);
            this.LBL_min.Name = "LBL_min";
            this.LBL_min.Size = new System.Drawing.Size(13, 13);
            this.LBL_min.TabIndex = 7;
            this.LBL_min.Text = "0";
            // 
            // RangeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel3);
            this.Name = "RangeControl";
            this.Size = new System.Drawing.Size(493, 104);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private MyTrackBar trackBar1;
      private System.Windows.Forms.NumericUpDown numericUpDown1;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
      private System.Windows.Forms.Label myLabel1;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.Label LBL_max;
      private System.Windows.Forms.Label LBL_min;

   }
}
