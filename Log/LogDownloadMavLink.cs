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
using System.Linq;
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
using System.Diagnostics;
using System.Threading;

namespace MissionPlanner.Log
{
    public partial class LogDownloadMavLink : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        SerialStatus status = SerialStatus.Connecting;
        int currentlog;
        bool closed;
        string logfile = "";
        uint receivedbytes; // current log file
        uint tallyBytes; // previous downloaded logs
        uint totalBytes; // total expected
        List<MAVLink.mavlink_log_entry_t> logEntries;
        ToolTip toolTip;

        //List<Model> orientation = new List<Model>();

        Object thisLock = new Object();

        enum SerialStatus
        {
            Connecting,
            Createfile,
            Closefile,
            Reading,
            Waiting,
            Done
        }

        public LogDownloadMavLink()
        {
            InitializeComponent();

            labelBytes.Text = "";

            ThemeManager.ApplyThemeTo(this);

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);            
        }

        private void Log_Load(object sender, EventArgs e)
        {
            LoadLogList();
        }

        void LoadLogList()
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                AppendSerialLog(LogStrings.NotConnected);
                BUT_clearlogs.Enabled = false;
                return;
            }
            else
            {
                BUT_clearlogs.Enabled = true;
            }

            CHK_logs.Items.Clear();

            AppendSerialLog(LogStrings.FetchingLogfileList);

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    this.logEntries = MainV2.comPort.GetLogList();
                    RunOnUIThread(LoadCheckedList);
                }
                catch (Exception ex)
                {
                    AppendSerialLog(LogStrings.UnhandledException + ex.ToString());
                }

            });
        }

        private void LoadCheckedList()
        {
            if (logEntries != null)
            {
                foreach (var item in logEntries)
                {
                    try
                    {
                        string caption = item.id + " " + GetItemCaption(item) + "  (" + item.size + ")";
                        AddCheckedListBoxItem(caption);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }

                if (logEntries.Count == 0)
                {
                    AppendSerialLog(LogStrings.NoLogsFound);
                }
                else
                {
                    AppendSerialLog(string.Format(LogStrings.SomeLogsFound, logEntries.Count));

                }
            }
            status = SerialStatus.Done;
        }

        string GetItemCaption(MAVLink.mavlink_log_entry_t item)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(item.time_utc).ToLocalTime().ToString();
        }


        void AddCheckedListBoxItem(string caption)
        {
            RunOnUIThread(new Action(() =>
            {
                if (!CHK_logs.Items.Contains(caption))
                {
                    CHK_logs.Items.Add(caption);
                }
            }));
        }


        void RunOnUIThread(Action a)
        {
            if (closed)
            {
                return;
            }
            this.BeginInvoke(new Action(() =>
            {
                try
                {
                    a();
                }
                catch (Exception e)
                {
                    Debug.WriteLine(LogStrings.UnhandledException + e.ToString());
                }
            }));
        }

        private void BUT_DLall_Click(object sender, EventArgs e)
        {
            if (status == SerialStatus.Done)
            {
                if (CHK_logs.Items.Count == 0)
                {
                    // try again...
                    LoadLogList();
                    return;
                }
                BUT_DLall.Enabled = false;
                BUT_DLthese.Enabled = false;
                int[] toDownload = GetAllLogIndices().ToArray();

                try
                {
                    Directory.CreateDirectory(Settings.Instance.LogDir);
                }
                catch (Exception ex)
                {
                    AppendSerialLog(string.Format(LogStrings.LogDirectoryError, Settings.Instance.LogDir) + "\r\n" + ex.Message);
                    return;
                }
                AppendSerialLog(string.Format(LogStrings.DownloadStarting, Settings.Instance.LogDir));

                System.Threading.Thread t11 =
                    new System.Threading.Thread(
                        delegate ()
                        {
                            DownloadThread(toDownload);
                        });
                t11.Name = "Log Download All thread";
                t11.Start();
            }
        }

        string GetLog(ushort no, string fileName)
        {
            log.Info("GetLog " + no);

            MainV2.comPort.Progress += comPort_Progress;

            status = SerialStatus.Reading;

            // used for log fn
            byte[] hbpacket = MainV2.comPort.getHeartBeat();

            if (hbpacket != null)
                log.Info("Got hbpacket length: " + hbpacket.Length);

            // get df log from mav
            using (var ms = MainV2.comPort.GetLog(no))
            {
                ms.Seek(0, SeekOrigin.Begin);

                if (ms != null)
                    log.Info("Got Log length: " + ms.Length);

                status = SerialStatus.Done;

                MAVLink.mavlink_heartbeat_t hb = (MAVLink.mavlink_heartbeat_t)MainV2.comPort.DebugPacket(hbpacket);

                logfile = Settings.Instance.LogDir + Path.DirectorySeparatorChar
                          + MainV2.comPort.MAV.aptype.ToString() + Path.DirectorySeparatorChar
                          + hbpacket[3] + Path.DirectorySeparatorChar + no + " " + MakeValidFileName(fileName) + ".bin";

                // make log dir
                Directory.CreateDirectory(Path.GetDirectoryName(logfile));

                log.Info("about to write: " + logfile);
                // save memorystream to file
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(logfile)))
                {
                    byte[] buffer = new byte[256 * 1024];
                    while (ms.Position < ms.Length)
                    {
                        int read = ms.Read(buffer, 0, buffer.Length);
                        bw.Write(buffer, 0, read);
                    }
                }
            }

            log.Info("about to convertbin: " + logfile);

            // create ascii log
            BinaryLog.ConvertBin(logfile, logfile + ".log");

            //update the new filename
            logfile = logfile + ".log";

            MainV2.comPort.Progress -= comPort_Progress;

            return logfile;
        }

        protected override void OnClosed(EventArgs e)
        {
            this.closed = true;
            MainV2.comPort.Progress -= comPort_Progress;

            base.OnClosed(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (status == SerialStatus.Reading)
            {
                if (CustomMessageBox.Show(LogStrings.CancelDownload, "Cancel Download", MessageBoxButtons.YesNo) ==
                    System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }
            }

            base.OnClosing(e);
        }

        private string MakeValidFileName(string fileName)
        {
            return fileName.Replace('/', '-').Replace('\\', '-').Replace(':', '-').Replace('?', ' ').Replace('"', '\'').Replace('<', '[').Replace('>', ']').Replace('|', ' ');
        }

        void comPort_Progress(int progress, string status)
        {
            receivedbytes = (uint)progress;

            UpdateProgress(0, totalBytes, tallyBytes + receivedbytes);
        }

        void CreateLog(string logfile)
        {
            TextReader tr = new StreamReader(logfile);
            //
            AppendSerialLog(string.Format(LogStrings.CreatingKmlPrompt, Path.GetFileName(logfile)));

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
            status = SerialStatus.Done;
        }

        private void DownloadThread(int[] selectedLogs)
        {
            try
            {

                status = SerialStatus.Reading;

                totalBytes = 0;
                tallyBytes = 0;
                receivedbytes = 0;
                foreach (int a in selectedLogs)
                {
                    currentlog = a;
                    var entry = logEntries[a]; // mavlink_log_entry_t
                    totalBytes += entry.size;
                }

                UpdateProgress(0, totalBytes, 0);
                foreach (int a in selectedLogs)
                {
                    currentlog = a;

                    var entry = logEntries[a]; // mavlink_log_entry_t
                    string fileName = GetItemCaption(entry);

                    AppendSerialLog(string.Format(LogStrings.FetchingLog, fileName));

                    var logname = GetLog(entry.id, fileName);

                    CreateLog(logname);

                    tallyBytes += receivedbytes;
                    receivedbytes = 0;
                    UpdateProgress(0, totalBytes, tallyBytes);
                }

                UpdateProgress(0, totalBytes, totalBytes);

                AppendSerialLog("Download complete.");
                Console.Beep();
            }
            catch (Exception ex)
            {
                AppendSerialLog("Error in log " + currentlog + ": " + ex.Message);
            }

            RunOnUIThread(() =>
                {
                    BUT_DLall.Enabled = true;
                    BUT_DLthese.Enabled = true;
                    status = SerialStatus.Done;
                });
        }

        IEnumerable<int> GetSelectedLogIndices()
        {
            foreach (int i in CHK_logs.CheckedIndices)
            {
                yield return i;
            }
        }

        IEnumerable<int> GetAllLogIndices()
        {
            for (int i = 0, n = logEntries.Count; i < n; i++)
            {
                yield return i;
            }
        }

        private void UpdateProgress(uint min, uint max, uint current)
        {
            RunOnUIThread(() =>
            {
                progressBar1.Minimum = (int)min;
                progressBar1.Maximum = (int)max;
                progressBar1.Value = (int)current;
                progressBar1.Visible = (current < max);

                if (current < max)
                {
                    labelBytes.Text = current.ToString();
                }
                else
                {
                    labelBytes.Text = "";
                }
            });

        }

        private void BUT_DLthese_Click(object sender, EventArgs e)
        {
            if (status == SerialStatus.Done)
            {
                int[] toDownload = GetSelectedLogIndices().ToArray();
                if (toDownload.Length == 0)
                {
                    AppendSerialLog(LogStrings.NothingSelected);
                }
                else
                {
                    BUT_DLall.Enabled = false;
                    BUT_DLthese.Enabled = false;
                    System.Threading.Thread t11 = new System.Threading.Thread(delegate () { DownloadThread(toDownload); });
                    t11.Name = "Log download single thread";
                    t11.Start();
                }
            }
        }

        private void BUT_clearlogs_Click(object sender, EventArgs e)
        {
            if (CustomMessageBox.Show(LogStrings.Confirmation, "sure", MessageBoxButtons.YesNo) ==
                System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    MainV2.comPort.EraseLog();
                    AppendSerialLog(LogStrings.EraseComplete);
                    status = SerialStatus.Done;
                    CHK_logs.Items.Clear();
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show(ex.Message, Strings.ERROR);
                }
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
                        AppendSerialLog(Environment.NewLine + Environment.NewLine +
                            string.Format(LogStrings.ProcessingLog, logfile));
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
                            AppendSerialLog(LogStrings.ErrorProcessingLogfile + Environment.NewLine + ex.ToString());
                        }

                        lo.writeKML(logfile + ".kml");

                        AppendSerialLog(LogStrings.Done);
                    }
                }
            }
        }

        private void AppendSerialLog(string msg)
        {
            RunOnUIThread(new Action(() =>
            {
                TXT_seriallog.AppendText(msg + Environment.NewLine);
            }));
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
                    Directory.CreateDirectory(Settings.Instance.LogDir);
                    openFileDialog1.InitialDirectory = Settings.Instance.LogDir + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir cannot be created

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (string logfile in openFileDialog1.FileNames)
                    {
                        AppendSerialLog(Environment.NewLine + Environment.NewLine +
                            string.Format(LogStrings.ProcessingLog, logfile));
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
                            AppendSerialLog(LogStrings.ErrorProcessingLogfile + Environment.NewLine + ex.Message);
                            continue;
                        }

                        lo.writeKMLFirstPerson(logfile + "-fp.kml");

                        AppendSerialLog(LogStrings.Done);
                    }
                }
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