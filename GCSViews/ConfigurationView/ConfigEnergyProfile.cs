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

            if (EnergyProfile.Enabled == true)
            {
                //Write back values to settingspage
                LoadEnergyProfileValues();
            }
        }

        private void LoadEnergyProfileValues()
        {
            //Current
            tbAmpINeg.Text = EnergyProfile.Current["NegAmp"].ToString();
            tbAngINeg.Text = EnergyProfile.Current["NegAmpPosition"].ToString();
            tbVarINeg.Text = EnergyProfile.Current["NegVariance"].ToString();

            tbAmpIPos.Text = EnergyProfile.Current["PosAmp"].ToString();
            tbAngIPos.Text = EnergyProfile.Current["PosAmpPosition"].ToString();
            tbVarINeg.Text = EnergyProfile.Current["PosVariance"].ToString();

            tbDeviationMax.Text = EnergyProfile.Current["MaxDeviation"].ToString();
            tbDeviationMin.Text = EnergyProfile.Current["MinDeviation"].ToString();
            tbLimitI.Text = EnergyProfile.Current["LowerLimit"].ToString();
            tbHoverI.Text = EnergyProfile.Current["CurrentHover"].ToString();

            //velocity
            tbAmpV.Text = EnergyProfile.Velocity["Amplitude"].ToString();
            tbAngV.Text = EnergyProfile.Velocity["AmpPosition"].ToString();
            tbVarV.Text = EnergyProfile.Velocity["Variance"].ToString();
            tbLowerAmp.Text = EnergyProfile.Velocity["LowerBound"].ToString();
            tbCurvatureV.Text = EnergyProfile.Velocity["Curvature"].ToString();
            tbGradientV.Text = EnergyProfile.Velocity["Gradient"].ToString();
        }

        private void LoadCopterFileSettings(bool _bLoadAutomatically = false)   //Energyprofile for copter
        {
            string filepath = string.Empty;

            if (_bLoadAutomatically)
            {
                if (MainV2.comPort.MAV.param.ContainsKey("BRD_SERIAL_NUM"))     //Check if copterID is existent
                {
                    tbCopterID.Text = MainV2.comPort.GetParam("BRD_SERIAL_NUM").ToString();
                    filepath = EnergyProfile.EnergyProfilePath + tbCopterID.Text + ".xml";
                }
                else
                {
                    CustomMessageBox.Show("Parameter BRD_SERIAL_NUM not available!");
                    return;
                }
            }
            else //!_bLoadAutomatically
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

                            tbAmpINeg.Text = xr.GetAttribute("NegAmp");
                            tbAngINeg.Text = xr.GetAttribute("NegAmpPosition");
                            tbVarINeg.Text = xr.GetAttribute("NegVariance");
                            tbAmpIPos.Text = xr.GetAttribute("PosAmp");
                            tbAngIPos.Text = xr.GetAttribute("PosAmpPosition");
                            tbVarIPos.Text = xr.GetAttribute("PosVariance");
                            tbDeviationMax.Text = xr.GetAttribute("MaxDeviation");
                            tbDeviationMin.Text = xr.GetAttribute("MinDeviation");
                            tbLimitI.Text = xr.GetAttribute("LowerLimit");
                            tbHoverI.Text = xr.GetAttribute("CurrentHover");

                            xr.ReadToFollowing("Velocity");
                            tbAmpV.Text = xr.GetAttribute("Amplitude");
                            tbAngV.Text = xr.GetAttribute("AmpPosition");
                            tbVarV.Text = xr.GetAttribute("Variance");
                            tbLowerAmp.Text = xr.GetAttribute("LowerBound");
                            tbCurvatureV.Text = xr.GetAttribute("Curvature");
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
                panelIDHover.Enabled = true;
                panelCurrentConfiguration.Enabled = true;
                panelVelocityConfiguration.Enabled = true;
                EnergyProfile.Enabled = true;

                if (DialogResult.Yes == CustomMessageBox.Show("Try to load coptersettings automatically?", "Load settings", MessageBoxButtons.YesNo))
                {
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
            if (MainV2.comPort.GetParam("BRD_SERIAL_NUM") != 0 && tbCopterID.Text != string.Empty)
            {
                double dSerialNum = 0.0f;

                if (double.TryParse(tbCopterID.Text, NumberStyles.AllowDecimalPoint, CultureInfo.GetCultureInfo("en-US"), out dSerialNum))
                {
                    MainV2.comPort.setParam("BRD_SERIAL_NUM", dSerialNum);
                }
                else
                {
                    CustomMessageBox.Show("CopterID has not been saved: error in copterID");
                }
            }

            //Save parameters to xml file
            if (!(Directory.Exists(EnergyProfile.EnergyProfilePath)))
            {
                Directory.CreateDirectory(EnergyProfile.EnergyProfilePath);
            }

            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = EnergyProfile.EnergyProfilePath;
            ofd.Multiselect = false;
            ofd.Filter = "Energyprofile settings (*.xml)|*.xml";
            ofd.DefaultExt = ".xml";

            if (DialogResult.OK == ofd.ShowDialog())
            {
                sFile = ofd.FileName;
            }
            else
            {
                return;
            }

            //sFile = EnergyProfile.EnergyProfilePath + tbCopterID.Text + ".xml";
            using (System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(sFile))
            {
                xw.WriteStartDocument();
                xw.WriteStartElement("EnergyProfile");

                xw.WriteStartElement("Current");
                xw.WriteAttributeString("NegAmp", tbAmpINeg.Text);
                xw.WriteAttributeString("NegAmpPosition", tbAngINeg.Text);
                xw.WriteAttributeString("NegVariance", tbVarINeg.Text);
                xw.WriteAttributeString("PosAmp", tbAmpIPos.Text);
                xw.WriteAttributeString("PosAmpPosition", tbAngIPos.Text);
                xw.WriteAttributeString("PosVariance", tbVarIPos.Text);
                xw.WriteAttributeString("MaxDeviation", tbDeviationMax.Text);
                xw.WriteAttributeString("MinDeviation", tbDeviationMin.Text);
                xw.WriteAttributeString("LowerLimit", tbLimitI.Text);
                xw.WriteAttributeString("CurrentHover", tbHoverI.Text);
                xw.WriteEndElement();

                xw.WriteStartElement("Velocity");
                xw.WriteAttributeString("Amplitude", tbAmpV.Text);
                xw.WriteAttributeString("AmpPosition", tbAngV.Text);
                xw.WriteAttributeString("Variance", tbVarV.Text);
                xw.WriteAttributeString("LowerBound", tbLowerAmp.Text);
                xw.WriteAttributeString("Curvature", tbCurvatureV.Text);
                xw.WriteAttributeString("Gradient", tbGradientV.Text);
                xw.WriteEndElement();

                xw.WriteEndElement();
                xw.WriteEndDocument();
                xw.Close();
            }

            CustomMessageBox.Show("Saved settings to " + sFile);
        }

        private bool ParseVelocityValues()
        {
            double dAmplitudeV, dLowerAmpV, dAngleV, dVarianceV, dCurvatureV, dGradientV;
            dAmplitudeV = dLowerAmpV = dAngleV = dVarianceV = dCurvatureV = dGradientV = 0.0f;

            if (!double.TryParse(tbAmpV.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dAmplitudeV) ||
                !double.TryParse(tbLowerAmp.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dLowerAmpV)||
                !double.TryParse(tbAngV.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dAngleV) ||
                !double.TryParse(tbVarV.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dVarianceV) ||
                !double.TryParse(tbCurvatureV.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dCurvatureV) ||
                !double.TryParse(tbGradientV.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dGradientV))
            {
                CustomMessageBox.Show("Velocity: Invalid format or textbox empty!");
                return false;
            }

            EnergyProfile.Velocity["Amplitude"] = dAmplitudeV;
            EnergyProfile.Velocity["LowerAmplitude"] = dLowerAmpV;
            EnergyProfile.Velocity["Angle"] = dAngleV;
            EnergyProfile.Velocity["Variance"] = dVarianceV;
            EnergyProfile.Velocity["Curvature"] = dCurvatureV;
            EnergyProfile.Velocity["Gradient"] = dGradientV;

            return true;
        }

        private bool ParseCurrentValues()
    {
            //try parsing the values from textboxes before they are written into the EP-class
            double dAmplitudeNegI, dAngleNegI, dVarianceNegI, dAmplitudePosI, dAnglePosI, dVariancePosI, dLowerLimitI, dHover, dDevMax, dDevMin;
            dAmplitudeNegI = dAngleNegI = dVarianceNegI = dAmplitudePosI = dAnglePosI = dVariancePosI = dLowerLimitI = dHover = dDevMax = dDevMin = 0.0f;

            if (!double.TryParse(tbAmpINeg.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dAmplitudeNegI) ||
                !double.TryParse(tbAngINeg.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dAngleNegI) ||
                !double.TryParse(tbVarINeg.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dVarianceNegI) ||
                !double.TryParse(tbAmpIPos.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dAmplitudePosI) ||
                !double.TryParse(tbAngIPos.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dAnglePosI) ||
                !double.TryParse(tbVarIPos.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dVariancePosI) ||
                !double.TryParse(tbLimitI.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dLowerLimitI) ||
                !double.TryParse(tbHoverI.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dHover) ||
                !double.TryParse(tbDeviationMax.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dDevMax) ||
                !double.TryParse(tbDeviationMin.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out dDevMin)
                )
            {
                CustomMessageBox.Show("Current: Invalid format or textbox empty!");
                return false;
            }

            //Save settings to energyprofile class
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

            return true;
        }

        //Save settings to energyprofile class
        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            ParseCurrentValues();
            ParseVelocityValues();
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
            if (!ParseVelocityValues()) { return; }

            ChartV.Series[0].Points.Clear();

            for (double i = -90; i <= 90; i += 5)
            {
                ChartV.Series[0].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, EnergyProfile.PolyValV(i)));
            }
        }

        private void PlotCurrent()
        {
            if (!ParseCurrentValues()) { return; }

            ChartI.Series[0].Points.Clear();
            ChartI.Series[1].Points.Clear();
            ChartI.Series[2].Points.Clear();

            for (double i = -90; i <= 90; i += 5)
            {
                ChartI.Series[0].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, EnergyProfile.PolyValI(i)));
                ChartI.Series[1].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, EnergyProfile.PolyValI(i, double.Parse(tbDeviationMax.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US")))));
                ChartI.Series[2].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, EnergyProfile.PolyValI(i, -double.Parse(tbDeviationMin.Text, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US")))));
            }

            ChartI.ChartAreas[0].AxisY.Minimum = ChartI.Series[2].Points.FindMinByValue().YValues[0] - 0.5;
            ChartI.ChartAreas[0].AxisY.Maximum = ChartI.Series[1].Points.FindMaxByValue().YValues[0] + 0.5;
            ChartI.ChartAreas[0].AxisY.LabelStyle.Format = "{0}";

            //(a1 - t) ℯ^((-0.5(x - b1)²) / (2c1²)) + (a2 - t) ℯ^((-0.5(x - b2)²) / (2c2²)) + t
        }
    }
}