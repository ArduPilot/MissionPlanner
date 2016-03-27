namespace MissionPlanner.GCSViews
{
    partial class NewAutoWP
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
            this.txtBulidingHeigth = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCircleHeigth = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtRadius = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtPhotoNumber = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btnProduceRoute = new System.Windows.Forms.Button();
            this.txtStartAzimuth = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtBulidingHeigth
            // 
            this.txtBulidingHeigth.Location = new System.Drawing.Point(83, 23);
            this.txtBulidingHeigth.Name = "txtBulidingHeigth";
            this.txtBulidingHeigth.Size = new System.Drawing.Size(65, 21);
            this.txtBulidingHeigth.TabIndex = 0;
            this.txtBulidingHeigth.Text = "30";
            this.txtBulidingHeigth.TextChanged += new System.EventHandler(this.txtBulidingHeigth_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "建筑高度";
            // 
            // txtCircleHeigth
            // 
            this.txtCircleHeigth.Location = new System.Drawing.Point(235, 24);
            this.txtCircleHeigth.Name = "txtCircleHeigth";
            this.txtCircleHeigth.Size = new System.Drawing.Size(140, 21);
            this.txtCircleHeigth.TabIndex = 0;
            this.txtCircleHeigth.Text = "40,50";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(180, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "盘旋高度";
            // 
            // txtRadius
            // 
            this.txtRadius.Location = new System.Drawing.Point(83, 68);
            this.txtRadius.Name = "txtRadius";
            this.txtRadius.Size = new System.Drawing.Size(150, 21);
            this.txtRadius.TabIndex = 0;
            this.txtRadius.Text = "30,40";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "盘旋半径";
            // 
            // txtPhotoNumber
            // 
            this.txtPhotoNumber.Location = new System.Drawing.Point(106, 116);
            this.txtPhotoNumber.Name = "txtPhotoNumber";
            this.txtPhotoNumber.Size = new System.Drawing.Size(65, 21);
            this.txtPhotoNumber.TabIndex = 0;
            this.txtPhotoNumber.Text = "20";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 121);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "每圈相片张数";
            // 
            // btnProduceRoute
            // 
            this.btnProduceRoute.Location = new System.Drawing.Point(275, 162);
            this.btnProduceRoute.Name = "btnProduceRoute";
            this.btnProduceRoute.Size = new System.Drawing.Size(75, 23);
            this.btnProduceRoute.TabIndex = 2;
            this.btnProduceRoute.Text = "生成航线";
            this.btnProduceRoute.UseVisualStyleBackColor = true;
            this.btnProduceRoute.Click += new System.EventHandler(this.btnProduceRoute_Click);
            // 
            // txtStartAzimuth
            // 
            this.txtStartAzimuth.Location = new System.Drawing.Point(97, 164);
            this.txtStartAzimuth.Name = "txtStartAzimuth";
            this.txtStartAzimuth.Size = new System.Drawing.Size(65, 21);
            this.txtStartAzimuth.TabIndex = 0;
            this.txtStartAzimuth.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "起始方位角";
            // 
            // NewAutoWP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 230);
            this.Controls.Add(this.btnProduceRoute);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtStartAzimuth);
            this.Controls.Add(this.txtPhotoNumber);
            this.Controls.Add(this.txtRadius);
            this.Controls.Add(this.txtCircleHeigth);
            this.Controls.Add(this.txtBulidingHeigth);
            this.Name = "NewAutoWP";
            this.Text = "创建曲线圈";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBulidingHeigth;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCircleHeigth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtRadius;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtPhotoNumber;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnProduceRoute;
        private System.Windows.Forms.TextBox txtStartAzimuth;
        private System.Windows.Forms.Label label5;
    }
}