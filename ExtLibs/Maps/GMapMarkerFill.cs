using GMap.NET;
using GMap.NET.WindowsForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace MissionPlanner.Maps
{
    public class GMapMarkerFill : GMapMarker
    {
        Bitmap elevation;

        private RectLatLng rect;

        public GMapMarkerFill(byte [,] imageData, RectLatLng rect, PointLatLng currentloc)
        : base(currentloc)
        {
            this.rect = rect;

            IsHitTestVisible = false;

            //create a new Bitmap
            Bitmap bmp = new Bitmap(imageData.GetLength(0), imageData.GetLength(1), PixelFormat.Format32bppArgb);
            
            //lock it to get the BitmapData Object
            BitmapData bmData =
                bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            //now we have to convert the 2 dimensional array into a one dimensional byte-array for use with 8bpp bitmaps
            // use stride and height to prevent stride mod 4 issues
            int[] pixels = new int[(bmData.Stride/4) * bmData.Height];
            for (int y = 0; y < imageData.GetLength(1); y++)
            {
                for (int x = 0; x < imageData.GetLength(0); x++)
                {
                    pixels[(y * (bmData.Stride/4) + x)] = ConvertColor(imageData[x, y]);
                }
            }

            //copy the bytes
            System.Runtime.InteropServices.Marshal.Copy(pixels, 0, bmData.Scan0, (bmData.Stride/4) * bmData.Height);            

            //never forget to unlock the bitmap
            bmp.UnlockBits(bmData);

            bmp.MakeTransparent();

            //display
            elevation = bmp;
        }

        static GMapMarkerFill()
        {
            var bmp = new Bitmap(1, 1, PixelFormat.Format8bppIndexed);
            pal = bmp.Palette;
            // viridis
            pal.Entries[0] = Color.FromArgb(255, 68, 1, 84);
            pal.Entries[1] = Color.FromArgb(255, 68, 2, 85);
            pal.Entries[2] = Color.FromArgb(255, 68, 3, 87);
            pal.Entries[3] = Color.FromArgb(255, 69, 5, 88);
            pal.Entries[4] = Color.FromArgb(255, 69, 6, 90);
            pal.Entries[5] = Color.FromArgb(255, 69, 8, 91);
            pal.Entries[6] = Color.FromArgb(255, 70, 9, 92);
            pal.Entries[7] = Color.FromArgb(255, 70, 11, 94);
            pal.Entries[8] = Color.FromArgb(255, 70, 12, 95);
            pal.Entries[9] = Color.FromArgb(255, 70, 14, 97);
            pal.Entries[10] = Color.FromArgb(255, 71, 15, 98);
            pal.Entries[11] = Color.FromArgb(255, 71, 17, 99);
            pal.Entries[12] = Color.FromArgb(255, 71, 18, 101);
            pal.Entries[13] = Color.FromArgb(255, 71, 20, 102);
            pal.Entries[14] = Color.FromArgb(255, 71, 21, 103);
            pal.Entries[15] = Color.FromArgb(255, 71, 22, 105);
            pal.Entries[16] = Color.FromArgb(255, 71, 24, 106);
            pal.Entries[17] = Color.FromArgb(255, 72, 25, 107);
            pal.Entries[18] = Color.FromArgb(255, 72, 26, 108);
            pal.Entries[19] = Color.FromArgb(255, 72, 28, 110);
            pal.Entries[20] = Color.FromArgb(255, 72, 29, 111);
            pal.Entries[21] = Color.FromArgb(255, 72, 30, 112);
            pal.Entries[22] = Color.FromArgb(255, 72, 32, 113);
            pal.Entries[23] = Color.FromArgb(255, 72, 33, 114);
            pal.Entries[24] = Color.FromArgb(255, 72, 34, 115);
            pal.Entries[25] = Color.FromArgb(255, 72, 35, 116);
            pal.Entries[26] = Color.FromArgb(255, 71, 37, 117);
            pal.Entries[27] = Color.FromArgb(255, 71, 38, 118);
            pal.Entries[28] = Color.FromArgb(255, 71, 39, 119);
            pal.Entries[29] = Color.FromArgb(255, 71, 40, 120);
            pal.Entries[30] = Color.FromArgb(255, 71, 42, 121);
            pal.Entries[31] = Color.FromArgb(255, 71, 43, 122);
            pal.Entries[32] = Color.FromArgb(255, 71, 44, 123);
            pal.Entries[33] = Color.FromArgb(255, 70, 45, 124);
            pal.Entries[34] = Color.FromArgb(255, 70, 47, 124);
            pal.Entries[35] = Color.FromArgb(255, 70, 48, 125);
            pal.Entries[36] = Color.FromArgb(255, 70, 49, 126);
            pal.Entries[37] = Color.FromArgb(255, 69, 50, 127);
            pal.Entries[38] = Color.FromArgb(255, 69, 52, 127);
            pal.Entries[39] = Color.FromArgb(255, 69, 53, 128);
            pal.Entries[40] = Color.FromArgb(255, 69, 54, 129);
            pal.Entries[41] = Color.FromArgb(255, 68, 55, 129);
            pal.Entries[42] = Color.FromArgb(255, 68, 57, 130);
            pal.Entries[43] = Color.FromArgb(255, 67, 58, 131);
            pal.Entries[44] = Color.FromArgb(255, 67, 59, 131);
            pal.Entries[45] = Color.FromArgb(255, 67, 60, 132);
            pal.Entries[46] = Color.FromArgb(255, 66, 61, 132);
            pal.Entries[47] = Color.FromArgb(255, 66, 62, 133);
            pal.Entries[48] = Color.FromArgb(255, 66, 64, 133);
            pal.Entries[49] = Color.FromArgb(255, 65, 65, 134);
            pal.Entries[50] = Color.FromArgb(255, 65, 66, 134);
            pal.Entries[51] = Color.FromArgb(255, 64, 67, 135);
            pal.Entries[52] = Color.FromArgb(255, 64, 68, 135);
            pal.Entries[53] = Color.FromArgb(255, 63, 69, 135);
            pal.Entries[54] = Color.FromArgb(255, 63, 71, 136);
            pal.Entries[55] = Color.FromArgb(255, 62, 72, 136);
            pal.Entries[56] = Color.FromArgb(255, 62, 73, 137);
            pal.Entries[57] = Color.FromArgb(255, 61, 74, 137);
            pal.Entries[58] = Color.FromArgb(255, 61, 75, 137);
            pal.Entries[59] = Color.FromArgb(255, 61, 76, 137);
            pal.Entries[60] = Color.FromArgb(255, 60, 77, 138);
            pal.Entries[61] = Color.FromArgb(255, 60, 78, 138);
            pal.Entries[62] = Color.FromArgb(255, 59, 80, 138);
            pal.Entries[63] = Color.FromArgb(255, 59, 81, 138);
            pal.Entries[64] = Color.FromArgb(255, 58, 82, 139);
            pal.Entries[65] = Color.FromArgb(255, 58, 83, 139);
            pal.Entries[66] = Color.FromArgb(255, 57, 84, 139);
            pal.Entries[67] = Color.FromArgb(255, 57, 85, 139);
            pal.Entries[68] = Color.FromArgb(255, 56, 86, 139);
            pal.Entries[69] = Color.FromArgb(255, 56, 87, 140);
            pal.Entries[70] = Color.FromArgb(255, 55, 88, 140);
            pal.Entries[71] = Color.FromArgb(255, 55, 89, 140);
            pal.Entries[72] = Color.FromArgb(255, 54, 90, 140);
            pal.Entries[73] = Color.FromArgb(255, 54, 91, 140);
            pal.Entries[74] = Color.FromArgb(255, 53, 92, 140);
            pal.Entries[75] = Color.FromArgb(255, 53, 93, 140);
            pal.Entries[76] = Color.FromArgb(255, 52, 94, 141);
            pal.Entries[77] = Color.FromArgb(255, 52, 95, 141);
            pal.Entries[78] = Color.FromArgb(255, 51, 96, 141);
            pal.Entries[79] = Color.FromArgb(255, 51, 97, 141);
            pal.Entries[80] = Color.FromArgb(255, 50, 98, 141);
            pal.Entries[81] = Color.FromArgb(255, 50, 99, 141);
            pal.Entries[82] = Color.FromArgb(255, 49, 100, 141);
            pal.Entries[83] = Color.FromArgb(255, 49, 101, 141);
            pal.Entries[84] = Color.FromArgb(255, 49, 102, 141);
            pal.Entries[85] = Color.FromArgb(255, 48, 103, 141);
            pal.Entries[86] = Color.FromArgb(255, 48, 104, 141);
            pal.Entries[87] = Color.FromArgb(255, 47, 105, 141);
            pal.Entries[88] = Color.FromArgb(255, 47, 106, 141);
            pal.Entries[89] = Color.FromArgb(255, 46, 107, 142);
            pal.Entries[90] = Color.FromArgb(255, 46, 108, 142);
            pal.Entries[91] = Color.FromArgb(255, 46, 109, 142);
            pal.Entries[92] = Color.FromArgb(255, 45, 110, 142);
            pal.Entries[93] = Color.FromArgb(255, 45, 111, 142);
            pal.Entries[94] = Color.FromArgb(255, 44, 112, 142);
            pal.Entries[95] = Color.FromArgb(255, 44, 113, 142);
            pal.Entries[96] = Color.FromArgb(255, 44, 114, 142);
            pal.Entries[97] = Color.FromArgb(255, 43, 115, 142);
            pal.Entries[98] = Color.FromArgb(255, 43, 116, 142);
            pal.Entries[99] = Color.FromArgb(255, 42, 117, 142);
            pal.Entries[100] = Color.FromArgb(255, 42, 118, 142);
            pal.Entries[101] = Color.FromArgb(255, 42, 119, 142);
            pal.Entries[102] = Color.FromArgb(255, 41, 120, 142);
            pal.Entries[103] = Color.FromArgb(255, 41, 121, 142);
            pal.Entries[104] = Color.FromArgb(255, 40, 122, 142);
            pal.Entries[105] = Color.FromArgb(255, 40, 122, 142);
            pal.Entries[106] = Color.FromArgb(255, 40, 123, 142);
            pal.Entries[107] = Color.FromArgb(255, 39, 124, 142);
            pal.Entries[108] = Color.FromArgb(255, 39, 125, 142);
            pal.Entries[109] = Color.FromArgb(255, 39, 126, 142);
            pal.Entries[110] = Color.FromArgb(255, 38, 127, 142);
            pal.Entries[111] = Color.FromArgb(255, 38, 128, 142);
            pal.Entries[112] = Color.FromArgb(255, 38, 129, 142);
            pal.Entries[113] = Color.FromArgb(255, 37, 130, 142);
            pal.Entries[114] = Color.FromArgb(255, 37, 131, 141);
            pal.Entries[115] = Color.FromArgb(255, 36, 132, 141);
            pal.Entries[116] = Color.FromArgb(255, 36, 133, 141);
            pal.Entries[117] = Color.FromArgb(255, 36, 134, 141);
            pal.Entries[118] = Color.FromArgb(255, 35, 135, 141);
            pal.Entries[119] = Color.FromArgb(255, 35, 136, 141);
            pal.Entries[120] = Color.FromArgb(255, 35, 137, 141);
            pal.Entries[121] = Color.FromArgb(255, 34, 137, 141);
            pal.Entries[122] = Color.FromArgb(255, 34, 138, 141);
            pal.Entries[123] = Color.FromArgb(255, 34, 139, 141);
            pal.Entries[124] = Color.FromArgb(255, 33, 140, 141);
            pal.Entries[125] = Color.FromArgb(255, 33, 141, 140);
            pal.Entries[126] = Color.FromArgb(255, 33, 142, 140);
            pal.Entries[127] = Color.FromArgb(255, 32, 143, 140);
            pal.Entries[128] = Color.FromArgb(255, 32, 144, 140);
            pal.Entries[129] = Color.FromArgb(255, 32, 145, 140);
            pal.Entries[130] = Color.FromArgb(255, 31, 146, 140);
            pal.Entries[131] = Color.FromArgb(255, 31, 147, 139);
            pal.Entries[132] = Color.FromArgb(255, 31, 148, 139);
            pal.Entries[133] = Color.FromArgb(255, 31, 149, 139);
            pal.Entries[134] = Color.FromArgb(255, 31, 150, 139);
            pal.Entries[135] = Color.FromArgb(255, 30, 151, 138);
            pal.Entries[136] = Color.FromArgb(255, 30, 152, 138);
            pal.Entries[137] = Color.FromArgb(255, 30, 153, 138);
            pal.Entries[138] = Color.FromArgb(255, 30, 153, 138);
            pal.Entries[139] = Color.FromArgb(255, 30, 154, 137);
            pal.Entries[140] = Color.FromArgb(255, 30, 155, 137);
            pal.Entries[141] = Color.FromArgb(255, 30, 156, 137);
            pal.Entries[142] = Color.FromArgb(255, 30, 157, 136);
            pal.Entries[143] = Color.FromArgb(255, 30, 158, 136);
            pal.Entries[144] = Color.FromArgb(255, 30, 159, 136);
            pal.Entries[145] = Color.FromArgb(255, 30, 160, 135);
            pal.Entries[146] = Color.FromArgb(255, 31, 161, 135);
            pal.Entries[147] = Color.FromArgb(255, 31, 162, 134);
            pal.Entries[148] = Color.FromArgb(255, 31, 163, 134);
            pal.Entries[149] = Color.FromArgb(255, 32, 164, 133);
            pal.Entries[150] = Color.FromArgb(255, 32, 165, 133);
            pal.Entries[151] = Color.FromArgb(255, 33, 166, 133);
            pal.Entries[152] = Color.FromArgb(255, 33, 167, 132);
            pal.Entries[153] = Color.FromArgb(255, 34, 167, 132);
            pal.Entries[154] = Color.FromArgb(255, 35, 168, 131);
            pal.Entries[155] = Color.FromArgb(255, 35, 169, 130);
            pal.Entries[156] = Color.FromArgb(255, 36, 170, 130);
            pal.Entries[157] = Color.FromArgb(255, 37, 171, 129);
            pal.Entries[158] = Color.FromArgb(255, 38, 172, 129);
            pal.Entries[159] = Color.FromArgb(255, 39, 173, 128);
            pal.Entries[160] = Color.FromArgb(255, 40, 174, 127);
            pal.Entries[161] = Color.FromArgb(255, 41, 175, 127);
            pal.Entries[162] = Color.FromArgb(255, 42, 176, 126);
            pal.Entries[163] = Color.FromArgb(255, 43, 177, 125);
            pal.Entries[164] = Color.FromArgb(255, 44, 177, 125);
            pal.Entries[165] = Color.FromArgb(255, 46, 178, 124);
            pal.Entries[166] = Color.FromArgb(255, 47, 179, 123);
            pal.Entries[167] = Color.FromArgb(255, 48, 180, 122);
            pal.Entries[168] = Color.FromArgb(255, 50, 181, 122);
            pal.Entries[169] = Color.FromArgb(255, 51, 182, 121);
            pal.Entries[170] = Color.FromArgb(255, 53, 183, 120);
            pal.Entries[171] = Color.FromArgb(255, 54, 184, 119);
            pal.Entries[172] = Color.FromArgb(255, 56, 185, 118);
            pal.Entries[173] = Color.FromArgb(255, 57, 185, 118);
            pal.Entries[174] = Color.FromArgb(255, 59, 186, 117);
            pal.Entries[175] = Color.FromArgb(255, 61, 187, 116);
            pal.Entries[176] = Color.FromArgb(255, 62, 188, 115);
            pal.Entries[177] = Color.FromArgb(255, 64, 189, 114);
            pal.Entries[178] = Color.FromArgb(255, 66, 190, 113);
            pal.Entries[179] = Color.FromArgb(255, 68, 190, 112);
            pal.Entries[180] = Color.FromArgb(255, 69, 191, 111);
            pal.Entries[181] = Color.FromArgb(255, 71, 192, 110);
            pal.Entries[182] = Color.FromArgb(255, 73, 193, 109);
            pal.Entries[183] = Color.FromArgb(255, 75, 194, 108);
            pal.Entries[184] = Color.FromArgb(255, 77, 194, 107);
            pal.Entries[185] = Color.FromArgb(255, 79, 195, 105);
            pal.Entries[186] = Color.FromArgb(255, 81, 196, 104);
            pal.Entries[187] = Color.FromArgb(255, 83, 197, 103);
            pal.Entries[188] = Color.FromArgb(255, 85, 198, 102);
            pal.Entries[189] = Color.FromArgb(255, 87, 198, 101);
            pal.Entries[190] = Color.FromArgb(255, 89, 199, 100);
            pal.Entries[191] = Color.FromArgb(255, 91, 200, 98);
            pal.Entries[192] = Color.FromArgb(255, 94, 201, 97);
            pal.Entries[193] = Color.FromArgb(255, 96, 201, 96);
            pal.Entries[194] = Color.FromArgb(255, 98, 202, 95);
            pal.Entries[195] = Color.FromArgb(255, 100, 203, 93);
            pal.Entries[196] = Color.FromArgb(255, 103, 204, 92);
            pal.Entries[197] = Color.FromArgb(255, 105, 204, 91);
            pal.Entries[198] = Color.FromArgb(255, 107, 205, 89);
            pal.Entries[199] = Color.FromArgb(255, 109, 206, 88);
            pal.Entries[200] = Color.FromArgb(255, 112, 206, 86);
            pal.Entries[201] = Color.FromArgb(255, 114, 207, 85);
            pal.Entries[202] = Color.FromArgb(255, 116, 208, 84);
            pal.Entries[203] = Color.FromArgb(255, 119, 208, 82);
            pal.Entries[204] = Color.FromArgb(255, 121, 209, 81);
            pal.Entries[205] = Color.FromArgb(255, 124, 210, 79);
            pal.Entries[206] = Color.FromArgb(255, 126, 210, 78);
            pal.Entries[207] = Color.FromArgb(255, 129, 211, 76);
            pal.Entries[208] = Color.FromArgb(255, 131, 211, 75);
            pal.Entries[209] = Color.FromArgb(255, 134, 212, 73);
            pal.Entries[210] = Color.FromArgb(255, 136, 213, 71);
            pal.Entries[211] = Color.FromArgb(255, 139, 213, 70);
            pal.Entries[212] = Color.FromArgb(255, 141, 214, 68);
            pal.Entries[213] = Color.FromArgb(255, 144, 214, 67);
            pal.Entries[214] = Color.FromArgb(255, 146, 215, 65);
            pal.Entries[215] = Color.FromArgb(255, 149, 215, 63);
            pal.Entries[216] = Color.FromArgb(255, 151, 216, 62);
            pal.Entries[217] = Color.FromArgb(255, 154, 216, 60);
            pal.Entries[218] = Color.FromArgb(255, 157, 217, 58);
            pal.Entries[219] = Color.FromArgb(255, 159, 217, 56);
            pal.Entries[220] = Color.FromArgb(255, 162, 218, 55);
            pal.Entries[221] = Color.FromArgb(255, 165, 218, 53);
            pal.Entries[222] = Color.FromArgb(255, 167, 219, 51);
            pal.Entries[223] = Color.FromArgb(255, 170, 219, 50);
            pal.Entries[224] = Color.FromArgb(255, 173, 220, 48);
            pal.Entries[225] = Color.FromArgb(255, 175, 220, 46);
            pal.Entries[226] = Color.FromArgb(255, 178, 221, 44);
            pal.Entries[227] = Color.FromArgb(255, 181, 221, 43);
            pal.Entries[228] = Color.FromArgb(255, 183, 221, 41);
            pal.Entries[229] = Color.FromArgb(255, 186, 222, 39);
            pal.Entries[230] = Color.FromArgb(255, 189, 222, 38);
            pal.Entries[231] = Color.FromArgb(255, 191, 223, 36);
            pal.Entries[232] = Color.FromArgb(255, 194, 223, 34);
            pal.Entries[233] = Color.FromArgb(255, 197, 223, 33);
            pal.Entries[234] = Color.FromArgb(255, 199, 224, 31);
            pal.Entries[235] = Color.FromArgb(255, 202, 224, 30);
            pal.Entries[236] = Color.FromArgb(255, 205, 224, 29);
            pal.Entries[237] = Color.FromArgb(255, 207, 225, 28);
            pal.Entries[238] = Color.FromArgb(255, 210, 225, 27);
            pal.Entries[239] = Color.FromArgb(255, 212, 225, 26);
            pal.Entries[240] = Color.FromArgb(255, 215, 226, 25);
            pal.Entries[241] = Color.FromArgb(255, 218, 226, 24);
            pal.Entries[242] = Color.FromArgb(255, 220, 226, 24);
            pal.Entries[243] = Color.FromArgb(255, 223, 227, 24);
            pal.Entries[244] = Color.FromArgb(255, 225, 227, 24);
            pal.Entries[245] = Color.FromArgb(255, 228, 227, 24);
            pal.Entries[246] = Color.FromArgb(255, 231, 228, 25);
            pal.Entries[247] = Color.FromArgb(255, 233, 228, 25);
            pal.Entries[248] = Color.FromArgb(255, 236, 228, 26);
            pal.Entries[249] = Color.FromArgb(255, 238, 229, 27);
            pal.Entries[250] = Color.FromArgb(255, 241, 229, 28);
            pal.Entries[251] = Color.FromArgb(255, 243, 229, 30);
            pal.Entries[252] = Color.FromArgb(255, 246, 230, 31);
            pal.Entries[253] = Color.FromArgb(255, 248, 230, 33);
            pal.Entries[254] = Color.FromArgb(255, 250, 230, 34);

            transparent = Color.Transparent.ToArgb();
        }

        private static ColorPalette pal;

        private static int transparent;

        int ConvertColor(byte incol)
        {
            if (incol == 0 || incol == 255)
                return transparent;

            return (int)(pal.Entries[incol].ToArgb() & 0xc8ffffff);
        }

        public override void OnRender(IGraphics g)
        {
            base.OnRender(g);

            var tlll = Overlay.Control.FromLatLngToLocal(rect.LocationTopLeft);
            var brll = Overlay.Control.FromLatLngToLocal(rect.LocationRightBottom);

            var old = g.Transform;

            g.ResetTransform();

            // maintain transperancy
            g.CompositingMode = CompositingMode.SourceOver;

            g.DrawImage(elevation, tlll.X, tlll.Y, brll.X - tlll.X, brll.Y - tlll.Y);

            g.Transform = old;
        }
    }
}
