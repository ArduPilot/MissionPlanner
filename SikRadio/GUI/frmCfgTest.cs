using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFD900Tools.GUI
{
    public partial class frmCfgTest : Form
    {
        public frmCfgTest()
        {
            InitializeComponent();
        }

        public RFDLib.GUI.ConfigArray GetControl()
        {
            return CFG;
        }
    }
}
