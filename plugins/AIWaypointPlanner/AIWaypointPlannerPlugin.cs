using System;
using System.Windows.Forms;

namespace MissionPlanner.AIWaypointPlanner
{
    public sealed class AIWaypointPlannerPlugin : MissionPlanner.Plugin.Plugin
    {
        private ToolStripMenuItem menuItem;

        public override string Name { get { return "AI 航点规划"; } }
        public override string Version { get { return "1.4.2"; } }
        public override string Author { get { return "Local Mission Planner Plugin"; } }

        public override bool Init()
        {
            return true;
        }

        public override bool Loaded()
        {
            menuItem = new ToolStripMenuItem(Name);
            menuItem.ToolTipText = "用自然语言生成经本地校验的候选航点";
            menuItem.Click += OpenPlanner;

            ToolStripItemCollection items = Host.FPMenuMap.Items;
            foreach (ToolStripItem item in items)
            {
                if (string.Equals(item.Name, "autoWPToolStripMenuItem", StringComparison.Ordinal) &&
                    item is ToolStripMenuItem)
                {
                    ((ToolStripMenuItem)item).DropDownItems.Add(menuItem);
                    return true;
                }
            }

            items.Add(menuItem);
            return true;
        }

        private void OpenPlanner(object sender, EventArgs e)
        {
            using (var form = new AIWaypointPlannerForm(this))
            {
                MissionPlanner.Utilities.ThemeManager.ApplyThemeTo(form);
                form.ShowDialog(Host.MainForm);
            }
        }

        public override bool Exit()
        {
            if (menuItem != null)
            {
                menuItem.Click -= OpenPlanner;
                if (menuItem.Owner != null)
                    menuItem.Owner.Items.Remove(menuItem);
                menuItem.Dispose();
                menuItem = null;
            }
            return true;
        }
    }
}
