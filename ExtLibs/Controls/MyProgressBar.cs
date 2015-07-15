using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace MissionPlanner.Controls
{
    public partial class MyProgressBar : UserControl
    {
        //
        // Summary:
        //     Gets or sets the maximum value of the range of the control.
        //
        // Returns:
        //     The maximum value of the range. The default is 100.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The value specified is less than 0.
        [DefaultValue(100)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int Maximum { get; set; }
        //
        // Summary:
        //     Gets or sets the minimum value of the range of the control.
        //
        // Returns:
        //     The minimum value of the range. The default is 0.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The value specified for the property is less than 0.
        [DefaultValue(0)]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int Minimum { get; set; }
        //
        // Summary:
        //     Gets or sets the current position of the progress bar.
        //
        // Returns:
        //     The position within the range of the progress bar. The default is 0.
        //
        // Exceptions:
        //   System.ArgumentException:
        //     The value specified is greater than the value of the System.Windows.Forms.ProgressBar.Maximum
        //     property.  -or- The value specified is less than the value of the System.Windows.Forms.ProgressBar.Minimum
        //     property.
        [Bindable(true)]
        [DefaultValue(0)]
        public int Value { get; set; }

        public int BarSize = 40;

        internal Color _BGGradTop = Color.FromArgb(102, 139, 26);
        internal Color _BGGradBot = Color.FromArgb(127, 167, 42);
        internal Color _TextColor = Color.FromArgb(31, 54, 8);
        internal Color _Outline = Color.FromArgb(150, 171, 112);

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
        public Color BGGradTop { get { return _BGGradTop; } set { _BGGradTop = value; } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
        public Color BGGradBot { get { return _BGGradBot; } set { _BGGradBot = value; } }

        // i want to ignore forecolor
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
        public Color TextColor { get { return _TextColor; } set { _TextColor = value; } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Colors")]
        public Color Outline { get { return _Outline; } set { _Outline = value; } }

        public MyProgressBar()
        {
            InitializeComponent();

            Maximum = 100;
            Minimum = 0;
            Value = 0;
        }

        void marquee_Tick(object sender, EventArgs e)
        {
            int delta = Maximum - Minimum;
            if (delta == 0)
                delta = 100;

            int newvalue = Value + (delta) / 50;
            if (newvalue > Maximum)
                newvalue = -BarSize;
            if (newvalue < (Minimum - BarSize))
                newvalue = -BarSize;
            Value = newvalue;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);

            int delta = Maximum - Minimum;
            if (delta == 0)
                delta = 100;
            int position = (int)((Value / (float)delta) * this.Width);

            Rectangle outside = new Rectangle(new Point(1, 1), new System.Drawing.Size(this.Width - 2,this.Height - 2));

            LinearGradientBrush linear = new LinearGradientBrush(outside, BGGradTop, BGGradBot, LinearGradientMode.Vertical);

            Rectangle progressdone = new Rectangle(new Point(1, 1), new System.Drawing.Size(position,this.Height -2));

            if (Style == ProgressBarStyle.Marquee)
                progressdone = new Rectangle(new Point(position, 1), new System.Drawing.Size(BarSize, this.Height - 2));

            // fill the background
            e.Graphics.FillRectangle(new SolidBrush(BackColor), outside);
            // draw the progress
            e.Graphics.FillRectangle(linear, progressdone);
            // draw the outside
            e.Graphics.DrawRectangle(new Pen(Outline,1),new Rectangle(0,0,this.Width-1,this.Height-1));
        }

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
        }

        ProgressBarStyle _style = ProgressBarStyle.Continuous;
        public ProgressBarStyle Style
        {
            get 
            { 
                return _style; 
            }
            set
            {
                _style = value;
                if (_style == ProgressBarStyle.Marquee)
                {
                    if (!marquee.Enabled)
                        marquee.Start();
                }
                else
                {
                    if (marquee.Enabled)
                        marquee.Stop(); 
                }

                this.Invalidate();
            }
        }
    }
}
