using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.Internals;
using GMap.NET.WindowsForms;
using MissionPlanner.Maps;
using GMap.NET.MapProviders;

namespace MissionPlanner.Controls
{
    public partial class GMAPCache : UserControl, IActivate
    {
        private DataGridViewButtonColumn buttonColumn;
        bool _init = false;

        public GMAPCache()
        {
            InitializeComponent();
        }

        public void Activate()
        {
            var dir = Path.Combine(CacheLocator.Location, "TileDBv3", "en");

            var dirs = Directory.GetDirectories(dir, "*.*");

            var sizecountoverall = DirSize(new DirectoryInfo(dir));
            var overall = new
            {
                Name = "Total", Size = Math.Round(sizecountoverall.Item1 / 1024.0 / 1024.0, MidpointRounding.AwayFromZero) + "MB", Count = sizecountoverall.Item2
            };

            List<object> list = new List<object>();

            foreach (var s in dirs)
            {
                var sizecount = DirSize(new DirectoryInfo(s));

                list.Add(new
                    {Name = Path.GetFileName(s), Size = Math.Round(sizecount.Item1 / 1024.0 / 1024.0, MidpointRounding.AwayFromZero) + "MB", Count = sizecount.Item2});
            }
            
            list.Add(overall);

            var bs = new BindingSource();
            bs.DataSource = list;
            myDataGridView1.DataSource = bs;

            if(!_init)
            {
                // Add a button column. 
                buttonColumn = new DataGridViewButtonColumn();
                buttonColumn.HeaderText = "";
                //buttonColumn.Name = "Status Request";
                buttonColumn.Text = "Remove old";
                buttonColumn.UseColumnTextForButtonValue = true;

                myDataGridView1.Columns.Add(buttonColumn);

                myDataGridView1.CellClick +=
                    new DataGridViewCellEventHandler(myDataGridView1_CellClick);
                _init = true;
            }
        }

        private void myDataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks that are not on button cells. 
            if (e.RowIndex < 0 || e.ColumnIndex != buttonColumn.Index) return;

            string dir = myDataGridView1[0, e.RowIndex].Value as string;

            {
                var removed = ((PureImageCache) MyImageCache.Instance).DeleteOlderThan(DateTime.Now.AddDays(-30),
                    GMapProviders.List.First(a => a.Name == dir).DbId);

                CustomMessageBox.Show("Removed " + removed + " images");

                Activate();
            }
        }

        public static Tuple<long,long> DirSize(DirectoryInfo d)
        {
            long size = 0;
            long count = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
                count++;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                var temp = DirSize(di);
                size += temp.Item1;
                count += temp.Item2;
            }

            return new Tuple<long, long>(size, count);
        }
    }


}
