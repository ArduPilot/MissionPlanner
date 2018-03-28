using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MissionPlanner.Utilities;

namespace MissionPlanner.Utilities
{
    public class DashWare
    {
        public static void Create(string filein, string fileout, List<string> fmtList = null)
        {
            using (StreamReader tr = new StreamReader(filein))
            using (CollectionBuffer logdata = new CollectionBuffer(tr.BaseStream))
            {
                List<string> colList = new List<string>();
                Dictionary<string, int> colStart = new Dictionary<string, int>();

                colStart.Add("GLOBAL", colList.Count);
                colList.Add("GLOBAL_TimeMS");

                foreach (var logformatValue in logdata.dflog.logformat.Values)
                {
                    if(fmtList != null && !fmtList.Contains(logformatValue.Name))
                        continue;
                    colStart.Add(logformatValue.Name, colList.Count);
                    foreach (var field in logformatValue.FieldNames)
                    {
                        colList.Add(logformatValue.Name + "_" + field);
                    }
                }

                using (StreamWriter sr = new StreamWriter(fileout))
                {
                    // header
                    foreach (var item in colList)
                    {
                        sr.Write(item + ",");
                    }
                    sr.WriteLine();

                    // lines
                    foreach (var dfitem in logdata.GetEnumeratorType(colStart.Keys.ToArray()))
                    {
                        if (dfitem.msgtype == "FMT")
                            continue;

                        var idx = 0;
                        StringBuilder sb = new StringBuilder();

                        sb.Append(dfitem.timems);
                        idx++;
                        sb.Append(',');

                        var start = colStart[dfitem.msgtype];

                        while (idx < start)
                        {
                            idx++;
                            sb.Append(',');
                        }

                        foreach (var item in dfitem.items.Skip(1))
                        {
                            sb.Append(item?.Trim());
                            idx++;
                            sb.Append(',');
                        }

                        sr.WriteLine(sb);
                    }
                }
            }
        }
    }
}
