using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using MissionPlanner.Utilities;

namespace MissionPlanner.Controls.PreFlight
{
    public partial class CheckListControl : UserControl
    {
        public List<CheckListItem> CheckListItems = new List<CheckListItem>();

        public string configfile = Settings.GetUserDataDirectory() + "checklist.xml";

        public string configfiledefault = Settings.GetRunningDirectory() + "checklistDefault.xml";

        int rowcount = 0;

        internal struct internaldata
        {
            internal Label desc;
            internal Label text;
            internal CheckBox tickbox;
            internal CheckListItem CLItem;
        }


        public CheckListControl()
        {
            InitializeComponent();

            try
            {
                MissionPlanner.Controls.PreFlight.CheckListItem.defaultsrc = MainV2.comPort.MAV.cs;

                LoadConfig();
            }
            catch
            {
                Console.WriteLine("Failed to read CheckList config file " + configfile);
            }

            timer1.Start();
        }

        public void Draw()
        {
            lock (this.CheckListItems)
            {
                if (rowcount == this.CheckListItems.Count)
                    return;

                panel1.SuspendLayout();
                panel1.Controls.Clear();

                int y = 0;

                rowcount = 0;

                foreach (var item in this.CheckListItems)
                {
                    var wrnctl = addwarningcontrol(5, y, item);

                    rowcount++;

                    y = wrnctl.Bottom;
                }
            }
            panel1.ResumeLayout(true);
        }

        void UpdateDisplay()
        {
            foreach (Control itemp in panel1.Controls)
            {
                foreach (Control item in itemp.Controls)
                {
                    if (item.Tag == null)
                        continue;

                    internaldata data = (internaldata)item.Tag;

                    if (item.Name.StartsWith("utext"))
                    {
                        item.Text = data.CLItem.DisplayText();
                        data.desc.Text = data.CLItem.Description;
                    }
                    if (item.Name.StartsWith("utickbox"))
                    {
                        var tickbox = item as CheckBox;
                        if (data.CLItem.ConditionType != CheckListItem.Conditional.NONE)
                            tickbox.Checked = data.CLItem.checkCond(data.CLItem);

                        if (tickbox.Checked)
                        {
                            data.text.ForeColor = data.CLItem._TrueColor;
                            data.desc.ForeColor = data.CLItem._TrueColor;
                        }
                        else
                        {
                            data.text.ForeColor = data.CLItem._FalseColor;
                            data.desc.ForeColor = data.CLItem._FalseColor;
                        }
                    }
                }
            }
        }

        Control addwarningcontrol(int x, int y, CheckListItem item, bool hideforchild = false)
        {
            var desctext = item.Description;
            var texttext = item.DisplayText();

            var height = TextRenderer.MeasureText(desctext, this.Font).Height;

            GroupBox gb = new GroupBox() { Text = "", Location = new Point(x,y), Size = new Size(330, 17 + height), Name = "gb" + y };

            Label desc = new Label() { Text = desctext, Location = new Point(5, 9), Size = new Size(150, height), Name = "udesc" + y };
            Label text = new Label() { Text = texttext, Location = new Point(desc.Right, 9), Size = new Size(150, height), Name = "utext" + y };
            CheckBox tickbox = new CheckBox() { Checked = item.checkCond(item), Location = new Point(text.Right, 7), Size = new Size(21, 21), Name = "utickbox" + y };

            desc.Tag = text.Tag = tickbox.Tag = new internaldata { CLItem = item, desc = desc, text = text, tickbox = tickbox };

            gb.Controls.Add(desc);
            gb.Controls.Add(text);
            gb.Controls.Add(tickbox);

            panel1.Controls.Add(gb);

            y = gb.Bottom;

            if (item.Child != null)
            {
                //return addwarningcontrol(x += 5, y, item.Child, true);
            }

            return gb;
        }

        public void LoadConfig()
        {
            string loadfile = configfile;

            if (!File.Exists(configfile))
            {
                if (!File.Exists(configfiledefault))
                {
                    return;
                }
                else
                {
                    loadfile = configfiledefault;
                }
            }

            System.Xml.Serialization.XmlSerializer reader =
                new System.Xml.Serialization.XmlSerializer(typeof(List<CheckListItem>),
                    new Type[] { typeof(CheckListItem) });

            using (StreamReader sr = new StreamReader(loadfile))
            {
                CheckListItems = (List<CheckListItem>)reader.Deserialize(sr);
            }
        }

        public void SaveConfig()
        {
            // save config
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(List<CheckListItem>),
                    new Type[] { typeof(CheckListItem), typeof(Color) });

            using (StreamWriter sw = new StreamWriter(configfile))
            {
                lock (CheckListItems)
                {
                    writer.Serialize(sw, CheckListItems);
                }
            }
        }

        private void BUT_edit_Click(object sender, EventArgs e)
        {
            CheckListEditor form = new CheckListEditor(this);
            form.Show();
            lock (this.CheckListItems)
                rowcount = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Draw();
            UpdateDisplay();
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible)
            {
                timer1.Enabled = true;
            }
            else
            {
                timer1.Enabled = false;
            }
        }
    }
}
