using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public static class ControlHelpers
    {
        public static void InvokeIfRequired<T>(this T control, Action<T> action) where T : ISynchronizeInvoke
        {
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(() => action(control)), null);
            }
            else
            {
                action(control);
            }
        }

        public static void InvokeIfRequired(this ISynchronizeInvoke control, MethodInvoker action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke(action, null);
            }
            else
            {
                action();
            }
        }

        public static void BeginInvokeIfRequired<T>(this T control, Action<T> action) where T : ISynchronizeInvoke
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(new Action(() => action(control)), null);
            }
            else
            {
                action(control);
            }
        }

        public static void BeginInvokeIfRequired(this ISynchronizeInvoke control, MethodInvoker action)
        {
            if (control.InvokeRequired)
            {
                control.BeginInvoke(action, null);
            }
            else
            {
                action();
            }
        }
    }
}
