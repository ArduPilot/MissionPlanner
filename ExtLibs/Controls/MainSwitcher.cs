using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class MainSwitcher : IDisposable
    {
        public delegate void ThemeManager(Control ctl);

        public static event ThemeManager ApplyTheme;

        public List<Screen> screens = new List<Screen>();
        public Screen current;
        UserControl MainControl = new UserControl();

        public int Width { get { return MainControl.Width; } }
        public int Height { get { return MainControl.Height; } }

        public Control.ControlCollection Controls { get { return MainControl.Controls; } }

        public MainSwitcher(Control Parent)
        {
            MainControl.Dock = DockStyle.Fill;

            Parent.Controls.Add(MainControl);
        }

        public void AddScreen(Screen Screen)
        {
            if (Screen == null || Screen.Control == null)
                return;

            // add to list
            screens.Add(Screen);

            // hide it
            Screen.Control.Visible = false;
        }

        public void Reload()
        {
            ShowScreen(current.Name);
        }

        public void ShowScreen(string name)
        {
            if (current != null && current.Control != null)
            {
                // hide current screen
                current.Visible = false;

                // remove reference
                MainControl.Controls.Remove(current.Control);

                if (current.Control is IDeactivate)
                {
                    ((IDeactivate)(current.Control)).Deactivate();
                }

                // check if we need to remove the current control
                if (!current.Persistent)
                {
                    // cleanup
                    current.Control.Close();

                    current.Control.Dispose();

                    Type type = current.Control.GetType();

                    current.Control = null;

                    GC.Collect();

                    // create new instance on gui thread
                    if (MainControl.InvokeRequired)
                    {
                        MainControl.Invoke((MethodInvoker) delegate
                        {
                            current.Control = (MyUserControl) Activator.CreateInstance(type);
                        });
                    }
                    else
                    {
                        current.Control = (MyUserControl)Activator.CreateInstance(type);
                    }

                    // set the next new instance as not visible
                    current.Control.Visible = false;
                }
            }

            if (name == "")
                return;

            // find next screen
            Screen nextscreen = screens.Single(s => s.Name == name);

            MainControl.SuspendLayout();
            nextscreen.Control.SuspendLayout();

            nextscreen.Control.Location = new Point(0, 0);

            nextscreen.Control.AutoScaleMode = AutoScaleMode.None;

            nextscreen.Control.Size = MainControl.Size;

            nextscreen.Control.Dock = DockStyle.Fill;

            MissionPlanner.Utilities.Tracking.AddPage(nextscreen.Control.GetType().ToString(), name);

            if (nextscreen.Control is IActivate)
            {
                ((IActivate)(nextscreen.Control)).Activate();
            }

            if (ApplyTheme != null)
                ApplyTheme(nextscreen.Control);


            if (MainControl.InvokeRequired)
            {
                MainControl.Invoke((MethodInvoker)delegate
                {
                    MainControl.Controls.Add(nextscreen.Control);
                    nextscreen.Control.ResumeLayout();
                    MainControl.ResumeLayout();
                });
            }
            else
            {
                MainControl.Controls.Add(nextscreen.Control);
                nextscreen.Control.ResumeLayout();
                MainControl.ResumeLayout();
            }

            nextscreen.Visible = true;

            current = nextscreen;

            current.Control.Focus();
        }

        public class Screen
        {
            public string Name;
            public MyUserControl Control;

            public bool Visible
            {
                get { return Control.Visible; }
                set { try { Control.Visible = value; } catch { } }
            }

            public bool Persistent;

            public Screen(string Name, MyUserControl Control, bool Persistent = false)
            {
                this.Name = Name;
                this.Control = Control;
                this.Persistent = Persistent;
            }
        }

        public void Dispose()
        {
            if (current.Control is IDeactivate)
            {
                ((IDeactivate)(current.Control)).Deactivate();
            }

            foreach (var item in screens)
            {
                try
                {
                    Console.WriteLine("MainSwitcher dispose " + item.Control.Name);
                    item.Control.Close();
                    item.Control.Dispose();
                }
                catch { }
            }

            MainControl.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
