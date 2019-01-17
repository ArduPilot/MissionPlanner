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

namespace OSDConfigurator.GUI.ItemControls
{
    public partial class CommonItemControl : ItemControlBase
    {        
        public CommonItemControl(ScreenControl screenControl, OSDItem item)
            :base(screenControl, item)
        {
            InitializeComponent();
            
            label.Text = item.Name;

            checkBox.CheckedChanged += CheckBoxChanged;
            
            label.MouseClick += (s, e) => DoSelect();
            this.MouseClick += (s, e) => DoSelect();
        }

        private void CheckBoxChanged(object sender, EventArgs e)
        {
            item.Enabled.Value = checkBox.Checked ? 1 : 0;
        }

        protected override void EnabledUpdated(IOSDSetting setting)
        {
            checkBox.Checked = setting.Value > 0;
        }
    }
}
