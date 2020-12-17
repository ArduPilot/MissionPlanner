using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RFDLib.GUI
{
    public partial class CBWithBackground : UserControl
    {
        public CBWithBackground()
        {
            InitializeComponent();

            OnResize(null);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            cmb.Top = (this.Height - cmb.Height) / 2;
            cmb.Width = this.Width;
        }

        public ComboBox CMB
        {
            get
            {
                return cmb;
            }
        }
    }
}
