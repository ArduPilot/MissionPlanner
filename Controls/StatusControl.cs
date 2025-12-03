using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public class StatusControl : UserControl
    {
        private PlaceholderTextBox _searchBox;
        private Panel _contentPanel;
        private List<StatusItem> _allItems;
        private List<StatusItem> _filteredItems;
        private Timer _updateTimer;
        private Font _boldFont;
        private Font _normalFont;
        private int _rowHeight = 18;
        private int _padding = 10;
        private int _nameWidth = 130;
        private int _columnGap = 20;
        private List<int> _columnWidths = new List<int>();
        private int _totalWidth = 0;
        private bool _columnWidthsCalculated = false;
        private bool _valuesPopulated = false;

        public StatusControl()
        {
            InitializeComponent();
            _allItems = new List<StatusItem>();
            _filteredItems = new List<StatusItem>();

            _updateTimer = new Timer();
            _updateTimer.Interval = 100;
            _updateTimer.Tick += UpdateTimer_Tick;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Search box - same height as ConfigRawParams (26)
            _searchBox = new PlaceholderTextBox();
            _searchBox.Dock = DockStyle.Top;
            _searchBox.Font = new Font(this.Font.FontFamily, 10);
            _searchBox.PlaceholderText = "Search";
            _searchBox.PlaceholderColor = Color.Gray;
            _searchBox.TextChanged += SearchBox_TextChanged;
            _searchBox.MinimumSize = new Size(0, 28);
            _searchBox.Height = 28;

            // Content panel with horizontal scrolling only
            _contentPanel = new DoubleBufferedPanel();
            _contentPanel.Dock = DockStyle.Fill;
            _contentPanel.AutoScroll = true;
            _contentPanel.Paint += ContentPanel_Paint;

            // Add controls - order matters for docking
            this.Controls.Add(_contentPanel);
            this.Controls.Add(_searchBox);

            this.ResumeLayout(false);
        }

        public void Start()
        {
            BuildItemList();
            _updateTimer.Start();
        }

        public void Stop()
        {
            _updateTimer.Stop();
        }

        private void BuildItemList()
        {
            _allItems.Clear();
            var list = MainV2.comPort.MAV.cs.GetItemList(true);
            foreach (var field in list)
            {
                _allItems.Add(new StatusItem { Name = field, Value = "" });
            }
            _filteredItems = _allItems;

            // Set up autocomplete
            _searchBox.SetAutoCompleteSource(list);

            UpdateScrollSize();
            _contentPanel.Invalidate();
        }

        private void UpdateScrollSize()
        {
            if (_filteredItems.Count == 0)
            {
                _contentPanel.AutoScrollMinSize = Size.Empty;
                return;
            }

            // Only calculate column widths once values have been populated
            if (!_columnWidthsCalculated && _valuesPopulated)
            {
                CalculateColumnWidths();
            }

            // Use a default width if not yet calculated
            if (!_columnWidthsCalculated)
            {
                int availableHeight = _contentPanel.ClientSize.Height;
                int rowsPerColumn = Math.Max(1, (availableHeight - _padding * 2) / _rowHeight);
                int numColumns = (int)Math.Ceiling((double)_filteredItems.Count / rowsPerColumn);
                int defaultColumnWidth = _nameWidth + 100 + _columnGap;
                _contentPanel.AutoScrollMinSize = new Size(_padding + (numColumns * defaultColumnWidth) + _padding, 0);
                return;
            }

            // Only horizontal scrolling - set height to 0 to disable vertical scroll
            _contentPanel.AutoScrollMinSize = new Size(_totalWidth, 0);
        }

        private void CalculateColumnWidths()
        {
            if (_filteredItems.Count == 0)
                return;

            int availableHeight = _contentPanel.ClientSize.Height;
            if (availableHeight <= 0)
                return;

            int rowsPerColumn = Math.Max(1, (availableHeight - _padding * 2) / _rowHeight);

            _columnWidths.Clear();
            _totalWidth = _padding;

            if (_normalFont == null)
                _normalFont = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Regular);

            using (var g = _contentPanel.CreateGraphics())
            {
                int columnStart = 0;

                while (columnStart < _filteredItems.Count)
                {
                    int columnEnd = Math.Min(columnStart + rowsPerColumn, _filteredItems.Count);
                    float maxValueWidth = 50; // minimum value width

                    for (int i = columnStart; i < columnEnd; i++)
                    {
                        var valueSize = g.MeasureString(_filteredItems[i].Value ?? "", _normalFont);
                        maxValueWidth = Math.Max(maxValueWidth, valueSize.Width);
                    }

                    int columnWidth = _nameWidth + (int)maxValueWidth + _columnGap;
                    _columnWidths.Add(columnWidth);
                    _totalWidth += columnWidth;

                    columnStart = columnEnd;
                }
            }

            _totalWidth += _padding;
            _columnWidthsCalculated = true;
        }

        private void ContentPanel_Paint(object sender, PaintEventArgs e)
        {
            if (_filteredItems.Count == 0)
                return;

            if (_boldFont == null || _boldFont.FontFamily != this.Font.FontFamily)
            {
                _boldFont = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Bold);
                _normalFont = new Font(this.Font.FontFamily, this.Font.Size, FontStyle.Regular);
            }

            var g = e.Graphics;

            // Apply scroll offset
            g.TranslateTransform(_contentPanel.AutoScrollPosition.X, _contentPanel.AutoScrollPosition.Y);

            // Clear background
            g.Clear(_contentPanel.BackColor);

            int availableHeight = _contentPanel.ClientSize.Height;
            int rowsPerColumn = Math.Max(1, (availableHeight - _padding * 2) / _rowHeight);

            int x = _padding;
            int y = _padding;
            int itemIndex = 0;
            int columnIndex = 0;

            using (var nameBrush = new SolidBrush(Color.FromArgb(0x94, 0xC1, 0x1F))) // Green
            using (var valueBrush = new SolidBrush(_contentPanel.ForeColor))
            {
                foreach (var item in _filteredItems)
                {
                    // Draw property name in bold green
                    g.DrawString(item.Name, _boldFont, nameBrush, x, y);

                    // Draw value in normal font
                    g.DrawString(item.Value, _normalFont, valueBrush, x + _nameWidth, y);

                    itemIndex++;
                    y += _rowHeight;

                    // Move to next column when we've filled this one
                    if (itemIndex % rowsPerColumn == 0)
                    {
                        if (columnIndex < _columnWidths.Count)
                        {
                            x += _columnWidths[columnIndex];
                            columnIndex++;
                        }
                        y = _padding;
                    }
                }
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (!this.Visible)
                return;

            try
            {
                var cs = MainV2.comPort.MAV.cs;
                bool changed = false;

                foreach (var item in _allItems)
                {
                    var prop = typeof(CurrentState).GetProperty(item.Name);
                    if (prop != null)
                    {
                        var val = prop.GetValue(cs)?.ToString() ?? "";
                        if (item.Value != val)
                        {
                            item.Value = val;
                            changed = true;
                        }
                    }
                }

                if (changed)
                {
                    // Mark values as populated after first update and recalculate widths
                    if (!_valuesPopulated)
                    {
                        _valuesPopulated = true;
                        UpdateScrollSize();
                    }
                    _contentPanel.Invalidate();
                }
            }
            catch
            {
                // Ignore errors during update
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            var searchText = _searchBox.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(searchText))
            {
                _filteredItems = _allItems;
            }
            else
            {
                _filteredItems = _allItems.Where(x => x.Name.ToLower().Contains(searchText)).ToList();
            }

            // Recalculate column widths for filtered results
            _columnWidthsCalculated = false;
            UpdateScrollSize();
            _contentPanel.Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            // Recalculate column widths since rows per column changes
            _columnWidthsCalculated = false;
            UpdateScrollSize();
            _contentPanel.Invalidate();
        }

        public void ApplyTheme()
        {
            this.BackColor = ThemeManager.BGColor;
            _searchBox.ApplyTheme();

            _contentPanel.BackColor = ThemeManager.BGColor;
            _contentPanel.ForeColor = ThemeManager.TextColor;
            _contentPanel.Invalidate();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _updateTimer?.Stop();
                _updateTimer?.Dispose();
                _boldFont?.Dispose();
                _normalFont?.Dispose();
            }
            base.Dispose(disposing);
        }

        public class StatusItem
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        /// <summary>
        /// A Panel with double buffering enabled to prevent flickering
        /// </summary>
        private class DoubleBufferedPanel : Panel
        {
            public DoubleBufferedPanel()
            {
                this.DoubleBuffered = true;
                this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                              ControlStyles.UserPaint |
                              ControlStyles.OptimizedDoubleBuffer, true);
                this.UpdateStyles();
            }
        }
    }
}
