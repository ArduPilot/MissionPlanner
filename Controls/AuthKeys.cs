using MissionPlanner.Mavlink;
using MissionPlanner.Utilities;
using System;
using System.Linq;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class AuthKeys : Form
    {
        public AuthKeys()
        {
            InitializeComponent();

            ThemeManager.ApplyThemeTo(this);

            LoadKeys();

            timer1.Interval = 190;
            timer1.Start();
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
                MainV2.comPort.setupSigning(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, "",
                    Convert.FromBase64String(dataGridView1[Key.Index, e.RowIndex].Value.ToString()));
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

                if (InputBox.Show("Input Seed", "Please enter your pass phrase/sentence\nNumbers, Lower Case, Upper Case, Symbols, and 12+ chars long using atleast 2 of each", ref pass) == DialogResult.OK)
                {
                    var input = InputBox.value;
                    {
                        var n = 0;
                        int score = 0;
                        var len = input.Length;

                        // chars
                        score += len * 4;
                        // upper
                        n = input.Count(c => char.IsUpper(c));
                        if(n > 0)
                        score += (len - n) * 2;
                        // lower
                        n = input.Count(c => char.IsLower(c));
                        if (n > 0)
                            score += (len - n) * 2;
                        //number
                        n = input.Count(c => char.IsNumber(c));
                        if (n > 0)
                            score +=  n * 4;
                        //symbols
                        n = input.Count(c => char.IsSymbol(c) || char.IsPunctuation(c));
                        if (n > 0)
                            score += n * 6;
                        //middle number or symbol
                        n = input.Skip(1).Take(len - 2).Count(c => char.IsSymbol(c) || char.IsPunctuation(c) ||  char.IsNumber(c));
                        if (n > 0)
                            score += n * 2;
                        // letters only
                        n = input.Count(c => char.IsLetter(c));
                        score += len == n ? -len : 0;
                        // numbers only
                        n = input.Count(c => char.IsNumber(c));
                        score += len == n ? -len : 0;

                        if(score <= 40)
                            CustomMessageBox.Show("Password Strength: " + score + " WEAK - it will be added, but please pick a better password");
                        else if (score <= 60)
                            CustomMessageBox.Show("Password Strength: " + score + " Good");
                        else if (score > 60)
                            CustomMessageBox.Show("Password Strength: " + score + " Strong");
                    }
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
            MainV2.comPort.setupSigning(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, "");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var name = "None/Unknown";
            var key = MainV2.comPort.MAV.signingKey;
            if (key != null)
            {
                foreach (var authKey in MAVAuthKeys.Keys)
                {
                    if (authKey.Value.Key.ByteArraysEqual(key))
                    {
                        name = authKey.Key;
                        break;
                    }
                }
            }
            
            lbl_sgnpkts.Text = "Using Key: " + name + ", Signed Packets: " + MainV2.comPort.Mavlink2Signed.ToString();
        }
    }
}
