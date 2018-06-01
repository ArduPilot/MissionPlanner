using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Mavlink;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class AuthKeys : Form
    {
        public AuthKeys()
        {
            InitializeComponent();

            ThemeManager.ApplyThemeTo(this);

            LoadKeys();
        }

        private void but_save_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void LoadKeys()
        {
            dataGridView1.Rows.Clear();
            foreach (var authKey in MAVAuthKeys.Keys)
            {
                int row = dataGridView1.Rows.Add();
                dataGridView1[FName.Index, row].Value = authKey.Key;
                dataGridView1[Key.Index, row].Value = Convert.ToBase64String(authKey.Value.Key);
            }
        }

        private void Save()
        {
            MAVAuthKeys.Save();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Use.Index)
            {
                MainV2.comPort.setupSigning("",Convert.FromBase64String(dataGridView1[Key.Index,e.RowIndex].Value.ToString()));
            }
        }

        private void but_add_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.Rows.Add();

            string name = "";

            if (InputBox.Show("Name", "Please enter a friendly name", ref name) == DialogResult.OK)
            {
                dataGridView1[FName.Index, row].Value = name;

                string pass = "";

                if (InputBox.Show("Input Seed", "Please enter your pass prase", ref pass) == DialogResult.OK)
                {
                    var input = InputBox.value;

                    MAVAuthKeys.AddKey(dataGridView1[FName.Index, row].Value.ToString(), input);
                }

                dataGridView1.EndEdit();

                Save();

                LoadKeys();
            }
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            MAVAuthKeys.Keys.Remove(e.Row.Cells[FName.Index].Value.ToString());
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            dataGridView1[Use.Index, e.RowIndex].Value = "Use";
        }

        private void but_disablesigning_Click(object sender, EventArgs e)
        {
            MainV2.comPort.setupSigning("");
        }
    }
}
