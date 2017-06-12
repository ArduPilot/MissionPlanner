using System;
using System.Collections.Generic;
using System.IO;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System.Windows.Forms;
using System.Globalization;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigEnergyProfile : UserControl, IActivate
    {
        /// <summary>
        /// local values for velocity
        /// </summary>
        private double _dAmplitudeV, _dLowerBoundV, _dAmpPosV, _dVarianceV, _dGradientV;

        /// <summary>
        /// local values for current
        /// </summary>
        private double _dNegativeAmplitudeI, _dNegativeAmpAngleI, _dNegativeVarianceI, _dPositiveAmplitudeI, _dPositiveAmpAngleI, _dPositiveVarianceI, _dLowerLimitI, _dHover, _dDevMax, _dDevMin;

        /// <summary>
        /// path for saving and load the energyprofile 
        /// </summary>
        private readonly string _energyProfilePath;

        public ConfigEnergyProfile()
        {
            InitializeComponent();
            _energyProfilePath = Settings.GetUserDataDirectory() + "EnergyProfile" + Path.DirectorySeparatorChar;
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
                if (!EnergyProfile.Initialized)
                {
                    EnergyProfile.Initialize();
                }

                panelIDHover.Enabled = true;
                panelCurrentConfiguration.Enabled = true;
                panelVelocityConfiguration.Enabled = true;

                if (!EnergyProfile.Enabled)
                {
                    EnergyProfile.Enabled = true;
                    LoadCopterFileSettings(true);
                }
            }
            else
            {
                panelIDHover.Enabled = false;
                panelCurrentConfiguration.Enabled = false;
                panelVelocityConfiguration.Enabled = false;
                EnergyProfile.Enabled = false;
            }
        }

        /// <summary>
        /// save the enrgyprofile of the copter in a file
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click</param>
        private void btnSaveCopterSettingsToFile_Click(object sender, EventArgs e)
        {
            SaveCopterFileSettings();
        }

        /// <summary>
        /// save setting if user leave the energy-config-form
        /// </summary>
        /// <param name="sender">form</param>
        /// <param name="e">leave</param>
        private void ConfigEnergyProfile_Leave(object sender, EventArgs e)
        {
            SaveEnergySetting();
        }

        /// <summary>
        /// load the energyprofile from copter out of file
        /// </summary>
        /// <param name="sender">button</param>
        /// <param name="e">click</param>
        private void btnLoadCopterSettings_Click(object sender, EventArgs e)
        {
            LoadCopterFileSettings(false);
        }

        /// <summary>
        /// only for textboxes in current-table
        /// </summary>
        /// <param name="sender">textbox</param>
        /// <param name="e">textchanged</param>
        private void Current_TextBoxes_TextChanged(object sender, EventArgs e)
        {
            TextBox tbChangedBox = ((TextBox)sender);

            if (!System.Text.RegularExpressions.Regex.Match(tbChangedBox.Text, @"^-{0,1}\d+\.{0,1}\d*$").Success)   //Match floating point number
            {
                if (tbChangedBox.TextLength > 0)
                {
                    tbChangedBox.Text = tbChangedBox.Text.Substring(0, tbChangedBox.TextLength - 1);    //remove last invalid input
                }
                else
                {
                    tbChangedBox.Text = string.Empty;
                }
            }
            else
            {
                if (ParseCurrentValues()) { PlotCurrent(); }
            }
        }

        /// <summary>
        /// only for textboxes in velocity-table
        /// </summary>
        /// <param name="sender">textbox</param>
        /// <param name="e">textchanged</param>
        private void Velocity_TextBoxes_TextChanged(object sender, EventArgs e)
        {
            TextBox tbChangedBox = ((TextBox)sender);

            if (!System.Text.RegularExpressions.Regex.Match(tbChangedBox.Text, @"^-{0,1}\d+\.{0,1}\d*$").Success)   //Match floating point number
            {
                if (tbChangedBox.TextLength > 0)
                {
                    tbChangedBox.Text = tbChangedBox.Text.Substring(0, tbChangedBox.TextLength - 1);    //remove last invalid input
                }
                else
                {
                    tbChangedBox.Text = string.Empty;
                }
            }
            else
            {
                if (ParseVelocityValues()) { PlotVelocity(); }
            }
        }

        // Private functions
        /// <summary>
        /// Update funtion for activated the form
        /// </summary>
        public void Activate()
        {
            CB_EnableEnergyProfile.Checked = EnergyProfile.Enabled;
            if (!EnergyProfile.Initialized) { EnergyProfile.Initialize(); }
            if (EnergyProfile.Enabled)
            {
                //Write back values to settings page
                LoadEnergyProfileValues();
            }
        }

        /// <summary>
        /// Load values into fields
        /// </summary>
        private void LoadEnergyProfileValues()
        {
            //Current
            try
            {
                //current
                tbAmpINeg.Text = EnergyProfile.Current["NegativeAmplitude"].ToString(CultureInfo.GetCultureInfo("en-US"));
                tbAngINeg.Text = EnergyProfile.Current["NegativeAmpAngle"].ToString(CultureInfo.GetCultureInfo("en-US"));
                tbVarINeg.Text = EnergyProfile.Current["NegativeVariance"].ToString(CultureInfo.GetCultureInfo("en-US"));
                tbAmpIPos.Text = EnergyProfile.Current["PositiveAmplitude"].ToString(CultureInfo.GetCultureInfo("en-US"));
                tbAngIPos.Text = EnergyProfile.Current["PositiveAmpAngle"].ToString(CultureInfo.GetCultureInfo("en-US"));
                tbVarIPos.Text = EnergyProfile.Current["PositiveVariance"].ToString(CultureInfo.GetCultureInfo("en-US"));
                tbDeviationMax.Text = EnergyProfile.Current["MaxDeviation"].ToString(CultureInfo.GetCultureInfo("en-US"));
                tbDeviationMin.Text = EnergyProfile.Current["MinDeviation"].ToString(CultureInfo.GetCultureInfo("en-US"));
                tbLimitI.Text = EnergyProfile.Current["LowerLimit"].ToString(CultureInfo.GetCultureInfo("en-US"));
                tbHoverI.Text = EnergyProfile.Current["Hover"].ToString(CultureInfo.GetCultureInfo("en-US"));
                //velocity
                tbAmpV.Text = EnergyProfile.Velocity["Amplitude"].ToString(CultureInfo.GetCultureInfo("en-US"));
                tbAngV.Text = EnergyProfile.Velocity["AmpPosition"].ToString(CultureInfo.GetCultureInfo("en-US"));
                tbVarV.Text = EnergyProfile.Velocity["Variance"].ToString(CultureInfo.GetCultureInfo("en-US"));
                tbLowerAmp.Text = EnergyProfile.Velocity["LowerBound"].ToString(CultureInfo.GetCultureInfo("en-US"));
                tbGradientV.Text = EnergyProfile.Velocity["Gradient"].ToString(CultureInfo.GetCultureInfo("en-US"));
            }
            catch (KeyNotFoundException e)
            {
                CustomMessageBox.Show("Error while parsing key from Energyprofile: " + e.Message);
            }
            ParseCurrentValues();
            ParseVelocityValues();
        }

        /// <summary>
        /// Energyprofile for copter
        /// </summary>
        /// <param name="bLoadAutomatically"></param>
        private void LoadCopterFileSettings(bool bLoadAutomatically)
        {
            string filepath;

            if (bLoadAutomatically)
            {
                if (MainV2.comPort.MAV.param.ContainsKey("BRD_SERIAL_NUM")) //Check if copterID is existent
                {
                    double dCopterId = MainV2.comPort.GetParam("BRD_SERIAL_NUM");
                    filepath = _energyProfilePath + dCopterId.ToString(CultureInfo.InvariantCulture) + ".xml";
                }
                else //an automatic load of values is not possible without BRD_SERIAL_NUM
                {
                    return;
                }
            }
            else //!_bLoadAutomatically => Let user load his file manually
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = _energyProfilePath;
                ofd.Multiselect = false;
                ofd.Filter = @"Energyprofile settings (*.xml)|*.xml";
                ofd.DefaultExt = ".xml";

                if (DialogResult.OK == ofd.ShowDialog())
                {
                    filepath = ofd.FileName;
                }
                else
                {
                    return;
                }
            }
            //load copter settings, if available => filename: [CopterID].xml
            if (File.Exists(filepath) && tbCopterID.Text != string.Empty)
            {
                using (System.Xml.XmlReader xr = System.Xml.XmlReader.Create(filepath))
                {
                    try
                    {
                        xr.ReadToFollowing("EnergyProfile");
                        xr.ReadToFollowing("Current");

                        tbAmpINeg.Text = xr.GetAttribute("NegativeAmplitude");
                        tbAngINeg.Text = xr.GetAttribute("NegativeAmpAngle");
                        tbVarINeg.Text = xr.GetAttribute("NegativeVariance");
                        tbAmpIPos.Text = xr.GetAttribute("PositiveAmplitude");
                        tbAngIPos.Text = xr.GetAttribute("PositiveAmpAngle");
                        tbVarIPos.Text = xr.GetAttribute("PositiveVariance");
                        tbDeviationMax.Text = xr.GetAttribute("MaxDeviation");
                        tbDeviationMin.Text = xr.GetAttribute("MinDeviation");
                        tbLimitI.Text = xr.GetAttribute("LowerLimit");
                        tbHoverI.Text = xr.GetAttribute("Hover");

                        xr.ReadToFollowing("Velocity");
                        tbAmpV.Text = xr.GetAttribute("Amplitude");
                        tbAngV.Text = xr.GetAttribute("AmpPosition");
                        tbVarV.Text = xr.GetAttribute("Variance");
                        tbLowerAmp.Text = xr.GetAttribute("LowerBound");
                        tbGradientV.Text = xr.GetAttribute("Gradient");
                        xr.Close();

                        ParseCurrentValues();
                        ParseVelocityValues();
                    }
                    catch (System.Xml.XmlException)
                    {
                        CustomMessageBox.Show("Error reading XmlFile");
                    }
                }
                SaveEnergySetting();
            }
            
        }

        /// <summary>
        /// Save settings to xml-file
        /// </summary>
        private void SaveCopterFileSettings()
        {
            string sFile;
            int iSerialNum = 0;

            if (MainV2.comPort.MAV.param.ContainsKey("BRD_SERIAL_NUM"))
            {
                iSerialNum = (int)MainV2.comPort.GetParam("BRD_SERIAL_NUM");
            }

            //Save parameters to xml file
            if (!(Directory.Exists(_energyProfilePath)))
            {
                Directory.CreateDirectory(_energyProfilePath);
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = _energyProfilePath;
            sfd.FileName = iSerialNum.ToString();   //file is saved with the id to allow automatic loading of values if id is set in copter
            sfd.AddExtension = true;
            sfd.Filter = @"Energyprofile settings (*.xml)|*.xml";
            sfd.DefaultExt = ".xml";

            if (DialogResult.OK == sfd.ShowDialog())
            {
                sFile = sfd.FileName;
            }
            else
            {
                return;
            }

            using (System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(sFile))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("EnergyProfile");

                xw.WriteStartElement("Current");
                xw.WriteAttributeString("NegativeAmplitude", tbAmpINeg.Text);
                xw.WriteAttributeString("NegativeAmpAngle", tbAngINeg.Text);
                xw.WriteAttributeString("NegativeVariance", tbVarINeg.Text);
                xw.WriteAttributeString("PositiveAmplitude", tbAmpIPos.Text);
                xw.WriteAttributeString("PositiveAmpAngle", tbAngIPos.Text);
                xw.WriteAttributeString("PositiveVariance", tbVarIPos.Text);
                xw.WriteAttributeString("MaxDeviation", tbDeviationMax.Text);
                xw.WriteAttributeString("MinDeviation", tbDeviationMin.Text);
                xw.WriteAttributeString("LowerLimit", tbLimitI.Text);
                xw.WriteAttributeString("Hover", tbHoverI.Text);
                xw.WriteEndElement();

                xw.WriteStartElement("Velocity");
                xw.WriteAttributeString("Amplitude", tbAmpV.Text);
                xw.WriteAttributeString("AmpPosition", tbAngV.Text);
                xw.WriteAttributeString("Variance", tbVarV.Text);
                xw.WriteAttributeString("LowerBound", tbLowerAmp.Text);
                xw.WriteAttributeString("Gradient", tbGradientV.Text);
                xw.WriteEndElement();

                xw.WriteEndElement();
                xw.WriteEndDocument();
                xw.Close();
            }
            CustomMessageBox.Show("Saved settings to " + sFile);
        }

        /// <summary>
        /// ensure textboxes contain values
        /// </summary>
        /// <returns>Boolean: false => if invalid format or textbox empty</returns>
        private bool ParseVelocityValues()
        {
            if (!double.TryParse(tbAmpV.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dAmplitudeV) ||
                !double.TryParse(tbLowerAmp.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dLowerBoundV) ||
                !double.TryParse(tbAngV.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dAmpPosV) ||
                !double.TryParse(tbVarV.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dVarianceV) ||
                !double.TryParse(tbGradientV.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dGradientV))
            {
                CustomMessageBox.Show("Velocity: Invalid format or textbox empty!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// ensure textboxes contain values
        /// </summary>
        /// <returns>Boolean: false => if invalid format or textbox empty</returns>
        private bool ParseCurrentValues()
        {
            if (!double.TryParse(tbAmpINeg.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dNegativeAmplitudeI) ||
                !double.TryParse(tbAngINeg.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dNegativeAmpAngleI) ||
                !double.TryParse(tbVarINeg.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dNegativeVarianceI) ||
                !double.TryParse(tbAmpIPos.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dPositiveAmplitudeI) ||
                !double.TryParse(tbAngIPos.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dPositiveAmpAngleI) ||
                !double.TryParse(tbVarIPos.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dPositiveVarianceI) ||
                !double.TryParse(tbLimitI.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dLowerLimitI) ||
                !double.TryParse(tbHoverI.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dHover) ||
                !double.TryParse(tbDeviationMax.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dDevMax) ||
                !double.TryParse(tbDeviationMin.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out _dDevMin)
                )
            {
                CustomMessageBox.Show("Current: Invalid format or textbox empty!");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Save settings to energyprofile class
        /// </summary>
        private void SaveEnergySetting()
        {
            if (!ParseCurrentValues() || !ParseVelocityValues())
            {
                CustomMessageBox.Show("Error saving values. Please check your fields");
                return;
            }
            //Write values to the energyprofile class
            EnergyProfile.Current["NegativeAmplitude"] = _dNegativeAmplitudeI;
            EnergyProfile.Current["NegativeAmpAngle"] = _dNegativeAmpAngleI;
            EnergyProfile.Current["NegativeVariance"] = _dNegativeVarianceI;
            EnergyProfile.Current["PositiveAmplitude"] = _dPositiveAmplitudeI;
            EnergyProfile.Current["PositiveAmpAngle"] = _dPositiveAmpAngleI;
            EnergyProfile.Current["PositiveVariance"] = _dPositiveVarianceI;
            EnergyProfile.Current["LowerLimit"] = _dLowerLimitI;
            EnergyProfile.Current["Hover"] = _dHover;
            EnergyProfile.Current["MaxDeviation"] = _dDevMax;
            EnergyProfile.Current["MinDeviation"] = _dDevMin;

            EnergyProfile.Velocity["Amplitude"] = _dAmplitudeV;
            EnergyProfile.Velocity["LowerBound"] = _dLowerBoundV;
            EnergyProfile.Velocity["AmpPosition"] = _dAmpPosV;
            EnergyProfile.Velocity["Variance"] = _dVarianceV;
            EnergyProfile.Velocity["Gradient"] = _dGradientV;
        }

        /// <summary>
        /// plot the velocity
        /// </summary>
        private void PlotVelocity()
        {
            ChartV.Series[0].Points.Clear();

            for (double angle = -90; angle <= 90; angle += 5)
            {
                ChartV.Series[0].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(angle, EnergyProfile.PolyValV(angle, _dAmplitudeV, _dAmpPosV, _dVarianceV, _dGradientV, _dLowerBoundV)));
            }

            //scale graph
            var findMinByValue = ChartV.Series[0].Points.FindMinByValue();
            if (findMinByValue != null)
                ChartV.ChartAreas[0].AxisY.Minimum = findMinByValue.YValues[0] - 0.5;
            var findMaxByValue = ChartV.Series[0].Points.FindMaxByValue();
            if (findMaxByValue != null)
                ChartV.ChartAreas[0].AxisY.Maximum = findMaxByValue.YValues[0] + 0.5;
            ChartV.ChartAreas[0].AxisY.LabelStyle.Format = "{0}";
        }

        /// <summary>
        /// plot the current
        /// </summary>
        private void PlotCurrent()
        {
            try
            {
                /*
                 * Series 0: Average Current
                 * Series 1: Max Current (Current + MaxDeviation)
                 * Series 2: Min Current (Current - MinDeviation)
                 */

                ChartI.Series[0].Points.Clear();
                ChartI.Series[1].Points.Clear();
                ChartI.Series[2].Points.Clear();
                var tolerance = 0;
                for (double angle = -90; angle <= 90; angle += 5)
                {
                    ChartI.Series[0].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(angle, EnergyProfile.PolyValI(angle, _dPositiveAmplitudeI, _dPositiveAmpAngleI, _dPositiveVarianceI, _dNegativeAmplitudeI, _dNegativeAmpAngleI, _dNegativeVarianceI, _dLowerLimitI)));

                    if (Math.Abs(_dDevMax) > tolerance) { ChartI.Series[1].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(angle, EnergyProfile.PolyValI(angle, _dPositiveAmplitudeI, _dPositiveAmpAngleI, _dPositiveVarianceI, _dNegativeAmplitudeI, _dNegativeAmpAngleI, _dNegativeVarianceI, _dLowerLimitI, _dDevMax))); }
                    if (Math.Abs(_dDevMin) > tolerance) { ChartI.Series[2].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(angle, EnergyProfile.PolyValI(angle, _dPositiveAmplitudeI, _dPositiveAmpAngleI, _dPositiveVarianceI, _dNegativeAmplitudeI, _dNegativeAmpAngleI, _dNegativeVarianceI, _dLowerLimitI, -_dDevMin))); }
                }

                //scale graph
                var findMinByValue = ChartI.Series[0].Points.FindMinByValue();
                if (findMinByValue != null)
                    ChartI.ChartAreas[0].AxisY.Minimum = findMinByValue.YValues[0] - 0.5;
                var findMaxByValue = ChartI.Series[0].Points.FindMaxByValue();
                if (findMaxByValue != null)
                    ChartI.ChartAreas[0].AxisY.Maximum = findMaxByValue.YValues[0] + 0.5;

                if (Math.Abs(_dDevMin) > tolerance)
                {
                    var minByValue = ChartI.Series[2].Points.FindMinByValue();
                    if (minByValue != null)
                        ChartI.ChartAreas[0].AxisY.Minimum = minByValue.YValues[0] - 0.5;
                }
                if (Math.Abs(_dDevMax) > tolerance)
                {
                    var maxByValue = ChartI.Series[1].Points.FindMaxByValue();
                    if (maxByValue != null)
                        ChartI.ChartAreas[0].AxisY.Maximum = maxByValue.YValues[0] + 0.5;
                }
                ChartI.ChartAreas[0].AxisY.LabelStyle.Format = "{0}";
            }
            catch (Exception e) { CustomMessageBox.Show(e.Message); }
        }
    }
}