using OSDConfigurator.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OSDConfigurator.GUI.Osd56ItemsSetup
{
    public partial class ItemControl : UserControl
    {
        private readonly int initialSelectedIndex;

        private string InitialMin => item.OSDItem.EndsWith("_MIN").Value.ToString("0.#####");
        private string InitialMax => item.OSDItem.EndsWith("_MAX").Value.ToString("0.#####");
        private int InitialType => (int)item.OSDItem.EndsWith("_TYPE").Value;
        private string InitialIncrement => item.OSDItem.EndsWith("_INCR").Value.ToString("0.#####");

        // see MAVLink.OSD_PARAM_CONFIG_TYPE
        public static string[] ItemTypes = new[]
        {
            "None",
            "Serial Protocol",
            "Servo Function",
            "Aux Function",
            "Flight Mode",
            "Failsafe Act 1",
            "Failsafe Act 2",
            "Num Types"
        };

        private readonly SetupDialog.OsdItemWrapper item;

        public ItemControl(SetupDialog.OsdItemWrapper item, string[] allParamNames)
            : this()
        {
            this.item = item;

            cbParamName.Items.AddRange(allParamNames);
            cbParamName.SelectedIndex = Array.IndexOf(allParamNames, item.AssignedFunction);
            initialSelectedIndex = cbParamName.SelectedIndex;

            cbType.Items.AddRange(ItemTypes);
            cbType.SelectedIndex = InitialType;

            tbMin.Text = InitialMin;
            tbMax.Text = InitialMax;
            tbIncrement.Text = InitialIncrement;
        }

        public ItemControl()
        {
            InitializeComponent();
        }

        private void TextBoxKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                e.KeyChar != '.' && e.KeyChar != '-')
            {
                e.Handled = true;
            }

            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            if (e.KeyChar == '-' && ((TextBox)sender).Text.Length > 0)
            {
                e.Handled = true;
            }
        }

        private void cbType_SelectedIndexChanged(object sender, EventArgs e)
        {
            tbMin.Enabled = cbType.SelectedIndex == 0;
            tbMax.Enabled = cbType.SelectedIndex == 0;
            tbIncrement.Enabled = cbType.SelectedIndex == 0;
        }

        public bool IsDirty(out SetupDialog.Change change)
        {
            change = new SetupDialog.Change
            {
                ParamName = cbParamName.SelectedItem as string,
                Screen = item.Screen,
                Index = item.Index,
                Type = (byte)cbType.SelectedIndex
            };

            // Preset by Type
            if (change.Type != 0)
            {
                return cbParamName.SelectedIndex != initialSelectedIndex
                    || cbType.SelectedIndex != InitialType;
            }
            // Manual 
            else
            {
                change.Min = double.Parse(tbMin.Text);
                change.Max = double.Parse(tbMax.Text);
                change.Increment = double.Parse(tbIncrement.Text);

                return tbMin.Text != InitialMin
                    || tbMax.Text != InitialMax
                    || tbIncrement.Text != InitialIncrement
                    || cbParamName.SelectedIndex != initialSelectedIndex
                    || cbType.SelectedIndex != InitialType;
            }
        }
    }
}
