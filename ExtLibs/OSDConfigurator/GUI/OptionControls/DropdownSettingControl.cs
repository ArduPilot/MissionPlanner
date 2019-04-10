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
    public partial class DropdownSettingControl : UserControl, IPrioritizedItem
    {
        private readonly IOSDSetting setting;

        public int Weight { get; private set; }

        public DropdownSettingControl(IOSDSetting setting, string[] values, int weight)
        {
            this.setting = setting ?? throw new ArgumentNullException(nameof(setting));

            Weight = weight;

            InitializeComponent();

            labelName.Text = setting.Name;

            comboValue.Items.AddRange(values);

            comboValue.SelectedIndex = (int)setting.Value;

            comboValue.SelectedIndexChanged += SelectedIndexChanged;
        }
        
        private void SelectedIndexChanged(object sender, EventArgs e)
        {
            setting.Value = comboValue.SelectedIndex;
        }
    }
}
