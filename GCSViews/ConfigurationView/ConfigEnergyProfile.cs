using System;
using System.Collections;
using System.Collections.Generic;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Microsoft.Scripting.Utils;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigEnergyProfile : UserControl, IActivate, IDeactivate
    {
        // Init variables for instance
        private Dictionary<int, CurrentModel> CurrentSet { get; set; }
        private Dictionary<int, VelocityModel> VelocitySet { get; set; }
        private CurrentModel CurrentHover { get; set; }
        private static int ComboBoxIndex { get; set; }
        private IConfigEnergyProfile _configEnergyProfile = new EnergyProfileController();

        /// <summary>
        /// Constructor
        /// </summary>
        public ConfigEnergyProfile()
        {
            InitializeComponent();
            SetComboBoxes();
        }
        /// <summary>
        /// set datasources or SelectedIndex for ComboBoxes
        /// </summary>
        private void SetComboBoxes()
        {
            ComboBoxCrntDeviation.DataSource = EnergyProfileModel.DeviationInPercentList;
            ComboBoxCrntDeviation.SelectedItem = EnergyProfileModel.DeviationInPercentList[0];
            CB_Interp_Curr.SelectedIndex = (int)EnergyProfileModel.InterpModeCurr;
            CB_Interp_Vel.SelectedIndex = (int)EnergyProfileModel.InterpModeVel;
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
        /// Set binding on the textboxes
        /// </summary>
        private void VelocityTBDatabindings()
        {
            VelocitySet = EnergyProfileModel.VelocitySet;
            if (VelocityTable != null && VelocitySet != null && EnergyProfileModel.Enabled)
            {
                //loop over all lines backward
                for (int j = 4; j > 1; j--)
                {
                    //loops over all culomns backward
                    for (int i = 11; i > 0; i--)
                    {
                        if (VelocitySet.ContainsKey(key: i))
                        {
                            string tbName;
                            if (i < 10)
                                tbName = "VelTblTb" + j + "0" + i;
                            else
                            {
                                tbName = "VelTblTb" + j + i;
                            }

                            foreach (var textBox in VelocityTable.Controls)
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
                                                    dataSource: VelocitySet[key: i], dataMember: "Angle");
                                                binding.Format +=
                                                    delegate (object sentFrom, ConvertEventArgs convertEventArgs)
                                                    {
                                                        convertEventArgs.Value =
                                                            ((double)convertEventArgs.Value).ToString("0");
                                                    };
                                                break;
                                            case 3:
                                                binding = new Binding(propertyName: "Text",
                                                    dataSource: VelocitySet[key: i], dataMember: "AverageVelocity");
                                                binding.Format +=
                                                    delegate (object sentFrom, ConvertEventArgs convertEventArgs)
                                                    {
                                                        convertEventArgs.Value =
                                                            ((double)convertEventArgs.Value).ToString("0.00")
                                                            .Replace(".", ","); //convert string
                                                    };
                                                break;
                                            case 4:
                                                binding = new Binding(propertyName: "Text",
                                                    dataSource: VelocitySet[key: i], dataMember: "Deviation");
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
                panelVelocityConfiguration.Enabled = true;
                Panel_LogAnalyzer.Enabled = true;
                Lb_LogAnalyzer.BackColor = System.Drawing.SystemColors.Window;
                if (!EnergyProfileModel.Enabled)
                {
                    EnergyProfileModel.Enabled = true;
                    UpdateTables(3);
                }
            }
            else
            {
                ComboBoxCrntDeviation.SelectedIndex = 0;
                panelHover.Enabled = false;
                panelExpImp.Enabled = false;
                panelCurrentConfiguration.Enabled = false;
                panelVelocityConfiguration.Enabled = false;
                Panel_LogAnalyzer.Enabled = false;
                Lb_LogAnalyzer.BackColor = System.Drawing.SystemColors.WindowFrame;
                EnergyProfileModel.Enabled = false;
                ClearChart(new List<Chart> { ChartI, ChartV });
            }
        }

        /// <summary>
        /// Clear all charts in list.
        /// </summary>
        /// <param name="chartlist">A list of charts.</param>
        private void ClearChart(List<Chart> chartlist)
        {
            if (chartlist.Count > 0)
            {
                foreach (Chart chart in chartlist)
                {
                    if (chart.Series.Count > 0)
                    {
                        foreach (Series serie in chart.Series)
                        {
                            serie.Points.Clear();
                            serie.Points.Add(0, -90);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Select a fix deviation for current.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxDeviation_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxIndex = ComboBoxCrntDeviation.SelectedIndex;
            _configEnergyProfile.ChangeDeviation((int)ComboBoxCrntDeviation.SelectedItem);
            UpdateTables(1);
        }

        /// <summary>
        /// Button for import an energy-profile from .xml.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnImport_Click(object sender, EventArgs e)
        {
            bool import = _configEnergyProfile.ImportProfile();
            if (import)
            {
                ComboBoxCrntDeviation.SelectedItem = (int)(EnergyProfileModel.PercentDevCrnt * 100);
                UpdateTables();
            }
        }

        /// <summary>
        /// Button for export the energy-profile into .xml.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExport_Click(object sender, EventArgs e)
        {
            _configEnergyProfile.ExportProfile();
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
                ComboBoxCrntDeviation.SelectedIndex = ComboBoxIndex;
                CB_Interp_Curr.SelectedIndex = (int) EnergyProfileModel.InterpModeCurr;
                CB_Interp_Vel.SelectedIndex = (int)EnergyProfileModel.InterpModeVel;
                //Write back values to settings page
                UpdateTables();
            }
        }

        private void SetInterpMode()
        {
            if (CB_Interp_Curr.Enabled)
            {
                if (CB_Interp_Curr.SelectedIndex.Equals(0))
                    EnergyProfileModel.InterpModeCurr = EnergyProfileModel.InterpolationMode.LinearInterp;
                else if (CB_Interp_Curr.SelectedIndex.Equals(1))
                    EnergyProfileModel.InterpModeCurr = EnergyProfileModel.InterpolationMode.CubicSpline;
            }
            if (CB_Interp_Vel.Enabled)
            {
                if (CB_Interp_Vel.SelectedIndex.Equals(0))
                    EnergyProfileModel.InterpModeVel = EnergyProfileModel.InterpolationMode.LinearInterp;
                else if (CB_Interp_Vel.SelectedIndex.Equals(1))
                    EnergyProfileModel.InterpModeVel = EnergyProfileModel.InterpolationMode.CubicSpline;
            }
        }

        public void Deactivate()
        {
            SetInterpMode();
            bool validCurr = _configEnergyProfile.Interpolation(EnergyProfileController.PlotProfile.Current);
            bool validVel = _configEnergyProfile.Interpolation(EnergyProfileController.PlotProfile.Velocity);
            if (!validCurr)
            {
                CustomMessageBox.Show(
                    "One or more values are incorrect in current-model. The energy-profile doesn't work.");
            }
            else if (!validVel)
            { 
                CustomMessageBox.Show(
                    "One or more values are incorrect in velocity-model. The energy-profile doesn't work.");
            }
            else if (EnergyProfileModel.CurrentHover.AverageCurrent.Equals(0))
            {
                CustomMessageBox.Show(
                    "Hover value can't be null. The energy-profile doesn't work.");
            }
        }

        /// <summary>
        /// Plot current-chart.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCrntPlot_Click(object sender, EventArgs e)
        {
            bool validCurr = _configEnergyProfile.Interpolation(EnergyProfileController.PlotProfile.Current);
            if (validCurr)
            {
                _configEnergyProfile.Plot(ChartI, EnergyProfileController.PlotProfile.Current);
            }
            else
            {
                CustomMessageBox.Show(
                    "One or more values are incorrect in current-model. The energy-profile doesn't work.");
            }
        }

        /// <summary>
        /// Plot velocity-chart.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnPlotVelocity_Click(object sender, EventArgs e)
        {
            bool validVel = _configEnergyProfile.Interpolation(EnergyProfileController.PlotProfile.Velocity);
            if (validVel)
            {
            _configEnergyProfile.Plot(ChartV, EnergyProfileController.PlotProfile.Velocity);
            }
            else
            {
                CustomMessageBox.Show(
                    "One or more values are incorrect in velocity-model. The energy-profile doesn't work.");
            }
        }

        /// <summary>
        /// Button for loading logfiles into list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_LoadLogfile_Click(object sender, EventArgs e)
        {
            ArrayList selectedItems = new ArrayList();
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "Log Files|*.log;*.bin";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;

                openFileDialog1.InitialDirectory = Settings.Instance.LogDir;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (var fileName in openFileDialog1.FileNames)
                    {
                        bool doubleEntree = false;
                        string file = System.IO.Path.GetFileName(fileName);
                        foreach (var item in Lb_LogAnalyzer.Items)
                        {
                            if (System.IO.Path.GetFileName(item.ToString()).Equals(file))
                            {
                                doubleEntree = true;
                                break;
                            }
                        }
                        if (!doubleEntree)
                            selectedItems.Add(fileName);

                    }
                }
            }
            selectedItems.AddRange(Lb_LogAnalyzer.Items);
            selectedItems.Sort();
            Lb_LogAnalyzer.Items.Clear();
            foreach (object selectedItem in selectedItems)
            {
                Lb_LogAnalyzer.Items.Add(selectedItem);
            }
            if (Lb_LogAnalyzer.Items.Count > 0)
                Btn_Analyze.Enabled = true;
        }

        /// <summary>
        /// Button for delete logfiles from list.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_DeleteLogfile_Click(object sender, EventArgs e)
        {
            if (Lb_LogAnalyzer.SelectedItems.Count.Equals(Lb_LogAnalyzer.Items.Count))
            {
                Lb_LogAnalyzer.Items.Clear();
                Btn_Analyze.Enabled = false;
            }
            else
            {
                for (int selectedIndex = Lb_LogAnalyzer.SelectedIndices.Count - 1; selectedIndex >= 0; selectedIndex--)
                {
                    Lb_LogAnalyzer.Items.RemoveAt(Lb_LogAnalyzer.SelectedIndices[selectedIndex]);
                }
            }
        }

        /// <summary>
        /// Button for Logfile - Analyze
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Btn_Analyze_Click(object sender, EventArgs e)
        {
            // clear actual charts
            ClearChart(new List<Chart> { ChartI, ChartV });
            List<string> filenames = new List<string>();
            foreach (var item in Lb_LogAnalyzer.Items)
            {
                filenames.Add(item.ToString());
            }
            _configEnergyProfile.SetTransitionState(ChB_CurrentTransition.Checked, ChB_SpeedTransition.Checked, Convert.ToInt16(tb_cmdflighttime.Text));
            bool validAnalyze = _configEnergyProfile.AnalyzeLogs(filenames, Convert.ToInt16(tb_minval.Text), Convert.ToInt16(tb_transtime.Text));
            

            // update view
            if (validAnalyze)
            {
                UpdateTables();
            }
            else
            {
                // plot only if analyse is valid, that user don't confused
                UpdateTables(2);
            }
        }

        /// <summary>
        /// Update the Tables an Charts.
        /// </summary>
        /// <param name="section">0 = all table and charts (default), 1 = only current-table and plot, 2 = only current and velocity tables without ploting</param>
        private void UpdateTables(int section = 0)
        {
            SetInterpMode();
            switch (section)
            {
                case 0:
                    CrntTBDatabindings();
                    VelocityTBDatabindings();
                    //if (validprofile)
                    //{
                    //    BtnPlotCrnt.PerformClick();
                    //    BtnPlotVelocity.PerformClick();
                    //}
                    //else
                    //{
                    //    CustomMessageBox.Show(
                    //        "One or more values are incorrect. The energy-profile doesn't work.");
                    //}
                    break;
                case 1:
                    CrntTBDatabindings();
                    //if (validprofile)
                    //    BtnPlotCrnt.PerformClick();
                    //else
                    //{
                    //    CustomMessageBox.Show(
                    //        "One or more values are incorrect in current-model. The energy-profile doesn't work.");
                    //}
                    break;
                case 2:
                    CrntTBDatabindings();
                    //if (validprofile)
                    //    VelocityTBDatabindings();
                    //else
                    //{
                    //    CustomMessageBox.Show(
                    //        "One or more values are incorrect in velocity-model. The energy-profile doesn't work.");
                    //}
                    break;
                case 3:
                    CrntTBDatabindings();
                    VelocityTBDatabindings();
                    break;
            }
        }

        private void CB_Interp_Curr_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetInterpMode();
        }

        private void CB_Interp_Vel_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetInterpMode();
        }
    }
}


