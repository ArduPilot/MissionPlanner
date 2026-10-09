//this file contains some simple extension methods

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace MissionPlanner.Utilities
{
    public static class CultureInfoEx
    {
        public static CultureInfo GetCultureInfo(string name)
        {
            try { return new CultureInfo(name); }
            catch (Exception) { return null; }
        }

        public static bool IsChildOf(this CultureInfo cX, CultureInfo cY)
        {

            if (cX == null || cY == null)
                return false;

            CultureInfo c = cX;
            while (!c.Equals(CultureInfo.InvariantCulture))
            {
                if (c.Equals(cY))
                    return true;
                c = c.Parent;
            }
            return false;
        }
    }

    public static class ComponentResourceManagerEx
    {
        public static void ApplyResource(this ComponentResourceManager rm, Control ctrl)
        {
            ApplyOwnResources(ctrl);

            rm.ApplyResources(ctrl, ctrl.Name);
            foreach (Control subctrl in ctrl.Controls)
                ApplyResource(rm, subctrl);

            if (ctrl.ContextMenuStrip != null)
                ApplyToolStripResources(rm, ctrl.ContextMenuStrip);

            if (ctrl is ToolStrip toolStrip)
                ApplyToolStripResources(rm, toolStrip);


            if (ctrl is DataGridView)
            {
                foreach (DataGridViewColumn col in (ctrl as DataGridView).Columns)
                    rm.ApplyResources(col, col.Name);
            }
        }

        private static void ApplyOwnResources(Control ctrl)
        {
            // 只有自定义窗体/控件才会把自己的 .resx 嵌入成 "<完整类型名>.resources"。
            // 对 CheckBox / Label / Panel 这类框架控件调用 ApplyResources，会去找
            // System.Windows.Forms.<Type>.resources 而抛 MissingManifestResourceException，
            // 所以先确认该类型确实带资源，再应用。
            var ctrlType = ctrl.GetType();
            if (ctrlType.Assembly.GetManifestResourceInfo(ctrlType.FullName + ".resources") == null)
                return;

            var ownrm = new ComponentResourceManager(ctrlType);

            ownrm.ApplyResources(ctrl, "$this");

            foreach (Control subctrl in ctrl.Controls)
                ownrm.ApplyResources(subctrl, subctrl.Name);

            if (ctrl.ContextMenuStrip != null)
                ApplyToolStripResources(ownrm, ctrl.ContextMenuStrip);

            if (ctrl is MenuStrip)
                ApplyToolStripResources(ownrm, ctrl as MenuStrip);

            if (ctrl is DataGridView)
            {
                foreach (DataGridViewColumn col in (ctrl as DataGridView).Columns)
                    ownrm.ApplyResources(col, col.Name);
            }
        }

        private static void ApplyToolStripResources(ComponentResourceManager rm, ToolStrip toolStrip)
        {
            if (toolStrip == null)
                return;

            foreach (ToolStripItem item in toolStrip.Items)
                ApplyToolStripItemResources(rm, item);
        }

        private static void ApplyToolStripItemResources(ComponentResourceManager rm, ToolStripItem item)
        {
            if (item == null)
                return;

            rm.ApplyResources(item, item.Name);

            if (item is ToolStripDropDownItem)
            {
                foreach (ToolStripItem child in ((ToolStripDropDownItem)item).DropDownItems)
                    ApplyToolStripItemResources(rm, child);
            }
        }
    }
}
