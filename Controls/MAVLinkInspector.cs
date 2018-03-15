using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Scripting.Utils;
using MissionPlanner.Mavlink;
using MissionPlanner.Utilities;
using ProtoBuf.Meta;

namespace MissionPlanner.Controls
{
    public class MAVLinkInspector: Form
    {
        private GroupBox groupBox1;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private MyTreeView treeView1;
        private Timer timer1;
        private IContainer components;
        MAVInspector mavi = new MAVInspector();
        private MAVLinkInterface mav;

        public MAVLinkInspector(MAVLinkInterface mav)
        {
            InitializeComponent();

            this.mav = mav;

            mav.OnPacketReceived += MavOnOnPacketReceived;

            mavi.NewSysidCompid += (sender, args) =>
            {
                this.BeginInvoke((MethodInvoker) delegate
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
            mavi.Add(linkMessage);
        }

        public new void Update()
        {
            treeView1.BeginUpdate();

            bool added = false;

            foreach (var mavLinkMessage in mavi.GetMAVLinkMessages())
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
                                  .ToString("0.0 Hz") + ", #" + mavLinkMessage.msgid + ") ";

                if (msgidnode.Text != msgidheader)
                    msgidnode.Text = msgidheader;

                var minfo = MAVLink.MAVLINK_MESSAGE_INFOS.GetMessageInfo(mavLinkMessage.msgid);

                foreach (var field in minfo.type.GetFields())
                {
                    if (!msgidnode.Nodes.ContainsKey(field.Name))
                    {
                        msgidnode.Nodes.Add(new TreeNode() {Name = field.Name});
                        added = true;
                    }

                    object value = field.GetValue(mavLinkMessage.data);

                    if (field.FieldType.IsArray)
                    {
                        var subtype = value.GetType();

                        var value2 = (Array) value;

                        if (field.Name == "param_id") // param_value
                        {
                            value = ASCIIEncoding.ASCII.GetString((byte[]) value2);
                        }
                        else if (field.Name == "text") // statustext
                        {
                            value = ASCIIEncoding.ASCII.GetString((byte[]) value2);
                        }
                        else
                        {
                            value = value2.Cast<object>().Aggregate((a, b) => a + "," + b);
                        }
                    }

                    msgidnode.Nodes[field.Name].Text = (String.Format("{0,-32} {1,20} {2,-20}", field.Name, value,
                        field.FieldType.ToString()));
                }
            }

            if(added)
                treeView1.Sort();

            treeView1.EndUpdate();
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.treeView1 = new MyTreeView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.treeView1.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.treeView1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeView1.Location = new System.Drawing.Point(3, 16);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(693, 259);
            this.treeView1.TabIndex = 0;
            this.treeView1.DrawNode += new System.Windows.Forms.DrawTreeNodeEventHandler(this.treeView1_DrawNode);
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
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(88, 3);
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
            // MAVLinkInspector
            // 
            this.ClientSize = new System.Drawing.Size(698, 311);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.groupBox1);
            this.Name = "MAVLinkInspector";
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
            mav.OnPacketReceived -= MavOnOnPacketReceived;

            timer1.Stop();
        }

        public class MyTreeView: TreeView
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
    }
}
