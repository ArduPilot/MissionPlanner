using System;
using System.Windows.Forms;

namespace MissionPlanner.Plugin
{
    public partial class PluginUI : Form
    {
        public PluginUI()
        {
            InitializeComponent();
        }

        private void PluginUI_Load(object sender, EventArgs e)
        {
            foreach (var item in MissionPlanner.Plugin.PluginLoader.Plugins)
            {
                int row = dataGridView1.Rows.Add();

                DataGridViewRow temp = dataGridView1.Rows[row];

                temp.Cells[Loaded.Index].Value = true;
                temp.Cells[PluginName.Index].Value = item.Name;
                temp.Cells[Author.Index].Value = item.Author;
                temp.Cells[Version.Index].Value = item.Version;
            }
        }
    }
}