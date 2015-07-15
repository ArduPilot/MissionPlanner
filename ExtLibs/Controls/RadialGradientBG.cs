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
    public partial class RadialGradientBG : UserControl
    {
         [           Browsable(true)]
        public Color CenterColor  {get;set;}
        [Browsable(true)]
        public Color OutsideColor {get;set;}

           [
           Browsable(true), 
           Description("Image"),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content) 
        ]
        public PictureBox Image { get { return this._Image; }} 

        public RadialGradientBG()
        {
            InitializeComponent();
            CenterColor = Color.FromArgb(121,164,33);
            OutsideColor = Color.FromArgb(67,107,10);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            RectangleF bounds = this.Bounds;
            using (var ellipsePath = new GraphicsPath())
            {
                ellipsePath.AddEllipse(bounds);
                using (var brush = new PathGradientBrush(ellipsePath))
                {
                    brush.CenterPoint = new PointF(bounds.Width / 2f, bounds.Height / 2f);
                    brush.CenterColor = CenterColor;
                    brush.SurroundColors = new[] { OutsideColor };
                    brush.FocusScales = new PointF(0f, 0.3f);

                    // fil lthe edge
                    e.Graphics.FillRectangle(new SolidBrush(OutsideColor), bounds);
                    // draw the gradient
                    e.Graphics.FillRectangle(brush, bounds);
                }
            }
        }
    }
}
