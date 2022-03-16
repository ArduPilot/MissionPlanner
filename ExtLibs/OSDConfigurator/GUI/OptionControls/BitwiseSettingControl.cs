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
    public partial class BitwiseSettingControl : OptionControlBase
    {
        private bool controlUpdating;

        public BitwiseSettingControl(IOSDSetting setting, object[] optionsList, int weight)
            :base(setting, weight)
        {            
            InitializeComponent();

            labelName.Text = setting.Name;

            tbValue.Text = setting.Value.ToString();
            tbValue.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) TbValueValidating(null, null); };
            tbValue.Validating += TbValueValidating;

            listBox.Items.AddRange(optionsList);
            listBox.ItemCheck += ListBoxItemCheck;            
        }

        private void TbValueValidating(object sender, CancelEventArgs e)
        {
            if (int.TryParse(tbValue.Text, out int value))
            {
                setting.Value = value;
                UpdateControl(setting);
            }
            else if (e != null)
                e.Cancel = true;
            else
                UpdateControl(setting);
        }

        private void ListBoxItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (controlUpdating)
                return;

            controlUpdating = true;

            if (e.NewValue == CheckState.Checked)
            {
                setting.Value = (int)setting.Value | (1 << e.Index);
            }
            else
            {
                setting.Value = setting.Value - (1 << e.Index);
            }
            
            tbValue.Text = setting.Value.ToString();

            controlUpdating = false;
        }
        
        protected override void UpdateControl(IOSDSetting setting)
        {
            if (controlUpdating)
                return;

            controlUpdating = true;

            tbValue.Text = setting.Value.ToString();

            int value = (int)setting.Value;

            for (int i=0; i<32; ++i)
            {
                if (listBox.Items.Count == i)
                    break;

                var optionEnabled = (value & 1) == 1;

                listBox.SetItemChecked(i, optionEnabled);

                value = value >> 1;
            }

            controlUpdating = false;
        }
    }
}
