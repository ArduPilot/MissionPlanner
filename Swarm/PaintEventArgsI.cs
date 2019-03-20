using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;
using SvgNet.SvgGdi;

namespace MissionPlanner.Swarm
{
    public class PaintEventArgsI : EventArgs, IDisposable
    {
        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.PaintEventArgs" /> class with the specified graphics and clipping rectangle.</summary>
        /// <param name="graphics">The <see cref="T:System.Drawing.Graphics" /> used to paint the item. </param>
        /// <param name="clipRect">The <see cref="T:System.Drawing.Rectangle" /> that represents the rectangle in which to paint. </param>
        public PaintEventArgsI(IGraphics graphics, Rectangle clipRect)
        {
            Graphics = graphics;
            ClipRectangle = clipRect;

            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);

            GL.Enable(EnableCap.Blend);

            GL.MatrixMode(MatrixMode.Projection);

            GL.LoadIdentity();

            GL.Ortho(0, ClipRectangle.Width, ClipRectangle.Height, 0, -1, 1);

            GL.Viewport(0,0,clipRect.Width,clipRect.Height);
        }
        /// <summary>Allows an object to try to free resources and perform other cleanup operations before it is reclaimed by garbage collection.</summary>
        ~PaintEventArgsI() { }
        /// <summary>Gets the rectangle in which to paint.</summary>
        /// <returns>The <see cref="T:System.Drawing.Rectangle" /> in which to paint.</returns>
        public Rectangle ClipRectangle { get; }
        /// <summary>Gets the graphics used to paint.</summary>
        /// <returns>The <see cref="T:System.Drawing.Graphics" /> object used to paint. The <see cref="T:System.Drawing.Graphics" /> object provides methods for drawing objects on the display device.</returns>
        public IGraphics Graphics { get; }
        /// <summary>Releases all resources used by the <see cref="T:System.Windows.Forms.PaintEventArgs" />.</summary>
        public void Dispose() { }
        /// <summary>Releases the unmanaged resources used by the <see cref="T:System.Windows.Forms.PaintEventArgs" /> and optionally releases the managed resources.</summary>
        /// <param name="disposing">
        /// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources. </param>
        protected virtual void Dispose(bool disposing) { }
    }
}