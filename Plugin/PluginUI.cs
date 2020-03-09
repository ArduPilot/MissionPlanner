using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class PluginUI : Form
    {
        public PluginUI()
        {
            InitializeComponent();
            PopulateGridView();
            PerformLayout();
            labelWarning.Visible = Plugin.PluginLoader.bRestartRequired;
        }


        private void PopulateGridView()
        {
            string path = Settings.GetRunningDirectory() + "plugins" +
                          Path.DirectorySeparatorChar;

            dgvPlugins.Rows.Clear();
            //First iterate through loaded plugins.
            //Not enabled but loaded plugins are Orange, loaded and enabed are Red
            foreach (Plugin.Plugin p in Plugin.PluginLoader.Plugins)
            {
                int rowindex = dgvPlugins.Rows.Add();
                var row = dgvPlugins.Rows[rowindex];
                row.Cells["pluginName"].Value = p.Name;
                row.Cells["pluginAuthor"].Value = p.Author;
                row.Cells["pluginVersion"].Value = p.Version;
                row.Cells["pluginDll"].Value = Path.GetFileName(p.Assembly.Location).ToLower();
                bool bEnabled = !Plugin.PluginLoader.DisabledPluginNames.Contains(Path.GetFileName(p.Assembly.Location), StringComparer.OrdinalIgnoreCase);
                row.Cells["pluginEnabled"].Value = bEnabled;
                if (bEnabled) row.DefaultCellStyle.BackColor = Color.Green;
                else row.DefaultCellStyle.BackColor = Color.DarkOrange;
            }

            //Go through names from config.xml, but do not display the ones that are loaded (Those are already displayed in Orange from previous iterate)
            foreach (String s in Plugin.PluginLoader.DisabledPluginNames)
            {
                //Iterate through loaded plugins, so do not add disabled but loaded plugins
                bool isLoaded = false;

                foreach (Plugin.Plugin p in Plugin.PluginLoader.Plugins)
                    if (Path.GetFileName(p.Assembly.Location).ToLower().Contains(s)) isLoaded = true;

                if (File.Exists(path + s) && !isLoaded)
                {
                    int rowindex = dgvPlugins.Rows.Add();
                    var row = dgvPlugins.Rows[rowindex];
                    row.Cells["pluginName"].Value = "Not loaded";
                    row.Cells["pluginAuthor"].Value = "--";
                    row.Cells["pluginVersion"].Value = "--";
                    row.Cells["pluginDll"].Value = s;
                    row.Cells["pluginEnabled"].Value = false;
                    row.DefaultCellStyle.BackColor = Color.DarkRed;
                }
            }
        }

        //Update the <DisabledPlugins> settings in config.xml
        private void UpdateDisabledPlugins()
        {
            Plugin.PluginLoader.DisabledPluginNames.Clear();
            foreach (DataGridViewRow r in dgvPlugins.Rows)
                if (!(Boolean)(r.Cells["pluginEnabled"].Value))
                    Plugin.PluginLoader.DisabledPluginNames.Add(r.Cells["pluginDll"].Value.ToString().ToLower());

            if (Plugin.PluginLoader.DisabledPluginNames.Count > 0)
                Settings.Instance.SetList("DisabledPlugins", Plugin.PluginLoader.DisabledPluginNames);
            else
                Settings.Instance.Remove("DisabledPlugins");
        }

        private void bSave_Click(object sender, EventArgs e)
        {
            UpdateDisabledPlugins();
            Plugin.PluginLoader.bRestartRequired = true;
            this.Close();
        }

        private void ResizeFormForDataGrid()
        {
            this.SuspendLayout();
            Control vertical = dgvPlugins.Controls[1];
            dgvPlugins.Width = dgvPlugins.PreferredSize.Width - vertical.Width + 1;
            this.Width = dgvPlugins.Width + 15;
            this.ResumeLayout(true);
        }

        private void dgvPlugins_RowHeadersWidthChanged(object sender, EventArgs e)
        {
            ResizeFormForDataGrid();
        }

        private void dgvPlugins_SelectionChanged(object sender, EventArgs e)
        {
            int selectedRow = dgvPlugins.CurrentCell.RowIndex;
            string r = (string)dgvPlugins.Rows[selectedRow].Cells["pluginName"].Value;
            if (r != null) btnLoadPlugin.Enabled = r.ToLower().Contains("not loaded");
        }

        private void btnLoadPlugin_Click(object sender, EventArgs e)
        {

            string path = Settings.GetRunningDirectory() + "plugins" +
              Path.DirectorySeparatorChar;

            int selectedRow = dgvPlugins.CurrentCell.RowIndex;
            string filename = (string)dgvPlugins.Rows[selectedRow].Cells["pluginDLL"].Value;
            //Remove from Disabled list to allow load
            Plugin.PluginLoader.DisabledPluginNames.Remove(filename);
            Plugin.PluginLoader.Load(path + filename);
            //Add back to the Disabled list, since we did not enabled it, just loaded
            Plugin.PluginLoader.DisabledPluginNames.Add(filename);

            PopulateGridView();
            dgvPlugins.CurrentCell = dgvPlugins.Rows[0].Cells[0];
            dgvPlugins.Rows[0].Selected = true;
            ResizeFormForDataGrid();
        }

        private void PluginUI_Shown(object sender, EventArgs e)
        {
            ResizeFormForDataGrid();
        }
    }
}
