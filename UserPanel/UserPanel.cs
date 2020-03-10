using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.UserPanel
{
    public partial class UserPanel : UserControl
    {
        public UserPanel()
        {
            InitializeComponent();
        }

        void test()
        {
            MainV2.comPort.doCommandAsync(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, 0, 0, 0, 0, 0, 0, 0, 0);
            MainV2.comPort.doARMAsync(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, true);
            MainV2.comPort.doCommandIntAsync(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, 0, 0, 0, 0, 0, 0, 0, 0);
            MainV2.comPort.getParamListAsync(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid);
            MainV2.comPort.getHeartBeatAsync();
            MainV2.comPort.getVersionAsync(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid);
            MainV2.comPort.GetParamAsync(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid);
        }
    }
}
