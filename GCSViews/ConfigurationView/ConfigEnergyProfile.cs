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
            EnergyProfilePath = MissionPlanner.Utilities.Settings.GetUserDataDirectory() + "EnergyProfile" + Path.DirectorySeparatorChar;
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
                    if (File.Exists(EnergyProfilePath + tbCopterID.Text + ".xml"))
                    {
                        using (System.Xml.XmlReader xr = System.Xml.XmlReader.Create(EnergyProfilePath + tbCopterID.Text + ".xml"))
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
        
        private void Btn_SaveChanges_Click(object sender, EventArgs e)
        {

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
                if (!(Directory.Exists(EnergyProfilePath)))
                {
                    Directory.CreateDirectory(EnergyProfilePath);
                }

                using (System.Xml.XmlWriter xw = System.Xml.XmlWriter.Create(EnergyProfilePath + tbCopterID.Text + ".xml"))
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
                dDeviationV = double.Parse(tbVarV.Text);
                dCurvatureV = double.Parse(tbCurvatureV.Text);
                dGradientV = double.Parse(tbGradientV.Text);
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
                dDeviationNegI = double.Parse(tbVarINeg.Text);
                dAmplitudePosI = double.Parse(tbAmpIPos.Text);
                dAnglePosI = double.Parse(tbAngIPos.Text);
                dDeviationPosI = double.Parse(tbVarIPos.Text);
                dLowerLimitI = double.Parse(tbLimitI.Text);
                dHover = double.Parse(tbHoverI.Text);
                dDevMax = double.Parse(tbDeviationMax.Text);
                dDevMin = double.Parse(tbDeviationMin.Text);
            }
            catch (FormatException)
            {
                CustomMessageBox.Show("Current: Invalid format or textbox empty!");
                return false;
            }

            return true;
        }

        private double dAmplitudeV, dLowerAmpV, dAngleV, dDeviationV, dCurvatureV, dGradientV;
        private double dAmplitudeNegI, dAngleNegI, dDeviationNegI, dAmplitudePosI, dAnglePosI, dDeviationPosI, dLowerLimitI, dHover, dDevMax, dDevMin;

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
                ChartV.Series[0].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, PolyValV(i)));
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
                ChartI.Series[0].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, PolyValI(i)));
                ChartI.Series[1].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, PolyValI(i, double.Parse(tbDeviationMax.Text))));
                ChartI.Series[2].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, PolyValI(i, -double.Parse(tbDeviationMin.Text))));
            }

            ChartI.ChartAreas[0].AxisY.Minimum = ChartI.Series[2].Points.FindMinByValue().YValues[0] - 0.5;
            ChartI.ChartAreas[0].AxisY.Maximum = ChartI.Series[1].Points.FindMaxByValue().YValues[0] + 0.5;
            ChartI.ChartAreas[0].AxisY.LabelStyle.Format = "{0}";

            //(a1 - t) ℯ^((-0.5(x - b1)²) / (2c1²)) + (a2 - t) ℯ^((-0.5(x - b2)²) / (2c2²)) + t
        }

        private double PolyValV(double _dAngle)
        {
            //(a1 - a2) ℯ^(-0.5*((x - b1) / c1)²) + a2 ℯ^(-0.5*((x - b2) / c2)²) + t x / 1000
            //Scaling factors for curvature and gradient
            double gauss = (dAmplitudeV - dLowerAmpV) * Math.Exp(-0.5*Math.Pow(((_dAngle - dAngleV) / dDeviationV), 2));
            gauss += dLowerAmpV * Math.Exp(-Math.Pow(0.5*(_dAngle / (dCurvatureV)), 2) + dGradientV*_dAngle / 1000);

            return gauss;
        }

        private double PolyValI(double _dAngle, double _dDeviation = 0)
        {
            double gauss = (dAmplitudeNegI - dLowerLimitI) * Math.Exp((-0.5 * Math.Pow((_dAngle - dAngleNegI), 2)) / Math.Pow(dDeviationNegI, 2));
            gauss += (dAmplitudePosI - dLowerLimitI) * Math.Exp((-0.5 * Math.Pow((_dAngle - dAnglePosI), 2)) / Math.Pow(dDeviationPosI, 2));
            gauss += dLowerLimitI;

            return gauss + _dDeviation;
        }

        private string EnergyProfilePath
        {
            get;
            set;
        }
    }
}