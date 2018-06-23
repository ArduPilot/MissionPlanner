using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner
{
    public partial class ConnectionConfigForm: Form
    {
        public ConnectionConfigForm()
        {
            InitializeComponent();
        }

        public ConnectionControl ConnectionControl
        {
            get { return connectionControl; }
            set { connectionControl = value; }
        }
    }
}
