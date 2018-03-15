using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using System.IO;

namespace MissionPlanner.Utilities
{
    public class fontgen
    {
        public static int width = 8;
        public static int height = 16;

        private static Font FindBestFitFont(Graphics g, String text, Font font, Size proposedSize)
        {
            // Compute actual size, shrink if needed
            while (true)
            {
                SizeF size = g.MeasureString(text, font);

                // It fits, back out
                if (size.Height <= proposedSize.Height &&
                    size.Width <= proposedSize.Width)
                {
                    return font;
                }

                // Try a smaller font (90% of old size)
                Font oldFont = font;
                font = new Font(font.Name, (float) (font.Size * .9), font.Style);
                oldFont.Dispose();
            }
        }

        public static void dowork()
        {
            char letter = (char) 0;

            Font font = new Font("Arial Narrow", 40, FontStyle.Regular);

            //font = FindBestFitFont(g, letter.ToString(), font, new Size(width,height), flags);

            var fsize = Graphics.FromImage(new Bitmap(1, 1)).MeasureString("@", font);

            width = (int) Math.Ceiling(fsize.Width);
            height = (int) Math.Ceiling(fsize.Height);

            Bitmap bmp = new Bitmap(width * 16, height * 16);

            bmp.MakeTransparent(Color.BlueViolet);

            Graphics g = Graphics.FromImage(bmp);

            //g.Clear(Color.BlueViolet);

            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;


            //var flags = TextFormatFlags.Default;// TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;// |  TextFormatFlags.GlyphOverhangPadding;

            for (; letter <= (char) 255; letter++)
            {
                var wl = (letter % 16);
                var hl = (letter - wl) / 16;

                g.DrawString(letter.ToString(), font, Brushes.Black,
                    new RectangleF(wl * width, hl * height, width, height),
                    new StringFormat() {Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center});



                bmp.Save("font.png");

            }
        }
    }
}