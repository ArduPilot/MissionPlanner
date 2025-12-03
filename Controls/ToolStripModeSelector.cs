using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    [Description("A mode selector control that displays the current flight mode and favorite modes.")]
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ToolStripComboBox))]
    public class ToolStripModeSelector : ToolStripControlHost
    {
        private Panel _container;
        private ModeDropdownButton _modeDropdown;
        private TableLayoutPanel _pinnedPanel;
        private string _lastMode = "";
        private bool _lastConnectionState = false;
        private List<string> _pinnedModes = new List<string>();
        private List<string> _favoriteModes = new List<string>();
        private Timer _updateTimer;
        private const string PinnedSettingsKey = "ModeSelectorPinned";
        private const string FavoritesSettingsKey = "ModeSelectorFavorites";
        private int _visiblePinnedCount = 0;
        private int _idealWidth = 0;
        private MenuStrip _parentMenuStrip = null;

        /// <summary>
        /// Gets the current mode text displayed.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [Description("The current flight mode displayed.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ModeText
        {
            get => _modeDropdown?.Text ?? "---";
        }

        /// <summary>
        /// Gets or sets the minimum width of the mode dropdown.
        /// </summary>
        [Browsable(true)]
        [Category("Layout")]
        [Description("The minimum width of the mode dropdown.")]
        [DefaultValue(100)]
        public int MinimumModeWidth { get; set; } = 100;

        private static Panel CreateControlPanel()
        {
            return new Panel();
        }

        public ToolStripModeSelector() : base(CreateControlPanel())
        {
            _container = (Panel)Control;
            _container.AutoSize = false;
            _container.Height = 32;
            _container.MinimumSize = new Size(100, 32);
            _container.BackColor = Color.Transparent;

            this.AutoSize = false;

            InitializeControls();

            if (!IsDesignMode())
            {
                // Start hidden - will show when vehicle connects
                this.Visible = false;
                _lastConnectionState = false;

                LoadSettings();
                StartUpdateTimer();
            }
            else
            {
                _container.Width = 120;
            }

            UpdateContainerWidth();
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            return new Size(_container.Width, _container.Height);
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            base.OnOwnerChanged(e);

            // Unsubscribe from old parent
            if (_parentMenuStrip != null)
            {
                _parentMenuStrip.Resize -= ParentMenuStrip_Resize;
            }

            // Subscribe to new parent's resize
            _parentMenuStrip = Owner as MenuStrip;
            if (_parentMenuStrip != null)
            {
                _parentMenuStrip.Resize += ParentMenuStrip_Resize;
            }
        }

        private void ParentMenuStrip_Resize(object sender, EventArgs e)
        {
            if (!this.Visible || _parentMenuStrip == null)
                return;

            AdjustWidthToAvailableSpace();
        }

        /// <summary>
        /// Calculates available space and adjusts the control width accordingly.
        /// Progressively hides pinned buttons when space is limited.
        /// </summary>
        private void AdjustWidthToAvailableSpace()
        {
            if (_parentMenuStrip == null || _modeDropdown == null)
                return;

            // Calculate space used by other items in the menu strip
            int otherItemsWidth = 0;
            foreach (ToolStripItem item in _parentMenuStrip.Items)
            {
                if (item != this && item.Visible)
                {
                    otherItemsWidth += item.Width + item.Margin.Horizontal;
                }
            }

            // Available space for mode selector (with some padding for safety)
            int availableWidth = _parentMenuStrip.Width - otherItemsWidth - 20;

            // Calculate dropdown width (always shown)
            int dropdownWidth = Math.Max(MinimumModeWidth, TextRenderer.MeasureText(_modeDropdown.Text, _modeDropdown.Font).Width + 40);

            // Calculate width needed for all pinned buttons
            var pinnedButtonWidths = new List<int>();
            foreach (var mode in _pinnedModes)
            {
                int btnWidth = TextRenderer.MeasureText(mode, new Font("Segoe UI", 9F)).Width + 40;
                pinnedButtonWidths.Add(btnWidth);
            }

            // Determine how many pinned buttons can fit
            int pinnedWidth = 0;
            int visiblePinned = 0;
            int basePadding = 12; // padding around dropdown

            for (int i = 0; i < pinnedButtonWidths.Count; i++)
            {
                int testWidth = basePadding + dropdownWidth + pinnedWidth + pinnedButtonWidths[i];
                if (testWidth <= availableWidth)
                {
                    pinnedWidth += pinnedButtonWidths[i];
                    visiblePinned++;
                }
                else
                {
                    break;
                }
            }

            // Only rebuild if visible count changed
            if (visiblePinned != _visiblePinnedCount)
            {
                _visiblePinnedCount = visiblePinned;
                UpdatePinnedButtons();
            }

            // Update width
            int totalWidth = basePadding + dropdownWidth + pinnedWidth;
            totalWidth = Math.Max(totalWidth, MinimumModeWidth + basePadding);

            if (_container.Width != totalWidth)
            {
                _container.Width = totalWidth;
                this.Size = new Size(totalWidth, _container.Height);
            }
        }

        private bool IsDesignMode()
        {
            return LicenseManager.UsageMode == LicenseUsageMode.Designtime ||
                   System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv";
        }

        private void InitializeControls()
        {
            var table = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(4, 1, 0, 1)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Dropdown
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Pinned buttons
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // Custom dropdown button for mode selection
            _modeDropdown = new ModeDropdownButton
            {
                Dock = DockStyle.Fill,
                Text = "---",
                Margin = new Padding(0, 0, 4, 0)
            };
            _modeDropdown.ModeSelected += ModeDropdown_ModeSelected;
            _modeDropdown.FavoriteToggled += ModeDropdown_FavoriteToggled;
            _modeDropdown.PinToggled += ModeDropdown_PinToggled;

            // Pinned modes panel (shows in toolbar)
            _pinnedPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                RowCount = 1,
                ColumnCount = 0,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(0)
            };
            _pinnedPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            table.Controls.Add(_modeDropdown, 0, 0);
            table.Controls.Add(_pinnedPanel, 1, 0);
            _container.Controls.Add(table);
        }

        private void ModeDropdown_ModeSelected(object sender, string mode)
        {
            SetMode(mode);
        }

        private void ModeDropdown_FavoriteToggled(object sender, string mode)
        {
            if (_favoriteModes.Contains(mode))
            {
                _favoriteModes.Remove(mode);
            }
            else
            {
                _favoriteModes.Add(mode);
            }
            SaveSettings();
            _modeDropdown.SetFavorites(_favoriteModes);
        }

        private void ModeDropdown_PinToggled(object sender, string mode)
        {
            if (_pinnedModes.Contains(mode))
            {
                _pinnedModes.Remove(mode);
            }
            else
            {
                _pinnedModes.Add(mode);
            }
            SaveSettings();
            UpdatePinnedButtons();
            _modeDropdown.SetPinned(_pinnedModes);
        }

        private void LoadSettings()
        {
            _pinnedModes.Clear();
            _favoriteModes.Clear();

            var savedPinned = Settings.Instance.GetString(PinnedSettingsKey, "");
            if (!string.IsNullOrEmpty(savedPinned))
            {
                _pinnedModes = savedPinned.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            }

            var savedFavorites = Settings.Instance.GetString(FavoritesSettingsKey, "");
            if (!string.IsNullOrEmpty(savedFavorites))
            {
                _favoriteModes = savedFavorites.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            }

            _modeDropdown.SetFavorites(_favoriteModes);
            _modeDropdown.SetPinned(_pinnedModes);
            UpdatePinnedButtons();
        }

        private void SaveSettings()
        {
            Settings.Instance[PinnedSettingsKey] = string.Join(",", _pinnedModes);
            Settings.Instance[FavoritesSettingsKey] = string.Join(",", _favoriteModes);
        }

        private void UpdatePinnedButtons()
        {
            _pinnedPanel.Controls.Clear();
            _pinnedPanel.ColumnStyles.Clear();

            // Determine how many pinned buttons to show
            // If _visiblePinnedCount is 0 and we haven't calculated yet, show all
            int maxVisible = _visiblePinnedCount > 0 || _parentMenuStrip != null
                ? _visiblePinnedCount
                : _pinnedModes.Count;

            var modesToShow = _pinnedModes.Take(maxVisible).ToList();
            _pinnedPanel.ColumnCount = modesToShow.Count;

            Color textColor, darkBgColor, greenColor;
            try
            {
                textColor = ThemeManager.TextColor;
                // Use a darker shade for pinned buttons
                var themeBg = ThemeManager.ControlBGColor;
                darkBgColor = Color.FromArgb(
                    Math.Max(0, themeBg.R - 20),
                    Math.Max(0, themeBg.G - 20),
                    Math.Max(0, themeBg.B - 20));
                greenColor = ThemeManager.ButBG;
            }
            catch
            {
                textColor = Color.White;
                darkBgColor = Color.FromArgb(0x32, 0x33, 0x34);
                greenColor = Color.FromArgb(148, 193, 31);
            }

            int col = 0;
            foreach (var mode in modesToShow)
            {
                string displayName = mode;
                int btnWidth = TextRenderer.MeasureText(displayName, new Font("Segoe UI", 9F)).Width + 32;
                _pinnedPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, btnWidth + 8));

                var btn = new Label
                {
                    Text = displayName,
                    AutoSize = false,
                    Font = new Font("Segoe UI", 9F),
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = textColor,
                    BackColor = darkBgColor,
                    Cursor = Cursors.Hand,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(4, 1, 4, 1),
                    Tag = mode // Keep original mode name for setting
                };

                var capturedDarkBg = darkBgColor;
                var capturedGreen = greenColor;
                btn.MouseEnter += (s, e) => {
                    btn.BackColor = capturedGreen;
                };
                btn.MouseLeave += (s, e) => {
                    btn.BackColor = capturedDarkBg;
                };

                btn.Click += PinnedButton_Click;
                _pinnedPanel.Controls.Add(btn, col, 0);
                col++;
            }

            UpdateContainerWidth();
        }

        private void UpdateContainerWidth()
        {
            if (_modeDropdown == null || _pinnedPanel == null)
                return;

            int dropdownWidth = Math.Max(MinimumModeWidth, TextRenderer.MeasureText(_modeDropdown.Text, _modeDropdown.Font).Width + 40);
            _modeDropdown.Width = dropdownWidth;

            int pinnedWidth = 0;
            foreach (ColumnStyle cs in _pinnedPanel.ColumnStyles)
            {
                if (cs.SizeType == SizeType.Absolute)
                    pinnedWidth += (int)cs.Width;
            }

            int padding = 4; // table left padding
            int totalWidth = padding + dropdownWidth + pinnedWidth + 8;

            _container.Width = totalWidth;
            this.Size = new Size(totalWidth, _container.Height);
        }

        private void PinnedButton_Click(object sender, EventArgs e)
        {
            if (sender is Label btn && btn.Tag is string mode)
            {
                SetMode(mode);
            }
        }

        private void SetMode(string mode)
        {
            try
            {
                if (MainV2.comPort?.MAV?.cs != null)
                {
                    if (MainV2.comPort.MAV.cs.failsafe)
                    {
                        if (CustomMessageBox.Show("You are in failsafe, are you sure?", "Failsafe",
                            MessageBoxButtons.YesNo) != (int)DialogResult.Yes)
                        {
                            return;
                        }
                    }
                    MainV2.comPort.setMode(mode);
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Failed to set mode: {ex.Message}", Strings.ERROR);
            }
        }

        private void StartUpdateTimer()
        {
            _updateTimer = new Timer();
            _updateTimer.Interval = 200;
            _updateTimer.Tick += UpdateTimer_Tick;
            _updateTimer.Start();
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                // Check connection state
                bool isConnected = IsVehicleConnected();

                // Update visibility if connection state changed
                if (isConnected != _lastConnectionState)
                {
                    _lastConnectionState = isConnected;

                    if (_container.InvokeRequired)
                    {
                        _container.BeginInvoke((Action)(() => UpdateVisibility(isConnected)));
                    }
                    else
                    {
                        UpdateVisibility(isConnected);
                    }
                }

                // Only update mode display if connected
                if (isConnected)
                {
                    string currentMode = MainV2.comPort?.MAV?.cs?.mode ?? "---";

                    if (currentMode != _lastMode)
                    {
                        _lastMode = currentMode;

                        if (_modeDropdown.InvokeRequired)
                        {
                            _modeDropdown.BeginInvoke((Action)(() => UpdateModeDisplay(currentMode)));
                        }
                        else
                        {
                            UpdateModeDisplay(currentMode);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// Checks if a vehicle is currently connected.
        /// </summary>
        private bool IsVehicleConnected()
        {
            try
            {
                // Check if we have an open connection
                if (MainV2.comPort?.BaseStream == null || !MainV2.comPort.BaseStream.IsOpen)
                    return false;

                // Also verify we have valid MAV data (not just serial connection)
                if (MainV2.comPort.MAV?.cs == null)
                    return false;

                // Check that we have received heartbeats (sysid > 0 indicates real vehicle)
                if (MainV2.comPort.MAV.sysid == 0)
                    return false;

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Updates the visibility of the mode selector based on connection state.
        /// </summary>
        private void UpdateVisibility(bool isConnected)
        {
            this.Visible = isConnected;

            if (isConnected)
            {
                // Recalculate width when becoming visible
                _visiblePinnedCount = _pinnedModes.Count; // Reset to show all initially
                UpdatePinnedButtons();
                AdjustWidthToAvailableSpace();
            }
            else
            {
                // Reset mode display when disconnected
                _lastMode = "";
                _modeDropdown.Text = "---";
                _modeDropdown.CurrentMode = "";
                // Close any open popup when disconnecting
                _modeDropdown.ClosePopup();
            }
        }

        private void UpdateModeDisplay(string mode)
        {
            _modeDropdown.Text = string.IsNullOrEmpty(mode) ? "---" : mode;
            _modeDropdown.CurrentMode = mode; // Keep original for comparison
            _modeDropdown.RefreshPopupIfOpen();
            UpdateContainerWidth();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _updateTimer?.Stop();
                _updateTimer?.Dispose();

                // Unsubscribe from parent resize
                if (_parentMenuStrip != null)
                {
                    _parentMenuStrip.Resize -= ParentMenuStrip_Resize;
                    _parentMenuStrip = null;
                }
            }
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// Custom dropdown button that shows current mode and opens a popup with mode list and favorites
    /// </summary>
    internal class ModeDropdownButton : Control
    {
        private bool _isHovered = false;
        private bool _isDropdownOpen = false;
        private ModeDropdownPopup _popup;
        private List<string> _favorites = new List<string>();
        private List<string> _pinned = new List<string>();
        private string _currentMode = "---";

        public string CurrentMode
        {
            get => _currentMode;
            set
            {
                if (_currentMode != value)
                {
                    _currentMode = value;
                    Invalidate();
                }
            }
        }

        public event EventHandler<string> ModeSelected;
        public event EventHandler<string> FavoriteToggled;
        public event EventHandler<string> PinToggled;

        public ModeDropdownButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Cursor = Cursors.Hand;
        }

        public override string Text
        {
            get => base.Text;
            set
            {
                if (base.Text != value)
                {
                    base.Text = value;
                    Invalidate();
                }
            }
        }

        public void SetFavorites(List<string> favorites)
        {
            _favorites = favorites.ToList();
        }

        public void SetPinned(List<string> pinned)
        {
            _pinned = pinned.ToList();
        }

        public void RefreshPopupIfOpen()
        {
            if (_isDropdownOpen && _popup != null && !_popup.IsDisposed)
            {
                _popup.CurrentMode = CurrentMode;
                _popup.RefreshCurrentMode();
            }
        }

        /// <summary>
        /// Closes the popup if it's open (used when disconnecting).
        /// </summary>
        public void ClosePopup()
        {
            if (_isDropdownOpen && _popup != null && !_popup.IsDisposed)
            {
                try
                {
                    _popup.Close();
                }
                catch { }
            }
            _isDropdownOpen = false;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _isHovered = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _isHovered = false;
            Invalidate();
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            ShowDropdown();
        }

        private void ShowDropdown()
        {
            if (_isDropdownOpen)
                return;

            _isDropdownOpen = true;

            _popup = new ModeDropdownPopup();
            _popup.SetFavorites(_favorites);
            _popup.SetPinned(_pinned);
            _popup.CurrentMode = CurrentMode;
            _popup.ModeSelected += (s, mode) => {
                ModeSelected?.Invoke(this, mode);
                _popup.Close();
            };
            _popup.FavoriteToggled += (s, mode) => {
                FavoriteToggled?.Invoke(this, mode);
                _popup.SetFavorites(_favorites);
            };
            _popup.PinToggled += (s, mode) => {
                PinToggled?.Invoke(this, mode);
                _popup.SetPinned(_pinned);
            };
            _popup.FormClosed += (s, args) => {
                _isDropdownOpen = false;
                Invalidate();
            };

            // Load modes
            try
            {
                var modesList = ArduPilot.Common.getModesList(MainV2.comPort?.MAV?.cs?.firmware ?? ArduPilot.Firmwares.ArduCopter2);
                _popup.SetModes(modesList);
            }
            catch { }

            var screenPoint = PointToScreen(new Point(0, Height));
            _popup.StartPosition = FormStartPosition.Manual;
            _popup.Location = screenPoint;
            _popup.Show(this.FindForm());

            // Force refresh after popup is shown to ensure colors are applied
            _popup.BeginInvoke((Action)(() => _popup.RefreshAfterShow()));
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Color bgColor, textColor, borderColor;
            try
            {
                bgColor = _isHovered || _isDropdownOpen ? ThemeManager.ButBG : ThemeManager.ControlBGColor;
                textColor = ThemeManager.TextColor;
                borderColor = ThemeManager.ButBG; // Green border
            }
            catch
            {
                bgColor = _isHovered || _isDropdownOpen ? Color.FromArgb(148, 193, 31) : Color.FromArgb(0x43, 0x44, 0x45);
                textColor = Color.White;
                borderColor = Color.FromArgb(148, 193, 31);
            }

            using (var bgBrush = new SolidBrush(bgColor))
            {
                e.Graphics.FillRectangle(bgBrush, ClientRectangle);
            }

            // Draw 1px green border
            using (var borderPen = new Pen(borderColor, 1))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            }

            // Draw text
            var textSize = TextRenderer.MeasureText(Text, Font);
            var textRect = new Rectangle(8, (Height - textSize.Height) / 2, Width - 24, textSize.Height);
            TextRenderer.DrawText(e.Graphics, Text, Font, textRect, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            // Draw dropdown arrow
            int arrowSize = 6;
            int arrowX = Width - 12;
            int arrowY = (Height - arrowSize / 2) / 2;
            var arrowPoints = new Point[]
            {
                new Point(arrowX - arrowSize / 2, arrowY),
                new Point(arrowX + arrowSize / 2, arrowY),
                new Point(arrowX, arrowY + arrowSize / 2)
            };
            using (var arrowBrush = new SolidBrush(textColor))
            {
                e.Graphics.FillPolygon(arrowBrush, arrowPoints);
            }
        }
    }

    /// <summary>
    /// Popup form that shows the list of modes with favorite stars
    /// </summary>
    internal class ModeDropdownPopup : Form
    {
        // Colors - initialized from theme in constructor
        private Color _textColor;
        private Color _bgColor;
        private Color _hoverColor;
        private Color _currentBgColor;
        private Color _goldColor;
        private Color _grayColor;
        private Color _greenColor;

        private Panel _scrollPanel;
        private FlowLayoutPanel _modesPanel;
        private List<string> _favorites = new List<string>();
        private List<string> _pinned = new List<string>();
        private List<KeyValuePair<int, string>> _modes = new List<KeyValuePair<int, string>>();

        public string CurrentMode { get; set; } = "";

        public event EventHandler<string> ModeSelected;
        public event EventHandler<string> FavoriteToggled;
        public event EventHandler<string> PinToggled;

        public ModeDropdownPopup()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.Manual;
            Size = new Size(240, 300);

            // Initialize colors from theme
            try
            {
                _textColor = ThemeManager.TextColor;
                _bgColor = ThemeManager.BGColor;
                _hoverColor = ThemeManager.ControlBGColor;
                _currentBgColor = ThemeManager.ButBG;
                _greenColor = ThemeManager.ButBG;
                _goldColor = Color.FromArgb(255, 215, 0); // Gold not in theme
                _grayColor = Color.FromArgb(128, 128, 128);
                BackColor = _bgColor;
            }
            catch
            {
                _textColor = Color.White;
                _bgColor = Color.FromArgb(0x26, 0x27, 0x28);
                _hoverColor = Color.FromArgb(0x43, 0x44, 0x45);
                _currentBgColor = Color.FromArgb(148, 193, 31);
                _greenColor = Color.FromArgb(148, 193, 31);
                _goldColor = Color.FromArgb(255, 215, 0);
                _grayColor = Color.FromArgb(128, 128, 128);
                BackColor = _bgColor;
            }

            // Scrollable container
            _scrollPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(4)
            };

            _modesPanel = new FlowLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Width = 220
            };

            _scrollPanel.Controls.Add(_modesPanel);
            Controls.Add(_scrollPanel);

            Deactivate += (s, e) => Close();
        }

        public void SetFavorites(List<string> favorites)
        {
            _favorites = favorites?.ToList() ?? new List<string>();
            RefreshModesList();
        }

        public void SetPinned(List<string> pinned)
        {
            _pinned = pinned?.ToList() ?? new List<string>();
            RefreshModesList();
        }

        public void SetModes(List<KeyValuePair<int, string>> modes)
        {
            _modes = modes ?? new List<KeyValuePair<int, string>>();
            RefreshModesList();
        }

        public void RefreshCurrentMode()
        {
            RefreshModesList();
        }

        public void RefreshAfterShow()
        {
            // Rebuild the list after the form is shown to ensure colors render properly
            RefreshModesList();
            _modesPanel.Invalidate(true);
            _scrollPanel.Invalidate(true);
            Invalidate(true);
        }

        private void RefreshModesList()
        {
            _modesPanel.SuspendLayout();
            _modesPanel.Controls.Clear();

            // Sort modes: favorites first (alphabetically), then the rest (alphabetically)
            var sortedModes = _modes
                .OrderByDescending(m => _favorites.Contains(m.Value))
                .ThenBy(m => m.Value)
                .ToList();

            foreach (var mode in sortedModes)
            {
                string displayName = mode.Value;

                var row = new Panel
                {
                    Width = 220,
                    Height = 28,
                    Margin = new Padding(0, 1, 0, 1),
                    Cursor = Cursors.Hand
                };

                bool isCurrent = mode.Value.Equals(CurrentMode, StringComparison.OrdinalIgnoreCase);
                bool isFavorite = _favorites.Contains(mode.Value);
                bool isPinned = _pinned.Contains(mode.Value);

                row.BackColor = isCurrent ? _currentBgColor : _bgColor;

                // Mode name label
                var modeLabel = new Label
                {
                    Text = displayName,
                    AutoSize = false,
                    Width = 140,
                    Height = 28,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = _textColor,
                    BackColor = Color.Transparent,
                    Font = isCurrent ? new Font("Segoe UI", 9F, FontStyle.Bold) : new Font("Segoe UI", 9F),
                    Padding = new Padding(8, 0, 0, 0),
                    Cursor = Cursors.Hand,
                    Tag = mode.Value // Keep original for mode setting
                };

                // Pin button - diamond indicator
                var pinLabel = new Label
                {
                    Text = isPinned ? "◆" : "◇",
                    AutoSize = false,
                    Width = 28,
                    Height = 28,
                    Left = 148,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = isPinned ? _greenColor : _grayColor,
                    BackColor = Color.Transparent,
                    Font = new Font("Segoe UI", 11F),
                    Cursor = Cursors.Hand,
                    Tag = mode.Value
                };

                // Star button (favorite)
                var starLabel = new Label
                {
                    Text = isFavorite ? "★" : "☆",
                    AutoSize = false,
                    Width = 28,
                    Height = 28,
                    Left = 180,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = isFavorite ? _goldColor : _grayColor,
                    BackColor = Color.Transparent,
                    Font = new Font("Segoe UI", 12F),
                    Cursor = Cursors.Hand,
                    Tag = mode.Value
                };

                // Capture values for closures
                bool isCurrentCaptured = isCurrent;
                bool isFavoriteCaptured = isFavorite;
                bool isPinnedCaptured = isPinned;

                // Capture colors for closures
                var hoverColor = _hoverColor;
                var bgColor = _bgColor;
                var greenColor = _greenColor;
                var grayColor = _grayColor;
                var goldColor = _goldColor;

                // Hover effects for row
                Action<bool> setHover = (hover) => {
                    if (!isCurrentCaptured)
                        row.BackColor = hover ? hoverColor : bgColor;
                };

                row.MouseEnter += (s, e) => setHover(true);
                row.MouseLeave += (s, e) => setHover(false);
                modeLabel.MouseEnter += (s, e) => setHover(true);
                modeLabel.MouseLeave += (s, e) => setHover(false);
                pinLabel.MouseEnter += (s, e) => {
                    setHover(true);
                    pinLabel.ForeColor = greenColor;
                };
                pinLabel.MouseLeave += (s, e) => {
                    setHover(false);
                    pinLabel.ForeColor = isPinnedCaptured ? greenColor : grayColor;
                };
                starLabel.MouseEnter += (s, e) => {
                    setHover(true);
                    starLabel.ForeColor = goldColor;
                };
                starLabel.MouseLeave += (s, e) => {
                    setHover(false);
                    starLabel.ForeColor = isFavoriteCaptured ? goldColor : grayColor;
                };

                // Click handlers
                row.Click += (s, e) => ModeSelected?.Invoke(this, mode.Value);
                modeLabel.Click += (s, e) => ModeSelected?.Invoke(this, mode.Value);
                pinLabel.Click += (s, e) => {
                    PinToggled?.Invoke(this, mode.Value);
                };
                starLabel.Click += (s, e) => {
                    FavoriteToggled?.Invoke(this, mode.Value);
                };

                row.Controls.Add(modeLabel);
                row.Controls.Add(pinLabel);
                row.Controls.Add(starLabel);
                _modesPanel.Controls.Add(row);
            }

            _modesPanel.ResumeLayout(true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw border
            using (var pen = new Pen(Color.FromArgb(80, 80, 80), 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
        }
    }
}
