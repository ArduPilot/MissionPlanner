using log4net;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MissionPlanner.Log
{
    public partial class LogDownloadMavLink : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        SerialStatus status = SerialStatus.Connecting;
        bool closed;
        string logfile = "";
        uint receivedbytes; // current log file
        uint tallyBytes; // previous downloaded logs
        uint totalBytes; // total expected
        List<MAVLink.mavlink_log_entry_t> logEntries;
        CancellationTokenSource downloadCts;

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

            if (MainV2.comPort.MAV.cs.armed)
                CustomMessageBox.Show("Please disarm the drone before downloading logs!", Strings.ERROR);
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
                        string caption = item.id + " " + GetItemCaption(item) + "  (" + MissionPlanner.Controls.ConnectionStats.ToHumanReadableByteCount((int)item.size) + ")";
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
            if (closed || this.IsDisposed)
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

                // the previous download (if any) has finished - the buttons gate on that
                downloadCts?.Dispose();
                downloadCts = new CancellationTokenSource();
                var cancel = downloadCts.Token;
                System.Threading.Thread t11 =
                    new System.Threading.Thread(
                        delegate ()
                        {
                            DownloadThread(toDownload, cancel);
                        })
                    {
                        Name = "Log Download All thread"
                    };
                t11.Start();
            }
        }

        async Task<string> GetLog(MAVLink.mavlink_log_entry_t entry, CancellationToken cancel)
        {
            log.Info("GetLog " + entry.id);

            MainV2.comPort.Progress += ComPort_Progress;
            try
            {
                return await GetLogUnsubscribed(entry, cancel).ConfigureAwait(false);
            }
            finally
            {
                // always drop the handler, also when the download throws or is
                // canceled - a leaked handler would double-count progress on
                // the next download
                MainV2.comPort.Progress -= ComPort_Progress;
            }
        }

        async Task<string> GetLogUnsubscribed(MAVLink.mavlink_log_entry_t entry, CancellationToken cancel)
        {
            status = SerialStatus.Reading;

            // get df log from mav
            var fn = await MainV2.comPort.GetLog(MainV2.comPort.MAV.sysid, MainV2.comPort.MAV.compid, entry.id, cancel)
                .ConfigureAwait(false);

            status = SerialStatus.Done;

            logfile = Settings.Instance.LogDir + Path.DirectorySeparatorChar
                                               + MainV2.comPort.MAV.aptype.ToString() + Path.DirectorySeparatorChar
                                               + MainV2.comPort.MAV.sysid + Path.DirectorySeparatorChar + entry.id + " " +
                                               MakeValidFileName(GetItemCaption(entry)) + ".bin";

            // make log dir
            Directory.CreateDirectory(Path.GetDirectoryName(logfile));

            log.Info("about to move " + fn + " to: " + logfile);
            try
            {
                File.Move(fn, logfile);
            }
            catch
            {
                CustomMessageBox.Show(Strings.ErrorRenameFile + " " + fn + "\nto " + logfile,
                    Strings.ERROR);
            }

            // rename file if needed
            // LOG_ENTRY already carries the log start time - only fall back to a full scan
            // of the fresh download when the vehicle reported no valid time
            DateTime logtime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(entry.time_utc).ToLocalTime();
            if (logtime.Year < 1990)
            {
                log.Info("about to GetFirstGpsTime: " + logfile);
                // scan the downloaded log for its first gps time
                var dflb = new DFLogBuffer(logfile);
                logtime = dflb.dflog.gpsstarttime;
                dflb.Clear();
            }

            // rename log fs we have a valid gps time, logtime is after 1990-01-01, since some GPS does not use Unix epoch for invalid time.
            if (logtime.Year >= 1990)
            {
                string newlogfilename = Settings.Instance.LogDir + Path.DirectorySeparatorChar
                                                                 + MainV2.comPort.MAV.aptype.ToString() +
                                                                 Path.DirectorySeparatorChar
                                                                 + MainV2.comPort.MAV.sysid +
                                                                 Path.DirectorySeparatorChar +
                                                                 logtime.ToString("yyyy-MM-dd HH-mm-ss") + ".bin";
                try
                {
                    File.Move(logfile, newlogfilename);
                    logfile = newlogfilename;
                }
                catch
                {
                    CustomMessageBox.Show(Strings.ErrorRenameFile + " " + logfile + "\nto " + newlogfilename,
                        Strings.ERROR);
                }
            }

            return logfile;
        }

        protected override void OnClosed(EventArgs e)
        {
            this.closed = true;
            CancelDownload();
            MainV2.comPort.Progress -= ComPort_Progress;

            base.OnClosed(e);
        }

        void CancelDownload()
        {
            try
            {
                downloadCts?.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // the download finished and disposed the source just as we canceled
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (status == SerialStatus.Reading)
            {
                if (CustomMessageBox.Show(LogStrings.CancelDownload, "Cancel Download", MessageBoxButtons.YesNo) ==
                    (int)System.Windows.Forms.DialogResult.No)
                {
                    e.Cancel = true;
                    return;
                }

                // actually stop the transfer, not just the form
                CancelDownload();
            }

            base.OnClosing(e);
        }

        private string MakeValidFileName(string fileName)
        {
            return fileName.Replace('/', '-').Replace('\\', '-').Replace(':', '-').Replace('?', ' ').Replace('"', '\'').Replace('<', '[').Replace('>', ']').Replace('|', ' ');
        }

        void ComPort_Progress(int progress, string status)
        {
            receivedbytes = (uint)progress;

            UpdateProgress(0, totalBytes, tallyBytes + receivedbytes);
        }

        void CreateKML(string logfile)
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

        private async void DownloadThread(int[] selectedLogs, CancellationToken cancel)
        {
            try
            {
                status = SerialStatus.Reading;

                totalBytes = 0;
                tallyBytes = 0;
                receivedbytes = 0;
                foreach (int a in selectedLogs)
                {
                    var entry = logEntries[a]; // mavlink_log_entry_t
                    totalBytes += entry.size;
                }

                UpdateProgress(0, totalBytes, 0);
                foreach (int a in selectedLogs)
                {
                    var entry = logEntries[a]; // mavlink_log_entry_t

                    AppendSerialLog(string.Format(LogStrings.FetchingLog, GetItemCaption(entry)));

                    await GetLog(entry, cancel).ConfigureAwait(false);

                    tallyBytes += receivedbytes;
                    receivedbytes = 0;
                    UpdateProgress(0, totalBytes, tallyBytes);
                }

                UpdateProgress(0, totalBytes, totalBytes);

                AppendSerialLog("Download complete.");
                Console.Beep();
            }
            catch (OperationCanceledException)
            {
                AppendSerialLog("Download canceled.");
            }
            catch (Exception ex)
            {
                AppendSerialLog("Error in log " + ex.Message);
            }
            finally
            {
                // this download owns the token source - release it before the
                // buttons re-arm; Cancel racing this from OnClosing is handled there
                Interlocked.Exchange(ref downloadCts, null)?.Dispose();

                RunOnUIThread(() =>
                {
                    BUT_DLall.Enabled = true;
                    BUT_DLthese.Enabled = true;
                    status = SerialStatus.Done;
                });
            }
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

        DateTime start = DateTime.Now;

        private void UpdateProgress(uint min, uint max, uint current)
        {
            RunOnUIThread(() =>
            {
                // scale to 0-1000 so byte counts beyond int.MaxValue don't overflow the
                // ProgressBar; clamp because the sender may deliver more bytes than the
                // LOG_ENTRY size it reported
                progressBar1.Minimum = 0;
                progressBar1.Maximum = 1000;
                progressBar1.Value = max == 0 ? 0 : (int)Math.Min(1000.0, current * 1000.0 / max);
                progressBar1.Visible = (current < max);

                if (current == 0)
                    start = DateTime.Now;

                if (current > 0 && current < max)
                {
                    var per = (current / (double)max) * 100;

                    var elapsed = DateTime.Now - start;
                    if (elapsed.TotalSeconds == 0)
                        elapsed = TimeSpan.FromSeconds(1);
                    var avgbps = current / elapsed.TotalSeconds;
                    if (avgbps == 0)
                        avgbps = 1;
                    var left = max - current;
                    var eta = DateTime.Now.AddSeconds(left / avgbps);
                    var remaining = new DateTime().AddSeconds(left / avgbps);
                    labelBytes.Text = MissionPlanner.Controls.ConnectionStats.ToHumanReadableByteCount((int)Math.Min(current, int.MaxValue)) + " "
                    + per.ToString("N1") + "% "
                    + MissionPlanner.Controls.ConnectionStats.ToHumanReadableByteCount((int)avgbps) + "/s "
                    + (remaining.Day > 1 || remaining.Hour > 0 ? ((remaining.Day - 1) * 24 + remaining.Hour).ToString() + ":" : "") + remaining.ToString("mm:ss") + " left";
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
                    // the previous download (if any) has finished - the buttons gate on that
                    downloadCts?.Dispose();
                    downloadCts = new CancellationTokenSource();
                    var cancel = downloadCts.Token;
                    System.Threading.Thread t11 = new System.Threading.Thread(delegate () { DownloadThread(toDownload, cancel); })
                    {
                        Name = "Log download single thread"
                    };
                    t11.Start();
                }
            }
        }

        private void BUT_clearlogs_Click(object sender, EventArgs e)
        {
            if (CustomMessageBox.Show(LogStrings.Confirmation, "sure", MessageBoxButtons.YesNo) ==
                (int)System.Windows.Forms.DialogResult.Yes)
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
                            using (TextReader tr = new StreamReader(logfile))
                            {
                                while (tr.Peek() != -1)
                                {
                                    lo.processLine(tr.ReadLine());
                                }
                            }
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
                ofd.Filter = "Binary Log|*.bin;*.BIN";

                ofd.ShowDialog();

                if (File.Exists(ofd.FileName))
                {
                    using (SaveFileDialog sfd = new SaveFileDialog())
                    {
                        sfd.Filter = "log|*.log;*.LOG";

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