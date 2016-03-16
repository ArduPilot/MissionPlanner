using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using log4net;
using MissionPlanner.Comms;
using MissionPlanner.Controls;
using MissionPlanner.Log;
using MissionPlanner.Utilities;
using SerialPort = MissionPlanner.Comms.SerialPort;

namespace MissionPlanner.GCSViews
{
    public partial class Terminal : MyUserControl, IActivate, IDeactivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal static ICommsSerial comPort;
        public static bool threadrun;
        private readonly List<string> cmdHistory = new List<string>();
        private readonly object thisLock = new object();
        private int history;
        private bool inlogview;
        private int inputStartPos;
        DateTime lastsend = DateTime.MinValue;

        public Terminal()
        {
            threadrun = false;

            InitializeComponent();
        }

        public void Activate()
        {
            MainV2.instance.MenuConnect.Visible = false;
        }

        public void Deactivate()
        {
            try
            {
                if (comPort.IsOpen)
                {
                    //comPort.Write("\rexit\rreboot\r");

                    comPort.Close();
                }
            }
            catch
            {
            }

            MainV2.instance.MenuConnect.Visible = true;
        }

        private void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!comPort.IsOpen)
                return;

            // if btr > 0 then this shouldnt happen
            comPort.ReadTimeout = 300;

            try
            {
                lock (thisLock)
                {
                    var buffer = new byte[256];
                    var a = 0;

                    while (comPort.IsOpen && comPort.BytesToRead > 0 && !inlogview)
                    {
                        var indata = (byte) comPort.ReadByte();

                        buffer[a] = indata;

                        if (buffer[a] >= 0x20 && buffer[a] < 0x7f || buffer[a] == '\n' || buffer[a] == 0x1b)
                        {
                            a++;
                        }

                        if (indata == '\n')
                            break;

                        if (a == (buffer.Length - 1))
                            break;
                    }

                    addText(Encoding.ASCII.GetString(buffer, 0, a + 1));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                if (!threadrun) return;
                TXT_terminal.AppendText("Error reading com port\r\n");
            }
        }

        private void addText(string data)
        {
            BeginInvoke((MethodInvoker) delegate
            {
                if (this.Disposing)
                    return;

                if (inputStartPos > TXT_terminal.Text.Length)
                    inputStartPos = TXT_terminal.Text.Length - 1;

                // gather current typed data
                string currenttypedtext = TXT_terminal.Text.Substring(inputStartPos,
                    TXT_terminal.Text.Length - inputStartPos);

                // remove typed data
                TXT_terminal.Text = TXT_terminal.Text.Remove(inputStartPos, TXT_terminal.Text.Length - inputStartPos);

                TXT_terminal.SelectionStart = TXT_terminal.Text.Length;

                data = data.TrimEnd('\r'); // else added \n all by itself
                data = data.Replace("\0", "");
                data = data.Replace((char) 0x1b + "[K", ""); // remove control code
                TXT_terminal.AppendText(data);

                if (data.Contains("\b"))
                {
                    TXT_terminal.Text = TXT_terminal.Text.Remove(TXT_terminal.Text.IndexOf('\b'));
                    TXT_terminal.SelectionStart = TXT_terminal.Text.Length;
                }

                // erase to end of line. in our case jump to end of line
                if (data.Contains((char) 0x1b + "[K"))
                {
                    TXT_terminal.SelectionStart = TXT_terminal.Text.Length;
                }
                inputStartPos = TXT_terminal.SelectionStart;

                //add back typed text
                TXT_terminal.AppendText(currenttypedtext);
            });
        }

        private void TXT_terminal_Click(object sender, EventArgs e)
        {
            // auto scroll
            //TXT_terminal.SelectionStart = TXT_terminal.Text.Length;

            //TXT_terminal.ScrollToCaret();

            //TXT_terminal.Refresh();
        }

        private void TXT_terminal_KeyDown(object sender, KeyEventArgs e)
        {
            TXT_terminal.SelectionStart = TXT_terminal.Text.Length;
            /*    if (e.KeyData == Keys.Up || e.KeyData == Keys.Down || e.KeyData == Keys.Left || e.KeyData == Keys.Right)
                {
                    e.Handled = true; // ignore it
                }*/
            lock (thisLock)
            {
                switch (e.KeyData)
                {
                    case Keys.Up:
                        if (history > 0)
                        {
                            TXT_terminal.Select(inputStartPos, TXT_terminal.Text.Length - inputStartPos);
                            TXT_terminal.SelectedText = "";
                            TXT_terminal.AppendText(cmdHistory[--history]);
                        }
                        e.Handled = true;
                        break;
                    case Keys.Down:
                        if (history < cmdHistory.Count - 1)
                        {
                            TXT_terminal.Select(inputStartPos, TXT_terminal.Text.Length - inputStartPos);
                            TXT_terminal.SelectedText = "";
                            TXT_terminal.AppendText(cmdHistory[++history]);
                        }
                        e.Handled = true;
                        break;
                    case Keys.Left:
                    case Keys.Back:
                        if (TXT_terminal.SelectionStart <= inputStartPos)
                            e.Handled = true;
                        break;

                    //case Keys.Right:
                    //    break;
                }
            }
        }

        private void Terminal_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadrun = false;

            try
            {
                if (comPort != null && comPort.IsOpen)
                {
                    comPort.Close();
                }
            }
            catch
            {
            } // Exception System.IO.IOException: The specified port does not exist.

            //System.Threading.Thread.Sleep(400);
        }

        private void TXT_terminal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (comPort.IsOpen)
                {
                    try
                    {
                        var cmd = "";
                        lock (thisLock)
                        {
                            if (MainV2.MONO)
                            {
                                cmd = TXT_terminal.Text.Substring(inputStartPos,
                                    TXT_terminal.Text.Length - inputStartPos);
                            }
                            else
                            {
                                cmd = TXT_terminal.Text.Substring(inputStartPos,
                                    TXT_terminal.Text.Length - inputStartPos - 1);
                            }
                            TXT_terminal.Select(inputStartPos, TXT_terminal.Text.Length - inputStartPos);
                            TXT_terminal.SelectedText = "";
                            if (cmd.Length > 0 && (cmdHistory.Count == 0 || cmdHistory.Last() != cmd))
                            {
                                cmdHistory.Add(cmd);
                                history = cmdHistory.Count;
                            }
                        }

                        log.Info("Command: " + cmd);

                        // do not change this  \r is correct - no \n
                        if (cmd == "+++")
                        {
                            comPort.Write(Encoding.ASCII.GetBytes(cmd), 0, cmd.Length);
                            lastsend = DateTime.Now;
                        }
                        else
                        {
                            comPort.Write(Encoding.ASCII.GetBytes(cmd + "\r"), 0, cmd.Length + 1);
                            lastsend = DateTime.Now;
                        }
                    }
                    catch
                    {
                        CustomMessageBox.Show(Strings.ErrorCommunicating, Strings.ERROR);
                    }
                }
            }
            /*
            if (comPort.IsOpen)
            {
                try
                {
                    comPort.Write(new byte[] { (byte)e.KeyChar }, 0, 1);
                }
                catch { MessageBox.Show("Error writing to com port"); }
            }
            e.Handled = true;*/
        }

        private void waitandsleep(int time)
        {
            var start = DateTime.Now;

            while ((DateTime.Now - start).TotalMilliseconds < time && !inlogview)
            {
                try
                {
                    if (!comPort.IsOpen || comPort.BytesToRead > 0)
                    {
                        return;
                    }
                }
                catch
                {
                    threadrun = false;
                    return;
                }
            }
        }

        private void readandsleep(int time)
        {
            var start = DateTime.Now;

            while ((DateTime.Now - start).TotalMilliseconds < time && !inlogview)
            {
                try
                {
                    if (!comPort.IsOpen)
                        return;
                    if (comPort.BytesToRead > 0)
                    {
                        comPort_DataReceived(null, null);
                    }
                }
                catch
                {
                    threadrun = false;
                    return;
                }
            }
        }

        private void Terminal_Load(object sender, EventArgs e)
        {
            setcomport();
        }

        private void setcomport()
        {
            if (comPort == null)
            {
                try
                {
                    comPort = new SerialPort();
                    comPort.PortName = MainV2.comPortName;
                    comPort.BaudRate = int.Parse(MainV2._connectionControl.CMB_baudrate.Text);
                    comPort.ReadBufferSize = 1024*1024*4;
                }
                catch
                {
                    CustomMessageBox.Show(Strings.InvalidBaudRate, Strings.ERROR);
                }
            }
        }

        private void start_Terminal(bool px4)
        {
            setcomport();

            try
            {
                if (MainV2.comPort != null && MainV2.comPort.BaseStream != null && MainV2.comPort.BaseStream.IsOpen)
                    MainV2.comPort.BaseStream.Close();

                if (comPort.IsOpen)
                {
                    Console.WriteLine("Terminal Start - Close Port");
                    threadrun = false;
                    //  if (DialogResult.Cancel == CustomMessageBox.Show("The port is open\n Continue?", "Continue", MessageBoxButtons.YesNo))
                    {
                        //  return;
                    }

                    comPort.Close();

                    // allow things to cleanup
                    Thread.Sleep(400);
                }

                comPort.ReadBufferSize = 1024*1024*4;

                comPort.PortName = MainV2.comPortName;

                // test moving baud rate line

                comPort.BaudRate = int.Parse(MainV2._connectionControl.CMB_baudrate.Text);

                if (px4)
                {
                    TXT_terminal.AppendText("Rebooting " + MainV2.comPortName + " at " + comPort.BaudRate + "\n");
                    // keep it local
                    using (var mine = new MAVLinkInterface())
                    {
                        mine.BaseStream.PortName = MainV2.comPortName;
                        mine.BaseStream.BaudRate = comPort.BaudRate;

                        mine.giveComport = true;
                        mine.BaseStream.Open();

                        // check if we are a mavlink stream
                        var buffer = mine.readPacket();

                        if (buffer.Length > 0)
                        {
                            log.Info("got packet - sending reboot via mavlink");
                            TXT_terminal.AppendText("Via Mavlink\n");
                            mine.doReboot(false);
                            try
                            {
                                mine.BaseStream.Close();
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            log.Info("no packet - sending reboot via console");
                            TXT_terminal.AppendText("Via Console\n");
                            try
                            {
                                mine.BaseStream.Write("reboot\r");
                                mine.BaseStream.Write("exit\rreboot\r");
                            }
                            catch
                            {
                            }
                            try
                            {
                                mine.BaseStream.Close();
                            }
                            catch
                            {
                            }
                        }
                    }

                    TXT_terminal.AppendText("Waiting for reboot\n");

                    // wait 7 seconds for px4 reboot
                    log.Info("waiting for reboot");
                    var deadline = DateTime.Now.AddSeconds(9);
                    while (DateTime.Now < deadline)
                    {
                        Thread.Sleep(500);
                        Application.DoEvents();
                    }

                    var a = 0;
                    while (a < 5)
                    {
                        try
                        {
                            if (!comPort.IsOpen)
                                comPort.Open();
                        }
                        catch
                        {
                        }
                        Thread.Sleep(200);
                        a++;
                    }
                }
                else
                {
                    log.Info("About to open " + comPort.PortName);

                    comPort.Open();

                    log.Info("toggle dtr");

                    comPort.toggleDTR();
                }

                try
                {
                    comPort.DiscardInBuffer();
                }
                catch
                {
                }

                startreadthread();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                TXT_terminal.AppendText("Cant open serial port\r\n");
                return;
            }
        }

        private void startreadthread()
        {
            Console.WriteLine("Terminal_Load run " + threadrun + " " + comPort.IsOpen);

            BUT_disconnect.Enabled = true;

            var t11 = new Thread(delegate()
            {
                threadrun = true;

                Console.WriteLine("Terminal thread start run run " + threadrun + " " + comPort.IsOpen);

                try
                {
                    comPort.Write("\r");
                }
                catch
                {
                }

                // 10 sec
                waitandsleep(10000);

                Console.WriteLine("Terminal thread 1 run " + threadrun + " " + comPort.IsOpen);

                // 100 ms
                readandsleep(100);

                Console.WriteLine("Terminal thread 2 run " + threadrun + " " + comPort.IsOpen);

                try
                {
                    if (!inlogview && comPort.IsOpen)
                        comPort.Write("\n\n\n");

                    // 1 secs
                    if (!inlogview && comPort.IsOpen)
                        readandsleep(1000);

                    if (!inlogview && comPort.IsOpen)
                        comPort.Write("\r\r\r?\r");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Terminal thread 3 " + ex);
                    ChangeConnectStatus(false);
                    threadrun = false;
                    return;
                }

                Console.WriteLine("Terminal thread 3 run " + threadrun + " " + comPort.IsOpen);

                while (threadrun)
                {
                    try
                    {
                        Thread.Sleep(10);

                        if (!threadrun)
                            break;
                        if (this.Disposing)
                            break;
                        if (inlogview)
                            continue;
                        if (!comPort.IsOpen)
                        {
                            Console.WriteLine("Comport Closed");
                            ChangeConnectStatus(false);
                            break;
                        }
                        if (comPort.BytesToRead > 0)
                        {
                            comPort_DataReceived(null, null);
                        }

                        if (comPort is MAVLinkSerialPort)
                        {
                            if (lastsend.AddMilliseconds(500) > DateTime.Now)
                            {
                                // 20 hz
                                ((MAVLinkSerialPort) comPort).timeout = 50;
                            }
                            else
                            {
                                // 5 hz
                                ((MAVLinkSerialPort) comPort).timeout = 200;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Terminal thread 4 " + ex);
                    }
                }

                threadrun = false;
                try
                {
                    comPort.DtrEnable = false;
                }
                catch
                {
                }
                try
                {
                    Console.WriteLine("term thread close run " + threadrun + " " + comPort.IsOpen);
                    ChangeConnectStatus(false);
                    comPort.Close();
                }
                catch
                {
                }

                Console.WriteLine("Comport thread close run " + threadrun);
            });
            t11.IsBackground = true;
            t11.Name = "Terminal serial thread";
            t11.Start();

            // doesnt seem to work on mac
            //comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);

            if (IsDisposed || Disposing)
                return;

            TXT_terminal.AppendText("Opened com port\r\n");
            inputStartPos = TXT_terminal.SelectionStart;


            TXT_terminal.Focus();
        }

        private void ChangeConnectStatus(bool connected)
        {
            if (IsDisposed || Disposing)
                return;

            Invoke((MethodInvoker) delegate
            {
                if (connected && BUT_disconnect.Enabled == false)
                {
                    BUT_disconnect.Enabled = true;
                }
                else if (!connected && BUT_disconnect.Enabled)
                {
                    BUT_disconnect.Enabled = false;
                }
            });
        }

        private void BUTsetupshow_Click(object sender, EventArgs e)
        {
            if (comPort.IsOpen)
            {
                try
                {
                    var encoding = new ASCIIEncoding();
                    var data = encoding.GetBytes("exit\rsetup\rshow\r");
                    comPort.Write(data, 0, data.Length);
                }
                catch
                {
                }
            }
            TXT_terminal.Focus();
        }

        private void BUTradiosetup_Click(object sender, EventArgs e)
        {
            if (comPort.IsOpen)
            {
                try
                {
                    var encoding = new ASCIIEncoding();
                    var data = encoding.GetBytes("exit\rsetup\r\nradio\r");
                    comPort.Write(data, 0, data.Length);
                }
                catch
                {
                }
            }
            TXT_terminal.Focus();
        }

        private void BUTtests_Click(object sender, EventArgs e)
        {
            if (comPort.IsOpen)
            {
                try
                {
                    var encoding = new ASCIIEncoding();
                    var data = encoding.GetBytes("exit\rtest\r?\r\n");
                    comPort.Write(data, 0, data.Length);
                }
                catch
                {
                }
            }
            TXT_terminal.Focus();
        }

        private void Logs_Click(object sender, EventArgs e)
        {
            inlogview = true;
            Thread.Sleep(300);
            Form Log = new LogDownload();
            ThemeManager.ApplyThemeTo(Log);
            Log.ShowDialog();
            inlogview = false;
        }

        private void BUT_logbrowse_Click(object sender, EventArgs e)
        {
            Form logbrowse = new LogBrowse();
            ThemeManager.ApplyThemeTo(logbrowse);
            logbrowse.Show();
        }

        private void BUT_RebootAPM_Click(object sender, EventArgs e)
        {
            if (comPort.IsOpen)
            {
                BUT_disconnect.Enabled = true;
                return;
            }

            if (MainV2.comPort.BaseStream.IsOpen)
            {
                if (CMB_boardtype.Text.Contains("NSH"))
                {
                    start_NSHTerminal();
                    return;
                }

                MainV2.comPort.BaseStream.Close();
            }

            if (CMB_boardtype.Text.Contains("APM"))
                start_Terminal(false);
            if (CMB_boardtype.Text.Contains("PX4"))
                start_Terminal(true);
            if (CMB_boardtype.Text.Contains("VRX"))
                start_Terminal(true);
        }

        private void start_NSHTerminal()
        {
            try
            {
                if (MainV2.comPort != null && MainV2.comPort.BaseStream != null && MainV2.comPort.BaseStream.IsOpen)
                {
                    comPort = new MAVLinkSerialPort(MainV2.comPort, MAVLink.SERIAL_CONTROL_DEV.SHELL);

                    comPort.BaudRate = 0;

                    // 20 hz
                    ((MAVLinkSerialPort) comPort).timeout = 50;

                    comPort.Open();

                    startreadthread();
                }
            }
            catch
            {
            }
        }

        private void BUT_disconnect_Click(object sender, EventArgs e)
        {
            try
            {
                try
                {
                    comPort.Write("reboot\n");
                }
                catch
                {
                }
                comPort.Close();
                TXT_terminal.AppendText("Closed\n");
            }
            catch
            {
            }
        }
    }
}