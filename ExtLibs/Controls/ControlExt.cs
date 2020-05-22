using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public static class SuspendUpdate
    {
        private const int WM_SETREDRAW = 0x000B;

        public static void Suspend(Control control)
        {
            Message msgSuspendUpdate = Message.Create(control.Handle, WM_SETREDRAW, IntPtr.Zero,
                IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgSuspendUpdate);
        }

        public static void Resume(Control control)
        {
            // Create a C "true" boolean as an IntPtr
            IntPtr wparam = new IntPtr(1);
            Message msgResumeUpdate = Message.Create(control.Handle, WM_SETREDRAW, wparam,
                IntPtr.Zero);

            NativeWindow window = NativeWindow.FromHandle(control.Handle);
            window.DefWndProc(ref msgResumeUpdate);

            control.Invalidate();
        }
    }

    public static class ControlExt
    {

        /// <summary>
        /// this is to fix a mono off screen drawing issue
        /// </summary>
        /// <returns></returns>
        public static bool ThisReallyVisible(this Control ctl)
        {
            if (ctl.Parent != null)
                return ctl.Bounds.IntersectsWith(ctl.Parent.ClientRectangle);

            return true;
        }


        public static bool ChildReallyVisible(this Control child)
        {
            var pos = child.Parent.PointToClient(child.PointToScreen(Point.Empty));
            if (child.GetChildAtPoint(pos) == child) return true;
            if (child.GetChildAtPoint(new Point(pos.X + child.Width - 1, pos.Y)) == child) return true;
            // two more to test
            //...
            return false;
        }
    }
}
