using log4net;
using MissionPlanner.Controls;
using MissionPlanner.Utilities;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using ZedGraph; // Graphs

namespace MissionPlanner.Log
{
    public partial class MavlinkLog : Form
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        List<string> selection = new List<string>();
        List<string> options = new List<string>();

        Hashtable datappl = new Hashtable();
        Hashtable packetdata = new Hashtable();

        PointLatLngAlt homepos = new PointLatLngAlt();

        string tlogfilemask = "Telemetry Log|*.tlog;*.tlog.*";

        public MavlinkLog()
        {
            InitializeComponent();

            zg1.GraphPane.YAxis.Title.IsVisible = false;
            zg1.GraphPane.Y2Axis.Title.Text = "";
            zg1.GraphPane.Title.IsVisible = true;
            zg1.GraphPane.Title.Text = "Mavlink Log Graph";
            zg1.GraphPane.XAxis.Title.Text = "Time (sec)";

            zg1.GraphPane.XAxis.Type = AxisType.Date;
            zg1.GraphPane.XAxis.Scale.Format = "HH:mm:ss";
            zg1.GraphPane.XAxis.Scale.MajorUnit = DateUnit.Minute;
            zg1.GraphPane.XAxis.Scale.MinorUnit = DateUnit.Second;
            zg1.PointDateFormat = "HH:mm:ss";

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        private void BUT_redokml_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = tlogfilemask;
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    string offsetalt = "0";

                    if (Control.ModifierKeys == Keys.Shift)
                    {
                        InputBox.Show("Alt offset",
                            "Please enter your offset altitude",
                            ref offsetalt);

                        float temp = 0;
                        if (!float.TryParse(offsetalt, out temp))
                        {
                            CustomMessageBox.Show("Bad Offset", "Error");
                            return;
                        }
                    }

                    foreach (string logfile in openFileDialog1.FileNames)
                    {
                        using (MAVLinkInterface mine = new MAVLinkInterface())
                        {
                            try
                            {
                                mine.logplaybackfile =
                                    new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read,
                                        FileShare.Read));
                            }
                            catch (Exception ex)
                            {
                                log.Debug(ex.ToString());
                                CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                                return;
                            }

                            mine.logreadmode = true;
                            mine.speechenabled = false;

                            bool newsample = false;
                            int sysidsample = 0;
                            int compidsample = 0;

                            mine.OnPacketReceived += ((o, message) =>
                            {
                                if (message.msgid == (int)MAVLink.MAVLINK_MSG_ID.GLOBAL_POSITION_INT)
                                {
                                    newsample = true;
                                    sysidsample = message.sysid;
                                    compidsample = message.compid;
                                }
                            });

                            int appui = 0;
                            Dictionary<int, List<CurrentState>> flightdataDictionary =
                                new Dictionary<int, List<CurrentState>>();

                            while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                            {
                                int percent =
                                    (int)
                                    ((float)mine.logplaybackfile.BaseStream.Position /
                                     (float)mine.logplaybackfile.BaseStream.Length * 100.0f);
                                if (progressBar1.Value != percent)
                                {
                                    progressBar1.Value = percent;
                                    progressBar1.Refresh();
                                }

                                MAVLink.MAVLinkMessage packet = mine.readPacket();

                                mine.MAV.cs.datetime = mine.lastlogread;

                                if (appui != DateTime.Now.Second)
                                {
                                    // cant do entire app as mixes with flightdata timer
                                    this.Refresh();
                                    appui = DateTime.Now.Second;
                                }

                                foreach (var mav in mine.MAVlist)
                                {
                                    mav.cs.UpdateCurrentSettings(null, true, mine, mav);
                                }

                                if (newsample)
                                {
                                    newsample = false;

                                    if (!flightdataDictionary.ContainsKey(sysidsample))
                                        flightdataDictionary[sysidsample] = new List<CurrentState>();

                                    flightdataDictionary[sysidsample]
                                        .Add((CurrentState)mine.MAVlist[sysidsample, compidsample].cs.Clone());
                                }
                            }

                            mine.logreadmode = false;
                            mine.logplaybackfile.Close();
                            mine.logplaybackfile = null;

                            string basealtstring = "0";

                            if (mine.MAV.wps.ContainsKey(0))
                            {
                                basealtstring = (mine.MAV.wps[0].z + float.Parse(offsetalt)).ToString();
                            }
                            else
                            {
                                InputBox.Show("Relative Alt",
                                    "Please enter your home altitude, or press cancel to use absolute alt",
                                    ref basealtstring);
                            }

                            Application.DoEvents();

                            log.Info(mine.MAV.cs.firmware + " : " + logfile);

                            foreach (var flightdata in flightdataDictionary)
                            {
                                MavlinkLogBase.writeGPX(logfile + "-" + flightdata.Key, flightdata.Value);
                            }

                            MavlinkLogBase.writeKML(logfile + ".kml", flightdataDictionary, (a) =>
                                {
                                    progressBar1.Value = (int)a;
                                    progressBar1.Refresh();
                                }
                                , double.Parse(basealtstring) / CurrentState.multiplierdist);

                            progressBar1.Value = 100;
                        }
                    }
                }
            }
        }



        private void BUT_humanreadable_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = tlogfilemask;
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
                        using (MAVLinkInterface mine = new MAVLinkInterface())
                        {
                            try
                            {
                                mine.logplaybackfile =
                                    new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read));
                            }
                            catch (Exception ex)
                            {
                                log.Debug(ex.ToString());
                                CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                                return;
                            }

                            mine.logreadmode = true;

                            StreamWriter sw =
                                new StreamWriter(Path.GetDirectoryName(logfile) + Path.DirectorySeparatorChar +
                                                 Path.GetFileNameWithoutExtension(logfile) + ".txt");

                            while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                            {
                                int percent =
                                    (int)
                                        ((float)mine.logplaybackfile.BaseStream.Position /
                                         (float)mine.logplaybackfile.BaseStream.Length * 100.0f);
                                if (progressBar1.Value != percent)
                                {
                                    progressBar1.Value = percent;
                                    progressBar1.Refresh();
                                }

                                MAVLink.MAVLinkMessage packet = mine.readPacket();
                                string text = "";
                                mine.DebugPacket(packet, ref text);

                                sw.Write(mine.lastlogread + " " + text);
                            }

                            sw.Close();

                            progressBar1.Value = 100;

                            mine.logreadmode = false;
                            mine.logplaybackfile.Close();
                            mine.logplaybackfile = null;
                        }
                    }
                }
            }
        }

        private void BUT_graphmavlog_Click(object sender, EventArgs e)
        {
            //http://devreminder.wordpress.com/net/net-framework-fundamentals/c-dynamic-math-expression-evaluation/
            //http://www.c-sharpcorner.com/UploadFile/mgold/CodeDomCalculator08082005003253AM/CodeDomCalculator.aspx

            //string mathExpression = "(1+1)*3";
            //Console.WriteLine(String.Format("{0}={1}",mathExpression, Evaluate(mathExpression)));


            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                try
                {
                    //  openFileDialog1.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath) + Path.DirectorySeparatorChar + @"logs" + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist
                openFileDialog1.Filter = tlogfilemask;
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = false;


                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    this.Text = "Log - " + Path.GetFileName(openFileDialog1.FileName);

                    List<string> fields = GetLogFileValidFields(openFileDialog1.FileName);

                    zg1.GraphPane.CurveList.Clear();

                    //GetLogFileData(zg1, openFileDialog1.FileName, fields);

                    try
                    {
                        // fix new line types
                        ThemeManager.ApplyThemeTo(this);

                        zg1.Invalidate();
                        zg1.AxisChange();
                    }
                    catch
                    {
                    }
                }
            }
        }


        // Form selectform;

        private static string ReplaceLastOccurrence(string Source, string Find, string Replace)
        {
            int place = Source.LastIndexOf(Find);

            if (place == -1)
                return Source;

            string result = Source.Remove(place, Find.Length).Insert(place, Replace);
            return result;
        }

        private List<string> GetLogFileValidFields(string logfile)
        {
            // if (selectform != null && !selectform.IsDisposed)
            //     selectform.Close();

            // selectform = SelectDataToGraphForm();

            Hashtable seenIt = new Hashtable();

            selection = new List<string>();

            options = new List<string>();

            this.datappl.Clear();
            this.packetdata.Clear();

            colorStep = 0;

            using (MAVLinkInterface MavlinkInterface = new MAVLinkInterface())
            {
                try
                {
                    MavlinkInterface.logplaybackfile =
                        new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read));
                }
                catch (Exception ex)
                {
                    log.Debug(ex.ToString());
                    CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                    return options;
                }
                MavlinkInterface.logreadmode = true;

                CurrentState cs = new CurrentState();

                // to get first packet time
                MavlinkInterface.getHeartBeat();
                MavlinkInterface.setAPType(MavlinkInterface.MAV.sysid, MavlinkInterface.MAV.compid);

                DateTime startlogtime = MavlinkInterface.lastlogread;

                while (MavlinkInterface.logplaybackfile.BaseStream.Position <
                       MavlinkInterface.logplaybackfile.BaseStream.Length)
                {
                    int percent =
                        (int)
                            ((float)MavlinkInterface.logplaybackfile.BaseStream.Position /
                             (float)MavlinkInterface.logplaybackfile.BaseStream.Length * 100.0f);
                    if (progressBar1.Value != percent)
                    {
                        progressBar1.Value = percent;
                        progressBar1.Refresh();
                    }

                    MAVLink.MAVLinkMessage packet = MavlinkInterface.readPacket();

                    cs.datetime = MavlinkInterface.lastlogread;

                    cs.UpdateCurrentSettings(null, true, MavlinkInterface);

                    object data = MavlinkInterface.DebugPacket(packet, false);

                    if (data == null)
                    {
                        log.Info("No info on packet");
                        continue;
                    }

                    if (data is MAVLink.mavlink_heartbeat_t)
                    {
                        if (((MAVLink.mavlink_heartbeat_t)data).type == (byte)MAVLink.MAV_TYPE.GCS)
                            continue;
                    }

                    Type test = data.GetType();


                    if (true)
                    {
                        string packetname = ReplaceLastOccurrence(test.Name, "_t", "");
                        packetname = packetname.Replace("mavlink_", "").ToUpper();

                        if (!packetdata.ContainsKey(packetname))
                        {
                            packetdata[packetname] = new Dictionary<DateTime, object>();
                        }

                        Dictionary<DateTime, object> temp = (Dictionary<DateTime, object>)packetdata[packetname];

                        //double time = (MavlinkInterface.lastlogread - startlogtime).TotalMilliseconds / 1000.0;
                        DateTime time = MavlinkInterface.lastlogread;

                        temp[time] = data;
                    }

                    foreach (var field in test.GetFields())
                    {
                        // field.Name has the field's name.

                        object fieldValue = field.GetValue(data); // Get value

                        if (field.FieldType.IsArray)
                        {
                        }
                        else
                        {
                            if (!seenIt.ContainsKey(field.DeclaringType.Name + "." + field.Name))
                            {
                                seenIt[field.DeclaringType.Name + "." + field.Name] = 1;
                                //AddDataOption(selectform, field.Name + " " + field.DeclaringType.Name);
                                options.Add(field.DeclaringType.Name + "." + field.Name);
                            }

                            if (!this.datappl.ContainsKey(field.Name + " " + field.DeclaringType.Name))
                                this.datappl[field.Name + " " + field.DeclaringType.Name] = new PointPairList();

                            PointPairList list =
                                ((PointPairList)this.datappl[field.Name + " " + field.DeclaringType.Name]);

                            object value = fieldValue;
                            // seconds scale
                            //double time = (MavlinkInterface.lastlogread - startlogtime).TotalMilliseconds / 1000.0;

                            XDate time = new XDate(MavlinkInterface.lastlogread);

                            if (value.GetType() == typeof(Single))
                            {
                                list.Add(time, (Single)field.GetValue(data));
                            }
                            else if (value.GetType() == typeof(short))
                            {
                                list.Add(time, (short)field.GetValue(data));
                            }
                            else if (value.GetType() == typeof(ushort))
                            {
                                list.Add(time, (ushort)field.GetValue(data));
                            }
                            else if (value.GetType() == typeof(byte))
                            {
                                list.Add(time, (byte)field.GetValue(data));
                            }
                            else if (value.GetType() == typeof(sbyte))
                            {
                                list.Add(time, (sbyte)field.GetValue(data));
                            }
                            else if (value.GetType() == typeof(Int32))
                            {
                                list.Add(time, (Int32)field.GetValue(data));
                            }
                            else if (value.GetType() == typeof(UInt32))
                            {
                                list.Add(time, (UInt32)field.GetValue(data));
                            }
                            else if (value.GetType() == typeof(ulong))
                            {
                                list.Add(time, (ulong)field.GetValue(data));
                            }
                            else if (value.GetType() == typeof(long))
                            {
                                list.Add(time, (long)field.GetValue(data));
                            }
                            else if (value.GetType() == typeof(double))
                            {
                                list.Add(time, (double)field.GetValue(data));
                            }

                            else
                            {
                                Console.WriteLine("Unknown data type");
                            }
                        }
                    }
                }

                MavlinkInterface.logreadmode = false;
                MavlinkInterface.logplaybackfile.Close();
                MavlinkInterface.logplaybackfile = null;

                try
                {
                    dospecial("GPS_RAW_INT");
                }
                catch (Exception ex)
                {
                    log.Info(ex.ToString());
                }
                try
                {
                    addMagField();
                    addDistHome();
                    addIMUTime();
                }
                catch (Exception ex)
                {
                    log.Info(ex.ToString());
                }

                // custom sort based on packet name
                options.Sort(delegate (string c1, string c2) { return String.Compare(c1, c2); });
                //String.Compare(c1.Substring(0,c1.IndexOf('.')),c2.Substring(0,c2.IndexOf('.')))

                // this needs sorting
                /*    string lastitem = "";
                foreach (string item in options)
                {
                    var items = item.Split('.');
                    if (items[0] != lastitem)
                        AddHeader(selectform, items[0].Replace("mavlink_","").Replace("_t","").ToUpper());
                    AddDataOption(selectform, items[1] + " " + items[0]);
                    lastitem = items[0];
                }
                */
                // add new treeview
                ResetTreeView(options);

                //  selectform.Show();

                progressBar1.Value = 100;
            }

            return selection;
        }

        public static T Cast<T>(object o)
        {
            return (T)o;
        }

        void dospecial(string PacketName)
        {
            Dictionary<DateTime, object> temp = null;

            try
            {
                temp = (Dictionary<DateTime, object>)packetdata[PacketName];
            }
            catch
            {
                CustomMessageBox.Show("Bad PacketName");
                return;
            }

            string code = @"

        public double stage(object inp) {
            return getAltAboveHome((MAVLink.mavlink_gps_raw_int_t) inp);
        }

        public double getAltAboveHome(MAVLink.mavlink_gps_raw_int_t gps)
        {
            if (customforusenumber == -1 && gps.fix_type != 2)
                customforusenumber = gps.alt  / 1000.0f;

            return (gps.alt / 1000.0f) - customforusenumber;
        }
";

            // build the class using codedom
            CodeGen.BuildClass(code);

            // compile the class into an in-memory assembly.
            // if it doesn't compile, show errors in the window
            CompilerResults results = CodeGen.CompileAssembly();

            if (results != null && results.CompiledAssembly != null)
            {
                string field = "custom mavlink_custom_t"; // reverse bellow

                options.Add("mavlink_custom_t.custom");

                this.datappl[field] = new PointPairList();


                MethodInfo mi = RunCode(results);


                // from here
                PointPairList result = (PointPairList)this.datappl[field];

                try
                {
                    if (temp == null)
                        return;

                    object assemblyInstance = results.CompiledAssembly.CreateInstance("ExpressionEvaluator.Calculator");

                    foreach (DateTime time in temp.Keys)
                    {
                        XDate time2 = new XDate(time);
                        result.Add(time2, (double)mi.Invoke(assemblyInstance, new object[] { temp[time] }));
                    }
                }
                catch
                {
                }
            }
            else
            {
                CustomMessageBox.Show("Compile Failed");
                return;
            }

            object answer = CodeGen.runCode(code);

            Console.WriteLine(answer);
        }

        public MethodInfo RunCode(CompilerResults results)
        {
            Assembly executingAssembly = results.CompiledAssembly;
            try
            {
                //cant call the entry method if the assembly is null
                if (executingAssembly != null)
                {
                    object assemblyInstance = executingAssembly.CreateInstance("ExpressionEvaluator.Calculator");
                    //Use reflection to call the static Main function

                    Module[] modules = executingAssembly.GetModules(false);
                    Type[] types = modules[0].GetTypes();

                    //loop through each class that was defined and look for the first occurrance of the entry point method
                    foreach (Type type in types)
                    {
                        MethodInfo[] mis = type.GetMethods();
                        foreach (MethodInfo mi in mis)
                        {
                            if (mi.Name == "stage")
                            {
                                return mi;
                                //object result = mi.Invoke(assemblyInstance, null);
                                //return result.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: An exception occurred while executing the script\n{0}", ex);
            }
            return null;
        }

        PointPairList GetValuesForField(string name)
        {
            // eg RAW_IMU.xmag to "xmag mavlink_raw_imu_t"

            string[] items = name.ToLower().Split(new char[] { '.', ' ' });

            PointPairList list = ((PointPairList)this.datappl[items[1] + " mavlink_" + items[0] + "_t"]);

            return list;
        }

        void addMagField()
        {
            string field = "mag_field mavlink_custom_t";

            options.Add("mavlink_custom_t.mag_field");

            this.datappl[field] = new PointPairList();

            PointPairList list = ((PointPairList)this.datappl[field]);

            PointPairList listx = ((PointPairList)this.datappl["xmag mavlink_raw_imu_t"]);
            PointPairList listy = ((PointPairList)this.datappl["ymag mavlink_raw_imu_t"]);
            PointPairList listz = ((PointPairList)this.datappl["zmag mavlink_raw_imu_t"]);

            //(float)Math.Sqrt(Math.Pow(mx, 2) + Math.Pow(my, 2) + Math.Pow(mz, 2));

            for (int a = 0; a < listx.Count; a++)
            {
                double ans = Math.Sqrt(Math.Pow(listx[a].Y, 2) + Math.Pow(listy[a].Y, 2) + Math.Pow(listz[a].Y, 2));

                //Console.WriteLine("{0} {1} {2} {3}", ans, listx[a].Y, listy[a].Y, listz[a].Y);

                list.Add(listx[a].X, ans);
            }
        }

        void addIMUTime()
        {
            string field = "sitltime mavlink_custom_t";

            options.Add("mavlink_custom_t.sitltime");

            this.datappl[field] = new PointPairList();

            PointPairList list = ((PointPairList)this.datappl[field]);

            PointPairList listtime = ((PointPairList)this.datappl["time_usec mavlink_raw_imu_t"]);

            double lastrealtime = listtime[0].X;
            double lastvalue = listtime[0].Y * 1.0e-6;

            for (int a = 0; a < listtime.Count; a++)
            {
                double delta = ((listtime[a].Y * 1.0e-6) - lastvalue);

                // convert to seconds
                list.Add(listtime[a].X, delta);

                lastvalue = listtime[a].Y * 1.0e-6;
                lastrealtime = listtime[a].X;
            }
        }

        void addDistHome()
        {
            string field = "dist_home mavlink_custom_t";

            options.Add("mavlink_custom_t.dist_home");

            this.datappl[field] = new PointPairList();

            PointLatLngAlt home = new PointLatLngAlt();

            PointPairList list = ((PointPairList)this.datappl[field]);

            PointPairList listfix = ((PointPairList)this.datappl["fix_type mavlink_gps_raw_int_t"]);
            PointPairList listx = ((PointPairList)this.datappl["lat mavlink_gps_raw_int_t"]);
            PointPairList listy = ((PointPairList)this.datappl["lon mavlink_gps_raw_int_t"]);
            PointPairList listz = ((PointPairList)this.datappl["alt mavlink_gps_raw_int_t"]);

            for (int a = 0; a < listfix.Count; a++)
            {
                if (listfix[a].Y == 3)
                {
                    home = new PointLatLngAlt(listx[a].Y / 10000000.0, listy[a].Y / 10000000.0, listz[a].Y / 1000.0, "Home");
                    break;
                }
            }

            //(float)Math.Sqrt(Math.Pow(mx, 2) + Math.Pow(my, 2) + Math.Pow(mz, 2));

            for (int a = 0; a < listx.Count; a++)
            {
                double ans =
                    home.GetDistance(new PointLatLngAlt(listx[a].Y / 10000000.0, listy[a].Y / 10000000.0, listz[a].Y / 1000.0,
                        "Point"));

                //Console.WriteLine("{0} {1} {2} {3}", ans, listx[a].Y, listy[a].Y, listz[a].Y);

                list.Add(listx[a].X, ans);
            }
        }

        private void ResetTreeView(List<string> seenmessagetypes)
        {
            treeView1.Nodes.Clear();

            Hashtable addedrootnodes = new Hashtable();
            TreeNode tn = treeView1.TopNode;

            foreach (var item in seenmessagetypes)
            {
                var items = item.Split('.');

                var item1text = ReplaceLastOccurrence(items[0], "_t", "");
                item1text = item1text.Replace("mavlink_", "").ToUpper();
                var item2text = items[1];

                if (!addedrootnodes.ContainsKey(item1text))
                {
                    tn = new TreeNode(item1text);
                    treeView1.Nodes.Add(tn);
                    addedrootnodes[item1text] = 1;
                }

                tn.Nodes.Add(item2text);
            }
        }

        private void AddHeader(Form selectform, string Name)
        {
            System.Windows.Forms.Label lbl_head = new System.Windows.Forms.Label();

            log.Info("Add Header " + Name);

            lbl_head.Text = Name;
            lbl_head.Name = Name;
            lbl_head.Location = new System.Drawing.Point(x, y);
            lbl_head.Size = new System.Drawing.Size(100, 20);

            selectform.Controls.Add(lbl_head);

            Application.DoEvents();

            x += 0;
            y += 20;

            if (y > selectform.Height - 60)
            {
                x += 100;
                y = 10;

                selectform.Width = x + 100;
            }
        }

        int colorStep = 0;
        bool rightclick = false;

        void chk_box_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                selection.Add(((CheckBox)sender).Name);

                LineItem myCurve;

                int colorvalue = MavlinkLogBase.ColourValues[colorStep % MavlinkLogBase.ColourValues.Length];
                colorStep++;
                Console.WriteLine("Color " + colorvalue);

                myCurve = zg1.GraphPane.AddCurve(((CheckBox)sender).Name.Replace("mavlink_", ""),
                    (PointPairList)datappl[((CheckBox)sender).Name],
                    Color.FromArgb(unchecked(colorvalue + (int)0xff000000)), SymbolType.None);

                var split = ((CheckBox)sender).Name.Split(' ');

                if (split.Length == 2)
                {
                    var unit = MAVLink.GetUnit(split[0],
                        name: split[1].Replace("mavlink_", "").RemoveFromEnd("_t").ToUpper());

                    var index = zg1.GraphPane.YAxisList.IndexOf(unit);

                    var index2 = zg1.GraphPane.Y2AxisList.IndexOf(unit);

                    if (index != -1 && !rightclick)
                    {
                        myCurve.YAxisIndex = index;
                        myCurve.GetYAxis(zg1.GraphPane).IsVisible = true;
                    }
                    else if (index2 != -1 && rightclick)
                    {
                        myCurve.YAxisIndex = index2;
                        myCurve.GetYAxis(zg1.GraphPane).IsVisible = true;
                    }
                    else
                    {
                        if (rightclick)
                        {
                            index = zg1.GraphPane.AddY2Axis(unit);
                            myCurve.YAxisIndex = index;
                        }
                        else
                        {
                            index = zg1.GraphPane.AddYAxis(unit);
                            myCurve.YAxisIndex = index;
                        }
                    }

                }
                else
                {
                    myCurve.YAxisIndex = 0;
                }

                myCurve.Tag = ((CheckBox)sender).Name;

                if (myCurve.Tag.ToString() == "roll mavlink_attitude_t" ||
                    myCurve.Tag.ToString() == "pitch mavlink_attitude_t" ||
                    myCurve.Tag.ToString() == "yaw mavlink_attitude_t")
                {
                    PointPairList ppl = new PointPairList((PointPairList)datappl[((CheckBox)sender).Name]);
                    for (int a = 0; a < ppl.Count; a++)
                    {
                        ppl[a].Y = ppl[a].Y * (180.0 / Math.PI);
                    }

                    myCurve.Points = ppl;
                }

                double xMin, xMax, yMin, yMax;

                myCurve.GetRange(out xMin, out xMax, out yMin, out yMax, true, false, zg1.GraphPane);

                if (rightclick)
                {
                    myCurve.IsY2Axis = true;
                    zg1.GraphPane.Y2Axis.IsVisible = true;

                    myCurve.Label.Text = myCurve.Label.Text + "-R";
                }
            }
            else
            {
                selection.Remove(((CheckBox)sender).Name);
                foreach (var item in zg1.GraphPane.CurveList)
                {
                    if (item.Tag == null)
                        continue;
                    if (item.Tag.ToString() == ((CheckBox)sender).Name)
                    {
                        zg1.GraphPane.CurveList.Remove(item);
                        break;
                    }
                }
            }

            try
            {
                // fix new line types
                ThemeManager.ApplyThemeTo(this);

                zg1.GraphPane.XAxis.AxisGap = 0;

                zg1.Invalidate();
                zg1.AxisChange();
            }
            catch
            {
            }
        }

        int x = 10;
        int y = 10;

        private Form SelectDataToGraphForm()
        {
            Form selectform = new Form()
            {
                Name = "select",
                Width = 50,
                Height = 500,
                Text = "Graph This",
                TopLevel = true
            };

            x = 10;
            y = 10;

            AddHeader(selectform, "Left Click");
            AddHeader(selectform, "Left Axis");
            AddHeader(selectform, "Right Click");
            AddHeader(selectform, "Right Axis");

            ThemeManager.ApplyThemeTo(selectform);

            return selectform;
        }

        private void BUT_convertcsv_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = tlogfilemask;
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
                        using (MAVLinkInterface mine = new MAVLinkInterface())
                        {
                            try
                            {
                                mine.logplaybackfile =
                                    new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read));
                            }
                            catch (Exception ex)
                            {
                                log.Debug(ex.ToString());
                                CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                                return;
                            }
                            mine.logreadmode = true;

                            StreamWriter sw =
                                new StreamWriter(Path.GetDirectoryName(logfile) + Path.DirectorySeparatorChar +
                                                 Path.GetFileNameWithoutExtension(logfile) + ".csv");

                            while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                            {
                                int percent =
                                    (int)
                                        ((float)mine.logplaybackfile.BaseStream.Position /
                                         (float)mine.logplaybackfile.BaseStream.Length * 100.0f);
                                if (progressBar1.Value != percent)
                                {
                                    progressBar1.Value = percent;
                                    progressBar1.Refresh();
                                }

                                MAVLink.MAVLinkMessage packet = mine.readPacket();
                                string text = "";
                                mine.DebugPacket(packet, ref text, false, ",");

                                if (!String.IsNullOrEmpty(text))
                                    sw.Write(mine.lastlogread.ToString("yyyy-MM-ddTHH:mm:ss.fff") + "," + text);
                            }

                            sw.Close();

                            progressBar1.Value = 100;

                            mine.logreadmode = false;
                            mine.logplaybackfile.Close();
                            mine.logplaybackfile = null;
                        }
                    }
                }
            }
        }

        private void BUT_paramsfromlog_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = tlogfilemask;
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
                        try
                        {
                            using (MAVLinkInterface mine = new MAVLinkInterface())
                            {
                                try
                                {
                                    mine.logplaybackfile =
                                        new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read,
                                            FileShare.Read));
                                }
                                catch (Exception ex)
                                {
                                    log.Debug(ex.ToString());
                                    CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                                    return;
                                }

                                mine.logreadmode = true;

                                StreamWriter sw =
                                    new StreamWriter(Path.GetDirectoryName(logfile) + Path.DirectorySeparatorChar +
                                                     Path.GetFileNameWithoutExtension(logfile) + ".param");

                                int percent =
                                    (int)
                                        ((float)mine.logplaybackfile.BaseStream.Position /
                                         (float)mine.logplaybackfile.BaseStream.Length * 100.0f);
                                if (progressBar1.Value != percent)
                                {
                                    progressBar1.Value = percent;
                                    progressBar1.Refresh();
                                }

                                mine.getHeartBeat();

                                mine.getParamList();

                                foreach (string item in mine.MAV.param.Keys)
                                {
                                    sw.WriteLine(item + "\t" + mine.MAV.param[item]);
                                }

                                sw.Close();

                                progressBar1.Value = 100;

                                mine.logreadmode = false;
                                mine.logplaybackfile.Close();
                                mine.logplaybackfile = null;
                            }
                            CustomMessageBox.Show("File Saved with log file");
                        }
                        catch
                        {
                            CustomMessageBox.Show("Error Extracting params");
                        }
                    }
                }
            }
        }

        private void BUT_getwpsfromlog_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = tlogfilemask;
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
                        int wplists = 0;

                        using (MAVLinkInterface mine = new MAVLinkInterface())
                        {
                            try
                            {
                                mine.logplaybackfile =
                                    new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read));
                            }
                            catch (Exception ex)
                            {
                                log.Debug(ex.ToString());
                                CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                                return;
                            }

                            mine.logreadmode = true;

                            mine.getHeartBeat();

                            while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                            {
                                int percent =
                                    (int)
                                        ((float)mine.logplaybackfile.BaseStream.Position /
                                         (float)mine.logplaybackfile.BaseStream.Length * 100.0f);
                                if (progressBar1.Value != percent)
                                {
                                    progressBar1.Value = percent;
                                    progressBar1.Refresh();
                                }

                                ushort count = 0;
                                try
                                {
                                    count = mine.getWPCount();
                                }
                                catch
                                {
                                }

                                if (count == 0)
                                {
                                    continue;
                                }


                                StreamWriter sw =
                                    new StreamWriter(Path.GetDirectoryName(logfile) + Path.DirectorySeparatorChar +
                                                     Path.GetFileNameWithoutExtension(logfile) + "-" + wplists +
                                                     ".waypoints");

                                sw.WriteLine("QGC WPL 110");
                                sw.WriteLine("# wp count " + count);
                                try
                                {
                                    //get mission count info 
                                    var item =
                                        mine.MAV.getPacket((uint)MAVLink.MAVLINK_MSG_ID.MISSION_COUNT)
                                            .ToStructure<MAVLink.mavlink_mission_count_t>();
                                    mine.MAV.clearPacket((uint)MAVLink.MAVLINK_MSG_ID.MISSION_COUNT);
                                    sw.WriteLine("# count packet sent to comp " + item.target_component + " sys " +
                                                 item.target_system + " # " + item.count);
                                }
                                catch
                                {
                                }
                                for (ushort a = 0; a < count; a++)
                                {
                                    try
                                    {
                                        Locationwp wp = mine.getWP(a);
                                        //sw.WriteLine(item + "\t" + mine.param[item]);

                                        sw.Write((a)); // seq
                                        sw.Write("\t" + 0); // current
                                        sw.Write("\t" + wp.frame); //frame 
                                        sw.Write("\t" + wp.id);
                                        sw.Write("\t" + wp.p1.ToString("0.000000", CultureInfo.InvariantCulture));
                                        sw.Write("\t" + wp.p2.ToString("0.000000", CultureInfo.InvariantCulture));
                                        sw.Write("\t" + wp.p3.ToString("0.000000", CultureInfo.InvariantCulture));
                                        sw.Write("\t" + wp.p4.ToString("0.000000", CultureInfo.InvariantCulture));
                                        sw.Write("\t" + wp.lat.ToString("0.000000", CultureInfo.InvariantCulture));
                                        sw.Write("\t" + wp.lng.ToString("0.000000", CultureInfo.InvariantCulture));
                                        sw.Write("\t" + (wp.alt / CurrentState.multiplierdist).ToString("0.000000",
                                                     CultureInfo.InvariantCulture));
                                        sw.Write("\t" + 1);
                                        sw.WriteLine("");
                                    }
                                    catch
                                    {
                                        break;
                                    }
                                }

                                sw.Close();
                                wplists++;
                            }

                            progressBar1.Value = 100;

                            mine.logreadmode = false;
                            mine.logplaybackfile.Close();
                            mine.logplaybackfile = null;

                            if (openFileDialog1.FileNames.Length == 1)
                            {
                                if (wplists == 0)
                                {
                                    CustomMessageBox.Show("No Waypoint found in file!");
                                }
                                else
                                {
                                    CustomMessageBox.Show("File Saved with log file!");
                                }
                            }
                        }
                    }
                }
            }
        }

        private void BUT_matlab_Click(object sender, EventArgs e)
        {
            MatLabForms.ProcessTLog();
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
                    // has it already been graphed?
                    foreach (var item in zg1.GraphPane.CurveList)
                    {
                        if (item.Label.Text.StartsWith(e.Node.Text) &&
                            item.Label.Text.Contains(e.Node.Parent.Text.ToLower()))
                        {
                            return;
                        }
                    }

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
                    List<Axis> visibleAxises = new List<Axis>();
                    // tag line for removal
                    foreach (var item in zg1.GraphPane.CurveList)
                    {
                        if (item.Label.Text.StartsWith(e.Node.Text) &&
                            item.Label.Text.Contains(e.Node.Parent.Text.ToLower()))
                        {
                            removeitems.Add(item);
                            //break;
                        }
                        else
                        {
                            visibleAxises.Add(item.GetYAxis(zg1.GraphPane));
                        }
                    }
                    // remove lines
                    foreach (var item in removeitems)
                    {
                        zg1.GraphPane.CurveList.Remove(item);
                    }
                    // hide unused yaxis
                    foreach (var item in zg1.GraphPane.YAxisList)
                    {
                        if (!visibleAxises.Contains(item))
                            item.IsVisible = false;
                    }
                    foreach (var item in zg1.GraphPane.Y2AxisList)
                    {
                        if (!visibleAxises.Contains(item))
                            item.IsVisible = false;
                    }
                }

                zg1.Invalidate();
            }
            else if (e.Node != null && e.Node.Parent == null) // root nood ticked
            {
                if (e.Node.Checked)
                {
                    e.Node.Checked = false;
                    /* foreach (var child in e.Node.Nodes)
                    {
                        ((TreeNode)child).Checked = true;
                        var newe = new TreeNodeMouseClickEventArgs((TreeNode)child, e.Button, e.Clicks, e.X, e.Y);
                        treeView1_NodeMouseClick(child, newe);
                    }
                    */
                }
                else
                {
                }
            }
        }

        private void GraphItem(string parenttext, string text, bool leftaxis)
        {
            rightclick = !leftaxis;

            chk_box_CheckedChanged(
                new CheckBox() { Name = text + " mavlink_" + parenttext.ToLower() + "_t", Checked = true }, null);
        }
    }
}