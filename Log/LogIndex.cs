using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BrightIdeasSoftware;
using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;

namespace MissionPlanner.Log
{
    public partial class LogIndex : Form
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        const int MaxPlaybackLogSize = 100000;


        public LogIndex()
        {
            InitializeComponent();

            this.olvColumnImage.Renderer = new NonSelectableRenderer();
        }

        private void LogIndex_Load(object sender, EventArgs e)
        {
            createFileList(Settings.Instance.LogDir);

            System.Threading.ThreadPool.QueueUserWorkItem(queueRunner);
            System.Threading.ThreadPool.QueueUserWorkItem(queueRunner);
            System.Threading.ThreadPool.QueueUserWorkItem(queueRunner);
            System.Threading.ThreadPool.QueueUserWorkItem(queueRunner);
            System.Threading.ThreadPool.QueueUserWorkItem(queueRunner);
            System.Threading.ThreadPool.QueueUserWorkItem(queueRunner);
            System.Threading.ThreadPool.QueueUserWorkItem(queueRunner);
            System.Threading.ThreadPool.QueueUserWorkItem(queueRunner);
        }

        List<string> files = new List<string>();

        void createFileList(object directory)
        {
            Loading.ShowLoading("Scanning for files", this);

            string[] files1 = Directory.GetFiles(directory.ToString(), "*.tlog", SearchOption.AllDirectories);
            string[] files2 = Directory.GetFiles(directory.ToString(), "*.bin", SearchOption.AllDirectories);
            string[] files3 = Directory.GetFiles(directory.ToString(), "*.log", SearchOption.AllDirectories);

            files.Clear();

            files.AddRange(files1);
            files.AddRange(files2);
            files.AddRange(files3);
        }

        List<Thread> threads = new List<Thread>();

        void queueRunner(object nothing)
        {
            lock (threads)
                threads.Add(Thread.CurrentThread);

            while (true)
            {
                string file = "";
                lock (files)
                {
                    if (IsDisposed)
                        return;

                    if (files.Count == 0)
                    {
                        break;
                    }

                    Loading.ShowLoading("Files loading left " + files.Count, this);

                    file = files[0];
                    files.RemoveAt(0);
                }

                if (File.Exists(file))
                    processbg(file);
            }

            if (threads[0] != Thread.CurrentThread)
                return;

            while (threads.Count > 1)
            {
                threads[1].Join();
                threads.RemoveAt(1);
            }

            Loading.ShowLoading("Populating Data", this);
            
            objectListView1.AddObjects(logs);

            Loading.Close();
        }

        List<object> logs = new List<object>();

        void processbg(string file)
        {
            if (!File.Exists(file + ".jpg"))
            {
                LogMap.MapLogs(new string[] {file});
            }

            var loginfo = new loginfo();

            loginfo.fullname = file;

            loginfo.Size = new FileInfo(file).Length;

            if (File.Exists(file + ".jpg"))
            {
                loginfo.img = new Bitmap(file + ".jpg");
            }

            if (file.ToLower().EndsWith(".tlog"))
            {
                using (MAVLinkInterface mine = new MAVLinkInterface())
                {
                    try
                    {
                        mine.logplaybackfile =
                            new BinaryReader(File.Open(file, FileMode.Open, FileAccess.Read, FileShare.Read));
                    }
                    catch (Exception ex)
                    {
                        log.Debug(ex.ToString());
                        CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                        return;
                    }
                    mine.logreadmode = true;

                    mine.MAV.packets.Initialize(); // clear

                    mine.getHeartBeat();

                    loginfo.Date = mine.lastlogread;
                    loginfo.Aircraft = mine.sysidcurrent;

                    var start = mine.lastlogread;

                    try
                    {
                        if (mine.logplaybackfile.BaseStream.Length > MaxPlaybackLogSize)
                        {
                            mine.logplaybackfile.BaseStream.Seek(-100000, SeekOrigin.End);
                        }
                        else
                        {
                            mine.logplaybackfile.BaseStream.Seek(0, SeekOrigin.Begin);
                        }
                    }
                    catch
                    {
                    }

                    var end = mine.lastlogread;

                    while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                    {
                        mine.readPacket();

                        if (mine.lastlogread > end)
                            end = mine.lastlogread;
                    }

                    loginfo.Duration = (end - start).ToString();
                }
            }

            logs.Add(loginfo);
        }

        static object locker = new object();

        public class loginfo
        {
            public string fullname { get; set; }

            public string Name
            {
                get { return Path.GetFileName(fullname); }
            }

            public string Directory
            {
                get { return Path.GetDirectoryName(fullname); }
            }

            public Image img { get; set; }
            public string Duration { get; set; }
            public DateTime Date { get; set; }
            public int Aircraft { get; set; }
            public long Size { get; set; }

            public loginfo()
            {
            }
        }

        private void objectListView1_FormatCell(object sender, FormatCellEventArgs e)
        {
            if (e.ColumnIndex != 0)
                return;

            loginfo info = (loginfo) e.Model;

            if (info.img == null)
                return;

            ImageDecoration decoration = new ImageDecoration(new Bitmap(info.img, 150, 150), 255);
            //decoration.ShrinkToWidth = true;
            decoration.AdornmentCorner = ContentAlignment.TopCenter;
            decoration.ReferenceCorner = ContentAlignment.TopCenter;
            e.SubItem.Decoration = decoration;

            // TextDecoration td = new TextDecoration("test", ContentAlignment.BottomCenter);

            // e.SubItem.Decorations.Add(td);

            Application.DoEvents();
        }

        /// <summary>
        /// This renderer doesn't ever draw it's cells as selected.
        /// </summary>
        public class NonSelectableRenderer : BaseRenderer
        {
            public override void Render(Graphics g, Rectangle r)
            {
                this.IsItemSelected = false;
                base.Render(g, r);
            }
        }

        private void BUT_changedir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();

            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                processbg(fbd.SelectedPath);
                //System.Threading.ThreadPool.QueueUserWorkItem(processbg, fbd.SelectedPath);
            }
        }
    }
}