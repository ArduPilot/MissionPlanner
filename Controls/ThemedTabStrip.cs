using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// A themed tab strip control that replaces TabControl with a ToolStrip-based approach
    /// for better theme support.
    /// </summary>
    public class ThemedTabStrip : UserControl
    {
        private ToolStrip _toolStrip;
        private Panel _contentPanel;
        private Dictionary<ToolStripButton, Control> _tabPages = new Dictionary<ToolStripButton, Control>();
        private Dictionary<Control, TabPage> _originalTabPages = new Dictionary<Control, TabPage>();
        private List<TabPage> _tabPagesList = new List<TabPage>();
        private ToolStripButton _selectedButton;
        private ContextMenuStrip _contextMenu;

        public event EventHandler SelectedIndexChanged;

        public ThemedTabStrip()
        {
            // Prevent the UserControl from auto-sizing to just the ToolStrip height
            this.AutoSize = false;
            this.AutoScaleMode = AutoScaleMode.None;
            this.BackColor = Color.Blue; // DEBUG: Make the UserControl itself visible

            InitializeComponent();

            // Also handle Load and Layout events to ensure proper sizing
            this.Load += (s, e) => UpdateContentPanelSize();
            this.Layout += (s, e) => UpdateContentPanelSize();
        }

        private void UpdateContentPanelSize()
        {
            if (_contentPanel != null && _toolStrip != null && this.Height > 0)
            {
                _contentPanel.Location = new Point(0, _toolStrip.Height);
                _contentPanel.Size = new Size(this.Width, Math.Max(0, this.Height - _toolStrip.Height));
                System.Diagnostics.Debug.WriteLine($"UpdateContentPanelSize: This={this.Size}, ToolStrip={_toolStrip.Height}, ContentPanel={_contentPanel.Size}");
            }
        }

        private void InitializeComponent()
        {
            _toolStrip = new ToolStrip();
            _contentPanel = new Panel();

            this.SuspendLayout();

            // ToolStrip setup - dock to top
            _toolStrip.Dock = DockStyle.Top;
            _toolStrip.GripStyle = ToolStripGripStyle.Hidden;
            _toolStrip.AutoSize = true;
            _toolStrip.Padding = new Padding(0);
            _toolStrip.BackColor = ThemeManager.BGColor;
            _toolStrip.Renderer = new ThemedToolStripRenderer();
            _toolStrip.LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow;
            _toolStrip.Name = "themedTabStrip_toolStrip";

            // Content panel setup - use anchoring instead of docking
            _contentPanel.BackColor = Color.Red; // DEBUG: Make it visible
            _contentPanel.Name = "themedTabStrip_contentPanel";
            _contentPanel.Visible = true;
            _contentPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            // Add ToolStrip first
            this.Controls.Add(_toolStrip);

            // Position content panel below toolstrip
            _contentPanel.Location = new Point(0, _toolStrip.Height);
            _contentPanel.Size = new Size(this.Width, this.Height - _toolStrip.Height);
            this.Controls.Add(_contentPanel);

            this.ResumeLayout(false);
            this.PerformLayout();

            // Handle resize to reposition content panel
            this.Resize += (s, e) => {
                _contentPanel.Location = new Point(0, _toolStrip.Height);
                _contentPanel.Size = new Size(this.Width, Math.Max(0, this.Height - _toolStrip.Height));
            };

            _toolStrip.Resize += (s, e) => {
                _contentPanel.Location = new Point(0, _toolStrip.Height);
                _contentPanel.Size = new Size(this.Width, Math.Max(0, this.Height - _toolStrip.Height));
            };
        }

        public ContextMenuStrip TabContextMenuStrip
        {
            get => _contextMenu;
            set => _contextMenu = value;
        }

        /// <summary>
        /// Gets the original TabPage for the currently selected tab.
        /// This allows comparisons like "SelectedTab == tabStatus" to work.
        /// </summary>
        public TabPage SelectedTab
        {
            get
            {
                if (_selectedButton != null && _tabPages.ContainsKey(_selectedButton))
                {
                    var content = _tabPages[_selectedButton];
                    if (_originalTabPages.ContainsKey(content))
                        return _originalTabPages[content];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the TabPages collection for compatibility with code that iterates tabs.
        /// </summary>
        public IReadOnlyList<TabPage> TabPages => _tabPagesList;

        public int SelectedIndex
        {
            get
            {
                int index = 0;
                foreach (var kvp in _tabPages)
                {
                    if (kvp.Key == _selectedButton)
                        return index;
                    index++;
                }
                return -1;
            }
            set
            {
                int index = 0;
                foreach (var kvp in _tabPages)
                {
                    if (index == value)
                    {
                        SelectTab(kvp.Key);
                        return;
                    }
                    index++;
                }
            }
        }

        public void AddTab(string text, Control content)
        {
            var button = new ToolStripButton(text);
            button.DisplayStyle = ToolStripItemDisplayStyle.Text;
            button.ForeColor = ThemeManager.TextColor;
            button.Padding = new Padding(8, 4, 8, 4);
            button.Click += TabButton_Click;
            button.MouseUp += TabButton_MouseUp;

            // Don't change content's Dock - let it fill naturally
            content.Dock = DockStyle.Fill;
            content.Visible = false;

            _tabPages[button] = content;
            _toolStrip.Items.Add(button);

            // Suspend layout while adding
            _contentPanel.SuspendLayout();
            _contentPanel.Controls.Add(content);
            _contentPanel.ResumeLayout(false);

            System.Diagnostics.Debug.WriteLine($"AddTab: {text}, ContentPanel size after add: {_contentPanel.Size}");

            // Select first tab by default
            if (_selectedButton == null)
            {
                SelectTab(button);
            }
        }

        public void AddTab(TabPage tabPage)
        {
            // Create a panel to hold the tab page's controls
            // (TabPage can only be parented to TabControl, so we need a wrapper)
            var panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = tabPage.BackColor != System.Drawing.Color.Empty ? tabPage.BackColor : ThemeManager.BGColor;
            panel.Name = tabPage.Name + "_content";
            panel.Tag = tabPage;
            panel.Padding = tabPage.Padding;

            // Suspend layout during control transfer
            panel.SuspendLayout();

            // Move all controls from TabPage to Panel, preserving their properties
            var controls = new List<Control>();
            foreach (Control ctl in tabPage.Controls)
            {
                controls.Add(ctl);
            }

            tabPage.Controls.Clear();

            foreach (var ctl in controls)
            {
                panel.Controls.Add(ctl);
            }

            panel.ResumeLayout(true);

            // Track the original TabPage for compatibility
            _originalTabPages[panel] = tabPage;
            _tabPagesList.Add(tabPage);

            AddTab(tabPage.Text, panel);
        }

        private void TabButton_Click(object sender, EventArgs e)
        {
            SelectTab((ToolStripButton)sender);
        }

        private void TabButton_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && _contextMenu != null)
            {
                _contextMenu.Show(_toolStrip, e.Location);
            }
        }

        public void SelectTab(ToolStripButton button)
        {
            if (_selectedButton == button)
                return;

            // Deselect previous
            if (_selectedButton != null)
            {
                _selectedButton.Checked = false;
                _selectedButton.BackColor = Color.Transparent;
                if (_tabPages.ContainsKey(_selectedButton))
                    _tabPages[_selectedButton].Visible = false;
            }

            // Select new
            _selectedButton = button;
            if (_selectedButton != null)
            {
                _selectedButton.Checked = true;
                _selectedButton.BackColor = ThemeManager.ControlBGColor;
                if (_tabPages.ContainsKey(_selectedButton))
                {
                    var content = _tabPages[_selectedButton];
                    content.Visible = true;
                    content.BringToFront();

                    // Ensure child controls are visible and properly laid out
                    foreach (Control child in content.Controls)
                    {
                        child.Visible = true;
                    }

                    content.PerformLayout();
                    content.Invalidate(true);
                    content.Refresh();

                    System.Diagnostics.Debug.WriteLine($"SelectTab: {button.Text}, Content size={content.Size}, ContentPanel size={_contentPanel.Size}, Children={content.Controls.Count}, Parent={content.Parent?.Name}");
                }
            }

            SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
        }

        public void SelectTabByContent(Control content)
        {
            foreach (var kvp in _tabPages)
            {
                if (kvp.Value == content)
                {
                    SelectTab(kvp.Key);
                    return;
                }
            }
        }

        public Control GetTabContent(int index)
        {
            int i = 0;
            foreach (var kvp in _tabPages)
            {
                if (i == index)
                    return kvp.Value;
                i++;
            }
            return null;
        }

        public int TabCount => _tabPages.Count;

        /// <summary>
        /// Selects a tab by its original TabPage reference.
        /// </summary>
        public void SelectTabByTabPage(TabPage tabPage)
        {
            foreach (var kvp in _originalTabPages)
            {
                if (kvp.Value == tabPage)
                {
                    SelectTabByContent(kvp.Key);
                    return;
                }
            }
        }

        /// <summary>
        /// Clears all tabs.
        /// </summary>
        public void Clear()
        {
            _toolStrip.Items.Clear();
            _contentPanel.Controls.Clear();
            _tabPages.Clear();
            _originalTabPages.Clear();
            _tabPagesList.Clear();
            _selectedButton = null;
        }

        /// <summary>
        /// Gets the button for a specific tab page.
        /// </summary>
        public ToolStripButton GetButtonForTabPage(TabPage tabPage)
        {
            foreach (var kvp in _originalTabPages)
            {
                if (kvp.Value == tabPage)
                {
                    foreach (var bp in _tabPages)
                    {
                        if (bp.Value == kvp.Key)
                            return bp.Key;
                    }
                }
            }
            return null;
        }

        public void ApplyTheme()
        {
            _toolStrip.BackColor = ThemeManager.BGColor;
            _contentPanel.BackColor = ThemeManager.BGColor;

            foreach (var kvp in _tabPages)
            {
                kvp.Key.ForeColor = ThemeManager.TextColor;
                kvp.Value.BackColor = ThemeManager.BGColor;

                if (kvp.Key == _selectedButton)
                    kvp.Key.BackColor = ThemeManager.ControlBGColor;
                else
                    kvp.Key.BackColor = Color.Transparent;
            }

            _toolStrip.Invalidate();
        }
    }

    /// <summary>
    /// Custom renderer for themed ToolStrip
    /// </summary>
    public class ThemedToolStripRenderer : ToolStripProfessionalRenderer
    {
        public ThemedToolStripRenderer() : base(new ThemedColorTable()) { }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            var button = e.Item as ToolStripButton;
            if (button == null)
            {
                base.OnRenderButtonBackground(e);
                return;
            }

            var g = e.Graphics;
            var bounds = new Rectangle(Point.Empty, e.Item.Size);

            Color bgColor;
            if (button.Checked)
            {
                bgColor = ThemeManager.ControlBGColor;
            }
            else if (button.Selected)
            {
                bgColor = Color.FromArgb(60, ThemeManager.ControlBGColor);
            }
            else
            {
                bgColor = ThemeManager.BGColor;
            }

            using (var brush = new SolidBrush(bgColor))
            {
                g.FillRectangle(brush, bounds);
            }
        }

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            using (var brush = new SolidBrush(ThemeManager.BGColor))
            {
                e.Graphics.FillRectangle(brush, e.AffectedBounds);
            }
        }

        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
        {
            // Don't draw border
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = ThemeManager.TextColor;
            base.OnRenderItemText(e);
        }
    }

    public class ThemedColorTable : ProfessionalColorTable
    {
        public override Color ToolStripGradientBegin => ThemeManager.BGColor;
        public override Color ToolStripGradientMiddle => ThemeManager.BGColor;
        public override Color ToolStripGradientEnd => ThemeManager.BGColor;
        public override Color MenuStripGradientBegin => ThemeManager.BGColor;
        public override Color MenuStripGradientEnd => ThemeManager.BGColor;
        public override Color ToolStripBorder => ThemeManager.BGColor;
    }
}
