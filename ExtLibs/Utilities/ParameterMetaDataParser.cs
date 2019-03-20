using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using MissionPlanner.Utilities;
using log4net;
using System.Xml.Linq;

namespace MissionPlanner.Utilities
{
    public static class ParameterMetaDataParser
    {
        private static readonly Regex _paramMetaRegex =
            new Regex(ParameterMetaDataConstants.ParamDelimeter +
                      @"(?<MetaKey>\w+)(?:{(?<MetaFrame>[^:]+)})?:(?<MetaValue>.+)");

        private static readonly Regex _parentDirectoryRegex = new Regex("(?<ParentDirectory>[../]*)(?<Path>.+)");

        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static Dictionary<string, string> cachequeue = new Dictionary<string, string>();
        static Dictionary<string, string> cache = new Dictionary<string, string>();

        private static Dictionary<string, string> truename_map = new Dictionary<string, string>()
        {
            {"ArduCopter2", "Copter"},
            {"ArduRover", "Rover"},

            {"APMrover2", "Rover"},
            {"ArduSub", "Sub"},
            {"ArduCopter", "Copter"},
            {"ArduPlane", "Plane"},
            {"AntennaTracker", "Tracker"},
        };

        /// <summary>
        /// retrived parameter info from the net
        /// </summary>
        public static void GetParameterInformation(string urls, string file = null)
        {
            var parameterLocationsString = urls;

            string XMLFileName =
                String.Format("{0}{1}", Settings.GetUserDataDirectory(), "ParameterMetaData.xml");

            if (file != null)
                XMLFileName = String.Format("{0}{1}", Settings.GetUserDataDirectory(), file);

            if (!String.IsNullOrEmpty(parameterLocationsString))
            {
                var parameterLocations = parameterLocationsString.Split(';').ToList();
                parameterLocations.RemoveAll(String.IsNullOrEmpty);

                // precache all the base urls
                Parallel.ForEach(parameterLocations,
                    parameterLocation =>
                    {
                        // load the base urls
                        var dataFromAddress = ReadDataFromAddress(parameterLocation.Trim());
                        // load the group urls
                        ParseGroupInformation(dataFromAddress, null, parameterLocation.Trim(), string.Empty,
                            string.Empty);
                    });

                using (var objXmlTextWriter = new XmlTextWriter(XMLFileName, null))
                {
                    objXmlTextWriter.Formatting = Formatting.Indented;
                    objXmlTextWriter.WriteStartDocument();

                    objXmlTextWriter.WriteStartElement("Params");

                    foreach (string parameterLocation in parameterLocations)
                    {
                        string element = "none";

                        if (parameterLocation.ToLower().Contains("arducopter"))
                        {
                            element = "ArduCopter2";
                        }
                        else if (parameterLocation.ToLower().Contains("arduplane"))
                        {
                            element = "ArduPlane";
                        }
                        else if (parameterLocation.ToLower().Contains("rover"))
                        {
                            element = "ArduRover";
                        }
                        else if (parameterLocation.ToLower().Contains("ardusub"))
                        {
                            element = "ArduSub";
                        }
                        else if (parameterLocation.ToLower().Contains("tracker"))
                        {
                            element = "ArduTracker";
                        }

                        // Read and parse the content.
                        string dataFromAddress = ReadDataFromAddress(parameterLocation.Trim());

                        if (String.IsNullOrEmpty(dataFromAddress)) // 404
                            continue;

                        if (dataFromAddress.Length < 200) // blank template file
                            continue;

                        // Write the start element for this parameter location
                        objXmlTextWriter.WriteStartElement(element);
                        ParseParameterInformation(dataFromAddress, objXmlTextWriter, string.Empty, string.Empty, element);
                        ParseGroupInformation(dataFromAddress, objXmlTextWriter, parameterLocation.Trim(), string.Empty, element);

                        // Write the end element for this parameter location
                        objXmlTextWriter.WriteEndElement();
                    }

                    objXmlTextWriter.WriteEndElement();

                    // Clear the stream
                    objXmlTextWriter.WriteEndDocument();
                    objXmlTextWriter.Flush();
                    objXmlTextWriter.Close();
                }

                XElement root = XElement.Load(XMLFileName);
                foreach (var vech in root.Elements())
                {
                    var paramlist = vech.Elements().OrderBy(xt => xt.Name.ToString()).ToArray();

                    vech.RemoveAll();

                    foreach (var item in paramlist)
                        vech.Add(item);
                }
                root.Save(XMLFileName);
            }
        }

        /// <summary>
        /// Parses the group parameter information.
        /// </summary>
        /// <param name="fileContents">The file contents.</param>
        /// <param name="objXmlTextWriter">The obj XML text writer.</param>
        /// <param name="parameterLocation">The parameter location.</param>
        private static void ParseGroupInformation(string fileContents, XmlTextWriter objXmlTextWriter,
            string parameterLocation, string parameterPrefix, string vehicleType)
        {
            var NestedGroups = Regex.Match(fileContents, ParameterMetaDataConstants.NestedGroup);

            if (NestedGroups != null && NestedGroups.Success)
            {
                Uri uri = new Uri(parameterLocation);

                var currentfn = uri.Segments[uri.Segments.Length - 1];

                var newfn = NestedGroups.Groups[1].ToString() + Path.GetExtension(currentfn);

                if (currentfn != newfn)
                {
                    var newPath = parameterLocation.Replace(currentfn, newfn);
                    var dataFromAddress = ReadDataFromAddress(newPath);
                    log.Info("Nested Group " + NestedGroups.Groups[1]);
                    ParseParameterInformation(dataFromAddress, objXmlTextWriter, parameterPrefix, string.Empty, vehicleType);
                    ParseGroupInformation(dataFromAddress, objXmlTextWriter, newPath, parameterPrefix, vehicleType);
                }
            }

            var parsedInformation = ParseKeyValuePairs(fileContents, ParameterMetaDataConstants.Group, vehicleType);
            if (parsedInformation != null && parsedInformation.Count > 0)
            {
                // node is the prefix of the parameter group here
                Parallel.ForEach(parsedInformation, node =>
                {
                    // node.Value is a nested dictionary containing the additional meta data
                    // In this case we are looking for the @Path key
                    if (node.Value != null && node.Value.Count > 0)
                    {
                        // Find the @Path key
                        node.Value
                            .Where(meta => meta.Key == ParameterMetaDataConstants.Path)
                            // We might have multiple paths to inspect, so break them out by the delimeter
                            .ForEach(
                                path =>
                                    path.Value.Split(new[] {ParameterMetaDataConstants.PathDelimeter},
                                        StringSplitOptions.None)
                                        .ForEach(separatedPath =>
                                        {
                                            log.Info("Process " + parameterPrefix + node.Key + " : " + separatedPath);
                                            Uri newUri = new Uri(new Uri(parameterLocation), separatedPath.Trim());

                                            var newPath = newUri.AbsoluteUri;

                                            if (newPath == parameterLocation)
                                                return;

                                            var dataFromAddress = ReadDataFromAddress(newPath);

                                            if (dataFromAddress == "")
                                                return;

                                            // Parse the param info from the newly constructed URL
                                            ParseParameterInformation(dataFromAddress,
                                                objXmlTextWriter, parameterPrefix+node.Key, newPath, vehicleType);

                                            ParseGroupInformation(dataFromAddress, objXmlTextWriter, newPath, parameterPrefix + node.Key, vehicleType);
                                        }));
                    }
                });
            }
        }

        /// <summary>
        /// Parses the parameter information.
        /// </summary>
        /// <param name="fileContents">The file contents.</param>
        /// <param name="objXmlTextWriter">The obj XML text writer.</param>
        /// <param name="parameterPrefix">The parameter prefix.</param>
        private static void ParseParameterInformation(string fileContents, XmlTextWriter objXmlTextWriter,
            string parameterPrefix, string url, string vehicleType)
        {
            if (objXmlTextWriter == null)
                return;

            var parsedInformation = ParseKeyValuePairs(fileContents, ParameterMetaDataConstants.Param, vehicleType);
            if (parsedInformation != null && parsedInformation.Count > 0)
            {
                parsedInformation.ForEach(node =>
                {
                    parameterPrefix = parameterPrefix.Replace('(', '_');
                    parameterPrefix = parameterPrefix.Replace(')', '_');
                    lock (objXmlTextWriter)
                    {
                        objXmlTextWriter.WriteStartElement(String.Format("{0}{1}", parameterPrefix.Replace(" ", "_"),
                            node.Key.Replace(" ", "_")));
                        if (node.Value != null && node.Value.Count > 0)
                        {
                            node.Value.ForEach(meta =>
                            {
                                // Write the key value pair to XML
                                objXmlTextWriter.WriteStartElement(meta.Key);
                                objXmlTextWriter.WriteString(meta.Value);
                                objXmlTextWriter.WriteEndElement();
                            });
                        }
                        objXmlTextWriter.WriteEndElement();
                    }
                });
            }
        }

        /// <summary>
        /// Parses the parameter information.
        /// </summary>
        /// <param name="fileContents">The file contents.</param>
        /// <param name="nodeKey">The node key.</param>
        /// <returns></returns>
        private static Dictionary<string, Dictionary<string, string>> ParseKeyValuePairs(string fileContents,
            string nodeKey, string vehicleType)
        {
            var returnDict = new Dictionary<string, Dictionary<string, string>>();

            var indicies = new List<int>();
            GetIndexOfMarkers(ref indicies, fileContents, ParameterMetaDataConstants.ParamDelimeter + nodeKey, 0);

            if (indicies.Count > 0)
            {
                // Loop through the indicies of the parameter comments found
                for (int i = 0; i < indicies.Count; i++)
                {
                    // This is the end index for a substring to search for parameter attributes
                    // If we are on the last index in our collection, we will search to the end of the file
                    var stopIdx = (i == indicies.Count - 1) ? fileContents.Length : indicies[i + 1] + 1;

                    string subStringToSearch = fileContents.Substring(indicies[i], (stopIdx - indicies[i]));
                    if (!String.IsNullOrEmpty(subStringToSearch))
                    {
                        var metaIndicies = new List<int>();
                        GetIndexOfMarkers(ref metaIndicies, subStringToSearch, ParameterMetaDataConstants.ParamDelimeter,
                            0);

                        if (metaIndicies.Count > 0)
                        {
                            // This meta param key
                            var paramNameKey = subStringToSearch.Substring(metaIndicies[0],
                                (metaIndicies[1] - metaIndicies[0]));

                            // Match based on the regex defined at the top of this class
                            Match paramNameKeyMatch = _paramMetaRegex.Match(paramNameKey);

                            if (paramNameKeyMatch.Success && paramNameKeyMatch.Groups["MetaKey"].Value == nodeKey)
                            {
                                string key = paramNameKeyMatch.Groups["MetaValue"].Value.Trim(new char[] {' '});
                                var metaDict = new Dictionary<string, string>();
                                if (!returnDict.ContainsKey(key))
                                {
                                    // Loop through the indicies of the meta data found
                                    for (int x = 1; x < metaIndicies.Count; x++)
                                    {
                                        // This is the end index for a substring to search for parameter attributes
                                        // If we are on the last index in our collection, we will search to the end of the file
                                        var stopMetaIdx = (x == metaIndicies.Count - 1)
                                            ? subStringToSearch.Length
                                            : metaIndicies[x + 1] + 1;

                                        // This meta param string
                                        var metaString = subStringToSearch.Substring(metaIndicies[x],
                                            (stopMetaIdx - metaIndicies[x]));

                                        // Match based on the regex defined at the top of this class
                                        Match metaMatch = _paramMetaRegex.Match(metaString);

                                        // Test for success
                                        if (metaMatch.Success)
                                        {
                                            string metaKey = metaMatch.Groups["MetaKey"].Value.Trim(new char[] {' '});
                                            if (!metaDict.ContainsKey(metaKey))
                                            {
                                                metaDict.Add(metaKey,
                                                    metaMatch.Groups["MetaValue"].Value.Trim(new char[] {' '}));
                                            }
                                            else if (metaMatch?.Groups["MetaFrame"]?.Value != "")
                                            {
                                                var metaframe = metaMatch?.Groups["MetaFrame"]?.Value.ToString();
                                                try
                                                {
                                                    var mapname = truename_map.First(a =>
                                                        a.Value.ToLower() == metaframe.ToLower());

                                                    if (mapname.Key.ToLower() == vehicleType.ToLower())
                                                    {
                                                        metaDict[metaKey] = metaMatch.Groups["MetaValue"].Value
                                                            .Trim(new char[] {' '});
                                                    }
                                                } catch { log.Error("Invalid MetaFrame " + metaframe); }
                                            }
                                        }
                                    }
                                }
                                if (!returnDict.ContainsKey(key))
                                {
                                    returnDict.Add(key, metaDict);
                                }
                                else
                                {
                                    log.Error("Duplicate Key " + key);
                                }
                            }
                        }
                    }
                }
            }
            return returnDict;
        }

        /// <summary>
        /// Gets the index of param markers.
        /// </summary>
        /// <param name="indicies">The indicies.</param>
        /// <param name="inspectThis">The string to be inspected for a parameter.</param>
        /// <param name="delimeter">The delimeter.</param>
        /// <param name="prevIdx">The prev idx.</param>
        private static void GetIndexOfMarkers(ref List<int> indicies, string inspectThis, string delimeter, int prevIdx)
        {
            // Find the index of the start of a parameter comment
            int idx = inspectThis.IndexOf(delimeter, prevIdx, StringComparison.InvariantCultureIgnoreCase);

            // If we can't find one we stop here
            if (idx != -1)
            {
                // Add the index we found
                indicies.Add(idx);

                // Move the index after the parameter delimeter
                int newIdx = idx + delimeter.Length;

                // If we have more string to inspect
                if (newIdx < inspectThis.Length)
                {
                    // Recursively search for the next index
                    GetIndexOfMarkers(ref indicies, inspectThis, delimeter, newIdx);
                }
            }
        }

        /// <summary>
        /// Reads the data from address.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <returns></returns>
        private static string ReadDataFromAddress(string address, int attempt = 0)
        {
            // check attempt no
            if (attempt > 2)
            {
                log.Error(String.Format("Failed {0}", address));
                lock (cachequeue)
                {
                    while (cachequeue.Remove(address)) ;
                }
                return String.Empty;
            }

            string data = string.Empty;

            while (true)
            {
                lock (cachequeue)
                {
                    if (!cachequeue.Keys.Contains(address))
                    {
                        cachequeue[address] = "";
                        break;
                    }
                }

                Console.WriteLine("ReadDataFromAddress attempt {1} Queued {0}", address, attempt);
                Thread.Sleep(500);
            }

            // check cache - after delay above for other loader
            if (cache.ContainsKey(address))
            {
                log.Info("using cache " + address);
                lock (cachequeue)
                {
                    while (cachequeue.Remove(address)) ;
                }
                return cache[address];
            }

            // Make sure we don't blow up if the user is not connected or the endpoint is not available
            try
            {
                var request = WebRequest.Create(address);

                // Plenty of timeout
                request.Timeout = 10000;

                // Set the Method property of the request to GET.
                request.Method = "GET";

                // Get the response.
                using (var response = request.GetResponse())
                {
                    // Display the status.
                    log.Info(((HttpWebResponse) response).StatusDescription);

                    // Get the stream containing content returned by the server.
                    using (var dataStream = response.GetResponseStream())
                    {
                        if (dataStream != null)
                        {
                            // Open the stream using a StreamReader for easy access.
                            using (var reader = new StreamReader(dataStream))
                            {
                                // Store the data to return
                                data = reader.ReadToEnd();
                            }
                        }
                    }
                }

                cache[address] = data;

                // Return the data
                return data;
            }
            catch (WebException ex)
            {
                log.Error(String.Format("The request to {0} failed.", address), ex);

                attempt++;

                lock (cachequeue)
                {
                    while (cachequeue.Remove(address)) ;
                }

                return ReadDataFromAddress(address, attempt);
            }
            finally
            {
                lock (cachequeue)
                {
                    while (cachequeue.Remove(address)) ;
                }
            }
        }
    }
}