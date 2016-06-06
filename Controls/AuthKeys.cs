using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Mavlink;

namespace MissionPlanner.Controls
{
    public partial class AuthKeys : Form
    {
        static string keyfile = "authkeys.xml";

        public AuthKeys()
        {
            InitializeComponent();

            Load();
        }

        private void but_save_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Load()
        {
            dataGridView1.Rows.Clear();
            foreach (var authKey in MAVAuthKeys.Keys)
            {
                int row = dataGridView1.Rows.Add();
                dataGridView1[DName.Index, row].Value = authKey.Key;
                dataGridView1[Key.Index, row].Value = BitConverter.ToString(authKey.Value.Key).Replace("-","");
            }
        }

        private void Save()
        {
            MAVAuthKeys.Save();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Key.Index)
            {
                /*
                string pass = "";

                if (InputBox.Show("Input Seed", "Please enter your pass prase", ref pass) == DialogResult.OK)
                {
                    var input = InputBox.value;

                    MAVAuthKeys.AddKey(dataGridView1[DName.Index, e.RowIndex].Value.ToString(), input);
                }
                 */
            }
        }

        private void but_add_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.Rows.Add();

            string name = "";

            if (InputBox.Show("Name", "Please enter a friendly name", ref name) == DialogResult.OK)
            {
                dataGridView1[DName.Index, row].Value = name;

                string pass = "";

                if (InputBox.Show("Input Seed", "Please enter your pass prase", ref pass) == DialogResult.OK)
                {
                    var input = InputBox.value;

                    MAVAuthKeys.AddKey(dataGridView1[DName.Index, row].Value.ToString(), input);
                }

                dataGridView1.EndEdit();

                Save();

                Load();
            }
        }

        private void dataGridView1_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            MAVAuthKeys.Keys.Remove(e.Row.Cells[DName.Index].Value.ToString());
        }
    }
}
