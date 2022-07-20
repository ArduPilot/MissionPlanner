using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MissionPlanner.Utilities;

namespace NMEA2000
{
    public class ConvertCANLogToYDWG
    {
        public static void runme(string file = @"C:\Users\mich1\OneDrive\2021-10-25 17-16-05.can")
        {
            Regex reg = new Regex("^T(.{8})(.)(.+)(....)$");

            var lines = File.ReadAllLines(file);

            Dictionary<int, List<byte>> msgbuffer = new Dictionary<Int32, List<Byte>>();

            foreach (var line in lines)
            {
                var match = reg.Match(line);
                if (match.Success)
                {
                    // ts prio src dst  pgn
                    var cf = new CANFrameNMEA((match.Groups[1].Value.HexStringToByteArray().Reverse().ToArray()));

                    var type = NMEA2000msgs.msgs.Where(a => a.Item1 == cf.PDU);
                    if (type.Count() > 0)
                    {
                        if (type.First().Item3 == PgnType.Single)
                        {
                            var msg = type.First().Item2.GetConstructor(new Type[] { typeof(byte[]) })
                                .Invoke(new Object[] { match.Groups[3].Value.HexStringToByteArray() });

                            Console.WriteLine(msg.ToJSONWithType());
                        }
                        if (type.First().Item3 == PgnType.Fast)
                        {
                            //0x1f
                            // E0 size data...
                            // E1 data...
                            var data = match.Groups[3].Value.HexStringToByteArray();

                            if ((data[0] & 0x1f) == 0)
                            {
                                //first frame
                                var size = data[1];

                                msgbuffer[type.First().Item1] = new List<Byte>(size);
                                msgbuffer[type.First().Item1].AddRange(data.Skip(2));
                            }
                            else
                            {
                                msgbuffer[type.First().Item1].AddRange(data.Skip(1));
                                var size = type.First().Item4;

                                if (msgbuffer[type.First().Item1].Count >= size)
                                {
                                    var msg = type.First().Item2.GetConstructor(new Type[] { typeof(byte[]) })
                                        .Invoke(new Object[] { msgbuffer[type.First().Item1].ToArray() });


                                    Console.WriteLine(msg.ToJSONWithType());

                                    msgbuffer.Remove(type.First().Item1);
                                }
                            }
                        }
                        else
                        {
                            //iso

                        }
                    }

                    //       {4} {5} {6} {7}
                    Console.WriteLine("{0} {1} {2} {3}", DateTime.Now.ToString("hh:mm:ss.fff"), "T", match.Groups[1].Value, (match.Groups[3].Value.HexStringToSpacedHex()), cf.Priority, cf.DataPage, cf.PDU, cf.SourceAddress);
                }
            }
        }
    }
}