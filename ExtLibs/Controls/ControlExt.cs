using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
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
