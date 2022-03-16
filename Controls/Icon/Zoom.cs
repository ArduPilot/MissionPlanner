using System.Drawing;

namespace MissionPlanner.Controls.Icon
{
    public class Zoom : Icon
    {
        internal override void doPaint(Graphics g)
        {
            var mid = Width / 2;
            var quartmid = mid / 4;

            var arcrect = new RectangleF(mid - quartmid, 0 + quartmid, mid, mid);

            g.DrawArc(LinePen, arcrect, 0, 360);

            g.DrawLine(LinePen, 0+ mid/2, Height - mid/2, mid, mid);

            arcrect.Inflate(-4, -4);

            //lr
            g.DrawLine(LinePen, arcrect.X,arcrect.Y + arcrect.Height/2, arcrect.X + arcrect.Width, arcrect.Y + arcrect.Height/2);

            //ud
            g.DrawLine(LinePen, arcrect.X + arcrect.Width / 2, arcrect.Y, arcrect.X + arcrect.Width / 2, arcrect.Y + arcrect.Height);
        }
    }
}
