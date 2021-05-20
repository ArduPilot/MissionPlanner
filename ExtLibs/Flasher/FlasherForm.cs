using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using log4net;
using MissionPlanner;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using px4uploader;

namespace Flasher
{
    public class FlasherForm : Form
    {
        private readonly string[] _args;
        private TextBox textBox1;
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public FlasherForm(string[] args)
        {
            _args = args;

            OpenFileDialog odf = new OpenFileDialog();
            odf.DefaultExt = ".apj";

            odf.ShowDialog(this);
            _args = new[] {odf.FileName};

            InitializeComponent();
        }
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(570, 431);
            this.textBox1.TabIndex = 0;
            textBox1.Anchor = (AnchorStyles) 15;
            // 
            // FlasherForm
            // 
            this.ClientSize = new System.Drawing.Size(594, 455);
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
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)] public byte[] dbcp_name;
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

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 255)] public Byte[] dbcc_name;
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
                    }
                    if (n == WM_DEVICECHANGE_enum.DBT_DEVICEARRIVAL ||
                        n == WM_DEVICECHANGE_enum.DBT_DEVICEREMOVECOMPLETE)
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
                                    log.InfoFormat("Interface {0}",
                                        ASCIIEncoding.Unicode.GetString(inter.dbcc_name, 0, inter.dbcc_size - (4 * 3)));
                                    break;
                                case DBT_DEVTYP_PORT:
                                    DEV_BROADCAST_PORT prt = new DEV_BROADCAST_PORT();
                                    Marshal.PtrToStructure(m.LParam, prt);
                                    log.InfoFormat("port {0}",
                                        ASCIIEncoding.Unicode.GetString(prt.dbcp_name, 0, prt.dbcp_size - (4 * 3)));

                                    this.BeginInvoke(new Action(() =>
                                    {
                                        textBox1.AppendText("port " +
                                                            ASCIIEncoding.Unicode.GetString(prt.dbcp_name, 0,
                                                                prt.dbcp_size - (4 * 3)).Trim().TrimEnd('\0') + "\r\n");
                                    }));

                                    new Thread(FWUpload).Start(new Tuple<int, string>(index++,
                                        ASCIIEncoding.Unicode.GetString(prt.dbcp_name, 0, prt.dbcp_size - (4 * 3))
                                            .Trim().TrimEnd('\0')));

                                    break;
                            }
                        }
                        catch
                        {
                        }

                        //string port = Marshal.PtrToStringAuto((IntPtr)((long)m.LParam + 12));
                        //Console.WriteLine("Added port {0}",port);
                    }
                    log.InfoFormat("Device Change {0} {1} {2}", m.Msg, (WM_DEVICECHANGE_enum)m.WParam, m.LParam);

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

        private List<string> ports = new List<string>();

        private void FWUpload(object? a)
        {
            try
            {
                Tuple<int, string> passin = (Tuple<int, string>)a;
                var index = passin.Item1;
                var portname = passin.Item2;
                var tryupload = true;

                lock (ports)
                    if (ports.Contains(portname))
                        tryupload = false;

                if (tryupload)
                {
                    var fw = Firmware.ProcessFirmware(_args[0]);
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
                        try
                        {
                            up.identify();
                            validcomms = true;
                            up.currentChecksum(fw);
                            flashit = true;
                        }
                        catch (Exception ex)
                        {
                            AppendLine(ex.Message);
                        }

                        if (flashit)
                        {
                            this.BeginInvoke(new Action(() => { textBox1.AppendText(portname + " Flashing fw\r\n"); }));
                            up.upload(fw);
                        }
                        else
                        {
                            this.BeginInvoke(new Action(() => { textBox1.AppendText(portname + " reboot\r\n"); }));
                            up.__reboot();
                        }

                        lock (ports)
                            ports.Add(portname);
                        up.close();
                        this.BeginInvoke(new Action(() => { textBox1.AppendText(portname + " Done fw\r\n"); }));
                        if (validcomms)
                            return;
                    }
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
                        }
                        DisplayText(message);
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


                    this.BeginInvoke(new Action(() =>
                    {
                        textBox1.AppendText(portname + " Done bl " + (bldone ? "OK" : "FAIL") + "\r\n");
                    }));
                }
                catch (Exception ex)
                {
                    AppendLine(ex.Message);
                }
            }
            catch (Exception ex)
            {
                AppendLine(ex.Message);
            }
        }

        private void AppendLine(string line)
        {
            this.BeginInvoke(new Action(() => { textBox1.AppendText(line + "\r\n"); }));
        }

        private void DisplayText(MAVLink.MAVLinkMessage message)
        {
            this.BeginInvoke(new Action(() => { textBox1.AppendText(ASCIIEncoding.ASCII.GetString(message.ToStructure<MAVLink.mavlink_statustext_t>().text).Trim() + "\r\n"); }));
        }

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
    }
    public class NativeMethods
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr RegisterDeviceNotification
        (IntPtr hRecipient,
            IntPtr NotificationFilter,
            Int32 Flags);
    }
}