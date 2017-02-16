using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
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
        }

        //Key: angle, Value: Current in A per angle
        private SortedDictionary<double, double> m_IValues = new SortedDictionary<double, double>();

        //Key: angle, Value, Velocity in m/s per angle
        private SortedDictionary<double, double> m_VValues = new SortedDictionary<double, double>();

        public void Activate()
        {
            CB_EnableEnergyProfile.Checked = Convert.ToBoolean(Settings.Instance["EnergyProfileEnabled"]);
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
            double.TryParse(tbAmpINeg.Text, out dAmplitudeNegI);
            double.TryParse(tbAngINeg.Text, out dAngleNegI);
            double.TryParse(tbDevINeg.Text, out dDeviationNegI);
            double.TryParse(tbAmpIPos.Text, out dAmplitudePosI);
            double.TryParse(tbAngIPos.Text, out dAnglePosI);
            double.TryParse(tbDevIPos.Text, out dDeviationPosI);
            double.TryParse(tbLimitI.Text, out dLowerLimitI);

            ChartI.Series[0].Points.Clear();

            for (double i = -90; i <= 90; i+=5)
            {
                ChartI.Series[0].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, PolyValI(i)));
            }
            ChartI.ChartAreas[0].AxisY.Minimum = ChartI.Series[0].Points.FindMinByValue().YValues[0] - 1;
            ChartI.ChartAreas[0].AxisY.Maximum = ChartI.Series[0].Points.FindMaxByValue().YValues[0] + 1;
            ChartI.ChartAreas[0].AxisY.LabelStyle.Format = "{0}";

            //(a1 - t) ℯ^((-0.5(x - b1)²) / (2c1²)) + (a2 - t) ℯ^((-0.5(x - b2)²) / (2c2²)) + t
        }

        private double dAmplitudeNegI, dAngleNegI, dDeviationNegI, dAmplitudePosI, dAnglePosI, dDeviationPosI, dLowerLimitI;

        private void btnSaveCopterSettings_Click(object sender, EventArgs e)
        {
        }

        private double dAmplitudeV, dLowerAmpV, dAngleV, dDeviationV, dCurvatureV, dGradientV;

        private void btnPlotV_Click(object sender, EventArgs e)
        {
            double.TryParse(tbAmpV.Text, out dAmplitudeV);
            double.TryParse(tbLowerAmp.Text, out dLowerAmpV);
            double.TryParse(tbAngV.Text, out dAngleV);
            double.TryParse(tbDevV.Text, out dDeviationV);
            double.TryParse(tbCurvatureV.Text, out dCurvatureV);
            double.TryParse(tbGradientV.Text, out dGradientV);

            ChartV.Series[0].Points.Clear();

            for (double i = -90; i <= 90; i+=5)
            {
                ChartV.Series[0].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, PolyValV(i)));
            }
        }


        private double PolyValV(double _dAngle)
        {
            //(a1 - a2) ℯ^(-0.5*((x - b1) / c1)²) + a2 ℯ^(-0.5*((x - b2) / c2)²) + t x / 1000
            //Scaling factors for curvature and gradient
            double gauss = (dAmplitudeV - dLowerAmpV) * Math.Exp(-0.5*Math.Pow(((_dAngle - dAngleV) / dDeviationV), 2));
            gauss += dLowerAmpV * Math.Exp(-Math.Pow(0.5*(_dAngle / (dCurvatureV)), 2) + dGradientV*_dAngle / 1000);

            return gauss;
        }

        private double PolyValI(double _dAngle)
        {
            double gauss = (dAmplitudeNegI - dLowerLimitI) * Math.Exp((-0.5 * Math.Pow((_dAngle - dAngleNegI), 2)) / Math.Pow(dDeviationNegI, 2));
            gauss += (dAmplitudePosI - dLowerLimitI) * Math.Exp((-0.5 * Math.Pow((_dAngle - dAnglePosI), 2)) / Math.Pow(dDeviationPosI, 2));
            gauss += dLowerLimitI;

            return gauss;
        }
    }
}