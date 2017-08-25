using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System.Windows.Forms;
using System.Globalization;
using System.Windows.Forms.DataVisualization.Charting;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigEnergyProfile : UserControl, IActivate
    {
        private Dictionary<int, CurrentModel> CurrentSet { get; set; }
        private CurrentModel CurrentHover { get; set; }
        private static int ComboBoxIndex { get; set; }

        private IConfigEnergyProfile _configEnergyProfile = new EnergyProfileController();

        public ConfigEnergyProfile()
        {
            InitializeComponent();
            SetDataSources();
        }
        /// <summary>
        /// set datasources for percent dev 
        /// </summary>
        private void SetDataSources()
        {
            ComboBoxDeviation.DataSource = EnergyProfileModel.DeviationInPercentList;
            ComboBoxDeviation.SelectedItem = EnergyProfileModel.DeviationInPercentList[0];
        }
        /// <summary>
        /// Set binding on the textboxes
        /// </summary>
        private void CrntTBDatabindings()
        {
            CurrentHover = EnergyProfileModel.CurrentHover;
            CurrentSet = EnergyProfileModel.CurrentSet;
            if (CrntTable != null && CurrentSet != null && EnergyProfileModel.Enabled)
            {
                //loop over all lines backward
                for (int j = 4; j > 1; j--)
                {
                    //loops over all culomns backward
                    for (int i = 11; i > 0; i--)
                    {
                        if (CurrentSet.ContainsKey(key: i))
                        {
                            string tbName;
                            if (i < 10)
                                tbName = "CrntTblTb" + j + "0" + i;
                            else
                            {
                                tbName = "CrntTblTb" + j + i;
                            }

                            foreach (var textBox in CrntTable.Controls)
                            {
                                if (textBox.GetType() == typeof(TextBox))
                                {
                                    if (((TextBox)textBox).Name == tbName)
                                    {
                                        ((TextBox)textBox).ResetBindings();
                                        Binding binding = null;
                                        switch (j)
                                        {

                                            case 2:
                                                binding = new Binding(propertyName: "Text",
                                                    dataSource: CurrentSet[key: i], dataMember: "Angle");
                                                binding.Format +=
                                                    delegate (object sentFrom, ConvertEventArgs convertEventArgs)
                                                    {
                                                        convertEventArgs.Value = ((double)convertEventArgs.Value).ToString("0");
                                                    };
                                                break;
                                            case 3:
                                                binding = new Binding(propertyName: "Text",
                                                    dataSource: CurrentSet[key: i], dataMember: "AverageCurrent");
                                                binding.Format +=
                                                    delegate (object sentFrom, ConvertEventArgs convertEventArgs)
                                                    {
                                                        convertEventArgs.Value =
                                                            ((double)convertEventArgs.Value).ToString("0.00")
                                                            .Replace(".", ",");//convert string
                                                    };
                                                break;
                                            case 4:
                                                binding = new Binding(propertyName: "Text",
                                                    dataSource: CurrentSet[key: i], dataMember: "Deviation");
                                                binding.Format +=
                                                    delegate (object sentFrom, ConvertEventArgs convertEventArgs)
                                                    {
                                                        convertEventArgs.Value =
                                                            ((double)convertEventArgs.Value).ToString("0.00")
                                                            .Replace(".", ","); //convert string
                                                    };

                                                break;
                                        }
                                        if (binding != null && ((TextBox)textBox).DataBindings.Count == 0)
                                            ((TextBox)textBox).DataBindings.Add(binding);
                                        ((TextBox)textBox).GotFocus += RemoveText;
                                        ((TextBox)textBox).LostFocus += AddText;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (CurrentHover != null)
            {
                // bindings for HoverCrntTextbox
                HoverCrntTB.ResetBindings();
                var bindingCurrent = new Binding(propertyName: "Text", dataSource: CurrentHover, dataMember: "AverageCurrent");
                bindingCurrent.Format += delegate (object sentFrom, ConvertEventArgs convertEventArgs)
                {
                    convertEventArgs.Value = ((double)convertEventArgs.Value).ToString("0.00").Replace(".", ",");//convert string
                };

                if (HoverCrntTB.DataBindings.Count == 0)
                {
                    HoverCrntTB.DataBindings.Add(bindingCurrent);
                }
                HoverCrntTB.GotFocus += RemoveText;
                HoverCrntTB.LostFocus += AddText;

                // bindings for HoverDevTextbox
                HoverDevTB.ResetBindings();
                var bindingDev = new Binding(propertyName: "Text", dataSource: CurrentHover, dataMember: "Deviation");
                bindingDev.Format += delegate (object sentFrom, ConvertEventArgs convertEventArgs)
                {
                    convertEventArgs.Value = ((double)convertEventArgs.Value).ToString("0.00").Replace(".", ",");//convert string
                };

                if (HoverDevTB.DataBindings.Count == 0)
                {
                    HoverDevTB.DataBindings.Add(bindingDev);
                }
                HoverDevTB.GotFocus += RemoveText;
                HoverDevTB.LostFocus += AddText;
            }
        }
        /// <summary>
        /// Remove text from a specific TextBox
        /// </summary>
        /// <param name="sender">TextBox</param>
        /// <param name="e">Event -> GetFocus</param>
        public void RemoveText(object sender, EventArgs e)
        {
            TextBox myTextBox = ((TextBox)sender);
            if (myTextBox.Text == @"0,00")
                myTextBox.Text = "";
        }
        /// <summary>
        /// Add text to a specific TextBox.
        /// </summary>
        /// <param name="sender">TextBox</param>
        /// <param name="e">Event -> LostFocus</param>
        public void AddText(object sender, EventArgs e)
        {
            TextBox myTextBox = ((TextBox)sender);
            if (!myTextBox.Name.Contains("CrntTblTb2"))
            {
                double d;
                myTextBox.Text =
                    string.IsNullOrWhiteSpace(myTextBox.Text) ||
                    !double.TryParse(myTextBox.Text.Replace('.', ','), out d)
                        ? @"0,00"
                        : d.ToString(@"0.00").Replace('.', ',');
            }
        }

        // Event_Trigger_Functions
        /// <summary>
        /// trigger event for change trigger-state
        /// </summary>
        /// <param name="sender">checkbox</param>
        /// <param name="e">statechanged</param>
        private void CB_EnableEnergyProfile_CheckStateChanged(object sender, EventArgs e)
        {
            if (CB_EnableEnergyProfile.Checked)
            {
                panelHover.Enabled = true;
                panelExpImp.Enabled = true;
                panelCurrentConfiguration.Enabled = true;
                if (!EnergyProfileModel.Enabled)
                {
                    EnergyProfileModel.Enabled = true;
                    CrntTBDatabindings();
                }
            }
            else
            {
                ComboBoxDeviation.SelectedIndex = 0;
                panelHover.Enabled = false;
                panelExpImp.Enabled = false;
                panelCurrentConfiguration.Enabled = false;
                EnergyProfileModel.Enabled = false;
            }
        }

        private void ComboBoxDeviation_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxIndex = ComboBoxDeviation.SelectedIndex;
            _configEnergyProfile.ChangeDeviation((int)ComboBoxDeviation.SelectedItem);
            CrntTBDatabindings();
        }

        private void BtnImport_Click(object sender, EventArgs e)
        {
            _configEnergyProfile.ImportProfile();
            ComboBoxDeviation.SelectedItem = (int)(EnergyProfileModel.PercentDev * 100);
            CrntTBDatabindings();
            _configEnergyProfile.PlotCurrent_Spline(ChartI);
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            _configEnergyProfile.ExportProfile();
        }

        /// <summary>
        /// save setting if user leave the energy-config-form
        /// </summary>
        /// <param name="sender">form</param>
        /// <param name="e">leave</param>
        private void ConfigEnergyProfile_Leave(object sender, EventArgs e)
        {
            _configEnergyProfile.LinearInterpolation();
        }

        private void BtnPlot_Click(object sender, EventArgs e)
        {
            _configEnergyProfile.PlotCurrent_Spline(ChartI);
        }

        // Private functions
        /// <summary>
        /// Update funtion for activated the form
        /// </summary>
        public void Activate()
        {
            CB_EnableEnergyProfile.Checked = EnergyProfileModel.Enabled;
            if (EnergyProfileModel.Enabled)
            {
                ComboBoxDeviation.SelectedIndex = ComboBoxIndex;
                //Write back values to settings page
                CrntTBDatabindings();
            }

        }
    }
}


