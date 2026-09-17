using log4net;
using MissionPlanner.Utilities;
using MissionPlanner.Utilities.Mission;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    /// <summary>
    /// Editor for mission segment and marker style rules. Stays on top of
    /// the main window so the user can interact with the map while editing.
    /// Supports loading/saving named presets and live preview.
    /// </summary>
    public partial class MissionStyleEditor : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        const string DefaultPresetName = "[Default]";

        BindingList<SegmentStyleRuleConfig> segmentRules;
        BindingList<MarkerStyleRuleConfig> markerRules;

        Dictionary<string, Uri> presetStyles = new Dictionary<string, Uri>()
        {
            { DefaultPresetName, null },
        };

        MissionStyleConfig backupConfig;
        MissionStyleConfig editingConfig;
        Action<MissionStyle> applyStyle;
        bool isLoading = true;
        bool isDirty;
        bool confirmed;

        /// <summary>
        /// Creates the editor. <paramref name="applyStyle"/> is called for live
        /// preview and on OK. On cancel, the original config is restored via
        /// the same callback.
        /// </summary>
        public MissionStyleEditor(MissionStyleConfig config, Action<MissionStyle> applyStyle)
        {
            InitializeComponent();

            rulePropertyEditor.PropertyValueChanged += (s, args) =>
            {
                isDirty = true;
                rulePropertyEditor.Refresh();
            };

            scanForPresets();

            foreach (var preset in presetStyles)
            {
                styleBox.Items.Add(preset.Key);
            }
            var presetFilename = Path.GetFileNameWithoutExtension(Settings.Instance["missionstyle", DefaultPresetName]);
            if (styleBox.Items.Contains(presetFilename))
            {
                styleBox.SelectedItem = presetFilename;
            }
            else
            {
                styleBox.SelectedIndex = 0;
            }

            this.backupConfig = config;
            this.editingConfig = config.Clone();
            this.applyStyle = applyStyle;
            UpdateRulesLists();

            isLoading = false;

            var selectedPreset = styleBox.SelectedItem?.ToString();
            if (selectedPreset != null && presetStyles.TryGetValue(selectedPreset, out var uri))
                saveButton.Enabled = (uri != null);
            else
                saveButton.Enabled = false;
        }

        private void UpdateRulesLists()
        {
            segmentRules = new BindingList<SegmentStyleRuleConfig>(editingConfig.SegmentRules);
            markerRules = new BindingList<MarkerStyleRuleConfig>(editingConfig.MarkerRules);
            segmentRuleListBox.DataSource = segmentRules;
            segmentRuleListBox.DisplayMember = "Description";
            segmentRuleListBox.SelectedIndex = -1;
            markerRuleListBox.DataSource = markerRules;
            markerRuleListBox.DisplayMember = "Description";
            markerRuleListBox.SelectedIndex = -1;

            rulePropertyEditor.SelectedObject = null;
        }

        private void scanForPresets()
        {
            var presetDir = Settings.GetUserDataDirectory();
            if (Directory.Exists(presetDir))
            {
                var files = Directory.GetFiles(presetDir, "*.mpmissionstyle");
                foreach (var file in files)
                {
                    var presetName = Path.GetFileNameWithoutExtension(file);
                    if (presetName == DefaultPresetName)
                    {
                        log.Warn($"Ignoring preset named '{DefaultPresetName}' (reserved).");
                        continue;
                    }
                    presetStyles[Path.GetFileNameWithoutExtension(file)] = new Uri(file);
                }
            }
        }

        private void ruleListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var senderBox = sender as ListBox;
            rulePropertyEditor.SelectedObject = senderBox.SelectedItem;
        }

        private void styleBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoading) return;

            var selectedPreset = styleBox.SelectedItem?.ToString();
            if (selectedPreset == null) return;
            if (presetStyles.ContainsKey(selectedPreset))
            {
                var presetUri = presetStyles[selectedPreset];
                editingConfig = MissionStyle.LoadConfig(presetUri?.LocalPath) ?? MissionStyleConfig.CreateDefault();
                applyStyle(new MissionStyle(editingConfig));
                saveButton.Enabled = (presetUri != null);
            }
            else
            {
                saveButton.Enabled = false;
                log.Error($"Selected preset '{selectedPreset}' not found in preset styles.");
            }
            UpdateRulesLists();
            isDirty = false;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            var selectedPreset = styleBox.SelectedItem?.ToString();
            if (selectedPreset == null) return;
            if (presetStyles.ContainsKey(selectedPreset))
            {
                var presetUri = presetStyles[selectedPreset];
                if (presetUri == null)
                {
                    log.Error("Cannot save over the default preset.");
                    return;
                }
                MissionStyle.SaveConfig(presetUri.LocalPath, editingConfig);
                isDirty = false;
            }
            else
            {
                log.Error($"Selected preset '{selectedPreset}' not found in preset styles.");
            }
        }

        private void saveAsButton_Click(object sender, EventArgs e)
        {
            TrySaveAs();
        }

        private bool TrySaveAs()
        {
            var saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Mission Style Files (*.mpmissionstyle)|*.mpmissionstyle";
            saveFileDialog.InitialDirectory = Settings.GetUserDataDirectory();

            while (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                var presetFilename = Path.GetFileNameWithoutExtension(saveFileDialog.FileName);
                if (presetFilename == DefaultPresetName)
                {
                    CustomMessageBox.Show(
                        $"The name \"{DefaultPresetName}\" is reserved.\nPlease choose a different name.",
                        "Reserved Name",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    continue;
                }

                MissionStyle.SaveConfig(saveFileDialog.FileName, editingConfig);

                // Refresh the presets list (suppress SelectedIndexChanged during rebuild)
                isLoading = true;
                styleBox.Items.Clear();
                presetStyles.Clear();
                presetStyles[DefaultPresetName] = null;
                scanForPresets();
                foreach (var preset in presetStyles)
                {
                    styleBox.Items.Add(preset.Key);
                }
                styleBox.SelectedItem = presetFilename;
                isLoading = false;
                saveButton.Enabled = true;
                isDirty = false;
                return true;
            }
            return false;
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            applyStyle(new MissionStyle(editingConfig));
        }

        private ListBox getActiveListBox()
        {
            if (ruleTabs.SelectedTab == segmentRuleTab)
            {
                return segmentRuleListBox;
            }
            else if (ruleTabs.SelectedTab == markerRuleTab)
            {
                return markerRuleListBox;
            }
            return null;
        }

        private void ruleMoveUpButton_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(-1);
        }

        private void ruleMoveDownButton_Click(object sender, EventArgs e)
        {
            MoveSelectedItem(1);
        }

        private void MoveSelectedItem(int direction)
        {
            var listBox = getActiveListBox();
            if (listBox == null)
                return;

            var list = listBox.DataSource as IList;
            if (list == null)
                return;

            int index = listBox.SelectedIndex;
            int newIndex = index + direction;

            if (index < 0 || newIndex < 0 || newIndex >= list.Count)
                return;

            var item = list[index];
            list.RemoveAt(index);
            list.Insert(newIndex, item);
            listBox.SelectedIndex = newIndex;
            isDirty = true;
        }

        private void ruleAddButton_Click(object sender, EventArgs e)
        {
            if (ruleTabs.SelectedTab == segmentRuleTab)
            {
                segmentRules.Add(new SegmentStyleRuleConfig { Description = "New Rule" });
                segmentRuleListBox.SelectedIndex = segmentRules.Count - 1;
                isDirty = true;
            }
            else if (ruleTabs.SelectedTab == markerRuleTab)
            {
                markerRules.Add(new MarkerStyleRuleConfig { Description = "New Rule" });
                markerRuleListBox.SelectedIndex = markerRules.Count - 1;
                isDirty = true;
            }
        }

        private void ruleDuplicateButton_Click(object sender, EventArgs e)
        {
            var listBox = getActiveListBox();
            if (listBox == null || listBox.SelectedIndex < 0)
                return;

            var list = listBox.DataSource as IList;
            if (list == null)
                return;

            var source = listBox.SelectedItem;
            var clone = Activator.CreateInstance(source.GetType());
            foreach (var prop in source.GetType().GetProperties())
            {
                if (prop.CanWrite)
                    prop.SetValue(clone, prop.GetValue(source));
            }
            list.Add(clone);
            listBox.SelectedIndex = list.Count - 1;
            isDirty = true;
        }

        private void ruleDeleteButton_Click(object sender, EventArgs e)
        {
            var listBox = getActiveListBox();
            if (listBox == null || listBox.SelectedIndex < 0)
                return;

            var list = listBox.DataSource as IList;
            if (list == null)
                return;

            list.RemoveAt(listBox.SelectedIndex);
            rulePropertyEditor.SelectedObject = null;
            isDirty = true;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            var selectedPreset = styleBox.SelectedItem?.ToString();
            if (selectedPreset != null && presetStyles.TryGetValue(selectedPreset, out var presetUri))
            {
                if (presetUri != null)
                {
                    MissionStyle.SaveConfig(presetUri.LocalPath, editingConfig);
                }
                else if (isDirty)
                {
                    CustomMessageBox.Show(
                        "Changes to the built-in preset must be saved as a new profile.",
                        "Save Required",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    if (!TrySaveAs())
                        return;
                }
            }
            var finalName = styleBox.SelectedItem?.ToString();
            if (finalName != null && presetStyles.TryGetValue(finalName, out var finalUri) && finalUri != null)
                Settings.Instance["missionstyle"] = Path.GetFileName(finalUri.LocalPath);
            else
                Settings.Instance["missionstyle"] = "";
            applyStyle(new MissionStyle(editingConfig));
            confirmed = true;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!confirmed)
                applyStyle(new MissionStyle(backupConfig));
            base.OnFormClosing(e);
        }
    }
}
