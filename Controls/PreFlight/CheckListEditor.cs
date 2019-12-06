using System;
using System.Drawing;
using System.Windows.Forms;

namespace MissionPlanner.Controls.PreFlight
{
    public partial class CheckListEditor : Form
    {
        CheckListControl _parent;

        public CheckListEditor(CheckListControl parent)
        {
            _parent = parent;

            InitializeComponent();

            reload();
        }

        public void reload()
        {
            panel1.Controls.Clear();

            int y = 0;

            lock (_parent.CheckListItems)
            {
                foreach (var item in _parent.CheckListItems)
                {
                    var wrnctl = addwarningcontrol(5, y, item);

                    y = wrnctl.Bottom;
                }
            }

            Utilities.ThemeManager.ApplyThemeTo(this);
        }

        CheckListInput addwarningcontrol(int x, int y, CheckListItem item, bool hideforchild = false)
        {
            CheckListInput wrnctl = new CheckListInput(_parent, item);

            wrnctl.ReloadList += wrnctl_ChildAdd;

            wrnctl.Location = new Point(x, y);

            if (hideforchild)
            {
                wrnctl.TXT_text.Visible = false;
                wrnctl.TXT_desc.Visible = false;
                wrnctl.CMB_colour1.Visible = false;
                wrnctl.CMB_colour2.Visible = false;
            }

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
            var newcw = new CheckListItem();

            CheckListItem.defaultsrc = MainV2.comPort.MAV.cs;
            newcw.SetField(newcw.GetOptions()[0]);

            lock (_parent.CheckListItems)
            {
                _parent.CheckListItems.Add(newcw);
            }

            reload();
        }

        private void BUT_save_Click(object sender, EventArgs e)
        {
            _parent.SaveConfig();
        }
    }
}