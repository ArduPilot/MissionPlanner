using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class OptionForm : Form
    {
        public ComboBox Combobox;
        public MyButton Button1;
        public MyButton Button2;

        public string SelectedItem ="";

        public OptionForm()
        {
            InitializeComponent();

            Combobox = comboBox1;
            Button1 = myButton1;
            Button2 = myButton2;
        }

        private void myButton1_Click(object sender, EventArgs e)
        {

            this.Close();
        }

        private void myButton2_Click(object sender, EventArgs e)
        {
            SelectedItem = "";
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedItem = comboBox1.Text;
        }
    }
}
