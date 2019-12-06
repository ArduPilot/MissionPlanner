namespace MissionPlanner.Controls
{
    partial class ConnectionStats
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
			if (_subscriptionsDisposable != null)
    	        _subscriptionsDisposable.Dispose();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionStats));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_BytesReceived = new System.Windows.Forms.TextBox();
            this.txt_BytesPerSecondRx = new System.Windows.Forms.TextBox();
            this.txt_PacketsRx = new System.Windows.Forms.TextBox();
            this.txt_PacketsLost = new System.Windows.Forms.TextBox();
            this.txt_LinkQuality = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_PacketsPerSecond = new System.Windows.Forms.TextBox();
            this.txt_BytesSent = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txt_BytesPerSecondSent = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txt_MaxPacketInterval = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chk_signing = new System.Windows.Forms.CheckBox();
            this.chk_mavlink2 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // txt_BytesReceived
            // 
            this.txt_BytesReceived.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_BytesReceived, "txt_BytesReceived");
            this.txt_BytesReceived.Name = "txt_BytesReceived";
            this.txt_BytesReceived.ReadOnly = true;
            // 
            // txt_BytesPerSecondRx
            // 
            this.txt_BytesPerSecondRx.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_BytesPerSecondRx, "txt_BytesPerSecondRx");
            this.txt_BytesPerSecondRx.Name = "txt_BytesPerSecondRx";
            this.txt_BytesPerSecondRx.ReadOnly = true;
            // 
            // txt_PacketsRx
            // 
            this.txt_PacketsRx.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_PacketsRx, "txt_PacketsRx");
            this.txt_PacketsRx.Name = "txt_PacketsRx";
            this.txt_PacketsRx.ReadOnly = true;
            // 
            // txt_PacketsLost
            // 
            this.txt_PacketsLost.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_PacketsLost, "txt_PacketsLost");
            this.txt_PacketsLost.Name = "txt_PacketsLost";
            this.txt_PacketsLost.ReadOnly = true;
            // 
            // txt_LinkQuality
            // 
            this.txt_LinkQuality.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_LinkQuality, "txt_LinkQuality");
            this.txt_LinkQuality.Name = "txt_LinkQuality";
            this.txt_LinkQuality.ReadOnly = true;
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // txt_PacketsPerSecond
            // 
            this.txt_PacketsPerSecond.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_PacketsPerSecond, "txt_PacketsPerSecond");
            this.txt_PacketsPerSecond.Name = "txt_PacketsPerSecond";
            this.txt_PacketsPerSecond.ReadOnly = true;
            // 
            // txt_BytesSent
            // 
            this.txt_BytesSent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_BytesSent, "txt_BytesSent");
            this.txt_BytesSent.Name = "txt_BytesSent";
            this.txt_BytesSent.ReadOnly = true;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // label7
            // 
            resources.ApplyResources(this.label7, "label7");
            this.label7.Name = "label7";
            // 
            // txt_BytesPerSecondSent
            // 
            this.txt_BytesPerSecondSent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_BytesPerSecondSent, "txt_BytesPerSecondSent");
            this.txt_BytesPerSecondSent.Name = "txt_BytesPerSecondSent";
            this.txt_BytesPerSecondSent.ReadOnly = true;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.txt_MaxPacketInterval);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.txt_BytesReceived);
            this.groupBox1.Controls.Add(this.txt_PacketsPerSecond);
            this.groupBox1.Controls.Add(this.txt_BytesPerSecondRx);
            this.groupBox1.Controls.Add(this.txt_LinkQuality);
            this.groupBox1.Controls.Add(this.txt_PacketsRx);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txt_PacketsLost);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // txt_MaxPacketInterval
            // 
            this.txt_MaxPacketInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            resources.ApplyResources(this.txt_MaxPacketInterval, "txt_MaxPacketInterval");
            this.txt_MaxPacketInterval.Name = "txt_MaxPacketInterval";
            this.txt_MaxPacketInterval.ReadOnly = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txt_BytesPerSecondSent);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txt_BytesSent);
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // chk_signing
            // 
            resources.ApplyResources(this.chk_signing, "chk_signing");
            this.chk_signing.Name = "chk_signing";
            this.chk_signing.UseVisualStyleBackColor = true;
            this.chk_signing.CheckedChanged += new System.EventHandler(this.chk_signing_CheckedChanged);
            // 
            // chk_mavlink2
            // 
            resources.ApplyResources(this.chk_mavlink2, "chk_mavlink2");
            this.chk_mavlink2.Name = "chk_mavlink2";
            this.chk_mavlink2.UseVisualStyleBackColor = true;
            this.chk_mavlink2.CheckedChanged += new System.EventHandler(this.chk_mavlink2_CheckedChanged);
            // 
            // ConnectionStats
            // 
            
            this.Controls.Add(this.chk_mavlink2);
            this.Controls.Add(this.chk_signing);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.Name = "ConnectionStats";
            resources.ApplyResources(this, "$this");
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_BytesReceived;
        private System.Windows.Forms.TextBox txt_BytesPerSecondRx;
        private System.Windows.Forms.TextBox txt_PacketsRx;
        private System.Windows.Forms.TextBox txt_PacketsLost;
        private System.Windows.Forms.TextBox txt_LinkQuality;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_PacketsPerSecond;
        private System.Windows.Forms.TextBox txt_BytesSent;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt_BytesPerSecondSent;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txt_MaxPacketInterval;
        private System.Windows.Forms.CheckBox chk_signing;
        private System.Windows.Forms.CheckBox chk_mavlink2;
    }
}
