using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using MissionPlanner.Utilities;
using System.IO;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    class fontgen
    {
        public static int width = 8;
        public static int height = 16;

        private static Font FindBestFitFont(Graphics g, String text, Font font, Size proposedSize, TextFormatFlags flags)
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
                font = new Font(font.Name, (float) (font.Size*.9), font.Style);
                oldFont.Dispose();
            }
        }

        public static void dowork()
        {
            char letter = '0';

            StreamWriter file = new StreamWriter(File.Open("fonts.txt", FileMode.Create));

            //var flags = TextFormatFlags.Default;// TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;// |  TextFormatFlags.GlyphOverhangPadding;

            for (; letter <= 'Z'; letter++)
            {
                Font font = new Font("Arial Narrow", 7, FontStyle.Regular);

                Bitmap bmp = new Bitmap(width, height);

                Graphics g = Graphics.FromImage(bmp);

                g.Clear(Color.White);

                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;

                //font = FindBestFitFont(g, letter.ToString(), font, new Size(width,height), flags);

                var fsize = g.MeasureString(letter.ToString(), font);

                g.DrawString(letter.ToString(), font, Brushes.Black, 0, 0);

                try
                {
                    // bmp = new Bitmap(bmp, new Size(8, 16));
                    if (letter >= ':' && letter <= '@')
                        continue;

                    bmp.Save("!" + letter + ".bmp");

                    string outlet = letter.ToString().ToLower();

                    switch (letter)
                    {
                        case '0':
                            outlet = "ze";
                            break;
                        case '1':
                            outlet = "on";
                            break;
                        case '2':
                            outlet = "tw";
                            break;
                        case '3':
                            outlet = "th";
                            break;
                        case '4':
                            outlet = "fo";
                            break;
                        case '5':
                            outlet = "fi";
                            break;
                        case '6':
                            outlet = "si";
                            break;
                        case '7':
                            outlet = "se";
                            break;
                        case '8':
                            outlet = "ei";
                            break;
                        case '9':
                            outlet = "ni";
                            break;
                    }

                    file.Write("byte " + outlet + "[16] = {");

                    for (int h = 0; h < 16; h++)
                    {
                        byte chr = 0x0;

                        for (int w = 0; w < 8; w++)
                        {
                            var pix = bmp.GetPixel(w, h);
                            if (pix.R == 255 && pix.G == 255 && pix.B == 255)
                            {
                                chr += (byte) (1 << w);
                            }
                            else
                            {
                            }
                        }

                        file.Write(String.Format("0x{0:X},", chr));
                    }

                    file.WriteLine("};");
                }
                catch
                {
                }
            }

            file.Close();
        }
    }
}