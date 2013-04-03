using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Eto.Forms;
using Eto.Drawing;
using Eto.Misc;

namespace System.Windows.Forms
{
    public class Control22
    {
        public ControlCollection Controls = new ControlCollection();

        public virtual void SuspendLayout() { }
        public virtual void ResumeLayout(bool value) { }
        public virtual void PerformLayout() { }
        public virtual void SetStyle(ControlStyles st, bool bo) { }
        public virtual void Invalidate() { }
        public virtual void Refresh() { }

        protected virtual void WndProc(ref Message m) { }

        public virtual string Name { get; set; }
        public virtual string Text { get; set; }

        public virtual int Width { get; set; }
        public virtual int Height { get; set; }
        public virtual Size Size { get; set; }

        public virtual DockStyle Dock { get; set; }

        public virtual AnchorStyles Anchor { get; set; }

        public Point Location { get; set; }

        public virtual Size ClientSize { get; set; }

        public virtual bool DoubleBuffered { get; set; }

        public virtual object ContextMenu { get; set; }

        public virtual bool Enabled { get; set; }
        public virtual bool Visible { get; set; }

        public virtual object Tag { get; set; }

        public virtual Control Parent { get; set; }

        public virtual Font Font { get; set; }

        public virtual Color BackColor { get; set; }
        public virtual Color ForeColor { get; set; }

        public event EventHandler TextChanged;
        public event EventHandler Click;
        public event EventHandler DoubleClick;
        public event EventHandler Resize;
        public event MouseEventHandler MouseDown;
        public event EventHandler MouseLeave;
        public event MouseEventHandler MouseMove;

        protected virtual void OnPaint(PaintEventArgs e) { }
        protected virtual void OnMouseEnter(EventArgs e) { }
        protected virtual void OnMouseLeave(EventArgs e) { }
        protected virtual void OnVisibleChanged(EventArgs e) { }
        protected virtual void OnPaintBackground(PaintEventArgs pevent) { }
        protected virtual void OnResize(EventArgs e) { }
        protected virtual void OnParentBindingContextChanged(EventArgs e) { }

        public Rectangle ClientRectangle;

        public void Invoke(object obj) { }
        public void BeginInvoke(object obj) { }
    }

    public class ControlCollection: List<Control>
    {
        public Control[] Find(string name, bool recursive)
        {
            return new Control[0];
        }
    }

}
