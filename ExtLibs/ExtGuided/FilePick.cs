using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ExtGuided
{
    public partial class FilePick : Form
    {
        public FilePick()
        {
            InitializeComponent();
        }

        private void but_browse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();

            if (ofd.FileName != null)
            {
                txt_File.Text = ofd.FileName;
                ExtGuidedPlugin.file = ofd.FileName;
            }
        }
    }
}
