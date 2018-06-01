
using System;
using OpenTK.Graphics.OpenGL;

namespace AssortedWidgets.Graphics
{
    public class SubImage
    {
        float upLeftX;
        float upLeftY;
        float bottomRightX;
        float bottomRightY;
        uint textureID;

        public SubImage(float upLeftX, float upLeftY, float bottomRightX, float bottomRightY, uint textureID)
        {
            this.upLeftX = upLeftX;
            this.upLeftY = upLeftY;
            this.bottomRightX = bottomRightX;
            this.bottomRightY = bottomRightY;
            this.textureID = textureID;
        }
        public void Paint(float x1, float y1, float x2, float y2)
        {
            //GL.Color3(1, 1, 1);
            GL.BindTexture(OpenTK.Graphics.OpenGL.TextureTarget.Texture2D, textureID);
            GL.Begin(BeginMode.Quads);
            GL.TexCoord2(upLeftX, upLeftY);
            GL.Vertex2(x1, y1);
            GL.TexCoord2(upLeftX, bottomRightY);
            GL.Vertex2(x1, y2);
            GL.TexCoord2(bottomRightX, bottomRightY);
            GL.Vertex2(x2, y2);
            GL.TexCoord2(bottomRightX, upLeftY);
            GL.Vertex2(x2, y1);
            GL.End();
        }
    }
}
