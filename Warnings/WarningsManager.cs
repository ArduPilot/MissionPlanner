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
                    var wrnctl = addwarningcontrol(5, y, item);

                    y = wrnctl.Bottom;
                }
            }

            Utilities.ThemeManager.ApplyThemeTo(this);
        }

        WarningControl addwarningcontrol(int x, int y, CustomWarning item, bool hideforchild = false)
        {
            WarningControl wrnctl = new WarningControl(item);

            wrnctl.ReloadList += wrnctl_ChildAdd;

            wrnctl.Location = new Point(x, y);

            if (hideforchild)
                wrnctl.TXT_warningtext.Visible = false;

            panel1.Controls.Add(wrnctl);

            y = wrnctl.Bottom;

            if (item.Child != null)
            {
                wrnctl = addwarningcontrol(x += 5, y, item.Child, true);
            }

            return wrnctl;
        }

        void wrnctl_ChildAdd(object sender, EventArgs e)
        {
            reload();
        }

        private void BUT_Add_Click(object sender, EventArgs e)
        {
            var newcw = new CustomWarning();

            CustomWarning.defaultsrc = MainV2.comPort.MAV.cs;
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