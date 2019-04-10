using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using MissionPlanner.Utilities;

namespace MissionPlanner.Log.test
{
    public class MotorFailure
    {
        public class Value
        {
            public double min { get; internal set; }
            public double max { get; internal set; }
            public double avg {
                get { return sum/count; } 
            }

            public double absminmax {
                get { return Math.Max(Math.Abs(max), Math.Abs(min)); }
            }

            public double maxdelta
            {
                get { return max - min; }
            }

            public double sum { get; internal set; }
            public double count { get; internal set; }

            public void AddSample(double sample)
            {
                if (count == 0)
                {
                    min = sample;
                    max = sample;
                }
                min = Math.Min(min, sample);
                max = Math.Max(max, sample);

                sum += sample;
                count++;
            }

            public override string ToString()
            {
                return string.Format("min: {0} max: {1} avg: {2} maxdelta {3} samples {4}", min, max, avg, max - min, count);
            }
        }

        public static void Process(string filename)
        {
            using (CollectionBuffer file = new CollectionBuffer(File.OpenRead(filename)))
            {
                List<Value> rc = new List<Value>();
                for (int i = 1; i <= 14; i++)
                {
                    rc.Add(new Value());
                }
                var roll = new Value();
                var pitch = new Value();

                foreach (var DFMSG in file.GetEnumeratorType(new string[] {"ATT", "RCOU"}))
                {
                    if (DFMSG.msgtype == "ATT")
                    {
                        var desroll = Double.Parse(DFMSG["DesRoll"], CultureInfo.InvariantCulture);
                        var actroll = Double.Parse(DFMSG["Roll"], CultureInfo.InvariantCulture);
                        var despitch = Double.Parse(DFMSG["DesPitch"], CultureInfo.InvariantCulture);
                        var actpitch = Double.Parse(DFMSG["Pitch"], CultureInfo.InvariantCulture);

                        var rolldelta = desroll - actroll;
                        var pitchdelta = despitch - actpitch;

                        roll.AddSample(rolldelta);
                        pitch.AddSample(pitchdelta);
                    }
                    else if (DFMSG.msgtype == "RCOU")
                    {
                        for (int i = 1; i <= 14; i++)
                        {
                            foreach (var name in new string[] {"C" + i, "Ch" + i, "Chan" + i})
                            {
                                if (DFMSG.parent.logformat["RCOU"].FieldNames.Contains(name))
                                {
                                    var C = Double.Parse(DFMSG[name], CultureInfo.InvariantCulture);

                                    rc[i - 1].AddSample(C);

                                    break;
                                }
                            }
                        }
                    }
                }

                Console.WriteLine(roll);
                Console.WriteLine(pitch);

                Value avg = new Value();
                for (int i = 1; i <= 14; i++)
                {
                    if (rc[i - 1].min > 800)
                    {
                        avg.AddSample(rc[i - 1].avg);
                    }
                }

                if (avg.maxdelta > 100)
                {
                    Console.Beep();
                    Console.WriteLine("Signs of motor failure");
                }

                for (int i = 1; i <= 14; i++)
                {
                    Console.WriteLine(rc[i-1]);
                }
            }
        }
    }
}
