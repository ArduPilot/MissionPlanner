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

namespace OSDConfigurator.GUI
{
    public partial class OSDUserControl : UserControl
    {
        private OSDConfiguration config;
        
        public OSDUserControl()
        {
            InitializeComponent();
        }

        public void ApplySettings(IList<IOSDSetting> settings)
        {
            ClearOptions();
            ClearScreens();

            config = ConfigFactory.Create(settings);

            FillGlobalOptions();

            foreach (var scr in config.Screens)
                AddScreen(scr);
        }
        
        private void FillGlobalOptions()
        {
            foreach(var ctr in OptionControlFactory.Create(config.Options))
            {
                ctr.Dock = DockStyle.Top;
                tabSettings.Controls.Add(ctr);
            }
            
        }

        private void ClearOptions()
        {
            tabSettings.Controls.Clear();
        }
        
        private void AddScreen(OSDScreen screen)
        {
            var screenControl = new ScreenControl(screen);
            screenControl.Dock = DockStyle.Fill;

            var tab = new TabPage($"   {screen.Name}   ");
            tab.Controls.Add(screenControl);

            tabControl.TabPages.Add(tab);
        }

        private void ClearScreens()
        {
            while (tabControl.TabPages.Count > 1)
                tabControl.TabPages.RemoveAt(1);
        }
    }
}
