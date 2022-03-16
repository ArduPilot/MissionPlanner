using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Flurl.Http;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using MissionPlanner.Utilities;
using Newtonsoft.Json;

namespace ParameterMetaDataGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            
            log4net.Repository.Hierarchy.Hierarchy hierarchy =
                (Hierarchy)log4net.LogManager.GetRepository(Assembly.GetAssembly(typeof(Program)));

            PatternLayout patternLayout = new PatternLayout();
            patternLayout.ConversionPattern = "%date [%thread] %-5level %logger - %message%newline";
            patternLayout.ActivateOptions();

            var cca = new ConsoleAppender();
            cca.Layout = patternLayout;
            cca.ActivateOptions();
            hierarchy.Root.AddAppender(cca);

            hierarchy.Root.Level = Level.Debug;
            hierarchy.Configured = true;

            if (false)
            {
                var resp = "https://api.github.com/repos/ardupilot/ardupilot/git/refs/tags"
                    .WithHeader("User-Agent",
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36")
                    .AllowAnyHttpStatus()
                    .GetAsync().Result;

                var tags = resp.GetJsonListAsync().Result;

                foreach(var tag in tags)
                {
                    var tagdict = (IDictionary<String, Object>)tag;

                    var refpath = tagdict["ref"].ToString();

                    if (refpath.Contains("ArduPlane") || refpath.Contains("Copter") ||
                        refpath.Contains("Rover") || refpath.Contains("APMrover2") ||
                        refpath.Contains("ArduSub") || refpath.Contains("AntennaTracker"))
                    {
                        var taginfo = (IDictionary<String, Object>) ((string) tagdict["url"])
                            .WithHeader("User-Agent",
                                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.198 Safari/537.36")
                            .GetJsonAsync().Result;
                        var sha = ((IDictionary<String, Object>) taginfo["object"])["sha"].ToString();
                        var refname = ((string) taginfo["ref"]).Replace("refs/tags/", "");
                        var paramfile = "";
                        if (refname.Contains("Copter"))
                        {
                            paramfile = "ArduCopter/Parameters.pde;ArduCopter/Parameters.cpp";
                        }    
                        if (refname.Contains("ArduPlane"))
                        {
                            paramfile = "ArduPlane/Parameters.pde;ArduPlane/Parameters.cpp";
                        }   
                        if (refname.Contains("ArduSub"))
                        {
                            paramfile = "ArduSub/Parameters.pde;ArduSub/Parameters.cpp";
                        } 
                        if (refname.Contains("Rover"))
                        {
                            paramfile = "Rover/Parameters.pde;Rover/Parameters.cpp";
                        }                       
                        if (refname.Contains("APMrover2"))
                        {
                            paramfile = "APMrover2/Parameters.pde;APMrover2/Parameters.cpp";
                        }  
                        if (refname.Contains("AntennaTracker"))
                        {
                            paramfile = "AntennaTracker/Parameters.pde;AntennaTracker/Parameters.cpp";
                        }

                        if (paramfile == "")
                            continue;

                        var XMLFileName = String.Format("{0}{1}", Settings.GetUserDataDirectory(), refname + ".xml");

                        if (File.Exists(XMLFileName))
                            continue;

                        ParameterMetaDataParser.GetParameterInformation(
                            paramfile.Split(';')
                                .Select(a =>
                                {
                                    a = a.Trim();
                                    return "https://raw.github.com/ardupilot/ardupilot/" + sha + "/" + a + ";";
                                }).Aggregate("", (a, b) => a + b)
                            , refname + ".xml");
                    }
                }
            }

            if (args.Length == 1)
            {
                Console.WriteLine("needs ot be started inside the git repo");

                var proc = new System.Diagnostics.Process {StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName="git",
                    Arguments ="tag",
                    UseShellExecute = false,
                    RedirectStandardOutput = true, 
                    CreateNoWindow = true
                }};

                proc.Start();
                var resp = proc.StandardOutput.ReadToEnd();

                var tags = resp.Split('\n');

                foreach(var tag in tags)
                {
                    var refpath = tag;

                    if (refpath.Contains("ArduPlane") || refpath.Contains("Copter") ||
                        refpath.Contains("Rover") || refpath.Contains("APMrover2") ||
                        refpath.Contains("ArduSub") || refpath.Contains("AntennaTracker"))
                    {
                        if (!Regex.IsMatch(refpath,@"\.[0-9]+$"))
                        {
                            continue;
                        }

                        var refname = tag;
                        var paramfile = "";
                        if (refname.Contains("Copter"))
                        {
                            paramfile = "ArduCopter/Parameters.pde;ArduCopter/Parameters.cpp";
                        }    
                        if (refname.Contains("ArduPlane"))
                        {
                            paramfile = "ArduPlane/Parameters.pde;ArduPlane/Parameters.cpp";
                        }   
                        if (refname.Contains("ArduSub"))
                        {
                            paramfile = "ArduSub/Parameters.pde;ArduSub/Parameters.cpp";
                        } 
                        if (refname.Contains("Rover") || refname.Contains("APMrover2"))
                        {
                            paramfile = "Rover/Parameters.pde;Rover/Parameters.cpp;APMrover2/Parameters.pde;APMrover2/Parameters.cpp";
                        }  
                        if (refname.Contains("AntennaTracker"))
                        {
                            paramfile = "AntennaTracker/Parameters.pde;AntennaTracker/Parameters.cpp";
                        }

                        if (paramfile == "")
                            continue;

                        var XMLFileName = String.Format("{0}{1}", Settings.GetUserDataDirectory(),
                            refname + ".xml");

                        if (File.Exists(XMLFileName))
                            continue;

                        var proc2 = new System.Diagnostics.Process()
                        {
                            StartInfo = new System.Diagnostics.ProcessStartInfo("git", "reset --hard " + tag)
                                {CreateNoWindow = true,  UseShellExecute = false}
                        };

                        proc2.Start();

                        proc2.WaitForExit();

                        ParameterMetaDataParser.GetParameterInformation(
                            paramfile.Split(';')
                                .Select(a =>
                                {
                                    a = a.Trim();
                                    return Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + a + ";";
                                }).Aggregate("", (a, b) => a + b)
                            , refname + ".xml");
                    }
                }
                return;
            }


            if (args.Length == 0)
            {
                Console.WriteLine(
                    "Usage: ParameterMetaDataGenerator.exe \"ArduCopter\\Parameters.cpp;ArduPlane\\Parameters.cpp;APMrover2\\Parameters.cpp\" output.xml");
                return;
            }


            ParameterMetaDataParser.GetParameterInformation(args[0], args[1]);
        }
    }
}
