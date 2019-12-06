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
    public partial class GradientBG : MyUserControl
    {
        [Browsable(true)]
        public Color CenterColor { get; set; }
        [Browsable(true)]
        public Color OutsideColor { get; set; }

        [
        Browsable(true),
        Description("Image"),
     DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
     ]
        public PictureBox Image { get { return this._Image; } }

        [
    Browsable(true),
    Description("Label"),
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
 ]
        public Label Label { get { return this._Label; } }

        public GradientBG()
        {
            InitializeComponent();
            CenterColor = Color.FromArgb(121, 164, 33);
            OutsideColor = Color.FromArgb(67, 107, 10);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);

            LinearGradientBrush lgb = new LinearGradientBrush(this.Bounds, OutsideColor, CenterColor, LinearGradientMode.Vertical);

            e.Graphics.FillRectangle(lgb, this.Bounds);

            e.Graphics.DrawLine(new Pen(Color.White), 0, this.Height - 1, this.Width, this.Height - 1);
        }
    }
}