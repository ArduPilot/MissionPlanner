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
            // 外层作用域：该控件在"父容器"的 .resx 里的条目（位置 / 尺寸 / 文本）。
            rm.ApplyResources(ctrl, ctrl.Name);

            // 若控件自带 .resx（自定义 Form / UserControl），它自身和它的后代都属于
            // 它自己的资源作用域，不能让外层窗体的资源渗进来。否则像 ModifyandSet 里
            // 的 myButton1 会被 FlightData.resx 中同名的 myButton1.Text 覆盖掉。
            var ownrm = TryGetOwnResourceManager(ctrl);
            var scope = ownrm ?? rm;

            // 只取子控件自己 resx 里的 $this.Text。绝不能整体 ApplyResources(ctrl, "$this")：
            // 那会把 $this.Size / $this.Margin / $this.MinimumSize 一并应用，覆盖父窗体
            // 已经分配好的布局（换语言时 FlightData 里的 servoOptions1 会从 315x27 变成 295x22）。
            if (ownrm != null)
            {
                var ownText = ownrm.GetString("$this.Text");
                if (!string.IsNullOrEmpty(ownText))
                    ctrl.Text = ownText;
            }

            foreach (Control subctrl in ctrl.Controls)
                scope.ApplyResource(subctrl);

            if (ctrl.ContextMenuStrip != null)
                ApplyToolStripResources(scope, ctrl.ContextMenuStrip);

            if (ctrl is ToolStrip toolStrip)
                ApplyToolStripResources(scope, toolStrip);

            if (ctrl is DataGridView)
            {
                foreach (DataGridViewColumn col in (ctrl as DataGridView).Columns)
                    scope.ApplyResources(col, col.Name);
            }
        }

        /// <summary>
        /// 只有自定义窗体/控件才会把自己的 .resx 嵌入成 "&lt;完整类型名&gt;.resources"。
        /// 对 CheckBox / Label / Panel 这类框架控件构造 ComponentResourceManager 并
        /// ApplyResources，会去找 System.Windows.Forms.&lt;Type&gt;.resources 而抛
        /// MissingManifestResourceException，所以先确认该类型确实带资源。
        /// </summary>
        private static ComponentResourceManager TryGetOwnResourceManager(Control ctrl)
        {
            var ctrlType = ctrl.GetType();
            if (ctrlType.Assembly.GetManifestResourceInfo(ctrlType.FullName + ".resources") == null)
                return null;

            return new ComponentResourceManager(ctrlType);
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
