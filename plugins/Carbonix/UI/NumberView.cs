using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SkiaSharp.Views.Desktop;

namespace Carbonix
{
    // Improved version of QuickView
    public partial class NumberView : MissionPlanner.Controls.QuickView
    {

        public NumberView()
        {
            DoubleBuffered = true;
            
            // Get rid of the inherited toolTip1. Have to do this the ugly way because it is private.
            ((ToolTip)typeof(MissionPlanner.Controls.QuickView).GetField("toolTip1", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this)).Dispose();

            PaintSurface += OnPaintSurface;
        }

        // Set this value to force the text size to fit this many "0" characters in the view
        // This allows for matching the text size of adjacent views.
        public int charWidth = -1;

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e2)
        {
            var e = new SkiaGraphics(e2.Surface);
            e2.Surface.Canvas.Clear();
            int y = 0;
            {
                Size extent = e.MeasureString(desc, this.Font).ToSize();

                // Wrap string if it's too wide for the container
                List<string> lines = new List<string>();
                if (extent.Width > Width)
                {
                    string[] words = desc.Split(' ');
                    int linelength = 0; // px
                    foreach (string word in words)
                    {
                        Size wordExtent = e.MeasureString(word, this.Font).ToSize();
                        if (linelength ==0 || wordExtent.Width + linelength > Width)
                        {
                            lines.Add(word);
                            linelength = wordExtent.Width;
                        }
                        else
                        {
                            lines[lines.Count-1] += " " + word;
                            linelength += wordExtent.Width;
                        }
                    }
                }
                else
                {
                    lines.Add(desc);
                }

                foreach(string line in lines)
                {
                    extent = e.MeasureString(line, this.Font).ToSize();
                    var mid = extent.Width / 2;
                    e.DrawString(line, this.Font, new SolidBrush(this.ForeColor), this.Width / 2 - mid, 5 + y);
                    y += extent.Height;
                }

            }
            //
            {
                string numb;
                if (numberformat.Contains(":"))
                {
                    numb = TimeSpan.FromSeconds(number).ToString(numberformat);
                }
                else
                {
                    numb = number.ToString(numberformat);
                }

                var charWidth = this.charWidth > 0 ? this.charWidth : numb.Length + 1;
                Size extent = e.MeasureString("0".PadLeft(charWidth, '0'), new Font(this.Font.FontFamily, (float)newSize, this.Font.Style)).ToSize();

                float hRatio = (this.Height - y) / (float)(extent.Height);
                float wRatio = this.Width / (float)extent.Width;
                float ratio = (hRatio < wRatio) ? hRatio : wRatio;

                newSize *= ratio;

                // I think this is to provide hysteresis to prevent resizing oscillation, but it doesn't work well for small views
                if (newSize > 30)
                {
                    newSize -= newSize % 5;
                }
                else
                {
                    newSize -= newSize % 1;
                }
              
                if (newSize < 8 || newSize > 999999)
                    newSize = 8;

                extent = e.MeasureString(numb, new Font(this.Font.FontFamily, (float)newSize, this.Font.Style)).ToSize();

                // Assume single-line description text.
                // This keeps adjacent views aligned at the cost of potentially overlaping the description.
                y = e.MeasureString(desc, this.Font).ToSize().Height;

                e.DrawString(numb, new Font(this.Font.FontFamily, (float)newSize, this.Font.Style), new SolidBrush(this.numberColor), this.Width / 2 - extent.Width / 2, y + ((this.Height - y) / 2 - extent.Height / 2));
            }
        }

        public override void Refresh()
        {
            if (this.Visible)
                base.Refresh();
        }

        protected override void WndProc(ref Message m) // seems to crash here on linux... so try ignore it
        {
            try
            {
                base.WndProc(ref m);
            }
            catch { }
        }

        protected override void OnInvalidated(InvalidateEventArgs e)
        {
            if (this.Visible)
                base.OnInvalidated(e);
        }

        float newSize = 8;

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Invalidate();
        }
    }
}
