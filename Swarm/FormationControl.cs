using MissionPlanner.Utilities;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using GeoAPI.CoordinateSystems;
using GeoAPI.CoordinateSystems.Transformations;
using static IronPython.Modules._ast;
using static MissionPlanner.GCSViews.FlightPlanner;
using System.Globalization;
using System.IO;
using static MissionPlanner.Swarm.Grid;
using static MissionPlanner.Utilities.MissionFile;
using System.Linq;
using Newtonsoft.Json;
using Accord.Imaging.Filters;
using static Microsoft.Scripting.Hosting.Shell.ConsoleHostOptions;
using System.Text.RegularExpressions;

namespace MissionPlanner.Swarm
{
    public partial class FormationControl : Form
    {
        Formation SwarmInterface = null;
        bool threadrun = false;
        public bool Vertical { get; set; }
        public FormationControl()
        {
            InitializeComponent();

            SwarmInterface = new Formation();

            TopMost = true;

            Dictionary<String, MAVState> mavStates = new Dictionary<string, MAVState>();

            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    mavStates.Add(port.BaseStream.PortName + " " + mav.sysid + " " + mav.compid, mav);
                }
            }

            if (mavStates.Count == 0)
                return;

            bindingSource1.DataSource = mavStates;

            CMB_mavs.DataSource = bindingSource1;
            CMB_mavs.ValueMember = "Value";
            CMB_mavs.DisplayMember = "Key";

            bindingSource2.DataSource = mavStates;

            comboBox1.DataSource = bindingSource2;
            comboBox1.ValueMember = "Value";
            comboBox1.DisplayMember = "Key";

         
            updateicons();

            this.MouseWheel += new MouseEventHandler(FollowLeaderControl_MouseWheel);

            MessageBox.Show("this is beta, use at own risk");

            MissionPlanner.Utilities.Tracking.AddPage(this.GetType().ToString(), this.Text);
        }

        void FollowLeaderControl_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta < 0)
            {
                grid1.setScale(grid1.getScale() + 4);
            }
            else
            {
                grid1.setScale(grid1.getScale() - 4);
            }
        }

        void updateicons()
        {
            bindingSource1.ResetBindings(false);
            bindingSource2.ResetBindings(false);
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav == SwarmInterface.getLeader())
                    {
                        ((Formation)SwarmInterface).setOffsets(mav, 0, 0, 0);
                        var vector = SwarmInterface.getOffsets(mav);
                        grid1.UpdateIcon(mav, (float)vector.x, (float)vector.y, (float)vector.z, false);
                    }
                    else
                    {
                        var vector = SwarmInterface.getOffsets(mav);
                        grid1.UpdateIcon(mav, (float)vector.x, (float)vector.y, (float)vector.z, true);
                    }
                }
            }
            grid1.Invalidate();
        }

        private void CMB_mavs_SelectedIndexChanged(object sender, EventArgs e)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    if (mav == CMB_mavs.SelectedValue)
                    {
                        MainV2.comPort = port;
                        port.sysidcurrent = mav.sysid;
                        port.compidcurrent = mav.compid;
                    }
                }
            }
        }

        private void BUT_Start_Click(object sender, EventArgs e)
        {
            if (threadrun == true)
            {
                threadrun = false;
                BUT_Start.Text = Strings.Start;
                return;
            }

            if (SwarmInterface != null)
            {
                new System.Threading.Thread(mainloop) { IsBackground = true }.Start();
                BUT_Start.Text = Strings.Stop;
            }
        }

        void mainloop()
        {
            threadrun = true;

            // make sure leader is high freq updates
            SwarmInterface.Leader.parent.requestDatastream(MAVLink.MAV_DATA_STREAM.POSITION, 10, SwarmInterface.Leader.sysid, SwarmInterface.Leader.compid);
            SwarmInterface.Leader.cs.rateposition = 10;
            SwarmInterface.Leader.cs.rateattitude = 10;

            while (threadrun && !this.IsDisposed)
            {
                // update leader pos
                SwarmInterface.Update();

                // update other mavs
                SwarmInterface.SendCommand();

                // 10 hz
                System.Threading.Thread.Sleep(100);
            }
        }

        private void BUT_Arm_Click(object sender, EventArgs e)
        {
            
            
        if (CustomMessageBox.Show("Are you sure you want to Arm" , "提示",
                CustomMessageBox.MessageBoxButtons.YesNo) !=
            CustomMessageBox.DialogResult.Yes)
            return;

            if (SwarmInterface != null)
            {
                SwarmInterface.Arm_ALL(Vertical);
            }
        }

        private void BUT_Disarm_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.Disarm_ALL(Vertical);
            }
        }

        private void BUT_Takeoff_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.Takeoff_ALL(Vertical);
            }
        }

        private void BUT_Land_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.Land_ALL(Vertical);
            }
        }

        private void BUT_leader_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                var vectorlead = SwarmInterface.getOffsets(MainV2.comPort.MAV);

                foreach (var port in MainV2.Comports)
                {
                    foreach (var mav in port.MAVlist)
                    {
                        var vector = SwarmInterface.getOffsets(mav);

                        SwarmInterface.setOffsets(mav, (float)(vector.x - vectorlead.x),
                            (float)(vector.y - vectorlead.y),
                            (float)(vector.z - vectorlead.z));
                    }
                }

                SwarmInterface.setLeader(MainV2.comPort.MAV);
                updateicons();
                BUT_Start.Enabled = true;
                BUT_Updatepos.Enabled = true;
                savePoint.Enabled = true;
                loadPoint.Enabled = true;


                
            }
        }

        private void BUT_connect_Click(object sender, EventArgs e)
        {
            Comms.CommsSerialScan.Scan(true);

            DateTime deadline = DateTime.Now.AddSeconds(50);

            while (Comms.CommsSerialScan.foundport == false)
            {
                System.Threading.Thread.Sleep(100);

                if (DateTime.Now > deadline)
                {
                    CustomMessageBox.Show("Timeout waiting for autoscan/no mavlink device connected");
                    return;
                }
            }

            bindingSource1.ResetBindings(false);
            bindingSource2.ResetBindings(false);
        }

        public Vector3 getOffsetFromLeader(MAVState leader, MAVState mav)
        {
            //convert Wgs84ConversionInfo to utm
            CoordinateTransformationFactory ctfac = new CoordinateTransformationFactory();

            IGeographicCoordinateSystem wgs84 = GeographicCoordinateSystem.WGS84;

            int utmzone = (int)((leader.cs.lng - -186.0) / 6.0);

            IProjectedCoordinateSystem utm = ProjectedCoordinateSystem.WGS84_UTM(utmzone,
                leader.cs.lat < 0 ? false : true);

            ICoordinateTransformation trans = ctfac.CreateFromCoordinateSystems(wgs84, utm);

            double[] masterpll = { leader.cs.lng, leader.cs.lat };

            // get leader utm coords
            double[] masterutm = trans.MathTransform.Transform(masterpll);

            double[] mavpll = { mav.cs.lng, mav.cs.lat };

            //getLeader follower utm coords
            double[] mavutm = trans.MathTransform.Transform(mavpll);

            var heading = -leader.cs.yaw;

            var norotation = new Vector3(masterutm[1] - mavutm[1], masterutm[0] - mavutm[0], 0);

            norotation.x *= -1;
            norotation.y *= -1;

            return new Vector3(norotation.x * Math.Cos(heading * MathHelper.deg2rad) - norotation.y * Math.Sin(heading * MathHelper.deg2rad), norotation.x * Math.Sin(heading * MathHelper.deg2rad) + norotation.y * Math.Cos(heading * MathHelper.deg2rad), 0);
        }

        private void grid1_UpdateOffsets(MAVState mav, float x, float y, float z, Grid.icon ico)
        {
            if (mav == SwarmInterface.Leader)
            {
                CustomMessageBox.Show("Can not move Leader");
                ico.z = 0;
            }
            else
            {
                ((Formation)SwarmInterface).setOffsets(mav, x, y, z);
            }
        }

        private void Control_FormClosing(object sender, FormClosingEventArgs e)
        {
            threadrun = false;
        }

        private void BUT_Updatepos_Click(object sender, EventArgs e)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    mav.cs.UpdateCurrentSettings(null, true, port, mav);

                    if (mav == SwarmInterface.Leader)
                        continue;

                    Vector3 offset = getOffsetFromLeader(((Formation)SwarmInterface).getLeader(), mav);

                    if (Math.Abs(offset.x) < 200 && Math.Abs(offset.y) < 200)
                    {
                        //此处是控制页面的icon位置
                    
                        
                            
                        //此处的y,x，z比较特殊，其余的是x,y,z
                        grid1.UpdateIcon(mav, (float)offset.y, (float)offset.x, (float)offset.z, true);
                        ((Formation)SwarmInterface).setOffsets(mav, offset.y, offset.x, offset.z);
                    }
                }
            }
        }
        static bool IsPositiveNumber(string input)
        {

            string pattern = @"^[+-]?\d+(\.\d+)?$"; // 允许正负号，并允许小数部分
            return Regex.IsMatch(input, pattern);
        }

        private void BUT_UpdateOption_pos_Click(object sender, EventArgs e)
        {
        

           
            bool isGreaterThanZero1 = IsPositiveNumber(textBox1.Text);
            bool isGreaterThanZero2 = IsPositiveNumber(textBox2.Text);
            bool isGreaterThanZero3 = IsPositiveNumber(textBox3.Text);
            if (!isGreaterThanZero1) {
                MessageBox.Show("请在x轴输入非空数字");
                return;
            }
            if (!isGreaterThanZero2)
            {
                MessageBox.Show("请在y轴输入非空数字");
                return;
            }
            if (!isGreaterThanZero3)
            {
                MessageBox.Show("请在z轴输入非空数字");
                return;
            }

            float selectedNumber2 = float.Parse(textBox1.Text);
            float selectedNumber3 = float.Parse(textBox2.Text);
            float selectedNumber4 = float.Parse(textBox3.Text);
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                   
                    if (mav == comboBox1.SelectedValue)
                    {
                        if (mav == SwarmInterface.Leader)
                            continue;



                       


                        grid1.UpdateIcon(mav, selectedNumber2, selectedNumber3, selectedNumber4, true);
                        ((Formation)SwarmInterface).setOffsets(mav, selectedNumber2, selectedNumber3, selectedNumber4);
                    }
                }
            }




        }

        private void timer_status_Tick(object sender, EventArgs e)
        {
            // clean up old
            foreach (Control ctl in PNL_status.Controls)
            {
                bool match = false;
                foreach (var port in MainV2.Comports)
                {
                    foreach (var mav in port.MAVlist)
                    {
                        if (mav == (MAVState)ctl.Tag)
                        {
                            match = true;

                        }
                    }
                }

                if (match == false)
                    ctl.Dispose();
            }

            // setup new
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    bool exists = false;
                    foreach (Control ctl in PNL_status.Controls)
                    {
                        if (ctl is Status && ctl.Tag == mav)
                        {
                            exists = true;
                            ((Status)ctl).GPS.Text = mav.cs.gpsstatus >= 3 ? "OK" : "Bad";
                            ((Status)ctl).Armed.Text = mav.cs.armed.ToString();
                            ((Status)ctl).Mode.Text = mav.cs.mode;
                            ((Status)ctl).MAV.Text = mav.ToString();
                            ((Status)ctl).Guided.Text = mav.GuidedMode.x / 1e7 + "," + mav.GuidedMode.y / 1e7 + "," +
                                                         mav.GuidedMode.z;
                            ((Status)ctl).Location1.Text = mav.cs.lat + ",\n" + mav.cs.lng + ",\n" +
                                                            mav.cs.alt;

                            if (mav == SwarmInterface.Leader)
                            {
                                ((Status)ctl).ForeColor = Color.Red;
                            }
                            else
                            {
                                ((Status)ctl).ForeColor = Color.Black;
                            }
                        }
                    }

                    if (!exists)
                    {
                        Status newstatus = new Status();
                        newstatus.Tag = mav;
                        PNL_status.Controls.Add(newstatus);
                    }
                }
            }
        }

        private void but_guided_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.GuidedMode_ALL(Vertical);
            }
        }

        private void but_auto_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.AutoMode_ALL(Vertical);
            }
        }
        internal string wpfilename;

        
        public void BUT_LoadPoint_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fd = new OpenFileDialog())
            {
                fd.Filter = "All Supported Types|*.txt;*.waypoints;*.shp;*.plan;*.kml";
                if (Directory.Exists(Settings.Instance["WPFileDirectory"] ?? ""))
                    fd.InitialDirectory = Settings.Instance["WPFileDirectory"];
                DialogResult result = fd.ShowDialog();
                string file = fd.FileName;

                if (File.Exists(file))
                {
                    Settings.Instance["WPFileDirectory"] = Path.GetDirectoryName(file);
                   
                   
                        string line = "";
                        using (var fstream = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                        using (var fs = new StreamReader(fstream))
                        {
                            line = fs.ReadLine();
                        }

                        if (line.StartsWith("{"))
                        {
                            var format = ReadFile(file);

                            var cmds = ConvertToPoint(format);

                            BUT_Updatepos_Click_New(cmds);
                        }
                }
            }
        }

        

        private void BUT_Updatepos_Click_New(List<new_icon> icons)
        {
            foreach (var port in MainV2.Comports)
            {
                foreach (var mav in port.MAVlist)
                {
                    mav.cs.UpdateCurrentSettings(null, true, port, mav);

                    //if (mav == SwarmInterface.Leader)
                    //    continue;

                    Vector3 offset = getOffsetFromLeader(((Formation)SwarmInterface).getLeader(), mav);
                    
                    if (Math.Abs(offset.x) < 200 && Math.Abs(offset.y) < 200)
                    {
                        //此处是控制页面的icon位置
                        //grid1.UpdateIcon(mav, (float)offset.y, (float)offset.x, (float)offset.z, true);
                        //((Formation)SwarmInterface).setOffsets(mav, offset.x, offset.y, offset.z);

                        //offset.x = 1+ mav.sysid;
                        //offset.y = 1+ mav.sysid;
                        //offset.z = 1+ mav.sysid;
                        foreach (var icon in icons) {
                            if (icon.interf == mav.sysid) {
                                bool colorIsRed = false;
                                if (icon.Color.Name.Equals( "Red")) {
                                    colorIsRed = true;
                                }
                                grid1.UpdateIcon(mav, (float)icon.x, (float)icon.y, (float)icon.z, colorIsRed);
                                ((Formation)SwarmInterface).setOffsets(mav, icon.x, icon.y, icon.z);
                            }
                        }

                           
                    }
                }
            }
        }

        private void BUT_SavePoint_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog fd = new SaveFileDialog())
            {
                fd.Filter = "Mission|*.waypoints;*.txt|Mission JSON|*.mission";
                fd.DefaultExt = ".waypoints";
                fd.InitialDirectory = Settings.Instance["WPFileDirectory"] ?? "";
                fd.FileName = wpfilename;
                DialogResult result = fd.ShowDialog();
                string file = fd.FileName;
                List<new_icon> icons = new List<new_icon>();

                if (file != "" && result == DialogResult.OK)
                {
                    Settings.Instance["WPFileDirectory"] = Path.GetDirectoryName(file);

                    try
                    {
                        
                            if (SwarmInterface != null)
                            {
                                var vectorlead = SwarmInterface.getOffsets(MainV2.comPort.MAV);

                                
                                foreach (var port in MainV2.Comports)
                                {
                                    foreach (var mav in port.MAVlist)
                                    {

                                    new_icon save_icon = new new_icon();
                                    var vector = SwarmInterface.getOffsets(mav);
                                        save_icon.x = (float)vector.x;
                                        save_icon.y = (float)vector.y;
                                        save_icon.z = (float)vector.z;
                                        if (mav == SwarmInterface.Leader) {
                                            save_icon.Color = Color.Blue;
                                        }
                                        save_icon.interf = mav.sysid;
         
                                        icons.Add( save_icon);
                                    }
                                }
                            }

                            // Convert icons list to RootObject type
                            var format = ConvertFromIcon(icons);

                            // Now you can save this format as object
                            WriteFile_icon(file, format);
                            return;
                        
                    }
                    catch (Exception)
                    {
                        CustomMessageBox.Show(Strings.ERROR);
                    }
                }
            }
        }
        public static void WriteFile_icon(string filename, RootObject_ICon format)
        {
            var fileout = JsonConvert.SerializeObject(format, Formatting.Indented);

            File.WriteAllText(filename, fileout);
        }
        public class new_icon
        {
            public float x = 0;
            public float y = 0;
            public float z = 10;
            public int icosize = 20;
            public RectangleF bounds = new RectangleF();
            public Color Color = Color.Red;
            public String Name = "";
            public int interf = 0;
            public bool Movable = true;
        }
            public class RootObject_ICon
        {
            public string fileType { get; set; }
            public GeoFence geoFence { get; set; }
            public string groundStation { get; set; }
            public List<new_icon> icon { get; set; }
            public RallyPoints rallyPoints { get; set; }
            public int version { get; set; }
        }
        // Modified ConvertFromIcon method
        public static RootObject_ICon ConvertFromIcon(List<new_icon> list, byte frame = (byte)MAVLink.MAV_FRAME.GLOBAL_RELATIVE_ALT)
        {
            RootObject_ICon temp = new RootObject_ICon()
            {
                groundStation = "MissionPlanner_ICon",
                version = 1,
                icon = new List<new_icon>()
            };


            temp.icon = list;

            return temp;
        }

        public static List<new_icon> ConvertToPoint(RootObject_ICon format)
        {
            List<new_icon> cmds = new List<new_icon>();

            // 空值检查
            if (format == null || format.icon == null)
                return cmds;

            // 遍历 RootObject_ICon 中的 Grid.icon 列表
            foreach (var gridIcon in format.icon)
            {
                // 创建目标 icon 对象
                var targetIcon = new new_icon();

                // 映射必要属性 (根据实际字段调整)
                targetIcon.x = gridIcon.x;
                targetIcon.y = gridIcon.y;
                targetIcon.z = gridIcon.z;
                targetIcon.Color = gridIcon.Color; 
                targetIcon.interf = gridIcon.interf; 
       

                cmds.Add(targetIcon);
            }

            return cmds;
        }

        public static RootObject_ICon ReadFile(string filename)
        {
            using (var file =
                new StreamReader(File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)))
            {
                var output = JsonConvert.DeserializeObject<RootObject_ICon>(file.ReadToEnd());

                return output;
            }
        }

        private void BUT_Brake_Click(object sender, EventArgs e)
        {
            if (SwarmInterface != null)
            {
                SwarmInterface.Brake_ALL(Vertical);
            }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Vertical = checkBox1.Checked;


        }
        
    }
}