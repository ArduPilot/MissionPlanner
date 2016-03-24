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
        int currentlog = 0;
        string logfile = "";
        List<MAVLink.mavlink_log_entry_t> logEntries;

        //List<Model> orientation = new List<Model>();

        Object thisLock = new Object();
        DateTime start = DateTime.Now;

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

            ThemeManager.ApplyThemeTo(this);

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void Log_Load(object sender, EventArgs e)
        {
            if (!MainV2.comPort.BaseStream.IsOpen)
            {
                this.Close();
                CustomMessageBox.Show(LogStrings.NotConnected);
                return;
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

        protected override void OnClosing(CancelEventArgs e)
        {
            if (downloadCancellation != null)
            {
                downloadCancellation.Cancel();
            }
            base.OnClosing(e);
        }

        private void LoadCheckedList()
        {
            if (logEntries != null && logEntries.Count > 0)
            {
                AppendSerialLog(string.Format(LogStrings.LogFilesFound, logEntries.Count));

                foreach (var item in logEntries)
                {
                    string caption = item.id + " " + GetItemCaption(item);
                    AddCheckedListBoxItem(caption);
                    try
                    {
                        AppendSerialLog(item.id + "\t" + GetItemCaption(item) + "\t\t" + item.size);
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }
            }
            else
            {
                AppendSerialLog(LogStrings.NoLogsFound);
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


        void UpdateStatus(bool force)
        {
            if (this.IsDisposed)
                return;

            System.TimeSpan span = DateTime.Now - start;

            if (span.TotalSeconds > 1 || force)
            {
                string caption = status.ToString();
                if (status == SerialStatus.Reading)
                {
                    double percent = ((double)receivedBytes * 100.0) / (double)totalDownload;
                    caption += " " + Math.Round(percent, 2) + "%";
                }

                RunOnUIThread(new Action(() =>
                {
                    LabelStatus.Text = caption;
                    UpdateProgress(0, totalDownload, receivedBytes);
                }));
            }
            start = DateTime.Now;
        }

        void RunOnUIThread(Action a)
        {
            Action wrapped = () =>
            {
                try
                {
                    a();
                }
                catch (Exception e)
                {
                    AppendSerialLog(LogStrings.UnhandledException + e.ToString());
                }
            };
            if (this.InvokeRequired)
            {
                this.BeginInvoke(wrapped);
            }
            else
            {
                wrapped();
            }
        }

        private void BUT_DLall_Click(object sender, EventArgs e)
        {
            if (status == SerialStatus.Done)
            {
                if (CHK_logs.Items.Count == 0)
                {
                    CustomMessageBox.Show(LogStrings.NoLogsFound);
                    return;
                }
                BUT_DLall.Enabled = false;
                BUT_DLthese.Enabled = false;
                int[] toDownload = GetAllLogIndices().ToArray();

                downloadCancellation = new CancellationTokenSource();
                System.Threading.Tasks.Task.Factory.StartNew(() =>
                {
                    DownloadThread(toDownload, downloadCancellation.Token);
                }, downloadCancellation.Token);
            }
        }

        string GetLog(ushort no, string fileName, CancellationToken token)
        {
            log.Info("GetLog " + no);

            MainV2.comPort.Progress += comPort_Progress;

            status = SerialStatus.Reading;

            // used for log fn
            byte[] hbpacket = MainV2.comPort.getHeartBeat(token);

            if (hbpacket != null)
                log.Info("Got hbpacket length: " + hbpacket.Length);

            // get df log from mav
            using (var ms = MainV2.comPort.GetLog(no))
            {
                ms.Seek(0, SeekOrigin.Begin);

                if (ms != null)
                    log.Info("Got Log length: " + ms.Length);

                status = SerialStatus.Done;
                UpdateStatus(true);

                MainV2.comPort.Progress -= comPort_Progress;

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

            return logfile;
        }

        private string MakeValidFileName(string fileName)
        {
            return fileName.Replace('/', '-').Replace('\\', '-').Replace(':', '-').Replace('?', ' ').Replace('"', '\'').Replace('<', '[').Replace('>', ']').Replace('|', ' ');
        }

        void comPort_Progress(int bytesReceived, string status)
        {
            receivedBytes = startOfCurrentFile + (uint)bytesReceived;
            UpdateStatus(false);
        }

        void CreateLog(string logfile)
        {
            TextReader tr = new StreamReader(logfile);
            //
            AppendSerialLog(string.Format(LogStrings.CreatingKmlPrompt, logfile));

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
            UpdateStatus(true);
        }

        private uint totalDownload;
        private uint startOfCurrentFile;
        private uint receivedBytes;
        CancellationTokenSource downloadCancellation;


        private void DownloadThread(int[] selectedLogs, CancellationToken token)
        {
            try
            {

                status = SerialStatus.Reading;

                totalDownload = 0;
                receivedBytes = 0;

                foreach (int a in selectedLogs)
                {
                    var entry = logEntries[a]; // mavlink_log_entry_t
                    totalDownload += entry.size;
                }

                UpdateStatus(true);

                foreach (int a in selectedLogs)
                {
                    if (token.IsCancellationRequested)
                    {
                        return;
                    }
                    currentlog = a;

                    var entry = logEntries[a]; // mavlink_log_entry_t
                    string fileName = GetItemCaption(entry);

                    startOfCurrentFile = receivedBytes;
                    var logname = GetLog((ushort)a, fileName, token);

                    CreateLog(logname);

                    UpdateStatus(false);
                }

                receivedBytes = totalDownload;
                UpdateStatus(true);

                AppendSerialLog("Download complete.");
                Console.Beep();
            }
            catch (Exception ex)
            {
                AppendSerialLog("Error in log " + currentlog);
                CustomMessageBox.Show(ex.Message, "Error in log " + currentlog);
            }

            RunOnUIThread(() =>
                {
                    BUT_DLall.Enabled = true;
                    BUT_DLthese.Enabled = true;
                    status = SerialStatus.Done;
                    UpdateStatus(true);
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
            // pin the value so it is inside min/max range.
            if (current > max)
            {
                current = max;
            }
            if (current < min)
            {
                current = min;
            }

            RunOnUIThread(() =>
            {
                progressBar1.Minimum = (int)min;
                progressBar1.Maximum = (int)max;
                progressBar1.Value = (int)current;
                progressBar1.Visible = (current < max);
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
                    downloadCancellation = new CancellationTokenSource();
                    System.Threading.Tasks.Task.Factory.StartNew(() =>
                    {
                        DownloadThread(toDownload, downloadCancellation.Token);
                    }, downloadCancellation.Token);
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
                    UpdateStatus(true);
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
                            CustomMessageBox.Show(LogStrings.ErrorProcessingLogfile + Environment.NewLine +
                                                  ex.ToString());
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
                            CustomMessageBox.Show(LogStrings.ErrorProcessingLogfile + Environment.NewLine + ex.Message);
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

        private void chk_droneshare_CheckedChanged(object sender, EventArgs e)
        {
        }
    }
}