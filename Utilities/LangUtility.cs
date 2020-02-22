﻿//this file contains some simple extension methods

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
            rm.ApplyResources(ctrl, ctrl.Name);
            foreach (Control subctrl in ctrl.Controls)
                ApplyResource(rm, subctrl);

            if (ctrl.ContextMenuStrip != null)
                ApplyResource(rm, ctrl.ContextMenuStrip);

            if (ctrl is MenuStrip)
            {
                foreach (ToolStripItem item in (ctrl as MenuStrip).Items)
                    rm.ApplyResources(item, item.Name);
            }


            if (ctrl is DataGridView)
            {
                foreach (DataGridViewColumn col in (ctrl as DataGridView).Columns)
                    rm.ApplyResources(col, col.Name);
            }
        }
    }
}
