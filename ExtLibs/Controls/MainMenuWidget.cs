using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Controls
{
    public partial class MainMenuWidget : MyUserControl
    {
        private bool active = false;
        public MainMenuWidget()
        {
            InitializeComponent();
        }

        private void MainButton_Click(object sender, EventArgs e)
        {
            active = !active;
            Console.WriteLine("MainButton pressed, now: "+active);
            updateSize();
        }

        private void updateSize() 
        {
            if (active)
            {
                this.Size = new Size(71, 71);
            }
            else {
                this.Size = new Size(717, 71);
            }
        }
    }
}
