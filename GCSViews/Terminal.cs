using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner;
using System.IO.Ports;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;
using System.Text.RegularExpressions;
using log4net;
using System.Reflection;
using MissionPlanner.Controls;

namespace MissionPlanner.GCSViews
{
    public partial class Terminal : MyUserControl, IActivate, IDeactivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static internal ICommsSerial comPort;
        Object thisLock = new Object();
        
        List<string> cmdHistory = new List<string>();
        int history = 0;
        int inputStartPos = 0;

        //state control variables
        bool inlogview = false;
        bool threaderror = false;
        bool threadrun = false;
        System.Threading.Thread t11;

        bool lastbuttonstate = true;

        public Terminal()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            MainV2.instance.MenuConnect.Visible = false;
        }

        public void Deactivate()
        {
            ExitThread();

            MainV2.instance.MenuConnect.Visible = true;
        }

        void ExitThread()
        {
            if (threadrun)
            {
                threadrun = false;
                t11.Join();
            }
        }

        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!comPort.IsOpen)
                return;

            // if btr > 0 then this shouldnt happen
            comPort.ReadTimeout = 300;

            lock (thisLock)
            {
                byte[] buffer = new byte[256];
                int a = 0;

                while (comPort.IsOpen && comPort.BytesToRead > 0 && threadrun)
                {
                    byte indata = (byte)comPort.ReadByte();

                    buffer[a] = indata;

                    if (buffer[a] >= 0x20 && buffer[a] < 0x7f || buffer[a] == (int)'\n' || buffer[a] == 0x1b)
                    {
                        a++;
                    }

                    if (indata == '\n')
                        break;

                    if (a == (buffer.Length-1))
                        break;
                }

                addText(ASCIIEncoding.ASCII.GetString(buffer,0,a+1));
            }
        }

        void addText(string data)
        {
            this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
            {
                if (this.Disposing)
                    return;

                TXT_terminal.SelectionStart = TXT_terminal.Text.Length;

                data = data.TrimEnd('\r'); // else added \n all by itself
                data = data.Replace("\0", "");
                data = data.Replace((char)0x1b+"[K",""); // remove control code
                TXT_terminal.AppendText(data);

                if (data.Contains("\b"))
                {
                    TXT_terminal.Text = TXT_terminal.Text.Remove(TXT_terminal.Text.IndexOf('\b'));
                    TXT_terminal.SelectionStart = TXT_terminal.Text.Length;
                }
               
                // erase to end of line. in our case jump to end of line
                if (data.Contains((char)0x1b + "[K"))
                {
                    TXT_terminal.SelectionStart = TXT_terminal.Text.Length;
                }
                inputStartPos = TXT_terminal.SelectionStart;
            });

        }

        private void TXT_terminal_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.Modifiers & Keys.Control) !=0)
            {
                if (e.KeyValue == (int)Keys.X)
                    e.Handled = true; // ignore it, to prevent 'cut' action
                return; //let it be handled by the system, for example ctrl+c for copy
            }
            TXT_terminal.SelectionStart = TXT_terminal.Text.Length;

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
            //done by Deactivate()

            //ExitThread();

            //MainV2.instance.MenuConnect.Visible = true;
        }

        private void TXT_terminal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (comPort.IsOpen)
                {
                    try
                    {
                        string cmd = "";
                        lock (thisLock)
                        {
                            if (MainV2.MONO)
                            {
                                cmd = TXT_terminal.Text.Substring(inputStartPos, TXT_terminal.Text.Length - inputStartPos);
                            }
                            else
                            {
                                cmd = TXT_terminal.Text.Substring(inputStartPos, TXT_terminal.Text.Length - inputStartPos - 1);
                            }
                            TXT_terminal.Select(inputStartPos, TXT_terminal.Text.Length - inputStartPos);
                            TXT_terminal.SelectedText = "";
                            if (cmd.Length > 0 && (cmdHistory.Count == 0 || cmdHistory.Last() != cmd))
                            {
                                cmdHistory.Add(cmd);
                                history = cmdHistory.Count;
                            }
                        }
                        // do not change this  \r is correct - no \n
                        if (cmd == "+++")
                        {
                            comPort.Write(Encoding.ASCII.GetBytes(cmd), 0, cmd.Length);
                        }
                        else
                        {
                            comPort.Write(Encoding.ASCII.GetBytes(cmd + "\r"), 0, cmd.Length + 1);
                        }
                    }
                    catch { CustomMessageBox.Show("Error writing to com port"); }
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
            DateTime start = DateTime.Now;

            while ((DateTime.Now - start).TotalMilliseconds < time)
            {
                if (!threadrun || comPort.BytesToRead > 0)
                {
                    return;
                }
            }
        }

        private void readandsleep(int time)
        {
            DateTime start = DateTime.Now;

            while ((DateTime.Now - start).TotalMilliseconds < time)
            {
                if (!threadrun)
                    return;
                if (comPort.BytesToRead > 0)
                {
                    comPort_DataReceived((object)null, (SerialDataReceivedEventArgs)null);
                }
            }
        }

        private void Terminal_Load(object sender, EventArgs e)
        {
            setButtonState(false);
        }

        void setcomport()
        {
            if (comPort == null)
            {
                comPort = new MissionPlanner.Comms.SerialPort();
                comPort.ReadBufferSize = 1024 * 1024 * 4;                
            }
            comPort.PortName = MainV2.comPortName;
            comPort.BaudRate = int.Parse(MainV2._connectionControl.CMB_baudrate.Text);
        }

        private void start_Terminal(bool px4)
        {
            try
            {
                if (MainV2.comPort != null && MainV2.comPort.BaseStream != null && MainV2.comPort.BaseStream.IsOpen)
                    MainV2.comPort.BaseStream.Close();

                setcomport();

                if (px4)
                {
                    TXT_terminal.AppendText("Rebooting " + MainV2.comPortName + " at " + comPort.BaudRate + "\n");
                    // keep it local
                    using (MAVLinkInterface mine = new MAVLinkInterface())
                    {

                        mine.BaseStream.PortName = MainV2.comPortName;
                        mine.BaseStream.BaudRate = comPort.BaudRate;

                        mine.giveComport = true;
                        mine.BaseStream.Open();

                        // check if we are a mavlink stream
                        byte[] buffer = mine.readPacket();

                        if (buffer.Length > 0)
                        {
                            log.Info("got packet - sending reboot via mavlink");
                            TXT_terminal.AppendText("Via Mavlink\n");
                            mine.doReboot(false);
                            try
                            {
                                mine.BaseStream.Close();
                            }
                            catch { }

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
                            catch { }
                            try
                            {
                                mine.BaseStream.Close();
                            }
                            catch { }
                        }
                    }

                    TXT_terminal.AppendText("Waiting for reboot\n");

                    // wait 7 seconds for px4 reboot
                    log.Info("waiting for reboot");
                    DateTime deadline = DateTime.Now.AddSeconds(9);
                    while (DateTime.Now < deadline)
                    {
                        System.Threading.Thread.Sleep(500);
                        Application.DoEvents();
                    }
                   
                    int a = 0;
                    while (a < 5)
                    {
                        try
                        {
                            if (!comPort.IsOpen)
                                comPort.Open();
                        }
                        catch { }
                        System.Threading.Thread.Sleep(200);
                        a++;
                    }
                     
                }
                else
                {
                    log.Info("About to open " + comPort.PortName);

                    comPort.Open();

                    log.Info("Toggle dtr");

                    comPort.toggleDTR();
                }

                try
                {
                    comPort.DiscardInBuffer();
                }
                catch { }

                Console.WriteLine("Terminal_Load run " + threadrun + " " + comPort.IsOpen);

                t11 = new System.Threading.Thread(delegate()
                {
                    threaderror = false;
                    threadrun = true;

                    Console.WriteLine("Terminal thread startup 1 " + threadrun + " " + comPort.IsOpen);

                    try
                    {
                        comPort.Write("\r");

                        // 10 sec
                        waitandsleep(10000);

                        Console.WriteLine("Terminal thread startup 2 " + threadrun + " " + comPort.IsOpen);

                        // 100 ms
                        readandsleep(100);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Terminal thread error 3 " + ex.ToString());
                        threaderror = true;
                    }
                    Console.WriteLine("Terminal thread startup 3 " + threadrun + " " + comPort.IsOpen);

                    try
                    {
                        comPort.Write("\n\n\n");

                        // 1 secs
                        readandsleep(1000);

                        comPort.Write("\r\r\r?\r");
                    }
                    catch (Exception ex)
                    {
                        if (!threaderror)
                            Console.WriteLine("Terminal thread error 4 " + ex.ToString());
                        threaderror = true;
                    }
                    Console.WriteLine("Terminal thread startup 4 " + threadrun + " " + comPort.IsOpen);

                    if (!threaderror) setButtonState(true);

                    while (threadrun && !threaderror)
                    {
                        try
                        {
                            System.Threading.Thread.Sleep(10);

                            if (inlogview)
                                continue;

                            if (comPort.BytesToRead > 0)
                            {
                                comPort_DataReceived((object)null, (SerialDataReceivedEventArgs)null);
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Terminal thread error 5 " + ex.ToString());
                            threaderror = true;
                        }
                    }

                    try
                    {
                        //comPort.Write("\rreboot\r");
                        comPort.DtrEnable = false;
                    }
                    catch { }
                    try
                    {
                        Console.WriteLine("Terminal thread close port");
                        comPort.Close();
                    }
                    catch { }

                    setButtonState(false);
                    while (threadrun)
                    {
                        //stay in thread if threaderror
                        System.Threading.Thread.Sleep(10);
                    }
                    log.Info("Terminal thread exit");
                });
                t11.IsBackground = true;
                t11.Name = "Terminal serial thread";
                t11.Start();

                // doesnt seem to work on mac
                //comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);

                BUT_ConnectAPM.Enabled = false;
                BUT_disconnect.Enabled = true;
                TXT_terminal.AppendText("Opened com port\r\n");

                inputStartPos = TXT_terminal.SelectionStart;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                TXT_terminal.AppendText("Cant open com port\r\n");
                return;
            }

            TXT_terminal.Focus();
        }

        void setButtonState(bool state)
        {
            if (state != lastbuttonstate)
            {
                lastbuttonstate = state;
                this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                {
                    this.BUTsetupshow.Enabled = state;
                    this.BUTtests.Enabled = state;
                    this.BUTradiosetup.Enabled = state;
                    this.Logs.Enabled = state;
                });
            }
        }

        private void BUTsetupshow_Click(object sender, EventArgs e)
        {
            if (comPort.IsOpen)
            {
                try
                {
                    System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                    byte[] data = encoding.GetBytes("exit\rsetup\rshow\r");
                    comPort.Write(data, 0, data.Length);
                }
                catch { }
            }
            TXT_terminal.Focus();
        }

        private void BUTradiosetup_Click(object sender, EventArgs e)
        {
            if (comPort.IsOpen)
            {
                try
                {
                    System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                    byte[] data = encoding.GetBytes("exit\rsetup\r\nradio\r");
                    comPort.Write(data, 0, data.Length);
                }
                catch { }
            }
            TXT_terminal.Focus();
        }

        private void BUTtests_Click(object sender, EventArgs e)
        {
            if (comPort.IsOpen)
            {
                try
                {
                    System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                    byte[] data = encoding.GetBytes("exit\rtest\r?\r\n");
                    comPort.Write(data, 0, data.Length);
                }
                catch { }
            }
            TXT_terminal.Focus();
        }

        private void Logs_Click(object sender, EventArgs e)
        {
            inlogview = true;

            Form Log = new MissionPlanner.Log.LogDownload();
            ThemeManager.ApplyThemeTo(Log);
            Log.ShowDialog();
            inlogview = false;
        }

        private void BUT_logbrowse_Click(object sender, EventArgs e)
        {
            Form logbrowse = new Log.LogBrowse();
            ThemeManager.ApplyThemeTo(logbrowse);
            logbrowse.Show();
        }

        private void BUT_RebootAPM_Click(object sender, EventArgs e)
        {
            if (MainV2.comPort.BaseStream.IsOpen)
                MainV2.comPort.BaseStream.Close();

            if (CMB_boardtype.Text.Contains("APM"))
                start_Terminal(false);
            if (CMB_boardtype.Text.Contains("PX4"))
                start_Terminal(true);
        }

        private void BUT_disconnect_Click(object sender, EventArgs e)
        {
            ExitThread();

            BUT_ConnectAPM.Enabled = true;
            BUT_disconnect.Enabled = false;

            TXT_terminal.AppendText("Closed\n");
        }

    }
}