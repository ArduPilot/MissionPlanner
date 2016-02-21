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
    public partial class LogDownloadMavLink : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        serialstatus status = serialstatus.Connecting;
        int currentlog = 0;
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
                CustomMessageBox.Show("Please Connect");
                return;
            }

            try
            {
                var list = MainV2.comPort.GetLogList();

                foreach (var item in list)
                {
                    genchkcombo(item.id);

                    try
                    {
                        TXT_seriallog.AppendText(item.id + "\t" +
                                                 new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(
                                                     item.time_utc).ToLocalTime() + "\test size:\t" + item.size + "\r\n");
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                }

                if (list.Count == 0)
                {
                    TXT_seriallog.AppendText("No logs to download");
                }
            }
            catch
            {
                CustomMessageBox.Show(Strings.ErrorLogList, Strings.ERROR);
                this.Close();
            }

            status = serialstatus.Done;
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
            if (this.IsDisposed)
                return;

            if (start.Second != DateTime.Now.Second)
            {
                this.BeginInvoke((System.Windows.Forms.MethodInvoker) delegate()
                {
                    try
                    {
                        TXT_status.Text = status.ToString() + " " + receivedbytes;
                    }
                    catch
                    {
                    }
                });
                start = DateTime.Now;
            }
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

        string GetLog(ushort no)
        {
            log.Info("GetLog " + no);

            MainV2.comPort.Progress += comPort_Progress;

            status = serialstatus.Reading;

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

                status = serialstatus.Done;
                updateDisplay();

                MainV2.comPort.Progress -= comPort_Progress;

                MAVLink.mavlink_heartbeat_t hb = (MAVLink.mavlink_heartbeat_t) MainV2.comPort.DebugPacket(hbpacket);

                logfile = Settings.Instance.LogDir + Path.DirectorySeparatorChar
                          + MainV2.comPort.MAV.aptype.ToString() + Path.DirectorySeparatorChar
                          + hbpacket[3] + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss") +
                          " " +
                          no + ".bin";

                // make log dir
                Directory.CreateDirectory(Path.GetDirectoryName(logfile));

                log.Info("about to write: " + logfile);
                // save memorystream to file
                using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(logfile)))
                {
                    byte[] buffer = new byte[256*1024];
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

            log.Info("about to GetFirstGpsTime: " + logfile);
            // get gps time of assci log
            DateTime logtime = new DFLog().GetFirstGpsTime(logfile);

            // rename log is we have a valid gps time
            if (logtime != DateTime.MinValue)
            {
                string newlogfilename = Settings.Instance.LogDir + Path.DirectorySeparatorChar
                                        + MainV2.comPort.MAV.aptype.ToString() + Path.DirectorySeparatorChar
                                        + hbpacket[3] + Path.DirectorySeparatorChar +
                                        logtime.ToString("yyyy-MM-dd HH-mm-ss") + ".log";
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

            return logfile;
        }

        void comPort_Progress(int progress, string status)
        {
            receivedbytes = progress;
            updateDisplay();
        }

        void CreateLog(string logfile)
        {
            TextReader tr = new StreamReader(logfile);
            //

            this.Invoke(
                (System.Windows.Forms.MethodInvoker)
                    delegate() { TXT_seriallog.AppendText("Creating KML for " + logfile + "\n"); });

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
            updateDisplay();
        }

        private void downloadthread(int startlognum, int endlognum)
        {
            try
            {
                for (int a = startlognum; a <= endlognum; a++)
                {
                    currentlog = a;

                    var logname = GetLog((ushort) a);

                    CreateLog(logname);

                    if (chk_droneshare.Checked)
                    {
                        try
                        {
                            Utilities.DroneApi.droneshare.doUpload(logname);
                        }
                        catch (Exception ex)
                        {
                            CustomMessageBox.Show("Droneshare upload failed " + ex.ToString());
                        }
                    }
                }

                status = serialstatus.Done;
                updateDisplay();

                Console.Beep();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.Message, "Error in log " + currentlog);
            }
        }

        private void downloadsinglethread()
        {
            try
            {
                for (int i = 0; i < CHK_logs.CheckedItems.Count; ++i)
                {
                    int a = (int) CHK_logs.CheckedItems[i];

                    currentlog = a;

                    var logname = GetLog((ushort) a);

                    CreateLog(logname);

                    if (chk_droneshare.Checked)
                    {
                        try
                        {
                            Utilities.DroneApi.droneshare.doUpload(logname);
                        }
                        catch (Exception ex)
                        {
                            CustomMessageBox.Show("Droneshare upload failed " + ex.ToString());
                        }
                    }
                }

                status = serialstatus.Done;
                updateDisplay();

                Console.Beep();
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.Message, "Error in log " + currentlog);
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
            if (CustomMessageBox.Show("Are you sure?", "sure", MessageBoxButtons.YesNo) ==
                System.Windows.Forms.DialogResult.Yes)
            {
                try
                {
                    MainV2.comPort.EraseLog();
                    TXT_seriallog.AppendText("!!Allow 30-90 seconds for erase\n");
                    status = serialstatus.Done;
                    updateDisplay();
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