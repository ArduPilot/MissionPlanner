using System;
using System.Windows.Forms;

namespace Carbonix
{
    public partial class LoiterTimeDialog : Form
    {
        public LoiterTimeDialog()
        {
            InitializeComponent();
        }

        private void but_ok_Click(object sender, EventArgs e)
        {
            // Set dialog result to OK
            this.DialogResult = DialogResult.OK;
            // Close the dialog
            this.Close();
        }

        private void LoiterTimeDialog_Load(object sender, EventArgs e)
        {
            dateTimePicker1.Value = dateTimePicker1.MinDate + new TimeSpan(0, 5, 0);
        }
    }
}
