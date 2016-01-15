using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MissionPlanner;
using csmatio.io;
using csmatio.types;
using System.Globalization;
using log4net;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MissionPlanner.Log
{
    public class MatLab
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void ProcessTLog()
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "*.tlog|*.tlog";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                try
                {
                    openFileDialog1.InitialDirectory = MainV2.LogDir + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (string logfile in openFileDialog1.FileNames)
                    {
                        MissionPlanner.Log.MatLab.tlog(logfile);
                    }
                }
            }
        }

        public static void ProcessLog()
        {
            using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
            {
                openFileDialog1.Filter = "Log Files|*.log;*.bin";
                openFileDialog1.FilterIndex = 2;
                openFileDialog1.RestoreDirectory = true;
                openFileDialog1.Multiselect = true;
                try
                {
                    openFileDialog1.InitialDirectory = MainV2.LogDir + Path.DirectorySeparatorChar;
                }
                catch
                {
                } // incase dir doesnt exist

                if (openFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    foreach (string logfile in openFileDialog1.FileNames)
                    {
                        MissionPlanner.Log.MatLab.ProcessLog(logfile);
                    }
                }
            }
        }

        private static MLArray CreateCellArray(string name, string[] names)
        {
            MLCell cell = new MLCell(name, new int[] {names.Length, 1});
            for (int i = 0; i < names.Length; i++)
                cell[i] = new MLChar(null, names[i]);
            return cell;
        }

        public static void ProcessLog(string fn)
        {
            StreamReader sr;

            if (fn.ToLower().EndsWith(".bin"))
            {
                string tmpfile = Path.GetTempFileName();
                BinaryLog.ConvertBin(fn, tmpfile);
                sr = new StreamReader(tmpfile);
            }
            else
            {
                sr = new StreamReader(fn);
            }

            // store all the arrays
            List<MLArray> mlList = new List<MLArray>();
            // store data to putinto the arrays
            Dictionary<string, DoubleList> data = new Dictionary<string, DoubleList>();
            // store line item lengths
            Hashtable len = new Hashtable();
            // store whats we have seen in the log
            Hashtable seen = new Hashtable();
            // store the params seen
            SortedDictionary<string, double> param = new SortedDictionary<string, double>();

            // keep track of line no
            int a = 0;

            log.Info("ProcessLog start " + (GC.GetTotalMemory(false)/1024.0/1024.0));

            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine();

                a++;
                if (a%100 == 0)
                    Console.Write(a + "\r");

                string strLine = line.Replace(", ", ",");
                strLine = strLine.Replace(": ", ":");

                string[] items = strLine.Split(',', ':');

                // process the fmt messages
                if (line.StartsWith("FMT"))
                {
                    // +1 for line no
                    string[] names = new string[items.Length - 5 + 1];
                    names[0] = "LineNo";
                    Array.ConstrainedCopy(items, 5, names, 1, names.Length - 1);

                    MLArray format = CreateCellArray(items[3] + "_label", names);

                    if (items[3] == "PARM")
                    {
                    }
                    else
                    {
                        mlList.Add(format);
                    }

                    len[items[3]] = names.Length;
                } // process param messages
                else if (line.StartsWith("PARM"))
                {
                    try
                    {
                        param[items[2]] = double.Parse(items[3], CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                    }
                } // everyting else is generic
                else
                {
                    // make sure the line is long enough
                    if (items.Length < 2)
                        continue;
                    // check we have a valid fmt message for this message type
                    if (!len.ContainsKey(items[0]))
                        continue;
                    // check the fmt length matchs what the log has
                    if (items.Length != (int) len[items[0]])
                        continue;

                    // make it as being seen
                    seen[items[0]] = 1;

                    double[] dbarray = new double[items.Length];

                    // set line no
                    dbarray[0] = a;

                    for (int n = 1; n < items.Length; n++)
                    {
                        double dbl = 0;

                        double.TryParse(items[n], NumberStyles.Any, CultureInfo.InvariantCulture, out dbl);

                        dbarray[n] = dbl;
                    }

                    if (!data.ContainsKey(items[0]))
                        data[items[0]] = new DoubleList();

                    data[items[0]].Add(dbarray);
                }

                // split at x records
                if (a%2000000 == 0)
                {
                    GC.Collect();
                    DoWrite(fn + "-" + a, data, param, mlList, seen);
                    mlList.Clear();
                    data.Clear();
                    param.Clear();
                    seen.Clear();
                    GC.Collect();
                }
            }

            DoWrite(fn + "-" + a, data, param, mlList, seen);

            sr.Close();
        }

        static void DoWrite(string fn, Dictionary<string, DoubleList> data, SortedDictionary<string, double> param,
            List<MLArray> mlList, Hashtable seen)
        {
            log.Info("DoWrite start " + (GC.GetTotalMemory(false)/1024.0/1024.0));

            foreach (var item in data)
            {
                double[][] temp = item.Value.ToArray();
                MLArray dbarray = new MLDouble(item.Key, temp);
                mlList.Add(dbarray);
                log.Info("DoWrite " + item.Key + " " + (GC.GetTotalMemory(false)/1024.0/1024.0));
            }

            log.Info("DoWrite mllist " + (GC.GetTotalMemory(false)/1024.0/1024.0));

            MLCell cell = new MLCell("PARM", new int[] {param.Keys.Count, 2});
            int m = 0;
            foreach (var item in param.Keys)
            {
                cell[m, 0] = new MLChar(null, item.ToString());
                cell[m, 1] = new MLDouble(null, new double[] {(double) param[item]}, 1);
                m++;
            }

            mlList.Add(cell);

            MLArray seenmsg = CreateCellArray("Seen", seen.Keys.Cast<string>().ToArray());

            mlList.Add(seenmsg);

            try
            {
                log.Info("write " + fn + ".mat");
                log.Info("DoWrite before" + (GC.GetTotalMemory(false)/1024.0/1024.0));
                MatFileWriter mfw = new MatFileWriter(fn + ".mat", mlList, true);
                log.Info("DoWrite done" + (GC.GetTotalMemory(false)/1024.0/1024.0));
            }
            catch (Exception err)
            {
                CustomMessageBox.Show("There was an error when creating the MAT-file: \n" + err.ToString(),
                    "MAT-File Creation Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        public static void tlog(string logfile)
        {
            List<MLArray> mlList = new List<MLArray>();
            Hashtable datappl = new Hashtable();

            using (MAVLinkInterface mine = new MAVLinkInterface())
            {
                try
                {
                    mine.logplaybackfile =
                        new BinaryReader(File.Open(logfile, FileMode.Open, FileAccess.Read, FileShare.Read));
                }
                catch
                {
                    CustomMessageBox.Show("Log Can not be opened. Are you still connected?");
                    return;
                }
                mine.logreadmode = true;

                mine.MAV.packets.Initialize(); // clear               

                while (mine.logplaybackfile.BaseStream.Position < mine.logplaybackfile.BaseStream.Length)
                {
                    byte[] packet = mine.readPacket();
                    object data = mine.GetPacket(packet);

                    if (data == null)
                        continue;

                    if (data is MAVLink.mavlink_heartbeat_t)
                    {
                        if (((MAVLink.mavlink_heartbeat_t) data).type == (byte) MAVLink.MAV_TYPE.GCS)
                            continue;
                    }

                    Type test = data.GetType();

                    DateTime time = mine.lastlogread;

                    double matlabtime = GetMatLabSerialDate(time);

                    try
                    {
                        foreach (var field in test.GetFields())
                        {
                            // field.Name has the field's name.

                            object fieldValue = field.GetValue(data); // Get value
                            if (field.FieldType.IsArray)
                            {
                            }
                            else
                            {
                                if (!datappl.ContainsKey(field.Name + "_" + field.DeclaringType.Name))
                                {
                                    datappl[field.Name + "_" + field.DeclaringType.Name] = new List<double[]>();
                                }

                                List<double[]> list =
                                    ((List<double[]>) datappl[field.Name + "_" + field.DeclaringType.Name]);

                                object value = fieldValue;

                                if (value.GetType() == typeof (Single))
                                {
                                    list.Add(new double[] {matlabtime, (double) (Single) field.GetValue(data)});
                                }
                                else if (value.GetType() == typeof (short))
                                {
                                    list.Add(new double[] {matlabtime, (double) (short) field.GetValue(data)});
                                }
                                else if (value.GetType() == typeof (ushort))
                                {
                                    list.Add(new double[] {matlabtime, (double) (ushort) field.GetValue(data)});
                                }
                                else if (value.GetType() == typeof (byte))
                                {
                                    list.Add(new double[] {matlabtime, (double) (byte) field.GetValue(data)});
                                }
                                else if (value.GetType() == typeof (sbyte))
                                {
                                    list.Add(new double[] {matlabtime, (double) (sbyte) field.GetValue(data)});
                                }
                                else if (value.GetType() == typeof (Int32))
                                {
                                    list.Add(new double[] {matlabtime, (double) (Int32) field.GetValue(data)});
                                }
                                else if (value.GetType() == typeof (UInt32))
                                {
                                    list.Add(new double[] {matlabtime, (double) (UInt32) field.GetValue(data)});
                                }
                                else if (value.GetType() == typeof (ulong))
                                {
                                    list.Add(new double[] {matlabtime, (double) (ulong) field.GetValue(data)});
                                }
                                else if (value.GetType() == typeof (long))
                                {
                                    list.Add(new double[] {matlabtime, (double) (long) field.GetValue(data)});
                                }
                                else if (value.GetType() == typeof (double))
                                {
                                    list.Add(new double[] {matlabtime, (double) (double) field.GetValue(data)});
                                }
                                else
                                {
                                    Console.WriteLine("Unknown data type");
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                mine.logreadmode = false;
                mine.logplaybackfile.Close();
                mine.logplaybackfile = null;
            }

            foreach (string item in datappl.Keys)
            {
                double[][] temp = ((List<double[]>) datappl[item]).ToArray();
                MLArray dbarray = new MLDouble(item.Replace(" ", "_"), temp);
                mlList.Add(dbarray);
            }

            try
            {
                MatFileWriter mfw = new MatFileWriter(logfile + ".mat", mlList, true);
            }
            catch (Exception err)
            {
                MessageBox.Show("There was an error when creating the MAT-file: \n" + err.ToString(),
                    "MAT-File Creation Error!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }
        }

        /// <summary>
        /// http://www.mathworks.com.au/help/matlab/matlab_prog/represent-date-and-times-in-MATLAB.html#bth57t1-1
        /// MATLAB also uses serial time to represent fractions of days beginning at midnight; for example, 6 p.m. equals 0.75 serial days.
        /// So the string '31-Oct-2003, 6:00 PM' in MATLAB is date number 731885.75.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static double GetMatLabSerialDate(DateTime dt)
        {
            // in c# i cant represent year 0000, so we add one year and the leap year
            DateTime timebase = new DateTime(1, 1, 1); // = 1

            double answer = (dt.AddYears(1).AddDays(1) - timebase).TotalDays;

            return answer;
        }

        public class DoubleList : IDisposable
        {
            Stream file;

            string filename;

            Stream offsetfile;

            string offsetfilename;

            const int offsetsize = sizeof (long);

            public int Count
            {
                get { return (int) (offsetfile.Length/offsetsize); }
            }

            public DoubleList()
            {
                filename = Path.GetTempFileName();
                file = File.Open(filename, FileMode.Create);

                offsetfilename = Path.GetTempFileName();
                offsetfile = File.Open(offsetfilename, FileMode.Create);
            }

            void setoffset(int index, long offset)
            {
                byte[] data = BitConverter.GetBytes(offset);
                offsetfile.Seek(offsetsize*index, SeekOrigin.Begin);
                offsetfile.Write(data, 0, offsetsize);
            }

            long getoffset(int index)
            {
                byte[] data = new byte[offsetsize];
                offsetfile.Seek(offsetsize*index, SeekOrigin.Begin);
                offsetfile.Read(data, 0, offsetsize);
                return BitConverter.ToInt64(data, 0);
            }

            ~DoubleList()
            {
                Dispose();
            }

            public void Dispose()
            {
                offsetfile.Close();
                offsetfile = null;

                file.Close();
                file = null;

                File.Delete(filename);
                File.Delete(offsetfilename);
            }

            public double[] this[int index]
            {
                get
                {
                    // init a buffer
                    byte[] buffer = new byte[sizeof (double)];
                    // seek to the offset of this index we want
                    file.Seek(getoffset(index), SeekOrigin.Begin);
                    // read the number of elements following
                    file.Read(buffer, 0, sizeof (int));
                    int elements = BitConverter.ToInt32(buffer, 0);

                    // read the elements
                    List<double> data = new List<double>();
                    for (int a = 0; a < elements; a++)
                    {
                        file.Read(buffer, 0, buffer.Length);
                        data.Add(BitConverter.ToDouble(buffer, 0));
                    }
                    // return the data
                    return data.ToArray();
                }
            }

            public int Add(double[] items)
            {
                // goto the end of the file
                file.Seek(0, SeekOrigin.End);
                // save the position of the data following
                setoffset(Count, file.Position);
                // save the number of elements following
                file.Write(BitConverter.GetBytes(items.Length), 0, sizeof (int));
                // save the elements
                foreach (var item in items)
                {
                    file.Write(BitConverter.GetBytes(item), 0, sizeof (double));
                }

                // return the index
                return Count;
            }

            public double[][] ToArray()
            {
                double[][] data = new double[Count][];

                for (int a = 0; a < Count; a++)
                {
                    data[a] = this[a];
                }

                return data;
            }
        }
    }
}