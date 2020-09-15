using System;
using System.Configuration;
using System.IO;
using System.Xml.Linq;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Compression;
using System.Threading.Tasks;
using log4net;
using SharpCompress.Compressors.Xz;

namespace MissionPlanner.Utilities
{
    public static class ParameterMetaDataRepositoryAPMpdef
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static Dictionary<string,XDocument> _parameterMetaDataXML = new Dictionary<string, XDocument>();

        static string[] vehicles = new[] { "Copter", "Plane", "Rover", "Tracker" };

        static string url = "https://autotest.ardupilot.org/Parameters/{0}/apm.pdef.xml.gz";


        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterMetaDataRepository"/> class.
        /// </summary>
        public static void CheckLoad(string vehicle = "")
        {
            if (_parameterMetaDataXML[vehicle] == null)
                Reload(vehicle);
        }

        public static async Task GetMetaData()
        {
            List<Task> tlist = new List<Task>();

            vehicles.ForEach(a =>
            {
                try
                {
                    var newurl = String.Format(url, a);
                    var file = Path.Combine(Settings.GetDataDirectory(), a + ".apm.pdef.xml.gz");
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
                    var fileout = Path.Combine(Settings.GetDataDirectory(), a + ".apm.pdef.xml");
                    var file = Path.Combine(Settings.GetDataDirectory(), a + ".apm.pdef.xml.gz");
                    if (File.Exists(file))
                        using (var read = File.OpenRead(file))
                        {
                            //if (XZStream.IsXZStream(read))
                            {
                                read.Position = 0;
                                var stream = new GZipStream(read, CompressionMode.Decompress);
                                //var stream = new XZStream(read);
                                using (var outst = File.OpenWrite(fileout))
                                {
                                    stream.CopyTo(outst);
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

        public static void Reload(string vehicle = "")
        {
            string paramMetaDataXMLFileName =
                String.Format("{0}{1}", Settings.GetUserDataDirectory(), vehicle + ".apm.pdef.xml");

            try
            {
                if (File.Exists(paramMetaDataXMLFileName))
                    _parameterMetaDataXML[vehicle] = XDocument.Load(paramMetaDataXMLFileName);

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        /// <summary>
        /// Gets the parameter meta data.
        /// </summary>
        /// <param name="nodeKey">The node key.</param>
        /// <param name="metaKey">The meta key.</param>
        /// <returns></returns>
        public static string GetParameterMetaData(string nodeKey, string metaKey, string vechileType)
        {
            // remap names
            if (vechileType == "ArduCopter2")
                vechileType = "ArduCopter";
            if (vechileType == "ArduRover")
                vechileType = "APMrover2";
            if (vechileType == "ArduTracker")
                vechileType = "AntennaTracker";

            CheckLoad(vechileType);

            // remap keys
            if (metaKey == ParameterMetaDataConstants.DisplayName)
                metaKey = "humanName";
            if (metaKey == ParameterMetaDataConstants.Description)
                metaKey = "documentation";
            if (metaKey == ParameterMetaDataConstants.User)
                metaKey = "user";

            if (_parameterMetaDataXML[vechileType] != null)
            {
                try
                {
                    foreach (var paramfile in _parameterMetaDataXML[vechileType].Element("paramfile").Elements())
                    {
                        foreach (var parameters in paramfile.Elements())
                        {
                            if (parameters.HasAttributes)
                            {
                                foreach (var param in parameters.Elements())
                                {
                                    if (param.Attribute("name").Value == (vechileType + ":" + nodeKey) ||
                                        param.Attribute("name").Value == nodeKey)
                                    {
                                        if (param.Attribute(metaKey) != null)
                                        {
                                            return param.Attribute(metaKey).Value;
                                        }
                                        if (metaKey == ParameterMetaDataConstants.Values)
                                        {
                                            var ans = "";
                                            param.Elements("values").Elements().ForEach(a =>
                                            {
                                                if (a.Name == "value")
                                                {
                                                    var code = a.Attribute("code");
                                                    var value = a.Value.ToString();
                                                    ans += String.Format("{0}:{1},", code.Value, value);
                                                }
                                            });
                                            return ans;
                                        }
                                        foreach (var xElement in param.Elements())
                                        {
                                            if (xElement.Name == "field")
                                            {
                                                var name = xElement.Attribute("name");
                                                if (name != null && name.Value == metaKey)
                                                {
                                                    return xElement.Value;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                } 
            }

            return string.Empty;
        }
    }
}