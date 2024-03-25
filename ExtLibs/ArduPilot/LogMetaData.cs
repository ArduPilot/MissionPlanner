using ExifLibrary;
using log4net;
using MissionPlanner.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using SharpCompress.Archives;
using SharpCompress.Compressors.Xz;
using SharpCompress.Readers;

namespace MissionPlanner.ArduPilot
{
    public class LogMetaData
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        static string[] vehicles = new[] { "Copter", "Plane", "Rover", "Tracker" };

        static string url = "https://autotest.ardupilot.org/LogMessages/{0}/LogMessages.xml.xz";

        public struct LogItemFeild
        {
            public string description;
            public struct LogItemFeildBitmask
            {
                public string name;
                public uint mask;
                public string description;
            }
            public List<LogItemFeildBitmask> bitmask;
        };

        public static  Dictionary<string, Dictionary<string, LogItemFeild>> MetaData { get; } = new Dictionary<string, Dictionary<string, LogItemFeild>>();

        public static async Task GetMetaData()
        {
            List<Task> tlist = new List<Task>();

            vehicles.ForEach(a =>
            {
                try
                {
                    var newurl = String.Format(url, a);
                    var file = Path.Combine(Settings.GetDataDirectory(), "LogMessages" + a + ".xml.xz");
                    if(File.Exists(file))
                        if (new FileInfo(file).LastWriteTime.AddDays(7) > DateTime.Now)
                            return;
                    var dltask = Download.getFilefromNetAsync(newurl, file);
                    tlist.Add(dltask);
                }
                catch (Exception ex) { log.Error(ex); }
            });

            await Task.WhenAll(tlist);

            vehicles.ForEach(a =>
            {
                try
                {
                    var fileout = Path.Combine(Settings.GetDataDirectory(), "LogMessages" + a + ".xml");
                    var file = Path.Combine(Settings.GetDataDirectory(), "LogMessages" + a + ".xml.xz");
                    if (File.Exists(file))
                        using (var read = File.OpenRead(file))
                        {
                            if (XZStream.IsXZStream(read))
                            {
                                read.Position = 0;
                                var stream = new XZStream(read);
                                using (var outst = File.OpenWrite(fileout))
                                {
                                    try
                                    {
                                        outst.SetLength(0);
                                        stream.CopyTo(outst);
                                    }
                                    catch (XZIndexMarkerReachedException)
                                    {
                                        // ignore
                                    }
                                }
                            }
                        }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            });
        }

        public static void ParseMetaData()
        {
            vehicles.ForEach(a =>
            {
                var file = Path.Combine(Settings.GetDataDirectory(), "LogMessages" + a + ".xml");

                if (!File.Exists(file))
                {
                    return;
                }

                try
                {
                    var xml = XDocument.Load(file);

                    xml.Root.Descendants("logformat").ForEach<XElement>(b =>
                    {
                        if (b == null)
                            return;

                        var type = b.Attribute("name");
                        var typedesc = b.Descendants("description").FirstOrDefault();

                        if (!MetaData.ContainsKey(type.Value))
                            MetaData[type.Value] = new Dictionary<string, LogItemFeild>();

                        LogItemFeild log_type = new LogItemFeild();
                        log_type.description = typedesc.Value;
                        MetaData[type.Value]["description"] = log_type;

                        b.Descendants("fields").Descendants("field").ForEach(c =>
                        {
                            var name = c.Attribute("name");
                            var desc = c.Descendants("description").FirstOrDefault();
                            var bits = c.Descendants("bitmask").FirstOrDefault();

                            LogItemFeild log_feild = new LogItemFeild();
                            log_feild.description = desc.Value;

                            if (bits != null)
                            {
                                if (log_feild.bitmask == null)
                                {
                                    log_feild.bitmask = new List<LogItemFeild.LogItemFeildBitmask>();
                                }

                                bits.Descendants("bit").ForEach(d =>
                                {
                                    LogItemFeild.LogItemFeildBitmask log_mask = new LogItemFeild.LogItemFeildBitmask();
                                    log_mask.name = (string)d.Attribute("name");
                                    log_mask.mask = (uint)d.Descendants("value").FirstOrDefault();
                                    log_mask.description = (string)d.Descendants("description").FirstOrDefault();
                                    log_feild.bitmask.Add(log_mask);
                                });
                            }
                            MetaData[type.Value][name.Value] = log_feild;
                        });
                    });
                }   
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            });
        }
    }
}
