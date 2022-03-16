using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class FileBrowse : MyUserControl
    {
        public string filename { get; set; }
        public string Filter { get; set; }
        [DefaultValue(true)]
        public bool OpenFile { get; set; }

        public FileBrowse()
        {
            OpenFile = true;

            InitializeComponent();
        }

        private void myButton1_Click(object sender, EventArgs e)
        {
            if (OpenFile)
            {
                openFileDialog1.Filter = Filter;
                openFileDialog1.ShowDialog();
                filename = openFileDialog1.FileName;
            }
            else
            {
                saveFileDialog1.Filter = Filter;
                saveFileDialog1.ShowDialog();
                filename = saveFileDialog1.FileName;
            }

            
            textBox1.Text = filename;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            filename = textBox1.Text;
        }
    }
}
