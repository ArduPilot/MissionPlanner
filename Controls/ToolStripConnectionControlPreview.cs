using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public class ToolStripConnectionControlPreview : ToolStripControlHost
    {
        // Call the base constructor passing in a MonthCalendar instance.     
        public ToolStripConnectionControlPreview() : base(new ConnectionControlPreview())
        {

        }

        public ConnectionControlPreview ConnectionControlPreview
        {
            get { return Control as ConnectionControlPreview; }
        }
    }
}