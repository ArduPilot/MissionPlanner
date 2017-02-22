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
            CB_EnableEnergyProfile.Checked = Convert.ToBoolean(Settings.Instance["EnergyProfileEnabled"]);
            if (CB_EnableEnergyProfile.Checked == true)
            {
                LoadCopterSettings();
            }
        }

        private void LoadCopterSettings()   //Energyprofile for vehicle
        {
            if (MainV2.comPort.BaseStream.IsOpen)
            {
                if (MainV2.comPort.MAV.param.ContainsKey("BRD_SERIAL_NUM"))
                {
                    tbCopterID.Text = MainV2.comPort.GetParam("BRD_SERIAL_NUM").ToString();

                    //Find copter settings
                    if (File.Exists(EnergyProfile.EnergyProfilePath + tbCopterID.Text + ".xml"))
                    {
                        using (System.Xml.XmlReader xr = System.Xml.XmlReader.Create(EnergyProfile.EnergyProfilePath + tbCopterID.Text + ".xml"))
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
            }
        }

        private void CB_EnableEnergyProfile_CheckStateChanged(object sender, EventArgs e)
        {
            if (CB_EnableEnergyProfile.Checked)
            {
                panelCurrentConfiguration.Enabled = true;
                panelVelocityConfiguration.Enabled = true;
            }
            if (!CB_EnableEnergyProfile.Checked)
            {
                panelCurrentConfiguration.Enabled = false;
                panelVelocityConfiguration.Enabled = false;
            }
        }

        private void btnPlotI_Click(object sender, EventArgs e)
        {
            PlotCurrent();
        }

        private void btnSaveCopterSettings_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.GetParam("BRD_SERIAL_NUM") != 0 && tbCopterID.Text != string.Empty)
            {
                MainV2.comPort.setParam("BRD_SERIAL_NUM", double.Parse(tbCopterID.Text));

                //Save parameters to xml file
                if (!(Directory.Exists(EnergyProfile.EnergyProfilePath)))
                {
                    Directory.CreateDirectory(EnergyProfile.EnergyProfilePath);
                }

                using (System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(EnergyProfile.EnergyProfilePath + tbCopterID.Text + ".xml"))
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
            }
        }

        private bool ParseVelocityValues()
        {
            try
            {
                dAmplitudeV = double.Parse(tbAmpV.Text);
                dLowerAmpV = double.Parse(tbLowerAmp.Text);
                dAngleV = double.Parse(tbAngV.Text);
                dVarianceV = double.Parse(tbVarV.Text);
                dCurvatureV = double.Parse(tbCurvatureV.Text);
                dGradientV = double.Parse(tbGradientV.Text);

                EnergyProfile.Velocity["Amplitude"] = dAmplitudeV;
                EnergyProfile.Velocity["LowerAmplitude"] = dLowerAmpV;
                EnergyProfile.Velocity["Angle"] = dAngleV;
                EnergyProfile.Velocity["Variance"] = dVarianceV;
                EnergyProfile.Velocity["Curvature"] = dCurvatureV;
                EnergyProfile.Velocity["Gradient"] = dGradientV;
            }
            catch (FormatException fe)
            {
                CustomMessageBox.Show("Velocity: Invalid format or textbox empty!");
                return false;
            }

            return true;
        }

        private bool ParseCurrentValues()
        {
            try
            {
                dAmplitudeNegI = double.Parse(tbAmpINeg.Text);
                dAngleNegI = double.Parse(tbAngINeg.Text);
                dVarianceNegI = double.Parse(tbVarINeg.Text);
                dAmplitudePosI = double.Parse(tbAmpIPos.Text);
                dAnglePosI = double.Parse(tbAngIPos.Text);
                dVariancePosI = double.Parse(tbVarIPos.Text);
                dLowerLimitI = double.Parse(tbLimitI.Text);
                dHover = double.Parse(tbHoverI.Text);
                dDevMax = double.Parse(tbDeviationMax.Text);
                dDevMin = double.Parse(tbDeviationMin.Text);

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
            }
            catch (FormatException)
            {
                CustomMessageBox.Show("Current: Invalid format or textbox empty!");
                return false;
            }

            return true;
        }

        

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            ParseCurrentValues();
            ParseVelocityValues();
        }

        private double dAmplitudeNegI, dAngleNegI, dVarianceNegI, dAmplitudePosI, dAnglePosI, dVariancePosI, dLowerLimitI, dHover, dDevMax, dDevMin;
        private double dAmplitudeV, dLowerAmpV, dAngleV, dVarianceV, dCurvatureV, dGradientV;

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
                ChartI.Series[1].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, EnergyProfile.PolyValI(i, double.Parse(tbDeviationMax.Text))));
                ChartI.Series[2].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, EnergyProfile.PolyValI(i, -double.Parse(tbDeviationMin.Text))));
            }

            ChartI.ChartAreas[0].AxisY.Minimum = ChartI.Series[2].Points.FindMinByValue().YValues[0] - 0.5;
            ChartI.ChartAreas[0].AxisY.Maximum = ChartI.Series[1].Points.FindMaxByValue().YValues[0] + 0.5;
            ChartI.ChartAreas[0].AxisY.LabelStyle.Format = "{0}";

            //(a1 - t) ℯ^((-0.5(x - b1)²) / (2c1²)) + (a2 - t) ℯ^((-0.5(x - b2)²) / (2c2²)) + t
        }
    }
}