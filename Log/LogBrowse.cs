using System;
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

namespace ArdupilotMega.Log
{
    public partial class LogBrowse : Form
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        int m_iColumnCount = 0;
        int rowno = 1;
        DataTable m_dtCSV = new DataTable();

        PointPairList list1 = new PointPairList();
        PointPairList list2 = new PointPairList();
        PointPairList list3 = new PointPairList();
        PointPairList list4 = new PointPairList();
        PointPairList list5 = new PointPairList();

        int graphs = 0;


        public struct Label
        {
            public int Id;
            public string Format;
            public string[] FieldNames;

            public int Length;
            public string Name;
        }


        Dictionary<string, Label> logformat = new Dictionary<string, Label>();

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
            if (keyData == (Keys.Control |Keys.G))
            {
                string lineno = "0";
                Common.InputBox("Line no", "Enter Line Number", ref lineno);

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
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            rowno = 1;

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
                        // extract log
                        List<string> loglines = BinaryLog.ReadLog(openFileDialog1.FileName);
                        // create single string with entire log
                        string log = "";
                        foreach (string line in loglines)
                        {
                            log += line;
                        }
                        // convert log to memory stream;
                        stream = new MemoryStream(ASCIIEncoding.ASCII.GetBytes(log));
                    } else {
                        stream = File.Open(openFileDialog1.FileName, FileMode.Open,FileAccess.Read,FileShare.Read);
                    }

                    this.Text = "Log Browser - " + Path.GetFileName(openFileDialog1.FileName);
                    
                    PopulateDataTableFromUploadedFile(stream);
                    stream.Close();

                    dataGridView1.DataSource = m_dtCSV;

                    dataGridView1.Columns[0].Visible = false;

                }
                catch (Exception ex) { CustomMessageBox.Show("Failed to read File: " + ex.ToString()); }

                foreach (DataGridViewColumn column in dataGridView1.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

                CreateChart(zg1);
            }
            else
            {
                return;
            }
        }

        private void PopulateDataTableFromUploadedFile(System.IO.Stream strm)
        {
            System.IO.StreamReader srdr = new System.IO.StreamReader(strm);
            String strLine = String.Empty;
            Int32 iLineCount = 0;
            logformat.Clear();
            do
            {
                strLine = srdr.ReadLine();
                if (strLine == null)
                {
                    break;
                }
                if (0 == iLineCount++)
                {
                    m_dtCSV = new DataTable("CSVTable");
                }
                this.AddDataRowToTable(strLine, m_dtCSV);

                strLine = strLine.Replace(", ", ",");
                strLine = strLine.Replace(": ", ":");

                string[] items = strLine.Split(',', ':');

                // populate fw type
                if (items[0].Contains("SYSID_SW_TYPE"))
                {
                    switch (items[1].ToString())
                    {
                        case "10":
                            MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduCopter2;
                            break;
                        case "7":
                            MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.Ateryx;
                            break;
                        case "20":
                            MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduRover;
                            break;
                        case "0":
                            MainV2.comPort.MAV.cs.firmware = MainV2.Firmwares.ArduPlane;
                            break;
                    }
                }


                if (items[0].Contains("FMT"))
                {
                    try
                    {
                        string[] names = new string[items.Length - 5];
                        Array.ConstrainedCopy(items, 5, names, 0, names.Length);

                        Label lbl = new Label() { Name = items[3], Id = int.Parse(items[1]), Format = items[4], Length = int.Parse(items[2]), FieldNames = names };

                        logformat[lbl.Name] = lbl;
                    }
                    catch { }
                }


            } while (true);
        }

        private DataRow AddDataRowToTable(String strCSVLine, DataTable dt)
        {
            String[] strVals = strCSVLine.Split(new char[] { ',', ':' });
            Int32 iTotalNumberOfValues = strVals.Length + 1;
            // If number of values in this line are more than the columns
            // currently in table, then we need to add more columns to table.
            if (iTotalNumberOfValues > m_iColumnCount)
            {
                if (!dt.Columns.Contains("rowno"))
                    dt.Columns.Add("rowno", Type.GetType("System.String"));

                // add only what doesnt exist already
                Int32 iDiff = iTotalNumberOfValues - m_iColumnCount;
                for (Int32 i = 0; i < iDiff; i++)
                {
                    String strColumnName = String.Format("col{0}", (m_iColumnCount + i));
                    dt.Columns.Add(strColumnName, Type.GetType("System.String"));
                }
                m_iColumnCount = iTotalNumberOfValues;
            }
            int idx = 0;
            DataRow drow = dt.NewRow();
            drow["rowno"] = rowno;
            foreach (String strVal in strVals)
            {
                String strColumnName = String.Format("col{0}", idx++);
                drow[strColumnName] = strVal.Trim();
            }
            dt.Rows.Add(drow);
            rowno++;
            return drow;
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // number the coloums
                int a = -1;
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
                string option = dataGridView1[1, e.RowIndex].EditedFormattedValue.ToString();

                // new self describing log
                if (logformat.ContainsKey(option))
                {
                    int a = 2;
                    foreach (string name in logformat[option].FieldNames)
                    {
                        dataGridView1.Columns[a].HeaderText = name;
                        a++;
                    }
                    for (; a < dataGridView1.Columns.Count; a++)
                    {
                        dataGridView1.Columns[a].HeaderText = "";
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

        public void CreateChart(ZedGraphControl zgc)
        {
            GraphPane myPane = zgc.GraphPane;

            // Set the titles and axis labels
            myPane.Title.Text = "Value Graph";
            myPane.XAxis.Title.Text = "Line Number";
            myPane.YAxis.Title.Text = "Output";

            LineItem myCurve;

            myCurve = myPane.AddCurve("Value", list1, Color.Red, SymbolType.None);
            myCurve = myPane.AddCurve("Value", list2, Color.Green, SymbolType.None);
            myCurve = myPane.AddCurve("Value", list3, Color.Blue, SymbolType.None);
            myCurve = myPane.AddCurve("Value", list4, Color.Pink, SymbolType.None);
            myCurve = myPane.AddCurve("Value", list5, Color.Yellow, SymbolType.None);

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
            if (dataGridView1.RowCount == 0 || dataGridView1.ColumnCount == 0)
            {
                CustomMessageBox.Show("Please load a valid file");
                return;
            }

            int col = dataGridView1.CurrentCell.ColumnIndex;
            int row = dataGridView1.CurrentCell.RowIndex;
            string type = dataGridView1[1, row].Value.ToString();
            double a = 0; // row counter

            if (col == 0)
            {
                CustomMessageBox.Show("Please pick another column, Highlight the cell you wish to graph");
                return;
            }

            int error = 0;

            foreach (DataGridViewRow datarow in dataGridView1.Rows)
            {
                if (datarow.Cells[1].Value.ToString() == type)
                {
                    try
                    {
                        double value = double.Parse(datarow.Cells[col].Value.ToString(), new System.Globalization.CultureInfo("en-US"));
                        if (graphs == 0)
                        {
                            zg1.GraphPane.CurveList[0].Label.Text = dataGridView1.Columns[col].HeaderText;
                            list1.Add(a, value);
                            leftorrightaxis(sender,zg1.GraphPane.CurveList[0]);
                        }
                        else if (graphs == 1)
                        {
                            zg1.GraphPane.CurveList[1].Label.Text = dataGridView1.Columns[col].HeaderText;
                            list2.Add(a, value);
                            leftorrightaxis(sender,zg1.GraphPane.CurveList[1]);
                        }
                        else if (graphs == 2)
                        {
                            zg1.GraphPane.CurveList[2].Label.Text = dataGridView1.Columns[col].HeaderText;
                            list3.Add(a, value);
                            leftorrightaxis(sender,zg1.GraphPane.CurveList[2]);
                        }
                        else if (graphs == 3)
                        {
                            zg1.GraphPane.CurveList[3].Label.Text = dataGridView1.Columns[col].HeaderText;
                            list4.Add(a, value);
                            leftorrightaxis(sender,zg1.GraphPane.CurveList[3]);
                        }
                        else if (graphs == 4)
                        {
                            zg1.GraphPane.CurveList[4].Label.Text = dataGridView1.Columns[col].HeaderText;
                            list5.Add(a, value);
                            leftorrightaxis(sender,zg1.GraphPane.CurveList[4]);
                        }
                        else
                        {
                            CustomMessageBox.Show("Max of 5");
                            break;
                        }
                    }
                    catch { error++; log.Info("Bad Data : " + type + " " + col + " " + a); if (error >= 500) { CustomMessageBox.Show("There is to much bad data - failing"); break; } }
                }
                a++;
            }

            // Make sure the Y axis is rescaled to accommodate actual data
            try
            {
                zg1.AxisChange();
            }
            catch { }
            // Zoom all
            zg1.ZoomOutAll(zg1.GraphPane);
            // Force a redraw
            zg1.Invalidate();

            graphs++;
        }

        private void leftorrightaxis(object sender, CurveItem myCurve)
        {
            if (sender == BUT_Graphit_R)
            {
                myCurve.Label.Text += " R";
                myCurve.IsY2Axis = true;
                myCurve.YAxisIndex = 0;
                zg1.GraphPane.Y2Axis.IsVisible = true;
            }
            else if (sender == BUT_Graphit)
            {
                myCurve.IsY2Axis = false;
            }
        }

        private void BUT_cleargraph_Click(object sender, EventArgs e)
        {
            graphs = 0;
            foreach (LineItem line in zg1.GraphPane.CurveList)
            {
                line.Clear();
                line.Label.Text = "Value";
            }
            zg1.Invalidate();
        }

        private void BUT_loadlog_Click(object sender, EventArgs e)
        {
            // reset column count
            m_iColumnCount = 0;
            // clear existing lists
            zg1.GraphPane.CurveList.Clear();
            // reload
            Form1_Load(sender, e);
        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            Point mp = Control.MousePosition;

            List<string> options = new List<string>();

            foreach (DataRow datarow in m_dtCSV.Rows)
            {
                string celldata = datarow.ItemArray[1].ToString().Trim();
                if (!options.Contains(celldata))
                {
                    options.Add(celldata);
                }
            }

            ArdupilotMega.Controls.OptionForm opt = new ArdupilotMega.Controls.OptionForm();

            opt.StartPosition = FormStartPosition.Manual;
            opt.Location = mp;

            opt.Combobox.DataSource = options;
            opt.Button1.Text = "Filter";
            opt.Button2.Text = "Cancel";

            opt.ShowDialog(this);

            if (opt.SelectedItem != "")
            {
                (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = "col0 like '" + opt.SelectedItem + "'";
            }
            else
            {
                (dataGridView1.DataSource as DataTable).DefaultView.RowFilter = "";
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
        }

        void BUT_go_Click(object sender, EventArgs e)
        {
            ArdupilotMega.Controls.MyButton  but = sender as ArdupilotMega.Controls.MyButton;

            
        }

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

        }

        private void BUT_Graphit_R_Click(object sender, EventArgs e)
        {
            Graphit_Click(sender,e);
        }
    }
}