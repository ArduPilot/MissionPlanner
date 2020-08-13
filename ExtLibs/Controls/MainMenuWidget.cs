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
        private int X = 0;
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
            if (!active)
            {
                this.Size = new Size(100, 100);
            }
            else {
                this.Size = new Size(750, 100);
            }
        }

        private void MainMenuWidget_MouseEnter(object sender, EventArgs e)
        {
            active = true;
            updateSize();
            System.Diagnostics.Debug.WriteLine("active - true");
        }

        private void MainMenuWidget_MouseLeave(object sender, EventArgs e)
        {
            if (this.GetChildAtPoint(this.PointToClient(MousePosition)) == null)
            {
                active = false;
                updateSize();
                System.Diagnostics.Debug.WriteLine("active - false");
            }
        }

        private void MapChoiseButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("WWWWWWWWWWWWWWWWWWWWWWW");
        }
    }
    
}
