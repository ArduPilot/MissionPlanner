using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MissionPlanner.Controls;
using Xamarin.Forms;
using Device = MissionPlanner.Utilities.Device;
using ListView = System.Windows.Forms.ListView;

namespace MissionPlanner.GCSViews.ConfigurationView
{
    public partial class ConfigHWCompass2 : MyUserControl, IActivate, IDeactivate
    {
        private List<CompassInfo> list;


        public ConfigHWCompass2()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            list = MainV2.comPort.MAV.param.Where(a => a.Name.StartsWith("COMPASS_DEV_ID"))
                .Select(a => new CompassInfo(a.Name, (uint) a.Value)).ToList();

            var bs = new BindingSource();
            bs.DataSource = list;
            myDataGridView1.DataSource = bs;
        }



        public void Deactivate()
        {

        }

        private async void myDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Up.Index)
            {
                var item = list[e.RowIndex];
                list.Remove(item);
                list.Insert(e.RowIndex - 1, item);

                await UpdateFirst3();
            }

            if (e.ColumnIndex == Down.Index)
            {
                var item = list[e.RowIndex];
                list.Remove(item);
                list.Insert(e.RowIndex + 1, item);

                await UpdateFirst3();
            }
        }

        private async Task UpdateFirst3()
        {
            if (myDataGridView1.Rows.Count >= 1)
                await MainV2.comPort.setParamAsync((byte) MainV2.comPort.sysidcurrent,
                    (byte) MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO1_ID",
                    int.Parse(myDataGridView1.Rows[0].Cells[devIDDataGridViewTextBoxColumn.Index].Value.ToString()));

            if (myDataGridView1.Rows.Count >= 2)
                await MainV2.comPort.setParamAsync((byte) MainV2.comPort.sysidcurrent,
                    (byte) MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO2_ID",
                    int.Parse(myDataGridView1.Rows[1].Cells[devIDDataGridViewTextBoxColumn.Index].Value.ToString()));

            if (myDataGridView1.Rows.Count >= 3)
                await MainV2.comPort.setParamAsync((byte) MainV2.comPort.sysidcurrent,
                    (byte) MainV2.comPort.compidcurrent,
                    "COMPASS_PRIO3_ID",
                    int.Parse(myDataGridView1.Rows[2].Cells[devIDDataGridViewTextBoxColumn.Index].Value.ToString()));

            myDataGridView1.Invalidate();
        }
    }

    public class CompassInfo
    {
        private readonly string _paramName;
        private Device.DeviceStructure _devid;

        public CompassInfo(string ParamName, uint id)
        {
            _paramName = ParamName;
            _devid = new Device.DeviceStructure(id);
        }

        public int DevID => (int) _devid.devid;

        public string BusType => _devid.bus_type.ToString();
        public int Bus => (int) _devid.bus;
        public int Address => (int) _devid.address;
        public string DevType => _devid.devtype.ToString();

    }
}
