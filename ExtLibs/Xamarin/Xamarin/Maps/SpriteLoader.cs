using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using GMap.NET.Drawing;
using GMap.NET.WindowsForms;

namespace MissionPlanner.Maps
{
    public class SpriteLoader
    {
        public static Image[,] Boats
        {
            get { return GetBitmap(Maps.Resources.boatsprite.ToBitmap(), 8, 14); }
        }
        public static Image[,] GetBitmap(Image source, int x, int y)
        {
            var sprint = new Image[x, y];

            var tilewidth = source.Width / x;
            var tileheight = source.Height / y;

            for (int yc = 0; yc < y; yc++)
            {
                for (int xc = 0; xc < x; xc++)
                {
                    sprint[xc,yc] = new Bitmap(tilewidth,tileheight);
                    var g = Graphics.FromImage(sprint[xc, yc]);
                    g.DrawImage(source, new Rectangle(0, 0, tilewidth, tileheight), tilewidth * xc, tileheight * yc,
                        tilewidth, tileheight, GraphicsUnit.Pixel);
                    g.Dispose();
                }
            }

            return sprint;
        }
    }
}
