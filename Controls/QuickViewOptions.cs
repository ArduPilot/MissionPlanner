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
        private bool _initializing = true;  // Flag to suppress event handlers during form setup

        public QuickViewOptions(QuickView qv)
        {
            InitializeComponent();
            _qv = qv;
            this.Icon = qv.FindForm()?.Icon;
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
            else
            {
                // Non-standard format - will be handled by custom format
                NUM_precision.Value = 2;
            }

            // Initialize label
            TXT_customlabel.Text = _qv.desc;

            // Initialize color
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

            // Initialize format
            string currentFormat = _qv.numberformat;
            TXT_customformat.Text = currentFormat.Replace("\\:", ":");

            // Initialize offset and scale
            double scale = _qv.scale;
            double offset = _qv.offset;
            TXT_scale.Text = scale.ToString("0.0########");
            TXT_offset.Text = offset.ToString("0.0########");

            // Initialize gauge settings - set text values BEFORE checkbox to avoid
            // triggering CHK_gauge_CheckedChanged with empty/default values
            TXT_gaugeMin.Text = _qv.gaugeMin.ToString("0.#######");
            TXT_gaugeMax.Text = _qv.gaugeMax.ToString("0.#######");
            CHK_gauge.Checked = _qv.isGauge;
            TXT_gaugeMin.Enabled = CHK_gauge.Checked;
            TXT_gaugeMax.Enabled = CHK_gauge.Checked;
            label3.Enabled = CHK_gauge.Checked;
            label4.Enabled = CHK_gauge.Checked;

            // Done initializing - now event handlers can save settings
            _initializing = false;
        }

        private void NUM_precision_ValueChanged(object sender, EventArgs e)
        {
            if (_initializing)
                return;

            // Update the format textbox based on precision
            if (NUM_precision.Value == 0)
            {
                TXT_customformat.Text = "0";
            }
            else
            {
                TXT_customformat.Text = "0." + new string('0', (int)NUM_precision.Value);
            }
        }

        private void TXT_customlabel_TextChanged(object sender, EventArgs e)
        {
            // No action needed - value is read on form close
        }

        private void TXT_color_TextChanged(object sender, EventArgs e)
        {
            if (_initializing)
                return;

            if (Regex.Match(TXT_color.Text, "^(?:[0-9a-fA-F]{2}){3,4}$").Success)
            {
                BUT_colorpicker.BackColor = System.Drawing.ColorTranslator.FromHtml("#"+TXT_color.Text);
                return;
            }
            try
            {
                BUT_colorpicker.BackColor = System.Drawing.ColorTranslator.FromHtml(TXT_color.Text);
            }
            catch(Exception)
            {
                // Not a valid color string
            }
        }

        private void TXT_customformat_TextChanged(object sender, EventArgs e)
        {
            if (_initializing)
                return;

            // Check if this is a valid format
            string numberformat = TXT_customformat.Text.Replace(":", "\\:");
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
                TXT_customformat.BackColor = backup_backcolor;
            }
            catch (FormatException)
            {
                TXT_customformat.BackColor = Color.Red;
            }
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
            // Validate input - show red if invalid
            if (!double.TryParse(((TextBox)sender).Text, out _))
            {
                ((Control)sender).BackColor = Color.Red;
            }
            else
            {
                ((Control)sender).BackColor = backup_backcolor;
            }
        }

        private void TXT_offset_TextChanged(object sender, EventArgs e)
        {
            // Validate input - show red if invalid
            if (!double.TryParse(((TextBox)sender).Text, out _))
            {
                ((Control)sender).BackColor = Color.Red;
            }
            else
            {
                ((Control)sender).BackColor = backup_backcolor;
            }
        }

        private void CHK_gauge_CheckedChanged(object sender, EventArgs e)
        {
            bool is_checked = CHK_gauge.Checked;
            TXT_gaugeMin.Enabled = is_checked;
            TXT_gaugeMax.Enabled = is_checked;
            label3.Enabled = is_checked;
            label4.Enabled = is_checked;
        }

        private void TXT_gaugeMin_TextChanged(object sender, EventArgs e)
        {
            // Validate input - show red if invalid
            if (!double.TryParse(TXT_gaugeMin.Text, out _))
            {
                TXT_gaugeMin.BackColor = Color.Red;
            }
            else
            {
                TXT_gaugeMin.BackColor = backup_backcolor;
            }
        }

        private void TXT_gaugeMax_TextChanged(object sender, EventArgs e)
        {
            // Validate input - show red if invalid
            if (!double.TryParse(TXT_gaugeMax.Text, out _))
            {
                TXT_gaugeMax.BackColor = Color.Red;
            }
            else
            {
                TXT_gaugeMax.BackColor = backup_backcolor;
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

                // Update label to match selected source
                TXT_customlabel.Text = MainV2.comPort.MAV.cs.GetNameandUnit((string)CMB_Source.SelectedValue);

                // Reset scale and offset if we select a new variable
                TXT_scale.Text = "1.0";
                TXT_offset.Text = "0.0";

                // Restore the full list
                CMB_Source.DataSource = all_fields;
                CMB_Source.SelectedItem = field;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            ApplySettingsToQuickView();
        }

        private void ApplySettingsToQuickView()
        {
            // Apply label from form
            _qv.desc = TXT_customlabel.Text;

            // Apply color from form
            _qv.numberColor = BUT_colorpicker.BackColor;
            _qv.numberColorBackup = _qv.numberColor;

            // Apply format from form
            string format = TXT_customformat.Text.Replace(":", "\\:");
            _qv.numberformat = !string.IsNullOrEmpty(format) ? format : "0.00";

            // Apply scale and offset from form
            if (double.TryParse(TXT_scale.Text, out double scale))
                _qv.scale = scale;
            else
                _qv.scale = 1;

            if (double.TryParse(TXT_offset.Text, out double offset))
                _qv.offset = offset;
            else
                _qv.offset = 0;

            // Apply gauge settings from form
            _qv.isGauge = CHK_gauge.Checked;

            if (double.TryParse(TXT_gaugeMin.Text, out double gaugeMin))
                _qv.gaugeMin = gaugeMin;
            else
                _qv.gaugeMin = 0;

            if (double.TryParse(TXT_gaugeMax.Text, out double gaugeMax))
                _qv.gaugeMax = gaugeMax;
            else
                _qv.gaugeMax = 100;

            // Update the data source if changed
            if (CMB_Source.SelectedItem is Tuple<string, string> selectedField)
            {
                string desc = selectedField.Item1;

                // Handle customfield
                if (desc.StartsWith("customfield"))
                {
                    _qv.Tag = "customfield:" + selectedField.Item2;
                }
                else
                {
                    _qv.Tag = desc;
                }

                // Rebind to new source
                _qv.DataBindings.Clear();
                try
                {
                    var bindingSource = _qv.Parent?.Parent?.Controls.OfType<System.Windows.Forms.BindingSource>().FirstOrDefault();
                    if (bindingSource == null)
                    {
                        // Try to find it from the form
                        var form = _qv.FindForm();
                        if (form != null)
                        {
                            var field = form.GetType().GetField("bindingSourceQuickTab",
                                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                            if (field != null)
                            {
                                bindingSource = field.GetValue(form) as System.Windows.Forms.BindingSource;
                            }
                        }
                    }

                    if (bindingSource != null)
                    {
                        var b = new System.Windows.Forms.Binding("number", bindingSource, selectedField.Item1, true);
                        _qv.DataBindings.Add(b);
                    }
                }
                catch { }
            }

            _qv.Invalidate();

            // Trigger save of all dashboard items
            try
            {
                // Find the FlightData instance
                if (MissionPlanner.GCSViews.FlightData.instance != null)
                {
                    // Use reflection to call the private SaveQuickViewArrangement method
                    var method = typeof(MissionPlanner.GCSViews.FlightData).GetMethod("SaveQuickViewArrangement",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    method?.Invoke(MissionPlanner.GCSViews.FlightData.instance, null);
                }
            }
            catch { }
        }
    }
}
