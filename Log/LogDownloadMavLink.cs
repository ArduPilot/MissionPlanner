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

        int currentlog = 0;
        string logfile = "";
        int receivedbytes = 0;
        
        //state control variables
        serialstatus status = serialstatus.Connecting;

        Object thisLock = new Object();
        DateTime start = DateTime.Now;

        enum serialstatus
        {
            Connecting,
            Createfile,
            Closefile,
            Reading,
            Done
        }

        enum dltype
        {
            All,
            Selected
        }

        public LogDownloadMavLink()
        {
            InitializeComponent();

            ThemeManager.ApplyThemeTo(this);
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

                    TXT_seriallog.AppendText(item.id + " " + new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(item.time_utc).ToLocalTime() + " est size: " + item.size +"\n");
                }
            }
            catch { CustomMessageBox.Show("Cannot get log list.","Error"); this.Close(); }

            status = serialstatus.Done;
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
            if (this.IsDisposed)
                return;

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
      
        string GetLog(ushort no)
        {
            MainV2.comPort.Progress += comPort_Progress;

            status = serialstatus.Reading;

            // get df log from mav
            var ms = MainV2.comPort.GetLog(no);

            status = serialstatus.Done;

            MainV2.comPort.Progress -= comPort_Progress;

            // set log fn
            byte[] hbpacket = MainV2.comPort.getHeartBeat();

            MAVLink.mavlink_heartbeat_t hb = (MAVLink.mavlink_heartbeat_t)MainV2.comPort.DebugPacket(hbpacket);

            logfile = MainV2.LogDir + Path.DirectorySeparatorChar
             + MainV2.comPort.MAV.aptype.ToString() + Path.DirectorySeparatorChar
             + hbpacket[3] + Path.DirectorySeparatorChar + DateTime.Now.ToString("yyyy-MM-dd HH-mm") + " " + no + ".bin";

            // make log dir
            Directory.CreateDirectory(Path.GetDirectoryName(logfile));

            // save memorystream to file
            using (BinaryWriter bw = new BinaryWriter(File.OpenWrite(logfile)))
            {
                bw.Write(ms.ToArray());
            }

            // read binary log to assci log
            var temp1 = Log.BinaryLog.ReadLog(logfile);

            // delete binary log file
            //File.Delete(logfile);

            logfile = logfile + ".log";

            // write assci log
            File.WriteAllLines(logfile, temp1);

            // get gps time of assci log
            DateTime logtime = DFLog.GetFirstGpsTime(logfile);

            // rename log is we have a valid gps time
            if (logtime != DateTime.MinValue)
            {
                string newlogfilename = MainV2.LogDir + Path.DirectorySeparatorChar
             + MainV2.comPort.MAV.aptype.ToString() + Path.DirectorySeparatorChar
             + hbpacket[3] + Path.DirectorySeparatorChar + logtime.ToString("yyyy-MM-dd HH-mm") + ".log";
                try
                {
                    File.Move(logfile, newlogfilename);
                    // rename bin as well
                    File.Move(logfile.Replace(".log", ""), newlogfilename.Replace(".log", ".bin"));
                    logfile = newlogfilename;
                }
                catch (Exception ex) { CustomMessageBox.Show("Failed to rename file " + logfile + "\nto " + newlogfilename, "Error"); }
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

            this.Invoke((System.Windows.Forms.MethodInvoker)delegate()
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
            if (status == serialstatus.Done)
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
                t11.Name = "Log Download All thread";
                t11.Start();
            }
        }

        private void BUT_DLthese_Click(object sender, EventArgs e)
        {
            if (status == serialstatus.Done)
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
        }

        private void downloadthread(dltype type)
        {
            try
            {
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
                }

                for (int i = 0; i < items.Count; ++i)
                {
                    currentlog = items[i];

                    var logname = GetLog((ushort)currentlog);

                    CreateLog(logname);
                }
                status = serialstatus.Done;

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
                MainV2.comPort.EraseLog();
                TXT_seriallog.AppendText("!!Allow 30-90 seconds for erase\n");
                status = serialstatus.Done;
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