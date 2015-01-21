using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BrightIdeasSoftware;

namespace MissionPlanner.Log
{
    public partial class LogIndex : Form
    {
        public LogIndex()
        {
            InitializeComponent();

            this.olvColumnImage.Renderer = new NonSelectableRenderer();
        }

        private void LogIndex_Load(object sender, EventArgs e)
        {
            //processbg(MainV2.LogDir);
            System.Threading.ThreadPool.QueueUserWorkItem(processbg, MainV2.LogDir);
        }

        void processbg(object directory)
        {
            //this.Invoke((MethodInvoker)delegate { objectListView1.Clear(); });            

            string[] files1 = Directory.GetFiles(directory.ToString(), "*.tlog", SearchOption.AllDirectories);
            string[] files2 = Directory.GetFiles(directory.ToString(), "*.bin", SearchOption.AllDirectories);
            string[] files3 = Directory.GetFiles(directory.ToString(), "*.log", SearchOption.AllDirectories);

            List<string> files = new List<string>();

            files.AddRange(files1);
            files.AddRange(files2);
            files.AddRange(files3);

            //objectListView1.VirtualListSize = files.Length;

            List<object> logs = new List<object>();

            foreach (var file in files)
            {
                if (!File.Exists(file + ".jpg"))
                {
                    LogMap.MapLogs(new string[] { file });
                }

                var loginfo = new loginfo();

                loginfo.fullname = file;

                if (File.Exists(file + ".jpg"))
                {
                    loginfo.img = new Bitmap(file + ".jpg");
                }

                //objectListView1.AddObject(loginfo);

                logs.Add(loginfo);
            }

            this.Invoke((MethodInvoker)delegate
            {
                objectListView1.AddObjects(logs);
            });
        }

        public class loginfo
        {
            public string fullname { get; set; }
            public string Name { get { return Path.GetFileName(fullname); } }
            public string Directory { get { return Path.GetDirectoryName(fullname); } }
            public Image img { get; set; }
            //public TimeSpan Duration { get; set; }

            public loginfo()
            {
                //Duration = new TimeSpan(0, 0, 1);
            }
        }

        private void objectListView1_FormatCell(object sender, FormatCellEventArgs e)
        {
            if (e.ColumnIndex != 0)
                return;

            loginfo info = (loginfo)e.Model;

            if (info.img == null)
                return;

            ImageDecoration decoration = new ImageDecoration(new Bitmap(info.img,150,150),255);
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
