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
using Renci.SshNet;
using SerialPort = MissionPlanner.Comms.SerialPort;
using Renci.SshNet.Common;
using System.Drawing;
using System.Text.RegularExpressions;
using System.IO;

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
        private SshClient client;
        public static bool SSHTerminal = false;
        private ShellStream shellStream;
        private int SelectStartIndex = 0;
        private bool inlogview;
        private int inputStartPos;
        private bool Bold = false;
        private bool Underline = false;
        private Color DefaultColor = Color.White;
        private Color DefaultBackgroundColor;
        private Color SetBackground;
        private bool Connected = false;
        private bool ArrowMovement = false;
        DateTime lastsend = DateTime.MinValue;
        private int ScreenWidth = 80;
        private int ScreenHeight = 24;
        private int CursorPosition;
        private int LineCounter = 24;
        private bool ByobuMode = false;
        private bool Scrolling = false;

        public Terminal()
        {
            threadrun = false;
            SSHTerminal = false;
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
                        var indata = (byte)comPort.ReadByte();

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
            BeginInvoke((MethodInvoker)delegate
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
               data = data.Replace((char)0x1b + "[K", ""); // remove control code

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

               //add back typed text
               if (!SSHTerminal) TXT_terminal.AppendText(currenttypedtext);

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
                        if (SSHTerminal) { shellStream.Write("\u001b[A"); ArrowMovement = true; }
                        e.Handled = true;
                        break;
                    case Keys.Down:
                        if (history < cmdHistory.Count - 1)
                        {
                            TXT_terminal.Select(inputStartPos, TXT_terminal.Text.Length - inputStartPos);
                            TXT_terminal.SelectedText = "";
                            TXT_terminal.AppendText(cmdHistory[++history]);
                        }
                        if (SSHTerminal) { shellStream.Write("\u001b[B"); ArrowMovement = true; }
                        e.Handled = true;
                        break;
                    case Keys.Right:
                        if (SSHTerminal) shellStream.Write("\u001b[C");
                        break;
                    case Keys.Left:
                        if (SSHTerminal) shellStream.Write("\u001b[D");
                        break;
                    case Keys.Back:
                        if (TXT_terminal.SelectionStart <= inputStartPos)
                            e.Handled = true;
                        break;
                    case Keys.F1:
                        if (SSHTerminal) shellStream.Write("\u001b[11~");
                        break;
                    case Keys.F2:
                        if (SSHTerminal) shellStream.Write("\u001b[12~");
                        break;
                    case Keys.F3:
                        if (SSHTerminal) shellStream.Write("\u001b[13~");
                        break;
                    case Keys.F4:
                        if (SSHTerminal) shellStream.Write("\u001b[14~");
                        break;
                    case Keys.F5:
                        if (SSHTerminal) shellStream.Write("\u001b[15~");
                        break;
                    case Keys.F6:
                        if (SSHTerminal) shellStream.Write("\u001b[17~");
                        break;
                    case Keys.F7:
                        if (SSHTerminal) shellStream.Write("\u001b[18~");
                        break;
                    case Keys.F8:
                        if (SSHTerminal) shellStream.Write("\u001b[19~");
                        break;
                    case Keys.F9:
                        if (SSHTerminal) shellStream.Write("\u001b[20~");
                        break;
                    case Keys.F10:
                        if (SSHTerminal) shellStream.Write("\u001b[21~");
                        break;
                    case Keys.F11:
                        if (SSHTerminal) shellStream.Write("\u001b[23~");
                        break;
                    case Keys.F12:
                        if (SSHTerminal) shellStream.Write("\u001b[24~");
                        break;
                    case Keys.PageUp:
                        if (SSHTerminal) shellStream.WriteLine("\u001b[5~");
                        break;
                    case Keys.PageDown:
                        if (SSHTerminal) shellStream.WriteLine("\u001b[6~");
                        break;
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            //If tab key is pressed send it through shellstream
            if (keyData == Keys.Tab)
            {
                shellStream.Write("\t");
            }
            return base.ProcessCmdKey(ref msg, keyData);
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

        private void IPAddressBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                BUT_RebootAPM_Click(this, new EventArgs());
            }
        }

        private void TXT_terminal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\u0003')
            {
                DefaultColor = Color.White;
            }

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
            if (SSHTerminal)
            {
                try
                {
                    shellStream.Write(e.KeyChar.ToString());
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Command Sending Error Occured {0} ", exp);
                    TXT_terminal.AppendText("Command Failed. Reason for failure was: " + exp.Message + " \r\n");
                }
            }
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
                    comPort.ReadBufferSize = 1024 * 1024 * 4;
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

                comPort.ReadBufferSize = 1024 * 1024 * 4;

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
                            mine.doReboot(false, false);
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

        private void start_SSHterminal()
        {
            try
            {
                int Port = 22;
                TXT_terminal.Clear();
                TXT_terminal.SelectedText = "Connecting.... \r\n";
                //Get user needed input to start an SSH connection then connect using SSH.net
                string promptvalue = Prompt.ShowDialog("Username", "Password", "Please enter username and password to connect");
                string IpAddress = IPAddressBox.Text;
                if (IPAddressBox.Text.Contains(":"))
                {
                    Port = Convert.ToInt32(IPAddressBox.Text.Substring(IPAddressBox.Text.IndexOf(":") + 1));
                    IpAddress = IPAddressBox.Text.Substring(0, IPAddressBox.Text.IndexOf(":"));
                }
                string Username = promptvalue.Substring(0, promptvalue.IndexOf(":"));
                string Password = promptvalue.Substring(promptvalue.IndexOf(":") + 1);
                client = new SshClient(IpAddress, Port, Username, Password);
                client.Connect();
                TXT_terminal.AppendText("SSH Connected \r\n");
                //Setting required variables for ssh terminal
                SelectStartIndex = inputStartPos = TXT_terminal.Text.Length;
                DefaultBackgroundColor = SetBackground = TXT_terminal.BackColor;
                IPAddressBox.Enabled = CMB_boardtype.Enabled = false;
                ChangeConnectStatus(true);
                //Terminal type is set to xterm
                shellStream = client.CreateShellStream("xterm", 0, 0, 0, 0, 1000);
                LineCounter = ScreenHeight - 1;
                startSSHthread();
            }
            catch (SshAuthenticationException)
            {
                Console.WriteLine("Password or username was incorrect");
                TXT_terminal.AppendText("Connected Failed, due to incorrect Username or Password \r\n");
            }
            catch (System.Net.Sockets.SocketException se)
            {
                Console.WriteLine("Exception {0} occurred", se.Message);
                TXT_terminal.AppendText("Connected Failed. Reason for failure was:  " + se.Message + "\r\n");
            }
            catch (Exception exp)
            {
                Console.WriteLine("Connection Error Occured {0} ", exp);
                TXT_terminal.AppendText("Connection Failed. Reason for failure was: " + exp.Message + " \r\n");
            }
        }

        private void startSSHthread()
        {
            threadrun = true;
            var thread = new Thread(delegate ()
            {
                Thread.Sleep(1000);
                while (threadrun)
                {
                    Thread.Sleep(10);
                    if (!threadrun)
                        break;
                    //If connection lost then break
                    if (!SSHTerminal)
                    {
                        Console.WriteLine("The SSH Connection has been lost");
                        break;
                    }
                    //If there is text to read in the do so else loop
                    if (shellStream.Length > 0)
                    {
                        SSH_StreamReader();
                    }
                }
                threadrun = false;
            });
            thread.IsBackground = true;
            thread.Name = "Read Thread";
            thread.Start();

            TXT_terminal.Focus();
        }

        private void SSH_StreamReader()
        {
            //While there are characters to read in the stream
            //read them in and add them to the terminal window
            while (shellStream.Length > 0)
            {
                var character = shellStream.Read();
                if (character == "\a" || character == "") return;
                SSH_AddText(character);
            }

        }

        private void SSH_AddText(string data)
        {
            BeginInvoke((MethodInvoker)delegate
            {

                Clipboard.Clear();
                if (inputStartPos > TXT_terminal.Text.Length)
                    inputStartPos = TXT_terminal.Text.Length - 1;

                // gather current typed data
                string currenttypedtext = TXT_terminal.Text.Substring(inputStartPos,
                    TXT_terminal.Text.Length - inputStartPos);

                // remove typed data
                TXT_terminal.Select(inputStartPos, 1);
                TXT_terminal.SelectedText = String.Empty;

                if (SelectStartIndex < inputStartPos)
                {
                    TXT_terminal.SelectionStart = SelectStartIndex;
                }
                //Acknowledging byobu mode has been entered
                //Byobu is currently disables
                if (Regex.IsMatch(data, @"- byobu\a"))
                {
                    //To Disable Byobu
                    ByobuMode = true;
                    shellStream.WriteLine("exit");
                    data = "";
                }
                //Acknowledging byobu mode
                if (data.Contains("[exited]")) { ByobuMode = false; TXT_terminal.Clear(); data = data.Insert(data.IndexOf("[exited]") + 8, " Byobu is unsupported by this terminal "); }

                //Handle directional arrows
                foreach (Match m in Regex.Matches(data, @".*ESC.*\[?[ABCD]\a"))
                {
                    var index = data.IndexOf(m.Value);
                    data = data.Remove(index, m.Length);
                    Scrolling = true;
                }
                //Handle page up or page down
                foreach (Match m in Regex.Matches(data, @".*ESC.*\[?[56~]"))
                {
                    var index = data.IndexOf(m.Value);
                    data = data.Remove(index, m.Length);
                    Scrolling = true;
                    TXT_terminal.Clear();
                }
                //If page needs to scroll then do so
                if (Scrolling) { data = PageUpDown(data); }
                //Stop byobu for outputting data
                if (ByobuMode) { data = ""; }
                string pattern = @"\e[\(\[\]](\d+;)*\??(\d+)?[ABCDEFGHQJKSPTLflXthmbnirpdsu]";
                foreach (Match match in Regex.Matches(data, pattern))
                {
                    var index = data.IndexOf(match.Value);
                    if (index != 0) //There are words to add before next ANSI character
                    {
                        var subString = data.Substring(0, data.IndexOf(match.Value));
                        if (subString != "") data = data.Remove(0, subString.Length);
                        //Handle text wrapping
                        if (subString.Contains("$")) { subString = WrapTextHandler(subString); }
                        //If newline or return character move down line instead of printing newline or return character
                        if (subString.Contains("\r\n")) subString = TrimText(subString, "\r\n");
                        if (subString.Contains("\r")) subString = TrimText(subString, "\r");
                        //Format Data to remove any unwanted characters
                        subString = FormatData(subString);
                        //Handle text wrapping
                        AppendText(subString);
                        inputStartPos = TXT_terminal.TextLength;
                    }
                    ResolveCommand(match.Value);
                    data = data.Remove(0, match.Length);
                }
                //If newline or return character move down line instead of printing newline or return character
                if (data.Contains("\r\n")) data = TrimText(data, "\r\n");
                if (data.Contains("\r")) data = TrimText(data, "\r");
                //Format Data to remove any unwanted characters
                data = FormatData(data);
                AppendText(data);
                inputStartPos = TXT_terminal.TextLength;
                SelectStartIndex = TXT_terminal.SelectionStart;
                ArrowMovement = false;
                TXT_terminal.Focus();
            });
        }

        private string PageUpDown(string data)
        {
            if (data == "") return data;
            if (Regex.IsMatch(data, @"\e\[H\eM")) //If page up then we need to reverse order
            {
                LineCounter = ScreenHeight;
                Match match = (Regex.Match(data, @"\e\[H\eM"));
                data = data.Replace(match.Value, "\u001b[Q");
            }
            if (Regex.IsMatch(data, @"\e\[7m\(END\)\e\[27m")) //Print only one end statement
            {
                Match match = (Regex.Match(data, @"\e\[7m\(END\)\e\[27m"));
                int RemoveIndex = match.Length + match.Index;
                data = data.Remove(RemoveIndex, data.Length - RemoveIndex);
            }
            Scrolling = false;
            return data;
        }

        private void ScreenScroll()
        {
            int temp = 0; int line = 0; int index = 0; int length = 0;
            //Remove first line
            TXT_terminal.Select(0, TXT_terminal.GetFirstCharIndexFromLine(1));
            TXT_terminal.SelectedText = "";
            //Find last line and set selectionstart to it then clear rest of screen
            for (int i = 0; i < TXT_terminal.Lines.Count(); i++)
            {
                temp = TXT_terminal.GetFirstCharIndexFromLine(i);
                if (temp != -1) { index = temp; line = i; }
            }
            if (TXT_terminal.Lines.Count() != 0) { length = TXT_terminal.Lines[line].Length; }
            TXT_terminal.SelectionStart = index + length;
            Clear("\u001b[J");
        }

        private string TrimText(string data, string trim)
        {
            int count = 0;
            if (trim == "\r\n") { count = data.Count(f => f == '\n'); }
            else if (trim == "\r") { count = data.Count(f => f == '\r'); }
            for (int i = 0; i < count; i++)
            {
                if (data == "" || data.Length < trim.Length) break;
                var check = data.Substring(0, trim.Length);
                //If start of next substring isn't newline or return character append
                //text up until next newline or return character
                if (check != trim)
                {
                    int newlineIndex = data.IndexOf(trim);
                    if (newlineIndex == -1) newlineIndex = data.Length;
                    var subString = data.Substring(0, newlineIndex);
                    subString = FormatData(subString);
                    subString = subString.TrimEnd('\r', '\n');
                    AppendText(subString);
                    data = data.Remove(0, newlineIndex);
                }
                int line = TXT_terminal.GetLineFromCharIndex(TXT_terminal.GetFirstCharIndexOfCurrentLine());
                //If last line finishes with a return line chracter move to front of line
                //else move down a line
                if (trim == "\r" && count == 1 && line == ScreenHeight - 1)
                {
                    data = data.TrimEnd('\r');
                    CursorToColumn(0);
                }
                else
                {
                    CursorDown("\u001b[B");
                }
                if (data != "") data = data.Remove(data.IndexOf(trim), trim.Length);
            }
            return data;
        }

        //Handle text wrapping in Nano
        private string WrapTextHandler(string data)
        {
            if (data.StartsWith("\r$"))
            {
                CursorToColumn(0);
                data = data.Remove(data.IndexOf('\r'), 1);
            }
            if (data.EndsWith("$"))
            {
                CursorToColumn(0);
                if (data.Contains("\r")) data = data.Remove(data.IndexOf('\r'), 1);
            }
            return data;
        }

        private string FormatData(string data)
        {
            //Save or restore cursor 
            foreach (Match match in Regex.Matches(data, @"\e[78]"))
            {
                if (match.Value.Contains("7")) { SaveCursor(); }
                if (match.Value.Contains("8")) { RestoreCursor(); }
                var index = data.IndexOf(match.Value);
                data = data.Remove(index, match.Length);
            }
            //Remove operating system insturctions
            foreach (Match match in Regex.Matches(data, @"\u001b]\d+;"))
            {
                int index = data.IndexOf('\u001b');
                int endIndex = data.IndexOf("\a");
                if (endIndex < index) { data = data.Remove(endIndex, 1); endIndex = data.IndexOf("\a"); }
                data = data.Remove(match.Index, endIndex - match.Index + 1);
            }
            //Remove unwanted ANSI characters
            foreach (Match match in Regex.Matches(data, @"\e\[?\]?\=?\>?[0-9]*[M@]?"))
            {
                var index = data.IndexOf(match.Value);
                data = data.Remove(index, match.Length);
            }
            //Remove ESC sequences that were not correctly handled
            foreach (Match match in Regex.Matches(data, @"^ESC"))
            {
                var index = data.IndexOf(match.Value);
                data = data.Remove(index, match.Length);
            }
            //Remove unneeded repeated sequences
            if (Regex.IsMatch(data, @"[A-Za-z0-9]\.*\r[A-Za-z0-09]"))
            {
                string txt = "";
                int SelectIndex = TXT_terminal.SelectionStart;
                int firstChar = TXT_terminal.GetFirstCharIndexOfCurrentLine();
                if (firstChar < SelectIndex)
                {
                    TXT_terminal.Select(firstChar, SelectIndex - firstChar);
                    txt = TXT_terminal.SelectedText;
                }
                var index = data.IndexOf('\r');
                ClearLine("\u001b[2K");
                data = data.Remove(index, data.Length - index);
                TXT_terminal.SelectedText = txt + data;
                TXT_terminal.SelectionStart = SelectIndex + 1;
                data = "";
            }
            //Remove unneeded alerts from data
            foreach (Match match in Regex.Matches(data, @".*\a.*"))
            {
                var index = data.IndexOf(match.Value);
                data = data.Remove(index, match.Length);
            }
            return data;
        }

        private string BackspaceHandler(string data, string backSpaceKey)
        {
            int count = 0;
            count = data.Count(f => f == '\b');
            for (int i = 0; i <= count; i++)
            {
                if (data == "") break;
                var check = data.Substring(0, backSpaceKey.Length);
                int backSpaceIndex = data.IndexOf(backSpaceKey);
                //Check if next substring is a backspace or more text to append
                if (check != backSpaceKey)
                {
                    if (backSpaceIndex == -1) backSpaceIndex = data.Length;
                    string Substring = data.Substring(0, backSpaceIndex);
                    AppendText(Substring);
                    if (data != "") data = data.Remove(0, backSpaceIndex);
                }
                if (backSpaceIndex == -1) backSpaceIndex = data.Length;
                //If double backspace then need to clearline, if not just move cursor back
                if (backSpaceKey == "\b \b")
                {
                    CursorBack("\u001b[D");
                    ClearLine("\u001b[K");
                }
                else if (backSpaceKey == "\b")
                {
                    TXT_terminal.SelectionStart = TXT_terminal.SelectionStart - 1;
                }
                if (data != "") data = data.Remove(data.IndexOf(backSpaceKey), backSpaceKey.Length);
            }
            return data;
        }

        private void AppendText(string text)
        {
            int lastIndex = 0;
            int firstchar = 0;
            if (text == "\n" || text == "") return;
            if (text.Contains("\b \b")) text = BackspaceHandler(text, "\b \b");
            if (text.Contains("\b")) text = BackspaceHandler(text, "\b");
            var index = TXT_terminal.SelectionStart;
            var line = TXT_terminal.GetLineFromCharIndex(index);
            var length = 0;
            if (TXT_terminal.Lines.Count() > line && TXT_terminal.Lines[line] != "")
            {
                lastIndex = TXT_terminal.Lines[line].LastIndexOf(TXT_terminal.Lines[line].Last());
                firstchar = TXT_terminal.GetFirstCharIndexFromLine(line);
                length = lastIndex + firstchar;
            }
            //Check if we need to scroll one up
            if (line == ScreenHeight)
            {
                ScreenScroll();
            }
            //Set selection length based on length of line
            if (index <= length)
            {
                if (index + text.Length > length)
                    TXT_terminal.SelectionLength = (length - index) + 1;
                else
                    TXT_terminal.SelectionLength = text.Length;
            }
            //Make sure not to remove newline character at the end of a line
            if (TXT_terminal.SelectedText == "\n") { TXT_terminal.SelectedText = text + Environment.NewLine; }
            else { TXT_terminal.SelectedText = text; }
            //Apply Character Attributes
            TXT_terminal.Select(index, text.Length);
            {
                TXT_terminal.SelectionBackColor = SetBackground;
                TXT_terminal.SelectionColor = DefaultColor;
                if (Bold && Underline) TXT_terminal.SelectionFont = new Font(TXT_terminal.Font, FontStyle.Bold | FontStyle.Underline);
                else if (Bold) TXT_terminal.SelectionFont = new Font(TXT_terminal.Font, FontStyle.Bold);
                else if (Underline) TXT_terminal.SelectionFont = new Font(TXT_terminal.Font, FontStyle.Underline);
                else TXT_terminal.SelectionFont = new Font(TXT_terminal.Font, FontStyle.Regular);
            }
            index = TXT_terminal.SelectionStart;
            var position = index + text.Length;
            //Move cursor to end
            TXT_terminal.Select(position, 0);
        }

        private void startreadthread()
        {
            Console.WriteLine("Terminal_Load run " + threadrun + " " + comPort.IsOpen);

            Connected = true;
            BUT_ConnectAPM.Text = "Disconnect";

            var t11 = new Thread(delegate ()
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
                                ((MAVLinkSerialPort)comPort).timeout = 50;
                            }
                            else
                            {
                                // 5 hz
                                ((MAVLinkSerialPort)comPort).timeout = 200;
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

            Invoke((MethodInvoker)delegate
            {
                if (connected && BUT_ConnectAPM.Text == "Connect")
                {
                    Connected = true;
                    BUT_ConnectAPM.Text = "Disconnect";
                }
                else if (!connected && BUT_ConnectAPM.Text == "Disconnect")
                {
                    Connected = false;
                    BUT_ConnectAPM.Text = "Connect";
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

        //Connect and Disconnect buttons have been condensed to one button
        private void BUT_RebootAPM_Click(object sender, EventArgs e)
        {
            if (Connected)
            {
                try
                {
                    if (SSHTerminal)
                    {
                        client.Disconnect();
                        client.Dispose();
                        CMB_boardtype.Enabled = true;
                        IPAddressBox.Enabled = true;
                        SSHTerminal = false;
                        ArrowMovement = false;
                        threadrun = false;
                    }
                    else
                        comPort.Write("reboot\n");
                }
                catch
                {
                }
                comPort.Close();
                TXT_terminal.AppendText("\nDisconnected\n");
                Connected = false;
                ChangeConnectStatus(Connected);
                return;
            }
            if (comPort.IsOpen)
            {
                Connected = true;
                BUT_ConnectAPM.Text = "Disconnect";
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
            if (CMB_boardtype.Text.Contains("SSH"))
            { start_SSHterminal(); SSHTerminal = true; }
            TXT_terminal.Focus();


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
                    ((MAVLinkSerialPort)comPort).timeout = 50;

                    comPort.Open();

                    startreadthread();
                }
            }
            catch
            {
            }
        }

        private void CMB_boardtype_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CMB_boardtype.Text.Contains("SSH"))
            {
                IPAddressBox.Enabled = true;
            }
            else
            {
                IPAddressBox.Enabled = false;
                IPAddressBox.Text = "IP Address:Socket";
            }
        }

        #region ANSI Escape Sequence Resolvers
        private void ResolveCommand(string command)
        {
            if (command.EndsWith("(B")) return;
            if (command.EndsWith("A")) CursorUp(command);
            if (command.EndsWith("B")) CursorDown(command);
            if (command.EndsWith("C")) CursorForward(command);
            if (command.EndsWith("D")) CursorBack(command);
            if (command.EndsWith("d")) AbsoluteLinePosition(command);
            if (command.EndsWith("F")) CursorToLine(command);
            if (command.EndsWith("G")) CursorToColumn(command);
            if (command.EndsWith("H")) MoveCursor(command);
            if (command.EndsWith("K")) ClearLine(command);
            if (command.EndsWith("J")) Clear(command);
            if (command.EndsWith("L")) NewLine();
            if (command.EndsWith("n")) ReportCursorPosition(command);
            if (command.EndsWith("m")) EditDisplay(command);
            if (command.EndsWith("r")) SetScrollSize(command);
            if (command.EndsWith("P")) DeleteChars(command);
            if (command.EndsWith("Q")) ReverseOrder(command);
            if (command.EndsWith("S")) ScrollUp(command);
            if (command.EndsWith("T")) ScrollDown(command);
            if (command.EndsWith("X")) RemoveChars(command);
        }

        private void CursorUp(string command)
        {
            int line = 1;
            string input = (command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2));
            if (input != "") { line = Convert.ToInt32(input); }
            int index = TXT_terminal.SelectionStart;
            int firstIndex = TXT_terminal.GetFirstCharIndexOfCurrentLine();
            int column = index - firstIndex;
            int currentLine = TXT_terminal.GetLineFromCharIndex(index);
            CursorToLine(currentLine - line);
            //Move to previous column if arrow key was pressed
            if (ArrowMovement)
            {
                CursorToColumn(column);
                ArrowMovement = false;
            }
        }

        private void CursorDown(string command)
        {
            int line = 1;
            string input = (command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2));
            if (input != "") { line = Convert.ToInt32(input); }
            int index = TXT_terminal.SelectionStart;
            int currentLine = TXT_terminal.GetLineFromCharIndex(index);
            CursorToLine(currentLine + line);
        }

        private void CursorForward(string command)
        {
            int index = TXT_terminal.SelectionStart;
            int line = TXT_terminal.GetLineFromCharIndex(index);
            int firstChar = TXT_terminal.GetFirstCharIndexFromLine(line);
            int col = index - firstChar;

            //Move curosr forward by given number else move by one
            var pattern = @"\e\[\d+C";
            if (Regex.IsMatch(command, pattern))
            {
                var length = Convert.ToInt32(command.Substring(command.IndexOf('[') + 1, command.IndexOf('C') - 2));
                CursorToColumn(col + length);
            }
            else
            {
                CursorToColumn(col + 1);
            }
        }

        private void CursorBack(string command)
        {
            int index = TXT_terminal.SelectionStart;
            int line = TXT_terminal.GetLineFromCharIndex(index);
            int firstChar = TXT_terminal.GetFirstCharIndexFromLine(line);
            int col = index - firstChar;

            var pattern = @"\e\[\d+D";
            //Cursor back to position else cursor back one
            if (Regex.IsMatch(command, pattern))
            {
                var length = Convert.ToInt32(command.Substring(command.IndexOf('[') + 1, command.IndexOf('D') - 2));
                CursorToColumn(col - length);
            }
            else
            {
                CursorToColumn(col - 1);

            }
        }

        private void AbsoluteLinePosition(string command)
        {
            int index = TXT_terminal.SelectionStart;
            int firstIndex = TXT_terminal.GetFirstCharIndexOfCurrentLine();
            int column = index - firstIndex;
            int line = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2))) - 1;
            CursorToLine(line);
            //Move to previous column if arrow key was pressed
            if (ArrowMovement)
            {
                int firstNewIndex = TXT_terminal.GetFirstCharIndexOfCurrentLine();
                if (column != 0 && TXT_terminal.Lines[line].Length >= column)
                {
                    CursorToColumn(column);
                }
                ArrowMovement = false;
            }
        }

        private void CursorToLine(string command)
        {
            int line = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2))) - 1;
            CursorToLine(line);
        }

        private void CursorToLine(int line)
        {
            //If line is bigger or equal to screen height, then we need to scroll up
            if (line >= ScreenHeight) { ScreenScroll(); if (line > ScreenHeight) { line = ScreenHeight; } else { line--; } }
            int index = TXT_terminal.SelectionStart;
            int currentLine = TXT_terminal.GetLineFromCharIndex(index);
            int rowIndex = TXT_terminal.GetFirstCharIndexFromLine(line);
            //If line exists move to it else loop until we reach that line
            if (rowIndex != -1)
            {
                TXT_terminal.Select(rowIndex, 1);
            }
            else
            {
                for (int i = currentLine; i <= line; i++)
                {
                    int charIndex = TXT_terminal.GetFirstCharIndexFromLine(i);
                    if (charIndex == -1)
                    {
                        int SelectionIndex = TXT_terminal.GetFirstCharIndexFromLine(i - 1);
                        if (i == line && SelectionIndex != -1)
                        {
                            TXT_terminal.Select(SelectionIndex, 0);
                            TXT_terminal.AppendText("\r");
                            TXT_terminal.Select(TXT_terminal.GetFirstCharIndexFromLine(i), 1);
                        }
                        else
                        {
                            for (int k = i; k <= line; k++)
                            {
                                TXT_terminal.AppendText("\r");
                            }
                            TXT_terminal.SelectionStart = TXT_terminal.TextLength;
                        }
                    }
                }
            }

        }

        private void CursorToColumn(string command)
        {
            int column = Convert.ToInt32(command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2)) - 1;
            CursorToColumn(column);
        }

        private void CursorToColumn(int column)
        {
            int index = TXT_terminal.SelectionStart;
            int line = TXT_terminal.GetLineFromCharIndex(index);
            int firstChar = TXT_terminal.GetFirstCharIndexFromLine(line);
            if (firstChar == -1) { firstChar = index; }
            //If column is bigger than max set to max column
            if (column > ScreenWidth) column = ScreenWidth;
            //Loop until find coloumn
            for (int i = 0; i < column; i++)
            {
                TXT_terminal.Select(firstChar, 1);
                string text = TXT_terminal.SelectedText;
                if (text == "") TXT_terminal.SelectedText = " ";
                if (text == "\n") { TXT_terminal.SelectedText = " \n"; }
                firstChar++;
            }
            TXT_terminal.Select(firstChar, 1);

        }

        private void MoveCursor(string command)
        {

            int row = 1;
            int column = 1;
            if (command.Contains(';'))
            {
                column = Convert.ToInt32(command.Substring(command.IndexOf(';') + 1, command.IndexOf('H') - 1 - command.IndexOf(';'))) - 1;
                row = Convert.ToInt32(command.Substring(2, command.IndexOf(';') - 2)) - 1;
                int line = TXT_terminal.GetLineFromCharIndex(TXT_terminal.SelectionStart);
                if (row <= line) //Move up or stay there
                {
                    int rowIndex = TXT_terminal.GetFirstCharIndexFromLine(row);
                    TXT_terminal.Select(rowIndex, 0);
                }
                else //Cursor to line
                {
                    CursorToLine(row);
                }
                //Cursor to column
                CursorToColumn(column);
            }
            else
            {
                //Select home
                TXT_terminal.Select(0, 0);
            }
        }

        private void ClearLine(string command)
        {
            int index = TXT_terminal.SelectionStart;
            int line = TXT_terminal.GetLineFromCharIndex(index);
            int charIndex = TXT_terminal.GetFirstCharIndexOfCurrentLine();
            if (TXT_terminal.Lines.Count() == 0 || TXT_terminal.Lines.Count() <= line) { return; };
            var distance = index - charIndex;

            var length = TXT_terminal.Lines[line].Length;
            var RemoveLength = length - distance;
            //Clear whole line
            if (command.Contains("2"))
            {
                TXT_terminal.Select(charIndex, length);
                TXT_terminal.SelectedText = "";
            }
            //Clear line left of cursor
            else if (command.Contains("1"))
            {
                TXT_terminal.Select(charIndex, index - charIndex);
                AppendText(new string(' ', index - charIndex));
                TXT_terminal.Select(index, 0);
            }
            //Clear line right of cursor
            else
            {
                TXT_terminal.Select(index, RemoveLength);
                TXT_terminal.SelectedText = "";
            }
        }

        private void Clear(string command)
        {

            var pattern = @"\e\[\d?J";
            if (Regex.IsMatch(command, pattern))
            {
                //Clear whole screen
                if (command.Contains("2")) { TXT_terminal.Clear(); inputStartPos = 0; return; }
                int index = TXT_terminal.SelectionStart;
                //Clear screen from cursor up
                if (command.Contains("1"))
                {
                    int firstIndex = TXT_terminal.GetFirstCharIndexFromLine(0);
                    TXT_terminal.Select(firstIndex, index - firstIndex);
                    TXT_terminal.SelectedText = new string(' ', index - firstIndex); ;
                }
                //Clear screen from cursor down
                else
                {
                    TXT_terminal.Select(index, TXT_terminal.TextLength);
                    TXT_terminal.SelectedText = "";
                    inputStartPos = TXT_terminal.TextLength;
                }
            }
        }

        private void NewLine()
        {

            int index = TXT_terminal.SelectionStart;
            int line = TXT_terminal.GetLineFromCharIndex(index);
            TXT_terminal.SelectionStart = index++;
        }

        //Calulate Cursor Position and report back using shellstream, used for resize
        private void ReportCursorPosition(string command)
        {
            int value = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2)));
            string report = "\u001b[";
            if (value == 6)
            {
                int index = TXT_terminal.SelectionStart;
                int currentLine = TXT_terminal.GetLineFromCharIndex(index);
                report += currentLine.ToString();
                report += ";";
                index = TXT_terminal.SelectionStart;
                int firstIndex = TXT_terminal.GetFirstCharIndexFromLine(currentLine);
                int column = index - firstIndex;
                report += column.ToString();
                report += "R";
                shellStream.WriteLine(report);
            }
        }

        //Set character attributes based on command
        private void EditDisplay(string command)
        {
            var pattern = @"\e\[(\d+;)*\d+m";
            if (Regex.IsMatch(command, pattern))
            {

                string[] Settings = new string[3];
                Match m = Regex.Match(command, pattern);
                int count = command.Count(f => f == ';');
                if (count == 2)
                {
                    Settings[0] = (command.Substring(command.IndexOf('[') + 1, command.IndexOf(';') - 1 - command.IndexOf('[')));
                    Settings[1] = (command.Substring(command.IndexOf(';') + 1, command.LastIndexOf(';') - 1 - command.IndexOf(';')));
                    Settings[2] = (command.Substring(command.LastIndexOf(';') + 1, command.IndexOf('m') - 1 - command.LastIndexOf(';')));
                }
                else if (count == 1)
                {
                    Settings[0] = (command.Substring(command.IndexOf('[') + 1, command.IndexOf(';') - 1 - command.IndexOf('[')));
                    Settings[1] = (command.Substring(command.IndexOf(';') + 1, command.IndexOf('m') - 1 - command.IndexOf(';')));
                }
                else Settings[0] = (command.Substring(command.IndexOf('[') + 1, command.IndexOf('m') - 1 - command.IndexOf('[')));

                foreach (string setting in Settings)
                {
                    if (setting == "0") { Bold = false; Underline = false; DefaultColor = Color.White; SetBackground = DefaultBackgroundColor; } //Reset
                    else if (setting == "1") Bold = true;
                    else if (setting == "4") Underline = true;
                    else if (setting == "7") { Color Temp = DefaultColor; DefaultColor = SetBackground; SetBackground = Temp; }
                    else if (setting == "27") { Color Temp = DefaultColor; DefaultColor = SetBackground; SetBackground = Temp; }
                    else if (setting == "30") DefaultColor = Color.Black;
                    else if (setting == "31") DefaultColor = Color.Red;
                    else if (setting == "32") DefaultColor = Color.LawnGreen;
                    else if (setting == "33") DefaultColor = Color.Yellow;
                    else if (setting == "34") DefaultColor = Color.DeepSkyBlue;
                    else if (setting == "35") DefaultColor = Color.Magenta;
                    else if (setting == "36") DefaultColor = Color.Cyan;
                    else if (setting == "37") DefaultColor = Color.White;
                    else if (setting == "39") DefaultColor = Color.White;
                    else if (setting == "40") SetBackground = Color.Black;
                    else if (setting == "41") SetBackground = Color.Red;
                    else if (setting == "42") SetBackground = Color.LawnGreen;
                    else if (setting == "43") SetBackground = Color.Yellow;
                    else if (setting == "44") SetBackground = Color.DeepSkyBlue;
                    else if (setting == "45") SetBackground = Color.Magenta;
                    else if (setting == "46") SetBackground = Color.Cyan;
                    else if (setting == "47") SetBackground = Color.White;
                    else if (setting == "49") SetBackground = DefaultBackgroundColor;
                }
            }
            else
            {
                //Set all to defaults
                Bold = false; DefaultColor = Color.White; Underline = false; SetBackground = DefaultBackgroundColor;
            }

        }

        private void SetScrollSize(string command)
        {
            //Set screen to specified size
            if (command.Contains(";"))
            {
                int height = Convert.ToInt32(command.Substring(command.IndexOf(';') + 1, command.IndexOf('r') - 1 - command.IndexOf(';')));
                if (height != 0 && height >= ScreenHeight) { ScreenHeight = height; }
            }
            else
            {
                //Set screen size to size of the window
                int height = TXT_terminal.Size.Height;
                int width = TXT_terminal.Size.Width;
                ScreenHeight = height / 17;
                ScreenWidth = width / 9;
            }
            LineCounter = ScreenHeight - 1;
        }

        //Delete given number of characters and replace with nothing
        private void DeleteChars(string command)
        {
            int delNum = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2)));
            int index = TXT_terminal.SelectionStart;
            int currentLine = TXT_terminal.GetLineFromCharIndex(index);
            TXT_terminal.Select(index, delNum);
            TXT_terminal.SelectedText = "";
        }

        //Reverse order of data so first line is printed on last line
        private void ReverseOrder(string command)
        {
            int firstChar = TXT_terminal.GetFirstCharIndexOfCurrentLine();
            int line = TXT_terminal.GetLineFromCharIndex(firstChar);
            line = ScreenHeight - line;
            if (LineCounter > 0)
            {
                CursorToLine(LineCounter);
                LineCounter--;
            }
        }

        private void ScrollUp(string command)
        {

            int LastIndex = TXT_terminal.SelectionStart;
            int LastLine = TXT_terminal.GetLineFromCharIndex(LastIndex);
            if (TXT_terminal.Lines[LastLine].Length != 0)
            {
                LastIndex = TXT_terminal.GetFirstCharIndexFromLine(LastLine + 1);
                if (LastIndex == -1)
                {
                    LastIndex = TXT_terminal.SelectionStart;
                }
                TXT_terminal.SelectionStart = LastIndex;
            }
            int scrollNum = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2)));
            //Scroll up to top of replacement text
            CursorUp("\u001b[" + scrollNum + "A");
            int IntitialScrollIndex = TXT_terminal.SelectionStart;
            //Select replacement text and save it
            TXT_terminal.Select(IntitialScrollIndex, LastIndex - IntitialScrollIndex);
            string replacementText = TXT_terminal.SelectedText;

            //Scroll to top of old text
            CursorUp("\u001b[" + scrollNum + "A");
            int FinalScrollIndex = TXT_terminal.SelectionStart;

            //Select old text and replace with new text
            TXT_terminal.Select(FinalScrollIndex, IntitialScrollIndex - FinalScrollIndex);
            TXT_terminal.SelectedText = replacementText;

            int charIndex = TXT_terminal.GetFirstCharIndexOfCurrentLine();
            int line = TXT_terminal.GetLineFromCharIndex(charIndex);
            //Replace lines with blank characters
            for (int i = line; i <= LastLine; i++)
            {
                int firstchar = TXT_terminal.GetFirstCharIndexFromLine(i);
                int length = TXT_terminal.Lines[i].Length;
                TXT_terminal.SelectionStart = firstchar;
                AppendText(new string(' ', length));
            }
            ArrowMovement = false;
        }

        private void ScrollDown(string command)
        {

            int LastIndex = TXT_terminal.SelectionStart;
            int FirstLine = TXT_terminal.GetLineFromCharIndex(LastIndex);
            int scrollNum = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2)));

            //Scroll down to bottom of replacement text
            CursorDown("\u001b[" + scrollNum + "B");
            int IntitialScrollIndex = TXT_terminal.SelectionStart;
            //Select replacement text and save it
            TXT_terminal.Select(IntitialScrollIndex, LastIndex - IntitialScrollIndex);
            string replacementText = TXT_terminal.SelectedText;

            //Scroll to bottom of old text
            TXT_terminal.Select(IntitialScrollIndex, 0);
            CursorDown("\u001b[" + scrollNum + "B");
            int FinalScrollIndex = TXT_terminal.SelectionStart;
            //Select old text and replace with new text
            TXT_terminal.Select(IntitialScrollIndex, FinalScrollIndex - IntitialScrollIndex);
            TXT_terminal.SelectedText = replacementText;

            int charIndex = TXT_terminal.GetFirstCharIndexOfCurrentLine();
            int line = TXT_terminal.GetLineFromCharIndex(IntitialScrollIndex);
            //Replace lines with blank characters
            for (int i = FirstLine; i < line; i++)
            {
                int firstchar = TXT_terminal.GetFirstCharIndexFromLine(i);
                int length = TXT_terminal.Lines[i].Length;
                TXT_terminal.SelectionStart = firstchar;
                AppendText(new string(' ', length));
            }
            ArrowMovement = false;
        }

        //Remove characters and replace them with the blank characters
        private void RemoveChars(string command)
        {
            int delNum = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2)));
            int index = TXT_terminal.SelectionStart;
            int currentLine = TXT_terminal.GetLineFromCharIndex(index);
            TXT_terminal.Select(index, delNum);
            TXT_terminal.SelectedText = new string(' ', delNum);
        }

        private void SaveCursor()
        {
            CursorPosition = TXT_terminal.SelectionStart;
        }

        private void RestoreCursor()
        {
            TXT_terminal.SelectionStart = CursorPosition;
        }

        #endregion
    }

    //Class used to get connection details from user
    public static class Prompt
    {
        public static string ShowDialog(string text, string text2, string caption)
        {
            Form prompt = new Form()
            {
                Width = 500,
                Height = 250,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = caption,
                StartPosition = FormStartPosition.CenterScreen
            };
            Font font = new Font("Calibri", 14.0f);
            Label textLabel = new Label() { Left = 50, Top = 20, Text = text };
            Label textLabel2 = new Label() { Left = 50, Top = 85, Text = text2 };
            TextBox textBox = new TextBox() { Left = 50, Top = 50, Width = 400 };
            TextBox textBox2 = new TextBox() { Left = 50, Top = 110, Width = 400 };
            Button confirmation = new Button() { Text = "Connect", Left = 350, Width = 110, Height = 50, Top = 155, DialogResult = DialogResult.OK };
            confirmation.Click += (sender, e) => { prompt.Close(); };
            textLabel.Font = font;
            textLabel2.Font = font;
            textBox.Font = font;
            textBox2.Font = font;
            textBox2.PasswordChar = '*';
            prompt.Controls.Add(textBox);
            prompt.Controls.Add(textBox2);
            prompt.Controls.Add(confirmation);
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textLabel2);
            prompt.AcceptButton = confirmation;

            return prompt.ShowDialog() == DialogResult.OK ? textBox.Text + ":" + textBox2.Text : "";
        }
    }

}