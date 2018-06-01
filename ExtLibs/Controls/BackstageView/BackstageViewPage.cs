using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls.BackstageView
{
    /// <summary>
    /// Data structure to hold information about a 'tab' in the <see cref="BackstageView"/>
    /// </summary>
    public class BackstageViewPage : Component, IBindableComponent
    {
        public delegate void ThemeManager(Control ctl);

        public static event ThemeManager ApplyTheme;

        private const int ButtonSpacing = 30;

        public bool Advanced { get; set; }

        [Bindable(true)]
        public bool Show { get; set; }

        public BackstageViewPage()
        {
            Show = true;
        }

        public BackstageViewPage(Type pageType, string linkText, BackstageViewPage parent = null, bool advanced = false)
        {
            Show = true;
            PageType = pageType;
            LinkText = linkText;
            Parent = parent;
            Advanced = advanced;
        }

        private UserControl _page = null;

        public bool isPageCreated {
            get { return _page != null; }
        }

        /// <summary>
        /// The user content of the tab
        /// </summary>
        public UserControl Page {
            get {
                if (_page == null)
                {
                    if (DesignMode)
                        return null;
                    _page = (UserControl) Activator.CreateInstance(PageType);
                    _page.Enabled = false;
                    _page.Visible = false;
                    _page.Location = new Point(0, 0);
                    _page.Dock = DockStyle.Fill;
                    _page.AutoScroll = true;
                    if (ApplyTheme != null)
                        ApplyTheme(_page);
                    _page.Enabled = true;
                }
                return _page;
            }
            set
            {
                _page = value;
                if (ApplyTheme != null)
                    ApplyTheme(_page);
            }
        }

        public Type PageType { get; internal set; }

        /// <summary>
        /// The text to go in the 'tab header'
        /// </summary>
        public string LinkText { get; set; }

        [EditorAttribute(
            "System.ComponentModel.Design.MultilineStringEditor, System.Design",
            "System.Drawing.Design.UITypeEditor"), Localizable(true)]
        public string Text { get { return LinkText; } set { LinkText = value; } }

        public BackstageViewPage Parent { get; set; }

        private int _spaceoverride = -1;

        public int Spacing
        {
            get { if (_spaceoverride != -1) return _spaceoverride; return ButtonSpacing; }
            set { _spaceoverride = value; }
        }

        public new string ToString()
        {
            return "Page: " + LinkText;
        }

        private BindingContext bindingContext;

        private ControlBindingsCollection dataBindings;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BindingContext BindingContext
        {
            get
            {
                if (bindingContext == null)
                {
                    bindingContext = new BindingContext();
                }

                return bindingContext;
            }

            set
            {
                bindingContext = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ControlBindingsCollection DataBindings
        {
            get
            {
                if (dataBindings == null)
                {
                    dataBindings = new ControlBindingsCollection(this);
                }
                return dataBindings;
            }
        }
    }
}