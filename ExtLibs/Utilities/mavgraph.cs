using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using log4net;

namespace MissionPlanner.Utilities
{
    public class mavgraph
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static bool readmavgraphsxml_runonce = false;

        static mavgraph()
        {
            readmavgraphsxml();

            graphs.Sort((a, b) => a.Name.ToUpper().CompareTo(b.Name.ToUpper()));
        }

        public class graphitem
        {
            public string name;
            public List<string> expressions = new List<string>();
            public string description;
        }

        public class displayitem
        {
            public string type;
            public string field;
            public string expression;
            public bool left = true;
        }

        public class displaylist
        {
            public string Name;
            public displayitem[] items;

            public override string ToString()
            {
                return Name;
            }
        }

        public static List<displaylist> graphs = new List<displaylist>()
        {
            new displaylist() {Name = "a/None"},
            new displaylist()
            {
                Name = "Builtin/Mechanical Failure",
                items = new displayitem[]
                {
                    new displayitem() {type = "ATT", field = "Roll"},
                    new displayitem() {type = "ATT", field = "DesRoll"},
                    new displayitem() {type = "ATT", field = "Pitch"},
                    new displayitem() {type = "ATT", field = "DesPitch"},
                    new displayitem() {type = "CTUN", field = "Alt", left = false},
                    new displayitem() {type = "CTUN", field = "DAlt", left = false}
                }
            },
            new displaylist()
            {
                Name = "Builtin/Mechanical Failure - Stab",
                items =
                    new displayitem[]
                    {
                        new displayitem() {type = "ATT", field = "Roll"},
                        new displayitem() {type = "ATT", field = "DesRoll"}
                    }
            },
            new displaylist()
            {
                Name = "Builtin/Mechanical Failure - Auto",
                items =
                    new displayitem[]
                    {
                        new displayitem() {type = "ATT", field = "Roll"},
                        new displayitem() {type = "NTUN", field = "DRoll"}
                    }
            },
            new displaylist()
            {
                Name = "Builtin/Vibrations",
                items =
                    new displayitem[]
                    {
                        new displayitem() {type = "IMU", field = "AccX"},
                        new displayitem() {type = "IMU", field = "AccY"},
                        new displayitem() {type = "IMU", field = "AccZ"}
                    }
            },
            new displaylist()
            {
                Name = "Builtin/Vibrations 3.3",
                items = new displayitem[]
                {
                    new displayitem() {type = "VIBE", field = "VibeX"},
                    new displayitem() {type = "VIBE", field = "VibeY"},
                    new displayitem() {type = "VIBE", field = "VibeZ"},
                    new displayitem() {type = "VIBE", field = "Clip0", left = false},
                    new displayitem() {type = "VIBE", field = "Clip1", left = false},
                    new displayitem() {type = "VIBE", field = "Clip2", left = false}
                }
            },
            new displaylist()
            {
                Name = "Builtin/GPS Glitch",
                items =
                    new displayitem[]
                    {
                        new displayitem() {type = "GPS", field = "HDop"},
                        new displayitem() {type = "GPS", field = "NSats", left = false}
                    }
            },
            new displaylist()
            {
                Name = "Builtin/Power Issues",
                items = new displayitem[]
                {
                    new displayitem() {type = "CURR", field = "Vcc"},
                    new displayitem() {type = "POWR", field = "Vcc"}
                }
            },
            new displaylist()
            {
                Name = "Builtin/Errors",
                items = new displayitem[] {new displayitem() {type = "ERR", field = "ECode"}}
            },
            new displaylist()
            {
                Name = "Builtin/Battery Issues",
                items =
                    new displayitem[]
                    {
                        new displayitem() {type = "CTUN", field = "ThrIn"},
                        new displayitem() {type = "CURR", field = "ThrOut"},
                        new displayitem() {type = "CURR", field = "Volt", left = false}
                    }
            },
            new displaylist()
            {
                Name = "Builtin/imu consistency xyz",
                items = new displayitem[]
                {
                    new displayitem() {type = "IMU", field = "AccX"},
                    new displayitem() {type = "IMU2", field = "AccX"},
                    new displayitem() {type = "IMU", field = "AccY"},
                    new displayitem() {type = "IMU2", field = "AccY"},
                    new displayitem() {type = "IMU", field = "AccZ", left = false},
                    new displayitem() {type = "IMU2", field = "AccZ", left = false},
                }
            },
            new displaylist()
            {
                Name = "Builtin/mag consistency xyz",
                items = new displayitem[]
                {
                    new displayitem() {type = "MAG", field = "MagX"},
                    new displayitem() {type = "MAG2", field = "MagX"},
                    new displayitem() {type = "MAG", field = "MagY", left = false},
                    new displayitem() {type = "MAG2", field = "MagY", left = false},
                    new displayitem() {type = "MAG", field = "MagZ"},
                    new displayitem() {type = "MAG2", field = "MagZ"},
                }
            },
            new displaylist()
            {
                Name = "Builtin/copter loiter",
                items = new displayitem[]
                {
                    new displayitem() {type = "NTUN", field = "DVelX"},
                    new displayitem() {type = "NTUN", field = "VelX"},
                    new displayitem() {type = "NTUN", field = "DVelY"},
                    new displayitem() {type = "NTUN", field = "VelY"},
                }
            },
            new displaylist()
            {
                Name = "Builtin/copter althold",
                items = new displayitem[]
                {
                    new displayitem() {type = "CTUN", field = "BarAlt"},
                    new displayitem() {type = "CTUN", field = "DAlt"},
                    new displayitem() {type = "CTUN", field = "Alt"},
                    new displayitem() {type = "GPS", field = "Alt"},
                }
            },
            new displaylist()
            {
                Name = "Builtin/ekf VEL tune",
                items = new displayitem[]
                {
                    new displayitem() {type = "NKF3", field = "IVN"},
                    new displayitem() {type = "NKF3", field = "IPN"},
                    new displayitem() {type = "NKF3", field = "IVE"},
                    new displayitem() {type = "NKF3", field = "IPE"},
                    new displayitem() {type = "NKF3", field = "IVD"},
                    new displayitem() {type = "NKF3", field = "IPD"},
                }
            },
        };


        public static void readmavgraphsxml()
        {
            if (readmavgraphsxml_runonce)
                return;

            readmavgraphsxml_runonce = true;

            List<graphitem> items = new List<graphitem>();

            log.Info("readmavgraphsxml from " + Settings.GetRunningDirectory() + Path.DirectorySeparatorChar +
                     "graphs");
            var files = Directory.GetFiles(Settings.GetRunningDirectory() + Path.DirectorySeparatorChar + "graphs",
                "*.xml");

            foreach (var file in files)
            {
                try
                {
                    log.Info("readmavgraphsxml file " + file);
                    using (
                        XmlReader reader =
                            XmlReader.Create(file))
                    {
                        while (reader.Read())
                        {
                            if (reader.ReadToFollowing("graph"))
                            {
                                graphitem newGraphitem = new graphitem();

                                for (int a = 0; a < reader.AttributeCount; a++)
                                {
                                    reader.MoveToAttribute(a);
                                    if (reader.Name.ToLower() == "name")
                                    {
                                        newGraphitem.name =
                                            reader.Value + " " + Path.GetFileNameWithoutExtension(file);
                                    }
                                }

                                reader.MoveToElement();

                                XmlReader inner = reader.ReadSubtree();

                                while (inner.Read())
                                {
                                    if (inner.IsStartElement())
                                    {
                                        if (inner.Name.ToLower() == "expression")
                                            newGraphitem.expressions.Add(inner.ReadString().Trim());
                                        else if (inner.Name.ToLower() == "description")
                                            newGraphitem.description = inner.ReadString().Trim();
                                    }
                                }

                                processGraphItem(newGraphitem);

                                items.Add(newGraphitem);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }

        static void processGraphItem(graphitem graphitem)
        {
            List<displayitem> list = new List<displayitem>();

            foreach (var expression in graphitem.expressions)
            {
                var items = expression.Split(new char[] { ' ', '\t', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in items)
                {
                    var matchs = Regex.Matches(item.Trim(), @"^([A-z0-9_]+)\.([A-z0-9_]+)[:2]*$");

                    if (matchs.Count > 0)
                    {
                        foreach (Match match in matchs)
                        {
                            var temp = new displayitem();
                            // right axis
                            if (item.EndsWith(":2"))
                                temp.left = false;

                            temp.type = match.Groups[1].Value.ToString();
                            temp.field = match.Groups[2].Value.ToString();

                            list.Add(temp);
                        }
                    }
                    else
                    {
                        var temp = new displayitem();
                        if (item.EndsWith(":2"))
                            temp.left = false;
                        temp.expression = item;
                        temp.type = item;
                        list.Add(temp);
                    }
                }
            }

            var dispitem = new displaylist()
            {
                Name = graphitem.name,
                items = list.ToArray()
            };

            graphs.Add(dispitem);
        }
    }
}