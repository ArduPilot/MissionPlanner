using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Controls.BackstageView;

namespace MissionPlanner.Controls.BackstageView
{
    /// <summary>
    /// A Control to emulate the 'backstage view' (File Menu style) seen in MS Office 2010/2013.
    /// Handles navigation between different configuration and setup screens.
    /// </summary>
    /// <remarks>
    /// 'Tabs' are added as a control in a <see cref="BackstageViewPage"/>
    /// </remarks>
    public partial class BackstageView : MyUserControl, IContainerControl
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private Color _buttonsAreaBgColor = Color.White;
        private Color _buttonsAreaPencilColor = Color.DarkGray;
        private Color _selectedTextColor = Color.White;
        private Color _unSelectedTextColor = Color.Gray;
        private Color _highlightColor1 = SystemColors.Highlight;
        private Color _highlightColor2 = SystemColors.MenuHighlight;

        private readonly BackstageViewCollection _items = new BackstageViewCollection();
        private BackstageViewPage _activePage;

        private const int ButtonHeight = 30;

        private List<BackstageViewPage> expanded = new List<BackstageViewPage>();

        public BackstageViewPage SelectedPage { get { return _activePage; } }

        public delegate void TrackingEventHandler(string page, string title);
        public static event TrackingEventHandler Tracking;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public BackstageViewCollection Pages { get { return _items; } }

        /// <summary>
        /// Show advanced items or not
        /// </summary>
        public static bool Advanced { get; set; }

        public int WidthMenu
        {
            get
            {
                return pnlMenu.Width;
            }
            set
            {
                int delta = value - pnlMenu.Width;
                pnlMenu.Width = value;
                pnlPages.Location = new Point(pnlPages.Location.X + delta, pnlPages.Location.Y);
                pnlPages.Width -= delta;
            }
        }

        // Tracks the currently popped-out page (if any)
        private BackstageViewPage popoutPage = null;

        private int ButtonTopPos = 0;

        public BackstageView()
        {
            InitializeComponent();

            this.pnlMenu.Height = this.Height;
            this.pnlPages.Height = this.Height;

            pnlMenu.BackColor = _buttonsAreaBgColor;
            pnlMenu.PencilBorderColor = _buttonsAreaPencilColor;
            pnlMenu.GradColor = this.BackColor;
        }

        public void UpdateDisplay()
        {
            foreach (BackstageViewPage itemType in _items)
            {
                if (!itemType.Show)
                    continue;

                if (itemType.Page != null)
                {
                    itemType.Page.Location = new Point(0, 0);
                    itemType.Page.Dock = DockStyle.Fill;
                    itemType.Page.Visible = false;

                    this.pnlPages.Controls.Add(itemType.Page);
                }
            }
        }

        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
                UpdateButtonAppearance();
                pnlMenu.GradColor = this.BackColor;
            }
        }

        [Description("Background pencil line color for the content region"), Category("Appearance")]
        [DefaultValue(typeof(Color), "DarkGray")]
        public Color ButtonsAreaPencilColor
        {
            get { return _buttonsAreaPencilColor; }
            set
            {
                _buttonsAreaPencilColor = value;
                pnlMenu.PencilBorderColor = _buttonsAreaPencilColor;
                pnlMenu.Invalidate();
                UpdateButtonAppearance();
                Invalidate();
            }
        }

        [Description("Background color for the buttons region"), Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color ButtonsAreaBgColor
        {
            get { return _buttonsAreaBgColor; }
            set
            {
                _buttonsAreaBgColor = value;
                this.pnlMenu.BackColor = _buttonsAreaBgColor;
                pnlMenu.Invalidate();
                Invalidate();
            }
        }

        [Description("Color for the selector buttons text"), Category("Appearance")]
        [DefaultValue(typeof(Color), "White")]
        public Color SelectedTextColor
        {
            get { return _selectedTextColor; }
            set
            {
                _selectedTextColor = value;
                UpdateButtonAppearance();
            }
        }

        [Description("Color for the un selected selector buttons text"), Category("Appearance")]
        [DefaultValue(typeof(Color), "Gray")]
        public Color UnSelectedTextColor
        {
            get { return _unSelectedTextColor; }
            set
            {
                _unSelectedTextColor = value;
                UpdateButtonAppearance();
                Invalidate();
            }
        }

        [Description("Color selected button background 1"), Category("Appearance")]
        [DefaultValue(typeof(Color), "DarkBlue")]
        public Color HighlightColor1
        {
            get { return _highlightColor1; }
            set
            {
                _highlightColor1 = value;
                UpdateButtonAppearance();
                Invalidate();
            }
        }

        [Description("Color selected button background 2"), Category("Appearance")]
        [DefaultValue(typeof(Color), "Blue")]
        public Color HighlightColor2
        {
            get { return _highlightColor2; }
            set
            {
                _highlightColor2 = value;
                UpdateButtonAppearance();
                Invalidate();
            }
        }

        /// <summary>
        /// Add a page (tab) to this backstage view. Will be added at the end/bottom
        /// </summary>
        public BackstageViewPage AddPage(Type userControl, string headerText, BackstageViewPage Parent, bool advanced)
        {
            var page = new BackstageViewPage(userControl, headerText, Parent, advanced);

            _items.Add(page);

            return page;
        }

        /// <summary>
        /// Add a spacer to this backstage view. Will be added at the end/bottom
        /// </summary>
        /// <param name="spacerheight">the amount to space by</param>
        public void AddSpacer(int spacerheight)
        {
            ButtonTopPos += spacerheight;
        }

        private void CreateLinkButton(BackstageViewPage page, bool haschild = false, bool child = false)
        {
            if (!page.Show)
                return;

            string label = page.LinkText;
            int heightextra = 0;

            if (haschild)
            {
                label = ">> " + label;
            }
            if (child)
            {
                int count = label.Split('\n').Count();
                label = "      " + label.Replace("\n", "\n      ");
                heightextra = 15 * (count - 1);
            }

            var lnkButton = new BackstageViewButton
            {
                Text = label,
                Tag = page,
                Top = ButtonTopPos,
                Width = this.pnlMenu.Width,
                Height = ButtonHeight + heightextra,
                ContentPageColor = this.BackColor,
                PencilBorderColor = _buttonsAreaPencilColor,
                SelectedTextColor = _selectedTextColor,
                UnSelectedTextColor = _unSelectedTextColor,
                HighlightColor1 = _highlightColor1,
                HighlightColor2 = _highlightColor2,
            };

            pnlMenu.Controls.Add(lnkButton);
            lnkButton.Click += this.ButtonClick;
            lnkButton.DoubleClick += lnkButton_DoubleClick;

            ButtonTopPos += lnkButton.Height;
            pnlMenu.Invalidate();
        }

        public void DrawMenu(BackstageViewPage CurrentPage, bool force = false)
        {
            if (!force)
            {
                if (_activePage == CurrentPage || CurrentPage == null)
                {
                    bool children = PageHasChildren(CurrentPage);
                    if (!children)
                    {
                        return;
                    }
                }
            }

            pnlMenu.Controls.Clear();

            // reset back to 0
            ButtonTopPos = 0;

            foreach (BackstageViewPage page in _items)
            {
                if (page.GetType() == typeof(BackstageViewPage))
                {
                    // skip advanced pages if we are not advanced
                    if (page.Advanced && !Advanced)
                    {
                        continue;
                    }

                    // its a base item. we want it
                    if (((BackstageViewPage)page).Parent == null)
                    {
                        bool children = PageHasChildren(page);

                        CreateLinkButton((BackstageViewPage)page, children);

                        // remember whats expanded
                        if (CurrentPage == page && children)
                        {
                            if (expanded.Contains((BackstageViewPage)page))
                            {
                                expanded.Remove((BackstageViewPage)page);
                            }
                            else
                            {
                                expanded.Add((BackstageViewPage)page);
                            }
                        }

                        // check for children
                        foreach (BackstageViewPage childrenpage in _items)
                        {
                            if (childrenpage.GetType() == typeof(BackstageViewPage))
                            {
                                if (((BackstageViewPage)childrenpage).Parent == ((BackstageViewPage)page))
                                {
                                    // check if current page has a parent thats not expanded
                                    if (CurrentPage == childrenpage && !expanded.Contains((BackstageViewPage)page))
                                    {
                                        expanded.Add((BackstageViewPage)page);
                                        DrawMenu(CurrentPage, true);
                                        return;
                                    }
                                    // draw all the siblings
                                    if (expanded.Contains((BackstageViewPage)page) || this.DesignMode)
                                    {
                                        CreateLinkButton((BackstageViewPage)childrenpage, false, true);
                                    }
                                }
                            }
                        }
                        continue;
                    }
                }
                else
                {
                    ButtonTopPos += page.Spacing;
                }
            }

            pnlMenu.Invalidate();
        }

        private bool PageHasChildren(BackstageViewPage parent)
        {
            // check for children
            foreach (BackstageViewPage child in _items)
            {
                if (child.GetType() == typeof(BackstageViewPage))
                {
                    if (((BackstageViewPage)child).Parent == parent)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void UpdateButtonAppearance()
        {
            foreach (var backstageViewButton in pnlMenu.Controls.OfType<BackstageViewButton>())
            {
                backstageViewButton.HighlightColor2 = _highlightColor2;
                backstageViewButton.HighlightColor1 = _highlightColor1;
                backstageViewButton.UnSelectedTextColor = _unSelectedTextColor;
                backstageViewButton.SelectedTextColor = _selectedTextColor;
                backstageViewButton.ContentPageColor = this.BackColor;
                backstageViewButton.PencilBorderColor = _buttonsAreaPencilColor;

                backstageViewButton.Invalidate();
            }
        }

        /// <summary>
        /// Experimental - double clicking a button will spawn it out into a new form
        /// Allows interacting with two pages simultaneously.
        /// </summary>
        private void lnkButton_DoubleClick(object sender, EventArgs e)
        {
            var backstageViewButton = ((BackstageViewButton)sender);
            var associatedPage = backstageViewButton.Tag as BackstageViewPage;

            var popoutForm = new Form();

            // ----------------------------------------------------------------------
            // FIX for missing icon in pop-out window
            // ----------------------------------------------------------------------

            // Strategy 1: Attempt to copy the icon from the main parent form
            if (this.FindForm() != null)
            {
                popoutForm.Icon = this.FindForm().Icon;
            }

            // Strategy 2: If strategy 1 fails, try to extract the icon from the executable.
            // This is wrapped in a preprocessor directive because ExtractAssociatedIcon is only available on Windows.
#if !NETSTANDARD
            try
            {
                if (popoutForm.Icon == null)
                {
                    popoutForm.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
                }
            }
            catch { /* Ignore failure to extract icon */ }
#endif

            // Ensure the icon is visible
            popoutForm.ShowIcon = true;

            // ----------------------------------------------------------------------

            popoutForm.FormClosing += popoutForm_FormClosing;

            int maxright = 0, maxdown = 0;

            foreach (Control ctl in associatedPage.Page.Controls)
            {
                maxright = Math.Max(ctl.Right, maxright);
                maxdown = Math.Max(ctl.Bottom, maxdown);
            }

            popoutForm.Height = 0;
            popoutForm.Size = new Size(maxright + 20, maxdown + 20 + popoutForm.Height);

            popoutForm.Controls.Add(associatedPage.Page);
            popoutForm.Tag = associatedPage;

            popoutForm.Text = associatedPage.LinkText;

            popoutPage = associatedPage;

            popoutForm.BackColor = this.BackColor;
            popoutForm.ForeColor = this.ForeColor;

            popoutForm.Show(this);
        }

        private void popoutForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // get the page back
            var temp = ((Form)sender).Tag as BackstageViewPage;

            // add back to where it belongs
            this.pnlPages.Controls.Add(temp.Page);

            // clear the controls, so we dont dispose the good control when it closes
            ((Form)sender).Controls.Clear();
            popoutPage = null;
        }

        private void ButtonClick(object sender, EventArgs e)
        {
            var backstageViewButton = ((BackstageViewButton)sender);
            var associatedPage = backstageViewButton.Tag as BackstageViewPage;
            this.ActivatePage(associatedPage);
        }

        /// <summary>
        /// Activates the selected page and displays it in the content panel.
        /// Also handles closing any existing pop-out windows.
        /// </summary>
        public void ActivatePage(BackstageViewPage associatedPage)
        {
            if (associatedPage == null)
            {
                if (_activePage == null)
                    DrawMenu(null, true);
                return;
            }

            Tracking?.Invoke(associatedPage.Page.GetType().ToString(), associatedPage.LinkText);

            var start = DateTime.Now;

            this.SuspendLayout();
            associatedPage.Page.SuspendLayout();

            DrawMenu(associatedPage, false);

            // Deactivate old page
            if (_activePage != null && _activePage.Page is IDeactivate)
            {
                try
                {
                    ((IDeactivate)(_activePage.Page)).Deactivate();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            if (_activePage != null && _activePage.Page != null)
                _activePage.Page.Visible = false;

            try
            {
                // if the button was on an expanded tab. when we leave it no longer exits
                if (_activePage != null)
                {
                    var oldButton = this.pnlMenu.Controls.OfType<BackstageViewButton>().Single(b => b.Tag == _activePage);
                    oldButton.IsSelected = false;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            associatedPage.Page.ResumeLayout(false);
            this.ResumeLayout(false);

            // show it
            associatedPage.Page.Visible = true;

            // ---------------------------------------------------------
            // FIX for Issue #3633: Pop-out window not closing when re-docking
            // ---------------------------------------------------------
            // If any page is currently popped out in an external window, close it
            // before navigating to prevent leaving an empty "ghost" window.
            if (popoutPage != null)
            {
                Form parentWindow = popoutPage.Page.Parent as Form;
                if (parentWindow != null)
                {
                    parentWindow.Close();
                }
            }
            // ---------------------------------------------------------

            if (!pnlPages.Controls.Contains(associatedPage.Page))
                this.pnlPages.Controls.Add(associatedPage.Page);

            // new way of notifying activation.
            if (associatedPage.Page is IActivate)
            {
                try
                {
                    ((IActivate)(associatedPage.Page)).Activate();
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }

            try
            {
                var newButton = this.pnlMenu.Controls.OfType<BackstageViewButton>().Single(b => b.Tag == associatedPage);
                newButton.IsSelected = true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

            var end = DateTime.Now;

            log.DebugFormat("{0} {1} {2}", associatedPage.Page.GetType().ToString(), associatedPage.LinkText,
                (end - start).TotalMilliseconds);

            _activePage = associatedPage;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (pnlMenu.Controls.Count == 0)
                DrawMenu(null, false);

            base.OnPaint(e);
        }

        public new void Close()
        {
            foreach (var page in _items)
            {
                if (popoutPage != null && popoutPage == page)
                    continue;

                if (!((BackstageViewPage)page).isPageCreated)
                    continue;

                if (((BackstageViewPage)page).Page is IDeactivate)
                {
                    try
                    {
                        ((IDeactivate)((BackstageViewPage)(page)).Page).Deactivate();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }

                    try
                    {
                        ((BackstageViewPage)page).Page.Dispose();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
                else
                {
                    try
                    {
                        ((BackstageViewPage)page).Page.Dispose();
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }
        }

        private void BackstageView_Load(object sender, EventArgs e)
        {
            UpdateDisplay();
            DrawMenu(_activePage, false);
        }
    }
}