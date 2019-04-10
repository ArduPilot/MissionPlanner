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
    public partial class IntSpinSettingControl : OptionControlBase
    {
        public IntSpinSettingControl(IOSDSetting setting, int weight)
            :base(setting, weight)
        {
            InitializeComponent();
            
            labelName.Text = setting.Name;

            numBox.ValueChanged += ValueChanged;
        }

        private void ValueChanged(object sender, EventArgs e)
        {
            setting.Value = (double)numBox.Value;
        }

        protected override void UpdateControl(IOSDSetting setting)
        {
            numBox.Value = (int)setting.Value;
        }        
    }
}
