using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using log4net;
using MissionPlanner.Utilities;
using System.Diagnostics;
using MissionPlanner.Controls;
using Renci.SshNet;
using Renci.SshNet.Sftp;

namespace MissionPlanner.Log
{
    public partial class LogDownloadscp : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        SerialStatus status = SerialStatus.Connecting;
        bool closed;
        string logfile = "";
        uint receivedbytes; // current log file
        uint tallyBytes; // previous downloaded logs
        uint totalBytes; // total expected
        List<SftpFile> logEntries = new List<SftpFile>();
        private ConnectionInfo _connectionInfo;

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

        public LogDownloadscp()
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

        private static void ScpClient_Downloading(object sender, Renci.SshNet.Common.ScpDownloadEventArgs e)
        {
            Console.WriteLine("{0} - {1} {2}", e.Filename, e.Downloaded, (e.Downloaded / (double)e.Size) * 100);
        }

        void LoadLogList()
        {
            string path = Settings.Instance["LogDownloadscppath"];

            if(path == null)
                path = "user:edison@10.0.1.128:/home/user/dflogger/dataflash/";

            //solo - root:TjSDBkAu@10.1.1.10:/log/dataflash/

            InputBox.Show("", "Please enter scp path eg (user:password@host:/dir/path/to/files/)", ref path);

            Uri ur = new Uri("http://"+path);

            Settings.Instance["LogDownloadscppath"] = path;

            CHK_logs.Items.Clear();

            AppendSerialLog(LogStrings.FetchingLogfileList);

            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                try
                {
                    SftpClient sshclient = new SftpClient(ur.Host, ur.UserInfo.Split(':').First(), ur.UserInfo.Split(':').Last());

                    sshclient.Connect();

                    var files = sshclient.ListDirectory(ur.AbsolutePath);

                    foreach (var sftpFile in files)
                    {
                        if (sftpFile.Name.ToLower().EndsWith(".bin"))
                            logEntries.Add(sftpFile);
                    }

                    _connectionInfo = sshclient.ConnectionInfo;

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
                        string caption = item.Name + " " + GetItemCaption(item) + "  (" + item.Length + ")";
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

        string GetItemCaption(SftpFile item)
        {
            return item.LastWriteTime.ToString();
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

        string GetLog(string no, string fileName)
        {
            log.Info("GetLog " + no);

            
            status = SerialStatus.Reading;

                logfile = Settings.Instance.LogDir + Path.DirectorySeparatorChar
                          + MakeValidFileName(fileName) + ".bin";

                // make log dir
                Directory.CreateDirectory(Path.GetDirectoryName(logfile));

                log.Info("about to write: " + logfile);
                // save memorystream to file


            SftpClient client = new SftpClient(_connectionInfo);

            client.Connect();

            using (var logstream = File.Open(logfile, FileMode.Create, FileAccess.Write))
            {
                client.DownloadFile(no, logstream, downloadCallback);
            }

            client.Disconnect();

            log.Info("about to convertbin: " + logfile);

            // create ascii log
            BinaryLog.ConvertBin(logfile, logfile + ".log");

            //update the new filename
            logfile = logfile + ".log";

            // rename file if needed
            log.Info("about to GetFirstGpsTime: " + logfile);
            // get gps time of assci log
            DateTime logtime = new DFLog().GetFirstGpsTime(logfile);

            // rename log is we have a valid gps time
            if (logtime != DateTime.MinValue)
            {
                string newlogfilename = Settings.Instance.LogDir + Path.DirectorySeparatorChar
                                        + logtime.ToString("yyyy-MM-dd HH-mm-ss") + ".log";
                try
                {
                    File.Move(logfile, newlogfilename);
                    // rename bin as well
                    File.Move(logfile.Replace(".log", ""), newlogfilename.Replace(".log", ".bin"));
                    logfile = newlogfilename;
                }
                catch
                {
                    CustomMessageBox.Show(Strings.ErrorRenameFile + " " + logfile + "\nto " + newlogfilename,
                        Strings.ERROR);
                }
            }

            MainV2.comPort.Progress -= comPort_Progress;

            return logfile;
        }

        private void downloadCallback(ulong obj)
        {
            comPort_Progress((int)obj, "Downloading");
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
                    (int)System.Windows.Forms.DialogResult.No)
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
                    var entry = logEntries[a]; // mavlink_log_entry_t
                    totalBytes += (uint)entry.Length;
                }

                UpdateProgress(0, totalBytes, 0);
                foreach (int a in selectedLogs)
                {
                    var entry = logEntries[a]; // mavlink_log_entry_t
                    string fileName = GetItemCaption(entry);

                    AppendSerialLog(string.Format(LogStrings.FetchingLog, fileName));

                    var logname = GetLog(entry.FullName, fileName);

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
                AppendSerialLog("Error in log " + ex.Message);
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
                (int)System.Windows.Forms.DialogResult.Yes)
            {
                try
                {

                    SftpClient sshclient = new SftpClient(_connectionInfo);
                    sshclient.Connect();

                    foreach (var logEntry in logEntries)
                    {
                        sshclient.DeleteFile(logEntry.FullName);
                    }

                    sshclient.Disconnect();

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