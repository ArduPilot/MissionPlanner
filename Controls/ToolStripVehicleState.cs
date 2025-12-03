using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.StatusStrip)]
    [Description("A vehicle state control that displays arm state, flight mode, and pinned modes.")]
    [ToolboxItem(true)]
    [ToolboxBitmap(typeof(ToolStripComboBox))]
    public class ToolStripVehicleState : ToolStripControlHost
    {
        private Panel _container;
        private ArmIndicatorButton _armButton;
        private ModeDropdownButton _modeDropdown;
        private TableLayoutPanel _pinnedPanel;
        private Panel _gpsPanel;
        private PictureBox _gpsIcon;
        private Label _gpsSatsLabel;
        private Label _gpsDopLabel;
        private GpsStatusPopup _gpsPopup;
        private string _lastMode = "";
        private bool _lastArmedState = false;
        private bool _lastConnectionState = false;
        private float _lastSatCount = -1;
        private float _lastGpsStatus = -1;
        private float _lastHdop = -1;
        private float _lastVdop = -1;
        private List<string> _pinnedModes = new List<string>();
        private List<string> _favoriteModes = new List<string>();
        private Timer _updateTimer;
        private const string PinnedSettingsKey = "ModeSelectorPinned";
        private const string FavoritesSettingsKey = "ModeSelectorFavorites";
        private int _visiblePinnedCount = 0;
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
        public int MinimumModeWidth { get; set; } = 0;

        private static Panel CreateControlPanel()
        {
            return new Panel();
        }

        public ToolStripVehicleState() : base(CreateControlPanel())
        {
            _container = (Panel)Control;
            _container.AutoSize = false;
            _container.Height = 32;
            _container.MinimumSize = new Size(180, 32);
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
                _container.Width = 200;
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
        /// Progressively hides pinned buttons and GPS panel when space is limited.
        /// Logo and connection controls take priority.
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

            // Available space for this control (with some padding for safety)
            int availableWidth = _parentMenuStrip.Width - otherItemsWidth - 20;

            // If no space available, hide entirely
            if (availableWidth <= 0)
            {
                if (_container.Width != 0)
                {
                    _container.Width = 0;
                    this.Size = new Size(0, _container.Height);
                }
                return;
            }

            // Calculate arm button width
            int armWidth = TextRenderer.MeasureText(_armButton.Text, _armButton.Font).Width + 24;

            // Calculate dropdown width (always shown)
            int dropdownWidth = Math.Max(MinimumModeWidth, TextRenderer.MeasureText(_modeDropdown.Text, _modeDropdown.Font).Width + 24);

            // Calculate GPS panel width
            int gpsWidth = 32; // icon width + margin
            if (_gpsSatsLabel != null && _gpsDopLabel != null)
            {
                int satsWidth = TextRenderer.MeasureText(_gpsSatsLabel.Text, _gpsSatsLabel.Font).Width;
                int dopWidth = TextRenderer.MeasureText(_gpsDopLabel.Text, _gpsDopLabel.Font).Width;
                gpsWidth += Math.Max(satsWidth, dopWidth) + 8;
            }

            int basePadding = 16; // padding around controls

            // Minimum required: arm button + dropdown
            int minRequired = basePadding + armWidth + dropdownWidth;

            // Calculate width needed for all pinned buttons
            var pinnedButtonWidths = new List<int>();
            foreach (var mode in _pinnedModes)
            {
                int btnWidth = TextRenderer.MeasureText(mode, new Font("Segoe UI", 9F)).Width + 20;
                pinnedButtonWidths.Add(btnWidth);
            }

            // Determine what fits: first pinned buttons, then GPS if there's room
            int pinnedWidth = 0;
            int visiblePinned = 0;
            bool showGps = false;

            // Check if GPS fits with just arm + dropdown
            if (availableWidth >= minRequired + gpsWidth)
            {
                showGps = true;

                // Now try to fit pinned buttons
                for (int i = 0; i < pinnedButtonWidths.Count; i++)
                {
                    int testWidth = minRequired + pinnedWidth + pinnedButtonWidths[i] + gpsWidth;
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
            }
            else if (availableWidth >= minRequired)
            {
                // Not enough room for GPS, just show arm + dropdown
                showGps = false;
            }
            else
            {
                // Not even enough for arm + dropdown, show compressed
                showGps = false;
            }

            // Show/hide GPS panel
            if (_gpsPanel != null && _gpsPanel.Visible != showGps)
            {
                _gpsPanel.Visible = showGps;
            }

            // Only rebuild if visible count changed
            if (visiblePinned != _visiblePinnedCount)
            {
                _visiblePinnedCount = visiblePinned;
                UpdatePinnedButtons();
            }

            // Update width
            int totalWidth = minRequired + pinnedWidth + (showGps ? gpsWidth : 0);
            totalWidth = Math.Max(totalWidth, Math.Min(availableWidth, minRequired)); // Don't exceed available, but at least show arm+dropdown

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
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent,
                Margin = new Padding(0),
                Padding = new Padding(4, 1, 0, 1)
            };
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Arm button
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Dropdown
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // Pinned buttons
            table.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize)); // GPS status
            table.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // Arm/Disarm button
            _armButton = new ArmIndicatorButton
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 8, 0),
                Width = 80
            };
            _armButton.Click += ArmButton_Click;

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

            // GPS status panel
            _gpsPanel = new Panel
            {
                AutoSize = true,
                BackColor = Color.Transparent,
                Margin = new Padding(8, 0, 0, 0),
                Padding = new Padding(0)
            };

            // GPS icon
            _gpsIcon = new PictureBox
            {
                Width = 28,
                Height = 28,
                Location = new Point(0, 2),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };

            // Try to load GPS icon
            try
            {
                _gpsIcon.Image = MissionPlanner.Properties.Resources.gps;
            }
            catch
            {
                // Icon not found
            }

            Color textColor;
            try
            {
                textColor = ThemeManager.TextColor;
            }
            catch
            {
                textColor = Color.White;
            }

            // Top label: Sats: X: <fix type>
            _gpsSatsLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8F),
                ForeColor = textColor,
                BackColor = Color.Transparent,
                Text = "Sats: --: ---",
                Location = new Point(32, 0),
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            // Bottom label: H: <hdop> | V: <vdop>
            _gpsDopLabel = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 8F),
                ForeColor = textColor,
                BackColor = Color.Transparent,
                Text = "H: -- | V: --",
                Location = new Point(32, 16),
                Margin = new Padding(0),
                Padding = new Padding(0)
            };

            _gpsPanel.Controls.Add(_gpsIcon);
            _gpsPanel.Controls.Add(_gpsSatsLabel);
            _gpsPanel.Controls.Add(_gpsDopLabel);

            // Add click handlers for GPS popup
            _gpsPanel.Click += GpsPanel_Click;
            _gpsIcon.Click += GpsPanel_Click;
            _gpsSatsLabel.Click += GpsPanel_Click;
            _gpsDopLabel.Click += GpsPanel_Click;
            _gpsPanel.Cursor = Cursors.Hand;
            _gpsIcon.Cursor = Cursors.Hand;
            _gpsSatsLabel.Cursor = Cursors.Hand;
            _gpsDopLabel.Cursor = Cursors.Hand;

            // Set initial GPS panel width
            _gpsPanel.Width = 120;

            table.Controls.Add(_armButton, 0, 0);
            table.Controls.Add(_modeDropdown, 1, 0);
            table.Controls.Add(_pinnedPanel, 2, 0);
            table.Controls.Add(_gpsPanel, 3, 0);
            _container.Controls.Add(table);
        }

        private void ArmButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (MainV2.comPort?.BaseStream == null || !MainV2.comPort.BaseStream.IsOpen)
                    return;

                bool isArmed = MainV2.comPort.MAV?.cs?.armed ?? false;
                string action = isArmed ? "Disarm" : "Arm";

                // Show confirmation dialog when arming (with "show me again" option)
                if (!isArmed)
                {
                    var confirmResult = Common.MessageShowAgain(
                        "Arm Vehicle",
                        "Please confirm you'd like to ARM the vehicle.",
                        true,
                        "ArmVehicleConfirmation");

                    if (confirmResult != System.Windows.Forms.DialogResult.OK)
                        return;
                }

                StringBuilder sb = new StringBuilder();
                var sub = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.STATUSTEXT, message =>
                {
                    sb.AppendLine(Encoding.ASCII.GetString(((MAVLink.mavlink_statustext_t)message.data).text).TrimEnd('\0'));
                    return true;
                }, (byte)MainV2.comPort.sysidcurrent, (byte)MainV2.comPort.compidcurrent);

                bool result = MainV2.comPort.doARM(!isArmed);
                MainV2.comPort.UnSubscribeToPacketType(sub);

                if (!result)
                {
                    if (CustomMessageBox.Show(
                            action + " failed.\n" + sb.ToString() + "\nForce " + action +
                            " can bypass safety checks,\nwhich can lead to the vehicle crashing\nand causing serious injuries.\n\nDo you wish to Force " +
                            action + "?", Strings.ERROR, CustomMessageBox.MessageBoxButtons.YesNo,
                            CustomMessageBox.MessageBoxIcon.Exclamation, "Force " + action, "Cancel") ==
                        CustomMessageBox.DialogResult.Yes)
                    {
                        result = MainV2.comPort.doARM(!isArmed, true);
                        if (!result)
                        {
                            CustomMessageBox.Show(Strings.ErrorRejectedByMAV, Strings.ERROR);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show($"Failed to arm/disarm: {ex.Message}", Strings.ERROR);
            }
        }

        private void GpsPanel_Click(object sender, EventArgs e)
        {
            // Close existing popup if open
            if (_gpsPopup != null && !_gpsPopup.IsDisposed)
            {
                _gpsPopup.Close();
                _gpsPopup = null;
                return;
            }

            // Get current GPS data
            var cs = MainV2.comPort?.MAV?.cs;
            if (cs == null)
                return;

            _gpsPopup = new GpsStatusPopup();
            _gpsPopup.UpdateValues(
                (int)cs.satcount,
                GetFixTypeString((int)cs.gpsstatus),
                cs.gpshdop,
                cs.gpsvdop,
                cs.groundcourse
            );

            _gpsPopup.FormClosed += (s, args) => {
                _gpsPopup = null;
            };

            // Position below the GPS panel
            var screenPoint = _gpsPanel.PointToScreen(new Point(0, _gpsPanel.Height));
            _gpsPopup.StartPosition = FormStartPosition.Manual;
            _gpsPopup.Location = screenPoint;
            _gpsPopup.Show(_gpsPanel.FindForm());
        }

        private void UpdateArmButtonAppearance(bool isArmed)
        {
            if (_armButton == null)
                return;

            _armButton.IsArmed = isArmed;

            // Update arm button width
            int armWidth = TextRenderer.MeasureText(_armButton.Text, _armButton.Font).Width + 24;
            _armButton.Width = armWidth;
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
            // Reset visible count to show all, then recalculate
            _visiblePinnedCount = _pinnedModes.Count;
            UpdatePinnedButtons();
            AdjustWidthToAvailableSpace();
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
                int btnWidth = TextRenderer.MeasureText(displayName, new Font("Segoe UI", 9F)).Width + 16;
                _pinnedPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, btnWidth + 4));

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
                    Margin = new Padding(2, 1, 2, 1),
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
            if (_armButton == null || _modeDropdown == null || _pinnedPanel == null || _gpsPanel == null)
                return;

            int armWidth = TextRenderer.MeasureText(_armButton.Text, _armButton.Font).Width + 24;
            _armButton.Width = armWidth;

            int dropdownWidth = Math.Max(MinimumModeWidth, TextRenderer.MeasureText(_modeDropdown.Text, _modeDropdown.Font).Width + 24);
            _modeDropdown.Width = dropdownWidth;

            int pinnedWidth = 0;
            foreach (ColumnStyle cs in _pinnedPanel.ColumnStyles)
            {
                if (cs.SizeType == SizeType.Absolute)
                    pinnedWidth += (int)cs.Width;
            }

            // Calculate GPS panel width based on label content
            int gpsWidth = 32; // icon width + margin
            if (_gpsSatsLabel != null && _gpsDopLabel != null)
            {
                int satsWidth = TextRenderer.MeasureText(_gpsSatsLabel.Text, _gpsSatsLabel.Font).Width;
                int dopWidth = TextRenderer.MeasureText(_gpsDopLabel.Text, _gpsDopLabel.Font).Width;
                gpsWidth += Math.Max(satsWidth, dopWidth) + 8; // margin for GPS panel
            }
            _gpsPanel.Width = gpsWidth;

            int padding = 8; // table padding
            int totalWidth = padding + armWidth + dropdownWidth + pinnedWidth + gpsWidth + 8;

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

                // Only update state if connected
                if (isConnected)
                {
                    var cs = MainV2.comPort?.MAV?.cs;

                    // Update arm state
                    bool isArmed = cs?.armed ?? false;
                    if (isArmed != _lastArmedState)
                    {
                        _lastArmedState = isArmed;

                        if (_armButton.InvokeRequired)
                        {
                            _armButton.BeginInvoke((Action)(() => {
                                UpdateArmButtonAppearance(isArmed);
                                UpdateContainerWidth();
                            }));
                        }
                        else
                        {
                            UpdateArmButtonAppearance(isArmed);
                            UpdateContainerWidth();
                        }
                    }

                    // Update mode
                    string currentMode = cs?.mode ?? "---";

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

                    // Update GPS status
                    if (cs != null)
                    {
                        float satCount = cs.satcount;
                        float gpsStatus = cs.gpsstatus;
                        float hdop = cs.gpshdop;
                        float vdop = cs.gpsvdop;

                        if (satCount != _lastSatCount || gpsStatus != _lastGpsStatus ||
                            hdop != _lastHdop || vdop != _lastVdop)
                        {
                            _lastSatCount = satCount;
                            _lastGpsStatus = gpsStatus;
                            _lastHdop = hdop;
                            _lastVdop = vdop;

                            if (_gpsSatsLabel.InvokeRequired)
                            {
                                _gpsSatsLabel.BeginInvoke((Action)(() => UpdateGpsDisplay(satCount, gpsStatus, hdop, vdop)));
                            }
                            else
                            {
                                UpdateGpsDisplay(satCount, gpsStatus, hdop, vdop);
                            }
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
        /// Updates the visibility of the control based on connection state.
        /// </summary>
        private void UpdateVisibility(bool isConnected)
        {
            this.Visible = isConnected;

            if (isConnected)
            {
                // Update arm state
                bool isArmed = MainV2.comPort?.MAV?.cs?.armed ?? false;
                _lastArmedState = isArmed;
                UpdateArmButtonAppearance(isArmed);

                // Recalculate width when becoming visible
                _visiblePinnedCount = _pinnedModes.Count; // Reset to show all initially
                UpdatePinnedButtons();
                AdjustWidthToAvailableSpace();
            }
            else
            {
                // Reset state when disconnected
                _lastMode = "";
                _lastArmedState = false;
                _lastSatCount = -1;
                _lastGpsStatus = -1;
                _lastHdop = -1;
                _lastVdop = -1;
                _modeDropdown.Text = "---";
                _modeDropdown.CurrentMode = "";
                UpdateArmButtonAppearance(false);
                // Reset GPS display
                _gpsSatsLabel.Text = "Sats: --: ---";
                _gpsDopLabel.Text = "H: -- | V: --";
                try { _gpsSatsLabel.ForeColor = ThemeManager.TextColor; } catch { _gpsSatsLabel.ForeColor = Color.White; }
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

        private void UpdateGpsDisplay(float satCount, float gpsStatus, float hdop, float vdop)
        {
            string fixType = GetFixTypeString((int)gpsStatus);
            _gpsSatsLabel.Text = $"Sats: {satCount:0}: {fixType}";
            _gpsDopLabel.Text = $"H: {hdop:0.0} | V: {vdop:0.0}";

            // Update label color based on fix quality
            _gpsSatsLabel.ForeColor = GetFixColor((int)gpsStatus);

            UpdateContainerWidth();
        }

        private string GetFixTypeString(int fixType)
        {
            switch (fixType)
            {
                case 0: return "No GPS";
                case 1: return "No Fix";
                case 2: return "2D Fix";
                case 3: return "3D Fix";
                case 4: return "DGPS";
                case 5: return "RTK Float";
                case 6: return "RTK Fixed";
                default: return fixType.ToString();
            }
        }

        private Color GetFixColor(int fixType)
        {
            switch (fixType)
            {
                case 0:
                case 1:
                    return Color.Red;
                case 2:
                    return Color.Orange;
                case 3:
                case 4:
                    try { return ThemeManager.TextColor; } catch { return Color.White; }
                case 5:
                    return Color.FromArgb(0, 200, 255); // Cyan for RTK Float
                case 6:
                    return Color.FromArgb(0, 255, 100); // Green for RTK Fixed
                default:
                    try { return ThemeManager.TextColor; } catch { return Color.White; }
            }
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
            var textRect = new Rectangle(4, (Height - textSize.Height) / 2, Width - 16, textSize.Height);
            TextRenderer.DrawText(e.Graphics, Text, Font, textRect, textColor, TextFormatFlags.Left | TextFormatFlags.VerticalCenter);

            // Draw dropdown arrow
            int arrowSize = 6;
            int arrowX = Width - 8;
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

    /// <summary>
    /// Custom button for arm/disarm indicator with red border
    /// </summary>
    internal class ArmIndicatorButton : Control
    {
        private bool _isHovered = false;
        private bool _isArmed = false;
        private static readonly Color RedBorder = Color.FromArgb(200, 35, 51);

        public bool IsArmed
        {
            get => _isArmed;
            set
            {
                if (_isArmed != value)
                {
                    _isArmed = value;
                    Text = _isArmed ? "ARMED" : "DISARMED";
                    Invalidate();
                }
            }
        }

        public ArmIndicatorButton()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            Cursor = Cursors.Hand;
            Text = "DISARMED";
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Color bgColor, textColor;

            if (_isArmed)
            {
                // Armed: Red background, white text
                bgColor = _isHovered ? Color.FromArgb(220, 53, 69) : Color.FromArgb(200, 35, 51);
                textColor = Color.White;
            }
            else
            {
                // Disarmed: Gray background like pinned buttons
                try
                {
                    var themeBg = ThemeManager.ControlBGColor;
                    bgColor = _isHovered
                        ? ThemeManager.ButBG
                        : Color.FromArgb(
                            Math.Max(0, themeBg.R - 20),
                            Math.Max(0, themeBg.G - 20),
                            Math.Max(0, themeBg.B - 20));
                    textColor = ThemeManager.TextColor;
                }
                catch
                {
                    bgColor = _isHovered ? Color.FromArgb(148, 193, 31) : Color.FromArgb(0x32, 0x33, 0x34);
                    textColor = Color.White;
                }
            }

            // Fill background
            using (var bgBrush = new SolidBrush(bgColor))
            {
                e.Graphics.FillRectangle(bgBrush, ClientRectangle);
            }

            // Draw 1px red border (always)
            using (var borderPen = new Pen(RedBorder, 1))
            {
                e.Graphics.DrawRectangle(borderPen, 0, 0, Width - 1, Height - 1);
            }

            // Draw text centered
            var textSize = TextRenderer.MeasureText(Text, Font);
            var textRect = new Rectangle(0, (Height - textSize.Height) / 2, Width, textSize.Height);
            TextRenderer.DrawText(e.Graphics, Text, Font, textRect, textColor, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }

    /// <summary>
    /// Popup form displaying detailed GPS status information.
    /// </summary>
    internal class GpsStatusPopup : Form
    {
        private Label _satCountLabel;
        private Label _gpsLockLabel;
        private Label _hdopLabel;
        private Label _vdopLabel;
        private Label _courseLabel;

        public GpsStatusPopup()
        {
            FormBorderStyle = FormBorderStyle.None;
            ShowInTaskbar = false;
            TopMost = true;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(1); // Leave room for border

            Color bgColor, textColor;
            try
            {
                bgColor = ThemeManager.BGColor;
                textColor = ThemeManager.TextColor;
            }
            catch
            {
                bgColor = Color.FromArgb(45, 45, 48);
                textColor = Color.White;
            }

            BackColor = bgColor;

            var mainPanel = new TableLayoutPanel
            {
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 1,
                RowCount = 5,
                Margin = new Padding(0),
                Padding = new Padding(12),
                BackColor = bgColor,
                Location = new Point(1, 1) // Offset for border
            };

            // GPS Count
            _satCountLabel = new Label
            {
                Text = "GPS Count: --",
                Font = new Font("Segoe UI", 9F),
                ForeColor = textColor,
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 1)
            };

            // GPS Lock
            _gpsLockLabel = new Label
            {
                Text = "GPS Lock: --",
                Font = new Font("Segoe UI", 9F),
                ForeColor = textColor,
                AutoSize = true,
                Margin = new Padding(0, 1, 0, 1)
            };

            // HDOP
            _hdopLabel = new Label
            {
                Text = "HDOP: --",
                Font = new Font("Segoe UI", 9F),
                ForeColor = textColor,
                AutoSize = true,
                Margin = new Padding(0, 1, 0, 1)
            };

            // VDOP
            _vdopLabel = new Label
            {
                Text = "VDOP: --",
                Font = new Font("Segoe UI", 9F),
                ForeColor = textColor,
                AutoSize = true,
                Margin = new Padding(0, 1, 0, 1)
            };

            // Course
            _courseLabel = new Label
            {
                Text = "Course: --",
                Font = new Font("Segoe UI", 9F),
                ForeColor = textColor,
                AutoSize = true,
                Margin = new Padding(0, 1, 0, 0)
            };

            mainPanel.Controls.Add(_satCountLabel, 0, 0);
            mainPanel.Controls.Add(_gpsLockLabel, 0, 1);
            mainPanel.Controls.Add(_hdopLabel, 0, 2);
            mainPanel.Controls.Add(_vdopLabel, 0, 3);
            mainPanel.Controls.Add(_courseLabel, 0, 4);

            Controls.Add(mainPanel);

            // Close when clicking outside or losing focus
            Deactivate += (s, e) => Close();
        }

        public void UpdateValues(int satCount, string gpsLock, float hdop, float vdop, float course)
        {
            _satCountLabel.Text = $"GPS Count: {satCount}";
            _gpsLockLabel.Text = $"GPS Lock: {gpsLock}";
            _hdopLabel.Text = $"HDOP: {hdop:0.00}";
            _vdopLabel.Text = $"VDOP: {vdop:0.00}";
            _courseLabel.Text = $"Course: {course:0.0}°";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // Draw border
            Color borderColor;
            try
            {
                borderColor = ThemeManager.ButBG;
            }
            catch
            {
                borderColor = Color.FromArgb(0x94, 0xC1, 0x1F);
            }

            using (var pen = new Pen(borderColor, 1))
            {
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
            }
        }
    }
}
