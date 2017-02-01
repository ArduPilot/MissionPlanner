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

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigEnergyProfile : UserControl, IActivate
    {
        public ConfigEnergyProfile()
        {
            InitializeComponent();
            FillValues();
            DGV_IValues.DataSource = m_DTIValues;
        }

        private void FillValues()
        {
            DataColumn dc = new DataColumn("Angle");
            dc.Caption = "Angle";
            m_DTIValues.Columns.Add(dc);

            dc = new DataColumn("Current");
            dc.Caption = "Current in A";
            m_DTIValues.Columns.Add(dc);

            dc = new DataColumn("Angle");
            dc.Caption = "Angle";
            m_DTVValues.Columns.Add(dc);

            dc = new DataColumn("Velocity");
            dc.Caption = "Velocity in m/s";
            m_DTVValues.Columns.Add(dc);

            //Integrate Polynom into energyprofile (as long as no copter data (logfiles) is being processed ==> second pullrequest)
            //For now: Coptervalues will be approximated through a polynom that has been determinated 
            //through samplecopter data (processed with regression and least squares).

            //The user can include his own measured values via energyprofile configuration to fit the energyconsumption the the specific copter.
            //The fitting value for an angle is found by linear interpolation of values ("linear" curve through all values)

            //Key: angle, Value: Current in A per angle
            SortedDictionary<double, double> IValues = new SortedDictionary<double, double>();

            //Key: angle, Value, Velocity in m/s per angle
            SortedDictionary<double, double> VValues = new SortedDictionary<double, double>();

            for (double angles = -90; angles <= 90; angles = angles + 5)
            {
                //Samplepolynom (measured one will follow very soon): 0.2*x^2 + 0.3*x + 15
                //measured I-Polynom is of grade 4
                IValues[angles] = 0.2 * angles * angles + 0.3 * angles + 15;
            }

            for (double angles = -90; angles <= 90; angles = angles + 5)
            {
                //Samplepolynom (measured one will follow very soon): 0.2*x^2 + 0.3*x + 15
                //measured V-Polynom is of grade 4
                VValues[angles] = 0.12 * angles * angles + 0.0573 * angles + 5;
            }

            for (int i = 0; i < IValues.Count; i++)
            {
                DataRow dr = m_DTIValues.NewRow();
                dr["Angle"] = 3.ToString();
                dr["Current"] = 5.ToString();
            }
            
            DGV_IValues.Update();
        }

        public void Activate()
        {
            CB_EnableEnergyProfile.Checked = Convert.ToBoolean(Settings.Instance["EnergyProfileEnabled"]);
        }

        private DataTable m_DTIValues = new DataTable();
        private DataTable m_DTVValues = new DataTable();


        private void Btn_SaveChanges_Click(object sender, EventArgs e)
        {
            
        }

        private void CB_EnableEnergyProfile_CheckStateChanged(object sender, EventArgs e)
        {
            if (CB_EnableEnergyProfile.Checked == false)
            {
                //GB_EnergyProfileConfiguration.Enabled = false;
                P_EnergyProfileConfiguration.Enabled = false;
                Settings.Instance["EnergyProfileEnabled"] = "False";

            }
            else
            {
                Settings.Instance["EnergyProfileEnabled"] = "True";
                //GB_EnergyProfileConfiguration.Enabled = true;
                P_EnergyProfileConfiguration.Enabled = true;
            }

        }
    }
}
