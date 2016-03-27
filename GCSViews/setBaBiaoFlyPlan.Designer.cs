namespace MissionPlanner.GCSViews
{
    partial class setBaBiaoFlyPlan
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbtnSensorDirection2 = new System.Windows.Forms.RadioButton();
            this.rbtnSensorDirection1 = new System.Windows.Forms.RadioButton();
            this.btnCalMoveByEdgeSpaceLengh = new System.Windows.Forms.Button();
            this.btnCalMoveByEdgeSpaceRatio = new System.Windows.Forms.Button();
            this.label26 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtWidthEdgeSpace = new System.Windows.Forms.TextBox();
            this.txtLengthEdgeSpace = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.txtEdgeSpace = new System.Windows.Forms.TextBox();
            this.label23 = new System.Windows.Forms.Label();
            this.txtWindEffect = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtWidthMove = new System.Windows.Forms.TextBox();
            this.txtGPS = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtBaBiaoWidth = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBaBiaoLength = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.txtLengthMove = new System.Windows.Forms.TextBox();
            this.txtFlyHeight = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSensorWidth = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtFocus = new System.Windows.Forms.TextBox();
            this.txtSensorLength = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.txtStartAzimuth = new System.Windows.Forms.TextBox();
            this.label28 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.groupBox1.Controls.Add(this.rbtnSensorDirection2);
            this.groupBox1.Controls.Add(this.rbtnSensorDirection1);
            this.groupBox1.Location = new System.Drawing.Point(72, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(155, 51);
            this.groupBox1.TabIndex = 98;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "相机长边与靶标长边";
            // 
            // rbtnSensorDirection2
            // 
            this.rbtnSensorDirection2.AutoSize = true;
            this.rbtnSensorDirection2.Enabled = false;
            this.rbtnSensorDirection2.Location = new System.Drawing.Point(88, 22);
            this.rbtnSensorDirection2.Name = "rbtnSensorDirection2";
            this.rbtnSensorDirection2.Size = new System.Drawing.Size(47, 16);
            this.rbtnSensorDirection2.TabIndex = 7;
            this.rbtnSensorDirection2.Text = "垂直";
            this.rbtnSensorDirection2.UseVisualStyleBackColor = true;
            // 
            // rbtnSensorDirection1
            // 
            this.rbtnSensorDirection1.AutoSize = true;
            this.rbtnSensorDirection1.Checked = true;
            this.rbtnSensorDirection1.Enabled = false;
            this.rbtnSensorDirection1.Location = new System.Drawing.Point(29, 22);
            this.rbtnSensorDirection1.Name = "rbtnSensorDirection1";
            this.rbtnSensorDirection1.Size = new System.Drawing.Size(47, 16);
            this.rbtnSensorDirection1.TabIndex = 6;
            this.rbtnSensorDirection1.TabStop = true;
            this.rbtnSensorDirection1.Text = "平行";
            this.rbtnSensorDirection1.UseVisualStyleBackColor = true;
            // 
            // btnCalMoveByEdgeSpaceLengh
            // 
            this.btnCalMoveByEdgeSpaceLengh.Location = new System.Drawing.Point(101, 365);
            this.btnCalMoveByEdgeSpaceLengh.Name = "btnCalMoveByEdgeSpaceLengh";
            this.btnCalMoveByEdgeSpaceLengh.Size = new System.Drawing.Size(179, 23);
            this.btnCalMoveByEdgeSpaceLengh.TabIndex = 97;
            this.btnCalMoveByEdgeSpaceLengh.Text = "依据长短边缘间距计算偏移量";
            this.btnCalMoveByEdgeSpaceLengh.UseVisualStyleBackColor = true;
            // 
            // btnCalMoveByEdgeSpaceRatio
            // 
            this.btnCalMoveByEdgeSpaceRatio.Location = new System.Drawing.Point(101, 326);
            this.btnCalMoveByEdgeSpaceRatio.Name = "btnCalMoveByEdgeSpaceRatio";
            this.btnCalMoveByEdgeSpaceRatio.Size = new System.Drawing.Size(179, 23);
            this.btnCalMoveByEdgeSpaceRatio.TabIndex = 96;
            this.btnCalMoveByEdgeSpaceRatio.Text = "依据边缘间距比例计算偏移量";
            this.btnCalMoveByEdgeSpaceRatio.UseVisualStyleBackColor = true;
            this.btnCalMoveByEdgeSpaceRatio.Click += new System.EventHandler(this.btnCalMoveByEdgeSpaceRatio_Click);
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.Location = new System.Drawing.Point(482, 230);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(41, 12);
            this.label26.TabIndex = 88;
            this.label26.Text = "（米）";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Location = new System.Drawing.Point(482, 189);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(41, 12);
            this.label24.TabIndex = 89;
            this.label24.Text = "（米）";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(482, 150);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(35, 12);
            this.label17.TabIndex = 90;
            this.label17.Text = "（%）";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(482, 111);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(41, 12);
            this.label12.TabIndex = 95;
            this.label12.Text = "（米）";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(459, 371);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(41, 12);
            this.label22.TabIndex = 92;
            this.label22.Text = "（米）";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(459, 333);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(41, 12);
            this.label21.TabIndex = 91;
            this.label21.Text = "（米）";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(226, 226);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(41, 12);
            this.label10.TabIndex = 93;
            this.label10.Text = "（米）";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(482, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 94;
            this.label3.Text = "（米）";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(482, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 87;
            this.label4.Text = "（米）";
            // 
            // txtWidthEdgeSpace
            // 
            this.txtWidthEdgeSpace.Location = new System.Drawing.Point(383, 225);
            this.txtWidthEdgeSpace.Name = "txtWidthEdgeSpace";
            this.txtWidthEdgeSpace.Size = new System.Drawing.Size(100, 21);
            this.txtWidthEdgeSpace.TabIndex = 85;
            // 
            // txtLengthEdgeSpace
            // 
            this.txtLengthEdgeSpace.Location = new System.Drawing.Point(383, 184);
            this.txtLengthEdgeSpace.Name = "txtLengthEdgeSpace";
            this.txtLengthEdgeSpace.Size = new System.Drawing.Size(100, 21);
            this.txtLengthEdgeSpace.TabIndex = 84;
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.Location = new System.Drawing.Point(302, 228);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(77, 12);
            this.label25.TabIndex = 74;
            this.label25.Text = "短边边缘间距";
            // 
            // txtEdgeSpace
            // 
            this.txtEdgeSpace.Location = new System.Drawing.Point(383, 145);
            this.txtEdgeSpace.Name = "txtEdgeSpace";
            this.txtEdgeSpace.Size = new System.Drawing.Size(100, 21);
            this.txtEdgeSpace.TabIndex = 83;
            this.txtEdgeSpace.Text = "10";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Location = new System.Drawing.Point(302, 187);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(77, 12);
            this.label23.TabIndex = 75;
            this.label23.Text = "长边边缘间距";
            // 
            // txtWindEffect
            // 
            this.txtWindEffect.Location = new System.Drawing.Point(383, 105);
            this.txtWindEffect.Name = "txtWindEffect";
            this.txtWindEffect.Size = new System.Drawing.Size(100, 21);
            this.txtWindEffect.TabIndex = 82;
            this.txtWindEffect.Text = "0";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(302, 148);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(77, 12);
            this.label14.TabIndex = 76;
            this.label14.Text = "边缘间距比例";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(326, 108);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 77;
            this.label11.Text = "风速影响";
            // 
            // txtWidthMove
            // 
            this.txtWidthMove.Location = new System.Drawing.Point(360, 366);
            this.txtWidthMove.Name = "txtWidthMove";
            this.txtWidthMove.Size = new System.Drawing.Size(100, 21);
            this.txtWidthMove.TabIndex = 80;
            // 
            // txtGPS
            // 
            this.txtGPS.Location = new System.Drawing.Point(127, 220);
            this.txtGPS.Name = "txtGPS";
            this.txtGPS.Size = new System.Drawing.Size(100, 21);
            this.txtGPS.TabIndex = 81;
            this.txtGPS.Text = "2";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(70, 223);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(47, 12);
            this.label8.TabIndex = 79;
            this.label8.Text = "GPS精度";
            // 
            // txtBaBiaoWidth
            // 
            this.txtBaBiaoWidth.Location = new System.Drawing.Point(383, 66);
            this.txtBaBiaoWidth.Name = "txtBaBiaoWidth";
            this.txtBaBiaoWidth.Size = new System.Drawing.Size(100, 21);
            this.txtBaBiaoWidth.TabIndex = 86;
            this.txtBaBiaoWidth.Text = "6";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(326, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 78;
            this.label5.Text = "靶标宽度";
            // 
            // txtBaBiaoLength
            // 
            this.txtBaBiaoLength.Location = new System.Drawing.Point(383, 27);
            this.txtBaBiaoLength.Name = "txtBaBiaoLength";
            this.txtBaBiaoLength.Size = new System.Drawing.Size(100, 21);
            this.txtBaBiaoLength.TabIndex = 73;
            this.txtBaBiaoLength.Text = "10";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(325, 30);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 72;
            this.label6.Text = "靶标长度";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(226, 264);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(41, 12);
            this.label18.TabIndex = 71;
            this.label18.Text = "（米）";
            // 
            // txtLengthMove
            // 
            this.txtLengthMove.Location = new System.Drawing.Point(360, 328);
            this.txtLengthMove.Name = "txtLengthMove";
            this.txtLengthMove.Size = new System.Drawing.Size(100, 21);
            this.txtLengthMove.TabIndex = 70;
            // 
            // txtFlyHeight
            // 
            this.txtFlyHeight.Location = new System.Drawing.Point(127, 259);
            this.txtFlyHeight.Name = "txtFlyHeight";
            this.txtFlyHeight.Size = new System.Drawing.Size(100, 21);
            this.txtFlyHeight.TabIndex = 69;
            this.txtFlyHeight.Text = "50";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(306, 369);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(53, 12);
            this.label20.TabIndex = 68;
            this.label20.Text = "短边偏移";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(306, 333);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(53, 12);
            this.label19.TabIndex = 67;
            this.label19.Text = "长边偏移";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(94, 262);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(29, 12);
            this.label9.TabIndex = 66;
            this.label9.Text = "航高";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(226, 185);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(53, 12);
            this.label16.TabIndex = 65;
            this.label16.Text = "（毫米）";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(226, 143);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 12);
            this.label15.TabIndex = 64;
            this.label15.Text = "（毫米）";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(227, 105);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 63;
            this.label7.Text = "（毫米）";
            // 
            // txtSensorWidth
            // 
            this.txtSensorWidth.Location = new System.Drawing.Point(127, 179);
            this.txtSensorWidth.Name = "txtSensorWidth";
            this.txtSensorWidth.Size = new System.Drawing.Size(100, 21);
            this.txtSensorWidth.TabIndex = 62;
            this.txtSensorWidth.Text = "15.4";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(56, 182);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 61;
            this.label2.Text = "传感器短边";
            // 
            // txtFocus
            // 
            this.txtFocus.Location = new System.Drawing.Point(127, 101);
            this.txtFocus.Name = "txtFocus";
            this.txtFocus.Size = new System.Drawing.Size(100, 21);
            this.txtFocus.TabIndex = 57;
            this.txtFocus.Text = "35";
            // 
            // txtSensorLength
            // 
            this.txtSensorLength.Location = new System.Drawing.Point(127, 139);
            this.txtSensorLength.Name = "txtSensorLength";
            this.txtSensorLength.Size = new System.Drawing.Size(100, 21);
            this.txtSensorLength.TabIndex = 60;
            this.txtSensorLength.Text = "23.2";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(68, 104);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(53, 12);
            this.label13.TabIndex = 59;
            this.label13.Text = "镜头焦距";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(56, 142);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 58;
            this.label1.Text = "传感器长边";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.Location = new System.Drawing.Point(339, 262);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(41, 12);
            this.label27.TabIndex = 66;
            this.label27.Text = "方位角";
            // 
            // txtStartAzimuth
            // 
            this.txtStartAzimuth.Location = new System.Drawing.Point(383, 259);
            this.txtStartAzimuth.Name = "txtStartAzimuth";
            this.txtStartAzimuth.Size = new System.Drawing.Size(100, 21);
            this.txtStartAzimuth.TabIndex = 69;
            this.txtStartAzimuth.Text = "0";
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.Location = new System.Drawing.Point(482, 264);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(41, 12);
            this.label28.TabIndex = 71;
            this.label28.Text = "（度）";
            // 
            // setBaBiaoFlyPlan
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 426);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCalMoveByEdgeSpaceLengh);
            this.Controls.Add(this.btnCalMoveByEdgeSpaceRatio);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtWidthEdgeSpace);
            this.Controls.Add(this.txtLengthEdgeSpace);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.txtEdgeSpace);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.txtWindEffect);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtWidthMove);
            this.Controls.Add(this.txtGPS);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtBaBiaoWidth);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtBaBiaoLength);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.txtLengthMove);
            this.Controls.Add(this.txtStartAzimuth);
            this.Controls.Add(this.txtFlyHeight);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtSensorWidth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtFocus);
            this.Controls.Add(this.txtSensorLength);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label1);
            this.Name = "setBaBiaoFlyPlan";
            this.Text = "setBaBiaoFlyPlan";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbtnSensorDirection2;
        private System.Windows.Forms.RadioButton rbtnSensorDirection1;
        private System.Windows.Forms.Button btnCalMoveByEdgeSpaceLengh;
        private System.Windows.Forms.Button btnCalMoveByEdgeSpaceRatio;
        private System.Windows.Forms.Label label26;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtWidthEdgeSpace;
        private System.Windows.Forms.TextBox txtLengthEdgeSpace;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.TextBox txtEdgeSpace;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.TextBox txtWindEffect;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtWidthMove;
        private System.Windows.Forms.TextBox txtGPS;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtBaBiaoWidth;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtBaBiaoLength;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txtLengthMove;
        private System.Windows.Forms.TextBox txtFlyHeight;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtSensorWidth;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtFocus;
        private System.Windows.Forms.TextBox txtSensorLength;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label27;
        private System.Windows.Forms.TextBox txtStartAzimuth;
        private System.Windows.Forms.Label label28;
    }
}