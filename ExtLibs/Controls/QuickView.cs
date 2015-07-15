﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class QuickView : UserControl
    {
        //http://stackoverflow.com/questions/3816362/winforms-label-flickering

        [System.ComponentModel.Browsable(true)]
        public string desc
        {
            get
            {
                return labelWithPseudoOpacity1.Text;
            }
            set
            {
                if (labelWithPseudoOpacity1.Text == value)
                    return;
                
                labelWithPseudoOpacity1.Text = value;
            }
        }
        [System.ComponentModel.Browsable(true)]
        public double number { get { return double.Parse(labelWithPseudoOpacity2.Text); } 
            set {
                string ans = (value).ToString(_numberformat);
                if (labelWithPseudoOpacity2.Text == ans) 
                    return;
                
                string before = labelWithPseudoOpacity2.Text;
                labelWithPseudoOpacity2.Text = ans;

                // only run when needed
                if (before.Length < ans.Length)
                    GetFontSize();
            }
        }

        string _numberformat = "0.00";
        [System.ComponentModel.Browsable(true)]
        public string numberformat { get { return _numberformat; } set { _numberformat = value; this.Invalidate(); } }

        [System.ComponentModel.Browsable(true)]
        public Color numberColor { get { return labelWithPseudoOpacity2.ForeColor; } set { if (labelWithPseudoOpacity2.ForeColor == value) return; labelWithPseudoOpacity2.ForeColor = value; } }

        public QuickView()
        {
            InitializeComponent();

            labelWithPseudoOpacity1.DoubleClick += new EventHandler(labelWithPseudoOpacity1_DoubleClick);
            labelWithPseudoOpacity2.DoubleClick += new EventHandler(labelWithPseudoOpacity2_DoubleClick);

            // set the initial value as something invalid
            number = -9999;
        }

        void labelWithPseudoOpacity2_DoubleClick(object sender, EventArgs e)
        {
            this.OnDoubleClick(e);
        }

        void labelWithPseudoOpacity1_DoubleClick(object sender, EventArgs e)
        {
            this.OnDoubleClick(e);
        }

        public override void Refresh()
        {
            if (this.Visible)
                base.Refresh();
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            if (this.Visible && this.ThisReallyVisible())
                base.OnInvalidated(e);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Visible)
                base.OnPaint(e);
        }

        void GetFontSize()
        {
            Size extent = TextRenderer.MeasureText(labelWithPseudoOpacity2.Text, this.Font);

            //SizeF extenttest2 = Graphics.FromHwnd(this.Handle).MeasureString(labelWithPseudoOpacity2.Text, this.Font);

         //   SizeF extent2 = Measure.MeasureString(Graphics.FromHwnd(this.Handle), this.Font, labelWithPseudoOpacity2.Text);

           // extent = extenttest;

            float hRatio = (labelWithPseudoOpacity2.Height) / (float)(extent.Height);
            float wRatio = this.Width / (float)extent.Width;
            float ratio = (hRatio < wRatio) ? hRatio : wRatio;

            float newSize = this.Font.Size * ratio;

            if (newSize < 8)
                newSize = 8;

            labelWithPseudoOpacity2.Font = new Font(labelWithPseudoOpacity2.Font.FontFamily, newSize - 2, labelWithPseudoOpacity2.Font.Style);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.ResizeRedraw = true;

            GetFontSize();

          
        }

        private void QuickView_Resize(object sender, EventArgs e)
        {
            this.Invalidate();
        }
    }
}
