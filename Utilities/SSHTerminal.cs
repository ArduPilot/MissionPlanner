using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using log4net;
using Renci.SshNet;
using Renci.SshNet.Common;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace MissionPlanner.Utilities
{
    public class SSHTerminal
    {
        private static readonly log4net.ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public static bool threadrun;
        private readonly List<string> cmdHistory = new List<string>();
        private readonly object thisLock = new object();
        private int history;
        private SshClient client;
        public static bool SSHTerm = false;
        private ShellStream shellStream;
        private int SelectStartIndex = 0;
        private int inputStartPos;
        DateTime lastsend = DateTime.MinValue;
        private int ScreenWidth = 0;
        private int ScreenHeight = 0;
        private int ScreenTop = 0;
        private int ScreenBottom = 0;
        //Variables used to set different modes based on ANSI codes
        #region ANSI Settings
        private bool Bold = false;
        private bool Underline = false;
        private bool Italicized = false;
        private Color DefaultColor = Color.White;
        private Color SetBackground = Color.Black;
        private bool ArrowMovement = false;
        private int CursorPosition;
        private bool DECCKM = false;
        private int AlternativeCursorPosition;
        private string AlternativeScreenData = "";
        private bool DECKPAM = false;
        private bool DECKAWM = false;
        private bool Scrolling = false;
        private bool ReverseIndex = false;
        //private bool ByobuMode = false;
        #endregion
        //Variables needed to get ConnectionInfo
        #region ConnectionInfo
        private bool DisplayChars = true;
        private string Username = "";
        private string Password = "";
        private string IPAddress = "";
        private int Port = 22;
        private bool ConnectionInfo = false;
        #endregion

        private RichTextBox TXT_terminal;

        public SSHTerminal(RichTextBox rtb)
        {
            TXT_terminal = rtb;
            SSHTerm = false;

            TXT_terminal.KeyDown += TXT_terminal_KeyDown;
            TXT_terminal.KeyPress += TXT_terminal_KeyPress;

            TXT_terminal.AcceptsTab = true;
        }
        
        [DllImport("user32.dll")]
        public static extern long ShowCaret(IntPtr hwnd);

        [DllImport("user32.dll")]
        public static extern long HideCaret(IntPtr hwnd);

        private void TXT_terminal_KeyDown(object sender, KeyEventArgs e)
        {
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
                        if (SSHTerm)
                        {
                            if (DECCKM) shellStream.Write("\u001bOA");
                            else shellStream.Write("\u001b[A");
                            ArrowMovement = true;
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
                        if (SSHTerm)
                        {
                            if (DECCKM) shellStream.Write("\u001bOB");
                            else shellStream.Write("\u001b[B");
                            ArrowMovement = true;
                        }
                        e.Handled = true;
                        break;
                    case Keys.Right:
                        if (SSHTerm)
                        {
                            if (DECCKM) shellStream.Write("\u001bOC");
                            else shellStream.Write("\u001b[C");
                            e.Handled = true;
                        }
                        break;
                    case Keys.Left:
                        if (SSHTerm)
                        {
                            if (DECCKM) shellStream.Write("\u001bOD");
                            else shellStream.Write("\u001b[D");
                            e.Handled = true;
                        }
                        break;
                    case Keys.Back:
                        if (TXT_terminal.SelectionStart <= inputStartPos)
                            e.Handled = true;
                        break;
                    case Keys.F1:
                        if (SSHTerm) {shellStream.Write("\u001bOP"); e.Handled = true; }
                        break;
                    case Keys.F2:
                        if (SSHTerm) { shellStream.Write("\u001bOQ"); e.Handled = true; }
                            break;
                    case Keys.F3:
                        if (SSHTerm) { shellStream.Write("\u001bOR"); e.Handled = true; }
                            break;
                    case Keys.F4:
                        if (SSHTerm) { shellStream.Write("\u001bOS"); e.Handled = true; }
                            break;
                    case Keys.F5:
                        if (SSHTerm) { shellStream.Write("\u001b[15~"); e.Handled = true; }
                            break;
                    case Keys.F6:
                        if (SSHTerm) { shellStream.Write("\u001b[17~"); e.Handled = true; }
                            break;
                    case Keys.F7:
                        if (SSHTerm) { shellStream.Write("\u001b[18~"); e.Handled = true; }
                            break;
                    case Keys.F8:
                        if (SSHTerm) { shellStream.Write("\u001b[19~"); e.Handled = true; }
                            break;
                    case Keys.F9:
                        if (SSHTerm) { shellStream.Write("\u001b[20~"); e.Handled = true; }
                            break;
                    case Keys.F10:
                        if (SSHTerm) { shellStream.Write("\u001b[21~"); e.Handled = true; }
                            break;
                    case Keys.F11:
                        if (SSHTerm) { shellStream.Write("\u001b[23~"); e.Handled = true; }
                            break;
                    case Keys.F12:
                        if (SSHTerm) { shellStream.Write("\u001b[24~"); e.Handled = true; }
                            break;
                    case Keys.PageUp:
                        if (SSHTerm) { shellStream.Write("\u001b[5~"); e.Handled = true; }
                            break;
                    case Keys.PageDown:
                        if (SSHTerm) { shellStream.Write("\u001b[6~"); e.Handled = true; }
                            break;
                    case Keys.Home:
                        if (SSHTerm)
                        {
                            if (DECCKM) shellStream.Write("\u001bOH");
                            else shellStream.Write("\u001b[H");
                            e.Handled = true;
                        }
                        break;
                    case Keys.End:
                        if (SSHTerm)
                        {
                            if (DECCKM) shellStream.Write("\u001bOF");
                            else shellStream.Write("\u001b[F");
                            e.Handled = true;
                        }
                        break;
                    case Keys.Insert:
                        if (SSHTerm && DECKPAM && !Control.IsKeyLocked(Keys.NumLock)) { shellStream.Write("\u001b[2~"); e.Handled = true; }
                            break;
                    case Keys.Delete:
                        if (SSHTerm && DECKPAM && !Control.IsKeyLocked(Keys.NumLock)) { shellStream.Write("\u001b[3~"); e.Handled = true; }
                            break;
                }
            }
        }

        public void Stop()
        {
            threadrun = false;

            try
            {
                if (SSHTerm)
                {
                    client.Disconnect();
                    client.Dispose();
                    SSHTerm = false;
                    threadrun = ConnectionInfo = ReverseIndex  = ArrowMovement = false;
                    DisplayChars = true;
                }
            }
            catch
            {
            }
        }

        private void TXT_terminal_KeyPress(object sender, KeyPressEventArgs e)
        {
            //If control-C then reset display
            if (e.KeyChar == '\u0003') { EditDisplay("\u001b[0m"); }

            //Getting ConnectionInfo form user
            if (ConnectionInfo)
            {
                if (e.KeyChar == '\r')
                {
                    if (!DisplayChars)
                    {
                        start_SSHterminal();
                        return;
                    }
                    else
                    {
                        TXT_terminal.Select(TXT_terminal.Text.IndexOf(":") + 1, TXT_terminal.TextLength - 1);
                        Username = TXT_terminal.SelectedText.Trim();
                        TXT_terminal.AppendText(Username + "@" + IPAddress + "'s password:");
                        DisplayChars = false; inputStartPos = TXT_terminal.TextLength;

                    }
                }
                if (!DisplayChars)
                {
                    if (e.KeyChar == '\b' && Password != "") Password = Password.Remove(Password.Length - 1, 1);
                    else if (e.KeyChar != '\b' && e.KeyChar != '\r' && e.KeyChar != '\n') Password += e.KeyChar.ToString();
                    e.KeyChar = '\0';
                }
                return;
            }

            if (SSHTerm)
            {
                try
                {
                    shellStream.Write(e.KeyChar.ToString());
                    e.KeyChar = '\0';
                }
                catch (Exception exp)
                {
                    Console.WriteLine("Command Sending Error Occured {0} ", exp);
                    TXT_terminal.AppendText("Command Failed. Reason for failure was: " + exp.Message + " \r\n");
                }
            }
        }

 

        private void start_SSHterminal()
        {
            try
            {
                DisplayChars = true; ConnectionInfo = false;
                //Connect to SSH client
                ConnectionInfo connectionInfo = new ConnectionInfo(IPAddress, Port, Username, new AuthenticationMethod[] { new PasswordAuthenticationMethod(Username, Password) });
                client = new SshClient(connectionInfo);
                client.Connect();
                SSHTerm = true;
                //Setting required variables for ssh terminal
                SelectStartIndex = inputStartPos = TXT_terminal.Text.Length;
                //Terminal type is set to xterm
                ScreenWidth = 80;
                ScreenHeight = 24;
                ScreenBottom = ScreenHeight - 1;
                //Create terminal of size 24x80 with TERM variable set to xterm, note other TERM values may not work for this SSH terminal interpretation 
                shellStream = client.CreateShellStream("xterm", 80, 24, 800, 600, 1024);
                startSSHthread();
            }
            catch (SshAuthenticationException)
            {
                Console.WriteLine("Password or username was incorrect");
                TXT_terminal.AppendText("Connection failed, due to incorrect Username or Password. Please try again \r\n");
            }
            catch (System.Net.Sockets.SocketException se)
            {
                Console.WriteLine("Exception {0} occurred", se.Message);
                TXT_terminal.AppendText("Connection Failed. Reason for failure was:  " + se.Message + "\r\n");
            }
            catch (SshConnectionException exp)
            {
                Console.WriteLine("SSHConnection Error Occured {0} ", exp);
                TXT_terminal.AppendText("Connection Failed. Reason for failure was: " + exp.Message + " \r\n");
            }
            catch (Exception exp)
            {
                Console.WriteLine("Connection Error Occured {0} ", exp);
                TXT_terminal.AppendText("Connection Failed. Reason for failure was: " + exp.Message + " \r\n");
            }
        }

        public void startSSHthread()
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
                    if (!SSHTerm)
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
            try
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
            catch (Exception exp)
            {
                Console.WriteLine("Reading from Shellstream error Occured {0} ", exp);
                TXT_terminal.AppendText("ShellStream read failed. Reason for failure was: " + exp.Message + " \r\n");
            }
        }

        private void SSH_AddText(string data)
        {
            TXT_terminal.BeginInvoke((MethodInvoker) delegate
            {
                //Need to clear clipboard for controls to work in nano
                try
                {
                    Clipboard.Clear();
                }
                catch
                {

                }

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

                //Set ReverseIndex if data contains flag
                if (Regex.IsMatch(data, @"\eM"))
                {
                    ReverseIndex = true;
                }

                //Handle directional arrows
                if (Regex.IsMatch(data, @".*ESC.*O.*[ABCD]\r"))
                {
                    Match m = Regex.Match(data, @".*ESC.*O.*[ABCD]\r");
                    data = data.Remove(m.Index, m.Length - 1);
                    Scrolling = true;
                }

                //Handle page up or page down
                if (Regex.IsMatch(data, @".*ESC.*\[?[56].*~\r"))
                {
                    Match m = Regex.Match(data, @".*ESC.*\[?[56].*~\r");
                    data = data.Remove(m.Index, m.Length - 1);
                    Scrolling = true;
                }

                #region Code to disable Byobu 

                //if (Regex.IsMatch(data, @"- byobu\a"))
                //{
                //    ByobuMode = true;
                //    Thread.Sleep(100);
                //    shellStream.WriteLine("\u001b[17~");
                //    data = "";
                //}
                //if (data.Contains("[detached")) { ByobuMode = false; data = data.Insert(data.IndexOf(")") + 1, " Byobu is unsupported by this terminal"); }
                // if (ByobuMode) { data = ""; } // Don't print characters if Byobu was started

                #endregion

                string pattern = @"\e[\(\[\]](\d+;)*\??(\d+)?[ABCDEFGHJKSPTLMflXthmbnirpdsu]";
                //Removing Unnessary carriage returns
                if (data.StartsWith("\r") && !data.StartsWith("\r\n"))
                {
                    data = data.Remove(data.IndexOf("\r"), 1);
                    CursorToColumn(0);
                }

                foreach (Match match in Regex.Matches(data, pattern))
                {
                    var index = data.IndexOf(match.Value);
                    if (index != 0) //There are words to add before next ANSI character
                    {
                        var subString = data.Substring(0, data.IndexOf(match.Value));
                        if (subString != "") data = data.Remove(0, subString.Length);
                        //Format Data to remove any unwanted characters
                        subString = FormatData(subString);
                        //If newline or return character move down line instead of printing newline or return character
                        if (subString.Contains("\r\n")) subString = TrimText(subString, "\r\n");
                        if (subString.Contains("\r")) subString = TrimText(subString, "\r");
                        //Print received  Text
                        SendReceivedPackets(subString);
                        inputStartPos = TXT_terminal.TextLength;
                    }

                    ResolveCommand(match.Value);
                    data = data.Remove(0, match.Length);
                }

                //Format Data to remove any unwanted characters
                data = FormatData(data);
                //If newline or return character move down line instead of printing newline or return character
                if (data.Contains("\r\n")) data = TrimText(data, "\r\n");
                if (data.Contains("\r") && !DECKAWM) data = TrimText(data, "\r");
                //Print received  Text
                SendReceivedPackets(data);
                inputStartPos = TXT_terminal.TextLength;
                SelectStartIndex = TXT_terminal.SelectionStart;
                TXT_terminal.Focus();
                ReverseIndex = ArrowMovement = false;
            });
            Thread.Sleep(70);
        }

        //Send the received data in packets of less than the screen width
        private void SendReceivedPackets(string subString)
        {
            if (subString.Contains("\b \b")) subString = BackspaceHandler(subString, "\b \b");
            if (subString.Contains("\b")) subString = BackspaceHandler(subString, "\b");
            int Width = ScreenWidth;
            int packets = (subString.Length / ScreenWidth) + 1;
            if (subString.Length == ScreenWidth) packets--;
            int length = subString.Length;
            int cursorPosition = TXT_terminal.SelectionStart;
            int column = cursorPosition - TXT_terminal.GetFirstCharIndexOfCurrentLine();
            //If inserting the packet at this position exceeds the screen
            //size then set max width and increase packets by one if packets is currently one
            if (column + length > ScreenWidth)
            {
                Width = ScreenWidth - column;
                if (packets == 1) packets++;
            }
            while (length > 0)
            {
                int n = (int)Math.Min(Width, length); //Get the max length the packet can be
                string packet = subString.Substring(0, n);
                subString = subString.Remove(0, n);
                AppendText(packet);
                length -= n;
                if (packets > 1) // If we didn't just print the last packet then move down a line
                {
                    CursorDown("\u001b[B");
                }
                packets--;
                Width = ScreenWidth;
            }
        }

        //Scroll the screen if we have reached the max possible display lines and
        //the next line is either below or above the screen size
        private void ScreenScroll(int Top, int Bottom)
        {
            int i = 0;
            if (ReverseIndex)
            {
                for (i = Bottom; i > Top; i--)
                {
                    TXT_terminal.Select(TXT_terminal.GetFirstCharIndexFromLine(i - 1), TXT_terminal.Lines[i - 1].Length);
                    string SavedText = TXT_terminal.SelectedRtf;
                    TXT_terminal.Select(TXT_terminal.GetFirstCharIndexFromLine(i), TXT_terminal.Lines[i].Length);
                    TXT_terminal.SelectedRtf = SavedText;
                }
            }
            else
            {

                for (i = Top; i < Bottom && i < TXT_terminal.Lines.Count() - 1; i++)
                {
                    TXT_terminal.Select(TXT_terminal.GetFirstCharIndexFromLine(i + 1), TXT_terminal.Lines[i + 1].Length);
                    string SavedText = TXT_terminal.SelectedRtf;
                    TXT_terminal.Select(TXT_terminal.GetFirstCharIndexFromLine(i), TXT_terminal.Lines[i].Length);
                    TXT_terminal.SelectedRtf = SavedText;
                }
                TXT_terminal.Select(TXT_terminal.GetFirstCharIndexFromLine(i), TXT_terminal.Lines[i].Length);
                TXT_terminal.SelectedText = "\0";
            }
        }

        //Trim text for return carriages and new line characters
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
                    subString = subString.Trim('\r', '\n');
                    SendReceivedPackets(subString);
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

        private string FormatData(string data)
        {
            if (data == "") return data;
            //Reverse Index - Move all lines down by one ready
            //to insert new line
            if (Regex.IsMatch(data, @"\eM") && Scrolling)
            {
                var index = TXT_terminal.SelectionStart;
                TXT_terminal.Select(index, TXT_terminal.TextLength);
                string movingText = TXT_terminal.SelectedRtf;
                TXT_terminal.SelectedRtf = "";
                TXT_terminal.Select(0, 0);
                CursorDown("\u001b[B");
                int newIndex = TXT_terminal.SelectionStart;
                TXT_terminal.Select(newIndex, TXT_terminal.TextLength);
                TXT_terminal.SelectedRtf = movingText;
                int count = TXT_terminal.Lines.Count();
                if (count > ScreenHeight)
                {
                    int charindex = TXT_terminal.GetFirstCharIndexFromLine(count - 2);
                    TXT_terminal.Select(charindex, TXT_terminal.TextLength - charindex);
                    TXT_terminal.SelectedRtf = "";
                }
                Match match = Regex.Match(data, @"\eM");
                data = data.Remove(match.Index, match.Length);
                MoveCursor("\u001b[H");

            }
            //Save or restore cursor 
            foreach (Match match in Regex.Matches(data, @"\e[78]"))
            {
                if (match.Value.Contains("7")) { SaveCursor(); }
                if (match.Value.Contains("8")) { RestoreCursor(); }
                data = data.Remove(match.Index, match.Length);
            }
            //Remove operating system insturctions
            foreach (Match match in Regex.Matches(data, @"\u001b]\d+;"))
            {
                int index = data.IndexOf('\u001b');
                int endIndex = data.IndexOf("\a");
                if (endIndex < index) { data = data.Remove(endIndex, 1); endIndex = data.IndexOf("\a"); }
                data = data.Remove(match.Index, endIndex - match.Index + 1);
                EditDisplay("\u001b[0m");
            }
            //Remove unwanted ANSI characters
            foreach (Match match in Regex.Matches(data, @"\e\[?\]?\=?\>?[0-9]*[M@]?"))
            {
                if (match.Value.Contains("=")) DECKPAM = true;
                if (match.Value.Contains(">")) DECKPAM = false;

                data = data.Remove(match.Index, match.Length);
            }
            //Remove ESC sequences that were not correctly handled
            foreach (Match match in Regex.Matches(data, @"^ESC"))
            {
                data = data.Remove(match.Index, data.Length);
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
            foreach (Match match in Regex.Matches(data, @"\a.*"))
            {
                data = data.Remove(match.Index, match.Length);
            }
            return data;
        }

        private string BackspaceHandler(string data, string backSpaceKey)
        {
            int count = 0;
            count = data.Count(f => f == '\b');
            for (int i = 0; i < count; i++)
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
            if (text == "") return;
            var index = TXT_terminal.SelectionStart;
            var line = TXT_terminal.GetLineFromCharIndex(index);
            var length = 0;
            if (TXT_terminal.Lines.Count() > line && TXT_terminal.Lines[line] != "")
            {
                lastIndex = TXT_terminal.Lines[line].LastIndexOf(TXT_terminal.Lines[line].Last());
                firstchar = TXT_terminal.GetFirstCharIndexFromLine(line);
                length = lastIndex + firstchar;
            }
            //Check if need to scroll screen up based on new screen size
            if (line == ScreenBottom && text == "\n")
            {
                ScreenScroll(ScreenTop, ScreenBottom);
                return;
            }
            //Check if we need to scroll one up
            if (line == ScreenHeight)
            {
                ScreenScroll(ScreenTop, ScreenBottom);
            }
            //Don't print newline chars
            if (text.Contains("\n")) text = text.Replace("\n", "");
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
                //Don't want black text on black background so set text color to "Bright" black
                if (SetBackground == Color.Black && DefaultColor == Color.Black)
                {
                    TXT_terminal.SelectionBackColor = SetBackground;
                    TXT_terminal.SelectionColor = Color.FromArgb(127, 127, 127);
                }
                else
                {
                    TXT_terminal.SelectionBackColor = SetBackground;
                    TXT_terminal.SelectionColor = DefaultColor;
                }
                if (Bold && Underline && Italicized) TXT_terminal.SelectionFont = new Font(TXT_terminal.Font, FontStyle.Bold | FontStyle.Underline | FontStyle.Italic);
                else if (Bold && Italicized) TXT_terminal.SelectionFont = new Font(TXT_terminal.Font, FontStyle.Bold | FontStyle.Italic);
                else if (Bold && Underline) TXT_terminal.SelectionFont = new Font(TXT_terminal.Font, FontStyle.Bold | FontStyle.Underline);
                else if (Italicized && Underline) TXT_terminal.SelectionFont = new Font(TXT_terminal.Font, FontStyle.Underline | FontStyle.Italic);
                else if (Bold) TXT_terminal.SelectionFont = new Font(TXT_terminal.Font, FontStyle.Bold);
                else if (Underline) TXT_terminal.SelectionFont = new Font(TXT_terminal.Font, FontStyle.Underline);
                else if (Italicized) TXT_terminal.SelectionFont = new Font(TXT_terminal.Font, FontStyle.Italic);
                else TXT_terminal.SelectionFont = new Font(TXT_terminal.Font, FontStyle.Regular);
            }
            index = TXT_terminal.SelectionStart;
            var position = index + text.Length;
            //Move cursor to end
            TXT_terminal.Select(position, 0);
        }

        //Set variables ready to receive input from user for SSH connection
        public void SSH_ConnectionInfo(string IPAddressBox)
        {
            TXT_terminal.Clear();
            TXT_terminal.BackColor = Color.Black;
            if (IPAddressBox == "IP Address:Port" || IPAddressBox == "")
            {
                TXT_terminal.AppendText("IP Address not specified, please enter an IP Address and reconnect");
            }
            else
            {
                Password = Username = "";
                string IpAddressBox_Text = IPAddressBox;
                if (IpAddressBox_Text.Contains(":"))
                {
                    IPAddress = IpAddressBox_Text.Substring(0, IpAddressBox_Text.IndexOf(":"));
                    Port = Convert.ToInt32(IpAddressBox_Text.Substring(IpAddressBox_Text.IndexOf(":") + 1));
                }
                else
                {
                    IPAddress = IpAddressBox_Text;
                }
                ConnectionInfo = true; DisplayChars = true;
                TXT_terminal.AppendText("login as:");
                inputStartPos = TXT_terminal.TextLength;
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
            if (command.EndsWith("L")) InsertNewLines(command);
            if (command.EndsWith("h")) DEC_Mode_Set(command);
            if (command.EndsWith("l")) DEC_Mode_Reset(command);
            if (command.EndsWith("n")) ReportCursorPosition(command);
            if (command.EndsWith("m")) EditDisplay(command);
            if (command.EndsWith("M")) DeleteLines(command);
            if (command.EndsWith("r")) SetScrollSize(command);
            if (command.EndsWith("P")) DeleteChars(command);
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

            //Move cursor forward by given number else move by one
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
            //Move to previous column
            int firstNewIndex = TXT_terminal.GetFirstCharIndexOfCurrentLine();
            if (column != 0 && TXT_terminal.Lines[line].Length >= column)
            {
                CursorToColumn(column);
            }
            ArrowMovement = false;
        }

        private void CursorToLine(string command)
        {
            int line = 0;
            if (Regex.IsMatch(command, @"\e\[\d+F"))
            {
                line = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2))) - 1;
            }
            CursorToLine(line);
        }

        private void CursorToLine(int line)
        {
            //If line is bigger or equal to screen height, then we need to scroll up
            if (line >= ScreenHeight) { ScreenScroll(ScreenTop, ScreenBottom); if (line > ScreenHeight) { line = ScreenHeight; } else { line--; } }
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
            int column = 0;
            if (Regex.IsMatch(command, @"\e\[\d+G"))
            {
                column = Convert.ToInt32(command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2)) - 1;
            }
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
                if (text == "") { TXT_terminal.SelectionBackColor = Color.Black; TXT_terminal.SelectionColor = Color.White; TXT_terminal.SelectedText = " "; }
                if (text == "\n") { TXT_terminal.SelectionBackColor = Color.Black; TXT_terminal.SelectionColor = Color.White; TXT_terminal.SelectedText = " \n"; }
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
                if (row == ScreenTop && ReverseIndex)
                {
                    ScreenScroll(ScreenTop, ScreenBottom);
                }
                if (row <= line)
                { //Move up or stay there

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
                TXT_terminal.SelectionBackColor = SetBackground;
                TXT_terminal.SelectionColor = DefaultColor;
                TXT_terminal.SelectedText = "\0";
            }
            //Clear line left of cursor
            else if (command.Contains("1"))
            {
                TXT_terminal.Select(charIndex, index - charIndex);
                TXT_terminal.SelectionBackColor = SetBackground;
                TXT_terminal.SelectionColor = DefaultColor;
                AppendText(new string(' ', index - charIndex));
                TXT_terminal.Select(index, 0);
            }
            //Clear line right of cursor
            else
            {
                TXT_terminal.Select(index, RemoveLength);
                TXT_terminal.SelectionBackColor = SetBackground;
                TXT_terminal.SelectionColor = DefaultColor;
                TXT_terminal.SelectedText = "\0";

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
                    TXT_terminal.SelectionBackColor = SetBackground;
                    TXT_terminal.SelectionColor = DefaultColor;
                    TXT_terminal.SelectedText = new string(' ', index - firstIndex); ;
                }
                //Clear screen from cursor down
                else
                {
                    TXT_terminal.Select(index, TXT_terminal.TextLength);
                    TXT_terminal.SelectionBackColor = SetBackground;
                    TXT_terminal.SelectionColor = DefaultColor;
                    TXT_terminal.SelectedText = "";
                    inputStartPos = TXT_terminal.TextLength;
                }
            }
        }

        //To Insert New lines move all the lines down by specified number
        //then insert new lines above
        private void InsertNewLines(string command)
        {
            int count = 1;
            var pattern = @"\e\[\d+L";
            if (Regex.IsMatch(command, pattern))
            {
                count = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2)));
            }
            int index = TXT_terminal.GetFirstCharIndexOfCurrentLine();
            int line = TXT_terminal.GetLineFromCharIndex(index);
            int lastline = ScreenBottom - count;
            int max = ScreenBottom;
            for (int i = lastline; i >= line; i--)
            {
                TXT_terminal.Select(TXT_terminal.GetFirstCharIndexFromLine(i), TXT_terminal.Lines[i].Length);
                string SavedText = TXT_terminal.SelectedRtf;
                for (int j = lastline + 1; j <= max; j++)
                {
                    TXT_terminal.Select(TXT_terminal.GetFirstCharIndexFromLine(j), TXT_terminal.Lines[j].Length);
                    TXT_terminal.SelectedRtf = SavedText;
                }
                lastline--;
                max--;
            }
        }

        //Set modes based on ANSI characters
        private void DEC_Mode_Set(string command)
        {
            int Commandvalue = 0;
            if (command.Contains("?"))
            {
                Commandvalue = Convert.ToInt32((command.Substring(command.IndexOf('?') + 1, command.Length - command.IndexOf('?') - 2)));
            }
            if (Commandvalue == 1) { DECCKM = true; }
            else if (Commandvalue == 7) { DECKAWM = true; }
            else if (Commandvalue == 25) {
                try
                {
                    ShowCaret(TXT_terminal.Handle);
                }
                catch
                {
                }
            }
            else if (Commandvalue == 1049) { AlternativeCursorPosition = TXT_terminal.SelectionStart; SaveScreen(); }

        }

        //Reset modes based on ANSI characters
        private void DEC_Mode_Reset(string command)
        {
            int Commandvalue = 0;
            if (command.Contains("?"))
            {
                Commandvalue = Convert.ToInt32((command.Substring(command.IndexOf('?') + 1, command.Length - command.IndexOf('?') - 2)));
            }
            if (Commandvalue == 1) { DECCKM = false; }
            else if (Commandvalue == 7) { DECKAWM = false; }
            else if (Commandvalue == 25) {
                try
                {
                    HideCaret(TXT_terminal.Handle);
                }
                catch
                {
                }
            }
            else if (Commandvalue == 1049) { DECKAWM = false; RestoreScreen(); TXT_terminal.SelectionStart = AlternativeCursorPosition; ; }
        }

        //Calulate Cursor Position and report back using shellstream, used for resize
        private void ReportCursorPosition(string command)
        {
            if (Regex.IsMatch(command, @"\e\[\dn"))
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
        }

        //Set character attributes based on command
        private void EditDisplay(string command)
        {
            var pattern = @"\e\[(\d+;)*\d+m";
            if (Regex.IsMatch(command, pattern))
            {

                int[] Settings;
                Match m = Regex.Match(command, pattern);
                int count = command.Count(f => f == ';');
                if (count == 2)
                {
                    Settings = new int[3];
                    Settings[0] = Convert.ToInt32(command.Substring(command.IndexOf('[') + 1, command.IndexOf(';') - 1 - command.IndexOf('[')));
                    Settings[1] = Convert.ToInt32(command.Substring(command.IndexOf(';') + 1, command.LastIndexOf(';') - 1 - command.IndexOf(';')));
                    Settings[2] = Convert.ToInt32(command.Substring(command.LastIndexOf(';') + 1, command.IndexOf('m') - 1 - command.LastIndexOf(';')));
                }
                else if (count == 1)
                {
                    Settings = new int[2];
                    Settings[0] = Convert.ToInt32(command.Substring(command.IndexOf('[') + 1, command.IndexOf(';') - 1 - command.IndexOf('[')));
                    Settings[1] = Convert.ToInt32(command.Substring(command.IndexOf(';') + 1, command.IndexOf('m') - 1 - command.IndexOf(';')));
                }
                else { Settings = new int[1]; Settings[0] = Convert.ToInt32(command.Substring(command.IndexOf('[') + 1, command.IndexOf('m') - 1 - command.IndexOf('['))); }

                //Set Forecolor using XTerm RBG Colors
                if (Settings[0] == 38 && Settings[1] == 5)
                {
                    TXT_terminal.ForeColor = XTermRBGColors.GetColor(Settings[2]);
                    return;
                }
                //Set Backcolor using XTerm RBG Colors
                if (Settings[0] == 48 && Settings[1] == 5)
                {
                    SetBackground = XTermRBGColors.GetColor(Settings[2]); ;
                    return;
                }
                foreach (int setting in Settings)
                {
                    if (setting == 0) { Bold = false; Underline = false; DefaultColor = Color.White; SetBackground = Color.Black; } //Reset
                    else if (setting == 1) Bold = true;
                    else if (setting == 3) Italicized = true;
                    else if (setting == 4) Underline = true;
                    else if (setting == 7) { Color Temp = DefaultColor; DefaultColor = SetBackground; SetBackground = Temp; }
                    else if (setting == 22) { Bold = false; }
                    else if (setting == 23) { Italicized = false; }
                    else if (setting == 24) { Underline = false; }
                    else if (setting == 27) { Color Temp = DefaultColor; DefaultColor = SetBackground; SetBackground = Temp; }
                    else if (setting == 30) DefaultColor = Color.Black;
                    else if (setting == 31) DefaultColor = Color.Red;
                    else if (setting == 32) DefaultColor = Color.FromArgb(0, 205, 0); //Green
                    else if (setting == 33) DefaultColor = Color.Yellow;
                    else if (setting == 34) DefaultColor = Color.FromArgb(92, 92, 255); // Blue
                    else if (setting == 35) DefaultColor = Color.Magenta;
                    else if (setting == 36) DefaultColor = Color.Cyan;
                    else if (setting == 37) DefaultColor = Color.White;
                    else if (setting == 39) DefaultColor = Color.White;
                    else if (setting == 40) SetBackground = Color.Black;
                    else if (setting == 41) DefaultColor = Color.Red;
                    else if (setting == 42) SetBackground = Color.FromArgb(0, 205, 0); //Green
                    else if (setting == 43) SetBackground = Color.Yellow;
                    else if (setting == 44) SetBackground = Color.FromArgb(92, 92, 255); //Blue
                    else if (setting == 45) SetBackground = Color.Magenta;
                    else if (setting == 46) SetBackground = Color.Cyan;
                    else if (setting == 47) SetBackground = Color.White;
                    else if (setting == 49) SetBackground = Color.Black;
                }
            }
            else
            {
                //Set all to defaults
                Bold = false; Italicized = false; DefaultColor = Color.White; Underline = false; SetBackground = Color.Black;
            }

        }

        private void SetScrollSize(string command)
        {
            //Set screen to specified size
            if (command.Contains(";"))
            {
                ScreenBottom = Convert.ToInt32(command.Substring(command.IndexOf(';') + 1, command.IndexOf('r') - 1 - command.IndexOf(';'))) - 1;
                ScreenTop = Convert.ToInt32(command.Substring(2, command.IndexOf(';') - 2)) - 1;
                ScreenHeight = Convert.ToInt32(command.Substring(command.IndexOf(';') + 1, command.IndexOf('r') - 1 - command.IndexOf(';')));
            }
            else
            {
                //Set screen size to size of the window
                int height = TXT_terminal.Size.Height;
                int width = TXT_terminal.Size.Width;
                ScreenHeight = height / 17;
                ScreenWidth = width / 9;
                ScreenBottom = ScreenHeight - 1;
                ScreenTop = 0;
            }
        }

        //Delete given number of lines 
        private void DeleteLines(string command)
        {
            int delNum = 1;
            if (Regex.IsMatch(command, @"\e\[\d+M"))
            {
                delNum = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2)));
            }
            int Max = ScreenTop + delNum;
            int TopLine = ScreenTop;
            //Move anylines already on the screen up to the top
            for (int i = Max; i < ScreenBottom; i++)
            {
                TXT_terminal.Select(TXT_terminal.GetFirstCharIndexFromLine(i), TXT_terminal.Lines[i].Length);
                string SavedText = TXT_terminal.SelectedRtf;
                TXT_terminal.SelectedText = "\0";
                TXT_terminal.Select(TXT_terminal.GetFirstCharIndexFromLine(TopLine), TXT_terminal.Lines[TopLine].Length);
                TXT_terminal.SelectedRtf = SavedText;
                TopLine++;

            }
            //Replace all remaining lines being deleted with nothing
            for (int i = TopLine; i < Max; i++)
            {
                int firstchar = TXT_terminal.GetFirstCharIndexFromLine(i);
                int length = TXT_terminal.Lines[i].Length;
                TXT_terminal.Select(firstchar, length);
                TXT_terminal.SelectedText = "\0";
            }
        }

        //Delete given number of characters and replace with nothing
        private void DeleteChars(string command)
        {
            int delNum = 1;
            if (Regex.IsMatch(command, @"\e\[\d+P"))
            {
                delNum = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2)));
            }
            int index = TXT_terminal.SelectionStart;
            int currentLine = TXT_terminal.GetLineFromCharIndex(index);
            TXT_terminal.Select(index, delNum);
            TXT_terminal.SelectedText = "";
        }

        //Scroll Up given number of lines, default is 1
        private void ScrollUp(string command)
        {
            int scrollNum = 1;
            if (Regex.IsMatch(command, @"\e\[\d+S"))
            {
                scrollNum = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2)));
            }
            int LastIndex = TXT_terminal.GetFirstCharIndexFromLine(ScreenBottom + 1);
            int RemovalLine = scrollNum + ScreenTop;
            //Get index of start of scroll
            int IntitialScrollIndex = TXT_terminal.GetFirstCharIndexFromLine(RemovalLine);
            //Select replacement Text
            TXT_terminal.Select(IntitialScrollIndex, LastIndex - IntitialScrollIndex);
            string replacementRtf = TXT_terminal.SelectedRtf;
            string replacementText = TXT_terminal.SelectedText;
            //Remove all data on the screen
            for (int i = ScreenTop; i <= ScreenBottom; i++)
            {
                int firstchar = TXT_terminal.GetFirstCharIndexFromLine(i);
                int length = TXT_terminal.Lines[i].Length;
                TXT_terminal.Select(firstchar, length);
                TXT_terminal.SelectedText = "\0";
            }
            //Select Position for scrolled data and paste replacment text here
            CursorToLine(ScreenTop);
            TXT_terminal.Select(TXT_terminal.GetFirstCharIndexFromLine(ScreenTop), TXT_terminal.GetFirstCharIndexFromLine(RemovalLine) - TXT_terminal.GetFirstCharIndexFromLine(ScreenTop));
            TXT_terminal.SelectedRtf = replacementRtf;
            if (!replacementText.EndsWith("\n\n"))
            {
                TXT_terminal.Select(TXT_terminal.GetFirstCharIndexFromLine(ScreenBottom), 1);
                TXT_terminal.SelectedText = "\n";
            }
            ArrowMovement = false;
        }

        //Scroll Down given number of lines, default is 1
        private void ScrollDown(string command)
        {
            int scrollNum = 1;
            if (Regex.IsMatch(command, @"\e\[\d+T"))
            {
                scrollNum = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2)));
            }

            int TopIndex = TXT_terminal.GetFirstCharIndexFromLine(ScreenTop);
            int RemovalLine = scrollNum + ScreenTop;
            //Select index for beginning of scroll
            int IntitialScrollIndex = TXT_terminal.GetFirstCharIndexFromLine(RemovalLine);
            //Select replacement text and save it
            TXT_terminal.Select(TopIndex, IntitialScrollIndex - TopIndex);
            string replacementText = TXT_terminal.SelectedRtf;
            //Remove all data on the screen
            for (int i = ScreenTop; i <= ScreenBottom; i++)
            {
                int firstchar = TXT_terminal.GetFirstCharIndexFromLine(i);
                int length = TXT_terminal.Lines[i].Length;
                TXT_terminal.Select(firstchar, length);
                TXT_terminal.SelectedText = "\0";
            }
            //Select Position for scrolled data and paste replacment text here
            CursorToLine(ScreenTop);
            TXT_terminal.Select(TXT_terminal.GetFirstCharIndexFromLine(RemovalLine + 1), TXT_terminal.GetFirstCharIndexFromLine(ScreenBottom) - TXT_terminal.GetFirstCharIndexFromLine(RemovalLine));
            TXT_terminal.SelectedRtf = replacementText;
            ArrowMovement = false;
        }

        //Remove characters and replace them with the blank characters
        private void RemoveChars(string command)
        {
            int delNum = 1;
            if (Regex.IsMatch(command, @"\e\[\d+X"))
            {
                delNum = Convert.ToInt32((command.Substring(command.IndexOf('[') + 1, command.Length - command.IndexOf('[') - 2)));
            }
            int index = TXT_terminal.SelectionStart;
            int currentLine = TXT_terminal.GetLineFromCharIndex(index);
            TXT_terminal.Select(index, delNum);
            TXT_terminal.SelectedText = new string(' ', delNum);
        }

        private void SaveCursor()
        {
            int index = TXT_terminal.SelectionStart;
            CursorPosition = TXT_terminal.GetLineFromCharIndex(index);
        }

        private void RestoreCursor()
        {
            CursorToLine(CursorPosition);
        }

        //Save screen before changing to Alternative Screen Buffer
        private void SaveScreen()
        {
            TXT_terminal.Select(0, TXT_terminal.TextLength);
            AlternativeScreenData = TXT_terminal.SelectedRtf;
            TXT_terminal.Clear();
        }

        //Restore screen after changing from Alternative Screen Buffer
        private void RestoreScreen()
        {
            TXT_terminal.Clear();
            TXT_terminal.Select(0, TXT_terminal.TextLength);
            TXT_terminal.SelectedRtf = AlternativeScreenData;
        }

        #endregion
    }

}