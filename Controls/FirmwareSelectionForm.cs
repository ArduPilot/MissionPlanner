using MissionPlanner.ArduPilot;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Field = MissionPlanner.ArduPilot.FirmwareFilter.Field;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// Lets the user pick one firmware from several manifest matches by filtering on its attributes.
    /// </summary>
    public class FirmwareSelectionForm : Form
    {
        public const int MaxResults = 100;

        private readonly FirmwareFilter _filter;
        private readonly Dictionary<Field, ComboBox> _combos = new Dictionary<Field, ComboBox>();
        private readonly DataGridView _grid;
        private readonly Label _status;
        private readonly Button _upload;
        private bool _updating;

        public FirmwareSelectionForm(List<APFirmware.FirmwareInfo> fwitems, DeviceInfo? device = null)
        {
            _filter = new FirmwareFilter(fwitems);

            Text = Strings.FirmwareSelectionTitle;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(720, 520);
            MinimumSize = new Size(500, 360);
            MinimizeBox = false;
            ShowIcon = false;
            ShowInTaskbar = false;

            var knownDevice = device.HasValue && !string.IsNullOrEmpty(device.Value.board);

            if (knownDevice)
            {
                // a board in its bootloader reports "<platform>-BL"
                var platform = device.Value.board;
                if (platform.EndsWith("-BL", StringComparison.Ordinal))
                    platform = platform.Substring(0, platform.Length - 3);
                if (_filter.GetOptions(Field.Platform).Contains(platform))
                    _filter.Set(Field.Platform, platform);
            }

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 6,
                Padding = new Padding(8)
            };
            root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            root.Controls.Add(new Label
            {
                Text = Strings.FirmwareSelectionPrompt,
                AutoSize = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(3, 3, 3, 8)
            }, 0, 0);

            var filters = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                ColumnCount = 4
            };
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

            foreach (var field in FirmwareFilter.AllFields)
            {
                if (knownDevice && !ShowForKnownDevice(field))
                    continue;

                var combo = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    Dock = DockStyle.Fill,
                    Tag = field
                };
                combo.SelectedIndexChanged += Combo_SelectedIndexChanged;
                _combos[field] = combo;

                filters.Controls.Add(new Label
                {
                    Text = GetLabel(field),
                    AutoSize = true,
                    Anchor = AnchorStyles.Left,
                    Margin = new Padding(3, 6, 3, 3)
                });
                filters.Controls.Add(combo);
            }

            root.Controls.Add(filters, 0, 1);

            root.Controls.Add(new Label
            {
                Text = Strings.FirmwareSelectionPickFile,
                AutoSize = true,
                Dock = DockStyle.Fill,
                Margin = new Padding(3, 8, 3, 3)
            }, 0, 2);

            _grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _grid.Columns.Add("platform", Strings.FirmwareSelectionPlatform);
            _grid.Columns.Add("type", Strings.FirmwareSelectionVehicleType);
            _grid.Columns.Add("reltype", Strings.FirmwareSelectionReleaseType);
            _grid.Columns.Add("version", Strings.FirmwareSelectionVersion);
            _grid.Columns.Add("format", Strings.FirmwareSelectionFormat);
            _grid.Columns.Add("file", Strings.FirmwareSelectionFile);
            _grid.SelectionChanged += (sender, e) => UpdateUploadEnabled();
            _grid.CellDoubleClick += (sender, e) =>
            {
                if (e.RowIndex >= 0)
                    Accept();
            };
            root.Controls.Add(_grid, 0, 3);

            _status = new Label { AutoSize = true, Margin = new Padding(3, 6, 3, 3) };
            root.Controls.Add(_status, 0, 4);

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft
            };
            var cancel = new MyButton
            {
                Text = Strings.Cancel,
                DialogResult = DialogResult.Cancel,
                Size = new Size(110, 30)
            };
            _upload = new MyButton
            {
                Text = Strings.FirmwareSelectionUpload,
                Size = new Size(140, 30)
            };
            _upload.Click += (sender, e) => Accept();
            buttons.Controls.Add(cancel);
            buttons.Controls.Add(_upload);
            root.Controls.Add(buttons, 0, 5);

            Controls.Add(root);
            AcceptButton = _upload;
            CancelButton = cancel;

            RefreshAll();

            // the grid selects its first row when shown; only keep that when it is the only match
            Shown += (sender, e) =>
            {
                if (_grid.Rows.Count > 1)
                    _grid.ClearSelection();
                UpdateUploadEnabled();
            };

            ThemeManager.ApplyThemeTo(this);
        }

        /// <summary>
        /// The selected firmware url, or null if the user cancelled.
        /// </summary>
        public string FinalResult => SelectedFirmware?.Url?.AbsoluteUri;

        public APFirmware.FirmwareInfo SelectedFirmware { get; private set; }

        /// <summary>
        /// For a detected board only platform, and vehicle type/release type/version when there is a real choice, are shown.
        /// Vehicle type only has a choice on the "all options" path, which passes the whole manifest.
        /// </summary>
        private bool ShowForKnownDevice(Field field)
        {
            switch (field)
            {
                case Field.Platform:
                    return true;
                case Field.MavType:
                case Field.VersionType:
                case Field.Version:
                    return _filter.GetOptions(field).Count > 1;
                default:
                    return false;
            }
        }

        private static string GetLabel(Field field)
        {
            switch (field)
            {
                case Field.MavType:
                    return Strings.FirmwareSelectionVehicleType;
                case Field.VersionType:
                    return Strings.FirmwareSelectionReleaseType;
                case Field.Platform:
                    return Strings.FirmwareSelectionPlatform;
                case Field.Version:
                    return Strings.FirmwareSelectionVersion;
                case Field.Format:
                    return Strings.FirmwareSelectionFormat;
                case Field.UsbId:
                    return Strings.FirmwareSelectionUsbId;
                case Field.BoardId:
                    return Strings.FirmwareSelectionBoardId;
                case Field.BootloaderStr:
                    return Strings.FirmwareSelectionBootloaderId;
                default:
                    return field.ToString();
            }
        }

        private void Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;

            var combo = (ComboBox)sender;
            _filter.Set((Field)combo.Tag, combo.SelectedIndex > 0 ? (string)combo.SelectedItem : null);

            RefreshAll();
        }

        private void RefreshAll()
        {
            _updating = true;
            try
            {
                foreach (var item in _combos)
                {
                    var combo = item.Value;
                    var selected = _filter.Get(item.Key);
                    var options = _filter.GetOptions(item.Key);
                    // keep a selection that other filters have made unreachable, so it can still be cleared
                    if (selected != null && !options.Contains(selected))
                        options.Insert(0, selected);

                    combo.BeginUpdate();
                    combo.Items.Clear();
                    combo.Items.Add(Strings.FirmwareSelectionAny);
                    combo.Items.AddRange(options.Cast<object>().ToArray());
                    combo.SelectedIndex = selected == null ? 0 : combo.Items.IndexOf(selected);
                    combo.EndUpdate();
                }
            }
            finally
            {
                _updating = false;
            }

            RefreshGrid();
        }

        private void RefreshGrid()
        {
            var matches = _filter.GetMatches();

            _grid.Rows.Clear();

            if (matches.Count == 0)
            {
                _status.Text = Strings.FirmwareSelectionNoMatches;
            }
            else if (matches.Count > MaxResults)
            {
                _status.Text = string.Format(Strings.FirmwareSelectionTooMany, matches.Count);
            }
            else
            {
                _status.Text = "";
                foreach (var fw in matches)
                {
                    var file = fw.Url == null ? "" : Path.GetFileName(fw.Url.AbsolutePath);
                    var index = _grid.Rows.Add(fw.Platform, fw.MavType, fw.MavFirmwareVersionType,
                        fw.MavFirmwareVersion?.ToString(), fw.Format, file);
                    var row = _grid.Rows[index];
                    row.Tag = fw;
                    row.Cells["file"].ToolTipText = fw.Url?.AbsoluteUri;
                }
            }

            _grid.ClearSelection();
            if (_grid.Rows.Count == 1)
                _grid.Rows[0].Selected = true;

            UpdateUploadEnabled();
        }

        private APFirmware.FirmwareInfo GetSelectedRow()
        {
            if (_grid.SelectedRows.Count != 1)
                return null;
            return _grid.SelectedRows[0].Tag as APFirmware.FirmwareInfo;
        }

        private void UpdateUploadEnabled()
        {
            _upload.Enabled = GetSelectedRow() != null;
        }

        private void Accept()
        {
            var fw = GetSelectedRow();
            if (fw?.Url == null)
                return;

            SelectedFirmware = fw;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
