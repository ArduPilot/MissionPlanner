using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using OpenTK.Graphics.OpenGL;

namespace ArdupilotMega.Widgets
{
    public class GraphicsGL
    {
        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        public static void DrawArc(Pen penn, RectangleF rect, float start, float degrees)
        {
            GL.LineWidth(penn.Width);
            GL.Color4(penn.Color);

            GL.Begin(BeginMode.LineStrip);

            start = 360 - start;
            start -= 30;

            float x = 0, y = 0;
            for (float i = start; i <= start + degrees; i++)
            {
                x = (float)Math.Sin(i * deg2rad) * rect.Width / 2;
                y = (float)Math.Cos(i * deg2rad) * rect.Height / 2;
                x = x + rect.X + rect.Width / 2;
                y = y + rect.Y + rect.Height / 2;
                GL.Vertex2(x, y);
            }
            GL.End();
        }

        public static void DrawEllipse(Pen penn, Rectangle rect)
        {
            GL.LineWidth(penn.Width);
            GL.Color4(penn.Color);

            GL.Begin(BeginMode.LineLoop);
            float x, y;
            for (float i = 0; i < 360; i += 1)
            {
                x = (float)Math.Sin(i * deg2rad) * rect.Width / 2;
                y = (float)Math.Cos(i * deg2rad) * rect.Height / 2;
                x = x + rect.X + rect.Width / 2;
                y = y + rect.Y + rect.Height / 2;
                GL.Vertex2(x, y);
            }
            GL.End();
        }


        public static void DrawImage(Image img, int x, int y, int width, int height)
        {
            /*
            if (img == null)
                return;
            bitmap = new Bitmap(512, 512);

            using (Graphics graphics = Graphics.FromImage(bitmap))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                //draw the image into the target bitmap 
                graphics.DrawImage(img, 0, 0, bitmap.Width, bitmap.Height);
            }


            GL.DeleteTexture(texture);

            GL.GenTextures(1, out texture);
            GL.BindTexture(TextureTarget.Texture2D, texture);

            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            //Console.WriteLine("w {0} h {1}",data.Width, data.Height);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);

            bitmap.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, texture);

            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(0, this.Height);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(this.Width, this.Height);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(this.Width, 0);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(0, 0);

            GL.End();

            GL.Disable(EnableCap.Texture2D);
            */
        }


        public static void DrawLine(Pen penn, double x1, double y1, double x2, double y2)
        {
            GL.Color4(penn.Color);
            GL.LineWidth(penn.Width);

            GL.Begin(BeginMode.Lines);
            GL.Vertex2(x1, y1);
            GL.Vertex2(x2, y2);
            GL.End();
        }


        public static void DrawPath(Pen penn, GraphicsPath gp)
        {
            try
            {
                DrawPolygon(penn, gp.PathPoints);
            }
            catch { }
        }

        public static void FillPath(Brush brushh, GraphicsPath gp)
        {
            try
            {
                FillPolygon(brushh, gp.PathPoints);
            }
            catch { }
        }

        public static void FillPolygon(Brush brushh, Point[] list)
        {
            GL.Begin(BeginMode.TriangleFan);
            GL.Color4(((SolidBrush)brushh).Color);
            foreach (Point pnt in list)
            {
                GL.Vertex2(pnt.X, pnt.Y);
            }
            GL.Vertex2(list[list.Length - 1].X, list[list.Length - 1].Y);
            GL.End();
        }

        public static void FillPolygon(Brush brushh, PointF[] list)
        {
            GL.Begin(BeginMode.Quads);
            GL.Color4(((SolidBrush)brushh).Color);
            foreach (PointF pnt in list)
            {
                GL.Vertex2(pnt.X, pnt.Y);
            }
            GL.Vertex2(list[0].X, list[0].Y);
            GL.End();
        }

        public static void FillRectangle(Brush brushh, RectangleF rectf)
        {
            float x1 = rectf.X;
            float y1 = rectf.Y;

            float width = rectf.Width;
            float height = rectf.Height;

            GL.Begin(BeginMode.Quads);

            if (((Type)brushh.GetType()) == typeof(LinearGradientBrush))
            {
                LinearGradientBrush temp = (LinearGradientBrush)brushh;
                GL.Color4(temp.LinearColors[0]);
            }
            else
            {
                GL.Color4(((SolidBrush)brushh).Color.R / 255f, ((SolidBrush)brushh).Color.G / 255f, ((SolidBrush)brushh).Color.B / 255f, ((SolidBrush)brushh).Color.A / 255f);
            }

            GL.Vertex2(x1, y1);
            GL.Vertex2(x1 + width, y1);

            if (((Type)brushh.GetType()) == typeof(LinearGradientBrush))
            {
                LinearGradientBrush temp = (LinearGradientBrush)brushh;
                GL.Color4(temp.LinearColors[1]);
            }
            else
            {
                GL.Color4(((SolidBrush)brushh).Color.R / 255f, ((SolidBrush)brushh).Color.G / 255f, ((SolidBrush)brushh).Color.B / 255f, ((SolidBrush)brushh).Color.A / 255f);
            }

            GL.Vertex2(x1 + width, y1 + height);
            GL.Vertex2(x1, y1 + height);
            GL.End();
        }

        public static void DrawPolygon(Pen penn, Point[] list)
        {
            GL.LineWidth(penn.Width);
            GL.Color4(penn.Color);

            GL.Begin(BeginMode.LineLoop);
            foreach (Point pnt in list)
            {
                GL.Vertex2(pnt.X, pnt.Y);
            }
            GL.End();
        }

        public static void DrawPolygon(Pen penn, PointF[] list)
        {
            GL.LineWidth(penn.Width);
            GL.Color4(penn.Color);

            GL.Begin(BeginMode.LineLoop);
            foreach (PointF pnt in list)
            {
                GL.Vertex2(pnt.X, pnt.Y);
            }

            GL.End();
        }

        public static void DrawRectangle(Pen penn, double x1, double y1, double width, double height)
        {
            GL.LineWidth(penn.Width);
            GL.Color4(penn.Color);

            GL.Begin(BeginMode.LineLoop);
            GL.Vertex2(x1, y1);
            GL.Vertex2(x1 + width, y1);
            GL.Vertex2(x1 + width, y1 + height);
            GL.Vertex2(x1, y1 + height);
            GL.End();
        }

        public static void DrawRectangle(Pen penn, RectangleF rect)
        {
            DrawRectangle(penn, rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static void Clear(Color color)
        {
            GL.ClearColor(color);
        }

        public static void ResetTransform()
        {
            GL.LoadIdentity();
        }

        public static void RotateTransform(float angle)
        {
            GL.Rotate(angle, 0, 0, 1);
        }

        public static void TranslateTransform(float x, float y)
        {
            GL.Translate(x, y, 0f);
        }
    }
}