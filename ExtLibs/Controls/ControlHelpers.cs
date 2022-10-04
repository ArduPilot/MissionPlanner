using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public static class ControlHelpers
    {
        /// <summary>
        /// Loads and applies saved form position and size settings from config
        /// </summary>
        /// <param name="control">Control to load and apply the settings for</param>
        public static void RestoreStartupLocation(this Form control)
        {
            var value = Settings.Instance[control.Text.Replace(" ", "_") + "_StartLocation"];

            if (value != null)
            {
                try
                {
                    var fsl = value.FromJSON<FormStartLocation>();
                    control.Location = fsl.Location;
                    control.Size = fsl.Size;
                    control.StartPosition = RectVisible(control.Bounds) ? FormStartPosition.Manual : FormStartPosition.WindowsDefaultLocation;
                    control.WindowState = fsl.State;
                } catch {}
            }
        }

        public class FormStartLocation
        {
            public Point Location { get; set; }
            public Size Size { get; set; }
            public FormWindowState State { get; set; }
        }

        /// <summary>
        /// Saves form position and size settings to config
        /// </summary>
        /// <param name="control">Control to save the settings for</param>
        public static void SaveStartupLocation(this Form control)
        {
            Settings.Instance[control.Text.Replace(" ", "_") + "_StartLocation"] = new FormStartLocation {Location = control.Location, Size = control.Size, State = control.WindowState}.ToJSON();
        }

        /// <summary>
        /// Determines if a rectangle is visible on screen
        /// </summary>
        /// <param name="rectangle">Rectangle to check visibility for</param>
        /// <returns>True, if the rectangle is fully visible, False, if even partially not</returns>
        private static bool RectVisible(Rectangle rectangle)
        {
            foreach (Screen screen in Screen.AllScreens)
            {
                if (screen.Bounds.Contains(rectangle)) return true;
            }
            return false;
        }

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

        public static void BeginInvokeIfRequired(this ISynchronizeInvoke control, Action action)
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
