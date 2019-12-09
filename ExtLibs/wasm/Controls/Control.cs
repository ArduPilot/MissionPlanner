using System;
using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MissionPlanner.Controls
{
    public static class Extension
    {
        public static string ToHex(this Color col)
        {
            return String.Format("rgba({0},{1},{2},{3})", col.R, col.G, col.B, col.A / (float) 255);
        }
    }
    public class Control: ComponentBase
    {
        public bool invalidated;
        public Canvas2DContext Context { get; set; }

        protected virtual void OnMouseClick(MouseEventArgs mouseEventArgs)
        {
            throw new NotImplementedException();
        }

        public bool DesignMode { get; set; }

        public string Name { get; set; }

        public bool IsHandleCreated { get; set; }

        public bool Visible { get; set; } = true;
        [Parameter]
        public int Width { get; set; }
        [Parameter]
        public int Height { get; set; }

        protected virtual void OnHandleDestroyed(EventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        protected void Invalidate()
        {
            invalidated = true;
        }

        public async virtual void Refresh()
        {
            var s1 = DateTime.Now;
            await this.Context.BeginBatchAsync();
            var s2 = DateTime.Now;
            OnPaint(new PaintEventArgs(new GraphicsWeb(Context, Width, Height), Rectangle.FromLTRB(0, 0, Width, Height)));
            var s3 = DateTime.Now;
            await this.Context.EndBatchAsync();
            var s4 = DateTime.Now;
            //Console.WriteLine("{0} {1} {2}", (s2 - s1).TotalMilliseconds, (s3 - s2).TotalMilliseconds, (s4 - s3).TotalMilliseconds);
        }

        protected virtual void OnMouseMove(MouseEventArgs mouseEventArgs)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnResize(EventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnHandleCreated(EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnLoad(EventArgs e)
        {
            throw new NotImplementedException();
        }

        protected virtual void OnPaint(PaintEventArgs e)
        {
            invalidated = false;
        }

        protected virtual void OnPaintBackground(PaintEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}