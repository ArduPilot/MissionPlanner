using IronPython.Hosting;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace MissionPlanner.Utilities
{
    public class LogAnalyzer
    {
        private static readonly ILog log =
            LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string CheckLogFile(string FileName)
        {
            var engine = Python.CreateEngine();
            var scope = engine.CreateScope();

            var all = System.Reflection.Assembly.GetExecutingAssembly();
            engine.Runtime.LoadAssembly(all);

            engine.CreateScriptSourceFromString("print 'hello world from python'").Execute(scope);

            List<string> paths = new List<string>(engine.GetSearchPaths());

            paths.Add(Environment.CurrentDirectory);

            paths.Add(
                Path.GetDirectoryName(Application.StartupPath + Path.DirectorySeparatorChar + "LogAnalyzer" +
                                      Path.DirectorySeparatorChar + "LogAnalyzer.py"));
            paths.Add(Application.StartupPath + Path.DirectorySeparatorChar + "lib" + Path.DirectorySeparatorChar +
                      "site-packages");

            engine.SetSearchPaths(paths);

            //  engine.CreateScriptSourceFromFile(@"C:\Users\hog\Desktop\DIYDrones\loganalysiscommon\Tools\LogAnalyzer\LogAnalyzer.py");


            string bootloader = @"
import sys
import clr
clr.AddReference('mtrand.dll')



import numpy
#import scipy

print numpy.__version__
#print scipy.__version__

import mtrand

import numpy

import LogAnalyzer

import sys

sys.argv.append('-x')
sys.argv.append('" + FileName.Replace('\\', '/') + @".xml')
sys.argv.append('-s')
sys.argv.append('" + FileName.Replace('\\', '/') + @"')

print sys.argv


LogAnalyzer.main()

";
            try
            {
                var memstream = new MemoryStream();

                engine.Runtime.IO.SetOutput(memstream, UnicodeEncoding.ASCII);

                engine.CreateScriptSourceFromString(bootloader).Execute(scope);

                stringresult = Encoding.ASCII.GetString(memstream.GetBuffer());
            }
            catch (Exception ex)
            {
                log.Error(ex);
                CustomMessageBox.Show(ex.Message, Strings.ERROR);
                return "";
            }

            engine = null;

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