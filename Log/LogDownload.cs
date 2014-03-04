using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Text.RegularExpressions;
using KMLib;
using KMLib.Feature;
using KMLib.Geometry;
using Core.Geometry;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.Core;
using log4net;
using MissionPlanner.Comms;
using MissionPlanner.Utilities;


namespace MissionPlanner.Log
{
    public partial class LogDownload : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        ICommsSerial comPort;
        StreamWriter sw;
        int currentlog = 0;
        string logfile = "";
        int receivedbytes = 0;
        
        //state control variables
        serialstatus status = serialstatus.Connecting;
        int connect_substate = 0;
        bool threadrun = true;
        bool exitpending = false;
        bool downloading = false;
        System.Threading.Thread t11;
        
        bool lastbuttonstate = true;

        Object thisLock = new Object();
        DateTime start = DateTime.Now;

        enum serialstatus
        {
            Connecting,
            ReceiveListing,
            Createfile,
            Closefile,
            Reading,
            Done,
            Error,
            Erasing
        }

        enum dltype
        {
            All,
            Selected,
            MinusOne
        }

        public LogDownload()
        {
            InitializeComponent();
        }

        private void Log_Load(object sender, EventArgs e)
        {
            log.Info("state ->Connecting\r");
            status = serialstatus.Connecting;
            connect_substate = 0;

            comPort = GCSViews.Terminal.comPort;

            t11 = new System.Threading.Thread(delegate()
            {
                var start = DateTime.Now;
                bool threaderror = false;

                threadrun = true;

                try
                {
                    comPort.Write("exit\rlogs\r"); // more in "connecting"
                }
                catch (Exception ex)
                {
                    log.Info("state ->Error\r");
                    status = serialstatus.Error;
                    log.Error("Error in comport thread " + ex);
                    threaderror = true;
                }

                while (threadrun)
                {
                    try
                    {
                        updateDisplay();

                        System.Threading.Thread.Sleep(1);
                        while (comPort.BytesToRead > 0)
                        {
                            comPort_DataReceived((object)null, (SerialDataReceivedEventArgs)null);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!threaderror)
                        {
                            log.Info("state ->Error\r");
                            status = serialstatus.Error;
                            log.Error("Error in comport thread " + ex);
                            threaderror = true;
                        }
                    } // cant exit unless told to
                }
                log.Info("Comport thread close");
            });
            t11.IsBackground = true;
            t11.Name = "Log serial thread";
            t11.Start();

            // doesnt seem to work on mac
            //comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }

        void genchkcombo(int lognr)
        {
            MethodInvoker m = delegate()
            {
                if (!CHK_logs.Items.Contains(lognr))
                {
                    CHK_logs.Items.Add(lognr);
                }
            };
            try
            {
                BeginInvoke(m);
            }
            catch
            {
            }
        }

        void updateDisplay()
        {
            if (start.Second != DateTime.Now.Second)
            {
                this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                {
                    try
                    {
                        TXT_status.Text = status.ToString() + " " + receivedbytes;
                    }
                    catch { }
                });
                start = DateTime.Now;
            }
        }

        void setButtonState(bool state)
        {
            if (state != lastbuttonstate)
            {
                lastbuttonstate = state;
                this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                {
                    this.BUT_dumpdf.Enabled = state;
                    this.BUT_DLthese.Enabled = state;
                    this.BUT_DLall.Enabled = state;
                    this.BUT_clearlogs.Enabled = state;
                    //this.BUT_bintolog.Enabled = state;
                    //this.BUT_firstperson.Enabled = state;
                    //this.BUT_redokml.Enabled = state;
                });
            }
        }

        void comPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                while (comPort.BytesToRead > 0 && threadrun)
                {
                    updateDisplay();

                    string line = "";

                    comPort.ReadTimeout = 500;
                    try
                    {
                        line = comPort.ReadLine();
                        if (!line.Contains("\n"))
                            line = line + "\n";
                    }
                    catch
                    {
                        line = comPort.ReadExisting();
                    }

                    receivedbytes += line.Length;

                    //string line = Encoding.ASCII.GetString(data, 0, data.Length);

                    switch (status)
                    {
                        case serialstatus.Connecting:
                            if (connect_substate==0 && line.Contains("] logs"))
                            {
                                connect_substate++;
                                break;
                            }
                            if (connect_substate == 1 && line.Contains("Log]"))
                            {
                                connect_substate++;
                            }

                            if (connect_substate == 2)
                            {
                                // clear history
                                this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
                                {
                                    TXT_seriallog.Clear();
                                });

                                log.Info("state ->ReceiveListing\r");
                                status = serialstatus.ReceiveListing;
                            }
                            break;
                        case serialstatus.Done:
                            break;
                        case serialstatus.Closefile:
                            sw.Close();

                            DateTime logtime = DFLog.GetFirstGpsTime(logfile);

                            if (logtime != DateTime.MinValue)
                            {
                                string newlogfilename = MainV2.LogDir + Path.DirectorySeparatorChar + logtime.ToString("yyyy-MM-dd HH-mm-ss") + ".log";
                                try
                                {
                                    File.Move(logfile, newlogfilename);
                                    logfile = newlogfilename;
                                }
                                catch (Exception ex) { CustomMessageBox.Show("Failed to rename file " + logfile + "\nto " + newlogfilename,"Error"); }
                            }

                            CreateLog(logfile);

                            log.Info("state ->ReceiveListing\r");
                            status = serialstatus.ReceiveListing;
                            break;
                        case serialstatus.Createfile:
                            receivedbytes = 0;
                            Directory.CreateDirectory(MainV2.LogDir);
                            logfile = MainV2.LogDir + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " " + currentlog + ".log";
                            sw = new StreamWriter(logfile);

                            log.Info("state ->Reading\r");
                            status = serialstatus.Reading;
                            break;
                        case serialstatus.ReceiveListing:
                            {
                                Regex regex2 = new Regex(@"^Log ([0-9]+)[,\s]", RegexOptions.IgnoreCase);
                                if (regex2.IsMatch(line))
                                {
                                    MatchCollection matchs = regex2.Matches(line);
                                    int lognr = int.Parse(matchs[0].Groups[1].Value);
                                    genchkcombo(lognr);
                                }
                            }
                            if (line.Contains("Log]"))
                            {
                                log.Info("state ->Done\r");
                                status = serialstatus.Done;
                            }
                            break;
                        case serialstatus.Reading:
                            if (line.Contains("packets read") || line.Contains("Done") || line.Contains("logs enabled"))
                            {
                                log.Info("state ->Closefile\r");
                                status = serialstatus.Closefile;
                                break;
                            }
                            sw.Write(line);
                            continue;
                        case serialstatus.Erasing:
                            if (line.Contains("Log]"))
                            {
                                log.Info("state ->ReceiveListing\r");
                                status = serialstatus.ReceiveListing;
                            }
                            break;
                    }

                    setButtonState(status == serialstatus.Done && !downloading);

                    lock (thisLock)
                    {
                        this.BeginInvoke((MethodInvoker)delegate()
                        {
                            Console.Write(line);

                            TXT_seriallog.AppendText(line.Replace((char)0x0,' '));

                            // auto scroll
                            if (TXT_seriallog.TextLength >= 50000)
                            {
                                TXT_seriallog.Text = TXT_seriallog.Text.Substring(TXT_seriallog.TextLength / 2);
                            }

                            TXT_seriallog.SelectionStart = TXT_seriallog.Text.Length;

                            TXT_seriallog.ScrollToCaret();

                            TXT_seriallog.Refresh();
                        });

                    }
                }

            }
            catch (Exception ex)
            {
                //CustomMessageBox.Show("Error reading data" + ex.ToString());
            }
        }

   
        private void Log_FormClosing(object sender, FormClosingEventArgs e)
        {
            //try controlled exit from logview
            DateTime start = DateTime.Now;
            exitpending = true;
            while (status != serialstatus.Done && status != serialstatus.Error)
            {
                System.Threading.Thread.Sleep(10);
                if((DateTime.Now - start).TotalMilliseconds>20000)
                {
                    //forced exit from logview
                    try
                    {
                        comPort.DtrEnable = false;
                    }
                    catch { }
                    try
                    {
                        log.Info("Log forced closing of port");
                        comPort.Close();
                    }
                    catch { }
                    break;
                }
            }
            threadrun = false;
            t11.Join();
            log.Info("Log form closed");
        }

        void CreateLog(string logfile)
        {
            TextReader tr = new StreamReader(logfile);

            this.BeginInvoke((System.Windows.Forms.MethodInvoker)delegate()
            {
                TXT_seriallog.AppendText("Creating KML for " + logfile + "\n");
            });

            LogOutput lo = new LogOutput();

            while (tr.Peek() != -1)
            {
                lo.processLine(tr.ReadLine());
            }

            tr.Close();

            try
            {
                lo.writeKML(logfile + ".kml");
            }
            catch { } // usualy invalid lat long error
        }

        private void BUT_DLall_Click(object sender, EventArgs e)
        {
            if (CHK_logs.Items.Count == 0)
            {
                CustomMessageBox.Show("Nothing to download");
                return;
            }

            System.Threading.Thread t11 = new System.Threading.Thread(delegate()
            {
                downloadthread(dltype.All);
            });
            t11.Name = "Log download All thread";
            t11.Start();
        }

        private void BUT_DLthese_Click(object sender, EventArgs e)
        {
            if (CHK_logs.CheckedItems.Count == 0)
            {
                CustomMessageBox.Show("Nothing to download");
                return;
            }

            System.Threading.Thread t11 = new System.Threading.Thread(delegate()
            {
                downloadthread(dltype.Selected);
            });
            t11.Name = "Log download Selected thread";
            t11.Start();
        }

        private void BUT_dumpdf_Click(object sender, EventArgs e)
        {
            if (CHK_logs.Items.Count == 0)
            {
                CustomMessageBox.Show("Nothing to download");
                return;
            }

            System.Threading.Thread t11 = new System.Threading.Thread(delegate() {
                downloadthread(dltype.MinusOne);
            });
            t11.Name = "Log download MinusOne thread";
            t11.Start();
        }

        private void downloadthread(dltype type)
        {
            try
            {
                downloading = true;

                List<int> items = new List<int>();
                switch(type)
                {
                    case dltype.All:
                        for (int i = 0; i < CHK_logs.Items.Count; ++i)
                            items.Add((int)CHK_logs.Items[i]);
                        break;
                    case dltype.Selected:
                        for (int i = 0; i < CHK_logs.CheckedItems.Count; ++i)
                            items.Add((int)CHK_logs.CheckedItems[i]);
                        break;
                    case dltype.MinusOne:
                        items.Add(-1);
                        break;
                }

                for (int i = 0; i < items.Count; ++i)
                {
                    currentlog = items[i];
                    comPort.Write("dump ");
                    comPort.Write(items[i].ToString() + "\r");

                    log.Info("state ->Createfile\r");
                    status = serialstatus.Createfile;

                    if (i==items.Count-1) downloading = false;

                    while (status != serialstatus.Done && status != serialstatus.Error)
                    {
                        System.Threading.Thread.Sleep(10);
                    }
                    if (exitpending || status == serialstatus.Error)
                    {
                        downloading = false;
                        return;
                    }
                }

                Console.Beep();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.Message, "Error in log " + currentlog);
            }
        }

        private void BUT_clearlogs_Click(object sender, EventArgs e)
        {
            try
            {
                log.Info("state ->Erasing\r");
                status = serialstatus.Erasing;
                comPort.Write("erase\r");
                System.Threading.Thread.Sleep(100);
                TXT_seriallog.AppendText("!!Allow 30-90 seconds for erase\n");
                CHK_logs.Items.Clear();
            }
            catch (Exception ex) { CustomMessageBox.Show(ex.Message, "Error"); }
        }

        private void BUT_redokml_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "*.log|*.log";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = true;
            try
            {
                openFileDialog1.InitialDirectory = MainV2.LogDir + Path.DirectorySeparatorChar;
            }
            catch { } // incase dir doesnt exist

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string logfile in openFileDialog1.FileNames)
                {
                    TXT_seriallog.AppendText("\n\nProcessing " + logfile + "\n");
                    this.Refresh();
                    LogOutput lo = new LogOutput();
                    try
                    {
                        TextReader tr = new StreamReader(logfile);

                        while (tr.Peek() != -1)
                        {
                            lo.processLine(tr.ReadLine());
                        }

                        tr.Close();
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show("Error processing file. Make sure the file is not in use.\n" + ex.ToString());
                    }

                    lo.writeKML(logfile + ".kml");

                    TXT_seriallog.AppendText("Done\n");
                }
            }
        }


        private void BUT_firstperson_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "*.log|*.log";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = true;
            try
            {
                openFileDialog1.InitialDirectory = MainV2.LogDir + Path.DirectorySeparatorChar;
            }
            catch { } // incase dir doesnt exist

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                foreach (string logfile in openFileDialog1.FileNames)
                {
                    TXT_seriallog.AppendText("\n\nProcessing " + logfile + "\n");
                    this.Refresh();

                    LogOutput lo = new LogOutput();

                    try
                    {
                        TextReader tr = new StreamReader(logfile);

                        while (tr.Peek() != -1)
                        {
                            lo.processLine(tr.ReadLine());
                        }

                        tr.Close();
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show("Error processing log. Is it still downloading? " + ex.Message);
                        continue;
                    }

                    lo.writeKMLFirstPerson(logfile + "-fp.kml");

                    TXT_seriallog.AppendText("Done\n");
                }
            }
        }

        private void BUT_bintolog_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Binary Log|*.bin";

            ofd.ShowDialog();

            if (File.Exists(ofd.FileName))
            {
                List<string> log = BinaryLog.ReadLog(ofd.FileName);

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "log|*.log";

                DialogResult res = sfd.ShowDialog();

                if (res == System.Windows.Forms.DialogResult.OK)
                {
                    StreamWriter sw = new StreamWriter(sfd.OpenFile());
                    foreach (string line in log)
                    {
                        sw.Write(line);
                    }
                    sw.Close();
                }
            }
        }

    }
}