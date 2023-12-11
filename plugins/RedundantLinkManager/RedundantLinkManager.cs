using MissionPlanner.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RedundantLinkManager
{
    public partial class RedundantLinkManager : Form
    {
        public RedundantLinkManager()
        {
            InitializeComponent();
        }

        // Force the "Priority" column to match the row index when reordering
        private void myDataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (grid_links.Rows[e.RowIndex].Cells[Priority.Index].Value?.ToString() != (e.RowIndex + 1).ToString())
            {
                grid_links.Rows[e.RowIndex].Cells[Priority.Index].Value = (e.RowIndex + 1).ToString();
            }
        }

        private void but_addLink_Click(object sender, EventArgs e)
        {
            // Add a row
            grid_links.Rows.Add();

            var row = grid_links.Rows[grid_links.Rows.Count - 1];
            row.Cells[Priority.Index].Value = grid_links.Rows.Count;
            row.Cells[Enabled.Index].Value = true;
            row.Cells[Type.Index].Value = Type.Items[0];
            row.Cells[Host.Index].Value = "COM1";
            row.Cells[Port.Index].Value = "57600";

            // Generate a unique name like "LinkName1" with the lowest unused number
            var name = "LinkName";
            var i = 1;
            while (grid_links.Rows.Cast<DataGridViewRow>().Where(r => r.Cells[LinkName.Index].Value?.ToString() == name + i.ToString()).Count() > 0)
            {
                i++;
            }
            row.Cells[LinkName.Index].Value = name + i.ToString();
        }

        private void grid_links_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // Turn the background red if this Name value is a duplicate
            if (e.ColumnIndex == LinkName.Index)
            {
                var name = grid_links.Rows[e.RowIndex].Cells[LinkName.Index].Value?.ToString();
                if (name != null && grid_links.Rows.Cast<DataGridViewRow>().Where(r => r.Cells[LinkName.Index].Value?.ToString() == name).Count() > 1)
                {
                    e.CellStyle.BackColor = Color.Red;
                }
            }
        }
    }
}
