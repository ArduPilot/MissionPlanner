using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner.Controls;

namespace MissionPlanner.Utilities.DroneApi.UI
{
    public partial class VehicleSelection : Form
    {
        public string uuid = "";

        public VehicleSelection(Dictionary<string, string> vehDictionary)
        {
            InitializeComponent();

            CMB_vehicle.DataSource = vehDictionary.ToArray();
        }

        private void CMB_vehicle_Format(object sender, ListControlConvertEventArgs e)
        {
            e.Value = ((KeyValuePair<string, string>) e.Value).Key;
        }

        private void BUT_Select_Click(object sender, EventArgs e)
        {
            uuid = ((KeyValuePair<string, string>) CMB_vehicle.SelectedValue).Value.ToString();

            this.Close();
        }

        private void BUT_new_Click(object sender, EventArgs e)
        {
            //string newname = "new";

            //InputBox.Show("Vehicle", "New vehicle name", ref newname);

            uuid = Guid.NewGuid().ToString();

            //droneshare.doVehicleCreate(newname, uuid);

            this.Close();
        }
    }
}