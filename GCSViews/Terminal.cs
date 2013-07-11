﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArdupilotMega;
using System.IO.Ports;
using ArdupilotMega.Comms;
using ArdupilotMega.Utilities;
using System.Text.RegularExpressions;
using log4net;
using System.Reflection;
using ArdupilotMega.Controls.BackstageView;

namespace ArdupilotMega.GCSViews
{
    public partial class Terminal : MyUserControl, IActivate, IDeactivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        ICommsSerial comPort = MainV2.comPort.BaseStream;
        Object thisLock = new Object();
        public static bool threadrun = false;
        bool inlogview = false;
        List<string> cmdHistory = new List<string>();
        int history = 0;
        int inputStartPos = 0;

        public Terminal()
        {
            threadrun = false;

            InitializeComponent();
        }

        public void Activate()
        {
            MainV2.instance.MenuConnect.Enabled = false;
        }

        public void Deactivate()
        {
            MainV2.instance.MenuConnect.Enabled = true;
        }

        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (!comPort.IsOpen)
                return;

            // if btr > 0 then this shouldnt happen
            comPort.ReadTimeout = 300;

            try
            {
                lock (thisLock)
                {
                    System.Threading.Thread.Sleep(20);
                    byte[] buffer = new byte[256];
                    int a = 0;

                    while (comPort.BytesToRead > 0 && !inlogview)
                    {
                        byte indata = (byte)comPort.ReadByte();

                        buffer[a] = indata;

                        if (buffer[a] >= 0x20 && buffer[a] < 0x7f || buffer[a] == (int)'\n' || buffer[a] == (int)'\r')
                        {
                            a++;
                        }
                        if (a == (buffer.Length-1))
                            break;
                    }

                    addText(ASCIIEncoding.ASCII.GetString(buffer,0,a+1));
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); if (!threadrun) return; TXT_terminal.AppendText("Error reading com port\r\n"); }
        }

        void addText(string data)
        {
            this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
            {
                TXT_terminal.SelectionStart = TXT_terminal.Text.Length;

                data = data.Replace("U3","");
                data = data.Replace("U$", "");
                data = data.Replace(@"U""","");
                data = data.Replace("d'`F", "");
                data = data.Replace("U.", "");
                data = data.Replace("'`","");

                data = data.TrimEnd('\r'); // else added \n all by itself
                data = data.Replace("\0", "");
                TXT_terminal.AppendText(data);
                if (data.Contains("\b"))
                {
                    TXT_terminal.Text = TXT_terminal.Text.Remove(TXT_terminal.Text.IndexOf('\b'));
                    TXT_terminal.SelectionStart = TXT_terminal.Text.Length;
                }
                inputStartPos = TXT_terminal.SelectionStart;
            });

        }

        private void TXT_terminal_Click(object sender, EventArgs e)
        {
            // auto scroll
            TXT_terminal.SelectionStart = TXT_terminal.Text.Length;

            TXT_terminal.ScrollToCaret();

            TXT_terminal.Refresh();
        }

        private void TXT_terminal_KeyDown(object sender, KeyEventArgs e)
        {
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
                if (comPort.IsOpen)
                {
                    comPort.Close();
                }
            }
            catch { } // Exception System.IO.IOException: The specified port does not exist.

            System.Threading.Thread.Sleep(400);

            MainV2.comPort.giveComport = false;
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

            while ((DateTime.Now - start).TotalMilliseconds < time && !inlogview)
            {
                try
                {
                    if (comPort.BytesToRead > 0)
                    {
                        return;
                    }
                }
                catch { threadrun = false; return; }
            }
        }

        private void readandsleep(int time)
        {
             DateTime start = DateTime.Now;

             while ((DateTime.Now - start).TotalMilliseconds < time && !inlogview)
                    {
                        try
                        {
                            if (comPort.BytesToRead > 0)
                            {
                                comPort_DataReceived((object)null, (SerialDataReceivedEventArgs)null);
                            }
                        }
                        catch { threadrun = false;  return; }
                    }
        }

        private void Terminal_Load(object sender, EventArgs e)
        {

        }

        private void start_Terminal(bool px4)
        {
            try
            {
                MainV2.comPort.giveComport = true;

                comPort = MainV2.comPort.BaseStream;

                if (comPort.IsOpen)
                {
                    threadrun = false;
                  //  if (DialogResult.Cancel == CustomMessageBox.Show("The port is open\n Continue?", "Continue", MessageBoxButtons.YesNo))
                    {
                      //  return;
                    }

                    comPort.Close();

                    // allow things to cleanup
                    System.Threading.Thread.Sleep(200);
                }

                comPort.ReadBufferSize = 1024 * 1024;

                comPort.PortName = MainV2.comPortName;

                // test moving baud rate line

                log.Info("About to open " + comPort.PortName);

                comPort.Open();

                comPort.BaudRate = int.Parse(MainV2._connectionControl.CMB_baudrate.Text);

                if (px4)
                {
                    // check if we are a mavlink stream
                    byte[] buffer = MainV2.comPort.readPacket();

                    if (buffer.Length > 0)
                    {
                        log.Info("got packet - sending reboot via mavlink");
                        MainV2.comPort.giveComport = true;
                        MainV2.comPort.doReboot();
                        try
                        {
                            comPort.Close();
                        }
                        catch { }

                    }
                    else
                    {
                        log.Info("no packet - sending reboot via console");
                        MainV2.comPort.giveComport = true;
                        MainV2.comPort.BaseStream.Write("exit\rreboot\r");
                        try
                        {
                            comPort.Close();
                        }
                        catch { }

                    }

                    MainV2.comPort.giveComport = true;

                    // wait 7 seconds for px4 reboot
                    log.Info("waiting for px4 reboot");
                    DateTime deadline = DateTime.Now.AddSeconds(8);
                    while (DateTime.Now < deadline)
                    {
                        System.Threading.Thread.Sleep(100);
                        Application.DoEvents();
                    }
                   
                    int a = 0;
                   // while (a < 5)
                    {
                        try
                        {
                            comPort.Open();
                        }
                        catch { }
                        System.Threading.Thread.Sleep(200);
                        a++;
                    }
                     
                }
                else
                {
                    comPort.toggleDTR();
                }

                comPort.DiscardInBuffer();

                Console.WriteLine("Terminal_Load");

                System.Threading.Thread t11 = new System.Threading.Thread(delegate()
                {
                    threadrun = true;

                    Console.WriteLine("Terminal thread start");

                    // 10 sec
                        waitandsleep(10000);

                    Console.WriteLine("Terminal thread 1");

                    // 100 ms
                        readandsleep(100);

                    Console.WriteLine("Terminal thread 2");

                    try
                    {
                        if (!inlogview)
                            comPort.Write("\n\n\n");

                        // 1 secs
                        if (!inlogview)
                            readandsleep(1000);

                        if (!inlogview)
                            comPort.Write("\r\r\r?\r");
                    }
                    catch (Exception ex) { Console.WriteLine("Terminal thread 3 " + ex.ToString()); threadrun = false; return; }

                    Console.WriteLine("Terminal thread 3");

                    while (threadrun)
                    {
                        try
                        {
                            System.Threading.Thread.Sleep(10);
                            if (inlogview)
                                continue;
                            if (!comPort.IsOpen)
                            {
                                Console.WriteLine("Comport Closed");
                                break;
                            }
                            if (comPort.BytesToRead > 0)
                            {
                                comPort_DataReceived((object)null, (SerialDataReceivedEventArgs)null);
                            }
                        }
                        catch (Exception ex) { Console.WriteLine("Terminal thread 4 " + ex.ToString()); }
                    }

                    threadrun = false;
                    try
                    {
                        comPort.DtrEnable = false;
                    }
                    catch { }
                    try
                    {
                        Console.WriteLine("term thread close");
                        comPort.Close();
                    }
                    catch { }

                    Console.WriteLine("Comport thread close");
                });
                t11.IsBackground = true;
                t11.Name = "Terminal serial thread";
                t11.Start();

                // doesnt seem to work on mac
                //comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);

                TXT_terminal.AppendText("Opened com port\r\n");
                inputStartPos = TXT_terminal.SelectionStart;
            }
            catch (Exception ex) { log.Error(ex); TXT_terminal.AppendText("Cant open serial port\r\n"); return; }

            TXT_terminal.Focus();
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
            System.Threading.Thread.Sleep(300);
            Form Log = new ArdupilotMega.Log.Log();
            ThemeManager.ApplyThemeTo(Log);
            Log.ShowDialog();
            inlogview = false;
            try
            {
                comPort.Write("\n\n\n");
            }
            catch { }
        }

        private void BUT_logbrowse_Click(object sender, EventArgs e)
        {
            Form logbrowse = new Log.LogBrowse();
            ThemeManager.ApplyThemeTo(logbrowse);
            logbrowse.Show();
        }

        private void BUT_RebootAPM_Click(object sender, EventArgs e)
        {
            start_Terminal(false);
        }

        private void BUT_ConnectPX4_Click(object sender, EventArgs e)
        {
            start_Terminal(true);
        }

        private void BUT_disconnect_Click(object sender, EventArgs e)
        {
            try
            {
                comPort.Close();
                TXT_terminal.AppendText("Closed\n");
            }
            catch { }
        }
    }
}