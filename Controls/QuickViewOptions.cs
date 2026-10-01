using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls
{
    public partial class QuickViewOptions : Form
    {
        private QuickView _qv;
        public QuickViewOptions(QuickView qv)
        {
            InitializeComponent();
            _qv = qv;
        }

        List<Tuple<string, string>> all_fields = new List<Tuple<string, string>>();

        // Stores the original value of the textbox background color to restore after error highlighting
        private Color backup_backcolor = Color.Empty;

        private void QuickViewOptions_Shown(object sender, EventArgs e)
        {
            backup_backcolor = TXT_color.BackColor;

            // Populate combobox with variables
            object thisBoxed = MainV2.comPort.MAV.cs;
            Type test = thisBoxed.GetType();

            foreach (var field in test.GetProperties())
            {
                object fieldValue = field.GetValue(thisBoxed, null); // Get value
                if (fieldValue == null)
                    continue;

                if (!fieldValue.IsNumber() && !(fieldValue is bool))
                {
                    continue;
                }

                if (field.Name.Contains("customfield"))
                {
                    if (CurrentState.custom_field_names.ContainsKey(field.Name))
                    {
                        string name = CurrentState.custom_field_names[field.Name];
                        all_fields.Add(new Tuple<string, string>(field.Name, name));
                    }
                }
                else
                {
                    all_fields.Add(new Tuple<string, string>(field.Name, field.Name));
                }
            }

            // Sort all_fields by display name
            all_fields.Sort((x, y) => x.Item2.CompareTo(y.Item2));

            // Fetch the selected variable name from the tag of the QuickView
            string variable_name = "";
            try
            {
                variable_name = (string)_qv.Tag;
                if (variable_name.StartsWith("customfield"))
                {
                    variable_name = CurrentState.custom_field_names[variable_name];
                }
            }
            catch
            {
                ; // silently fail
            }

            // Initialize combobox
            CMB_Source.ValueMember = "Item1";
            CMB_Source.DisplayMember = "Item2";
            CMB_Source.DataSource = all_fields;
            CMB_Source.Text = variable_name;

            // Initialize precision field, check if _qv.numberformat is in the form "0" or "0.00" etc.
            // and count the zeros after the decimal if it is
            if (_qv.numberformat == "0")
            {
                NUM_precision.Value = 0;
            }
            else if (Regex.IsMatch(_qv.numberformat, @"^0\.0+$"))
            {
                int precision = _qv.numberformat.Substring(_qv.numberformat.IndexOf('.') + 1).Length;
                if (precision > NUM_precision.Maximum)
                {
                    NUM_precision.Maximum = precision;
                }
                NUM_precision.Value = precision;
            }
            // If it doesn't match, then we are using a custom format instead of the precision box
            else
            {
                NUM_precision.Enabled = false;
            }

            // Initialize custom label
            CHK_customlabel.Checked = Settings.Instance[_qv.Name + "_label"] != null;
            TXT_customlabel.Enabled = CHK_customlabel.Checked;
            TXT_customlabel.Text = _qv.desc;

            // Initialize custom color
            CHK_customcolor.Checked = Settings.Instance[_qv.Name + "_color"] != null;
            TXT_color.Enabled = CHK_customcolor.Checked;
            BUT_colorpicker.Enabled = CHK_customcolor.Checked;
            string colorname = _qv.numberColorBackup.Name;
            if (colorname.ToLower().StartsWith("ff") && colorname.Length == 8)
            {
                TXT_color.Text = colorname.Substring(2);
            }
            else
            {
                TXT_color.Text = colorname;
            }
            BUT_colorpicker.BackColor = _qv.numberColorBackup;

            // Initialize custom format
            CHK_customformat.Checked = !NUM_precision.Enabled;
            TXT_customformat.Enabled = CHK_customformat.Checked;
            TXT_customformat.Text = _qv.numberformat.Replace("\\:", ":");

            // Initialize custom width
            int width = _qv.charWidth;
            CHK_customwidth.Checked = width > 0;
            NUM_customwidth.Enabled = CHK_customwidth.Checked;
            if (width > NUM_customwidth.Maximum)
            {
                NUM_customwidth.Maximum = width;
            }
            if (CHK_customwidth.Checked)
            {
                NUM_customwidth.Value = width;
            }
            
            // Initialize offset and scale
            double scale = _qv.scale;
            double offset = _qv.offset;
            TXT_scale.Text = scale.ToString("0.0########");
            TXT_offset.Text = offset.ToString("0.0########");
        }

        private void NUM_precision_ValueChanged(object sender, EventArgs e)
        {
            if (NUM_precision.Value == 0)
            {
                Settings.Instance[_qv.Name + "_format"] = "0";
            }
            else if (NUM_precision.Value == 2)
            {
                Settings.Instance.Remove(_qv.Name + "_format");
            }
            else if (NUM_precision.Value >= 1)
            {
                Settings.Instance[_qv.Name + "_format"] = "0." + new string('0', (int)NUM_precision.Value);
            }
        }

        private void CHK_customlabel_CheckedChanged(object sender, EventArgs e)
        {
            bool is_checked = CHK_customlabel.Checked;
            string setting_name = _qv.Name + "_label";
            TXT_customlabel.Enabled = is_checked;
            if(is_checked)
            {
                Settings.Instance[setting_name] = TXT_customlabel.Text;
            }
            else
            {
                Settings.Instance.Remove(setting_name);
            }
        }

        private void CHK_customcolor_CheckedChanged(object sender, EventArgs e)
        {
            bool is_checked = CHK_customcolor.Checked;
            string setting_name = _qv.Name + "_color";
            TXT_color.Enabled = is_checked;
            BUT_colorpicker.Enabled = is_checked;
            if (is_checked)
            {
                TXT_color_TextChanged(null, null);
            }
            else
            {
                Settings.Instance.Remove(setting_name);
            }
        }

        private void CHK_customformat_CheckedChanged(object sender, EventArgs e)
        {
            bool is_checked = CHK_customformat.Checked;
            string setting_name = _qv.Name + "_format";
            TXT_customformat.Enabled = is_checked;
            if (is_checked)
            {
                Settings.Instance[setting_name] = TXT_customformat.Text.Replace(":","\\:");
                NUM_precision.Enabled = false;
            }
            else
            {
                NUM_precision_ValueChanged(null, null);
                NUM_precision.Enabled = true;
            }
        }

        private void CHK_customwidth_CheckedChanged(object sender, EventArgs e)
        {
            bool is_checked = CHK_customwidth.Checked;
            string setting_name = _qv.Name + "_charWidth";
            NUM_customwidth.Enabled = is_checked;
            if (is_checked)
            {
                Settings.Instance[setting_name] = NUM_customwidth.Text;
            }
            else
            {
                Settings.Instance.Remove(setting_name);
            }
        }

        private void TXT_customlabel_TextChanged(object sender, EventArgs e)
        {
            if (!CHK_customlabel.Checked)
                return;

            Settings.Instance[_qv.Name + "_label"] = TXT_customlabel.Text;
        }

        private void TXT_color_TextChanged(object sender, EventArgs e)
        {
            if (!CHK_customcolor.Checked)
            {
                return;
            }
            
            if (Regex.Match(TXT_color.Text, "^(?:[0-9a-fA-F]{2}){3,4}$").Success)
            {
                BUT_colorpicker.BackColor = System.Drawing.ColorTranslator.FromHtml("#"+TXT_color.Text);
                Settings.Instance[_qv.Name + "_color"] = "#"+TXT_color.Text;
                return;
            }
            try
            {
                BUT_colorpicker.BackColor = System.Drawing.ColorTranslator.FromHtml(TXT_color.Text);
            }
            catch(Exception)
            {
                // Not a valid color string
                return;
            }
            Settings.Instance[_qv.Name + "_color"] = TXT_color.Text;
        }

        private void TXT_customformat_TextChanged(object sender, EventArgs e)
        {
            if (!CHK_customformat.Checked)
                return;

            string setting_name = _qv.Name + "_format";

            // Check if this is a valid format
            string numberformat = ((Control)sender).Text.Replace(":", "\\:");
            double test_number = 125.2;
            try
            {
                if (numberformat.Contains(":"))
                {
                    TimeSpan.FromSeconds(test_number).ToString(numberformat);
                }
                else
                {
                    test_number.ToString(numberformat);
                }
                ((Control)sender).BackColor = backup_backcolor;
                Settings.Instance[setting_name] = numberformat;
            }
            catch (FormatException)
            {
                ((Control)sender).BackColor = Color.Red;
                Settings.Instance.Remove(setting_name);
            }
        }

        private void NUM_customwidth_ValueChanged(object sender, EventArgs e)
        {
            if (!CHK_customwidth.Checked)
                return;

            Settings.Instance[_qv.Name + "_charWidth"] = NUM_customwidth.Value.ToString();
        }

        private void BUT_colorpicker_Click(object sender, EventArgs e)
        {
            ColorDialog colorDlg = new ColorDialog();
            colorDlg.Color = BUT_colorpicker.BackColor;

            if (colorDlg.ShowDialog() == DialogResult.OK)
            {
                BUT_colorpicker.BackColor = colorDlg.Color;
                // Get rid of opacity from name
                string colorname = colorDlg.Color.Name;
                if(colorname.ToLower().StartsWith("ff") && colorname.Length == 8)
                {
                    TXT_color.Text = colorname.Substring(2);
                }
                else
                {
                    TXT_color.Text = colorname;
                }
            }
        }

        private void TXT_scale_offset_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.') && (e.KeyChar != '-'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }

            // only allow negative sign first
            if ((e.KeyChar == '-') && ((sender as TextBox).SelectionStart > 0))
            {
                e.Handled = true;
            }
        }

        private void TXT_scale_TextChanged(object sender, EventArgs e)
        {
            string setting_name = _qv.Name + "_scale";

            // Try to parse the text as a number, and change background to red if invalid
            double result;
            if (!double.TryParse(((TextBox)sender).Text, out result))
            {
                ((Control)sender).BackColor = Color.Red;
                Settings.Instance.Remove(setting_name);
            }
            else
            {
                ((Control)sender).BackColor = backup_backcolor;
                Settings.Instance[setting_name] = ((TextBox)sender).Text;
            }
        }

        private void TXT_offset_TextChanged(object sender, EventArgs e)
        {
            string setting_name = _qv.Name + "_offset";

            // Try to parse the text as a number, and change background to red if invalid
            double result;
            if (!double.TryParse(((TextBox)sender).Text, out result))
            {
                ((Control)sender).BackColor = Color.Red;
                Settings.Instance.Remove(setting_name);
            }
            else
            {
                ((Control)sender).BackColor = backup_backcolor;
                Settings.Instance[setting_name] = ((TextBox)sender).Text;
            }
        }

        // Runs when the user types in the box, but not when the text is changed programatically
        private void CMB_Source_TextUpdate(object sender, EventArgs e)
        {
            List<Tuple<string, string>> filtered_fields;
            
            // Back up cursor position and text
            int cursor_pos = CMB_Source.SelectionStart;
            string searchText = CMB_Source.Text.ToLower();
            if (false)//CMB_Source.SelectedIndex > 0 || searchText == "")
            {
                filtered_fields = new List<Tuple<string, string>>(all_fields);
            }
            else
            {
                // Check all the fields in the table and see if any of them contain `searchText` as a DisplayName
                filtered_fields = all_fields.Where(f => f.Item2.ToLower().Contains(searchText.ToLower())).ToList();
                CMB_Source.DroppedDown = true;
                Cursor.Current = Cursors.Default;
            }

            CMB_Source.DataSource = filtered_fields;
            CMB_Source.Text = searchText;
            CMB_Source.SelectionStart = cursor_pos;
        }

        private void CMB_Source_DropDownClosed(object sender, EventArgs e)
        {
            if(CMB_Source.SelectedItem != null)
            {
                Tuple<string, string> field = CMB_Source.SelectedItem as Tuple<string, string>;
                if (field.Item1.StartsWith("customfield"))
                {
                    Settings.Instance[_qv.Name] = "customfield:" + field.Item2;
                }
                else
                {
                    Settings.Instance[_qv.Name] = field.Item1;
                }
                TXT_customlabel.Text = MainV2.comPort.MAV.cs.GetNameandUnit((string)CMB_Source.SelectedValue);

                // Reset scale and offset if we select a new variable
                TXT_scale.Text = "1.0";
                TXT_offset.Text = "0.0";

                // Restore the full list
                CMB_Source.DataSource = all_fields;
                CMB_Source.SelectedItem = field;
            }
        }
    }
}
