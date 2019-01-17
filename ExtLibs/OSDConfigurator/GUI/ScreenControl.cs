using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OSDConfigurator.Models;
using OSDConfigurator.GUI.ItemControls;

namespace OSDConfigurator.GUI
{
    public partial class ScreenControl : UserControl
    {
        private readonly OSDScreen screen;

        private LayoutControl layoutControl;

        public OSDItem SelectedItem { get; private set; }

        public ScreenControl(OSDScreen screen)
        {
            this.screen = screen ?? throw new ArgumentNullException(nameof(screen));

            InitializeComponent();
            
            layoutControl = new LayoutControl(this, screen.Items, new Visualizer());
            layoutControl.Margin = new Padding(8);

            tableLeft.Controls.Add(layoutControl, 0, 0);

            //layoutControl.Location = new Point(10, 10);
            //panelLayout.Controls.Add(layoutControl);
            //panelLayout.Size = new Size(layoutControl.Width + 20, layoutControl.Height + 20);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            FillScreenOptions();

            var items = new List<UserControl>();
            
            foreach(var item in screen.Items)
                items.Add(new CommonItemControl(this, item));            

            items.Reverse();
            items.ForEach(AddItem);
        }

        private void FillScreenOptions()
        {
            foreach (var ctr in OptionControlFactory.Create(screen.Options))
            {
                ctr.Dock = DockStyle.Top;
                groupScreenOptions.Controls.Add(ctr);
            }
        }

        private void AddItem(UserControl control)
        {
            //control.Dock = DockStyle.Top;
            // panelItems.Controls.Add(control);

            panelItemList.Controls.Add(control);
        }

        public void ItemSelected(OSDItem osdItem)
        {
            if (groupOptions.Text == osdItem.Name)
                return;

            SelectedItem = osdItem;

            groupOptions.Text = osdItem.Name;
            groupOptions.Controls.Clear();

            foreach (var ctr in OptionControlFactory.Create(osdItem.Options))
            {
                ctr.Dock = DockStyle.Top;
                groupOptions.Controls.Add(ctr);
            }
            
            layoutControl.ReDraw();
        }
    }
}
