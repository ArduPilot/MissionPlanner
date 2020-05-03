using MissionPlanner.Utilities;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using UAVCAN;
using ZedGraph;

namespace MissionPlanner.Controls
{
    public class UAVCANInspector : Form
    {
        private GroupBox groupBox1;
        private MyTreeView treeView1;
        private Timer timer1;
        private IContainer components;

        private PacketInspector<(UAVCAN.CANFrame frame, object message)>
            pktinspect = new PacketInspector<(UAVCAN.CANFrame, object)>();
        private MyButton but_graphit;
        private UAVCAN.uavcan can;

        public UAVCANInspector(UAVCAN.uavcan can)
        {
            InitializeComponent();

            this.can = can;

            can.MessageReceived += Can_MessageReceived;

            pktinspect.NewSysidCompid += (sender, args) =>
            {

            };

            timer1.Tick += (sender, args) => Update();

            timer1.Start();

            ThemeManager.ApplyThemeTo(this);
        }

        private void Can_MessageReceived(UAVCAN.CANFrame frame, object msg, byte transferID)
        {
            pktinspect.Add(frame.SourceNode, 0, frame.MsgTypeID, (frame, msg), 0);
        }

        public new void Update()
        {
            treeView1.BeginUpdate();

            bool added = false;

            foreach (var uavcanMessage in pktinspect.GetPacketMessages())
            {
                TreeNode sysidnode;
                TreeNode compidnode;
                TreeNode msgidnode;

                var sysidnodes = treeView1.Nodes.Find(uavcanMessage.frame.SourceNode.ToString(), false);
                if (sysidnodes.Length == 0)
                {
                    sysidnode = new TreeNode("ID " + uavcanMessage.frame.SourceNode)
                    {
                        Name = uavcanMessage.frame.SourceNode.ToString()
                    };
                    treeView1.Nodes.Add(sysidnode);
                    added = true;
                }
                else
                    sysidnode = sysidnodes.First();

                var compidnodes = sysidnode.Nodes.Find(0.ToString(), false);
                if (compidnodes.Length == 0)
                {
                    compidnode = new TreeNode("Comp " + 0)
                    {
                        Name = 0.ToString()
                    };
                    sysidnode.Nodes.Add(compidnode);
                    added = true;
                }
                else
                    compidnode = compidnodes.First();

                var msgidnodes = compidnode.Nodes.Find(uavcanMessage.frame.MsgTypeID.ToString(), false);
                if (msgidnodes.Length == 0)
                {
                    msgidnode = new TreeNode(uavcanMessage.frame.MsgTypeID.ToString())
                    {
                        Name = uavcanMessage.frame.MsgTypeID.ToString()
                    };
                    compidnode.Nodes.Add(msgidnode);
                    added = true;
                }
                else
                    msgidnode = msgidnodes.First();

                var msgidheader = uavcanMessage.message.GetType().Name + " (" +
                                  (pktinspect.SeenRate(uavcanMessage.frame.SourceNode, 0, uavcanMessage.frame.MsgTypeID))
                                  .ToString("0.0 Hz") + ", #" + uavcanMessage.frame.MsgTypeID + ") ";

                if (msgidnode.Text != msgidheader)
                    msgidnode.Text = msgidheader;

                var minfo = UAVCAN.uavcan.MSG_INFO.First(a => a.Item1 == uavcanMessage.Item2.GetType());
                var fields = minfo.Item1.GetFields();

                PopulateMSG(fields, msgidnode, uavcanMessage.message);
            }

            if (added)
                treeView1.Sort();

            treeView1.EndUpdate();
        }

        private static void PopulateMSG(FieldInfo[] Fields, TreeNode MsgIdNode, object message)
        {
            bool added;
            foreach (var field in Fields)
            {
                if (!MsgIdNode.Nodes.ContainsKey(field.Name))
                {
                    MsgIdNode.Nodes.Add(new TreeNode() { Name = field.Name });
                    added = true;
                }

                object value = field.GetValue(message);

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

                    if (field.Name == "param_id" || field.Name == "text" ||
                        field.Name == "string_value" || field.Name == "name") // param_value
                    {
                        value = ASCIIEncoding.ASCII.GetString((byte[])value2);
                    }
                    else
                    {
                        value = value2.Cast<object>().Aggregate((a, b) => a + "," + b);
                    }
                }

                if (!field.FieldType.IsArray && field.FieldType.IsClass)
                {
                    MsgIdNode.Nodes[field.Name].Text = field.Name;
                    PopulateMSG(field.FieldType.GetFields(), MsgIdNode.Nodes[field.Name], value);
                    continue;
                }

                MsgIdNode.Nodes[field.Name].Text = (String.Format("{0,-32} {1,20} {2,-20}", field.Name, value,
                    field.FieldType.Name));
            }
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.treeView1 = new MissionPlanner.Controls.UAVCANInspector.MyTreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.but_graphit = new MissionPlanner.Controls.MyButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.Location = new System.Drawing.Point(3, 16);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(693, 259);
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
            this.groupBox1.Size = new System.Drawing.Size(699, 278);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
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
            // UAVCANInspector
            // 
            this.ClientSize = new System.Drawing.Size(698, 311);
            this.Controls.Add(this.but_graphit);
            this.Controls.Add(this.groupBox1);
            this.Name = "UAVCANInspector";
            this.Text = "UAVCAN Inspector";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MAVLinkInspector_FormClosing);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Bounds.Y < 0 || e.Bounds.X == -1)
                return;

            var tv = sender as TreeView;

            new SolidBrush(Color.FromArgb(e.Bounds.Y % 200, e.Bounds.Y % 200, e.Bounds.Y % 200));

            e.Graphics.DrawString(e.Node.Text, tv.Font, new SolidBrush(this.ForeColor)
                , e.Bounds.X,
                e.Bounds.Y);
        }

        private void MAVLinkInspector_FormClosing(object sender, FormClosingEventArgs e)
        {
            can.MessageReceived -= Can_MessageReceived;

            timer1.Stop();
        }

        public class MyTreeView : TreeView
        {
            public MyTreeView()
            {
                SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
                //SetStyle(ControlStyles.AllPaintingInWmPaint, true);
                //UpdateStyles();
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                if (GetStyle(ControlStyles.UserPaint))
                {
                    Message m = new Message
                    {
                        HWnd = Handle
                    };
                    int WM_PRINTCLIENT = 0x318;
                    m.Msg = WM_PRINTCLIENT;
                    m.WParam = e.Graphics.GetHdc();
                    int PRF_CLIENT = 0x00000004;
                    m.LParam = (IntPtr)PRF_CLIENT;
                    DefWndProc(ref m);
                    e.Graphics.ReleaseHdc(m.WParam);
                }
                base.OnPaint(e);
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
                var msginfo = uavcan.MSG_INFO.First(a => a.Item2 == msgid);
                var typeofthing = msginfo.Item1.GetField(
                    msgidfield);
                if (typeofthing != null)
                {
                    var attrib = typeofthing.GetCustomAttributes(false).OfType<MAVLink.Units>().ToArray();
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
            uavcan.MessageRecievedDel msgrecv = (frame, msg, id) =>
            {
                line.AddPoint(new XDate(DateTime.Now),
                    (double)(dynamic)msg.GetPropertyOrField(msgidfield));
            };
            can.MessageReceived += msgrecv;
            timer.Tick += (o, args) =>
            {
                // Make sure the Y axis is rescaled to accommodate actual data
                zg1.AxisChange();

                // Force a redraw

                zg1.Invalidate();

            };
            form.Controls.Add(zg1);
            form.Closing += (o2, args2) => { can.MessageReceived -= msgrecv; };
            ThemeManager.ApplyThemeTo(form);
            form.Show(this);
            timer.Start();
            but_graphit.Enabled = false;
        }
    }
}
