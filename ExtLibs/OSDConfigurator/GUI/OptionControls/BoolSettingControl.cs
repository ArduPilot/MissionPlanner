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
    public partial class BoolSettingControl : OptionControlBase
    {        
        public BoolSettingControl(IOSDSetting setting, int weight)
            :base(setting, weight)
        {            
            InitializeComponent();

            labelName.Text = setting.Name;
            
            checkBox.CheckedChanged += CheckedChanged;            
        }

        private void CheckedChanged(object sender, EventArgs e)
        {
            setting.Value = checkBox.Checked ? 1 : 0;
        }

        protected override void UpdateControl(IOSDSetting setting)
        {
            checkBox.Checked = setting.Value > 0;
        }
    }
}
