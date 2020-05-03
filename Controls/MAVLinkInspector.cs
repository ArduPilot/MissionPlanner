using MissionPlanner.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ZedGraph;

namespace MissionPlanner.Controls
{
    public class MAVLinkInspector : Form
    {
        private GroupBox groupBox1;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private MyTreeView treeView1;
        private Timer timer1;
        private IContainer components;
        PacketInspector<MAVLink.MAVLinkMessage> mavi = new PacketInspector<MAVLink.MAVLinkMessage>();
        private MyButton but_graphit;
        private CheckBox chk_gcstraffic;
        private MAVLinkInterface mav;

        public MAVLinkInspector(MAVLinkInterface mav)
        {
            InitializeComponent();

            this.mav = mav;

            mav.OnPacketReceived += MavOnOnPacketReceived;

            mavi.NewSysidCompid += (sender, args) =>
            {
                this.BeginInvoke((MethodInvoker)delegate
               {
                   comboBox1.DataSource = mavi.SeenSysid();
                   comboBox2.DataSource = mavi.SeenCompid();
               });
            };

            timer1.Tick += (sender, args) => Update();

            timer1.Start();

            ThemeManager.ApplyThemeTo(this);
        }

        private void MavOnOnPacketReceived(object o, MAVLink.MAVLinkMessage linkMessage)
        {
            mavi.Add(linkMessage.sysid, linkMessage.compid, linkMessage.msgid, linkMessage, linkMessage.Length);
        }

        public new void Update()
        {
            treeView1.BeginUpdate();

            bool added = false;

            foreach (var mavLinkMessage in mavi.GetPacketMessages())
            {
                TreeNode sysidnode;
                TreeNode compidnode;
                TreeNode msgidnode;

                var sysidnodes = treeView1.Nodes.Find(mavLinkMessage.sysid.ToString(), false);
                if (sysidnodes.Length == 0)
                {
                    sysidnode = new TreeNode("Vehicle " + mavLinkMessage.sysid)
                    {
                        Name = mavLinkMessage.sysid.ToString()
                    };
                    treeView1.Nodes.Add(sysidnode);
                    added = true;
                }
                else
                    sysidnode = sysidnodes.First();

                var compidnodes = sysidnode.Nodes.Find(mavLinkMessage.compid.ToString(), false);
                if (compidnodes.Length == 0)
                {
                    compidnode = new TreeNode("Comp " + mavLinkMessage.compid)
                    {
                        Name = mavLinkMessage.compid.ToString()
                    };
                    sysidnode.Nodes.Add(compidnode);
                    added = true;
                }
                else
                    compidnode = compidnodes.First();

                var msgidnodes = compidnode.Nodes.Find(mavLinkMessage.msgid.ToString(), false);
                if (msgidnodes.Length == 0)
                {
                    msgidnode = new TreeNode(mavLinkMessage.msgtypename)
                    {
                        Name = mavLinkMessage.msgid.ToString()
                    };
                    compidnode.Nodes.Add(msgidnode);
                    added = true;
                }
                else
                    msgidnode = msgidnodes.First();

                var msgidheader = mavLinkMessage.msgtypename + " (" +
                                  (mavi.SeenRate(mavLinkMessage.sysid, mavLinkMessage.compid, mavLinkMessage.msgid))
                                  .ToString("0.0 Hz") + ", #" + mavLinkMessage.msgid + ") " +
                                  mavi.SeenBps(mavLinkMessage.sysid, mavLinkMessage.compid, mavLinkMessage.msgid).ToString("0bps");

                if (msgidnode.Text != msgidheader)
                    msgidnode.Text = msgidheader;

                var minfo = MAVLink.MAVLINK_MESSAGE_INFOS.GetMessageInfo(mavLinkMessage.msgid);
                if (minfo.@type == null)
                    continue;

                foreach (var field in minfo.type.GetFields())
                {
                    if (!msgidnode.Nodes.ContainsKey(field.Name))
                    {
                        msgidnode.Nodes.Add(new TreeNode() { Name = field.Name });
                        added = true;
                    }

                    object value = field.GetValue(mavLinkMessage.data);

                    if (field.Name == "time_unix_usec")
                    {
                        DateTime date1 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        try
                        {
                            value = date1.AddMilliseconds((ulong)value / 1000);
                        }
                        catch
                        {
                        }
                    }

                    if (field.FieldType.IsArray)
                    {
                        var subtype = value.GetType();

                        var value2 = (Array)value;

                        if (field.Name == "param_id") // param_value
                        {
                            value = new String((char[])value2);
                        }
                        else if (field.Name == "text") // statustext
                        {
                            value = new String((char[])value);
                        }
                        else
                        {
                            value = value2.Cast<object>().Aggregate((a, b) => a + "," + b);
                        }
                    }

                    msgidnode.Nodes[field.Name].Tag = new[]
                    {
                        field.Name, value,
                        field.FieldType.ToString()
                    };
                    msgidnode.Nodes[field.Name].Text = (String.Format("{0,-32} {1,20} {2,-20}", field.Name, value,
                        field.FieldType.ToString()));
                }
            }

            if (added)
                treeView1.Sort();

            treeView1.EndUpdate();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.treeView1 = new MissionPlanner.Controls.MAVLinkInspector.MyTreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.but_graphit = new MissionPlanner.Controls.MyButton();
            this.chk_gcstraffic = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.FullRowSelect = true;
            this.treeView1.Location = new System.Drawing.Point(3, 16);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(521, 259);
            this.treeView1.TabIndex = 0;
            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.treeView1);
            this.groupBox1.Location = new System.Drawing.Point(0, 30);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(527, 278);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(206, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.Visible = false;
            // 
            // comboBox2
            // 
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Location = new System.Drawing.Point(356, 3);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 21);
            this.comboBox2.TabIndex = 3;
            this.comboBox2.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Interval = 333;
            // 
            // but_graphit
            // 
            this.but_graphit.Enabled = false;
            this.but_graphit.Location = new System.Drawing.Point(12, 3);
            this.but_graphit.Name = "but_graphit";
            this.but_graphit.Size = new System.Drawing.Size(75, 23);
            this.but_graphit.TabIndex = 4;
            this.but_graphit.Text = "Graph It";
            this.but_graphit.UseVisualStyleBackColor = true;
            this.but_graphit.Click += new System.EventHandler(this.but_graphit_Click);
            // 
            // chk_gcstraffic
            // 
            this.chk_gcstraffic.AutoSize = true;
            this.chk_gcstraffic.Location = new System.Drawing.Point(93, 5);
            this.chk_gcstraffic.Name = "chk_gcstraffic";
            this.chk_gcstraffic.Size = new System.Drawing.Size(111, 17);
            this.chk_gcstraffic.TabIndex = 5;
            this.chk_gcstraffic.Text = "Show GCS Traffic";
            this.chk_gcstraffic.UseVisualStyleBackColor = true;
            this.chk_gcstraffic.CheckedChanged += new System.EventHandler(this.chk_gcstraffic_CheckedChanged);
            // 
            // MAVLinkInspector
            // 
            this.ClientSize = new System.Drawing.Size(526, 311);
            this.Controls.Add(this.chk_gcstraffic);
            this.Controls.Add(this.but_graphit);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.groupBox1);
            this.Name = "MAVLinkInspector";
            this.Text = "Mavlink Inspector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MAVLinkInspector_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Bounds.Y < 0 || e.Bounds.X == -1)
                return;

            var tv = sender as TreeView;

            if (e.Node.Tag == null)
            {
                e.DrawDefault = true;
                return;
            }

            var items = (object[])e.Node.Tag;

            //(String.Format("{0,-32} {1,20} {2,-20}", field.Name, value, field.FieldType.ToString()));

            e.Graphics.DrawString(items[0].ToString(), tv.Font, new SolidBrush(tv.ForeColor)
                , e.Bounds.X,
                e.Bounds.Y);

            e.Graphics.DrawString(items[1].ToString().PadLeft(20, ' '), tv.Font, new SolidBrush(tv.ForeColor)
                , e.Bounds.X + tv.Width * 0.4f,
                e.Bounds.Y);

            e.Graphics.DrawString(items[2].ToString(), tv.Font, new SolidBrush(tv.ForeColor)
                , e.Bounds.X + tv.Width * 0.75f,
                e.Bounds.Y);
        }

        private void MAVLinkInspector_FormClosing(object sender, FormClosingEventArgs e)
        {
            mav.OnPacketReceived -= MavOnOnPacketReceived;
            mav.OnPacketSent -= MavOnOnPacketReceived;

            timer1.Stop();
        }

        public class MyTreeView : TreeView
        {
            public MyTreeView()
            {
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                //UpdateStyles();
            }
        }

        private (string msgid, string name) selectedmsgid;

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e == null || e.Node == null || e.Node.Parent == null)
                return;

            int throwaway = 0;
            if (int.TryParse(e.Node.Parent.Name, out throwaway))
            {
                selectedmsgid = (e.Node.Parent.Name, e.Node.Name);
                but_graphit.Enabled = true;
            }
            else
            {
                but_graphit.Enabled = false;
            }
        }

        int history = 50;

        private void but_graphit_Click(object sender, EventArgs e)
        {
            InputBox.Show("Points", "Points of history?", ref history);
            var form = new Form() { Size = new Size(640, 480) };
            var zg1 = new ZedGraphControl() { Dock = DockStyle.Fill };
            var msgid = int.Parse(selectedmsgid.msgid);
            var msgidfield = selectedmsgid.name;
            var line = new LineItem(msgidfield, new RollingPointPairList(history), Color.Red, SymbolType.None);
            zg1.GraphPane.Title.Text = "";
            try
            {
                var msginfo = MAVLink.MAVLINK_MESSAGE_INFOS.First(a => a.msgid == msgid);
                var typeofthing = msginfo.type.GetField(
                    msgidfield);
                if (typeofthing != null)
                {
                    var attrib = typeofthing.GetCustomAttributes(false);
                    if (attrib.Length > 0)
                        zg1.GraphPane.YAxis.Title.Text = attrib.OfType<MAVLink.Units>().First().Unit;
                }
            }
            catch { }

            zg1.GraphPane.CurveList.Add(line);

            zg1.GraphPane.XAxis.Type = AxisType.Date;
            zg1.GraphPane.XAxis.Scale.Format = "HH:mm:ss.fff";
            zg1.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Minute;
            zg1.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Second;

            var timer = new Timer() { Interval = 100 };
            var subscribeToPacket =
                mav.SubscribeToPacketType((MAVLink.MAVLINK_MSG_ID)msgid,
                    message =>
                    {
                        line.AddPoint(new XDate(message.rxtime),
                            (double)(dynamic)message.data.GetPropertyOrField(msgidfield));
                        return true;
                    });
            timer.Tick += (o, args) =>
            {
                // Make sure the Y axis is rescaled to accommodate actual data
                zg1.AxisChange();

                // Force a redraw

                zg1.Invalidate();

            };
            form.Controls.Add(zg1);
            form.Closing += (o2, args2) => { mav.UnSubscribeToPacketType(subscribeToPacket); };
            ThemeManager.ApplyThemeTo(form);
            form.Show(this);
            timer.Start();
            but_graphit.Enabled = false;
        }

        private void chk_gcstraffic_CheckedChanged(object sender, EventArgs e)
        {
            if (chk_gcstraffic.Checked)
                mav.OnPacketSent += MavOnOnPacketReceived;
            if (!chk_gcstraffic.Checked)
            {
                mav.OnPacketSent -= MavOnOnPacketReceived;
                mav.OnPacketSent -= MavOnOnPacketReceived;
            }
        }
    }
}
