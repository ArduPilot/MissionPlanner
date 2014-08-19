using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Collections;
using netDxf;
using netDxf.Entities;
using netDxf.Tables;
using netDxf.Header;
using System.Reflection;
using log4net;
using MissionPlanner.HIL;
using MissionPlanner.Controls;

namespace MissionPlanner
{
    public class MagCalib
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        const float rad2deg = (float)(180 / Math.PI);
        const float deg2rad = (float)(1.0 / rad2deg);

        static double[] ans;
        static double[] ans2;
       
        /// <summary>
        /// Self contained process tlog and save/display offsets
        /// </summary>
        public static void ProcessLog(int throttleThreshold = 0)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Log Files|*.tlog;*.log";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Multiselect = true;
            try
            {
                openFileDialog1.InitialDirectory = MainV2.LogDir + Path.DirectorySeparatorChar;
            }
            catch { } // incase dir doesnt exist

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    double[] ans;

                    if (openFileDialog1.FileName.ToLower().EndsWith("tlog"))
                    {
                        ans = getOffsets(openFileDialog1.FileName, throttleThreshold);
                    }
                    else
                    {
                        ans = getOffsetsLog(openFileDialog1.FileName);
                    }

                    if (ans.Length != 1)
                        SaveOffsets(ans);
                }
                catch (Exception ex) { log.Debug(ex.ToString()); }
            }
        }


        public static void DoGUIMagCalib(bool dointro = true)
        {
            ans = null;
            filtercompass1.Clear();
            datacompass1.Clear();
            datacompass2.Clear();
            filtercompass2.Clear();

            if (dointro)
                CustomMessageBox.Show("Please click ok and move the autopilot around all axises in a circular motion");

            ProgressReporterSphere prd = new ProgressReporterSphere();

            prd.btnCancel.Text = "Done";

            Utilities.ThemeManager.ApplyThemeTo(prd);

            prd.DoWork += prd_DoWork;

            prd.RunBackgroundOperationAsync();

            if (ans != null)
                MagCalib.SaveOffsets(ans);

            if (ans2 != null)
                MagCalib.SaveOffsets2(ans2);
        }

        // filter data points to only x number per quadrant
        static int div = 20;
        static Hashtable filtercompass1 = new Hashtable();
        static Hashtable filtercompass2 = new Hashtable();

        // list of x,y,z 's
        static List<Tuple<float, float, float>> datacompass1 = new List<Tuple<float, float, float>>();

        // list no 2
        static List<Tuple<float, float, float>> datacompass2 = new List<Tuple<float, float, float>>();

        static bool ReceviedPacket(byte[] rawpacket)
        {
            if (rawpacket[5] == (byte)MAVLink.MAVLINK_MSG_ID.SCALED_IMU2)
            {
                MAVLink.mavlink_scaled_imu2_t packet = rawpacket.ByteArrayToStructure<MAVLink.mavlink_scaled_imu2_t>();

                // filter dataset
                string item = (int)(packet.xmag / div) + "," +
                    (int)(packet.ymag / div) + "," +
                    (int)(packet.zmag / div);

                if (filtercompass2.ContainsKey(item))
                {
                    filtercompass2[item] = (int)filtercompass2[item] + 1;

                    if ((int)filtercompass2[item] > 3)
                        return false;
                }
                else
                {
                    filtercompass2[item] = 1;
                }

                // values - offsets are 0
                float rawmx = packet.xmag;
                float rawmy = packet.ymag;
                float rawmz = packet.zmag;

                // add data
                lock (datacompass2)
                {
                    datacompass2.Add(new Tuple<float, float, float>(rawmx, rawmy, rawmz));
                }

                return true;
            }
            else if (rawpacket[5] == (byte)MAVLink.MAVLINK_MSG_ID.RAW_IMU)
            {
                MAVLink.mavlink_raw_imu_t packet = rawpacket.ByteArrayToStructure<MAVLink.mavlink_raw_imu_t>();


                // filter dataset
                string item = (int)(packet.xmag / div) + "," +
                    (int)(packet.ymag / div) + "," +
                    (int)(packet.zmag / div);

                if (filtercompass1.ContainsKey(item))
                {
                    filtercompass1[item] = (int)filtercompass1[item] + 1;

                    if ((int)filtercompass1[item] > 3)
                        return false;
                }
                else
                {
                    filtercompass1[item] = 1;
                }

                // values
                float rawmx = packet.xmag - (float)MainV2.comPort.MAV.cs.mag_ofs_x;
                float rawmy = packet.ymag - (float)MainV2.comPort.MAV.cs.mag_ofs_y;
                float rawmz = packet.zmag - (float)MainV2.comPort.MAV.cs.mag_ofs_z;

                // add data
                lock (datacompass1)
                {
                    datacompass1.Add(new Tuple<float, float, float>(rawmx, rawmy, rawmz));
                }

                return true;
            }

            return true;
        }

        static void prd_DoWork(object sender, ProgressWorkerEventArgs e, object passdata = null)
        {
            // turn learning off
            MainV2.comPort.setParam("COMPASS_LEARN", 0);

            bool havecompass2 = false;

            //compass2 get mag2 offsets
            if (MainV2.comPort.MAV.param.ContainsKey("COMPASS_OFS2_X"))
            {
                //com2ofsx = MainV2.comPort.GetParam("COMPASS_OFS2_X");
                //com2ofsy = MainV2.comPort.GetParam("COMPASS_OFS2_Y");
                //com2ofsz = MainV2.comPort.GetParam("COMPASS_OFS2_Z");

                MainV2.comPort.setParam("COMPASS_OFS2_X", 0);
                MainV2.comPort.setParam("COMPASS_OFS2_Y", 0);
                MainV2.comPort.setParam("COMPASS_OFS2_Z", 0);

                havecompass2 = true;
            }
            
            // old method
            float minx = 0;
            float maxx = 0;
            float miny = 0;
            float maxy = 0;
            float minz = 0;
            float maxz = 0;

            // backup current rate and set to 10 hz
            byte backupratesens = MainV2.comPort.MAV.cs.ratesensors;
            MainV2.comPort.MAV.cs.ratesensors = 10;
            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MainV2.comPort.MAV.cs.ratesensors); // mag captures at 10 hz

            var sub = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.RAW_IMU, ReceviedPacket);

            var sub2 = MainV2.comPort.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.SCALED_IMU2, ReceviedPacket);

            string extramsg = "";

            ((ProgressReporterSphere)sender).sphere1.Clear();
            ((ProgressReporterSphere)sender).sphere2.Clear();

            int lastcount = 0;
            DateTime lastlsq = DateTime.MinValue;
            DateTime lastlsq2 = DateTime.MinValue;

            HIL.Vector3 centre = new HIL.Vector3();

            while (true)
            {
                // slow down execution
                System.Threading.Thread.Sleep(20);

                ((ProgressReporterDialogue)sender).UpdateProgressAndStatus(-1, "Got " + datacompass1.Count + " Samples " + extramsg);

                if (e.CancelRequested)
                {
                    // restore old sensor rate
                    MainV2.comPort.MAV.cs.ratesensors = backupratesens;
                    MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MainV2.comPort.MAV.cs.ratesensors);

                    e.CancelAcknowledged = false;
                    e.CancelRequested = false;
                    break;
                }

                if (datacompass1.Count == 0)
                    continue;
                
                // dont use dup data
                if (lastcount == datacompass1.Count)
                    continue;

                lastcount = datacompass1.Count;

                float rawmx = datacompass1[datacompass1.Count - 1].Item1;
                float rawmy = datacompass1[datacompass1.Count - 1].Item2;
                float rawmz = datacompass1[datacompass1.Count - 1].Item3;

                // for old method
                setMinorMax(rawmx, ref minx, ref maxx);
                setMinorMax(rawmy, ref miny, ref maxy);
                setMinorMax(rawmz, ref minz, ref maxz);

                // get the current estimated centerpoint
                //new HIL.Vector3((float)-((maxx + minx) / 2), (float)-((maxy + miny) / 2), (float)-((maxz + minz) / 2));

                // run lsq every second when more than 100 datapoints
                if (datacompass1.Count > 100 && lastlsq.Second != DateTime.Now.Second)
                {
                    lastlsq = DateTime.Now;
                    lock (datacompass1)
                    {
                        var lsq = MagCalib.LeastSq(datacompass1, false);
                        // simple validation
                        if (Math.Abs(lsq[0]) < 999)
                        {
                            centre = new HIL.Vector3(lsq[0], lsq[1], lsq[2]);
                            log.Info("new centre " + centre.ToString());

                            ((ProgressReporterSphere)sender).sphere1.CenterPoint = new OpenTK.Vector3((float)centre.x, (float)centre.y, (float)centre.z);
                        }
                    }
                }

                // run lsq every second when more than 100 datapoints
                if (datacompass2.Count > 100 && lastlsq2.Second != DateTime.Now.Second)
                {
                    lastlsq2 = DateTime.Now;
                    lock (datacompass2)
                    {
                        var lsq = MagCalib.LeastSq(datacompass2, false);
                        // simple validation
                        if (Math.Abs(lsq[0]) < 999)
                        {
                            HIL.Vector3 centre2 = new HIL.Vector3(lsq[0], lsq[1], lsq[2]);
                            log.Info("new centre2 " + centre2.ToString());

                            ((ProgressReporterSphere)sender).sphere2.CenterPoint = new OpenTK.Vector3((float)centre2.x, (float)centre2.y, (float)centre2.z);
                        }
                    }
                }

                // add to sphere with center correction
                ((ProgressReporterSphere)sender).sphere1.AddPoint(new OpenTK.Vector3(rawmx, rawmy, rawmz));
                ((ProgressReporterSphere)sender).sphere1.AimClear();

                if (havecompass2 && datacompass2.Count > 0)
                {
                    float raw2mx = datacompass2[datacompass2.Count - 1].Item1;
                    float raw2my = datacompass2[datacompass2.Count - 1].Item2;
                    float raw2mz = datacompass2[datacompass2.Count - 1].Item3;

                    ((ProgressReporterSphere)sender).sphere2.AddPoint(new OpenTK.Vector3(raw2mx, raw2my, raw2mz));
                    ((ProgressReporterSphere)sender).sphere2.AimClear();
                }
                
                HIL.Vector3 point;

                point = new HIL.Vector3(rawmx, rawmy, rawmz) + centre;

                //find the mean radius                    
                float radius = 0;
                for (int i = 0; i < datacompass1.Count; i++)
                {
                    point = new HIL.Vector3(datacompass1[i].Item1, datacompass1[i].Item2, datacompass1[i].Item3);
                    radius += (float)(point + centre).length();
                }
                radius /= datacompass1.Count;

                //test that we can find one point near a set of points all around the sphere surface
                string displayresult = "";
                int factor = 3; // 4 point check 16 points
                float max_distance = radius / 3; //pretty generouse
                for (int j = 0; j <= factor; j++)
                {
                    double theta = (Math.PI * (j +0)) / factor;

                    for (int i = 0; i <= factor; i++)
                    {
                        double phi = (2 * Math.PI * i) / factor;

                        HIL.Vector3 point_sphere = new HIL.Vector3(
                            (float)(Math.Sin(theta) * Math.Cos(phi) * radius),
                            (float)(Math.Sin(theta) * Math.Sin(phi) * radius),
                            (float)(Math.Cos(theta) * radius)) - centre;

                        //log.DebugFormat("magcalib check - {0} {1} dist {2}", theta * rad2deg, phi * rad2deg, max_distance);

                        bool found = false;
                        for (int k = 0; k < datacompass1.Count; k++)
                        {
                            point = new HIL.Vector3(datacompass1[k].Item1, datacompass1[k].Item2, datacompass1[k].Item3);
                            double d = (point_sphere - point).length();
                            if (d < max_distance)
                            {
                                found = true;
                                break;
                            }
                        }
                        // draw them all
                        //((ProgressReporterSphere)sender).sphere1.AimFor(new OpenTK.Vector3((float)point_sphere.x, (float)point_sphere.y, (float)point_sphere.z));
                        if (!found)
                        {
                            displayresult = "more data needed " + (theta * rad2deg).ToString("0") + " " + (phi * rad2deg).ToString("0");
                            ((ProgressReporterSphere)sender).sphere1.AimFor(new OpenTK.Vector3((float)point_sphere.x, (float)point_sphere.y, (float)point_sphere.z));
                            //j = factor;
                            //break;
                        }
                    }
                }
                extramsg = displayresult;
            }

            MainV2.comPort.UnSubscribeToPacketType(sub);
            MainV2.comPort.UnSubscribeToPacketType(sub2);

            if (minx > 0 && maxx > 0 || minx < 0 && maxx < 0 || miny > 0 && maxy > 0 || miny < 0 && maxy < 0 || minz > 0 && maxz > 0 || minz < 0 && maxz < 0)
            {
                e.ErrorMessage = "Bad compass raw values. Check for magnetic interferance.";
                ans = null;
                ans2 = null;
                return;
            }

            // restore old sensor rate
            MainV2.comPort.MAV.cs.ratesensors = backupratesens;
            MainV2.comPort.requestDatastream(MAVLink.MAV_DATA_STREAM.RAW_SENSORS, MainV2.comPort.MAV.cs.ratesensors);

            if (extramsg != "")
            {
                if (CustomMessageBox.Show("You are missing data points. do you want to run the calibration anyway?", "run anyway", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    e.CancelAcknowledged = true;
                    e.CancelRequested = true;
                    ans = null;
                    ans2 = null;
                    return;
                }
            }

            // remove outlyers
            RemoveOutliers(ref datacompass1);
            if (havecompass2 && datacompass2.Count > 0)
            {
                RemoveOutliers(ref datacompass2);
            }

            if (datacompass1.Count < 10)
            {
                e.ErrorMessage = "Log does not contain enough data";
                ans = null;
                ans2 = null;
                return;
            }

            bool ellipsoid = false;

            if (MainV2.comPort.MAV.param.ContainsKey("MAG_DIA"))
            {
                ellipsoid = true;
            }

            log.Info("Compass 1");
            ans = MagCalib.LeastSq(datacompass1, ellipsoid);

            if (havecompass2 && datacompass2.Count > 0)
            {
                log.Info("Compass 2");
                ans2 = MagCalib.LeastSq(datacompass2, ellipsoid);
            }
        }

        static void RemoveOutliers(ref List<Tuple<float, float, float>> data)
        {
            // remove outlyers
            data.Sort(
         delegate(Tuple<float, float, float> d1, Tuple<float, float, float> d2)
         {
             // get distance from 0,0,0
             double ans1 = Math.Sqrt(d1.Item1 * d1.Item1 + d1.Item2 * d1.Item2 + d1.Item3 * d1.Item3);
             double ans2 = Math.Sqrt(d2.Item1 * d2.Item1 + d2.Item2 * d2.Item2 + d2.Item3 * d2.Item3);
             if (ans1 > ans2)
                 return 1;
             if (ans1 < ans2)
                 return -1;
             return 0;
         }
         );

            data.RemoveRange(data.Count - (data.Count / 16), data.Count / 16);
        }

        public static double[] getOffsetsLog(string fn)
        {
            // this is for a dxf
            Polyline3dVertex vertex;
            List<Polyline3dVertex> vertexes = new List<Polyline3dVertex>();

            List<Tuple<float, float, float>> data = new List<Tuple<float, float, float>>();

            List<Tuple<float, float, float>> data2 = new List<Tuple<float, float, float>>();

            var logfile = Log.DFLog.ReadLog(fn);

            foreach (var line in logfile)
            {
                if (line.msgtype == "MAG" || line.msgtype == "MAG2")
                {
                    int indexmagx = Log.DFLog.FindMessageOffset(line.msgtype, "MagX");
                    int indexmagy = Log.DFLog.FindMessageOffset(line.msgtype, "MagY");
                    int indexmagz = Log.DFLog.FindMessageOffset(line.msgtype, "MagZ");

                    int indexoffsetx = Log.DFLog.FindMessageOffset(line.msgtype, "OfsX");
                    int indexoffsety = Log.DFLog.FindMessageOffset(line.msgtype, "OfsY");
                    int indexoffsetz = Log.DFLog.FindMessageOffset(line.msgtype, "OfsZ");

                    if (indexmagx != -1 && indexoffsetx != -1)
                    {
                        float magx = float.Parse(line.items[indexmagx]);
                        float magy = float.Parse(line.items[indexmagy]);
                        float magz = float.Parse(line.items[indexmagz]);

                        float offsetx = float.Parse(line.items[indexoffsetx]);
                        float offsety = float.Parse(line.items[indexoffsety]);
                        float offsetz = float.Parse(line.items[indexoffsetz]);

                        //offsetx = offsety = offsetz = 0;

                        if (line.msgtype == "MAG")
                        {
                            data.Add(new Tuple<float, float, float>(
                               magx - offsetx,
                               magy - offsety,
                               magz - offsetz));

                            // fox dxf
                            vertex = new Polyline3dVertex(new Vector3f(magx - offsetx,
                               magy - offsety,
                               magz - offsetz)
                                );
                            vertexes.Add(vertex);
                        }
                        else if (line.msgtype == "MAG2")
                        {
                            data2.Add(new Tuple<float, float, float>(
                              magx - offsetx,
                              magy - offsety,
                              magz - offsetz));
                        }
                    }
                }
            }

            double[] x = LeastSq(data);

            double[] x2 = LeastSq(data2);

            log.Info("Least Sq Done " + DateTime.Now);

            doDXF(vertexes, x);

            Array.Resize<double>(ref x, 3);

            return x;
        }

        /// <summary>
        /// Processes a tlog to get the offsets - creates dxf of data
        /// </summary>
        /// <param name="fn">Filename</param>
        /// <returns>Offsets</returns>
        public static double[] getOffsets(string fn, int throttleThreshold = 0)
        {
            // based off tridge's work
            string logfile = fn;

            // old method
            float minx = 0;
            float maxx = 0;
            float miny = 0;
            float maxy = 0;
            float minz = 0;
            float maxz = 0;

            // this is for a dxf
            Polyline3dVertex vertex;
            List<Polyline3dVertex> vertexes = new List<Polyline3dVertex>();

            // data storage
            Tuple<float, float, float> offset = new Tuple<float, float, float>(0, 0, 0);
            List<Tuple<float, float, float>> data = new List<Tuple<float, float, float>>();

            Hashtable filter = new Hashtable();

            // track data to use
            bool useData = false;

            if (throttleThreshold <= 0)
                useData = true;

            log.Info("Start log: " + DateTime.Now);

            MAVLinkInterface mine = new MAVLinkInterface();
            try
            {
                mine.logplaybackfile = new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read));
            }
            catch (Exception ex) { log.Debug(ex.ToString()); CustomMessageBox.Show("Log Can not be opened. Are you still connected?"); return new double[] { 0 }; }

            mine.logreadmode = true;

            mine.MAV.packets.Initialize(); // clear

            // gather data
            while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
            {
                byte[] packetraw = mine.readPacket();

                var packet = mine.DebugPacket(packetraw, false);

                // this is for packets we dont know about
                if (packet == null)
                    continue;

                if (packet.GetType() == typeof(MAVLink.mavlink_vfr_hud_t))
                {
                    if (((MAVLink.mavlink_vfr_hud_t)packet).throttle >= throttleThreshold)
                    {
                        useData = true;
                    }
                    else
                    {
                        useData = false;
                    }

                }

                if (packet.GetType() == typeof(MAVLink.mavlink_sensor_offsets_t))
                {
                    offset = new Tuple<float, float, float>(
                        ((MAVLink.mavlink_sensor_offsets_t)packet).mag_ofs_x,
                        ((MAVLink.mavlink_sensor_offsets_t)packet).mag_ofs_y,
                        ((MAVLink.mavlink_sensor_offsets_t)packet).mag_ofs_z);
                }
                else if (packet.GetType() == typeof(MAVLink.mavlink_raw_imu_t) && useData)
                {
                    int div = 20;

                    // fox dxf
                    vertex = new Polyline3dVertex(new Vector3f(
                        ((MAVLink.mavlink_raw_imu_t)packet).xmag - offset.Item1,
                        ((MAVLink.mavlink_raw_imu_t)packet).ymag - offset.Item2,
                        ((MAVLink.mavlink_raw_imu_t)packet).zmag - offset.Item3)
                        );
                    vertexes.Add(vertex);


                    // for old method
                    setMinorMax(((MAVLink.mavlink_raw_imu_t)packet).xmag - offset.Item1, ref minx, ref maxx);
                    setMinorMax(((MAVLink.mavlink_raw_imu_t)packet).ymag - offset.Item2, ref miny, ref maxy);
                    setMinorMax(((MAVLink.mavlink_raw_imu_t)packet).zmag - offset.Item3, ref minz, ref maxz);

                    // for new lease sq
                    string item = (int)(((MAVLink.mavlink_raw_imu_t)packet).xmag / div) + "," +
                        (int)(((MAVLink.mavlink_raw_imu_t)packet).ymag / div) + "," +
                        (int)(((MAVLink.mavlink_raw_imu_t)packet).zmag / div);

                    if (filter.ContainsKey(item))
                    {
                        filter[item] = (int)filter[item] + 1;

                        if ((int)filter[item] > 3)
                            continue;
                    }
                    else
                    {
                        filter[item] = 1;
                    }


                    data.Add(new Tuple<float, float, float>(
                        ((MAVLink.mavlink_raw_imu_t)packet).xmag - offset.Item1,
                        ((MAVLink.mavlink_raw_imu_t)packet).ymag - offset.Item2,
                        ((MAVLink.mavlink_raw_imu_t)packet).zmag - offset.Item3));

                }

            }

            log.Info("Log Processed " + DateTime.Now);

            Console.WriteLine("Extracted " + data.Count + " data points");
            Console.WriteLine("Current offset: " + offset);

            mine.logreadmode = false;
            mine.logplaybackfile.Close();
            mine.logplaybackfile = null;

            if (data.Count < 10)
            {
                CustomMessageBox.Show("Log does not contain enough data");
                throw new Exception("Not Enough Data");
            }

            data.Sort(
                delegate(Tuple<float, float, float> d1, Tuple<float, float, float> d2)
                {
                    // get distance from 0,0,0
                    double ans1 = Math.Sqrt(d1.Item1 * d1.Item1 + d1.Item2 * d1.Item2 + d1.Item3 * d1.Item3);
                    double ans2 = Math.Sqrt(d2.Item1 * d2.Item1 + d2.Item2 * d2.Item2 + d2.Item3 * d2.Item3);
                    if (ans1 > ans2)
                        return 1;
                    if (ans1 < ans2)
                        return -1;
                    return 0;
                }
                );

            data.RemoveRange(data.Count - (data.Count / 16), data.Count / 16);

            System.Console.WriteLine("Old Method {0} {1} {2}", -(maxx + minx) / 2, -(maxy + miny) / 2, -(maxz + minz) / 2);

            double[] x = LeastSq(data);

            log.Info("Least Sq Done " + DateTime.Now);

            doDXF(vertexes, x);

            Array.Resize<double>(ref x, 3);

            return x;
        }

        static void doDXF(List<Polyline3dVertex> vertexes, double[] x)
        {
            // create a dxf for those who want to "see" the calibration
            DxfDocument dxf = new DxfDocument();

            Polyline3d polyline = new Polyline3d(vertexes, true);
            polyline.Layer = new Layer("polyline3d");
            polyline.Layer.Color.Index = 24;
            dxf.AddEntity(polyline);

            var pnt = new Point(new Vector3f(-(float)x[0], -(float)x[1], -(float)x[2]));
            pnt.Layer = new Layer("new offset");
            pnt.Layer.Color.Index = 21;
            dxf.AddEntity(pnt);

            dxf.Save("magoffset.dxf", DxfVersion.AutoCad2000);

            log.Info("dxf Done " + DateTime.Now);
        }

        static double avg_samples = 0;

        /// <summary>
        /// Does the least sq adjustment to find the center of the sphere
        /// </summary>
        /// <param name="data">list of x,y,z data</param>
        /// <returns>offsets</returns>
        public static double[] LeastSq(List<Tuple<float, float, float>> data, bool ellipsoid = false)
        {
            avg_samples = 0;
            foreach (var item in data)
            {
                avg_samples += Math.Sqrt(Math.Pow(item.Item1, 2) + Math.Pow(item.Item2, 2) + Math.Pow(item.Item3, 2));
            }

            avg_samples /= data.Count;

            log.Info("lsq avg " + avg_samples + " count " + data.Count);

            double[] x;

            //
            x = new double[] { 0, 0, 0, 0 };

            x = doLSQ(data, sphere_error, x);

            rad = x[3];

            log.Info("lsq rad " + rad);

            if (ellipsoid)
            {
                // offsets + diagonals
                x = new double[] { x[0], x[1], x[2], 1, 1, 1 };

                x = doLSQ(data, sphere_ellipsoid_error, x);

                // offsets + diagonals + offdiagonals
                x = new double[] { x[0], x[1], x[2], x[3], x[4], x[5], 0, 0, 0 };

                x = doLSQ(data, sphere_ellipsoid_error, x);
            }

            return x;
        }

        static double[] doLSQ(List<Tuple<float, float, float>> data, Action<double[], double[], object> fitalgo, double[] x)
        {
            double epsg = 0.00000001;
            double epsf = 0;
            double epsx = 0;
            int maxits = 0;

            alglib.minlmstate state;
            alglib.minlmreport rep;

            alglib.minlmcreatev(data.Count, x, 100, out state);
            alglib.minlmsetcond(state, epsg, epsf, epsx, maxits);

            var t1 = new alglib.ndimensional_fvec(fitalgo);

            alglib.minlmoptimize(state, t1, null, data);

            alglib.minlmresults(state, out x, out rep);

            log.InfoFormat("passes {0}", rep.iterationscount);
            log.InfoFormat("term type {0}", rep.terminationtype);
            log.InfoFormat("ans {0}", alglib.ap.format(x, 4));

            return x;
        }

        /// <summary>
        /// saves the offests to eeprom, os displays if cant
        /// </summary>
        /// <param name="ofs">offsets</param>
        public static void SaveOffsets(double[] ofs)
        {
            if (MainV2.comPort.MAV.param.ContainsKey("COMPASS_OFS_X") && MainV2.comPort.BaseStream.IsOpen)
            {
                try
                {
                    // disable learning
                    MainV2.comPort.setParam("COMPASS_LEARN", 0);

                    if (!MainV2.comPort.SetSensorOffsets(MAVLinkInterface.sensoroffsetsenum.magnetometer, (float)ofs[0], (float)ofs[1], (float)ofs[2]))
                    {
                        // set values
                        MainV2.comPort.setParam("COMPASS_OFS_X", (float)ofs[0]);
                        MainV2.comPort.setParam("COMPASS_OFS_Y", (float)ofs[1]);
                        MainV2.comPort.setParam("COMPASS_OFS_Z", (float)ofs[2]);
                    }

                    if (ofs.Length > 3)
                    {
                        // ellipsoid
                    }
                }
                catch
                {
                    CustomMessageBox.Show("Set Compass offset failed");
                    return;
                }

                CustomMessageBox.Show("New offsets are " + ofs[0].ToString("0") + " " + ofs[1].ToString("0") + " " + ofs[2].ToString("0") + "\nThese have been saved for you.", "New Mag Offsets");
            }
            else
            {
                CustomMessageBox.Show("New offsets are " + ofs[0].ToString("0") + " " + ofs[1].ToString("0") + " " + ofs[2].ToString("0") + "\n\nPlease write these down for manual entry", "New Mag Offsets");
            }
        }

        public static void SaveOffsets2(double[] ofs)
        {
            if (MainV2.comPort.MAV.param.ContainsKey("COMPASS_OFS2_X") && MainV2.comPort.BaseStream.IsOpen)
            {
                try
                {
                    // disable learning
                    MainV2.comPort.setParam("COMPASS_LEARN", 0);

                    if (!MainV2.comPort.SetSensorOffsets(MAVLinkInterface.sensoroffsetsenum.second_magnetometer, (float)ofs[0], (float)ofs[1], (float)ofs[2]))
                    {
                        // set values
                        MainV2.comPort.setParam("COMPASS_OFS2_X", (float)ofs[0]);
                        MainV2.comPort.setParam("COMPASS_OFS2_Y", (float)ofs[1]);
                        MainV2.comPort.setParam("COMPASS_OFS2_Z", (float)ofs[2]);
                    }
                    if (ofs.Length > 3)
                    {
                        // ellipsoid
                    }
                }
                catch
                {
                    CustomMessageBox.Show("Set Compass2 offset failed");
                    return;
                }

                CustomMessageBox.Show("New compass2 offsets are " + ofs[0].ToString("0") + " " + ofs[1].ToString("0") + " " + ofs[2].ToString("0") + "\nThese have been saved for you.", "New Mag Offsets");
            }
            else
            {
                CustomMessageBox.Show("New compass2 offsets are " + ofs[0].ToString("0") + " " + ofs[1].ToString("0") + " " + ofs[2].ToString("0") + "\n\nPlease write these down for manual entry", "New Mag Offsets");
            }
        }
        /// <summary>
        /// Min or max finder
        /// </summary>
        /// <param name="value">value to process</param>
        /// <param name="min">current min</param>
        /// <param name="max">current max</param>
        private static void setMinorMax(float value, ref float min, ref float max)
        {
            if (value > max)
                max = value;
            if (value < min)
                min = value;
        }

        static double rad = 0;

        static void sphere_ellipsoid_error(double[] p1, double[] fi, object obj)
        {
            var offsets = new Vector3(p1[0], p1[1], p1[2]);
            var diagonals = new Vector3(1.0, 1.0, 1.0);
            var offdiagonals = new Vector3(0.0, 0.0, 0.0);
            if (p1.Length >= 6)
                diagonals = new Vector3(p1[3], p1[4], p1[5]);
            if (p1.Length >= 8)
                offdiagonals = new Vector3(p1[6], p1[7], p1[8]);

            diagonals.x = 1.0;

            int a = 0;
            foreach (var d in (List<Tuple<float, float, float>>)obj)
            {
                var mag = new Vector3(d.Item1, d.Item2, d.Item3);
                double err = rad - radius(mag, offsets, diagonals, offdiagonals);
                fi[a] = err;
                a++;
            }
        }

        static double radius(Vector3 mag, Vector3 offsets, Vector3 diagonals, Vector3 offdiagonals)
        {
            //'''return radius give data point and offsets'''
            Vector3 mag2 = mag + offsets;
            var rot = new Matrix3(new Vector3(diagonals.x, offdiagonals.x, offdiagonals.y),
                           new Vector3(offdiagonals.x, diagonals.y, offdiagonals.z),
                           new Vector3(offdiagonals.y, offdiagonals.z, diagonals.z));
            mag2 = rot * mag2;
            return mag2.length();
        }


        static void sphere_error(double[] xi, double[] fi, object obj)
        {
            double xofs = xi[0];
            double yofs = xi[1];
            double zofs = xi[2];
            double r = xi[3];
            int a = 0;
            foreach (var d in (List<Tuple<float, float, float>>)obj)
            {
                double x = d.Item1;
                double y = d.Item2;
                double z = d.Item3;
                double err = r - Math.Sqrt(Math.Pow((x + xofs), 2) + Math.Pow((y + yofs), 2) + Math.Pow((z + zofs), 2));
                fi[a] = err;
                a++;
            }
        }

        static void sphere_scale_error(double[] xi, double[] fi, object obj)
        {
            double xofs = xi[0];
            double yofs = xi[1];
            double zofs = xi[2];
            double xscale = xi[3];
            double yscale = xi[4];
            double zscale = xi[5];
            // double avg_samples = xi[6];

            // scale out of range
            //if (xscale < 0.8 || yscale < 0.8 || zscale < 0.8)
            //  xscale = yscale = zscale = 1;

            int a = 0;
            foreach (var d in (List<Tuple<float, float, float>>)obj)
            {
                double x = d.Item1;
                double y = d.Item2;
                double z = d.Item3;

                double err = avg_samples - Math.Sqrt(Math.Pow((x + xofs) * xscale, 2) + Math.Pow((y + yofs) * yscale, 2) + Math.Pow((z + zofs) * zscale, 2));
                fi[a] = err;
                a++;
            }
        }
    }
}