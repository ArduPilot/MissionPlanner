using log4net;
using MissionPlanner.ArduPilot;
using MissionPlanner.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace MissionPlanner.GCSViews
{
    public partial class ConfigREPL : MyUserControl, IActivate, IDeactivate
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly List<string> cmdHistory = new List<string>();
        private readonly object thisLock = new object();
        private int history;
        private int inputStartPos;
        DateTime lastsend = DateTime.MinValue;
        private AP_REPL AP_REPL;

        public ConfigREPL()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            AP_REPL = new AP_REPL(MainV2.comPort);
            AP_REPL.NewResponse += (sender, s) =>
            {
                try
                {
                    addText(s);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            };
        }

        public void Deactivate()
        {
            AP_REPL.Stop();
        }


        private void addText(string data)
        {
            BeginInvoke((MethodInvoker)delegate
           {
               if (this.Disposing)
                   return;

               if (inputStartPos > TXT_terminal.Text.Length)
                   inputStartPos = TXT_terminal.Text.Length - 1;

               if (inputStartPos == -1)
                   inputStartPos = 0;

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
               TXT_terminal.AppendText(currenttypedtext);
           });
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

        private void TXT_terminal_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                if (true)
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

                            cmd = cmd.TrimEnd(new[] { '\r', '\n' }).TrimEnd(new[] { '\r', '\n' });

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
                            AP_REPL.Write(Encoding.ASCII.GetBytes(cmd), 0, cmd.Length);
                            lastsend = DateTime.Now;
                        }
                        else
                        {
                            AP_REPL.Write(Encoding.ASCII.GetBytes(cmd + "\n"), 0, cmd.Length + 1);
                            lastsend = DateTime.Now;

                            //local echo
                            addText(cmd + "\n");
                        }
                    }
                    catch
                    {
                        CustomMessageBox.Show(Strings.ErrorCommunicating, Strings.ERROR);
                    }
                }
            }
        }

        private void BUT_RebootAPM_Click(object sender, EventArgs e)
        {
            TXT_terminal.ReadOnly = false;
            TXT_terminal.Clear();
            addText("Starting REPL\n");
            BUT_disconnect.Enabled = true;
            AP_REPL.Start();
        }

        private void BUT_disconnect_Click(object sender, EventArgs e)
        {
            AP_REPL.Stop();
            TXT_terminal.ReadOnly = true;
        }
    }
}