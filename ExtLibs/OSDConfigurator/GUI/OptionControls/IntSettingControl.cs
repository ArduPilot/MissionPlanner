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
    public partial class IntSettingControl : OptionControlBase
    {
        public IntSettingControl(IOSDSetting setting, int weight)
            :base(setting, weight)
        {
            InitializeComponent();
            
            labelName.Text = setting.Name;

            tbValue.Validating += ValueValidating;            
        }

        protected override void UpdateControl(IOSDSetting setting)
        {
            tbValue.Text = setting.Value.ToString();
        }

        private void ValueValidating(object sender, CancelEventArgs e)
        {
            if (double.TryParse(tbValue.Text, out var result))
            {
                setting.Value = result;
            }
            else
            {
                UpdateControl(setting);
            }
        }        
    }
}
