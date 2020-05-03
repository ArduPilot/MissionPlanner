using System.Windows.Forms;
namespace MissionPlanner.Controls
{
    public class VerticalProgressBar : HorizontalProgressBar
    {
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style |= 0x04;
                return cp;
            }
        }
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
        }
    }
}
