using MissionPlanner.Controls;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace RedundantLinkManager
{
    public partial class RedundantLinkManager : Form
    {
        private readonly RedundantLinkManager_Plugin Plugin;

        private bool initializing = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="plugin">Reference to the plugin that hosts this form</param>
        public RedundantLinkManager(RedundantLinkManager_Plugin plugin)
        {
            Plugin = plugin;
            InitializeComponent();
            grid_links.AutoGenerateColumns = false;

            // Populate the preset dropdown
            cmb_presets.Items.AddRange(Plugin.Presets.Keys.OrderBy(r => r).ToArray());
            cmb_presets.Items.Add("New...");

            // Bind the plugin links to the grid
            grid_links.DataSource = Plugin.Links;
            grid_links.Refresh();

            cmb_presets.Text = Plugin.SelectedPreset;
            initializing = false;
        }

        private void grid_links_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex == LinkName.Index)
            {
                // Check for duplicate names
                var name = e.FormattedValue.ToString();
                if (Plugin.Links.Where(r => r.Name == name).Count() > 1)
                {
                    MessageBox.Show("Duplicate name: " + name);
                    e.Cancel = true;
                }
            }
        }

        private void cmb_presets_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Skip this during initialization
            if (initializing)
            {
                return;
            }
            // Create new preset
            if (cmb_presets.SelectedIndex == cmb_presets.Items.Count - 1)
            {
                // Prompt user for name
                var newPresetName = "NewPreset";
                bool retry = false;
                do
                {
                    var result = InputBox.Show("New Preset", "Enter a name for the new preset", ref newPresetName);
                    if (result != DialogResult.OK)
                    {
                        cmb_presets.Text = Plugin.SelectedPreset;
                        return;
                    }

                    // Check valid name
                    if (newPresetName == "")
                    {
                        CustomMessageBox.Show("Invalid name");
                        retry = true;
                    }
                    else if (Plugin.Presets.ContainsKey(newPresetName))
                    {
                        retry = CustomMessageBox.Show("Overwrite existing preset?", MessageBoxButtons: MessageBoxButtons.YesNo) != (int)DialogResult.Yes;
                        if (!retry)
                        {
                            // Delete the preset from the combobox
                            cmb_presets.Items.Remove(newPresetName);
                        }
                    }

                } while (retry);

                // Copy the current link list into Presets
                Plugin.SavePreset(newPresetName);

                // Add the new preset to the combobox
                initializing = true;
                cmb_presets.Items.Clear();
                cmb_presets.Items.AddRange(Plugin.Presets.Keys.OrderBy(r => r).ToArray());
                cmb_presets.Items.Add("New...");
                cmb_presets.Text = newPresetName;

                Plugin.LoadPreset(newPresetName);
                grid_links.Refresh();
                initializing = false;
            }
            else
            {
                if (Plugin.Presets.ContainsKey(Plugin.SelectedPreset) &&
                    !Plugin.Presets[Plugin.SelectedPreset].SequenceEqual(Plugin.Links))
                {
                    var result = CustomMessageBox.Show("You have unsaved changes. Do you want to save them?", MessageBoxButtons: MessageBoxButtons.YesNo);
                    if (result == (int)DialogResult.Yes)
                    {
                        // Save these settings
                        Plugin.SavePreset(Plugin.SelectedPreset);
                        if (Plugin.Links.Count == 0)
                        {
                            cmb_presets.Items.Remove(cmb_presets.SelectedItem);
                        }
                    }
                }

                // Load the new preset
                Plugin.LoadPreset(cmb_presets.Text);
                grid_links.Refresh();
            }
        }

        private void RedundantLinkManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Plugin.Presets[Plugin.SelectedPreset].SequenceEqual(Plugin.Links))
            {
                var result = CustomMessageBox.Show("Save changes to the current preset?", MessageBoxButtons: MessageBoxButtons.YesNo);
                if (result == (int)DialogResult.Yes)
                {
                    Plugin.SavePreset(Plugin.SelectedPreset);
                }
            }
        }

        private string FirstUniqueName(string prefix)
        {
            // Generate a unique name like "LinkName1" with the lowest unused number
            var i = 1;
            while (Plugin.Links.Where(r => r.Name == prefix + i.ToString()).Count() > 0)
            {
                i++;
            }
            return prefix + i.ToString();
        }

        private void grid_links_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            Console.WriteLine("grid_links_UserAddedRow");

            var linkOptions = Plugin.Links[grid_links.CurrentCell.RowIndex];
            // Fill in default values for the new row
            linkOptions.Type = grid_links.CurrentCell.EditedFormattedValue.ToString();
            linkOptions.Enabled = false; // So we don't start autoconnecting until we fill everything in
            linkOptions.Name = FirstUniqueName(linkOptions.Type);
            switch (linkOptions.Type)
            {
            default:
            case "Serial":
                linkOptions.HostOrCom = "COM1";
                linkOptions.PortOrBaud = "57600";
                break;
            case "TCP":
                linkOptions.HostOrCom = "127.0.0.1";
                linkOptions.PortOrBaud = "5760";
                break;
            case "UDP":
                linkOptions.HostOrCom = "";
                linkOptions.PortOrBaud = "14450";
                break;
            case "UDPCl":
                linkOptions.HostOrCom = "127.0.0.1";
                linkOptions.PortOrBaud = "14450";
                break;
            }

            grid_links.Refresh();
        }

        private void grid_links_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            // Make all cells except the type dropdown uneditable for the new row
            var cell = grid_links[e.ColumnIndex, e.RowIndex];
            if (e.RowIndex == grid_links.NewRowIndex && cell.OwningColumn != Type)
            {
                e.Cancel = true;
                return;
            }
            // Don't allow editing the host column of UDP rows
            var row = grid_links.Rows[e.RowIndex];
            if (row.Cells[Type.Index].Value?.ToString() == "UDP" && cell.OwningColumn == Host)
            {
                row.Cells[Host.Index].Value = null;
                e.Cancel = true;
                return;
            }
        }

        private void but_save_Click(object sender, EventArgs e)
        {
            Plugin.SavePreset(cmb_presets.Text);
            if (Plugin.Links.Count == 0)
            {
                cmb_presets.Items.Remove(cmb_presets.SelectedItem);
            }
        }

        private void grid_links_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (grid_links.Columns[e.ColumnIndex] == Up)
            {
                // Move this row up
                if (e.RowIndex == 0) return; // Can't move the first row up
                if (e.RowIndex == grid_links.NewRowIndex) return; // Can't move the new row up
                var linkOpts = Plugin.Links[e.RowIndex];
                Plugin.Links.RemoveAt(e.RowIndex);
                Plugin.Links.Insert(e.RowIndex - 1, linkOpts);
                return;
            }
            if (grid_links.Columns[e.ColumnIndex] == Down)
            {
                // Move this row down
                if (e.RowIndex > Plugin.Links.Count - 1) return; // Can't move the last row down
                var linkOpts = Plugin.Links[e.RowIndex];
                Plugin.Links.RemoveAt(e.RowIndex);
                Plugin.Links.Insert(e.RowIndex + 1, linkOpts);
                return;
            }
            if (grid_links.Columns[e.ColumnIndex] == Delete)
            {
                // Delete this row
                if (e.RowIndex == grid_links.NewRowIndex) return; // Can't delete the new row
                var result = CustomMessageBox.Show("Delete this link?", MessageBoxButtons: MessageBoxButtons.OKCancel);
                if (result == (int)DialogResult.Cancel) return;
                Plugin.Links[e.RowIndex].Dispose();
                Plugin.Links.RemoveAt(e.RowIndex);
                return;
            }
        }

        /// <summary>
        /// Commit checkbox changes immediately
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grid_links_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (grid_links.CurrentCell is DataGridViewCheckBoxCell && grid_links.IsCurrentCellDirty)
            {
                grid_links.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }
    }
}
