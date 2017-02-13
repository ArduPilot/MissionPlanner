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
            //dAmplitudeNeg = dAngleNeg = dDeviationNeg = dAmplitudePos = dAnglePos = dDeviationPos = dLowerLimit = 0.0f;

            double.TryParse(tbAmpINeg.Text, out dAmplitudeNegI);
            double.TryParse(tbAngINeg.Text, out dAngleNegI);
            double.TryParse(tbDevINeg.Text, out dDeviationNegI);
            double.TryParse(tbAmpIPos.Text, out dAmplitudePosI);
            double.TryParse(tbAngIPos.Text, out dAnglePosI);
            double.TryParse(tbDevIPos.Text, out dDeviationPosI);
            double.TryParse(tbLimitI.Text, out dLowerLimitI);

            ChartI.Series[0].Points.Clear();
            DGV_IValues.Rows.Clear();

            for (double i = -90; i <= 90; i+=5)
            {
                double Value = PolyValI(i);
                DataGridViewRow dgvr = new DataGridViewRow();

                dgvr.CreateCells(DGV_IValues);
                ChartI.Series[0].Points.Add(new System.Windows.Forms.DataVisualization.Charting.DataPoint(i, Value));

                dgvr.Cells[colAngleI.Index].Value = i.ToString();
                dgvr.Cells[colCurrent.Index].Value = Value.ToString("0.00");

                DGV_IValues.Rows.Add(dgvr);

            }
            ChartI.ChartAreas[0].AxisY.Minimum = ChartI.Series[0].Points.FindMinByValue().YValues[0] - 1;
            ChartI.ChartAreas[0].AxisY.Maximum = ChartI.Series[0].Points.FindMaxByValue().YValues[0] + 1;
            ChartI.ChartAreas[0].AxisY.LabelStyle.Format = "{0}";

            //(a1 - t) ℯ^((-(x - b1)²) / (2c1²)) + (a2 - t) ℯ^((-(x - b2)²) / (2c2²)) + t
        }

        private double dAmplitudeNegI, dAngleNegI, dDeviationNegI, dAmplitudePosI, dAnglePosI, dDeviationPosI, dLowerLimitI;

        private double PolyValI(double _dAngle)
        {
            double gauss = (dAmplitudeNegI - dLowerLimitI) * Math.Exp((-0.5 * Math.Pow((_dAngle - dAngleNegI), 2)) / Math.Pow(dDeviationNegI, 2));
            gauss += (dAmplitudePosI - dLowerLimitI) * Math.Exp((-0.5 * Math.Pow((_dAngle - dAnglePosI), 2)) / Math.Pow(dDeviationPosI, 2));
            gauss += dLowerLimitI;

            return gauss;
        }
    }
}