using OSDConfigurator.Models;
using OSDConfigurator.Properties;
using System.Collections.Generic;
using System.Drawing;

namespace OSDConfigurator.GUI
{
    public class Visualizer
    {
        private readonly Size fontCharSizePix = new Size(12, 18); 
        private readonly Size charSizePix = new Size(24, 36); 
        private readonly Size screenSizeChar = new Size(30, 16); 
        private readonly int NtscRows = 13; 

        private static Font noteFont = new Font("Arial", 6, FontStyle.Regular);

        private static List<Bitmap> font;
        
        public Visualizer()
        {
            if (font == null)
                font = MakeFont(Resources.clarity, 16, 16, 1);
        }
        
        public Size GetCanvasSize()
        {
            return new Size(charSizePix.Width * screenSizeChar.Width, charSizePix.Height * screenSizeChar.Height);
        }

        public Point ToOSDLocation(Point screenPoint)
        {
            return new Point(screenPoint.X / charSizePix.Width, screenPoint.Y / charSizePix.Height);
        }

        public Point ToScreenPoint(OSDItem item)
        {
            return new Point((int)item.X.Value * charSizePix.Width, (int)item.Y.Value * charSizePix.Height);
        }

        private Rectangle ToScreenRectangle(OSDItem item)
        {
            var loc = ToScreenPoint(item);
            var size = new Size(ItemCaptions.GetCaption(item).Length * charSizePix.Width, charSizePix.Height);
            return new Rectangle(loc, size);
        }

        public void Draw(OSDItem item, Graphics graphics)
        {
            Point loc = ToScreenPoint(item);
            
            foreach (var c in ItemCaptions.GetCaption(item))
            {
                graphics.DrawImage(font[c], loc.X, loc.Y, charSizePix.Width, charSizePix.Height);
                loc.X += charSizePix.Width;
            }
        }

        public void DrawSelection(OSDItem item, Graphics graphics)
        {
            if (item == null)
                return;

            var rect = ToScreenRectangle(item);
            rect.Inflate(5, 5);

            graphics.DrawRectangle(Pens.Yellow, rect);
        }

        private List<Bitmap> MakeFont(Bitmap source, int horCount, int vertCount, int margin)
        {
            List<Bitmap> font = new List<Bitmap>();
            
            Rectangle chRect = new Rectangle(0, 0, fontCharSizePix.Width, fontCharSizePix.Height);
            Rectangle srcRect = new Rectangle(0, 0, fontCharSizePix.Width, fontCharSizePix.Height);

            for (int h = 0; h< horCount; ++h)
            {
                srcRect.X = 0;

                for (int v = 0; v < vertCount; ++v)
                {
                    var ch = new Bitmap(fontCharSizePix.Width, fontCharSizePix.Height);

                    using (Graphics grD = Graphics.FromImage(ch))
                    {
                        grD.DrawImage(source, chRect, srcRect, GraphicsUnit.Pixel);
                    }

                    font.Add(ch);

                    srcRect.X += fontCharSizePix.Width + margin;
                }

                srcRect.Y += fontCharSizePix.Height + margin;
            }

            return font;
        }

        public bool Contains(OSDItem item, Point point)
        {
            return ToScreenRectangle(item).Contains(point);
        }

        internal void DrawBackground(Graphics g)
        {
            g.DrawLine(Pens.DimGray, 0, charSizePix.Height * NtscRows, charSizePix.Width * screenSizeChar.Width, charSizePix.Height * NtscRows);
            g.DrawString("NTSC", noteFont, Brushes.Black, noteFont.Size, charSizePix.Height * NtscRows - noteFont.Size * 2);
            g.DrawString("PAL", noteFont, Brushes.Black, noteFont.Size, charSizePix.Height * NtscRows + noteFont.Size * 1.5f);
        }
    }
}
