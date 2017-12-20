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

        public delegate void TrackingEventHandler(string page, string title);
        public static event TrackingEventHandler Tracking;

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
            if (Screen == null)
                return;

            // add to list
            screens.Add(Screen);

            // hide it
            if (Screen.Control != null)
                Screen.Control.Visible = false;
        }

        public void Reload()
        {
            ShowScreen(current.Name);
        }

        void CreateControl(Screen current)
        {
            Type type = current.Type;

            // create new instance on gui thread
            if (MainControl.InvokeRequired)
            {
                MainControl.Invoke((MethodInvoker) delegate
                {
                    try
                    {
                        current.Control = (MyUserControl)Activator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to invoke create control " + current.Name + " of type " + current.Type, ex);
                    }
                });
            }
            else
            {
                try
                {
                    current.Control = (MyUserControl) Activator.CreateInstance(type);
                }
                catch
                {
                    try
                    {
                        current.Control = (MyUserControl) Activator.CreateInstance(type);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to create control " + current.Name + " of type " + current.Type, ex);
                    }
                }
            }

            // set the next new instance as not visible
            current.Control.Visible = false;
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

                    current.Control = null;

                    GC.Collect();
                }
            }

            if (name == "")
                return;

            // find next screen
            Screen nextscreen = screens.Single(s => s.Name == name);

            // screen name dosnt exist
            if (nextscreen == null)
                return;

            // screen control is null, create it
            if (nextscreen.Control == null || nextscreen.Control.IsDisposed)
                CreateControl(nextscreen);

            MainControl.SuspendLayout();
            nextscreen.Control.SuspendLayout();

            nextscreen.Control.Location = new Point(0, 0);

            nextscreen.Control.AutoScaleMode = AutoScaleMode.None;

            nextscreen.Control.Size = MainControl.Size;

            nextscreen.Control.Dock = DockStyle.Fill;

            Tracking?.Invoke(nextscreen.Control.GetType().ToString(), name);

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

            nextscreen.Control.Refresh();

            nextscreen.Visible = true;

            current = nextscreen;

            current.Control.Focus();
        }

        public class Screen
        {
            public string Name;
            public MyUserControl Control;
            public Type Type;

            public bool Visible
            {
                get
                {
                    if (Control == null) 
                        return false;
                    return Control.Visible;
                }
                set
                {
                    try
                    {
                        Control.Visible = value;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }

            public bool Persistent;

            public Screen(string Name, MyUserControl Control, bool Persistent = false)
            {
                this.Name = Name;
                this.Control = Control;
                this.Persistent = Persistent;
                if (Control == null)
                    return;
                this.Type = Control.GetType();
            }

            public Screen(string Name, Type Type, bool Persistent = false)
            {
                this.Name = Name;
                this.Type = Type;
                this.Persistent = Persistent;
            }
        }

        public void Dispose()
        {
            if (current != null && current.Control != null && current.Control is IDeactivate)
            {
                ((IDeactivate)(current.Control)).Deactivate();
            }

            foreach (var item in screens)
            {
                try
                {
                    Console.WriteLine("MainSwitcher dispose " + item?.Name);
                    if (item?.Control != null)
                    {
                        item.Control.Close();
                        item.Control.Dispose();
                    }
                }
                catch { }
            }

            MainControl.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
