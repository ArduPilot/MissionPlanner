﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.IO;
using log4net;
using ZedGraph; // Graphs
using System.Xml;
using System.Collections;
using MissionPlanner.Controls;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace MissionPlanner.Log
{
    public partial class LogBrowse : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        DataTable m_dtCSV = new DataTable();

        List<DFLog.DFItem> logdata;
        Hashtable logdatafilter = new Hashtable();

        const int typecoloum = 2;

        int m_timeStart = 0;
        int m_timeInterval = 100;
        Dictionary<int, int> m_timeTable = new Dictionary<int, int>();
        int[] m_timeKeys;
        int m_currSampleID = 0;

        List<PointPairList> listdata = new List<PointPairList>();
        GMapOverlay mapoverlay;
		GMapOverlay markeroverlay;
		LineObj m_cursorLine = null;

        class displayitem
        {
            public string type;
            public string field;
            public bool left = true;
        }
        class displaylist
        {
            public string Name;
            public displayitem[] items;

            public override string ToString()
            {
                return Name;
            }
        }

        List<displaylist> graphs = new List<displaylist>()
        { 
            new displaylist() { Name = "None"},
            new displaylist() { Name = "Mechanical Failure", items = new displayitem[] { 
                new displayitem(){ type= "ATT", field ="Roll"}, 
                new displayitem(){ type= "ATT", field = "DesRoll"},
                new displayitem(){ type= "ATT", field = "Pitch"},
                new displayitem(){ type= "ATT", field = "DesPitch"},
                new displayitem(){ type= "CTUN", field = "Alt", left =false},
                new displayitem(){ type= "CTUN", field = "DAlt", left =false }
            }},
            new displaylist() { Name = "Mechanical Failure - Stab", items = new displayitem[] { new displayitem(){ type= "ATT", field ="Roll"}, new displayitem(){ type= "ATT", field = "DesRoll"}}},
            new displaylist() { Name = "Mechanical Failure - Auto", items = new displayitem[] { new displayitem(){ type= "ATT", field ="Roll"}, new displayitem(){ type= "NTUN", field = "DRoll"}}},
            new displaylist() { Name = "Vibrations", items = new displayitem[] { new displayitem(){ type= "IMU", field ="AccX"}, new displayitem(){ type= "IMU", field ="AccY"},new displayitem(){ type= "IMU", field ="AccZ"}}},
            new displaylist() { Name = "GPS Glitch", items = new displayitem[] { new displayitem(){ type= "GPS", field ="HDop"},new displayitem(){ type= "GPS", field ="NSats", left = false}}},
            new displaylist() { Name = "Power Issues", items = new displayitem[] { new displayitem(){ type= "CURR", field ="Vcc"}}},
            new displaylist() { Name = "Errors", items = new displayitem[] { new displayitem(){ type= "ERR", field ="ECode"}}},
            new displaylist() { Name = "Battery Issues", items = new displayitem[] { new displayitem(){ type= "CTUN", field ="ThrIn"},new displayitem(){ type= "CURR", field ="ThrOut"},new displayitem(){ type= "CURR", field ="Volt", left = false}}},
        };

        /*  
    105    +Format characters in the format string for binary log messages  
    106    +  b   : int8_t  
    107    +  B   : uint8_t  
    108    +  h   : int16_t  
    109    +  H   : uint16_t  
    110    +  i   : int32_t  
    111    +  I   : uint32_t  
    112    +  f   : float  
    113    +  N   : char[16]  
    114    +  c   : int16_t * 100  
    115    +  C   : uint16_t * 100  
    116    +  e   : int32_t * 100  
    117    +  E   : uint32_t * 100  
    118    +  L   : uint32_t latitude/longitude  
    119    + */

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.G))
            {
                string lineno = "0";
                InputBox.Show("Line no", "Enter Line Number", ref lineno);

                int line = int.Parse(lineno);

                try
                {
                    dataGridView1.CurrentCell = dataGridView1[1, line - 1];
                }
                catch { CustomMessageBox.Show("Line Doesn't Exist"); }

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public LogBrowse()
        {
            InitializeComponent();

             mapoverlay = new GMapOverlay("overlay");
             markeroverlay = new GMapOverlay("markers");

            myGMAP1.MapProvider = GMap.NET.MapProviders.GoogleSatelliteMapProvider.Instance;

            myGMAP1.Overlays.Add(mapoverlay);
			myGMAP1.Overlays.Add(markeroverlay);
			
            //CMB_preselect.DisplayMember = "Name";
            CMB_preselect.DataSource = graphs;

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Hashtable seenmessagetypes = new Hashtable();
            mapoverlay.Clear();
            markeroverlay.Clear();

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Log Files|*.log;*.bin";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;

            openFileDialog1.InitialDirectory = MainV2.LogDir;

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    Stream stream;

                    if (openFileDialog1.FileName.ToLower().EndsWith(".bin"))
                    {
                        log.Info("before " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));
                        string tempfile = Path.GetTempFileName();

                        BinaryLog.ConvertBin(openFileDialog1.FileName, tempfile);

                        stream = File.Open(tempfile, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }
                    else
                    {
                        stream = File.Open(openFileDialog1.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    }

                    logdata = DFLog.ReadLog(stream);

                    this.Text = "Log Browser - " + Path.GetFileName(openFileDialog1.FileName);
                    log.Info("about to create DataTable " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));
                    m_dtCSV = new DataTable();

                    log.Info("process to datagrid " + (GC.GetTotalMemory(false)/1024.0/1024.0));

                    bool largelog = logdata.Count > 500000 ? true : false;

                    foreach (var item in logdata)
                    {

                        if (item.items != null)
                        {
                            while (m_dtCSV.Columns.Count < (item.items.Length + typecoloum))
                            {
                                m_dtCSV.Columns.Add();
                            }

                            seenmessagetypes[item.msgtype] = "";

                            if (largelog)
                                continue;

                            DataRow dr = m_dtCSV.NewRow();

                            dr[0] = item.lineno;
                            dr[1] = item.time.ToString("yyyy-MM-dd HH:mm:ss.fff");

                            for (int a = 0; a < item.items.Length; a++)
                            {
                                dr[a + typecoloum] = item.items[a];
                            }

                            m_dtCSV.Rows.Add(dr);
                        }
                    }

                    log.Info("Done " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));

                    //PopulateDataTableFromUploadedFile(stream);

                    stream.Close();

                    log.Info("set dgv datasourse " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));

                    if (MainV2.MONO)
                    {
                        //if (m_dtCSV.Rows.Count > 5000)
                       // {
                       //     CustomMessageBox.Show("This log apears to be a large log, the grid view will be disabled.\nAll graphing will still work however", "Large Log");
                       //     dataGridView1.Visible = false;
                       // }
                       // else
                        {
                            BindingSource bs = new BindingSource();
                            bs.DataSource = m_dtCSV;
                            dataGridView1.DataSource = bs;
                        }
                    }
                    else
                    {
                        dataGridView1.VirtualMode = true;
                        dataGridView1.RowCount = logdata.Count;
                        dataGridView1.ColumnCount = m_dtCSV.Columns.Count;
                    }

        

                    dataGridView1.Columns[0].Visible = false;

                    log.Info("datasource set " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));

                }
                catch (Exception ex) { CustomMessageBox.Show("Failed to read File: " + ex.ToString()); }

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

				BuildTimeTable(m_dtCSV, DFLog.logformat);

                DrawMap();

                CreateChart(zg1);

                ResetTreeView(seenmessagetypes);

                if (DFLog.logformat.Count == 0)
                {
                    CustomMessageBox.Show("Log Browse will not function correctly without FMT messages in your log.\nThese appear to be missing from your log.", "Error");
                    this.Close();
                    return;
                }
            }
            else
            {
                this.Close();
                return;
            }
        }

        private void UntickTreeView()
        {
            foreach (TreeNode node1 in treeView1.Nodes)
            {
                node1.Checked = false;
                foreach (TreeNode node2 in node1.Nodes)
                {
                    node2.Checked = false;
                    foreach (TreeNode node3 in node2.Nodes)
                    {
                        node3.Checked = false;
                    }
                }
            }
        }

        private void BuildTimeTable(DataTable dt, Dictionary<string, DFLog.Label> logfmt)
        {

            m_timeStart = 0;
            m_timeTable.Clear();
            m_timeInterval = 100;

            int timeIndex = -1;
            string timeType = "";

            if (logfmt.ContainsKey("IMU"))
            {
                if (timeIndex < 0)
                {
                    timeIndex = DFLog.FindMessageOffset("IMU", "TimeMS");
                    if (timeIndex >= 0)
                        timeType = "IMU";
                }
            }

            if (logfmt.ContainsKey("GPS"))
            {
                if (timeIndex < 0)
                {
                    timeIndex = DFLog.FindMessageOffset("GPS", "TimeMS");
                    if (timeIndex >= 0)
                        timeType = "GPS";
                }
                if (timeIndex < 0)
                {
                    timeIndex = DFLog.FindMessageOffset("GPS", "T");
                    if (timeIndex >= 0)
                        timeType = "GPS";
                }
            }


            if (timeIndex >= 0)
            {
                int i = 0;
                int lastmillis = -1;
                for (i = 0; i < dt.Rows.Count; i++)
                {
                    DataRow datarow = dt.Rows[i];
                    if (datarow[1].ToString() == timeType)
                    {
                        int millis = int.Parse(datarow[timeIndex].ToString());

                        if (m_timeStart == 0)
                            m_timeStart = millis;
                        if (millis != lastmillis)
                        {
                            int diff = millis - lastmillis;
                            if (diff > 0)
                                m_timeInterval = Math.Min(m_timeInterval, diff);

                            m_timeTable.Add(millis, i);
                            lastmillis = millis;
                        }
                    }
                }

                m_timeKeys = new int[m_timeTable.Count];
                m_timeTable.Keys.CopyTo(m_timeKeys, 0);
            }

        }


        private void ResetTreeView(Hashtable seenmessagetypes)
        {
            treeView1.Nodes.Clear();

            var sorted = new SortedList(DFLog.logformat);

            foreach (DFLog.Label item in sorted.Values)
            {
                TreeNode tn = new TreeNode(item.Name);

                if (seenmessagetypes.ContainsKey(item.Name))
                {
                    treeView1.Nodes.Add(tn);
                    foreach (var item1 in item.FieldNames)
                    {
                        tn.Nodes.Add(item1);
                    }
                }
            }
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.ColumnCount < typecoloum)
                return;

            try
            {
                // number the coloums
                int a = -typecoloum;
                foreach (DataGridViewColumn col in dataGridView1.Columns)
                {
                    col.HeaderText = a.ToString();
                    a++;
                }
            }
            catch { }
            try
            {
                // process the line type
                string option = dataGridView1[typecoloum, e.RowIndex].EditedFormattedValue.ToString();

                // new self describing log
                if (DFLog.logformat.ContainsKey(option))
                {
                    int a = typecoloum + 1;
                    foreach (string name in DFLog.logformat[option].FieldNames)
                    {
                        dataGridView1.Columns[a].HeaderText = name;
                        a++;
                    }
                    for (; a < dataGridView1.Columns.Count; a++)
                    {
                        dataGridView1.Columns[a].HeaderText = "";
                    }

                    if (option == "GPS")
                    {
                        // display current gps point
                        var ans = getPointLatLng(logdata.Find(x => { return x.lineno.ToString() == dataGridView1[0, e.RowIndex].Value.ToString(); }));// dataGridView1.Rows[e.RowIndex]);
                        if (ans.HasValue)
                        {
                            mapoverlay.Markers.Clear();
                            mapoverlay.Markers.Add(new GMarkerGoogle(ans.Value, GMarkerGoogleType.red));
                            myGMAP1.MarkersEnabled = true;
                        }
                        else
                        {
                            // mapoverlay.Markers.Clear();
                        }
                    }

                    return;
                }

                if (option.StartsWith("PID-"))
                    option = "PID-1";

                using (XmlReader reader = XmlReader.Create(Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + "dataflashlog.xml"))
                {
                    reader.Read();
                    reader.ReadStartElement("LOGFORMAT");
                    if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduPlane)
                    {
                        reader.ReadToFollowing("APM");
                    }
                    else if (MainV2.comPort.MAV.cs.firmware == MainV2.Firmwares.ArduRover)
                    {
                        reader.ReadToFollowing("APRover");
                    }
                    else
                    {
                        reader.ReadToFollowing("AC2");
                    }
                    reader.ReadToFollowing(option);

                    dataGridView1.Columns[1].HeaderText = "";

                    if (reader.NodeType == XmlNodeType.None)
                        return;

                    XmlReader inner = reader.ReadSubtree();

                    inner.MoveToElement();

                    int a = 2;

                    while (inner.Read())
                    {
                        inner.MoveToElement();
                        if (inner.IsStartElement())
                        {
                            if (inner.Name.StartsWith("F"))
                            {
                                dataGridView1.Columns[a].HeaderText = inner.ReadString();
                                log.Info(a + " " + dataGridView1.Columns[a].HeaderText);
                                a++;
                            }

                        }
                    }

                    for (; a < dataGridView1.Columns.Count; a++)
                    {
                        dataGridView1.Columns[a].HeaderText = "";
                    }

                }
            }
            catch { log.Info("DGV logbrowse error"); }
        }

        Color[] colours = new Color[] {      
            Color.Red, 
           Color.Green, 
           Color.Blue, 
           Color.Pink, 
           Color.Yellow, 
           Color.Orange, 
           Color.Violet, 
           Color.Wheat, 
           Color.Teal, 
           Color.Silver };

        public void CreateChart(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Value Graph";
            myPane.XAxis.Title.Text = "Line Number";
            myPane.YAxis.Title.Text = "Output";

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;

            myPane.XAxis.Scale.Min = 0;
            //myPane.XAxis.Scale.Max = -1;

            // Make the Y axis scale red
            myPane.YAxis.Scale.FontSpec.FontColor = Color.Red;
            myPane.YAxis.Title.FontSpec.FontColor = Color.Red;
            // turn off the opposite tics so the Y tics don't show up on the Y2 axis
            myPane.YAxis.MajorTic.IsOpposite = false;
            myPane.YAxis.MinorTic.IsOpposite = false;
            // Don't display the Y zero line
            myPane.YAxis.MajorGrid.IsZeroLine = true;
            // Align the Y axis labels so they are flush to the axis
            myPane.YAxis.Scale.Align = AlignP.Inside;
            // Manually set the axis range
            //myPane.YAxis.Scale.Min = -1;
            //myPane.YAxis.Scale.Max = 1;

            // Fill the axis background with a gradient
            //myPane.Chart.Fill = new Fill(Color.White, Color.LightGray, 45.0f);


            // Calculate the Axis Scale Ranges
            try
            {
                zg1.AxisChange();
            }
            catch { }



        }

        private void Graphit_Click(object sender, EventArgs e)
        {
            graphit_clickprocess(true);
        }

        void graphit_clickprocess(bool left = true)
        {
            if (dataGridView1 == null || dataGridView1.RowCount == 0 || dataGridView1.ColumnCount == 0)
            {
                CustomMessageBox.Show("Please load a valid file", "Error");
                return;
            }

            if (dataGridView1.CurrentCell == null)
            {
                CustomMessageBox.Show("Please select a cell first", "Error");
                return;
            }

            int col = dataGridView1.CurrentCell.ColumnIndex;
            int row = dataGridView1.CurrentCell.RowIndex;
            string type = dataGridView1[typecoloum, row].Value.ToString();

            if (col == 0)
            {
                CustomMessageBox.Show("Please pick another column, Highlight the cell you wish to graph", "Error");
                return;
            }

            if (!DFLog.logformat.ContainsKey(type))
            {
                CustomMessageBox.Show("Your log file doesnt contain the required FMT messages","Error");
                return;
            }

            if ((col - typecoloum - 1) < 0)
            {
                CustomMessageBox.Show("Cannot graph this field", "Error");
                return;
            }

            if (DFLog.logformat[type].FieldNames.Length <= (col - typecoloum - 1))
            {
                CustomMessageBox.Show("Invalid Field (not in FMT)", "Error");
                return;
            }

            string fieldname = DFLog.logformat[type].FieldNames[col - typecoloum - 1];

            GraphItem(type, fieldname, left);
        }

        void GraphItem(string type, string fieldname, bool left = true)
        {
            double a = 0; // row counter
            int error = 0;

            // ensure we tick the treeview
            foreach (TreeNode node in treeView1.Nodes)
            {
                if (node.Text == type)
                {
                    foreach (TreeNode subnode in node.Nodes)
                    {
                        if (subnode.Text == fieldname && subnode.Checked != true)
                        {
                            subnode.Checked = true;
                            break;
                        }
                    }
                }
            }

            if (!DFLog.logformat.ContainsKey(type))
            {
                CustomMessageBox.Show("No FMT message for "+type + " - " + fieldname,"Error");
                return;
            }

            int col = DFLog.FindMessageOffset(type, fieldname);

            // field does not exist
            if (col == -1)
                return;

            PointPairList list1 = new PointPairList();

            string header = fieldname;

            foreach (var item in logdata)
            {
                if (item.msgtype == type)
                {
                    try
                    {
                        double value = double.Parse(item.items[col], System.Globalization.CultureInfo.InvariantCulture);

                        // XDate time = new XDate(DateTime.Parse(datarow.Cells[1].Value.ToString()));

                        list1.Add(a, value);
                    }
                    catch { error++; log.Info("Bad Data : " + type + " " + col + " " + a); if (error >= 500) { CustomMessageBox.Show("There is to much bad data - failing"); break; } }
                }


                a++;
            }

            if (list1.Count < 1)
                return;

            LineItem myCurve;

            myCurve = zg1.GraphPane.AddCurve(header, list1, colours[zg1.GraphPane.CurveList.Count % colours.Length], SymbolType.None);

            leftorrightaxis(left, myCurve);

            // Make sure the Y axis is rescaled to accommodate actual data
            try
            {
                zg1.AxisChange();
            }
            catch { }
            // Zoom all
            zg1.ZoomOutAll(zg1.GraphPane);

            try
            {
                DrawModes();

                DrawErrors();

                DrawTime();
            }
            catch { }

            // Force a redraw
            zg1.Invalidate();
        }

        void DrawErrors()
        {
            bool top = false;
            int a = 0;

            foreach (var item in logdata)
            {
                if (item.msgtype == "ERR")
                {
                    if (!DFLog.logformat.ContainsKey("ERR"))
                        return;

                    int index = DFLog.FindMessageOffset("ERR", "Subsys");
                    if (index == -1)
                    {
                        continue;
                    }

                    int index2 = DFLog.FindMessageOffset("ERR", "ECode");
                    if (index2 == -1)
                    {
                        continue;
                    }

                    string mode = "Err: " + ((DFLog.error_subsystem)int.Parse(item.items[index].ToString())) + "-" + item.items[index2].ToString().Trim();
                    if (top)
                    {
                        var temp = new TextObj(mode, a, zg1.GraphPane.YAxis.Scale.Max, CoordType.AxisXYScale, AlignH.Left, AlignV.Top);
                        temp.FontSpec.Fill.Color = Color.Red;
                        zg1.GraphPane.GraphObjList.Add(temp);
                    }
                    else
                    {
                        var temp = new TextObj(mode, a, zg1.GraphPane.YAxis.Scale.Max, CoordType.AxisXYScale, AlignH.Left, AlignV.Bottom);
                        temp.FontSpec.Fill.Color = Color.Red;
                        zg1.GraphPane.GraphObjList.Add(temp);
                    }
                    top = !top;
                }
                a++;
            }
        }

        void DrawModes()
        {
            bool top = false;
            int a = 0;

            zg1.GraphPane.GraphObjList.Clear();

            foreach (var item in logdata)
            {
                if (item.msgtype == "MODE")
                {
                    if (!DFLog.logformat.ContainsKey("MODE"))
                        return;

                    int index = DFLog.FindMessageOffset("MODE", "Mode");
                    if (index == -1)
                    {
                        continue;
                    }

                    string mode = item.items[index].ToString().Trim();
                    if (top)
                    {
                        zg1.GraphPane.GraphObjList.Add(new TextObj(mode, a, zg1.GraphPane.YAxis.Scale.Min, CoordType.AxisXYScale, AlignH.Left, AlignV.Top));
                    }
                    else
                    {
                        zg1.GraphPane.GraphObjList.Add(new TextObj(mode, a, zg1.GraphPane.YAxis.Scale.Min, CoordType.AxisXYScale, AlignH.Left, AlignV.Bottom));
                    }
                    top = !top;
                }
                a++;
            }
        }

        void DrawTime()
        {
            int a = 0;

            DateTime starttime = DateTime.MinValue;
            int startdelta = 0;
            DateTime workingtime = starttime;

            DateTime lastdrawn = DateTime.MinValue;

            //zg1.GraphPane.GraphObjList.Clear();

            foreach (var item in logdata)
            {
                if (item.msgtype == "GPS")
                {
                    if (!DFLog.logformat.ContainsKey("GPS"))
                        break;

                    int index = DFLog.FindMessageOffset("GPS", "TimeMS");
                    if (index == -1)
                    {
                        a++;
                        continue;
                    }

                    string time = item.items[index].ToString();
                    int temp;
                    if (int.TryParse(time, out temp))
                    {
                        if (startdelta == 0)
                            startdelta = temp;

                        workingtime = starttime.AddMilliseconds(temp - startdelta);

                        TimeSpan span = workingtime - starttime;

                        if (workingtime.Minute != lastdrawn.Minute)
                        {
                            zg1.GraphPane.GraphObjList.Add(new TextObj(span.TotalMinutes.ToString("0") + " min", a, zg1.GraphPane.YAxis.Scale.Max, CoordType.AxisXYScale, AlignH.Left, AlignV.Top));
                            lastdrawn = workingtime;
                        }
                    }
                }
                a++;
            }
        }

        class LogRouteInfo
        {
            public int firstpoint = 0;
            public int lastpoint = 0;
            public List<int> samples = new List<int>();
        }

        void DrawMap()
        {
            int rtcnt = 0;

            try
            {
                mapoverlay.Routes.Clear();

                DateTime starttime = DateTime.MinValue;
                DateTime workingtime = starttime;

                DateTime lastdrawn = DateTime.MinValue;

                List<PointLatLng> routelist = new List<PointLatLng>();
                List<int> samplelist = new List<int>();

                //zg1.GraphPane.GraphObjList.Clear();
				
				//check if GPS data are available
				if (!DFLog.logformat.ContainsKey("GPS"))
                    return;

                int index = DFLog.FindMessageOffset("GPS", "Lat");
                if (index == -1)
                {
                    return;
                }

                int index2 = DFLog.FindMessageOffset("GPS", "Lng");
                if (index2 == -1)
                {
                    return;
                }

                int index3 = DFLog.FindMessageOffset("GPS", "Status");
                if (index3 == -1)
                {
                    return;
                }				
				
                int i = 0;
                int firstpoint = 0;
                foreach (var item in logdata)
                {
                    if (item.msgtype == "GPS")
                    {
                        var ans = getPointLatLng(item);

                        if (ans.HasValue)
                        {
                            routelist.Add(ans.Value);
							samplelist.Add(i);
							
							
							if (routelist.Count > 1000)
                            {
                                //split the route in several small parts (due to memory errors)
                                GMapRoute route_part = new GMapRoute(routelist, "route_" + rtcnt);
                                route_part.Stroke = new Pen(Color.FromArgb(127, Color.Blue), 2);

                                LogRouteInfo lri = new LogRouteInfo();
                                lri.firstpoint = firstpoint;
                                lri.lastpoint = i;
                                lri.samples.AddRange(samplelist);
                                
                                route_part.Tag = lri;
                                route_part.IsHitTestVisible = true;
                                mapoverlay.Routes.Add(route_part);
                                rtcnt++;

                                //clear the list and set the last point as first point for the next route
                                routelist.Clear();
                                samplelist.Clear();
                                firstpoint = i;
                                samplelist.Add(firstpoint);
                                routelist.Add(ans.Value);
                            }							
                        }
                    }
					i++;
                }

                GMapRoute route = new GMapRoute(routelist, "route_" + rtcnt);
                route.Stroke = new Pen(Color.FromArgb(127, Color.Blue), 2);

                LogRouteInfo lri2 = new LogRouteInfo();
                lri2.firstpoint = firstpoint;
                lri2.lastpoint = i;
                lri2.samples.AddRange(samplelist);
                route.Tag = lri2;
                route.IsHitTestVisible = true;
                mapoverlay.Routes.Add(route);
                rtcnt++;
                myGMAP1.ZoomAndCenterRoute(route);
                
            }
            catch (Exception ex) { log.Error(ex); }
            if (rtcnt > 0)
                myGMAP1.RoutesEnabled = true;
        }

        PointLatLng? getPointLatLng(DFLog.DFItem item)
        {
            if (item.msgtype == "GPS")
            {
                if (!DFLog.logformat.ContainsKey("GPS"))
                    return null;

                int index = DFLog.FindMessageOffset("GPS", "Lat");
                if (index == -1)
                {
                    return null;
                }

                int index2 = DFLog.FindMessageOffset("GPS", "Lng");
                if (index2 == -1)
                {
                    return null;
                }

                int index3 = DFLog.FindMessageOffset("GPS", "Status");
                if (index3 == -1)
                {
                    return null;
                }

                try
                {
                    if (double.Parse(item.items[index3].ToString(), System.Globalization.CultureInfo.InvariantCulture) != 3)
                    {
                        return null;
                    }

                    string lat = item.items[index].ToString();
                    string lng = item.items[index2].ToString();

                    PointLatLng pnt = new PointLatLng() { };
                    pnt.Lat = double.Parse(lat, System.Globalization.CultureInfo.InvariantCulture);
                    pnt.Lng = double.Parse(lng, System.Globalization.CultureInfo.InvariantCulture);

                    return pnt;
                }
                catch { }
            }

            return null;
        }

        int FindInArray(string[] array, string find)
        {
            int a = 0;
            foreach (string item in array)
            {
                if (item == find)
                {
                    return a;
                }
                a++;
            }
            return -1;
        }

        private void leftorrightaxis(bool left, CurveItem myCurve)
        {
            if (!left)
            {
                myCurve.Label.Text += " R";
                myCurve.IsY2Axis = true;
                myCurve.YAxisIndex = 0;
                zg1.GraphPane.Y2Axis.IsVisible = true;
            }
            else if (left)
            {
                myCurve.IsY2Axis = false;
            }
        }

        private void BUT_cleargraph_Click(object sender, EventArgs e)
        {
            zg1.GraphPane.CurveList.Clear();
            zg1.GraphPane.GraphObjList.Clear();
            zg1.Invalidate();

            UntickTreeView();
        }

        private void BUT_loadlog_Click(object sender, EventArgs e)
        {
            // clear existing lists
            zg1.GraphPane.CurveList.Clear();
            // reload
            Form1_Load(sender, e);
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Point mp = Control.MousePosition;

            List<string> options = new List<string>();

            foreach (var item in logdata)
            {
                if (item.msgtype == null)
                    continue;
    
                string celldata = item.msgtype.Trim();
                if (!options.Contains(celldata))
                {
                    options.Add(celldata);
                }
            }

            Controls.OptionForm opt = new Controls.OptionForm();

            opt.StartPosition = FormStartPosition.Manual;
            opt.Location = mp;

            opt.Combobox.DataSource = options;
            opt.Button1.Text = "Filter";
            opt.Button2.Text = "Cancel";

            opt.ShowDialog(this);

            if (opt.SelectedItem != "")
            {
                logdatafilter.Clear();

                int a = 0;
                foreach (var item in logdata)
                {
                    if (item.msgtype == opt.SelectedItem) 
                    {
                        logdatafilter.Add(a,item);
                        a++; 
                    }                                   
                }

                if (!MainV2.MONO)
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.RowCount = logdatafilter.Count;
                }
            }
            else
            {
                logdatafilter.Clear();
                if (!MainV2.MONO)
                {
                    dataGridView1.Rows.Clear();
                    dataGridView1.RowCount = logdata.Count;
                }
            }

            /*
            dataGridView1.SuspendLayout();
            
            foreach (DataGridViewRow datarow in dataGridView1.Rows)
            {
                string celldata = datarow.Cells[0].Value.ToString().Trim();
                if (celldata == opt.SelectedItem || opt.SelectedItem == "")
                    datarow.Visible = true;
                else
                {
                    try
                    {
                        datarow.Visible = false;
                    }
                    catch { }
                }
            }

            dataGridView1.ResumeLayout();
             * */
            dataGridView1.Invalidate();
        }

        void BUT_go_Click(object sender, EventArgs e)
        {
            Controls.MyButton but = sender as Controls.MyButton;


        }

        /// <summary>
        /// Update row number display for those only in view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            var grid = sender as DataGridView;
            var rowIdx = grid.Rows[e.RowIndex].Cells[0].Value.ToString();

            var centerFormat = new StringFormat()
            {
                // right alignment might actually make more sense for numbers
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };

            var headerBounds = new Rectangle(e.RowBounds.Left, e.RowBounds.Top, grid.RowHeadersWidth, e.RowBounds.Height);
            e.Graphics.DrawString(rowIdx, this.Font, new SolidBrush(this.ForeColor), headerBounds, centerFormat);

            //
        }

        private void BUT_Graphit_R_Click(object sender, EventArgs e)
        {
            graphit_clickprocess(false);
        }

        private void zg1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            try
            {
                DrawModes();
                DrawErrors();
                DrawTime();
            }
            catch { }
        }

        private void CHK_map_CheckedChanged(object sender, EventArgs e)
        {
            splitContainer2.Panel2Collapsed = !splitContainer2.Panel2Collapsed;

            if (CHK_map.Checked)
            {
                log.Info("Get map");

                DrawMap();

                log.Info("map done");
            }
        }

        private void BUT_removeitem_Click(object sender, EventArgs e)
        {
            Point mp = Control.MousePosition;

            Controls.OptionForm opt = new Controls.OptionForm();

            opt.StartPosition = FormStartPosition.Manual;
            opt.Location = mp;

            List<string> list = new List<string>();

            zg1.GraphPane.CurveList.ForEach(x => list.Add(x.Label.Text));

            opt.Combobox.DataSource = list.ToArray();
            opt.Button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            opt.Button1.Text = "Remove";
            opt.Button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            opt.Button2.Text = "Cancel";

            if (opt.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {
                if (opt.SelectedItem != "")
                {
                    foreach (var item in zg1.GraphPane.CurveList)
                    {
                        if (item.Label.Text == opt.SelectedItem)
                        {
                            zg1.GraphPane.CurveList.Remove(item);
                            break;
                        }
                    }
                }
            }

            zg1.Invalidate();
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {

        }

        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node != null && e.Node.Parent != null)
            {
                // set the check if we right click
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    e.Node.Checked = !e.Node.Checked;
                }

                if (e.Node.Checked)
                {
                    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        GraphItem(e.Node.Parent.Text, e.Node.Text, false);
                    }
                    else
                    {
                        GraphItem(e.Node.Parent.Text, e.Node.Text, true);
                    }
                }
                else
                {
                    List<CurveItem> removeitems = new List<CurveItem>();

                    foreach (var item in zg1.GraphPane.CurveList)
                    {
                        if (item.Label.Text.StartsWith(e.Node.Text))
                        {
                            removeitems.Add(item);
                            //break;
                        }
                    }

                    foreach (var item in removeitems)
                        zg1.GraphPane.CurveList.Remove(item);
                }

                zg1.Invalidate();
            }
        }

        private void CMB_preselect_SelectedIndexChanged(object sender, EventArgs e)
        {
            displaylist selectlist = (displaylist)CMB_preselect.SelectedValue;

            if (selectlist.items == null)
                return;

            BUT_cleargraph_Click(null, null);

            foreach (var item in selectlist.items)
            {
                try
                {
                    GraphItem(item.type, item.field, item.left);
                }
                catch { }
            }

        }

        private void treeView1_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            var area = e.Node.Bounds;

            if (e.Node.Parent == null)
            {
                area.X -= 17;
                area.Width += 17;

            }

            using (SolidBrush brush = new SolidBrush(treeView1.BackColor))
            {
                e.Graphics.FillRectangle(brush, area);
            }



            TextRenderer.DrawText(e.Graphics, e.Node.Text, treeView1.Font, e.Node.Bounds, treeView1.ForeColor, treeView1.BackColor);

            if ((e.State & TreeNodeStates.Focused) == TreeNodeStates.Focused)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, e.Node.Bounds, treeView1.ForeColor, treeView1.BackColor);
            }
        }

        private void LogBrowse_FormClosed(object sender, FormClosedEventArgs e)
        {
            logdata = null;
            m_dtCSV = null;
            dataGridView1.DataSource = null;
            mapoverlay = null;
            GC.Collect();
        }

        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                var item = logdata[e.RowIndex];

                if (logdatafilter.Count > 0)
                {
                    item = (DFLog.DFItem)logdatafilter[e.RowIndex];
                }

                if (e.ColumnIndex == 0)
                {
                    e.Value = item.lineno;
                }
                else if (e.ColumnIndex == 1)
                {
                    e.Value = item.time.ToString("yyyy-MM-dd HH:mm:ss.fff");
                }
                else if (item.items != null && e.ColumnIndex < item.items.Length + 2)
                {
                    e.Value = item.items[e.ColumnIndex - 2];
                }
                else
                {
                    e.Value = null;
                }
            
            }
            catch { }
        }
        

        private void zg1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            PointF ptClick = new PointF(e.X, e.Y);
            double x,y;
            zg1.GraphPane.ReverseTransform(ptClick, out x, out y);

            GoToSample((int)x, true, false, true);


        }

        private void scrollGrid(DataGridView dataGridView, int index)
        {
            int halfWay = (dataGridView.DisplayedRowCount(false)/2);

            if ((index < 0) && (dataGridView.SelectedRows.Count > 0))
            {
                index = dataGridView.SelectedRows[0].Index;
            }

            if (dataGridView.FirstDisplayedScrollingRowIndex + halfWay > index ||
                (dataGridView.FirstDisplayedScrollingRowIndex + dataGridView.DisplayedRowCount(false)-halfWay) <= index)
            {
                int targetRow = index;
               
                targetRow = Math.Max(targetRow-halfWay, 0);
                try
                {
                    dataGridView.FirstDisplayedScrollingRowIndex = targetRow;
                }
                catch 
                { 
                    //soft fail
                }
            }
        }

        bool GetTimeFromRow(int lineNumber, out int millis)
        {
            bool ret = false;
            millis = 0;

            if (!DFLog.logformat.ContainsKey("IMU"))
                return ret;

            int index = DFLog.FindMessageOffset("IMU", "TimeMS");
            if (index < 0)
                return ret;

            const int maxSearch = 100;


            for (int i = 0; i < maxSearch; i++)
            {
                for (int s = -1; s < 2; s = s + 2)
                {
                    int r = lineNumber + s * i;
                    if ((r >= 0) && (r < m_dtCSV.Rows.Count))
                    {
                        DataRow datarow = m_dtCSV.Rows[r];

                        if (datarow[1].ToString() == "IMU")
                        {
                            try
                            {

                                string mil = datarow[index + 2].ToString();
                                millis = int.Parse(mil);
                                ret = true;
                                break;

                            }
                            catch { }
                        }
                    }
                }
                if (ret)
                    break;
            }

            return ret;
        }        

        bool GetGPSFromRow(int lineNumber, out PointLatLng pt)
        {
            bool ret = false;
            pt = new PointLatLng();

            if (!DFLog.logformat.ContainsKey("GPS"))
                return ret;

            int index = DFLog.FindMessageOffset("GPS", "Lat");
            int index2 = DFLog.FindMessageOffset("GPS", "Lng");
            int index3 = DFLog.FindMessageOffset("GPS", "Status");

            if ((index < 0) || (index2 < 0) || (index3 < 0))
                return ret;

            const int maxSearch = 100;


            


            for (int i = 0; i < maxSearch; i++)
            {
                for (int s = -1; s < 2; s = s + 2)
                {
                    int r = lineNumber + s * i;
                    if ((r >= 0) && (r < m_dtCSV.Rows.Count))
                    {
                        DataRow datarow = m_dtCSV.Rows[r];
                        string option = datarow[typecoloum].ToString();

                        if (option == "GPS")
                        {
                            try
                            {
                                if (double.Parse(datarow[index3 + 2].ToString(), System.Globalization.CultureInfo.InvariantCulture) != 3)
                                {

                                    continue;
                                }

                                string lat = datarow[index + 2].ToString();
                                string lng = datarow[index2 + 2].ToString();

                                //PointLatLng pnt = new PointLatLng() { };
                                pt.Lat = double.Parse(lat, System.Globalization.CultureInfo.InvariantCulture);
                                pt.Lng = double.Parse(lng, System.Globalization.CultureInfo.InvariantCulture);

                                ret = true;
                                break;

                            }
                            catch { }
                        }
                    }
                }
                if (ret)
                    break;
            }


            return ret;
        }
        
        





        private void myGMAP1_OnRouteClick(GMapRoute item, MouseEventArgs e)
        {

            if ((item.Name != null) && (item.Name.StartsWith("route_")))
            {
                LogRouteInfo lri = item.Tag as LogRouteInfo;
                if (lri != null)
                { 
                    //cerco il punto più vicino
                    MissionPlanner.Utilities.PointLatLngAlt pt2 = new MissionPlanner.Utilities.PointLatLngAlt(myGMAP1.FromLocalToLatLng(e.X, e.Y));
                    double dBest = double.MaxValue;
                    int nBest = 0;
                    for (int i = 0; i < item.LocalPoints.Count; i++)
                    {
                        PointLatLng pt = item.Points[i];
                        double d = Math.Sqrt((pt.Lat - pt2.Lat) * (pt.Lat - pt2.Lat) + (pt.Lng - pt2.Lng) * (pt.Lng - pt2.Lng));
                        if (d < dBest)
                        {
                            dBest = d;
                            nBest = i;
                        }
                    }
                    double perc = (double)nBest / (double) item.LocalPoints.Count;
                    int SampleID = (int)(lri.firstpoint + (lri.lastpoint - lri.firstpoint) * perc);

                    if ((lri.samples.Count > 0) && (nBest < lri.samples.Count))
                        SampleID = lri.samples[nBest];

                    GoToSample(SampleID, false, true, true);

                    

                    //debugging route click
                    //GMapMarker pos2 = new GMarkerGoogle(pt2, GMarkerGoogleType.orange_dot);
                    //markeroverlay.Markers.Add(pos2);

                }

            }
        }

        private void GoToSample(int SampleID, bool movemap, bool movegraph, bool movegrid)
        {
            bool zoomgraph = false;

            m_currSampleID = SampleID;

            markeroverlay.Markers.Clear();

            PointLatLng pt1;
            if (GetGPSFromRow(SampleID, out pt1))
            {
                MissionPlanner.Utilities.PointLatLngAlt pt3 = new MissionPlanner.Utilities.PointLatLngAlt(pt1);
                GMapMarker pos3 = new GMarkerGoogle(pt3, GMarkerGoogleType.pink_dot);
                markeroverlay.Markers.Add(pos3);
            }

            if (movemap)
            {
                //double z = myGMAP1.Zoom;
                //myGMAP1.ZoomAndCenterMarkers(markeroverlay.Id);
                //myGMAP1.MarkersEnabled = true;
                //myGMAP1.Zoom = z;
                myGMAP1.Position = pt1;
            }

            //move the graph "cursor"
            if (m_cursorLine != null)
            {
                zg1.GraphPane.GraphObjList.Remove(m_cursorLine);
            }
            m_cursorLine = new LineObj(Color.Black, SampleID, 0, SampleID, 1);

            m_cursorLine.Location.CoordinateFrame = CoordType.XScaleYChartFraction; // This do the trick !
            m_cursorLine.IsClippedToChartRect = true;
            m_cursorLine.Line.Style = System.Drawing.Drawing2D.DashStyle.Dash;
            m_cursorLine.Line.Width = 2f;
            m_cursorLine.Line.Color = Color.LightGray;
            m_cursorLine.ZOrder = ZOrder.E_BehindCurves;
            zg1.GraphPane.GraphObjList.Add(m_cursorLine);
            

            if (movegraph)
            {
                double delta = zg1.GraphPane.XAxis.Scale.Max - zg1.GraphPane.XAxis.Scale.Min;
                if (zoomgraph)
                {
                    delta = 2 * 60.0 / 20;
                    delta = Math.Max(100, delta);
                }
                zg1.GraphPane.XAxis.Scale.Min = SampleID - delta / 2;
                zg1.GraphPane.XAxis.Scale.Max = SampleID + delta / 2;
                zg1.AxisChange();                
            }
            zg1.Invalidate();


            if (movegrid)
            {
                scrollGrid(dataGridView1, SampleID);
                dataGridView1.CurrentCell = dataGridView1.Rows[SampleID].Cells[1];

                dataGridView1.ClearSelection();
                dataGridView1.Rows[(int)SampleID].Selected = true;
                dataGridView1.Rows[(int)SampleID].Cells[1].Selected = true;
            }


            
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int x = e.RowIndex;
            if ((x >= 0) && (x < dataGridView1.Rows.Count))
            {
                GoToSample(x, true, true, false);
            }
        }
       



        
    }
}