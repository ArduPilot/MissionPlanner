using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

using AssortedWidgets.Widgets;
using AssortedWidgets;

using OpenTK.Graphics.OpenGL;

namespace ArdupilotMega.Widgets
{
    public class PictureBox : Component
    {
        Bitmap _img = new Bitmap(1,1);
        public Bitmap Image { get { return _img; } set { _img = value; tex = new AssortedWidgets.Graphics.Texture("pb"); tex.FromBitmap(_img); } }

        AssortedWidgets.Graphics.Texture tex;

        public PictureBox()
        {
            Size = new AssortedWidgets.Util.Size(100, 100);
            Position = new AssortedWidgets.Util.Position(10,10);
        }

        public override void Paint()
        {
            if (tex == null)
                return;

            AssortedWidgets.Util.Position origin = UI.Instance.GetOrigin();

            int x = origin.X + Position.X;
            int y = origin.Y + Position.Y;

            GL.Enable(EnableCap.Texture2D);

            GL.BindTexture(TextureTarget.Texture2D, tex.TextureId);

            GL.Begin(BeginMode.Quads);

            GL.TexCoord2(0.0f, 1.0f); GL.Vertex2(0 + x, this.Size.width + y);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex2(this.Size.width + x, this.Size.height + y);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex2(this.Size.width + x, 0 + y);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex2(0 + x, 0 + y);

            GL.End();

            GL.Disable(EnableCap.Texture2D);

            GL.Begin(BeginMode.LineLoop);
            GL.Color3(Color.Gray);
            GL.Vertex2(0 + x, this.Size.width + y);
            GL.Vertex2(this.Size.width + x, this.Size.height + y);
            GL.Vertex2(this.Size.width + x, 0 + y);
            GL.Vertex2(0 + x, 0 + y);
            GL.End();
        }
    }
}