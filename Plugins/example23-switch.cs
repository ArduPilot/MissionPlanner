using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MissionPlanner.Controls;
using MissionPlanner.GCSViews;
using MissionPlanner.Plugin;
using log4net;
using System.Linq;
using DeviceProgramming;
using System.Threading;
using Xamarin.Forms.PlatformConfiguration;

//loadassembly: DeviceProgramming
//loadassembly: log4net

namespace MissionPlanner.plugins
{
    public class ConfigSwitch : UserControl, IActivate
    {
        private readonly static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
                
        public struct Command
        {
            public byte phyaddress;
            public byte register;
            public byte highbyte;
            public byte lowbyte;
        }

        const ushort CommandStart = 0x55aa;
        List<Command> CommandList = new List<Command>();
        private TableLayoutPanel tableLayoutPanel1;
        const ushort CommandEnd = 0xaa55;

        public enum phyaddress
        {
            cos = 21, // class of service
            EEE = 22, // Energy Efficient Ethernet
            vlan = 23, // Virtual Local Area Network
        }

        /// <summary>
        /// Initializes the ConfigPayload control with a checklist of available payload selections.
        /// </summary>
        public ConfigSwitch()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }


        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 12.5F));
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 13;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(596, 322);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // ConfigSwitch
            // 
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ConfigSwitch";
            this.Size = new System.Drawing.Size(596, 322);
            this.ResumeLayout(false);

        }

        /// <summary>
        /// Called when the ConfigPayload view is activated.
        /// </summary>
        public void Activate()
        {
            tableLayoutPanel1.PerformLayout();

            tableLayoutPanel1.RowCount = 13;
            for (int a = 0; a <= 7; a++)
            {
                CheckBox cbcos = new CheckBox();
                cbcos.Text = "Port " + a + " COS Enable";

                cbcos.Tag = (21, a, 10);

                tableLayoutPanel1.Controls.Add(cbcos);

                cbcos.CheckedChanged += (s, e) => {
                    var data = (ValueTuple<int, int, int>)cbcos.Tag;
                    ProcessCmd(data, cbcos.Checked);
                };

                SetDefault(cbcos, false);
            }

            for (int a = 0; a <= 7; a++)
            {
                CheckBox cbcoshp = new CheckBox();
                cbcoshp.Text = "Port " + a + " COS High Priority";
                cbcoshp.Tag = (21, a, 11);

                tableLayoutPanel1.Controls.Add(cbcoshp);

                cbcoshp.CheckedChanged += (s, e) => {
                    var data = (ValueTuple<int, int, int>)cbcoshp.Tag;
                    ProcessCmd(data, cbcoshp.Checked);
                };

                SetDefault(cbcoshp, false);
            }

            for (int a = 0; a <= 7; a++)
            {
                CheckBox cbeee = new CheckBox();

                cbeee.Text = "Port " + a + " EEE Enable";
                cbeee.Tag = (22, 0, a);

                tableLayoutPanel1.Controls.Add(cbeee);

                cbeee.CheckedChanged += (s, e) => {
                    var data = (ValueTuple<int, int, int>)cbeee.Tag;
                    ProcessCmd(data, cbeee.Checked);
                };

                SetDefault(cbeee, true);
            }

            for (int a = 0; a <= 7; a++)
            {
                CheckBox cbtagged = new CheckBox();
                cbtagged.Checked = false;

                cbtagged.Text = "Port " + a + " VLAN Tagged";

                cbtagged.Tag = (23, 13, a);

                tableLayoutPanel1.Controls.Add(cbtagged);

                cbtagged.CheckedChanged += (s, e) => {
                    var data = (ValueTuple<int, int, int>)cbtagged.Tag;
                    ProcessCmd(data, cbtagged.Checked);
                };

                SetDefault(cbtagged, false);
            }

            for (int port = 0; port <= 7; port++)
            {
                for (int a = 0; a <= 7; a++)
                {
                    CheckBox cbvlanportmembers = new CheckBox();

                    cbvlanportmembers.Text = "Port " + port + " to Port " + a + " ";

                    var high = (int)(port / 2);
                    var align = port % 2;

                    cbvlanportmembers.Tag = (23, 15 + high, a + (align == 1 ? 8 : 0));

                    tableLayoutPanel1.Controls.Add(cbvlanportmembers);

                    cbvlanportmembers.CheckedChanged += (s, e) =>
                    {
                        var data = (ValueTuple<int, int, int>)cbvlanportmembers.Tag;
                        ProcessCmd(data, cbvlanportmembers.Checked);
                    };

                    SetDefault(cbvlanportmembers, true);
                }
            }

            MyButton btn = new MyButton();
            btn.Text = "Apply";
            btn.Click += (s, e) =>
            {
                Apply();
            };
            tableLayoutPanel1.Controls.Add(btn);

            MyButton btn2 = new MyButton();
            btn2.Text = "Reset";
            btn2.Click += (s, e) =>
            {
                CommandList.Clear();
                Apply();
            };
            tableLayoutPanel1.Controls.Add(btn2);

            var buffer = new byte[100];
            var result = MainV2.comPort.device_op(1, 1, out buffer,
            MAVLink.DEVICE_OP_BUSTYPE.I2C,
            "", 0, 80,
            0, (byte)buffer.Length);

            if (result != 0)
            {
                this.Enabled = false;
                return;
            }
            // load save config
            decode(buffer);

            tableLayoutPanel1.Controls.Add(new Label() { Text = buffer.Select(a => a.ToString("X2")).Aggregate((a, b) => a + b) });
            tableLayoutPanel1.Controls.Add(new Label() { Text = "i2c2 only" });

            // restore config
            foreach (var item in tableLayoutPanel1.Controls)
            {
                if(item is CheckBox)
                {
                    SetDefaultFromConfig((CheckBox)item);
                }
            }
        }

        private void SetDefault(CheckBox cbcontrol, bool @default)
        {
            // set the default
            cbcontrol.Checked = @default;
            // create the config
            var data = (ValueTuple<int, int, int>)cbcontrol.Tag;
            ProcessCmd(data, @default);
        }

        private void SetDefaultFromConfig(CheckBox cbcontrol)
        {
            var data = (ValueTuple<int, int, int>)cbcontrol.Tag;
            // get the saved default
            var newdefault = ProcessCmdGet(data);

            cbcontrol.Checked = newdefault;
        }

        private void decode(byte[] buffer) 
        {
            var work = buffer.AsSpan();
            if (buffer[0] == 0xaa && buffer[1] == 0x55)
            {
                // drop header
                work = work.Slice(2);
                while(work.Length > 3)
                {
                    var phy = work[0];
                    var reg = work[1];
                    if (work[0] == 0x55 && work[1] == 0xaa)
                        break;
                    var value = (work[2] << 8) + work[3];

                    var command = new Command() { phyaddress = phy, register = reg, highbyte = work[2], lowbyte = work[3] };

                    log.InfoFormat("phy {0} {1} reg {2} value {3}", phy, (phyaddress)phy, reg, value);

                    work = work.Slice(4);

                    // add to list - replace existing
                    var item = CommandList.Where(a => a.phyaddress == command.phyaddress && a.register == command.register);
                    if (item.Count() > 0)
                        CommandList.Remove(item.First());

                    CommandList.Add(command);
                }
            }
        }

        private void Apply()
        {
            var start = BitConverter.GetBytes(CommandStart);
            var end = BitConverter.GetBytes(CommandEnd);

            var data = new List<byte>();
            data.AddRange(start);

            CommandList.Sort((a, b) =>
            {
                var cmp1 = a.phyaddress.CompareTo(b.phyaddress);
                var cmp2 = a.register.CompareTo(b.register);
                if (cmp1 != 0)
                    return cmp1;
                return cmp2;
            });

            CommandList.ForEach(a => data.AddRange(a.ToBytes()));

            data.AddRange(end);

            data.AddRange(BitConverter.GetBytes(0xffffffff));

            byte count = 1;
            var buffer = new byte[count];

            var offset = 0;
            foreach (var item in data)
            {
            again:

                // read it
                var result = MainV2.comPort.device_op(1, 1, out buffer,
     MAVLink.DEVICE_OP_BUSTYPE.I2C,
    "", 0, 80,
    (byte)offset, 1);

                if (result != 0 || buffer.Length == 0)
                    goto again;

                // skip already set
                if (buffer[0] == item)
                {
                    offset++;
                    continue;
                }

                // write it
                result = MainV2.comPort.device_op(1, 1, out buffer,
               MAVLink.DEVICE_OP_BUSTYPE.I2C,
              "", 0, 80,
              (byte)offset, 0, new[] { item });

                if (result != 0)
                {
                    log.Error("bad i2c op " + result);
                    goto again;
                }

                goto again;
            }
        }

        int SetBitTo1(int value, int position)
        {
            return value |= (1 << position);
        }

        int SetBitTo0(int value, int position)
        {
            return value & ~(1 << position);
        }

        bool IsBitSetTo1(int value, int position)
        {
            // Return whether bit at position is set to 1.
            return (value & (1 << position)) != 0;
        }

        bool IsBitSetTo0(int value, int position)
        {
            // If not 1, bit is 0.
            return !IsBitSetTo1(value, position);
        }

        private void ProcessCmd((int, int, int) data, bool @checked)
        {
            // update
            if (CommandList.Any(a => a.phyaddress == data.Item1 && a.register == data.Item2))
            {
                var item = CommandList.First(a => a.phyaddress == data.Item1 && a.register == data.Item2);
                CommandList.Remove(item);

                var value = (item.highbyte << 8) + item.lowbyte;

                if (@checked)
                    value = (ushort)SetBitTo1(value, data.Item3);
                else
                    value = (ushort)SetBitTo0(value, data.Item3);

                item.highbyte = (byte)(value >> 8);
                item.lowbyte = (byte)(value & 0xff);

                CommandList.Add(item);
            }
            else
            {
                //add
                var value = (ushort)((@checked ? 1 : 0) << data.Item3);

                CommandList.Add(new Command()
                {
                    phyaddress = (byte)data.Item1,
                    register = (byte)data.Item2,
                    highbyte = (byte)(value >> 8),
                    lowbyte = (byte)(value & 0xff)
                });
            }
        }

        private bool ProcessCmdGet((int, int, int) data)
        {
            if (CommandList.Any(a => a.phyaddress == data.Item1 && a.register == data.Item2))
            {
                var item = CommandList.First(a => a.phyaddress == data.Item1 && a.register == data.Item2);

                var value = (item.highbyte << 8) + item.lowbyte;

                var ans = IsBitSetTo1(value, data.Item3);
                log.InfoFormat("ProcessCmdGet: phy {0} reg {1} value {2} bit {3} ans {4}", item.phyaddress, item.register, value, data.Item3, ans);
                return ans;
            }

            throw new Exception("no prior config - setdefault should have created this");
        }

        public class example23_switch : Plugin.Plugin
        {
            public override string Name => "CubeLan 8 port Switch";
            public override string Version => "0.1";
            public override string Author => "Michael Oborne";


            public override bool Init()
            {
                return true;
            }

            public override bool Loaded()
            {
                // Register the ConfigPayload view on the Config tab
                SoftwareConfig.AddPluginViewPage(typeof(ConfigSwitch), Name, SoftwareConfig.pageOptions.isConnected);

                return true;
            }

            public override bool Exit() { return true; }
        }
    }
}