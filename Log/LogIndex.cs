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
            System.Threading.ThreadPool.QueueUserWorkItem(processbg);
        }

        void processbg(object crap)
        {
            string[] files = Directory.GetFiles(MainV2.LogDir, "*.tlog", SearchOption.AllDirectories);

            //objectListView1.VirtualListSize = files.Length;

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

                objectListView1.AddObject(loginfo);
            }
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
            decoration.AdornmentCorner = ContentAlignment.BottomCenter;
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
    }
}
