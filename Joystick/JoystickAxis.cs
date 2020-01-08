using System;
using System.Windows.Forms;

namespace MissionPlanner.Joystick
{
    public class JoystickAxis : UserControl
    {
        public JoystickAxis()
        {
            InitializeComponent();
        }

        private Controls.MyButton BUT_detch;
        private CheckBox revCH;
        private Label label13;
        private TextBox expo_ch;
        private Controls.HorizontalProgressBar ProgressBarCH;
        private ComboBox CMB_CH;

        public Func<string> Detect;
        public Action SetAxis;
        public Action Reverse;
        public Action Expo;
        public Func<int> GetValue;

        private Timer timer1;

        public int ChannelNo
        {
            get;
            set;
        }

        public string Label
        {
            get { return label13.Text; }
            set { label13.Text = value; }
        }

        public Array AxisArray
        {
            get { return (Array)CMB_CH.DataSource; }
            set { CMB_CH.DataSource = value; }
        }

        public string ChannelValue
        {
            get { return CMB_CH.Text; }
            set { CMB_CH.Text = value; }
        }

        public string ExpoValue
        {
            get { return expo_ch.Text; }
            set { expo_ch.Text = value; }
        }

        public bool ReverseValue
        {
            get { return revCH.Checked; }
            set { revCH.Checked = value; }
        }

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
            this.BUT_detch = new MissionPlanner.Controls.MyButton();
            this.revCH = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.expo_ch = new System.Windows.Forms.TextBox();
            this.ProgressBarCH = new MissionPlanner.Controls.HorizontalProgressBar();
            this.CMB_CH = new System.Windows.Forms.ComboBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // BUT_detch
            // 
            this.BUT_detch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.BUT_detch.Location = new System.Drawing.Point(141, 3);
            this.BUT_detch.Name = "BUT_detch";
            this.BUT_detch.Size = new System.Drawing.Size(45, 23);
            this.BUT_detch.TabIndex = 62;
            this.BUT_detch.Text = "Auto Detect";
            this.BUT_detch.UseVisualStyleBackColor = true;
            this.BUT_detch.Click += new System.EventHandler(this.BUT_detch_Click);
            // 
            // revCH
            // 
            this.revCH.AutoSize = true;
            this.revCH.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.revCH.Location = new System.Drawing.Point(406, 6);
            this.revCH.Name = "revCH";
            this.revCH.Size = new System.Drawing.Size(15, 14);
            this.revCH.TabIndex = 61;
            this.revCH.UseVisualStyleBackColor = true;
            this.revCH.CheckedChanged += new System.EventHandler(this.revCH_CheckedChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(2, 8);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(22, 13);
            this.label13.TabIndex = 60;
            this.label13.Text = "CH";
            // 
            // expo_ch
            // 
            this.expo_ch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.expo_ch.Location = new System.Drawing.Point(300, 7);
            this.expo_ch.Name = "expo_ch";
            this.expo_ch.Size = new System.Drawing.Size(100, 13);
            this.expo_ch.TabIndex = 59;
            this.expo_ch.Text = "0";
            this.expo_ch.TextChanged += new System.EventHandler(this.expo_ch_TextChanged);
            // 
            // ProgressBarCH
            // 
            this.ProgressBarCH.DrawLabel = true;
            this.ProgressBarCH.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ProgressBarCH.Label = null;
            this.ProgressBarCH.Location = new System.Drawing.Point(192, 2);
            this.ProgressBarCH.Maximum = 2200;
            this.ProgressBarCH.maxline = 0;
            this.ProgressBarCH.Minimum = 800;
            this.ProgressBarCH.minline = 0;
            this.ProgressBarCH.Name = "ProgressBarCH";
            this.ProgressBarCH.Size = new System.Drawing.Size(100, 23);
            this.ProgressBarCH.TabIndex = 58;
            this.ProgressBarCH.Value = 800;
            // 
            // CMB_CH
            // 
            this.CMB_CH.FormattingEnabled = true;
            this.CMB_CH.Items.AddRange(new object[] {
            "RZ",
            "X",
            "Y",
            "SL1"});
            this.CMB_CH.Location = new System.Drawing.Point(65, 3);
            this.CMB_CH.Name = "CMB_CH";
            this.CMB_CH.Size = new System.Drawing.Size(70, 21);
            this.CMB_CH.TabIndex = 57;
            this.CMB_CH.SelectedIndexChanged += new System.EventHandler(this.CMB_CH_SelectedIndexChanged);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // JoystickAxis
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.BUT_detch);
            this.Controls.Add(this.revCH);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.expo_ch);
            this.Controls.Add(this.ProgressBarCH);
            this.Controls.Add(this.CMB_CH);
            this.Name = "JoystickAxis";
            this.Size = new System.Drawing.Size(427, 28);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private void CMB_CH_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetAxis?.Invoke();
        }

        private void BUT_detch_Click(object sender, EventArgs e)
        {
            ChannelValue = Detect?.Invoke();
        }

        private void expo_ch_TextChanged(object sender, EventArgs e)
        {
            Expo?.Invoke();
        }

        private void revCH_CheckedChanged(object sender, EventArgs e)
        {
            Reverse?.Invoke();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (GetValue == null)
                return;

            ProgressBarCH.maxline = GetValue();
            ProgressBarCH.Value = GetValue();
        }
    }
}
