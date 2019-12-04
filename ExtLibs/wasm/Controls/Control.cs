using System;
using System.Drawing;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace MissionPlanner.Controls
{
    public class Control: ComponentBase
    {
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
            throw new NotImplementedException();
        }

        public virtual void Refresh()
        {
            Context.BeginBatchAsync();
            OnPaint(new PaintEventArgs(new GraphicsWeb(Context), Rectangle.FromLTRB(0, 0, Width, Height)));
            Context.EndBatchAsync();
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
            
        }

        protected virtual void OnPaintBackground(PaintEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}