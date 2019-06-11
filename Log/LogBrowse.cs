using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.IO;
using log4net;
using ZedGraph; // Graphs
using System.Xml;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MissionPlanner.Controls;
using GMap.NET;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;
using MissionPlanner.ArduPilot;
using MissionPlanner.Utilities;

namespace MissionPlanner.Log
{
    public partial class LogBrowse : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static string lastLogDir;

        CollectionBuffer logdata;
        Hashtable logdatafilter = new Hashtable();

        List<TextObj> ModeCache = new List<TextObj>();
        List<TextObj> ModePolyCache = new List<TextObj>();
        List<TextObj> MSGCache = new List<TextObj>();
        List<TextObj> ErrorCache = new List<TextObj>();
        List<TextObj> EVCache = new List<TextObj>();
        List<TextObj> TimeCache = new List<TextObj>();
        DFLog.DFItem[] gpscache = new DFLog.DFItem[0];

        const int typecoloum = 2;

        List<PointPairList> listdata = new List<PointPairList>();
        GMapOverlay mapoverlay;
        GMapOverlay markeroverlay;
        LineObj m_cursorLine = null;
        Hashtable dataModifierHash = new Hashtable();

        DFLog dflog;
        public string logfilename;

        private bool readmavgraphsxml_runonce = false;

        class DataModifer
        {
            private readonly bool isValid;
            public readonly string commandString;
            public double offset = 0;
            public double scalar = 1;
            public bool doOffsetFirst = false;

            public DataModifer()
            {
                this.commandString = "";
                this.isValid = false;
            }

            public DataModifer(string _commandString)
            {
                this.commandString = _commandString;
                this.isValid = ParseCommandString(_commandString);
            }

            private bool ParseCommandString(string _commandString)
            {
                if (_commandString == null)
                {
                    return false;
                }

                char[] splitOnThese = {' ', ','};
                string[] split = _commandString.Trim().Split(splitOnThese, 2, StringSplitOptions.RemoveEmptyEntries);

                if (split.Length < 1)
                {
                    return false;
                }

                for (int i = 0; i < split.Length; i++)
                {
                    string strTrimmed = split[i].Trim();

                    // each command is a minimum of 2 chars
                    // expecting: x123, /5, +1000, *10, *0.01, -50,
                    if (strTrimmed.Length < 2)
                    {
                        return false;
                    }

                    char cmd = strTrimmed[0];
                    string param = strTrimmed.Substring(1);
                    double value = 0;

                    if (double.TryParse(param, out value) == false)
                    {
                        return false;
                    }

                    switch (cmd)
                    {
                        case 'x':
                        case '*':
                            this.scalar = value;
                            break;
                        case '\\':
                        case '/':
                            this.scalar = 1.0/value;
                            break;

                        case '+':
                            this.doOffsetFirst = (i == 0);
                            this.offset = value;
                            break;
                        case '-':
                            this.doOffsetFirst = (i == 0);
                            this.offset = -value;
                            break;

                        default:
                            return false;
                    } // switch
                } // for i
                return true;
            }

            public bool IsValid()
            {
                return this.isValid;
            }

            public static string GetNodeName(string parent, string child)
            {
                return parent + "." + child;
            }
        }


        class displayitem
        {
            public string type;
            public string field;
            public string expression;
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
            new displaylist() {Name = "None"},
            new displaylist()
            {
                Name = "Mechanical Failure",
                items = new displayitem[]
                {
                    new displayitem() {type = "ATT", field = "Roll"},
                    new displayitem() {type = "ATT", field = "DesRoll"},
                    new displayitem() {type = "ATT", field = "Pitch"},
                    new displayitem() {type = "ATT", field = "DesPitch"},
                    new displayitem() {type = "CTUN", field = "Alt", left = false},
                    new displayitem() {type = "CTUN", field = "DAlt", left = false}
                }
            },
            new displaylist()
            {
                Name = "Mechanical Failure - Stab",
                items =
                    new displayitem[]
                    {
                        new displayitem() {type = "ATT", field = "Roll"},
                        new displayitem() {type = "ATT", field = "DesRoll"}
                    }
            },
            new displaylist()
            {
                Name = "Mechanical Failure - Auto",
                items =
                    new displayitem[]
                    {
                        new displayitem() {type = "ATT", field = "Roll"},
                        new displayitem() {type = "NTUN", field = "DRoll"}
                    }
            },
            new displaylist()
            {
                Name = "Vibrations",
                items =
                    new displayitem[]
                    {
                        new displayitem() {type = "IMU", field = "AccX"},
                        new displayitem() {type = "IMU", field = "AccY"},
                        new displayitem() {type = "IMU", field = "AccZ"}
                    }
            },
            new displaylist()
            {
                Name = "Vibrations 3.3",
                items = new displayitem[]
                {
                    new displayitem() {type = "VIBE", field = "VibeX"},
                    new displayitem() {type = "VIBE", field = "VibeY"},
                    new displayitem() {type = "VIBE", field = "VibeZ"}
                    , new displayitem() {type = "VIBE", field = "Clip0", left = false},
                    new displayitem() {type = "VIBE", field = "Clip1", left = false},
                    new displayitem() {type = "VIBE", field = "Clip2", left = false}
                }
            },
            new displaylist()
            {
                Name = "GPS Glitch",
                items =
                    new displayitem[]
                    {
                        new displayitem() {type = "GPS", field = "HDop"},
                        new displayitem() {type = "GPS", field = "NSats", left = false}
                    }
            },
            new displaylist()
            {
                Name = "Power Issues",
                items = new displayitem[]
                {
                    new displayitem() {type = "CURR", field = "Vcc"},
                    new displayitem() {type = "POWR", field = "Vcc"}
                }
            },
            new displaylist()
            {
                Name = "Errors",
                items = new displayitem[] {new displayitem() {type = "ERR", field = "ECode"}}
            },
            new displaylist()
            {
                Name = "Battery Issues",
                items =
                    new displayitem[]
                    {
                        new displayitem() {type = "CTUN", field = "ThrIn"},
                        new displayitem() {type = "CURR", field = "ThrOut"},
                        new displayitem() {type = "CURR", field = "Volt", left = false}
                    }
            },
            new displaylist()
            {
                Name = "imu consistency xyz",
                items = new displayitem[]
                {
                    new displayitem() {type = "IMU", field = "AccX"},
                    new displayitem() {type = "IMU2", field = "AccX"},
                    new displayitem() {type = "IMU", field = "AccY"},
                    new displayitem() {type = "IMU2", field = "AccY"},
                    new displayitem() {type = "IMU", field = "AccZ", left = false},
                    new displayitem() {type = "IMU2", field = "AccZ", left = false},
                }
            },
            new displaylist()
            {
                Name = "mag consistency xyz",
                items = new displayitem[]
                {
                    new displayitem() {type = "MAG", field = "MagX"},
                    new displayitem() {type = "MAG2", field = "MagX"},
                    new displayitem() {type = "MAG", field = "MagY", left = false},
                    new displayitem() {type = "MAG2", field = "MagY", left = false},
                    new displayitem() {type = "MAG", field = "MagZ"},
                    new displayitem() {type = "MAG2", field = "MagZ"},
                }
            },
            new displaylist()
            {
                Name = "copter loiter",
                items = new displayitem[]
                {
                    new displayitem() {type = "NTUN", field = "DVelX"},
                    new displayitem() {type = "NTUN", field = "VelX"},
                    new displayitem() {type = "NTUN", field = "DVelY"},
                    new displayitem() {type = "NTUN", field = "VelY"},
                }
            },
            new displaylist()
            {
                Name = "copter althold",
                items = new displayitem[]
                {
                    new displayitem() {type = "CTUN", field = "BarAlt"},
                    new displayitem() {type = "CTUN", field = "DAlt"},
                    new displayitem() {type = "CTUN", field = "Alt"},
                    new displayitem() {type = "GPS", field = "Alt"},
                }
            },
            new displaylist()
            {
                Name = "ekf VEL tune",
                items = new displayitem[]
                {
                    new displayitem() {type = "NKF3", field = "IVN"},
                    new displayitem() {type = "NKF3", field = "IPN"},
                    new displayitem() {type = "NKF3", field = "IVE"},
                    new displayitem() {type = "NKF3", field = "IPE"},
                    new displayitem() {type = "NKF3", field = "IVD"},
                    new displayitem() {type = "NKF3", field = "IPD"},
                }
            },
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
                catch
                {
                    CustomMessageBox.Show("Line Doesn't Exist");
                }

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        public LogBrowse()
        {
            InitializeComponent();

            ThemeManager.ApplyThemeTo(this);

            mapoverlay = new GMapOverlay("overlay");
            markeroverlay = new GMapOverlay("markers");

            if (GCSViews.FlightData.mymap != null)
                myGMAP1.MapProvider = GCSViews.FlightData.mymap.MapProvider;

            myGMAP1.Overlays.Add(mapoverlay);
            myGMAP1.Overlays.Add(markeroverlay);

            dataGridView1.RowUnshared += dataGridView1_RowUnshared;

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        public class graphitem
        {
            public string name;
            public List<string> expressions = new List<string>();
            public string description;
        }

        private void readmavgraphsxml()
        {
            if (readmavgraphsxml_runonce)
                return;

            readmavgraphsxml_runonce = true;

            List<graphitem> items = new List<graphitem>();

            using (
                XmlReader reader =
                    XmlReader.Create(Settings.GetRunningDirectory() + "mavgraphs.xml"))
            {
                while (reader.Read())
                {
                    if (reader.ReadToFollowing("graph"))
                    {
                        graphitem newGraphitem = new graphitem();

                        for (int a = 0; a < reader.AttributeCount; a++)
                        {
                            reader.MoveToAttribute(a);
                            if (reader.Name.ToLower() == "name")
                            {
                                newGraphitem.name = reader.Value;
                            }
                        }

                        reader.MoveToElement();

                        XmlReader inner = reader.ReadSubtree();

                        while (inner.Read())
                        {
                            if (inner.IsStartElement())
                            {
                                if (inner.Name.ToLower() == "expression")
                                    newGraphitem.expressions.Add(inner.ReadString().Trim());
                                else if (inner.Name.ToLower() == "description")
                                    newGraphitem.description = inner.ReadString().Trim();
                            }
                        }

                        processGraphItem(newGraphitem);

                        items.Add(newGraphitem);
                    }
                }
            }
        }

        void processGraphItem(graphitem graphitem)
        {
            List<displayitem> list = new List<displayitem>();

            foreach (var expression in graphitem.expressions)
            {
                var items = expression.Split(new char[] {' ', '\t', '\n'}, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in items)
                {
                    var matchs = Regex.Matches(item.Trim(), @"^([A-z0-9_]+)\.([A-z0-9_]+)[:2]*$");

                    if (matchs.Count > 0)
                    {
                        foreach (Match match in matchs)
                        {
                            var temp = new displayitem();
                            // right axis
                            if (item.EndsWith(":2"))
                                temp.left = false;

                            temp.type = match.Groups[1].Value.ToString();
                            temp.field = match.Groups[2].Value.ToString();

                            list.Add(temp);
                        }
                    }
                    else
                    {
                        var temp = new displayitem();
                        if (item.EndsWith(":2"))
                            temp.left = false;
                        temp.expression = item;
                        temp.type = item;
                        list.Add(temp);
                    }
                }
            }

            var dispitem = new displaylist()
            {
                Name = graphitem.name,
                items = list.ToArray()
            };

            graphs.Add(dispitem);
        }

        void dataGridView1_RowUnshared(object sender, DataGridViewRowEventArgs e)
        {
        }

        private void LogBrowse_Load(object sender, EventArgs e)
        {
            mapoverlay.Clear();
            markeroverlay.Clear();

            logdatafilter.Clear();

            if (logdata != null)
                logdata.Clear();

            GC.Collect();

            ErrorCache = new List<TextObj>();
            EVCache = new List<TextObj>();
            ModeCache = new List<TextObj>();
            ModePolyCache = new List<TextObj>();
            TimeCache = new List<TextObj>();
            MSGCache = new List<TextObj>();
            gpscache = new DFLog.DFItem[0];

            chk_time_CheckedChanged(null, null);

            if (!File.Exists(logfilename))
            {
                using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
                {
                    openFileDialog1.Filter = "Log Files|*.log;*.bin;*.BIN;*.LOG";
                    openFileDialog1.FilterIndex = 2;
                    openFileDialog1.Multiselect = true;
                    openFileDialog1.InitialDirectory = lastLogDir ?? Settings.Instance.LogDir;

                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        lastLogDir = Path.GetDirectoryName(openFileDialog1.FileName);

                        int a = 0;
                        foreach (var fileName in openFileDialog1.FileNames)
                        {
                            Loading.ShowLoading(fileName, this);

                            if (a == 0)
                            {
                                // load first file
                                logfilename = fileName;
                                ThreadPool.QueueUserWorkItem(o => LoadLog(logfilename));
                            }
                            else
                            {
                                // load additional files in new windows
                                if (File.Exists(fileName))
                                {
                                    LogBrowse browse = new LogBrowse();
                                    browse.logfilename = fileName;
                                    browse.Show(this);
                                }
                            }
                            a++;
                        }
                    }
                    else
                    {
                        this.BeginInvoke((Action) delegate
                        {
                            this.Close();
                        });
                        return;
                    }
                }
            }
            else
            {
                ThreadPool.QueueUserWorkItem(o => LoadLog(logfilename));
            }

            log.Info("LogBrowse_Load Done");
        }

        public void LoadLog(string FileName)
        {
            while (!this.IsHandleCreated)
                Thread.Sleep(100);

            Loading.ShowLoading(Strings.Scanning_File, this);

            try
            {
                Stream stream;

                stream = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.Read);

                log.Info("before read " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));

                logdata = new CollectionBuffer(stream);

                dflog = logdata.dflog;

                log.Info("got log lines " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));

                log.Info("process to datagrid " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));

                Loading.ShowLoading("Scanning coloum widths", this);

                colcount = 0;

                foreach (var msgid in logdata.FMT)
                {
                    if (msgid.Value.Item4 == null)
                        continue;
                    var colsplit = msgid.Value.Item4.FirstOrDefault().ToString().Split(',').Length;
                    colcount = Math.Max(colcount, (msgid.Value.Item4.Length + typecoloum + colsplit));
                }

                log.Info("Done " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));

                this.BeginInvokeIfRequired(()=>
                {
                    LoadLog2(FileName, logdata, colcount);
                });
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show("Failed to read File: " + ex.ToString());
                return;
            }

            log.Info("LoadLog Done");
        }

        void LoadLog2(String FileName, CollectionBuffer logdata, int colcount)
        {
            this.Text = "Log Browser - " + Path.GetFileName(FileName);

            CreateChart(zg1);

            ResetTreeView(logdata.SeenMessageTypes);

            zg1_ZoomEvent(zg1, null, null);

            chk_datagrid.Checked = Settings.Instance.GetBoolean("LB_Grid", false);
            chk_time.Checked = Settings.Instance.GetBoolean("LB_Time", true);
            CHK_map.Checked = Settings.Instance.GetBoolean("LB_Map", false);
            chk_errors.Checked = Settings.Instance.GetBoolean("LB_Error", true);
            chk_mode.Checked = Settings.Instance.GetBoolean("LB_Mode", true);
            chk_msg.Checked = Settings.Instance.GetBoolean("LB_MSG", true);

            chk_datagrid.CheckedChanged += (e, a) => { Settings.Instance["LB_Grid"] = chk_datagrid.Checked.ToString(); };
            chk_time.CheckedChanged += (e, a) => { Settings.Instance["LB_Time"] = chk_time.Checked.ToString(); };
            CHK_map.CheckedChanged += (e, a) => { Settings.Instance["LB_Map"] = CHK_map.Checked.ToString(); };
            chk_errors.CheckedChanged += (e, a) => { Settings.Instance["LB_Error"] = chk_errors.Checked.ToString(); };
            chk_mode.CheckedChanged += (e, a) => { Settings.Instance["LB_Mode"] = chk_mode.Checked.ToString(); };
            chk_msg.CheckedChanged += (e, a) => { Settings.Instance["LB_MSG"] = chk_msg.Checked.ToString(); };

            Loading.Close();

            if (dflog.logformat.Count == 0)
            {
                CustomMessageBox.Show(Strings.WarningLogBrowseFMTMissing, Strings.ERROR);
                this.Close();
                return;
            }

            // update preselection graphs
            readmavgraphsxml();

            //CMB_preselect.DisplayMember = "Name";
            CMB_preselect.DataSource = null;
            CMB_preselect.DataSource = graphs;

            log.Info("LoadLog2 Done");
        }

        private void populateRowData(int rowstartoffset, int rowIndex, int destDGV = -1)
        {
            //Console.WriteLine("populateRowData {0} {1} {2}", rowstartoffset, rowIndex, destDGV);
            var DGVrow = (destDGV == -1) ? rowstartoffset + rowIndex : destDGV;

            var cellcount = dataGridView1.Rows[DGVrow].Cells.Count;
            for (int i = 0; i < cellcount; i++)
            {
                if (DGVrow > dataGridView1.Rows.Count)
                {
                    dataGridView1.Rows[DGVrow].Cells[i].Value = "";
                    continue;
                }

                var data = new DataGridViewCellValueEventArgs(i, rowstartoffset + rowIndex);

                dataGridView1_CellValueNeeded(dataGridView1, data);

                string existing = dataGridView1.Rows[DGVrow].Cells[i].Value as string;
                string newvalue = data.Value as string;

                if (existing == newvalue)
                {
                    continue;
                }

                //Console.WriteLine("set data {0} = {1}", dataGridView1.Rows[DGVrow].Cells[i].Value, data.Value);
                dataGridView1.Rows[DGVrow].Cells[i].Value = String.IsNullOrEmpty(newvalue)  ? "" : newvalue;
            }
            //Console.WriteLine("populateRowData done {0} {1} {2}", rowstartoffset, rowIndex, destDGV);
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

        private void ResetTreeView(List<string> seenmessagetypes)
        {
            treeView1.Nodes.Clear();
            dataModifierHash = new Hashtable();

            var sorted = new SortedList(dflog.logformat);

            foreach (DFLog.Label item in sorted.Values)
            {
                TreeNode tn = new TreeNode(item.Name);

                if (seenmessagetypes.Contains(item.Name))
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
            catch
            {
            }
            try
            {
                // process the line type
                string option = dataGridView1[typecoloum, e.RowIndex].EditedFormattedValue.ToString();

                // new self describing log
                if (dflog.logformat.ContainsKey(option))
                {
                    int a = typecoloum + 1;
                    foreach (string name in dflog.logformat[option].FieldNames)
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
                        /*
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
                         */
                    }

                    return;
                }

                if (option == "")
                    return;

                if (option.StartsWith("PID-"))
                    option = "PID-1";

                using (
                    XmlReader reader =
                        XmlReader.Create(Settings.GetRunningDirectory() +
                                         "dataflashlog.xml"))
                {
                    reader.Read();
                    reader.ReadStartElement("LOGFORMAT");
                    if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduPlane)
                    {
                        reader.ReadToFollowing("APM");
                    }
                    else if (MainV2.comPort.MAV.cs.firmware == Firmwares.ArduRover)
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
            catch
            {
                log.Info("DGV logbrowse error");
            }
        }

        Color[] colours = new Color[]
        {
            Color.Red,
            Color.Green,
            Color.Blue,
            Color.Orange,
            Color.Yellow,
            Color.Violet,
            Color.Pink,
            Color.Teal,
            Color.Wheat,
            Color.Silver,
            Color.Purple,
            Color.Aqua,
            Color.Brown,
            Color.WhiteSmoke
        };

        private Color[] colourspastal = new Color[]
        {
            ConvertFromRange(1.0, 0, 0),

            ConvertFromRange(0, 1.0, 0),

            ConvertFromRange(0, 0, 1.0),



            ConvertFromRange(0, 1.0, 1.0),

            ConvertFromRange(1.0, 0, 1.0),

            ConvertFromRange(1.0, 1.0, 0),



            ConvertFromRange(1.0, 0.5, 0),

            ConvertFromRange(1.0, 0, 0.5),

            ConvertFromRange(0.5, 1.0, 0),

            ConvertFromRange(0, 1.0, 0.5),

            ConvertFromRange(0.5, 0, 1.0),

            ConvertFromRange(0, 0.5, 1.0),

            ConvertFromRange(1.0, 0.5, 0.5),

            ConvertFromRange(0.5, 1.0, 0.5),

            ConvertFromRange(0.5, 0.5, 1.0),
            ConvertFromHex("#5757FF"),
            ConvertFromHex("#62A9FF"),
            ConvertFromHex("#62D0FF"),
            ConvertFromHex("#06DCFB"),
            ConvertFromHex("#01FCEF"),
            ConvertFromHex("#03EBA6"),
            ConvertFromHex("#01F33E"),
            ConvertFromHex("#6A6AFF"),
            ConvertFromHex("#75B4FF"),
            ConvertFromHex("#75D6FF"),
            ConvertFromHex("#24E0FB"),
            ConvertFromHex("#1FFEF3"),
            ConvertFromHex("#03F3AB"),
            ConvertFromHex("#0AFE47"),
            ConvertFromHex("#7979FF"),
            ConvertFromHex("#86BCFF"),
            ConvertFromHex("#8ADCFF"),
            ConvertFromHex("#3DE4FC"),
            ConvertFromHex("#5FFEF7"),
            ConvertFromHex("#33FDC0"),
            ConvertFromHex("#4BFE78"),
            ConvertFromHex("#8C8CFF"),
            ConvertFromHex("#99C7FF"),
            ConvertFromHex("#99E0FF"),
            ConvertFromHex("#63E9FC"),
            ConvertFromHex("#74FEF8"),
            ConvertFromHex("#62FDCE"),
            ConvertFromHex("#72FE95"),
            ConvertFromHex("#9999FF"),
            ConvertFromHex("#99C7FF"),
            ConvertFromHex("#A8E4FF"),
            ConvertFromHex("#75ECFD"),
            ConvertFromHex("#92FEF9"),
            ConvertFromHex("#7DFDD7"),
            ConvertFromHex("#8BFEA8"),
            ConvertFromHex("#AAAAFF"),
            ConvertFromHex("#A8CFFF"),
            ConvertFromHex("#BBEBFF"),
            ConvertFromHex("#8CEFFD"),
            ConvertFromHex("#A5FEFA"),
            ConvertFromHex("#8FFEDD"),
            ConvertFromHex("#A3FEBA"),
            ConvertFromHex("#BBBBFF"),
            ConvertFromHex("#BBDAFF"),
            ConvertFromHex("#CEF0FF"),
            ConvertFromHex("#ACF3FD"),
            ConvertFromHex("#B5FFFC"),
            ConvertFromHex("#A5FEE3"),
            ConvertFromHex("#B5FFC8"),
            ConvertFromHex("#CACAFF"),
            ConvertFromHex("#D0E6FF"),
            ConvertFromHex("#D9F3FF"),
            ConvertFromHex("#C0F7FE"),
            ConvertFromHex("#CEFFFD"),
            ConvertFromHex("#BEFEEB"),
            ConvertFromHex("#CAFFD8")
        };

        public static Color ConvertFromRange(double r, double g, double b)
        {
            return Color.FromArgb(255, (int) (r * 127.0)+127, (int) (g * 127.0)+127, (int) (b * 127.0)+127);
        }

        public static Color ConvertFromHex(string hex)
        {
            hex = hex.TrimStart('#');

            var r = Convert.ToInt32(hex.Substring(0, 2), 16);
            var g = Convert.ToInt32(hex.Substring(2, 2), 16);
            var b = Convert.ToInt32(hex.Substring(4, 2), 16);

            return Color.FromArgb(20, r, g, b);
        }

        public void CreateChart(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Value Graph";
            myPane.XAxis.Title.Text = "Line Number";
            myPane.YAxis.Title.Text = "Output";

            // Show the x axis grid
            myPane.XAxis.MajorGrid.IsVisible = true;

            //myPane.XAxis.Scale.Min = 0;
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
                zg1.Invalidate();
            }
            catch
            {
            }
        }

        private void Graphit_Click(object sender, EventArgs e)
        {
            graphit_clickprocess(true);
        }

        void graphit_clickprocess(bool left = true)
        {
            if (dataGridView1 == null || dataGridView1.RowCount == 0 || dataGridView1.ColumnCount == 0)
            {
                CustomMessageBox.Show(Strings.PleaseLoadValidFile, Strings.ERROR);
                return;
            }

            if (dataGridView1.CurrentCell == null)
            {
                CustomMessageBox.Show(Strings.PleaseSelectCell, Strings.ERROR);
                return;
            }

            int col = dataGridView1.CurrentCell.ColumnIndex;
            int row = dataGridView1.CurrentCell.RowIndex;
            string type = dataGridView1[typecoloum, row].Value.ToString();

            if (col == 0)
            {
                CustomMessageBox.Show("Please pick another column, Highlight the cell you wish to graph", Strings.ERROR);
                return;
            }

            if (!dflog.logformat.ContainsKey(type))
            {
                CustomMessageBox.Show(Strings.NoFMTMessage + type, Strings.ERROR);
                return;
            }

            if ((col - typecoloum - 1) < 0)
            {
                CustomMessageBox.Show(Strings.CannotGraphField, Strings.ERROR);
                return;
            }

            if (dflog.logformat[type].FieldNames.Count <= (col - typecoloum - 1))
            {
                CustomMessageBox.Show(Strings.InvalidField, Strings.ERROR);
                return;
            }

            string fieldname = dflog.logformat[type].FieldNames[col - typecoloum - 1];

            GraphItem(type, fieldname, left);
        }

        void GraphItem(string type, string fieldname, bool left = true, bool displayerror = true,
            bool isexpression = false)
        {
            log.InfoFormat("GraphItem: {0} {1}", type, fieldname);
            DataModifer dataModifier = new DataModifer();
            string nodeName = DataModifer.GetNodeName(type, fieldname);

            foreach (var curve in zg1.GraphPane.CurveList)
            {
                // its already on the graph, abort
                if (curve.Label.Text.StartsWith(nodeName+" (") ||
                    curve.Label.Text.StartsWith(nodeName + " R ("))
                    return;
            }

            if (dataModifierHash.ContainsKey(nodeName))
            {
                dataModifier = (DataModifer) dataModifierHash[nodeName];
            }

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

            // clear the filter on add
            logdatafilter.Clear();

            if (!isexpression)
            {
                if (!dflog.logformat.ContainsKey(type))
                {
                    if (displayerror)
                        CustomMessageBox.Show(Strings.NoFMTMessage + type + " - " + fieldname, Strings.ERROR);
                    return;
                }

                log.Info("Graphing " + type + " - " + fieldname);

                Loading.ShowLoading("Graphing " + type + " - " + fieldname, this);

                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        GraphItem_GetList(fieldname, type, dflog, dataModifier, left);
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show("Failed to graph item: " + ex.Message, Strings.ERROR);
                    }
                });
            }
            else
            {
                var list1 = DFLogScript.ProcessExpression(ref dflog, ref logdata, type);
                var newlist = new PointPairList();
                list1.ForEach(a =>
                {
                    if (chk_time.Checked)
                        newlist.Add(new PointPair(new XDate(a.Item1.time), a.Item2));
                    else
                        newlist.Add(new PointPair(a.Item1.lineno, a.Item2));
                });
                GraphItem_AddCurve(newlist, type, fieldname, left);
            }
        }

        void GraphItem_GetList(string fieldname, string type, DFLog dflog, DataModifer dataModifier, bool left)
        {
            log.Info("GraphItem_GetList " + type + " " + fieldname);
            int col = dflog.FindMessageOffset(type, fieldname);

            // field does not exist
            if (col == -1)
                return;

            PointPairList list1 = new PointPairList();

            int error = 0;

            double a = 0; // row counter
            double b = 0;
            DateTime screenupdate = DateTime.MinValue;
            double value_prev = 0;

            foreach (var item in logdata.GetEnumeratorType(type))
            {
                b = item.lineno;

                if (screenupdate.Second != DateTime.Now.Second)
                {
                    Console.Write(b + " of " + logdata.Count + "     \r");
                    screenupdate = DateTime.Now;
                }

                if (item.msgtype == type)
                {
                    try
                    {
                        double value = double.Parse(item.items[col],
                            System.Globalization.CultureInfo.InvariantCulture);

                        // abandon realy bad data
                        if (Math.Abs(value) > 9.15e8)
                        {
                            a++;
                            continue;
                        }

                        if (dataModifier.IsValid())
                        {
                            if ((a != 0) && Math.Abs(value - value_prev) > 1e5)
                            {
                                // there is a glitch in the data, reject it by replacing it with the previous value
                                value = value_prev;
                            }
                            value_prev = value;

                            if (dataModifier.doOffsetFirst)
                            {
                                value += dataModifier.offset;
                                value *= dataModifier.scalar;
                            }
                            else
                            {
                                value *= dataModifier.scalar;
                                value += dataModifier.offset;
                            }
                        }

                        if (chk_time.Checked)
                        {
                            XDate time = new XDate(item.time);

                            list1.Add(time, value);
                        }
                        else
                        {
                            list1.Add(b, value);
                        }
                    }
                    catch
                    {
                        error++;
                        log.Info("Bad Data : " + type + " " + col + " " + a);
                        if (error >= 500)
                        {
                            CustomMessageBox.Show("There is to much bad data - failing");
                            break;
                        }
                    }
                }

                a++;
            }

            Invoke((Action) delegate {
                GraphItem_AddCurve(list1, type, fieldname, left);
            });
        }

        Color pickColour()
        {
            List<Color> notused = new List<Color>();
            notused.AddRange(colours);

            foreach (var curve in zg1.GraphPane.CurveList)
            {
                notused.Remove(curve.Color);
            }

            if (notused.Count > 0)
                return notused.First();

            // failback to old method
            return colours[zg1.GraphPane.CurveList.Count%colours.Length];
        }
        
        void GraphItem_AddCurve(PointPairList list1,string type, string header, bool left)
        {
            if (list1.Count < 1)
            {
                Loading.Close();
                return;
            }

            var ans = logdata.GetUnit(type, header);
            string unit = ans.Item1;
            double multiplier = ans.Item2;

            if (unit != "")
                header += " (" + unit + ")";

            if (multiplier != 0 && multiplier != 1)
            {
                for (var i = 0; i < list1.Count; i++)
                {
                    list1[i].Y *= multiplier;
                }
            }

            LineItem myCurve;

            myCurve = zg1.GraphPane.AddCurve(type + "." + header, list1,
                pickColour(), SymbolType.None);

            leftorrightaxis(left, myCurve);

            // Make sure the Y axis is rescaled to accommodate actual data
            try
            {
                zg1.AxisChange();
            }
            catch
            {
            }
            // Zoom all
            zg1.ZoomOutAll(zg1.GraphPane);

            zg1_ZoomEvent(zg1, null, null);

            // Force a redraw
            zg1.Refresh();
            Loading.Close();
        }

        async Task DrawErrors()
        {
            await Task.Run(() =>
            {
                log.Info("Start DrawErrors");
                bool top = false;
                double a = 0;

                if (ErrorCache.Count > 0)
                {
                    foreach (var item in ErrorCache)
                    {
                        item.Location.Y = zg1.GraphPane.YAxis.Scale.Max;
                        zg1.GraphPane.GraphObjList.Add(item);
                    }
                    return;
                }

                ErrorCache.Clear();

                double b = 0;

                //ErrorCache.Add(new TextObj("", -500, 0));

                if (!dflog.logformat.ContainsKey("ERR"))
                    return;

                foreach (var item in logdata.GetEnumeratorType("ERR"))
                {
                    b = item.lineno;

                    if (item.msgtype == "ERR")
                    {
                        if (!dflog.logformat.ContainsKey("ERR"))
                            return;

                        int index = dflog.FindMessageOffset("ERR", "Subsys");
                        if (index == -1)
                        {
                            continue;
                        }

                        int index2 = dflog.FindMessageOffset("ERR", "ECode");
                        if (index2 == -1)
                        {
                            continue;
                        }

                        if (chk_time.Checked)
                        {
                            XDate date = new XDate(item.time);
                            b = date.XLDate;
                        }

                        if (item.items.Length <= index)
                            continue;

                        string mode = "Err: " + ((DFLog.error_subsystem) int.Parse(item.items[index].ToString())) +
                                      "-" +
                                      item.items[index2].ToString().Trim();
                        if (top)
                        {
                            var temp = new TextObj(mode, b, zg1.GraphPane.YAxis.Scale.Max, CoordType.AxisXYScale,
                                AlignH.Left, AlignV.Top);
                            temp.FontSpec.Fill.Color = Color.Red;
                            ErrorCache.Add(temp);
                            this.BeginInvokeIfRequired(() => zg1.GraphPane.GraphObjList.Add(temp));
                        }
                        else
                        {
                            var temp = new TextObj(mode, b, zg1.GraphPane.YAxis.Scale.Max, CoordType.AxisXYScale,
                                AlignH.Left, AlignV.Bottom);
                            temp.FontSpec.Fill.Color = Color.Red;
                            ErrorCache.Add(temp);
                            this.BeginInvokeIfRequired(() => zg1.GraphPane.GraphObjList.Add(temp));
                        }

                        top = !top;
                    }

                    a++;
                }
                log.Info("End DrawErrors");
            });
        }

        async Task DrawEV()
        {
            await Task.Run(() =>
            {
                log.Info("Start DrawEV");
                bool top = false;
                double a = 0;

                if (EVCache.Count > 0)
                {
                    foreach (var item in EVCache)
                    {
                        item.Location.Y = zg1.GraphPane.YAxis.Scale.Max;
                        zg1.GraphPane.GraphObjList.Add(item);
                    }
                    return;
                }

                EVCache.Clear();

                double b = 0;

                //ErrorCache.Add(new TextObj("", -500, 0));

                if (!dflog.logformat.ContainsKey("EV"))
                    return;

                foreach (var item in logdata.GetEnumeratorType("EV"))
                {
                    b = item.lineno;

                    if (item.msgtype == "EV")
                    {
                        if (!dflog.logformat.ContainsKey("EV"))
                            return;

                        int index = dflog.FindMessageOffset("EV", "Id");
                        if (index == -1)
                        {
                            continue;
                        }

                        if (chk_time.Checked)
                        {
                            XDate date = new XDate(item.time);
                            b = date.XLDate;
                        }

                        if (item.items.Length <= index)
                            continue;

                        string mode = "EV: " + ((DFLog.events) int.Parse(item.items[index].ToString()));
                        if (top)
                        {
                            var temp = new TextObj(mode, b, zg1.GraphPane.YAxis.Scale.Max, CoordType.AxisXYScale,
                                AlignH.Left, AlignV.Top);
                            temp.FontSpec.Fill.Color = Color.Red;
                            EVCache.Add(temp);
                            this.BeginInvokeIfRequired(() => zg1.GraphPane.GraphObjList.Add(temp));
                        }
                        else
                        {
                            var temp = new TextObj(mode, b, zg1.GraphPane.YAxis.Scale.Max, CoordType.AxisXYScale,
                                AlignH.Left, AlignV.Bottom);
                            temp.FontSpec.Fill.Color = Color.Red;
                            EVCache.Add(temp);
                            this.BeginInvokeIfRequired(() => zg1.GraphPane.GraphObjList.Add(temp));
                        }

                        top = !top;
                    }

                    a++;
                }
                log.Info("End DrawEV");
            });
        }


        async Task DrawModes()
        {
            await Task.Run(() =>
            {
                log.Info("Start DrawModes");
                bool top = false;

                var prevx = zg1.GraphPane.XAxis.Scale.Min;
                int prevmodeno = 0;
                // 2% of total
                var modeheighty = zg1.GraphPane.YAxis.Scale.Max -
                                  (zg1.GraphPane.YAxis.Scale.Max - zg1.GraphPane.YAxis.Scale.Min) * 0.02;

                ModePolyCache.Clear();
                ModeCache.Clear();

                int modenum = 0;

                foreach (var item in logdata.GetEnumeratorType("MODE"))
                {
                    double a = item.lineno;

                    if (item.msgtype == "MODE")
                    {
                        if (!dflog.logformat.ContainsKey("MODE"))
                            return;

                        int index = dflog.FindMessageOffset("MODE", "Mode");
                        if (index == -1)
                        {
                            continue;
                        }

                        int indexnum = dflog.FindMessageOffset("MODE", "ModeNum");
                        if (indexnum == -1)
                        {
                            continue;
                        }

                        if (chk_time.Checked)
                        {
                            XDate date = new XDate(item.time);
                            a = date.XLDate;
                        }

                        if (item.items.Length <= index)
                            continue;

                        string mode = item.items[index].ToString().Trim();

                        prevmodeno = modenum;

                        modenum = int.Parse(item.items[indexnum].ToString().Trim());

                        var poly = new PolyObj()
                        {
                            Points = new[]
                            {
                                new PointD(prevx, modeheighty), // bl
                                new PointD(prevx, zg1.GraphPane.YAxis.Scale.Max), // tl
                                new PointD(Math.Min( Math.Max(a,prevx),zg1.GraphPane.XAxis.Scale.Max), zg1.GraphPane.YAxis.Scale.Max), // tr
                                new PointD(Math.Min( Math.Max(a,prevx),zg1.GraphPane.XAxis.Scale.Max), modeheighty), // br                                
                            },
                            Fill = new Fill(colourspastal[prevmodeno]),
                            ZOrder = ZOrder.E_BehindCurves
                        };

                        // only draw if our start position is less than the graph max and our end position is > our start (dont draw offscreen elements)
                        if (prevx < zg1.GraphPane.XAxis.Scale.Max && a > zg1.GraphPane.XAxis.Scale.Min)
                            this.BeginInvokeIfRequired(() => zg1.GraphPane.GraphObjList.Add(poly));

                        if (top)
                        {
                            var temp = new TextObj(mode, a, zg1.GraphPane.YAxis.Scale.Min, CoordType.AxisXYScale,
                                AlignH.Left, AlignV.Top);
                            ModeCache.Add(temp);
                            this.BeginInvokeIfRequired(() => zg1.GraphPane.GraphObjList.Add(temp));
                        }
                        else
                        {
                            var temp = new TextObj(mode, a, zg1.GraphPane.YAxis.Scale.Min, CoordType.AxisXYScale,
                                AlignH.Left, AlignV.Bottom);
                            ModeCache.Add(temp);
                            this.BeginInvokeIfRequired(() => zg1.GraphPane.GraphObjList.Add(temp));
                        }

                        top = !top;
                    }

                    a++;
                }

                // put from last to end of graph as well
                {
                    var a = zg1.GraphPane.XAxis.Scale.Max;
                    var poly2 = new PolyObj()
                    {
                        Points = new[]
                        {
                             new PointD(Math.Min(prevx,a), modeheighty), // bl
                                new PointD(Math.Min(prevx,a), zg1.GraphPane.YAxis.Scale.Max), // tl
                                new PointD(a, zg1.GraphPane.YAxis.Scale.Max), // tr
                                new PointD(a, modeheighty), // br   
                    },
                        Fill = new Fill(colourspastal[modenum]),
                        ZOrder = ZOrder.E_BehindCurves
                    };

                    this.BeginInvokeIfRequired(() =>
                    {
                        zg1.GraphPane.GraphObjList.Add(poly2);
                        zg1.Invalidate();
                    });
                }
                log.Info("End DrawModes");
            });
        }

        async Task DrawMSG()
        {
            await Task.Run(() =>
            {
                log.Info("Start DrawMSG");
                bool top = false;
                double a = 0;

                if (MSGCache.Count > 0)
                {
                    foreach (var item in MSGCache)
                    {
                        item.Location.Y = zg1.GraphPane.YAxis.Scale.Min;
                        this.BeginInvokeIfRequired(() => zg1.GraphPane.GraphObjList.Add(item));
                    }

                    return;
                }

                MSGCache.Clear();

                foreach (var item in logdata.GetEnumeratorType("MSG"))
                {
                    a = item.lineno;

                    if (item.msgtype == "MSG")
                    {
                        if (!dflog.logformat.ContainsKey("MSG"))
                            return;

                        int index = dflog.FindMessageOffset("MSG", "Message");
                        if (index == -1)
                        {
                            continue;
                        }

                        if (chk_time.Checked)
                        {
                            XDate date = new XDate(item.time);
                            a = date.XLDate;
                        }

                        if(item.items.Length <= index)
                            continue;

                        string mode = item.items[index].ToString().Trim();
                        if (top)
                        {
                            var temp = new TextObj(mode, a, zg1.GraphPane.YAxis.Scale.Min, CoordType.AxisXYScale,
                                AlignH.Left, AlignV.Top);
                            MSGCache.Add(temp);
                            this.BeginInvokeIfRequired(() => zg1.GraphPane.GraphObjList.Add(temp));
                        }
                        else
                        {
                            var temp = new TextObj(mode, a, zg1.GraphPane.YAxis.Scale.Min, CoordType.AxisXYScale,
                                AlignH.Left, AlignV.Bottom);
                            MSGCache.Add(temp);
                            this.BeginInvokeIfRequired(() => zg1.GraphPane.GraphObjList.Add(temp));
                        }

                        top = !top;
                    }

                    a++;
                }
                log.Info("End DrawMSG");
            });
        }

        async Task DrawTime()
        {
            await Task.Run(() =>
            {
                log.Info("Start DrawTime");
                if (chk_time.Checked)
                    return;

                int a = 0;

                DateTime starttime = DateTime.MinValue;
                UInt64 startdelta = 0;
                DateTime workingtime = starttime;

                DateTime lastdrawn = DateTime.MinValue;


                if (TimeCache.Count > 0)
                {
                    foreach (var item in TimeCache)
                    {
                        item.Location.Y = zg1.GraphPane.YAxis.Scale.Max;
                        this.BeginInvokeIfRequired(() => zg1.GraphPane.GraphObjList.Add(item));
                    }

                    return;
                }

                double b = 0;

                foreach (var item in logdata.GetEnumeratorType("GPS"))
                {
                    b = item.lineno;

                    if (item.msgtype == "GPS")
                    {
                        if (!dflog.logformat.ContainsKey("GPS"))
                            break;

                        int index = dflog.FindMessageOffset("GPS", "TimeMS");
                        int index2 = dflog.FindMessageOffset("GPS", "TimeUS");
                        if (index == -1)
                        {
                            if (index2 == -1)
                            {
                                a++;
                                continue;
                            }
                            else
                            {
                                index = index2;
                            }
                        }

                        if (item.items.Length <= index)
                            continue;

                        string time = double.Parse(item.items[index]).ToString();
                        UInt64 tempt;
                        if (UInt64.TryParse(time, out tempt))
                        {
                            if (startdelta == 0)
                                startdelta = tempt;

                            if (index2 != -1)
                            {
                                workingtime = starttime.AddMilliseconds(((tempt) - startdelta) / 1000.0);
                            }
                            else
                            {
                                workingtime = starttime.AddMilliseconds((double) (tempt - startdelta));
                            }

                            TimeSpan span = workingtime - starttime;

                            if (workingtime.Minute != lastdrawn.Minute)
                            {
                                var temp = new TextObj(span.TotalMinutes.ToString("0") + " min", b,
                                    zg1.GraphPane.YAxis.Scale.Max, CoordType.AxisXYScale, AlignH.Left, AlignV.Top);
                                TimeCache.Add(temp);
                                this.BeginInvokeIfRequired(() => zg1.GraphPane.GraphObjList.Add(temp));
                                lastdrawn = workingtime;
                            }
                        }
                    }

                    a++;
                }
                log.Info("End DrawTime");
            });
        }

        class LogRouteInfo
        {
            public int firstpoint = 0;
            public int lastpoint = 0;
            public List<int> samples = new List<int>();
        }

        async Task DrawMap(long startline =0, long endline = long.MaxValue)
        {
            await Task.Run(() =>
            {
                log.Info("Start DrawMap");
                int rtcnt = 0;

                try
                {
                    var mapoverlay = new GMapOverlay("overlay");
                    if (gpscache.Length == 0)
                        gpscache = logdata.GetEnumeratorType(new string[] {"GPS", "POS", "GPS2", "GPSB", "CMD"})
                            .ToArray();

                    DateTime starttime = DateTime.MinValue;
                    DateTime workingtime = starttime;

                    DateTime lastdrawn = DateTime.MinValue;

                    List<PointLatLng> routelist = new List<PointLatLng>();
                    List<int> samplelist = new List<int>();

                    List<PointLatLng> routelistgps2 = new List<PointLatLng>();
                    List<int> samplelistgps2 = new List<int>();

                    List<PointLatLng> routelistgpsb = new List<PointLatLng>();
                    List<int> samplelistgpsb = new List<int>();

                    List<PointLatLng> routelistpos = new List<PointLatLng>();
                    List<int> samplelistpos = new List<int>();

                    List<PointLatLng> routelistcmd = new List<PointLatLng>();
                    List<int> samplelistcmd = new List<int>();

                    int i = 0;
                    int firstpoint = 0;
                    int firstpointpos = 0;
                    int firstpointgps2 = 0;
                    int firstpointgpsb = 0;
                    int firstpointcmd = 0;

                    foreach (var item in gpscache)
                    {
                        i = item.lineno;

                        if (i < startline || i > endline)
                            continue;

                        if (item.msgtype == "GPS")
                        {
                            var ans = getPointLatLng(item);

                            if (ans != null)
                            {
                                routelist.Add(ans);
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
                                    route_part.IsHitTestVisible = false;
                                    mapoverlay.Routes.Add(route_part);
                                    rtcnt++;

                                    //clear the list and set the last point as first point for the next route
                                    routelist.Clear();
                                    samplelist.Clear();
                                    firstpoint = i;
                                    samplelist.Add(firstpoint);
                                    routelist.Add(ans);
                                }
                            }
                        }
                        else if (item.msgtype == "GPS2")
                        {
                            var ans = getPointLatLng(item);

                            if (ans != null)
                            {
                                routelistgps2.Add(ans);
                                samplelistgps2.Add(i);

                                if (routelistgps2.Count > 1000)
                                {
                                    //split the route in several small parts (due to memory errors)
                                    GMapRoute route_part = new GMapRoute(routelistgps2, "routegps2_" + rtcnt);
                                    route_part.Stroke = new Pen(Color.FromArgb(127, Color.Green), 2);

                                    LogRouteInfo lri = new LogRouteInfo();
                                    lri.firstpoint = firstpointgps2;
                                    lri.lastpoint = i;
                                    lri.samples.AddRange(samplelistgps2);

                                    route_part.Tag = lri;
                                    route_part.IsHitTestVisible = false;
                                    mapoverlay.Routes.Add(route_part);
                                    rtcnt++;

                                    //clear the list and set the last point as first point for the next route
                                    routelistgps2.Clear();
                                    samplelistgps2.Clear();
                                    firstpointgps2 = i;
                                    samplelistgps2.Add(firstpointgps2);
                                    routelistgps2.Add(ans);
                                }
                            }
                        }
                        else if (item.msgtype == "GPSB")
                        {
                            var ans = getPointLatLng(item);

                            if (ans != null)
                            {
                                routelistgpsb.Add(ans);
                                samplelistgpsb.Add(i);

                                if (routelistgpsb.Count > 1000)
                                {
                                    //split the route in several small parts (due to memory errors)
                                    GMapRoute route_part = new GMapRoute(routelistgpsb, "routegpsb_" + rtcnt);
                                    route_part.Stroke = new Pen(Color.FromArgb(127, Color.Yellow), 2);

                                    LogRouteInfo lri = new LogRouteInfo();
                                    lri.firstpoint = firstpointgpsb;
                                    lri.lastpoint = i;
                                    lri.samples.AddRange(samplelistgpsb);

                                    route_part.Tag = lri;
                                    route_part.IsHitTestVisible = false;
                                    mapoverlay.Routes.Add(route_part);
                                    rtcnt++;

                                    //clear the list and set the last point as first point for the next route
                                    routelistgpsb.Clear();
                                    samplelistgpsb.Clear();
                                    firstpointgpsb = i;
                                    samplelistgpsb.Add(firstpointgpsb);
                                    routelistgpsb.Add(ans);
                                }
                            }
                        }
                        else if (item.msgtype == "POS")
                        {
                            var ans = getPointLatLng(item);

                            if (ans != null)
                            {
                                routelistpos.Add(ans);
                                samplelistpos.Add(i);

                                if (routelistpos.Count > 1000)
                                {
                                    //split the route in several small parts (due to memory errors)
                                    GMapRoute route_part = new GMapRoute(routelistpos, "routepos_" + rtcnt);
                                    route_part.Stroke = new Pen(Color.FromArgb(127, Color.Red), 2);

                                    LogRouteInfo lri = new LogRouteInfo();
                                    lri.firstpoint = firstpointpos;
                                    lri.lastpoint = i;
                                    lri.samples.AddRange(samplelistpos);

                                    route_part.Tag = lri;
                                    route_part.IsHitTestVisible = false;
                                    mapoverlay.Routes.Add(route_part);
                                    rtcnt++;

                                    //clear the list and set the last point as first point for the next route
                                    routelistpos.Clear();
                                    samplelistpos.Clear();
                                    firstpointpos = i;
                                    samplelistpos.Add(firstpointpos);
                                    routelistpos.Add(ans);
                                }
                            }
                        }
                        else if (item.msgtype == "CMD")
                        {
                            var ans = getPointLatLng(item);

                            if (ans != null && ans.Lat != 0 && ans.Lng != 0)
                            {
                                routelistcmd.Add(ans);
                                samplelistcmd.Add(i);

                                mapoverlay.Markers.Add(new GMarkerGoogle(ans, GMarkerGoogleType.lightblue_dot));

                                //FMT, 146, 45, CMD, QHHHfffffff, TimeUS,CTot,CNum,CId,Prm1,Prm2,Prm3,Prm4,Lat,Lng,Alt
                                //CMD, 43368479, 19, 18, 85, 0, 0, 0, 0, -27.27409, 151.2901, 0

                                if (item["CTot"] != null && item["CNum"] != null &&
                                    (int.Parse(item["CTot"]) - 1) == int.Parse(item["CNum"]))
                                {
                                    //split the route in several small parts (due to memory errors)
                                    GMapRoute route_part = new GMapRoute(routelistcmd, "routecmd_" + rtcnt);
                                    route_part.Stroke = new Pen(Color.FromArgb(127, Color.Indigo), 2);

                                    LogRouteInfo lri = new LogRouteInfo();
                                    lri.firstpoint = firstpointpos;
                                    lri.lastpoint = i;
                                    lri.samples.AddRange(samplelistcmd);

                                    route_part.Tag = lri;
                                    route_part.IsHitTestVisible = false;
                                    mapoverlay.Routes.Add(route_part);

                                    rtcnt++;

                                    //clear the list and set the last point as first point for the next route
                                    routelistcmd.Clear();
                                    samplelistcmd.Clear();
                                    firstpointcmd = i;
                                    samplelistcmd.Add(firstpointcmd);
                                    routelistcmd.Add(ans);
                                }
                            }
                        }

                        i++;
                    }

                    log.Info("done reading map points");

                    // add last part of each
                    // gps1
                    GMapRoute route = new GMapRoute(routelist, "route_" + rtcnt);
                    route.Stroke = new Pen(Color.FromArgb(127, Color.Blue), 2);
                    route.IsHitTestVisible = false;

                    LogRouteInfo lri2 = new LogRouteInfo();
                    lri2.firstpoint = firstpoint;
                    lri2.lastpoint = i;
                    lri2.samples.AddRange(samplelist);
                    route.Tag = lri2;
                    route.IsHitTestVisible = false;
                    mapoverlay.Routes.Add(route);

                    // gps2
                    GMapRoute route2 = new GMapRoute(routelistgps2, "routegps2_" + rtcnt);
                    route2.Stroke = new Pen(Color.FromArgb(127, Color.Green), 2);
                    route2.IsHitTestVisible = false;

                    LogRouteInfo lri3 = new LogRouteInfo();
                    lri3.firstpoint = firstpointgps2;
                    lri3.lastpoint = i;
                    lri3.samples.AddRange(samplelistgps2);
                    route2.Tag = lri3;
                    route2.IsHitTestVisible = false;
                    mapoverlay.Routes.Add(route2);

                    // gpsb
                    GMapRoute routeb = new GMapRoute(routelistgpsb, "routegpsb_" + rtcnt);
                    routeb.Stroke = new Pen(Color.FromArgb(127, Color.Yellow), 2);
                    routeb.IsHitTestVisible = false;

                    LogRouteInfo lrib = new LogRouteInfo();
                    lrib.firstpoint = firstpointgpsb;
                    lrib.lastpoint = i;
                    lrib.samples.AddRange(samplelistgpsb);
                    routeb.Tag = lrib;
                    routeb.IsHitTestVisible = false;
                    mapoverlay.Routes.Add(routeb);

                    // pos
                    GMapRoute route3 = new GMapRoute(routelistpos, "routepos_" + rtcnt);
                    route3.Stroke = new Pen(Color.FromArgb(127, Color.Red), 2);
                    route3.IsHitTestVisible = false;

                    LogRouteInfo lri4 = new LogRouteInfo();
                    lri4.firstpoint = firstpointpos;
                    lri4.lastpoint = i;
                    lri4.samples.AddRange(samplelistpos);
                    route3.Tag = lri4;
                    route3.IsHitTestVisible = false;
                    mapoverlay.Routes.Add(route3);

                    // cmd
                    GMapRoute route4 = new GMapRoute(routelistcmd, "routecmd_" + rtcnt);
                    route4.Stroke = new Pen(Color.FromArgb(127, Color.Indigo), 2);
                    route4.IsHitTestVisible = false;

                    LogRouteInfo lri5 = new LogRouteInfo();
                    lri5.firstpoint = firstpointcmd;
                    lri5.lastpoint = i;
                    lri5.samples.AddRange(samplelistcmd);
                    route4.Tag = lri5;
                    route4.IsHitTestVisible = false;
                    mapoverlay.Routes.Add(route4);


                    rtcnt++;
                    this.BeginInvokeIfRequired(() =>
                    {
                        if (rtcnt > 0)
                            myGMAP1.RoutesEnabled = true;
                        myGMAP1.Overlays.Remove(myGMAP1.Overlays.First(a => a.Id == mapoverlay.Id));
                        myGMAP1.Overlays.Add(mapoverlay);
                        myGMAP1.ZoomAndCenterRoutes(mapoverlay.Id);
                        zg1.Invalidate();
                    });
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
                log.Info("End DrawMap");
            });
        }

        PointLatLngAlt getPointLatLng(DFLog.DFItem item)
        {
            if (item.msgtype == "GPS")
            {
                if (!dflog.logformat.ContainsKey("GPS"))
                    return null;

                int index = dflog.FindMessageOffset("GPS", "Lat");
                if (index == -1)
                {
                    return null;
                }

                int index2 = dflog.FindMessageOffset("GPS", "Lng");
                if (index2 == -1)
                {
                    return null;
                }

                int index3 = dflog.FindMessageOffset("GPS", "Status");
                if (index3 == -1)
                {
                    return null;
                }

                try
                {
                    if (double.Parse(item.items[index3].ToString(), System.Globalization.CultureInfo.InvariantCulture) <
                        3)
                    {
                        return null;
                    }

                    string lat = item.items[index].ToString();
                    string lng = item.items[index2].ToString();

                    PointLatLngAlt pnt = new PointLatLngAlt() { };
                    pnt.Lat = double.Parse(lat, System.Globalization.CultureInfo.InvariantCulture);
                    pnt.Lng = double.Parse(lng, System.Globalization.CultureInfo.InvariantCulture);
                    pnt.Tag = item.lineno.ToString();
                    return pnt;
                }
                catch
                {
                }
            }
            else if (item.msgtype == "GPS2")
            {
                if (!dflog.logformat.ContainsKey("GPS2"))
                    return null;

                int index = dflog.FindMessageOffset("GPS2", "Lat");
                if (index == -1)
                {
                    return null;
                }

                int index2 = dflog.FindMessageOffset("GPS2", "Lng");
                if (index2 == -1)
                {
                    return null;
                }

                int index3 = dflog.FindMessageOffset("GPS2", "Status");
                if (index3 == -1)
                {
                    return null;
                }

                try
                {
                    if (double.Parse(item.items[index3].ToString(), System.Globalization.CultureInfo.InvariantCulture) <
                        3)
                    {
                        return null;
                    }

                    string lat = item.items[index].ToString();
                    string lng = item.items[index2].ToString();

                    PointLatLngAlt pnt = new PointLatLngAlt() { };
                    pnt.Lat = double.Parse(lat, System.Globalization.CultureInfo.InvariantCulture);
                    pnt.Lng = double.Parse(lng, System.Globalization.CultureInfo.InvariantCulture);
                    pnt.Tag = item.lineno.ToString();
                    return pnt;
                }
                catch
                {
                }
            }
            else if (item.msgtype == "GPSB")
            {
                if (!dflog.logformat.ContainsKey("GPSB"))
                    return null;

                int index = dflog.FindMessageOffset("GPSB", "Lat");
                if (index == -1)
                {
                    return null;
                }

                int index2 = dflog.FindMessageOffset("GPSB", "Lng");
                if (index2 == -1)
                {
                    return null;
                }

                int index3 = dflog.FindMessageOffset("GPSB", "Status");
                if (index3 == -1)
                {
                    return null;
                }

                try
                {
                    if (double.Parse(item.items[index3].ToString(), System.Globalization.CultureInfo.InvariantCulture) <
                        3)
                    {
                        return null;
                    }

                    string lat = item.items[index].ToString();
                    string lng = item.items[index2].ToString();

                    PointLatLngAlt pnt = new PointLatLngAlt() { };
                    pnt.Lat = double.Parse(lat, System.Globalization.CultureInfo.InvariantCulture);
                    pnt.Lng = double.Parse(lng, System.Globalization.CultureInfo.InvariantCulture);
                    pnt.Tag = item.lineno.ToString();
                    return pnt;
                }
                catch
                {
                }
            }
            else if (item.msgtype == "POS")
            {
                if (!dflog.logformat.ContainsKey("POS"))
                    return null;

                int index = dflog.FindMessageOffset("POS", "Lat");
                if (index == -1)
                {
                    return null;
                }

                int index2 = dflog.FindMessageOffset("POS", "Lng");
                if (index2 == -1)
                {
                    return null;
                }

                try
                {
                    string lat = item.items[index].ToString();
                    string lng = item.items[index2].ToString();

                    PointLatLngAlt pnt = new PointLatLngAlt() { };
                    pnt.Lat = double.Parse(lat, System.Globalization.CultureInfo.InvariantCulture);
                    pnt.Lng = double.Parse(lng, System.Globalization.CultureInfo.InvariantCulture);
                    pnt.Tag = item.lineno.ToString();
                    if (Math.Abs(pnt.Lat) > 90 || Math.Abs(pnt.Lng) > 180)
                        return null;

                    return pnt;
                }
                catch
                {
                }
            }
            else if (item.msgtype == "CMD")
            {
                //FMT, 146, 45, CMD, QHHHfffffff, TimeUS,CTot,CNum,CId,Prm1,Prm2,Prm3,Prm4,Lat,Lng,Alt
                if (!dflog.logformat.ContainsKey("CMD"))
                    return null;

                int index = dflog.FindMessageOffset("CMD", "Lat");
                if (index == -1)
                {
                    return null;
                }

                int index2 = dflog.FindMessageOffset("CMD", "Lng");
                if (index2 == -1)
                {
                    return null;
                }

                try
                {
                    string lat = item.items[index].ToString();
                    string lng = item.items[index2].ToString();

                    PointLatLngAlt pnt = new PointLatLngAlt() { };
                    pnt.Lat = double.Parse(lat, System.Globalization.CultureInfo.InvariantCulture);
                    pnt.Lng = double.Parse(lng, System.Globalization.CultureInfo.InvariantCulture);
                    pnt.Tag = item.lineno.ToString();
                    if (Math.Abs(pnt.Lat) > 90 || Math.Abs(pnt.Lng) > 180)
                        return null;

                    return pnt;
                }
                catch
                {
                }
            }

            return null;
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
            // reset logname
            logfilename = "";
            // reload
            LogBrowse_Load(sender, e);
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Point mp = Control.MousePosition;

            List<string> options = new List<string>();

            int b = 0;

            foreach (string item2 in logdata.SeenMessageTypes)
            {
                string celldata = item2.Trim();
                if (!options.Contains(celldata))
                {
                    options.Add(celldata);
                }
            }

            options.Sort();

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
                b = 0;

                foreach (var item2 in logdata)
                {
                    b++;
                    var item = dflog.GetDFItemFromLine(item2, b);

                    if (item.msgtype == opt.SelectedItem)
                    {
                        logdatafilter.Add(a, item);
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
        }

        private void BUT_Graphit_R_Click(object sender, EventArgs e)
        {
            graphit_clickprocess(false);
        }

        private async void zg1_ZoomEvent(ZedGraphControl sender, ZoomState oldState, ZoomState newState)
        {
            try
            {
                sender.GraphPane.GraphObjList.Clear();

                Task a = null, b = null, c = null, d = null, e = null, f = null;

                if (chk_mode.Checked)
                    a=DrawModes();
                if (chk_errors.Checked)
                    b=DrawErrors();
                if (!chk_time.Checked)
                    c=DrawTime();

                if (chk_events.Checked)
                    f = DrawEV();

                if (chk_msg.Checked)
                    d=DrawMSG();

                if (!chk_time.Checked && CHK_map.Checked)
                {
                    if (sender.GraphPane.CurveList.Count == 0)
                    {
                        e = DrawMap();
                    }
                    else
                    {
                        e = DrawMap((long) sender.GraphPane.XAxis.Scale.Min, (long) sender.GraphPane.XAxis.Scale.Max);
                    }
                }

                if (chk_time.Checked && CHK_map.Checked)
                {
                    if (sender.GraphPane.CurveList.Count == 0)
                    {
                        e = DrawMap();
                    }
                    else
                    {
                        e = DrawMap(
                            dflog.GetLineNoFromTime(logdata, new XDate(sender.GraphPane.XAxis.Scale.Min).DateTime),
                            dflog.GetLineNoFromTime(logdata, new XDate(sender.GraphPane.XAxis.Scale.Max).DateTime));
                    }
                }

                if(a!= null)
                    await a;
                if (b != null)
                    await b;
                if (c != null)
                    await c;
                if (d != null)
                    await d;
                if (e != null)
                    await e;
                if (f != null)
                    await f;

                sender.Invalidate();
            }
            catch
            {
            }
        }

        private void CHK_map_CheckedChanged(object sender, EventArgs e)
        {
            splitContainerZgMap.Panel2Collapsed = !splitContainerZgMap.Panel2Collapsed;

            if (CHK_map.Checked)
            {
                splitContainerZgMap.SplitterDistance = splitContainerZgMap.Width / 2;

                log.Info("Get map");

                myGMAP1.MapProvider = GCSViews.FlightData.mymap.MapProvider;

                zg1_ZoomEvent(zg1, null, null);

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

            opt.Dispose();

            zg1.Invalidate();
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            // apply a slope and offset to a selected child
            if (treeView1.SelectedNode == null || treeView1.SelectedNode.Parent == null)
            {
                // only apply scalers to children
                return;
            }

            string dataModifer_str = "";
            string nodeName = DataModifer.GetNodeName(treeView1.SelectedNode.Parent.Text, treeView1.SelectedNode.Text);

            if (dataModifierHash.ContainsKey(nodeName))
            {
                DataModifer initialDataModifier = (DataModifer) dataModifierHash[nodeName];
                if (initialDataModifier.IsValid())
                    dataModifer_str = initialDataModifier.commandString;
            }

            string title = "Apply scaler and offset to " + nodeName;
            string instructions =
                "Enter modifer then value, they are applied in the order you provide. Modifiers are x + - /\n";
            instructions += "Example: Convert cm to to m with an offset of 50: '/100 +50' or 'x0.01 +50' or '*0.01,+50'";
            InputBox.Show(title, instructions, ref dataModifer_str);

            // if it's already there, remove it.
            dataModifierHash.Remove(nodeName);

            DataModifer dataModifer = new DataModifer(dataModifer_str);
            if (dataModifer.IsValid())
            {
                dataModifierHash.Add(nodeName, dataModifer);
            }
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
                        if (item.Label.Text.StartsWith(e.Node.Parent.Text + "." + e.Node.Text + " ") ||
                            item.Label.Text.StartsWith(e.Node.Parent.Text + "." + e.Node.Text + " R ") ||
                            item.Label.Text.Equals(e.Node.Parent.Text + "." + e.Node.Text) ||
                            item.Label.Text.Equals(e.Node.Parent.Text + "." + e.Node.Text + " R"))
                        {
                            removeitems.Add(item);
                            //break;
                        }
                    }

                    foreach (var item in removeitems)
                        zg1.GraphPane.CurveList.Remove(item);
                }

                zg1.AxisChange();
                zg1.Invalidate();
            }
        }

        private void CMB_preselect_SelectedIndexChanged(object sender, EventArgs e)
        {
            displaylist selectlist = (displaylist) CMB_preselect.SelectedValue;

            if (selectlist == null || selectlist.items == null)
                return;

            BUT_cleargraph_Click(null, null);

            foreach (var item in selectlist.items)
            {
                try
                {
                    if (!string.IsNullOrEmpty(item.expression))
                    {
                        GraphItem(item.expression, "", item.left, false, true);
                    }
                    else
                    {
                        GraphItem(item.type, item.field, item.left, false);
                    }
                }
                catch
                {
                }
            }

            this.Invalidate();
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


            TextRenderer.DrawText(e.Graphics, e.Node.Text, treeView1.Font, e.Node.Bounds, treeView1.ForeColor,
                treeView1.BackColor);

            if ((e.State & TreeNodeStates.Focused) == TreeNodeStates.Focused)
            {
                ControlPaint.DrawFocusRectangle(e.Graphics, e.Node.Bounds, treeView1.ForeColor, treeView1.BackColor);
            }
        }

        private void LogBrowse_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (logdata != null)
                logdata.Clear();
            logdata = null;
            dataGridView1.DataSource = null;
            mapoverlay = null;
            GC.Collect();
        }

        private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            try
            {
                if (e.RowIndex >= logdata.Count)
                    return;
 
                var item2 = logdata[e.RowIndex];

                var item = dflog.GetDFItemFromLine(item2, e.RowIndex);

                if (logdatafilter.Count > 0)
                {
                    if (e.RowIndex > logdatafilter.Count)
                        return;

                    item = (DFLog.DFItem) logdatafilter[e.RowIndex];
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
            catch
            {
            }
        }


        private void zg1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            PointF ptClick = new PointF(e.X, e.Y);
            double x, y;
            zg1.GraphPane.ReverseTransform(ptClick, out x, out y);

            try
            {
                if (chk_time.Checked)
                {
                    x = dflog.GetLineNoFromTime(logdata, XDate.XLDateToDateTime(x));
                }
                //TODO - time fails
                GoToSample((int) x, true, false, true);
            } catch { }
        }

        private void scrollGrid(DataGridView dataGridView, int index)
        {
            int halfWay = (dataGridView.DisplayedRowCount(false)/2);

            if ((index < 0) && (dataGridView.SelectedRows.Count > 0))
            {
                index = dataGridView.SelectedRows[0].Index;
            }

            if (dataGridView.FirstDisplayedScrollingRowIndex + halfWay > index ||
                (dataGridView.FirstDisplayedScrollingRowIndex + dataGridView.DisplayedRowCount(false) - halfWay) <=
                index)
            {
                int targetRow = index;

                targetRow = Math.Max(targetRow - halfWay, 0);
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
        
        bool GetGPSFromRow(int lineNumber, out PointLatLng pt)
        {
            bool ret = false;
            int index_lat = -1;
            int index_lng = -1;
            pt = new PointLatLng();

            if (lineNumber >= logdata.Count)
                return ret;

            if (!dflog.logformat.ContainsKey("GPS") && !dflog.logformat.ContainsKey("POS"))
                return ret;

            const int maxSearch = 1000;
            int offset = maxSearch;
            int roffset = -maxSearch;
            bool found = false;

            for (int i = 0; i < maxSearch && lineNumber + i < logdata.Count && !found; i++)
            {
                string searching = logdata[lineNumber + i];
                if (searching.StartsWith("GPS") || searching.StartsWith("POS"))
                {
                    offset = i;
                    found = true;
                }
            }

            found = false;
            for (int i = 0; i < maxSearch && lineNumber - i >= 0 && !found; i++)
            {
                string searching = logdata[lineNumber - i];
                if (searching.StartsWith("GPS") || searching.StartsWith("POS"))
                {
                    roffset = i;
                    found = true;
                }
            }

            if (offset < roffset)
            {
                lineNumber += offset;
                ret = true;
            }
            else if (roffset < maxSearch)
            {
                lineNumber -= roffset;
                ret = true;
            }

            if (ret == true)
            {
                if (lineNumber >= logdata.Count || lineNumber < 0)
                    return false;

                string gpsline = logdata[lineNumber];
                var item = dflog.GetDFItemFromLine(gpsline, lineNumber);
                if (gpsline.StartsWith("GPS"))
                {
                    index_lat = dflog.FindMessageOffset("GPS", "Lat");
                    index_lng = dflog.FindMessageOffset("GPS", "Lng");
                    int index_status = dflog.FindMessageOffset("GPS", "Status");

                    if (index_status < 0)
                        ret = false;
                    if (index_status > 0)
                    {
                        int status = int.Parse(item.items[index_status],
                            System.Globalization.CultureInfo.InvariantCulture);
                        if (status < 3)
                            ret = false;
                    }
                }
                else if (gpsline.StartsWith("POS"))
                {
                    index_lat = dflog.FindMessageOffset("POS", "Lat");
                    index_lng = dflog.FindMessageOffset("POS", "Lng");
                }

                if (index_lat < 0 || index_lng < 0)
                    ret = false;

                if (ret)
                {
                    string lat = item.items[index_lat];
                    string lng = item.items[index_lng];

                    pt.Lat = double.Parse(lat, System.Globalization.CultureInfo.InvariantCulture);
                    pt.Lng = double.Parse(lng, System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            return ret;
        }


        private void myGMAP1_OnRouteClick(GMapRoute item, object ei)
        {
            var e = ei as MouseEventArgs;
            if ((item.Name != null) && (item.Name.StartsWith("route_")))
            {
                LogRouteInfo lri = item.Tag as LogRouteInfo;
                if (lri != null)
                {
                    //cerco il punto più vicino
                    MissionPlanner.Utilities.PointLatLngAlt pt2 =
                        new MissionPlanner.Utilities.PointLatLngAlt(myGMAP1.FromLocalToLatLng(e.X, e.Y));
                    double dBest = double.MaxValue;
                    int nBest = 0;
                    for (int i = 0; i < item.LocalPoints.Count; i++)
                    {
                        PointLatLng pt = item.Points[i];
                        double d =
                            Math.Sqrt((pt.Lat - pt2.Lat)*(pt.Lat - pt2.Lat) + (pt.Lng - pt2.Lng)*(pt.Lng - pt2.Lng));
                        if (d < dBest)
                        {
                            dBest = d;
                            nBest = i;
                        }
                    }
                    double perc = (double) nBest/(double) item.LocalPoints.Count;
                    int SampleID = (int) (lri.firstpoint + (lri.lastpoint - lri.firstpoint)*perc);

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
            markeroverlay.Markers.Clear();

            PointLatLng pt1;
            if (GetGPSFromRow(SampleID, out pt1))
            {
                MissionPlanner.Utilities.PointLatLngAlt pt3 = new MissionPlanner.Utilities.PointLatLngAlt(pt1);
                GMapMarker pos3 = new GMarkerGoogle(pt3, GMarkerGoogleType.pink_dot);
                markeroverlay.Markers.Add(pos3);
                if (movemap)
                {
                    myGMAP1.Position = pt1;
                }
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
                zg1.GraphPane.XAxis.Scale.Min = SampleID - delta/2;
                zg1.GraphPane.XAxis.Scale.Max = SampleID + delta/2;
                zg1.AxisChange();
            }
            zg1.Invalidate();


            if (movegrid)
            {
                try
                {
                    scrollGrid(dataGridView1, SampleID);
                    dataGridView1.CurrentCell = dataGridView1.Rows[SampleID].Cells[1];

                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[(int) SampleID].Selected = true;
                    dataGridView1.Rows[(int) SampleID].Cells[1].Selected = true;
                }
                catch
                {
                }
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

        private void chk_time_CheckedChanged(object sender, EventArgs e)
        {
            ModeCache.Clear();
            EVCache.Clear();
            ErrorCache.Clear();
            TimeCache.Clear();
            MSGCache.Clear();

            BUT_cleargraph_Click(null, null);

            if (chk_time.Checked)
            {
                zg1.GraphPane.XAxis.Type = AxisType.Date;
                zg1.GraphPane.XAxis.Scale.Format = "HH:mm:ss.fff";
                zg1.GraphPane.XAxis.Title.Text = "Time (sec)";
                zg1.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Minute;
                zg1.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Second;
                zg1.GraphPane.YAxis.Title.Text = "Output";
            }
            else
            {
                // Set the titles and axis labels
                zg1.GraphPane.XAxis.Type = AxisType.Linear;
                zg1.GraphPane.XAxis.Scale.Format = "f0";
                zg1.GraphPane.Title.Text = "Value Graph";
                zg1.GraphPane.XAxis.Title.Text = "Line Number";
                zg1.GraphPane.YAxis.Title.Text = "Output";
            }

            zg1.AxisChange();
            zg1.Invalidate();
        }

        double prevMouseX = 0;
        double prevMouseY = 0;
        private int colcount;

        private bool zg1_MouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            // debounce for mousemove and tooltip label

            if (e.X == prevMouseX && e.Y == prevMouseY)
                return true;

            prevMouseX = e.X;
            prevMouseY = e.Y;

            // not handled
            return false;
        }

        private void exportVisibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.FileName = "output.csv";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(sfd.OpenFile()))
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        StringBuilder sb = new StringBuilder();
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            sb.Append(cell.FormattedValue);
                            sb.Append(',');
                        }
                        sw.WriteLine(sb.ToString());
                    }
                }
            }
        }

        private void chk_mode_CheckedChanged(object sender, EventArgs e)
        {
            zg1_ZoomEvent(zg1, null, null);
        }

        private void chk_errors_CheckedChanged(object sender, EventArgs e)
        {
            zg1_ZoomEvent(zg1, null, null);
        }

        private void chk_msg_CheckedChanged(object sender, EventArgs e)
        {
            zg1_ZoomEvent(zg1, null, null);
        }

        private void splitContainer2_Resize(object sender, EventArgs e)
        {
            splitContainerZgMap.Visible = false;
            splitContainerZgMap.Visible = true;
            splitContainerZgMap.Panel1.Invalidate();
            splitContainerZgMap.Panel2.Invalidate();
        }

        private void splitContainer1_Resize(object sender, EventArgs e)
        {
            splitContainerZgGrid.Visible = false;
            splitContainerZgGrid.Visible = true;
            splitContainerZgGrid.Panel1.Invalidate();
            splitContainerZgGrid.Panel2.Invalidate();
        }

        private void chk_datagrid_CheckedChanged(object sender1, EventArgs e)
        {
            splitContainerButGrid.Panel2Collapsed = !splitContainerButGrid.Panel2Collapsed;
            

            if (!splitContainerButGrid.Panel2Collapsed)
            {
                splitContainerZgGrid.SplitterDistance = splitContainerZgGrid.Height / 2;
                try
                {
                    log.Info("set dgv datasourse " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));

                    if (MainV2.MONO)
                    {
                        int rowstartoffset = 0;

                        dataGridView1.ScrollBars = ScrollBars.Horizontal;

                        var VBar = new VScrollBar();
                        VBar.Visible = true;
                        VBar.Top = 0;
                        VBar.Height = dataGridView1.Height;
                        VBar.Dock = DockStyle.Right;
                        VBar.Maximum = logdata.Count;

                        dataGridView1.Controls.Add(VBar);

                        dataGridView1.PerformLayout();

                        dataGridView1.RowPrePaint += (sender, args) =>
                        {
                            VBar.Maximum = logdata.Count;
                            populateRowData(rowstartoffset, args.RowIndex, args.RowIndex);
                        };

                        dataGridView1.ColumnCount = colcount;

                        int a = 0;
                        while (a++ < 1000)
                            dataGridView1.Rows.Add();

                        // populate first row
                        populateRowData(0, 0, 0);

                        VBar.ValueChanged += (sender, args) =>
                        {
                            rowstartoffset = VBar.Value;
                            dataGridView1.Invalidate();
                        };
                    }
                    else
                    {
                        dataGridView1.VirtualMode = true;
                        dataGridView1.ColumnCount = colcount;
                        dataGridView1.RowCount = logdata.Count;
                        log.Info("datagrid size set " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));
                    }

                    log.Info("datasource set " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));
                }
                catch (Exception ex)
                {
                    CustomMessageBox.Show("Failed to read File: " + ex.ToString());
                    return;
                }

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                log.Info("Done timetable " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));
            }
            else
            {
                splitContainerZgGrid.SplitterDistance = splitContainerZgGrid.Height - splitContainerButGrid.Panel1.MinimumSize.Height;
            }
        }

        bool mousedown = false;
        private PointLatLng MouseDownStart;

        private void myGMAP1_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            MouseDownStart = myGMAP1.FromLocalToLatLng(e.X, e.Y);
        }

        private void myGMAP1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousedown)
            {
                PointLatLng point = myGMAP1.FromLocalToLatLng(e.X, e.Y);

                double latdif = MouseDownStart.Lat - point.Lat;
                double lngdif = MouseDownStart.Lng - point.Lng;

                try
                {
                    myGMAP1.Position = new PointLatLng(myGMAP1.Position.Lat + latdif, myGMAP1.Position.Lng + lngdif);
                }
                catch
                {
                }
            }
        }

        private void myGMAP1_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
        }

        private void LogBrowse_Resize(object sender, EventArgs e)
        {
            if(chk_datagrid.Checked)
                splitContainerZgGrid.SplitterDistance = this.Height / 2;
            if (!chk_datagrid.Checked)
                splitContainerZgGrid.SplitterDistance = this.Height - splitContainerButGrid.Panel2.Height;
        }

        public IEnumerable<(double[],double[],double[])> fft(int bins = 10, int startfreq = 5)
        {
            Utilities.FFT2 fft = new FFT2();

            var file = logdata;

            int N = 1 << bins;

            // 3 imus * 2 sets of measurements(gyr/acc)
            FFT2.datastate[] alldata = new FFT2.datastate[3 * 2];
            for (int a = 0; a < alldata.Length; a++)
                alldata[a] = new FFT2.datastate();

            // state cache
            int Ns = 0;
            int type = 0;
            int instance = 0;
            int sensorno = 0;

            foreach (var item in file.GetEnumeratorType(new string[] { "ISBH", "ISBD" }))
            {
                if (item.msgtype == null)
                {
                    continue;
                }

                if (item.msgtype.StartsWith("ISBH"))
                {
                    Ns = int.Parse(item.items[file.dflog.FindMessageOffset(item.msgtype, "N")],
                        CultureInfo.InvariantCulture);
                    type = int.Parse(item.items[file.dflog.FindMessageOffset(item.msgtype, "type")],
                        CultureInfo.InvariantCulture);
                    instance = int.Parse(item.items[file.dflog.FindMessageOffset(item.msgtype, "instance")],
                        CultureInfo.InvariantCulture);

                    sensorno = type * 3 + instance;

                    alldata[sensorno].sample_rate = double.Parse(item.items[file.dflog.FindMessageOffset(item.msgtype, "smp_rate")],
                    CultureInfo.InvariantCulture);

                    if (type == 0)
                        alldata[sensorno].type = "ACC" + instance.ToString();
                    if (type == 1)
                        alldata[sensorno].type = "GYR" + instance.ToString();

                }
                else if (item.msgtype.StartsWith("ISBD"))
                {
                    var Nsdata = int.Parse(item.items[file.dflog.FindMessageOffset(item.msgtype, "N")],
                        CultureInfo.InvariantCulture);

                    if (Ns != Nsdata)
                        continue;

                    int offsetX = file.dflog.FindMessageOffset(item.msgtype, "x");
                    int offsetY = file.dflog.FindMessageOffset(item.msgtype, "y");
                    int offsetZ = file.dflog.FindMessageOffset(item.msgtype, "z");
                    int offsetTime = file.dflog.FindMessageOffset(item.msgtype, "TimeUS");

                    double time = double.Parse(item.items[offsetTime],
                                      CultureInfo.InvariantCulture) / 1000.0;

                    if (time < alldata[sensorno].lasttime)
                        continue;

                    if (time != alldata[sensorno].lasttime)
                        alldata[sensorno].timedelta = alldata[sensorno].timedelta * 0.99 +
                                                      (time - alldata[sensorno].lasttime) * 0.01;

                    alldata[sensorno].lasttime = time;

                    item.items[offsetX].Split(new[] { ' ', '[', ']' }, StringSplitOptions.RemoveEmptyEntries).ForEach(aa => {
                        alldata[sensorno].datax.Add(double.Parse(aa,
CultureInfo.InvariantCulture));
                    });
                    item.items[offsetY].Split(new[] { ' ', '[', ']' }, StringSplitOptions.RemoveEmptyEntries).ForEach(aa => {
                        alldata[sensorno].datay.Add(double.Parse(aa,
CultureInfo.InvariantCulture));
                    });
                    item.items[offsetZ].Split(new[] { ' ', '[', ']' }, StringSplitOptions.RemoveEmptyEntries).ForEach(aa => {
                        alldata[sensorno].dataz.Add(double.Parse(aa,
CultureInfo.InvariantCulture));
                    });
                }
            }

            foreach (var sensordata in alldata)
            {
                if (sensordata.datax.Count <= N)
                    continue;

                double samplerate = 0;

                samplerate = sensordata.sample_rate; // Math.Round(1000 / sensordata.timedelta, 1);

                double[] freqt =  fft.FreqTable(N, (int) samplerate);

                double[] avgx = new double[N / 2];
                double[] avgy = new double[N / 2];
                double[] avgz = new double[N / 2];

                int totalsamples = sensordata.datax.Count;
                int count = totalsamples / N;
                int done = 0;
                while (count > 1) // skip last part
                {
                    var fftanswerx = fft.rin(sensordata.datax.Skip(N * done).Take(N).ToArray(), (uint) bins);
                    var fftanswery = fft.rin(sensordata.datay.Skip(N * done).Take(N).ToArray(), (uint) bins);
                    var fftanswerz = fft.rin(sensordata.dataz.Skip(N * done).Take(N).ToArray(), (uint) bins);

                    for (int b = 0; b < N / 2; b++)
                    {
                        if (freqt[b] < (double) startfreq)
                            continue;

                        avgx[b] += fftanswerx[b] / (done + count);
                        avgy[b] += fftanswery[b] / (done + count);
                        avgz[b] += fftanswerz[b] / (done + count);
                    }

                    count--;
                    done++;
                }

                yield return (avgx, avgy, avgz);
            }
        }

        private void chk_events_CheckedChanged(object sender, EventArgs e)
        {
            zg1_ZoomEvent(zg1, null, null);
        }
    }
}