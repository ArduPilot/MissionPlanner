using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace MissionPlanner.Warnings
{
    public partial class WarningsManager : Form
    {

        public WarningsManager()
        {
            InitializeComponent();

            reload();
        }

        public void reload()
        {
            panel1.Controls.Clear();

            int y = 0;

            lock (WarningEngine.warnings)
            {
                foreach (var item in WarningEngine.warnings)
                {
                    WarningControl wrnctl = new WarningControl(item);

                    wrnctl.Location = new Point(5, y);

                    y = wrnctl.Bottom;

                    panel1.Controls.Add(wrnctl);
                }
            }
        }


        private void BUT_Add_Click(object sender, EventArgs e)
        {
            var newcw = new CustomWarning();

            newcw.SetSource(MainV2.comPort.MAV.cs);
            newcw.SetField(newcw.GetOptions()[0]);

            lock (WarningEngine.warnings)
            {
                WarningEngine.warnings.Add(newcw);
            }

            reload();
        }

        private void BUT_save_Click(object sender, EventArgs e)
        {
            WarningEngine.SaveConfig();
        }
    }
}
