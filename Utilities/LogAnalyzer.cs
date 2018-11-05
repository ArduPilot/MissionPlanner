using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using ICSharpCode.SharpZipLib.Zip;
using MissionPlanner.Controls;

namespace MissionPlanner.Utilities
{
    public class LogAnalyzer
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string CheckLogFile(string FileName)
        {
            if (Program.WindowsStoreApp)
            {
                CustomMessageBox.Show(Strings.Not_available_when_used_as_a_windows_store_app);
                return "";
            }

            var dir = Settings.GetDataDirectory() + "LogAnalyzer" +
                      Path.DirectorySeparatorChar;

            var runner = dir + "runner.exe";

            var zip = dir + "LogAnalyzer.zip";

            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            //if (!File.Exists(runner))
            {
                Loading.ShowLoading("Downloading LogAnalyzer");
                bool gotit = false;
                if (Environment.Is64BitOperatingSystem)
                {
                    gotit = Download.getFilefromNet(
                        "http://firmware.ardupilot.org/Tools/MissionPlanner/LogAnalyzer/LogAnalyzer64.zip",
                        zip);
                }
                else
                {
                    gotit = Download.getFilefromNet(
                        "http://firmware.ardupilot.org/Tools/MissionPlanner/LogAnalyzer/LogAnalyzer.zip",
                        zip);
                }

                // download zip
                if (gotit)
                {
                    Loading.ShowLoading("Extracting zip file");
                    // extract zip
                    FastZip fzip = new FastZip();
                    fzip.ExtractZip(zip, dir, "");
                }
                else
                {
                    if (!File.Exists(runner))
                    {
                        CustomMessageBox.Show("Failed to download LogAnalyzer");
                        return "";
                    }
                }

            }

            if (!File.Exists(runner))
            {
                CustomMessageBox.Show("Failed to download LogAnalyzer");
                return "";
            }

            var sb = new StringBuilder();

            Process P = new Process();
            P.StartInfo.FileName = runner;
            P.StartInfo.Arguments = @" -x """ + FileName + @".xml"" -s """ + FileName + @"""";

            P.StartInfo.UseShellExecute = false;
            P.StartInfo.WorkingDirectory = dir;

            P.StartInfo.RedirectStandardOutput = true;
            P.StartInfo.RedirectStandardError = true;

            P.OutputDataReceived += (sender, args) => sb.AppendLine(args.Data);
            P.ErrorDataReceived += (sender, args) => sb.AppendLine(args.Data);

            try
            {
                Loading.ShowLoading("Running LogAnalyzer");

                P.Start();

                P.BeginOutputReadLine();
                P.BeginErrorReadLine();

                // until we are done
                P.WaitForExit();

                log.Info(sb.ToString());
            }
            catch
            {
                CustomMessageBox.Show("Failed to start LogAnalyzer");
            }

            Loading.Close();

            return FileName + ".xml";
        }

        public static analysis Results(string xmlfile)
        {
            analysis answer = new analysis();

            using (XmlReader reader = XmlReader.Create(xmlfile))
            {
                while (!reader.EOF)
                {
                    if (reader.ReadToFollowing("header"))
                    {
                        var subtree = reader.ReadSubtree();

                        while (subtree.Read())
                        {
                            subtree.MoveToElement();
                            if (subtree.IsStartElement())
                            {
                                try
                                {
                                    switch (subtree.Name.ToLower())
                                    {
                                        case "logfile":
                                            answer.logfile = subtree.ReadString();
                                            break;
                                        case "sizekb":
                                            answer.sizekb = subtree.ReadString();
                                            break;
                                        case "sizelines":
                                            answer.sizelines = subtree.ReadString();
                                            break;
                                        case "duration":
                                            answer.duration = subtree.ReadString();
                                            break;
                                        case "vehicletype":
                                            answer.vehicletype = subtree.ReadString();
                                            break;
                                        case "firmwareversion":
                                            answer.firmwareversion = subtree.ReadString();
                                            break;
                                        case "firmwarehash":
                                            answer.firmwarehash = subtree.ReadString();
                                            break;
                                        case "hardwaretype":
                                            answer.hardwaretype = subtree.ReadString();
                                            break;
                                        case "freemem":
                                            answer.freemem = subtree.ReadString();
                                            break;
                                        case "skippedlines":
                                            answer.skippedlines = subtree.ReadString();
                                            break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.Error(ex);
                                }
                            }
                        }
                    }
                    // params - later
                    if (reader.ReadToFollowing("results"))
                    {
                        var subtree = reader.ReadSubtree();

                        result res = null;

                        while (subtree.Read())
                        {
                            subtree.MoveToElement();
                            if (subtree.IsStartElement())
                            {
                                switch (subtree.Name.ToLower())
                                {
                                    case "result":
                                        if (res != null && res.name != "")
                                            answer.results.Add(res);
                                        res = new result();
                                        break;
                                    case "name":
                                        res.name = subtree.ReadString();
                                        break;
                                    case "status":
                                        res.status = subtree.ReadString();
                                        break;
                                    case "message":
                                        res.message = subtree.ReadString();
                                        break;
                                    case "data":
                                        res.data = subtree.ReadString();
                                        break;
                                }
                            }
                        }
                    }
                }
            }

            return answer;
        }

        public class analysis
        {
            public string logfile;
            public string sizekb;
            public string sizelines;
            public string duration;
            public string vehicletype;
            public string firmwareversion;
            public string firmwarehash;
            public string hardwaretype;
            public string freemem;
            public string skippedlines;

            public List<result> results = new List<result>();
        }

        public class result
        {
            public string name;
            public string status;
            public string message;
            public string data;
        }

        public static string stringresult { get; set; }
    }
}
