using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class WindDir : UserControl
    {
        public WindDir()
        {
            InitializeComponent();
            this.BackColor = Color.Transparent;
            this.DoubleBuffered = true;
        }

        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);
        double _direction = 0;
        double _speed = 0;

        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Options")]
        public double Direction { get { return _direction; } set { _direction = value; this.Invalidate(); } }
        [System.ComponentModel.Browsable(true), System.ComponentModel.Category("Options")]
        public double Speed { get { return _speed; } set { _speed = value; this.Invalidate(); } }

        Pen blackpen = new Pen(Color.Black,2);
        Pen redpen = new Pen(Color.Red, 2);

        protected override void OnPaint(PaintEventArgs e)
        {
           // e.Graphics.Clear(Color.Transparent);

            if (_direction > 360)
                _direction = _direction % 360;

            base.OnPaint(e);

            Rectangle outside = new Rectangle(1,1,this.Width - 3, this.Height -3);

            e.Graphics.DrawArc(blackpen, outside, 0, 360);

            Rectangle inside = new Rectangle(this.Width / 4,this.Height / 4, (this.Width/4) * 2,(this.Height / 4) * 2);

            e.Graphics.DrawArc(blackpen, inside, 0, 360);

            double x = (this.Width / 2) * Math.Cos((_direction - 90) * deg2rad);

            double y = (this.Height / 2) * Math.Sin((_direction-90) * deg2rad);

            // full scale is 10ms

            x = x/10 * Speed;
            y = y/10 * Speed;

            if (x != 0 || y != 0)
                e.Graphics.DrawLine(redpen, this.Width / 2, this.Height / 2, (float)(this.Width / 2 + x), (float)(this.Height / 2 + y));
            e.Graphics.DrawString(Speed.ToString("0"), this.Font, Brushes.Red, (float)(this.Width / 2), (float)(this.Height / 2));
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            
            base.OnPaintBackground(e);
            //e.Graphics.Clear(Color.Transparent);
        }
    }
}
