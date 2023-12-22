using OSDConfigurator.Models;
using OSDConfigurator.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace OSDConfigurator.GUI
{
    public class Visualizer
    {
        private Size charSizePix= new Size(24, 36);
        private readonly Size fontCharSizePix = new Size(12, 18); 
        private Size screenSizeChar = new Size(30, 16); 
        private readonly int NtscRows = 13;
        private readonly int DJICols = 50;
        private readonly int DJIRows = 18;
        private readonly IItemCaptionProvider captionProvider;
        private static Font sdNoteFont = new Font("Arial", 6, FontStyle.Regular);
        private static Font hdNoteFont = new Font("Arial", 8, FontStyle.Regular);

        private static List<Bitmap> font;
        
        public Visualizer(IItemCaptionProvider captionProvider)
        {
            if (font == null)
                font = MakeFont(Resources.clarity, 16, 16, 1);
            this.captionProvider = captionProvider;
        }
        
        public Size GetCanvasSize(Size charSizePix)
        {
            this.charSizePix = charSizePix;
            return new Size(charSizePix.Width * screenSizeChar.Width, charSizePix.Height * screenSizeChar.Height);
        }

        public void SetScreenSizeChar(Size screenSize)
        {
            screenSizeChar = screenSize;
        }
        
        public Point ToOSDLocation(Point screenPoint)
        {
            screenPoint.X = screenPoint.X < 0 ? 0 : screenPoint.X;
            screenPoint.Y = screenPoint.Y < 0 ? 0 : screenPoint.Y;
            return new Point(screenPoint.X / charSizePix.Width, screenPoint.Y / charSizePix.Height);
        }

        public Point ToScreenPoint(OSDItem item, int xOffset)
        {
            return new Point(((int)item.X.Value - xOffset) * charSizePix.Width, (int)item.Y.Value * charSizePix.Height);
        }

        private Rectangle ToScreenRectangle(OSDItem item)
        {
            var size = new Size(captionProvider.GetItemCaption(item, out int xOffset).Length * charSizePix.Width, charSizePix.Height);
            var loc = ToScreenPoint(item, xOffset);
            return new Rectangle(loc, size);
        }

        public void Draw(OSDItem item, Graphics graphics)
        {
            var caption = captionProvider.GetItemCaption(item, out int xOffset); // ItemCaptions.GetCaption(item, out int xOffset);
            Point loc = ToScreenPoint(item, xOffset);
            
            foreach (var c in caption)
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

        internal void DrawSDBackground(Graphics g)
        {
            g.DrawLine(Pens.DimGray, 0, charSizePix.Height * NtscRows, charSizePix.Width * screenSizeChar.Width, charSizePix.Height * NtscRows);
            g.DrawString("NTSC", sdNoteFont, Brushes.Black, sdNoteFont.Size, charSizePix.Height * NtscRows - sdNoteFont.Size * 2);
            g.DrawString("PAL", sdNoteFont, Brushes.Black, sdNoteFont.Size, charSizePix.Height * NtscRows + sdNoteFont.Size * 1.5f);
        }
        internal void DrawHDBackground(Graphics g)
        {
            g.DrawLine(Pens.DimGray, 0, charSizePix.Height * DJIRows, charSizePix.Width * DJICols, charSizePix.Height * DJIRows);
            g.DrawLine(Pens.DimGray, charSizePix.Width * DJICols, charSizePix.Height * DJIRows, charSizePix.Width * DJICols, 0);
            g.DrawString("50x18", hdNoteFont, Brushes.Black, charSizePix.Width * DJICols - 4.5f * hdNoteFont.Size, charSizePix.Height * DJIRows - hdNoteFont.Size * 2);
            g.DrawString("60x22", hdNoteFont, Brushes.Black, charSizePix.Width * screenSizeChar.Width - 4.5f * hdNoteFont.Size, charSizePix.Height * screenSizeChar.Height - hdNoteFont.Size * 2);
        }
    }
}
