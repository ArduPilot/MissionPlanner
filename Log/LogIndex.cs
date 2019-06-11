using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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



        public LogIndex()
        {
            InitializeComponent();

            this.olvColumnImage.Renderer = new NonSelectableRenderer();
        }

        private void LogIndex_Load(object sender, EventArgs e)
        {
            createFileList(Settings.Instance.LogDir);

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

        private void queueRunner(object nothing)
        {
            Parallel.ForEach(files, file => { ProcessFile(file); });

            Loading.ShowLoading("Populating Data", this);

            this.BeginInvokeIfRequired(a =>
            {
                objectListView1.AddObjects(logs);
            });

            Loading.Close();
        }

        private void ProcessFile(string file)
        {
            if (File.Exists(file))
                processbg(file);
        }

        List<object> logs = new List<object>();
        int a = 0;
        void processbg(string file)
        {
            a++;
            Loading.ShowLoading(a+"/"+files.Count + " " + file, this);

            if (!File.Exists(file + ".jpg"))
            {
                LogMap.MapLogs(new string[] {file});
            }

            var loginfo = new loginfo();

            loginfo.fullname = file;

            try
            {
                // file not found exception even though it passes the exists check above.
                loginfo.Size = new FileInfo(file).Length;
            }
            catch
            {
                
            }

            if (File.Exists(file + ".jpg"))
            {
                loginfo.imgfile = file + ".jpg";
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
                    mine.speechenabled = false;

                    // file is to small
                    if (mine.logplaybackfile.BaseStream.Length < 1024*4)
                        return;

                    mine.getHeartBeat();

                    loginfo.Date = mine.lastlogread;
                    loginfo.Aircraft = mine.sysidcurrent;

                    loginfo.Frame = mine.MAV.aptype.ToString();

                    var start = mine.lastlogread;

                    try
                    {
                        mine.logplaybackfile.BaseStream.Seek(0, SeekOrigin.Begin);
                    }
                    catch
                    {
                    }

                    var end = mine.lastlogread;

                    var length = mine.logplaybackfile.BaseStream.Length;

                    var a = 0;

                    // abandon last 100 bytes
                    while (mine.logplaybackfile.BaseStream.Position < (length-100))
                    {
                        var packet = mine.readPacket();

                        // gcs
                        if(packet.sysid == 255)
                            continue;

                        if (packet.msgid == (uint)MAVLink.MAVLINK_MSG_ID.CAMERA_FEEDBACK)
                            loginfo.CamMSG++;

                        if (a % 10 == 0)
                            mine.MAV.cs.UpdateCurrentSettings(null, true, mine);

                        a++;

                        if (mine.lastlogread > end)
                            end = mine.lastlogread;
                    }

                    loginfo.Home = mine.MAV.cs.Location;

                    loginfo.TimeInAir = mine.MAV.cs.timeInAir;

                    loginfo.DistTraveled = mine.MAV.cs.distTraveled;

                    loginfo.Duration = (end - start).ToString();
                }
            }
            else if (file.ToLower().EndsWith(".bin") || file.ToLower().EndsWith(".log"))
            {
                using (CollectionBuffer colbuf = new CollectionBuffer(File.OpenRead(file)))
                {
                    PointLatLngAlt lastpos = null;
                    DateTime start = DateTime.MinValue;
                    DateTime end = DateTime.MinValue;
                    DateTime tia = DateTime.MinValue;
                    // set time in air/home/distancetraveled
                    foreach (var dfItem in colbuf.GetEnumeratorType("GPS"))
                    {
                        if (dfItem["Status"] != null)
                        {
                            var status = int.Parse(dfItem["Status"]);
                            if (status >= 3)
                            {
                                var pos = new PointLatLngAlt(
                                    double.Parse(dfItem["Lat"], CultureInfo.InvariantCulture),
                                    double.Parse(dfItem["Lng"], CultureInfo.InvariantCulture),
                                    double.Parse(dfItem["Alt"], CultureInfo.InvariantCulture));

                                if (lastpos == null)
                                    lastpos = pos;

                                if (start == DateTime.MinValue)
                                {
                                    loginfo.Date = dfItem.time;
                                    start = dfItem.time;
                                }

                                end = dfItem.time;

                                // add distance
                                loginfo.DistTraveled += (float)lastpos.GetDistance(pos);

                                // set home
                                if (loginfo.Home == null)
                                    loginfo.Home = pos;

                                if (dfItem.time > tia.AddSeconds(1))
                                {
                                    // ground speed  > 0.2 or  alt > homelat+2
                                    if (double.Parse(dfItem["Spd"], CultureInfo.InvariantCulture) > 0.2 ||
                                        pos.Alt > (loginfo.Home.Alt + 2))
                                    {
                                        loginfo.TimeInAir++;
                                    }
                                    tia = dfItem.time;
                                }
                            }
                        }
                    }

                    loginfo.Duration = (end - start).ToString();

                    loginfo.CamMSG = colbuf.GetEnumeratorType("CAM").Count();

                    loginfo.Aircraft = 0;//colbuf.dflog.param[""];

                    loginfo.Frame = "Unknown";//mine.MAV.aptype.ToString();
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

            internal string imgfile { get; set; }
            private Bitmap image = null;
            public Image img { get { lock (this) { if (image == null && !String.IsNullOrEmpty(imgfile)) image = GetBitmap(imgfile); return image; } } }
            public string Duration { get; set; }
            public DateTime Date { get; set; }
            public int Aircraft { get; set; }
            public long Size { get; set; }
            public PointLatLngAlt Home {get;set;}

            public string Frame { get; set; }

            public float TimeInAir { get; set; }

            public float DistTraveled { get; set; }

            public int CamMSG { get; set; }

            public loginfo()
            {
            }
        }

        private static Bitmap GetBitmap(string imgFile)
        {
            using (var file = File.Open(imgFile, FileMode.Open, FileAccess.Read))
            {
                return new Bitmap(file);
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
                files.Clear();
                createFileList(fbd.SelectedPath);
                System.Threading.ThreadPool.QueueUserWorkItem(queueRunner);
            }
        }

        private void btnDeleteLog_Click(object sender, EventArgs e)
        {
            if (CustomMessageBox.Show(string.Format("Do you really want to delete {0} logs?", objectListView1.SelectedIndices.Count), MessageBoxButtons: CustomMessageBox.MessageBoxButtons.YesNo) == CustomMessageBox.DialogResult.Yes)
            {
                foreach (int i in objectListView1.SelectedIndices)
                {
                    var item = (OLVListItem)objectListView1.Items[i];
                    var log = (LogIndex.loginfo)item.RowObject;

                    if (log.fullname == "--- DELETED ---")
                        continue;

                    var files = Directory.GetFiles(Path.GetDirectoryName(log.fullname), Path.GetFileName(log.fullname) + ".*").ToList();
                    if (Path.GetExtension(log.fullname).Equals(".TLOG", StringComparison.InvariantCultureIgnoreCase))
                        files.Add(Path.ChangeExtension(log.fullname, "rlog"));

                    foreach (var file in files)
                    {
                        try
                        {
                            if (File.Exists(file))
                                File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            CustomMessageBox.Show(string.Format("Error has been occured when trying to delete {0}\r\n{1}", log.fullname, ex.Message));
                        }
                    }

                    log.fullname = "--- DELETED ---";
                    log.imgfile = null;
                    log.TimeInAir = 0;
                    log.DistTraveled = 0;
                    log.Duration = "";
                }

                objectListView1.Refresh();
            }
        }

        private void objectListView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnDeleteLog.Enabled = objectListView1.SelectedIndices.Count > 0;

            float timeInAir = 0;
            float distTraveled = 0;
            foreach (int i in objectListView1.SelectedIndices)
            {
                var item = (OLVListItem)objectListView1.Items[i];
                var log = (LogIndex.loginfo)item.RowObject;

                timeInAir += log.TimeInAir;
                distTraveled += log.DistTraveled;

            }

            lbStats.Text = string.Format("Selected: {2}; TimeInAir: {0}; DistTraveled: {1}m", ToHours(timeInAir), (int)distTraveled, objectListView1.SelectedIndices.Count);
        }

        private static string ToHours(float seconds)
        {
            var hours = (int)Math.Floor(seconds / 3600);
            var minutes = (int)Math.Floor((seconds % 3600) / 60);
            var sec = (int)(seconds % 60);

            return (hours < 10 ? "0" : "") + hours + ":" + minutes.ToString("D2") + ":" + sec.ToString("D2");
        }
    }
}