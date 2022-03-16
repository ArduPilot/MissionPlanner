using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;
using Microsoft.Win32;
using MissionPlanner;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using px4uploader;

namespace Flasher
{
    public class FlasherForm : Form
    {
        private readonly string[] _args;
        private RichTextBox textBox1;
        private Button button1;
        private TextBox txt_vid;
        private TextBox txt_pid;
        private TextBox txt_int;
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FlasherForm(string[] args)
        {
            _args = args;

            OpenFileDialog odf = new OpenFileDialog();
            odf.DefaultExt = ".apj";
            odf.Filter = "*.apj|*.apj";

            odf.ShowDialog(this);
            _args = new[] {odf.FileName};

            InitializeComponent();
        }
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.txt_vid = new System.Windows.Forms.TextBox();
            this.txt_pid = new System.Windows.Forms.TextBox();
            this.txt_int = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(12, 34);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(570, 409);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 5);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "scan now";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txt_vid
            // 
            this.txt_vid.Location = new System.Drawing.Point(93, 5);
            this.txt_vid.Name = "txt_vid";
            this.txt_vid.Size = new System.Drawing.Size(52, 20);
            this.txt_vid.TabIndex = 2;
            this.txt_vid.Text = "2DAE";
            // 
            // txt_pid
            // 
            this.txt_pid.Location = new System.Drawing.Point(151, 5);
            this.txt_pid.Name = "txt_pid";
            this.txt_pid.Size = new System.Drawing.Size(49, 20);
            this.txt_pid.TabIndex = 3;
            this.txt_pid.Text = "1057";
            // 
            // txt_int
            // 
            this.txt_int.Location = new System.Drawing.Point(206, 5);
            this.txt_int.Name = "txt_int";
            this.txt_int.Size = new System.Drawing.Size(43, 20);
            this.txt_int.TabIndex = 4;
            this.txt_int.Text = "00";
            // 
            // FlasherForm
            // 
            this.ClientSize = new System.Drawing.Size(594, 455);
            this.Controls.Add(this.txt_int);
            this.Controls.Add(this.txt_pid);
            this.Controls.Add(this.txt_vid);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Name = "FlasherForm";
            this.Load += new System.EventHandler(this.FlasherForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void FlasherForm_Load(object sender, System.EventArgs e)
        {
 
        }


        [StructLayout(LayoutKind.Sequential)]
        public class DEV_BROADCAST_HDR
        {
            public Int32 dbch_size;
            public Int32 dbch_devicetype;
            public Int32 dbch_reserved;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class DEV_BROADCAST_PORT
        {
            public int dbcp_size;
            public int dbcp_devicetype;
            public int dbcp_reserved; // MSDN say "do not use"
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)] public string dbcp_name;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class DEV_BROADCAST_DEVICEINTERFACE
        {
            public Int32 dbcc_size;
            public Int32 dbcc_devicetype;
            public Int32 dbcc_reserved;

            [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 16)]
            public Byte[]
                dbcc_classguid;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)] public string dbcc_name;
        }

        private static string GetDeviceName(DEV_BROADCAST_DEVICEINTERFACE dvi)
        {
            string[] Parts = dvi.dbcc_name.Split('#');
            if (Parts.Length >= 3)
            {
                string DevType = Parts[0].Substring(Parts[0].IndexOf(@"?\") + 2);
                string DeviceInstanceId = Parts[1];
                string DeviceUniqueID = Parts[2];
                string RegPath = @"SYSTEM\CurrentControlSet\Enum\" + DevType + "\\" + DeviceInstanceId + "\\" + DeviceUniqueID;
                RegistryKey key = Registry.LocalMachine.OpenSubKey(RegPath);
                if (key != null)
                {
                    object result = key.GetValue("FriendlyName");
                    if (result != null)
                        return result.ToString();
                    result = key.GetValue("DeviceDesc");
                    if (result != null)
                        return result.ToString();
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Compile an array of COM port names associated with given VID and PID
        /// </summary>
        /// <param name="VID">string representing the vendor id of the USB/Serial convertor</param>
        /// <param name="PID">string representing the product id of the USB/Serial convertor</param>
        /// <returns></returns>
        private static List<string> getPortByVPid(String VID, String PID)
        {
            String pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (String s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            comports.Add((string)rk6.GetValue("PortName"));
                        }
                    }
                }
            }
            return comports;
        }

        private static List<string> getPortByVPidInt(String VID, String PID, String Interface)
        {
            String pattern = String.Format("^VID_{0}.PID_{1}.MI_{2}", VID, PID, Interface);
            Regex _rx = new Regex(pattern, RegexOptions.IgnoreCase);
            List<string> comports = new List<string>();
            RegistryKey rk1 = Registry.LocalMachine;
            RegistryKey rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum");
            foreach (String s3 in rk2.GetSubKeyNames())
            {
                RegistryKey rk3 = rk2.OpenSubKey(s3);
                foreach (String s in rk3.GetSubKeyNames())
                {
                    if (_rx.Match(s).Success)
                    {
                        RegistryKey rk4 = rk3.OpenSubKey(s);
                        foreach (String s2 in rk4.GetSubKeyNames())
                        {
                            RegistryKey rk5 = rk4.OpenSubKey(s2);
                            RegistryKey rk6 = rk5.OpenSubKey("Device Parameters");
                            if(rk6 != null)
                                comports.Add((string)rk6.GetValue("PortName"));
                        }
                    }
                }
            }
            return comports;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_CREATE:
                    try
                    {
                        DEV_BROADCAST_DEVICEINTERFACE devBroadcastDeviceInterface = new DEV_BROADCAST_DEVICEINTERFACE();
                        IntPtr devBroadcastDeviceInterfaceBuffer;
                        IntPtr deviceNotificationHandle = IntPtr.Zero;
                        Int32 size = 0;

                        // frmMy is the form that will receive device-change messages.


                        size = Marshal.SizeOf(devBroadcastDeviceInterface);
                        devBroadcastDeviceInterface.dbcc_size = size;
                        devBroadcastDeviceInterface.dbcc_devicetype = DBT_DEVTYP_DEVICEINTERFACE;
                        devBroadcastDeviceInterface.dbcc_reserved = 0;
                        devBroadcastDeviceInterface.dbcc_classguid = GUID_DEVINTERFACE_USB_DEVICE.ToByteArray();
                        devBroadcastDeviceInterfaceBuffer = Marshal.AllocHGlobal(size);
                        Marshal.StructureToPtr(devBroadcastDeviceInterface, devBroadcastDeviceInterfaceBuffer, true);


                        deviceNotificationHandle = NativeMethods.RegisterDeviceNotification(this.Handle,
                            devBroadcastDeviceInterfaceBuffer, DEVICE_NOTIFY_WINDOW_HANDLE);
                    }
                    catch
                    {
                    }

                    break;

                case WM_DEVICECHANGE:
                    // The WParam value identifies what is occurring.
                    WM_DEVICECHANGE_enum n = (WM_DEVICECHANGE_enum)m.WParam;
                    var l = m.LParam;
                    if (n == WM_DEVICECHANGE_enum.DBT_DEVICEREMOVEPENDING)
                    {
                        Console.WriteLine("DBT_DEVICEREMOVEPENDING");
                    }
                    if (n == WM_DEVICECHANGE_enum.DBT_DEVNODES_CHANGED)
                    {
                        Console.WriteLine("DBT_DEVNODES_CHANGED");

                       // DoScan();
                    }
                    if (n == WM_DEVICECHANGE_enum.DBT_DEVICEARRIVAL)
                    {
                        Console.WriteLine(((WM_DEVICECHANGE_enum)n).ToString());

                        DEV_BROADCAST_HDR hdr = new DEV_BROADCAST_HDR();
                        Marshal.PtrToStructure(m.LParam, hdr);

                        try
                        {
                            switch (hdr.dbch_devicetype)
                            {
                                case DBT_DEVTYP_DEVICEINTERFACE:
                                    DEV_BROADCAST_DEVICEINTERFACE inter = new DEV_BROADCAST_DEVICEINTERFACE();
                                    Marshal.PtrToStructure(m.LParam, inter);
                                    var devname = GetDeviceName(inter);
                                    Console.WriteLine("Interface {0} {1}", inter.dbcc_name, devname);
                                    break;
                                case DBT_DEVTYP_PORT:
                                    DEV_BROADCAST_PORT prt = new DEV_BROADCAST_PORT();
                                    Marshal.PtrToStructure(m.LParam, prt);
                                    Console.WriteLine("port {0}",prt.dbcp_name);

                                    Uploader up = null;
                                    try
                                    {
                                        up = new px4uploader.Uploader(new SerialPort(prt.dbcp_name, 115200));

                                        up.identify();

                                        up.close();
                                    }
                                    catch
                                    {
                                    }
                                    break;
                            }
                        }
                        catch
                        {
                        }

                        //string port = Marshal.PtrToStringAuto((IntPtr)((long)m.LParam + 12));
                        //Console.WriteLine("Added port {0}",port);
                    }
                    Console.WriteLine("Device Change {0} {1} {2}", m.Msg, (WM_DEVICECHANGE_enum)m.WParam, m.LParam);

                    if (DeviceChanged != null)
                    {
                        try
                        {
                            DeviceChanged((WM_DEVICECHANGE_enum)m.WParam);
                        }
                        catch
                        {
                        }
                    }

             

                    break;
     
                default:
                    //Console.WriteLine(m.ToString());
                    break;
            }
            base.WndProc(ref m);
        }

        private void DoScan()
        {
            portstatus.Clear();

            var ports = SerialPort.GetPortNames();

            ports.ForEach(a =>
            {
                new Thread(FWUpload).Start(new Tuple<int, string>(index++, a));
            });
        }

        private Dictionary<string, string> portstatus = new Dictionary<string, string>();

        private void FWUpload(object a)
        {
            
            Tuple<int, string> passin = (Tuple<int, string>)a;
            var index = passin.Item1;
            var portname = passin.Item2;
            Console.WriteLine("Thread start for " + portname);
            try
            {
                if (!portstatus.ContainsKey(portname))
                    portstatus[portname] = "";

                {
                   
                    Uploader up = null;
                    try
                    {
                        up = new px4uploader.Uploader(new SerialPort(portname, 115200));
                    }
                    catch
                    {
                    }

                    if (up != null)
                    {
                        up.ProgressEvent += completed =>
                        {
                           /* this.BeginInvoke(new Action(() =>
                            {
                                textBox1.AppendText(portname + " " + (int)completed + "\r\n");
                            }));*/
                        };
                        
                        bool flashit = false;
                        bool validcomms = false;
                        Firmware fw = null;
                        try
                        {
                            up.identify();
                            validcomms = true;
                            portstatus[portname] += "BL ";
                            UpdateTextBox();
                            fw = Firmware.ProcessFirmware(_args[0]);
                            up.currentChecksum(fw);

                            fw.board_id = up.board_type;

                            flashit = true;
                            portstatus[portname] += "CS ";
                            UpdateTextBox();
                        }
                        catch (Exception ex)
                        {
                            if(ex.Message.Contains("Same Firmware"))
                                portstatus[portname] += "BLUPD OK ";
                            else
                                portstatus[portname] += "BLEX ";
                            UpdateTextBox();
                        }

                        if (flashit)
                        {
                            DateTime ts = DateTime.Now;
                            up.ProgressEvent += completed =>
                            {
                                if (ts.Second != DateTime.Now.Second)
                                {
                                    Console.WriteLine("{0}: {1}", portname, completed);
                                    ts = DateTime.Now;
                                }
                            };
                            up.upload(fw);
                            portstatus[portname] += "BLUPD OK ";
                            UpdateTextBox();
                        }
                        else
                        {
                            up.__reboot();
                            UpdateTextBox();
                        }

                        up.close();

                        if (validcomms)
                            return;
                    }
                }

                var ports = getPortByVPidInt(txt_vid.Text, txt_pid.Text, txt_int.Text);
                if (!ports.Contains(portname))
                {
                    portstatus[portname] += "NOTML OK ";
                    UpdateTextBox();
                    return;
                }

                var mav = new MAVLinkInterface();
                mav.BaseStream = new SerialPort((string) portname, 115200);
                var bldone = false;
                mav.OnPacketReceived += (sender, message) =>
                {
                    if (message.msgid == (ulong) MAVLink.MAVLINK_MSG_ID.STATUSTEXT)
                    {
                        if(ASCIIEncoding.ASCII.GetString(message.ToStructure<MAVLink.mavlink_statustext_t>().text).Trim().Contains("Bootloader up-to-date") ||
                           ASCIIEncoding.ASCII.GetString(message.ToStructure<MAVLink.mavlink_statustext_t>().text).Trim().Contains("Flash OK"))
                        {
                            bldone = true;

                            portstatus[portname] += ASCIIEncoding.ASCII
                                .GetString(message.ToStructure<MAVLink.mavlink_statustext_t>().text).Trim()
                                .TrimEnd(new char[] {'\0', '\r', '\n'});
                            UpdateTextBox();
                        }
                        //DisplayText(message);
                    }
                };
                try
                {
                    mav.BaseStream.Open();

                    mav.getHeartBeat();

                    mav.doCommand(MAVLink.MAV_CMD.FLASH_BOOTLOADER, 0, 0, 0, 0, 290876, 0, 0, false);

                    mav.getHeartBeat();
                    if (!bldone) mav.getHeartBeat();
                    if (!bldone) mav.getHeartBeat();
                    if (!bldone) mav.getHeartBeat();
                    if (!bldone) mav.getHeartBeat();
                    if (!bldone) mav.getHeartBeat();
                    if (!bldone) mav.getHeartBeat();
                    if (!bldone) mav.getHeartBeat();
                    if (!bldone) mav.getHeartBeat();

                    mav.Close();


                    if (bldone)
                        portstatus[portname] += "MLBLUPD OK";

                    UpdateTextBox();
                }
                catch (Exception ex)
                {
                    portstatus[portname] += "MLEX ";

                    UpdateTextBox();
                }
            }
            catch (Exception ex)
            {
                portstatus[portname] += "EX!! ";

                UpdateTextBox();
            }

            UpdateTextBox();
        }

        private void UpdateTextBox()
        {
            this.BeginInvoke(new Action(() =>
            {
                try
                {
                    var blcount = 0;
                    var fwcount = 0;
                    textBox1.Text = "";
                    portstatus.ToArray().Select(a => a.Key + ": " + a.Value + "\r\n").Select(
                            a =>
                            {
                                if (a.Contains("MLBLUPD OK")) blcount++;
                                if (a.Contains("BLUPD OK")) fwcount++;
                                textBox1.AppendText(a, a.Contains(" OK") ? Color.Green : Color.Red);
                                return 0;
                            })
                        .ToArray();

                    textBox1.AppendText("bootloader updated: " + blcount + "\r\n");
                    textBox1.AppendText("firmware uploaded: " + fwcount + "\r\n");
                    textBox1.AppendText("entries: " + portstatus.Count + "\r\n");
                } catch {}
            }));
        }
        /*
        private void DisplayText(MAVLink.MAVLinkMessage message)
        {
            this.BeginInvoke(new Action(() => { textBox1.AppendText(ASCIIEncoding.ASCII.GetString(message.ToStructure<MAVLink.mavlink_statustext_t>().text).Trim() + "\r\n"); }));
        }*/

        const int DBT_DEVTYP_PORT = 0x00000003;
        const int WM_CREATE = 0x0001;
        const Int32 DBT_DEVTYP_HANDLE = 6;
        const Int32 DBT_DEVTYP_DEVICEINTERFACE = 5;
        const Int32 DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        const Int32 DIGCF_PRESENT = 2;
        const Int32 DIGCF_DEVICEINTERFACE = 0X10;
        const Int32 WM_DEVICECHANGE = 0X219;
        public static Guid GUID_DEVINTERFACE_USB_DEVICE = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED");
        private int index;


        public enum WM_DEVICECHANGE_enum
        {
            DBT_CONFIGCHANGECANCELED = 0x19,
            DBT_CONFIGCHANGED = 0x18,
            DBT_CUSTOMEVENT = 0x8006,
            DBT_DEVICEARRIVAL = 0x8000,
            DBT_DEVICEQUERYREMOVE = 0x8001,
            DBT_DEVICEQUERYREMOVEFAILED = 0x8002,
            DBT_DEVICEREMOVECOMPLETE = 0x8004,
            DBT_DEVICEREMOVEPENDING = 0x8003,
            DBT_DEVICETYPESPECIFIC = 0x8005,
            DBT_DEVNODES_CHANGED = 0x7,
            DBT_QUERYCHANGECONFIG = 0x17,
            DBT_USERDEFINED = 0xFFFF,
        }

        public delegate void WMDeviceChangeEventHandler(WM_DEVICECHANGE_enum cause);

        public event WMDeviceChangeEventHandler DeviceChanged;

        private void button1_Click(object sender, EventArgs e)
        {
            
            DoScan();
        }
    }
    public class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr RegisterDeviceNotification
        (IntPtr hRecipient,
            IntPtr NotificationFilter,
            Int32 Flags);
    }
    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}