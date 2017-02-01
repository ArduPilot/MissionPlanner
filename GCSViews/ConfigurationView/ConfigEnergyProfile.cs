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
            FillValues();
        }

        //Key: angle, Value: Current in A per angle
        private SortedDictionary<double, double> m_IValues = new SortedDictionary<double, double>();

        //Key: angle, Value, Velocity in m/s per angle
        private SortedDictionary<double, double> m_VValues = new SortedDictionary<double, double>();

        private void FillValues()
        {
            //Integrate Polynom into energyprofile (as long as no copter data (logfiles) is being processed ==> second pullrequest)
            //For now: Coptervalues will be approximated through a polynom that has been determinated 
            //through samplecopter data (processed with regression and least squares).

            //The user can include his own measured values via energyprofile configuration to fit the energyconsumption the the specific copter.
            //The fitting value for an angle is found by linear interpolation of values ("linear" curve through all values)

            for (double angle = -90; angle <= 90; angle = angle + 5)
            {
                //Samplepolynom (measured one will follow very soon): 0.2*x^2 + 0.3*x + 15
                //measured I-Polynom is of grade 4
                m_IValues[angle] = PolyValI(angle);
                DGV_IValues.Rows.Add(angle.ToString(), m_IValues[angle].ToString());
            }

            for (double angle = -90; angle <= 90; angle = angle + 5)
            {
                //Samplepolynom (measured one will follow very soon): 0.2*x^2 + 0.3*x + 15
                //measured V-Polynom is of grade 4
                m_VValues[angle] = PolyValV(angle);
                DGV_VValues.Rows.Add(angle.ToString(), m_VValues[angle]);
            }

            //ToDo: Save user defined values

            //necessary?
            DGV_IValues.Update();
            DGV_VValues.Update();
        }

        //Returns value of set polynom for I
        private double PolyValI(double _angle)
        {
            //Coefficients for I: -0.000000124085386  -0.000003451062258   0.001242329399311   0.048771354900547  16.876403038851002
            double polynomialValue = 0.0f;
            polynomialValue = -0.000000124085386 * _angle * _angle * _angle * _angle;
            polynomialValue += -0.000003451062258*_angle*_angle*_angle;
            polynomialValue += 0.001242329399311 * _angle * _angle;
            polynomialValue += 0.048771354900547 * _angle;
            polynomialValue += 16.876403038851002;

            return polynomialValue;
        }

        //Returns value of set polynom for V
        private double PolyValV(double _angle)
        {
            return 0.12 * _angle * _angle + 0.0573 * _angle + 5;
        }

        public void Activate()
        {
            CB_EnableEnergyProfile.Checked = Convert.ToBoolean(Settings.Instance["EnergyProfileEnabled"]);
        }
        
        private void Btn_SaveChanges_Click(object sender, EventArgs e)
        {
            string I_ValueData = string.Empty;
            string V_ValueData = string.Empty;

            var asdf = m_IValues.Keys.ToArray();

            for (int i = 0; i < m_IValues.Count;i++)
            {
                double key = m_IValues.Keys.ToArray()[i];

                if (I_ValueData != string.Empty) { I_ValueData += "#"; }    //Split entries (angle,value) by #
                I_ValueData += key + "|" + m_IValues[key].ToString();
            }

            for (int i = 0; i < m_VValues.Count; i++)
            {
                double key = m_VValues.Keys.ToArray()[i];
                if (V_ValueData != string.Empty) { V_ValueData += "#"; }    //Split entries (angle,value) by #
                V_ValueData += key + "|" + m_VValues[key].ToString();
            }

            //Todo: WriteUserData into string

            Settings.Instance["EP_Current"] = I_ValueData;
            Settings.Instance["EP_Velocity"] = V_ValueData;
        }

        private void CB_EnableEnergyProfile_CheckStateChanged(object sender, EventArgs e)
        {
            if (CB_EnableEnergyProfile.Checked == false)
            {
                P_EnergyProfileConfiguration.Enabled = false;
                Settings.Instance["EnergyProfileEnabled"] = "False";

            }
            else
            {
                Settings.Instance["EnergyProfileEnabled"] = "True";
                P_EnergyProfileConfiguration.Enabled = true;
            }

        }

        private void P_EnergyProfileConfiguration_Paint(object sender, PaintEventArgs e)
        {

        }

        private void BtnAddIValue_Click(object sender, EventArgs e)
        {
            double val = 0;
            //User input validation:
            if (TB_AngleI.Text == string.Empty || TB_Current.Text == string.Empty)
            {
                MessageBox.Show("Field is empty!", "Missing field");
            }
            else if (!double.TryParse(TB_AngleI.Text, out val) || !double.TryParse(TB_Current.Text, out val)
                || TB_AngleI.Text.Contains(",") || TB_Current.Text.Contains(","))
            {
                MessageBox.Show("Invalid value in field!");
            }
            else if (double.Parse(TB_AngleI.Text) <= -90 || double.Parse(TB_AngleI.Text) >= 90)
            {
                MessageBox.Show("Angle is out of Range.", "Out of range");
            }
            else
            {
                m_IValues[double.Parse(TB_AngleI.Text)] = double.Parse(TB_Current.Text);
                DGV_IValues.Rows.Add(double.Parse(TB_AngleI.Text), double.Parse(TB_Current.Text));
            }
        }

        private void BtnAddVValue_Click(object sender, EventArgs e)
        {
            double val = 0;

            //User input validation:
            if (TB_AngleV.Text == string.Empty || TB_Velocity.Text == string.Empty)
            {
                MessageBox.Show("Field is empty!", "Missing field");
            }
            else if (!double.TryParse(TB_AngleV.Text, out val) || !double.TryParse(TB_Velocity.Text, out val)
                || TB_AngleV.Text.Contains(",") || TB_Velocity.Text.Contains(","))
            {
                MessageBox.Show("Invalid value in field!");
            }
            else if (double.Parse(TB_AngleV.Text, new CultureInfo("en-US")) <= -90 || double.Parse(TB_AngleV.Text, new CultureInfo("en-US")) >= 90)
            {
                MessageBox.Show("Angle is out of Range.", "Out of range");
            }
            else
            {
                m_VValues[double.Parse(TB_AngleV.Text,new CultureInfo("en-US"))] = double.Parse(TB_Velocity.Text, new CultureInfo("en-US"));

                /*bool bValueInList = false;
                foreach(DataGridViewRow row in DGV_VValues.Rows)
                {
                    if(row.Cells[colAngleV.Index].Value.ToString().Contains(TB_AngleV.Text.Replace('.', ',')))
                    {
                        row.Cells[colVelocity.Index].Value = TB_Velocity.Text;
                        bValueInList = true;
                    }
                }
                if (bValueInList == false)
                {*/
                    DGV_VValues.Rows.Add(double.Parse(TB_AngleV.Text, new CultureInfo("en-US")), double.Parse(TB_Velocity.Text, new CultureInfo("en-US")));
                //}
            }
        }
    }
}