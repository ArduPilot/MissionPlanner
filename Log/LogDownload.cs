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
        int logcount = 0;
        serialstatus status = serialstatus.Connecting;
        StreamWriter sw;
        int currentlog = 0;
        bool threadrun = true;
        string logfile = "";
        int receivedbytes = 0;

        //List<Model> orientation = new List<Model>();

        Object thisLock = new Object();
        DateTime start = DateTime.Now;

        enum serialstatus
        {
            Connecting,
            Createfile,
            Closefile,
            Reading,
            Waiting,
            Done
        }

        public LogDownload()
        {
            InitializeComponent();

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void waitandsleep(int time)
        {
            DateTime start = DateTime.Now;

            while ((DateTime.Now - start).TotalMilliseconds < time)
            {
                try
                {
                    if (comPort.BytesToRead > 0)
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
            DateTime start = DateTime.Now;

            while ((DateTime.Now - start).TotalMilliseconds < time)
            {
                try
                {
                    if (comPort.BytesToRead > 0)
                    {
                        comPort_DataReceived((object) null, (SerialDataReceivedEventArgs) null);
                    }
                }
                catch
                {
                    threadrun = false;
                    return;
                }
            }
        }

        private void Log_Load(object sender, EventArgs e)
        {
            status = serialstatus.Connecting;

            comPort = GCSViews.Terminal.comPort;

            try
            {
                Console.WriteLine("Log_load " + comPort.IsOpen);

                if (!comPort.IsOpen)
                    comPort.Open();

                //Console.WriteLine("Log dtr");

                //comPort.toggleDTR();

                Console.WriteLine("Log discard");

                comPort.DiscardInBuffer();

                Console.WriteLine("Log w&sleep");

                try
                {
                    // try provoke a response
                    comPort.Write("\n\n?\r\n\n");
                }
                catch
                {
                }

                // 10 sec
                waitandsleep(10000);
            }
            catch (Exception ex)
            {
                log.Error("Error opening comport", ex);
                CustomMessageBox.Show("Error opening comport");
                return;
            }

            var t11 = new System.Threading.Thread(delegate()
            {
                var start = DateTime.Now;

                threadrun = true;

                if (comPort.IsOpen)
                    readandsleep(100);

                try
                {
                    if (comPort.IsOpen)
                        comPort.Write("\n\n\n\nexit\r\nlogs\r\n"); // more in "connecting"
                }
                catch
                {
                }

                while (threadrun)
                {
                    try
                    {
                        updateDisplay();

                        System.Threading.Thread.Sleep(1);
                        if (!comPort.IsOpen)
                            break;
                        while (comPort.BytesToRead >= 4)
                        {
                            comPort_DataReceived((object) null, (SerialDataReceivedEventArgs) null);
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error("crash in comport reader " + ex);
                    } // cant exit unless told to
                }
                log.Info("Comport thread close");
            }) {Name = "comport reader", IsBackground = true};
            t11.Start();

            // doesnt seem to work on mac
            //comPort.DataReceived += new SerialDataReceivedEventHandler(comPort_DataReceived);
        }

        void genchkcombo(int logcount)
        {
            MethodInvoker m = delegate()
            {
                //CHK_logs.Items.Clear();
                //for (int a = 1; a <= logcount; a++)
                if (!CHK_logs.Items.Contains(logcount))
                {
                    CHK_logs.Items.Add(logcount);
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
                this.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                {
                    try
                    {
                        if (comPort.IsOpen)
                            TXT_status.Text = status.ToString() + " " + receivedbytes + " " + comPort.BytesToRead;
                    }
                    catch
                    {
                    }
                });
                start = DateTime.Now;
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
                        line = comPort.ReadLine(); //readline(comPort);
                        if (!line.Contains("\n"))
                            line = line + "\n";
                    }
                    catch
                    {
                        line = comPort.ReadExisting();
                        //byte[] data = readline(comPort);
                        //line = Encoding.ASCII.GetString(data, 0, data.Length);
                    }

                    receivedbytes += line.Length;

                    //string line = Encoding.ASCII.GetString(data, 0, data.Length);

                    switch (status)
                    {
                        case serialstatus.Connecting:

                            if (line.Contains("ENTER") || line.Contains("GROUND START") || line.Contains("reset to FLY") ||
                                line.Contains("interactive setup") || line.Contains("CLI") || line.Contains("Ardu"))
                            {
                                try
                                {
                                    //System.Threading.Thread.Sleep(200);
                                    //comPort.Write("\n\n\n\n");
                                }
                                catch
                                {
                                }

                                System.Threading.Thread.Sleep(500);

                                // clear history
                                this.Invoke((System.Windows.Forms.MethodInvoker) delegate() { TXT_seriallog.Clear(); });

                                // comPort.Write("logs\r");
                                status = serialstatus.Done;
                            }
                            break;
                        case serialstatus.Closefile:
                            sw.Close();

                            DateTime logtime = new DFLog().GetFirstGpsTime(logfile);

                            if (logtime != DateTime.MinValue)
                            {
                                string newlogfilename = Settings.Instance.LogDir + Path.DirectorySeparatorChar +
                                                        logtime.ToString("yyyy-MM-dd HH-mm-ss") + ".log";
                                try
                                {
                                    File.Move(logfile, newlogfilename);
                                    logfile = newlogfilename;
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex);
                                    CustomMessageBox.Show(
                                        "Failed to rename file " + logfile + "\nto " + newlogfilename, Strings.ERROR);
                                }
                            }

                            TextReader tr = new StreamReader(logfile);

                            //

                            this.Invoke(
                                (System.Windows.Forms.MethodInvoker)
                                    delegate() { TXT_seriallog.AppendText("Creating KML for " + logfile); });

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
                            catch
                            {
                            } // usualy invalid lat long error
                            status = serialstatus.Done;
                            comPort.DiscardInBuffer();
                            break;
                        case serialstatus.Createfile:
                            receivedbytes = 0;
                            Directory.CreateDirectory(Settings.Instance.LogDir);
                            logfile = Settings.Instance.LogDir + Path.DirectorySeparatorChar +
                                      DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") + " " + currentlog + ".log";
                            sw = new StreamWriter(logfile);
                            status = serialstatus.Waiting;
                            lock (thisLock)
                            {
                                this.Invoke((System.Windows.Forms.MethodInvoker) delegate() { TXT_seriallog.Clear(); });
                            }
                            //if (line.Contains("Dumping Log"))
                        {
                            status = serialstatus.Reading;
                        }
                            break;
                        case serialstatus.Done:
                            // 
                            // if (line.Contains("start") && line.Contains("end"))
                        {
                            Regex regex2 = new Regex(@"^Log ([0-9]+)[,\s]", RegexOptions.IgnoreCase);
                            if (regex2.IsMatch(line))
                            {
                                MatchCollection matchs = regex2.Matches(line);
                                logcount = int.Parse(matchs[0].Groups[1].Value);
                                genchkcombo(logcount);
                                //status = serialstatus.Done;
                            }
                        }
                            if (line.Contains("No logs"))
                            {
                                status = serialstatus.Done;
                            }
                            break;
                        case serialstatus.Reading:
                            if (line.Contains("packets read") || line.Contains("Done") || line.Contains("logs enabled"))
                            {
                                status = serialstatus.Closefile;
                                Console.Write("CloseFile: " + line);
                                break;
                            }
                            sw.Write(line);
                            continue;
                        case serialstatus.Waiting:
                            if (line.Contains("Dumping Log") || line.Contains("GPS:") || line.Contains("NTUN:") ||
                                line.Contains("CTUN:") || line.Contains("PM:"))
                            {
                                status = serialstatus.Reading;
                                Console.Write("Reading: " + line);
                            }
                            break;
                    }
                    lock (thisLock)
                    {
                        this.BeginInvoke((MethodInvoker) delegate()
                        {
                            Console.Write(line);

                            TXT_seriallog.AppendText(line.Replace((char) 0x0, ' '));

                            // auto scroll
                            if (TXT_seriallog.TextLength >= 10000)
                            {
                                TXT_seriallog.Text = TXT_seriallog.Text.Substring(TXT_seriallog.TextLength/2);
                            }

                            TXT_seriallog.SelectionStart = TXT_seriallog.Text.Length;

                            TXT_seriallog.ScrollToCaret();

                            TXT_seriallog.Refresh();
                        });
                    }
                }

                //log.Info("exit while");
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Error reading data" + ex.ToString());
            }
        }


        private void Log_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadrun = false;
            System.Threading.Thread.Sleep(500);
            if (comPort.IsOpen)
            {
                comPort.Close();
            }
        }

        private void CHK_logs_Click(object sender, EventArgs e)
        {
            ListBox lb = sender as ListBox;
        }

        private void BUT_DLall_Click(object sender, EventArgs e)
        {
            if (status == serialstatus.Done)
            {
                if (CHK_logs.Items.Count == 0)
                {
                    CustomMessageBox.Show("Nothing to download");
                    return;
                }

                System.Threading.Thread t11 =
                    new System.Threading.Thread(
                        delegate()
                        {
                            downloadthread(int.Parse(CHK_logs.Items[0].ToString()),
                                int.Parse(CHK_logs.Items[CHK_logs.Items.Count - 1].ToString()));
                        });
                t11.Name = "Log Download All thread";
                t11.Start();
            }
        }

        private void downloadthread(int startlognum, int endlognum)
        {
            try
            {
                for (int a = startlognum; a <= endlognum; a++)
                {
                    currentlog = a;
                    System.Threading.Thread.Sleep(1100);
                    comPort.Write("dump ");
                    System.Threading.Thread.Sleep(100);
                    comPort.Write(a.ToString() + "\r");
                    comPort.DiscardInBuffer();
                    status = serialstatus.Createfile;

                    while (status != serialstatus.Done)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                }

                Console.Beep();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.Message, Strings.ERROR);
            }
        }

        private void downloadsinglethread()
        {
            try
            {
                for (int i = 0; i < CHK_logs.CheckedItems.Count; ++i)
                {
                    int a = (int) CHK_logs.CheckedItems[i];
                    {
                        currentlog = a;
                        System.Threading.Thread.Sleep(1100);
                        comPort.Write("dump ");
                        System.Threading.Thread.Sleep(100);
                        comPort.Write(a.ToString() + "\r");
                        comPort.DiscardInBuffer();
                        status = serialstatus.Createfile;

                        while (status != serialstatus.Done)
                        {
                            System.Threading.Thread.Sleep(100);
                        }
                    }
                }

                Console.Beep();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.Message, Strings.ERROR);
            }
        }

        private void BUT_DLthese_Click(object sender, EventArgs e)
        {
            if (status == serialstatus.Done)
            {
                System.Threading.Thread t11 = new System.Threading.Thread(delegate() { downloadsinglethread(); });
                t11.Name = "Log download single thread";
                t11.Start();
            }
        }

        private void BUT_clearlogs_Click(object sender, EventArgs e)
        {
            try
            {
                System.Threading.Thread.Sleep(500);
                comPort.Write("erase\r");
                System.Threading.Thread.Sleep(100);
                TXT_seriallog.AppendText("!!Allow 30-90 seconds for erase\n");
                status = serialstatus.Done;
                CHK_logs.Items.Clear();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.Message, Strings.ERROR);
            }
        }

        private void BUT_redokml_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "*.log|*.log";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                try
                {
                    openFileDialog1.InitialDirectory = Settings.Instance.LogDir + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist

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
                            CustomMessageBox.Show("Error processing file. Make sure the file is not in use.\n" +
                                                  ex.ToString());
                        }

                        lo.writeKML(logfile + ".kml");

                        TXT_seriallog.AppendText("Done\n");
                    }
                }
            }
        }


        private void BUT_firstperson_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "*.log|*.log";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                try
                {
                    openFileDialog1.InitialDirectory = Settings.Instance.LogDir + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist

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
        }


        private void BUT_dumpdf_Click(object sender, EventArgs e)
        {
            if (status == serialstatus.Done)
            {
                // add -1 entry
                CHK_logs.Items.Add(-1, true);

                System.Threading.Thread t11 = new System.Threading.Thread(delegate() { downloadsinglethread(); });
                t11.Name = "Log download single thread";
                t11.Start();
            }
        }

        private void BUT_bintolog_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Binary Log|*.bin";

                ofd.ShowDialog();

                if (File.Exists(ofd.FileName))
                {
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "log|*.log";

                        DialogResult res = sfd.ShowDialog();

                        if (res == System.Windows.Forms.DialogResult.OK)
                        {
                            BinaryLog.ConvertBin(ofd.FileName, sfd.FileName);
                        }
                    }
                }
            }
        }
    }
}