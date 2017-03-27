using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System.Windows.Forms;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigEnergyProfile : UserControl, IActivate
    {
        public ConfigEnergyProfile()
        {
            InitializeComponent();

            EnergyProfile.EnergyProfilePath = MissionPlanner.Utilities.Settings.GetUserDataDirectory() + "EnergyProfile" + Path.DirectorySeparatorChar;
        }

        public void Activate()
        {
            CB_EnableEnergyProfile.Checked = EnergyProfile.Enabled;
            if (!EnergyProfile.Initialized) { EnergyProfile.Initialize(); }
            if (EnergyProfile.Enabled == true)
            {
                //Write back values to settings page
                LoadEnergyProfileValues();
            }
            
        }

        private void LoadEnergyProfileValues()  //Load values into fields
        {
            //Current
            try
            {
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
        
        private void LoadCopterFileSettings(bool _bLoadAutomatically)   //Energyprofile for copter
        {
            string filepath = string.Empty;

            if (_bLoadAutomatically)
            {
                if (MainV2.comPort.MAV.param.ContainsKey("BRD_SERIAL_NUM")) //Check if copterID is existent
                {
                    double dCopterID = MainV2.comPort.GetParam("BRD_SERIAL_NUM");

                    filepath = EnergyProfile.EnergyProfilePath + dCopterID.ToString() + ".xml";
                }
                else //an automatic load of values is not possible without BRD_SERIAL_NUM
                {
                    return;
                }
            }
            else //!_bLoadAutomatically => Let user load his file manually
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = EnergyProfile.EnergyProfilePath;
                ofd.Multiselect = false;
                ofd.Filter = "Energyprofile settings (*.xml)|*.xml";
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

                            //update visuals
                            PlotVelocity();
                            PlotCurrent();
                        }
                        catch (System.Xml.XmlException)
                        {
                            CustomMessageBox.Show("Error reading XmlFile");
                        }
                    }
                }
        }

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
            if (!CB_EnableEnergyProfile.Checked)
            {
                panelIDHover.Enabled = false;
                panelCurrentConfiguration.Enabled = false;
                panelVelocityConfiguration.Enabled = false;
                EnergyProfile.Enabled = false;
            }
        }

        //Save settings to xml-file
        private void btnSaveCopterSettingsToFile_Click(object sender, EventArgs e)
        {
            string sFile = string.Empty;
            int iSerialNum = 0;

            if (MainV2.comPort.MAV.param.ContainsKey("BRD_SERIAL_NUM"))
            {
                iSerialNum = (int)MainV2.comPort.GetParam("BRD_SERIAL_NUM");
            }

            //Save parameters to xml file
            if (!(Directory.Exists(EnergyProfile.EnergyProfilePath)))
            {
                Directory.CreateDirectory(EnergyProfile.EnergyProfilePath);
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.InitialDirectory = EnergyProfile.EnergyProfilePath;
            sfd.FileName = iSerialNum.ToString();   //file is saved with the id to allow automatic loading of values if id is set in copter
            sfd.AddExtension = true;
            sfd.Filter = "Energyprofile settings (*.xml)|*.xml";
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

        //ensure textboxes contain values
        private bool ParseVelocityValues()
        {
            if (!double.TryParse(tbAmpV.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dAmplitudeV) ||
                !double.TryParse(tbLowerAmp.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dLowerAmpV)||
                !double.TryParse(tbAngV.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dAngleV) ||
                !double.TryParse(tbVarV.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dVarianceV) ||
                !double.TryParse(tbGradientV.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dGradientV))
            {
                CustomMessageBox.Show("Velocity: Invalid format or textbox empty!");
                return false;
            }
            
            return true;
        }

        //ensure textboxes contain values
        private bool ParseCurrentValues()
        {
            if (!double.TryParse(tbAmpINeg.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dAmplitudeNegI)   ||
                !double.TryParse(tbAngINeg.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dAngleNegI)       ||
                !double.TryParse(tbVarINeg.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dVarianceNegI)    ||
                !double.TryParse(tbAmpIPos.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dAmplitudePosI)   ||
                !double.TryParse(tbAngIPos.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dAnglePosI)       ||
                !double.TryParse(tbVarIPos.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dVariancePosI)    ||
                !double.TryParse(tbLimitI.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dLowerLimitI)      ||
                !double.TryParse(tbHoverI.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dHover)            ||
                !double.TryParse(tbDeviationMax.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dDevMax)     ||
                !double.TryParse(tbDeviationMin.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dDevMin)
                )
            {
                CustomMessageBox.Show("Current: Invalid format or textbox empty!");
                return false;
            }
            
            return true;
        }

        //Save settings to energyprofile class
        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            if (!ParseCurrentValues() || !ParseVelocityValues())
            {
                CustomMessageBox.Show("Error saving values. Please check your fields");
                return;
            }

            //Write values to the energyprofile class
            EnergyProfile.Current["NegativeAmplitude"] = dAmplitudeNegI;
            EnergyProfile.Current["NegativeAmpAngle"] = dAngleNegI;
            EnergyProfile.Current["NegativeVariance"] = dVarianceNegI;
            EnergyProfile.Current["PositiveAmplitude"] = dAmplitudePosI;
            EnergyProfile.Current["PositiveAmpAngle"] = dAnglePosI;
            EnergyProfile.Current["PositiveVariance"] = dVariancePosI;
            EnergyProfile.Current["LowerLimit"] = dLowerLimitI;
            EnergyProfile.Current["Hover"] = dHover;
            EnergyProfile.Current["MaxDeviation"] = dDevMax;
            EnergyProfile.Current["MinDeviation"] = dDevMin;

            EnergyProfile.Velocity["Amplitude"] = dAmplitudeV;
            EnergyProfile.Velocity["LowerBound"] = dLowerAmpV;
            EnergyProfile.Velocity["AmpPosition"] = dAngleV;
            EnergyProfile.Velocity["Variance"] = dVarianceV;
            EnergyProfile.Velocity["Gradient"] = dGradientV;
        }

        private void btnLoadCopterSettings_Click(object sender, EventArgs e)
        {
            LoadCopterFileSettings(false);
        }

        private void btnPlotI_Click(object sender, EventArgs e)
        {
            PlotCurrent();
        }

        private void btnPlotV_Click(object sender, EventArgs e)
        {
            PlotVelocity();
        }

        private void PlotVelocity()
        {
            ChartV.Series[0].Points.Clear();

            for (double i = -90; i <= 90; i += 5)
            {
                ChartV.Series[0].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, EnergyProfile.PolyValV(i)));
            }

            //scale graph
            ChartV.ChartAreas[0].AxisY.Minimum = ChartV.Series[0].Points.FindMinByValue().YValues[0] - 0.5;
            ChartV.ChartAreas[0].AxisY.Maximum = ChartV.Series[0].Points.FindMaxByValue().YValues[0] + 0.5;
            ChartV.ChartAreas[0].AxisY.LabelStyle.Format = "{0}";
        }

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

                for (double i = -90; i <= 90; i += 5)
                {
                    
                    ChartI.Series[0].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, EnergyProfile.PolyValI(i, dAmplitudePosI, dAnglePosI, dVariancePosI, dAmplitudeNegI, dAngleNegI, dVarianceNegI, dLowerLimitI)));

                    if (dDevMax != 0) { ChartI.Series[1].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, EnergyProfile.PolyValI(i, dAmplitudePosI, dAnglePosI, dVariancePosI, dAmplitudeNegI, dAngleNegI, dVarianceNegI, dLowerLimitI, dDevMax))); }
                    if (dDevMin != 0) { ChartI.Series[2].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, EnergyProfile.PolyValI(i, dAmplitudePosI, dAnglePosI, dVariancePosI, dAmplitudeNegI, dAngleNegI, dVarianceNegI, dLowerLimitI, -dDevMin))); }
                }

                //scale graph
                ChartI.ChartAreas[0].AxisY.Minimum = ChartI.Series[0].Points.FindMinByValue().YValues[0] - 0.5;
                ChartI.ChartAreas[0].AxisY.Maximum = ChartI.Series[0].Points.FindMaxByValue().YValues[0] + 0.5;

                if (dDevMin != 0) { ChartI.ChartAreas[0].AxisY.Minimum = ChartI.Series[2].Points.FindMinByValue().YValues[0] - 0.5; }
                if (dDevMax != 0) { ChartI.ChartAreas[0].AxisY.Maximum = ChartI.Series[1].Points.FindMaxByValue().YValues[0] + 0.5; }
                ChartI.ChartAreas[0].AxisY.LabelStyle.Format = "{0}";
            }
            catch (Exception e) { CustomMessageBox.Show(e.Message); }
        }

        private void TextBoxes_TextChanged(object sender, EventArgs e)
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
                if (ParseVelocityValues()) { PlotVelocity(); }
            }
        }
        /*
        private bool CurrentFilled()
        {
            //Currentboxes.Add(tb)
            bool tbCurrentFilled = true;

            List<TextBox> Currentboxes = new List<TextBox>();
            Currentboxes.Add(tbAmpIPos);
            Currentboxes.Add(tbAngIPos);
            Currentboxes.Add(tbVarIPos);
            Currentboxes.Add(tbAmpINeg);
            Currentboxes.Add(tbAngINeg);
            Currentboxes.Add(tbVarINeg);
            Currentboxes.Add(tbLimitI);
            
            foreach (TextBox txt in Currentboxes)  //Plot automatically, if all required textboxes are filled
            {
                if (txt.Text == string.Empty)
                {
                    tbCurrentFilled = false;
                    break;
                }
            }

            return tbCurrentFilled;
        }

        private bool VelocityFilled()
        {
            bool tbVelocityFilled = true;

            List<TextBox> Velocityboxes = new List<TextBox>();

            Velocityboxes.Add(tbAmpV);
            Velocityboxes.Add(tbAngV);
            Velocityboxes.Add(tbVarV);
            Velocityboxes.Add(tbGradientV);
            Velocityboxes.Add(tbLowerAmp);
            
            foreach (TextBox txt in Velocityboxes)  //Plot automatically, if all required textboxes are filled
            {
                if (txt.Text == string.Empty)
                {
                    tbVelocityFilled = false;
                    break;
                }
            }

            return tbVelocityFilled;
        }
        */
        private double dAmplitudeV, dLowerAmpV, dAngleV, dVarianceV, dGradientV;
        private double dAmplitudeNegI, dAngleNegI, dVarianceNegI, dAmplitudePosI, dAnglePosI, dVariancePosI, dLowerLimitI, dHover, dDevMax, dDevMin;
    }
}