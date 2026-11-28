using MissionPlanner.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigGPSOrder : MyUserControl, IActivate
    {
        public ConfigGPSOrder()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            if (!MainV2.comPort.MAV.param.ContainsKey("GPS1_CAN_OVRIDE"))
            {
                this.Enabled = false;
                return;
            }

            // get the detected id's
            var id1 = MainV2.comPort.MAV.param[new[] { "GPS1_CAN_NODEID", "GPS_CAN_NODEID1" }]?.Value ?? 0;
            var id2 = MainV2.comPort.MAV.param[new[] { "GPS2_CAN_NODEID", "GPS_CAN_NODEID2" }]?.Value ?? 0;

            // get the override id's
            var id1ovr = MainV2.comPort.MAV.param["GPS1_CAN_OVRIDE"]?.Value ?? 0;
            var id2ovr = MainV2.comPort.MAV.param["GPS2_CAN_OVRIDE"]?.Value ?? 0;

            var list = new List<GPSCAN>();

            if (id1ovr != 0)
                list.Add(new GPSCAN() {Order = 1, NodeID = (int) id1ovr, Name = "GPS Override 1"});
            if (id2ovr != 0)
                list.Add(new GPSCAN() {Order = 2, NodeID = (int) id2ovr, Name = "GPS Override 2"});
            if (id1 != 0 && id1 != id1ovr && id1 != id2ovr)
                list.Add(new GPSCAN() {Order = 98, NodeID = (int) id1, Name = "GPS Detect 1"});
            if (id2 != 0 && id2 != id1ovr && id2 != id2ovr)
                list.Add(new GPSCAN() {Order = 99, NodeID = (int) id2, Name = "GPS Detect 2"});

            var bs = new BindingSource();
            bs.DataSource = list;
            myDataGridView1.DataSource = bs;
        }

        public class GPSCAN
        {
            public int Order { get; set; }

            public string Name { get; set; } = "GPS";

            public int NodeID { get; set; }
        }

        private void myDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == GPS1.Index)
                {
                    MainV2.comPort.setParam(MainV2.comPort.sysidcurrent, (byte) MainV2.comPort.compidcurrent,
                        "GPS1_CAN_OVRIDE",
                        int.Parse(myDataGridView1[nodeIDDataGridViewTextBoxColumn.Index, e.RowIndex].Value.ToString()));
                    Activate();
                }

                if (e.ColumnIndex == GPS2.Index)
                {
                    MainV2.comPort.setParam(MainV2.comPort.sysidcurrent, (byte) MainV2.comPort.compidcurrent,
                        "GPS2_CAN_OVRIDE",
                        int.Parse(myDataGridView1[nodeIDDataGridViewTextBoxColumn.Index, e.RowIndex].Value.ToString()));
                    Activate();
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(Strings.ERROR, "Failed to set param " + ex.ToString());
            }
        }
    }
}
