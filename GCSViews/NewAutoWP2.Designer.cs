namespace MissionPlanner.GCSViews
{
    partial class NewAutoWP2
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.btnProduceRoute = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtStartAzimuth = new System.Windows.Forms.TextBox();
            this.txtPhotoNumber = new System.Windows.Forms.TextBox();
            this.txtRadius = new System.Windows.Forms.TextBox();
            this.txtCircleHeigth = new System.Windows.Forms.TextBox();
            this.txtBulidingHeigth = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnProduceRoute
            // 
            this.btnProduceRoute.Location = new System.Drawing.Point(253, 242);
            this.btnProduceRoute.Name = "btnProduceRoute";
            this.btnProduceRoute.Size = new System.Drawing.Size(75, 23);
            this.btnProduceRoute.TabIndex = 13;
            this.btnProduceRoute.Text = "生成航线";
            this.btnProduceRoute.UseVisualStyleBackColor = true;
            this.btnProduceRoute.Click += new System.EventHandler(this.btnProduceRoute_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(68, 242);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 8;
            this.label5.Text = "起始方位角";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(67, 194);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "每圈相片张数";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(68, 146);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "盘旋半径";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(220, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "盘旋高度";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(68, 101);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "建筑高度";
            // 
            // txtStartAzimuth
            // 
            this.txtStartAzimuth.Location = new System.Drawing.Point(137, 237);
            this.txtStartAzimuth.Name = "txtStartAzimuth";
            this.txtStartAzimuth.Size = new System.Drawing.Size(65, 21);
            this.txtStartAzimuth.TabIndex = 3;
            // 
            // txtPhotoNumber
            // 
            this.txtPhotoNumber.Location = new System.Drawing.Point(146, 189);
            this.txtPhotoNumber.Name = "txtPhotoNumber";
            this.txtPhotoNumber.Size = new System.Drawing.Size(65, 21);
            this.txtPhotoNumber.TabIndex = 4;
            // 
            // txtRadius
            // 
            this.txtRadius.Location = new System.Drawing.Point(123, 141);
            this.txtRadius.Name = "txtRadius";
            this.txtRadius.Size = new System.Drawing.Size(65, 21);
            this.txtRadius.TabIndex = 5;
            // 
            // txtCircleHeigth
            // 
            this.txtCircleHeigth.Location = new System.Drawing.Point(275, 97);
            this.txtCircleHeigth.Name = "txtCircleHeigth";
            this.txtCircleHeigth.Size = new System.Drawing.Size(65, 21);
            this.txtCircleHeigth.TabIndex = 6;
            // 
            // txtBulidingHeigth
            // 
            this.txtBulidingHeigth.Location = new System.Drawing.Point(123, 96);
            this.txtBulidingHeigth.Name = "txtBulidingHeigth";
            this.txtBulidingHeigth.Size = new System.Drawing.Size(65, 21);
            this.txtBulidingHeigth.TabIndex = 7;
            // 
            // NewAutoWP2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.CornflowerBlue;
            this.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
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
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Name = "NewAutoWP2";
            this.Size = new System.Drawing.Size(403, 356);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnProduceRoute;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtStartAzimuth;
        private System.Windows.Forms.TextBox txtPhotoNumber;
        private System.Windows.Forms.TextBox txtRadius;
        private System.Windows.Forms.TextBox txtCircleHeigth;
        private System.Windows.Forms.TextBox txtBulidingHeigth;
    }
}
