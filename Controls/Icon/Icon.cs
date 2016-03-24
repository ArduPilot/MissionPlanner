using System.Drawing;
using System.Windows.Forms;

namespace MissionPlanner.Controls.Icon
{
    public abstract class Icon
    {
        private Color _backColor = Color.Black;
        private Color _backColorSelected = Color.SeaGreen;
        private Color _foreColor = Color.WhiteSmoke;
        private int _lineWidth = 1;
        private int _width = 30;
        private int _height = 30;
        private bool _isSelected = false;
        private Point _location = new Point(0,0);

        public Pen LinePen { get; set; }
        public SolidBrush BgSolidBrush { get; set; }
        public SolidBrush BgSelectedSolidBrush { get; set; }

        public Icon()
        {
            updateColor();
        }

        void updateColor()
        {
            BgSolidBrush = new SolidBrush(_backColor);
            BgSelectedSolidBrush = new SolidBrush(_backColorSelected);
            LinePen = new Pen(_foreColor, _lineWidth);
        }

        public Color BackColor
        {
            get { return _backColor; }
            set { _backColor = value; updateColor(); }
        }

        public Color BackColorSelected
        {
            get { return _backColorSelected; }
            set { _backColorSelected = value; updateColor(); }
        }

        public Color ForeColor
        {
            get { return _foreColor; }
            set { _foreColor = value; updateColor(); }
        }

        public int LineWidth
        {
            get { return _lineWidth; }
            set { _lineWidth = value; updateColor(); }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set { _isSelected = value; }
        }

        public Point Location
        {
            get { return _location; }
            set { _location = value; }
        }

        public Rectangle Rectangle
        {
            get { return new Rectangle(Location.X, Location.Y, Width, Height); }
        }

        public void Paint(Graphics g)
        {
            // move 0,0 to out start location - no clipping is used, so we can draw anywhere on the parent control
            g.TranslateTransform(Location.X, Location.Y);

            var rect = new Rectangle(0, 0, _width, _height);

            if (IsSelected)
                g.FillPie(BgSelectedSolidBrush, rect, 0, 360);
            else 
                g.FillPie(BgSolidBrush, rect, 0, 360);
            g.DrawArc(LinePen, rect, 0, 360);

            doPaint(g);
        }

        internal abstract void doPaint(Graphics g);
    }
}
