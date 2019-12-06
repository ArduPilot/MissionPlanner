using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using csmatio.io;
using csmatio.types;
using System.Globalization;
using log4net;
using System.Reflection;
using MissionPlanner.Utilities;
using MissionPlanner.Comms;

namespace MissionPlanner.Log
{
    public class MatLab
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static MLArray CreateCellArray(string name, string[] names)
        {
            MLCell cell = new MLCell(name, new int[] {names.Length, 1});
            for (int i = 0; i < names.Length; i++)
                cell[i] = new MLChar(null, names[i].Trim());
            return cell;
        }

        private static MLCell CreateCellArrayCustom(string name, string[] items)
        {
            var cell = new MLCell(name, new int[] { items.Length, 1 });
            int i = 0;
            foreach (var item in items)
            {
                double ans = 0;
                if (double.TryParse(items[i], out ans))
                {
                    cell[i] = new MLDouble(null, new double[] { ans }, 1);
                    i++;
                    continue;
                }

                if (items[i].TrimStart().StartsWith("[") && items[i].TrimEnd().EndsWith("]"))
                {
                    cell[i] = CreateCellArrayCustom("",
                        items[i].Split(new[] { ' ', '[', ']' }, StringSplitOptions.RemoveEmptyEntries));
                    i++;
                    continue;
                }

                cell[i] = new MLChar(null, (string)items[i].Trim());

                i++;
            }
            return cell;
        }

        public static void ProcessLog(string fn, Action<string> ProgressEvent = null)
        {
            using (CollectionBuffer colbuf = new CollectionBuffer(File.OpenRead(fn)))
            {
                // store all the arrays
                List<MLArray> mlList = new List<MLArray>();
                // store data to putinto the arrays
                Dictionary<string, MatLab.DoubleList> data = new Dictionary<string, MatLab.DoubleList>();

                Dictionary<string, List<MLCell>> dataCell = new Dictionary<string, List<MLCell>>();
                // store line item lengths
                Hashtable len = new Hashtable();
                // store whats we have seen in the log
                Hashtable seen = new Hashtable();
                // store the params seen
                SortedDictionary<string, double> param = new SortedDictionary<string, double>();

                // keep track of line no
                int a = 0;

                log.Info("ProcessLog start " + (GC.GetTotalMemory(false)/1024.0/1024.0));

                foreach (var line in colbuf)
                {
                    a++;
                    if (a%1000 == 0)
                    {
                        Console.Write(a + "/" + colbuf.Count + "\r");
                        ProgressEvent?.Invoke("Processing "+a + "/" + colbuf.Count);
                    }

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

                        MLArray format = CreateCellArray(items[3].Trim() + "_label", names);

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
                            param[items[colbuf.dflog.FindMessageOffset("PARM", "Name")]] = double.Parse(items[colbuf.dflog.FindMessageOffset("PARM", "Value")], CultureInfo.InvariantCulture);
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

                        // mark it as being seen
                        seen[items[0]] = 1;
                        if (items[0].ToLower().Equals("msg"))
                        {
                            var cells = CreateCellArrayCustom(items[0], items);

                            if (!dataCell.ContainsKey(items[0]))
                                dataCell[items[0]] = new List<MLCell>();

                            dataCell[items[0]].Add(cells);
                        }

                        if (items[0].ToUpper().Equals("ISBD"))
                        {
                            //ISBD
                            var cells = CreateCellArrayCustom(items[0], items);

                            if (!dataCell.ContainsKey(items[0]))
                                dataCell[items[0]] = new List<MLCell>();

                            dataCell[items[0]].Add(cells);
                        }

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
                            data[items[0]] = new MatLab.DoubleList();

                        data[items[0]].Add(dbarray);
                    }

                    // split at x records
                    if (a%2000000 == 0 && !Environment.Is64BitProcess)
                    {
                        GC.Collect();
                        DoWrite(fn + "-" + a, data, dataCell, param, mlList, seen);
                        mlList.Clear();
                        data.Clear();
                        dataCell.Clear();
                        param.Clear();
                        seen.Clear();
                        GC.Collect();
                    }
                }

                DoWrite(fn + "-" + a, data, dataCell, param, mlList, seen);
            }
        }

        static void DoWrite(string fn, Dictionary<string, MatLab.DoubleList> data, Dictionary<string, List<MLCell>> dataCell, SortedDictionary<string, double> param,
            List<MLArray> mlList, Hashtable seen)
        {
            log.Info("DoWrite start " + (GC.GetTotalMemory(false)/1024.0/1024.0));

            foreach (var item in data)
            {
                double[][] temp = item.Value.ToArray();
                MLArray dbarray = new MLDouble(item.Key, temp);
                mlList.Add(dbarray);
                log.Info("DoWrite Double " + item.Key + " " + (GC.GetTotalMemory(false)/1024.0/1024.0));
            }

            // datacell contains rows
            foreach (var item in dataCell)
            {
                // create msg table
                MLCell temp1 = new MLCell(item.Key+"1", new int[] {1, item.Value.Count});
                int a = 0;
                // add rows to msg table
                foreach (var mlCell in item.Value)
                {
                    temp1[a] = item.Value[a];
                    a++;
                }
                // add table to masterlist
                mlList.Add(temp1);
                log.Info("DoWrite Cell " + item.Key + " " + (GC.GetTotalMemory(false) / 1024.0 / 1024.0));            
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
                MatFileWriter mfw = new MatFileWriter(fn + ".mat", mlList, false);
                log.Info("DoWrite done" + (GC.GetTotalMemory(false)/1024.0/1024.0));
            }
            catch (Exception err)
            {
                throw new Exception("There was an error when creating the MAT-file: \n" + err.ToString(), err);
            }
        }

        public static void tlog(string logfile)
        {
            List<MLArray> mlList = new List<MLArray>();
            Hashtable datappl = new Hashtable();

            using (Comms.CommsFile cf = new CommsFile(logfile))
            using (CommsStream cs = new CommsStream(cf, cf.BytesToRead))
            {
                MAVLink.MavlinkParse parse = new MAVLink.MavlinkParse(true);

                while (cs.Position < cs.Length)
                {
                    MAVLink.MAVLinkMessage packet = parse.ReadPacket(cs);

                    if(packet == null)
                        continue;

                    object data = packet.data;

                    if (data == null)
                        continue;

                    if (data is MAVLink.mavlink_heartbeat_t)
                    {
                        if (((MAVLink.mavlink_heartbeat_t)data).type == (byte)MAVLink.MAV_TYPE.GCS)
                            continue;
                    }

                    Type test = data.GetType();

                    DateTime time = packet.rxtime;

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
                                    ((List<double[]>)datappl[field.Name + "_" + field.DeclaringType.Name]);

                                object value = fieldValue;

                                if (value.GetType() == typeof(Single))
                                {
                                    list.Add(new double[] { matlabtime, (double)(Single)field.GetValue(data) });
                                }
                                else if (value.GetType() == typeof(short))
                                {
                                    list.Add(new double[] { matlabtime, (double)(short)field.GetValue(data) });
                                }
                                else if (value.GetType() == typeof(ushort))
                                {
                                    list.Add(new double[] { matlabtime, (double)(ushort)field.GetValue(data) });
                                }
                                else if (value.GetType() == typeof(byte))
                                {
                                    list.Add(new double[] { matlabtime, (double)(byte)field.GetValue(data) });
                                }
                                else if (value.GetType() == typeof(sbyte))
                                {
                                    list.Add(new double[] { matlabtime, (double)(sbyte)field.GetValue(data) });
                                }
                                else if (value.GetType() == typeof(Int32))
                                {
                                    list.Add(new double[] { matlabtime, (double)(Int32)field.GetValue(data) });
                                }
                                else if (value.GetType() == typeof(UInt32))
                                {
                                    list.Add(new double[] { matlabtime, (double)(UInt32)field.GetValue(data) });
                                }
                                else if (value.GetType() == typeof(ulong))
                                {
                                    list.Add(new double[] { matlabtime, (double)(ulong)field.GetValue(data) });
                                }
                                else if (value.GetType() == typeof(long))
                                {
                                    list.Add(new double[] { matlabtime, (double)(long)field.GetValue(data) });
                                }
                                else if (value.GetType() == typeof(double))
                                {
                                    list.Add(new double[] { matlabtime, (double)(double)field.GetValue(data) });
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
            }

            foreach (string item in datappl.Keys)
            {
                double[][] temp = ((List<double[]>) datappl[item]).ToArray();
                MLArray dbarray = new MLDouble(item.Replace(" ", "_"), temp);
                mlList.Add(dbarray);
            }

            try
            {
                MatFileWriter mfw = new MatFileWriter(logfile + ".mat", mlList, false);
            }
            catch (Exception err)
            {
                throw new Exception("There was an error when creating the MAT-file: \n" + err.ToString(), err);
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
            DateTime timebase = DateTime.MinValue; // = 1

            double answer = (dt.AddYears(1).AddDays(2) - timebase).TotalDays;

            return answer;
        }

        /// <summary>
        /// File backed list
        /// One file for data (double)
        /// One file for offsets (long)
        /// </summary>
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