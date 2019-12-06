using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// Mono handles calls from other thread difrently - this prevents those crashs
    /// </summary>
    public class myGMAP : GMap.NET.WindowsForms.GMapControl
    {
        public bool inOnPaint = false;
        string otherthread = "";
        int lastx = 0;
        int lasty = 0;
        public myGMAP()
            : base()
        {
            this.Text = "Map";
            IgnoreMarkerOnMouseWheel = true;
        }

        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
        {
            var start = DateTime.Now;

            if (inOnPaint)
            {
                Console.WriteLine("Was in onpaint Gmap th:" + System.Threading.Thread.CurrentThread.Name + " in " + otherthread);
                return;
            }

            otherthread = System.Threading.Thread.CurrentThread.Name;

            inOnPaint = true;

            try
            {
                base.OnPaint(e);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            inOnPaint = false;

            var end = DateTime.Now;

            System.Diagnostics.Debug.WriteLine("map draw time " + (end-start).TotalMilliseconds);
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            try
            {
                var buffer = 1;
                // try prevent alot of cpu usage
                if (e.X >= lastx - buffer && e.X <= lastx + buffer && e.Y >= lasty - buffer && e.Y <= lasty + buffer)
                    return;

                if (HoldInvalidation)
                    return;

                lastx = e.X;
                lasty = e.Y;

                base.OnMouseMove(e);
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }
    }
}
