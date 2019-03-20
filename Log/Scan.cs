using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using MissionPlanner.Utilities;

namespace MissionPlanner.Log
{
    public class Scan
    {
        public static void ScanAccel()
        {
            string[] files = Directory.GetFiles(Settings.Instance.LogDir, "*.tlog", SearchOption.AllDirectories);

            List<string> badfiles = new List<string>();

            foreach (var file in files)
            {
                bool board = false;

                Console.WriteLine(file);

                using (MAVLinkInterface mavi = new MAVLinkInterface())
                using (mavi.logplaybackfile = new BinaryReader(File.OpenRead(file)))
                {
                    mavi.logreadmode = true;

                    try
                    {
                        while (mavi.logplaybackfile.BaseStream.Position < mavi.logplaybackfile.BaseStream.Length)
                        {
                            MAVLink.MAVLinkMessage packet = mavi.readPacket();

                            if (packet.Length == 0)
                                break;

                            var objectds = mavi.DebugPacket(packet, false);

                            if (objectds is MAVLink.mavlink_param_value_t)
                            {
                                MAVLink.mavlink_param_value_t param = (MAVLink.mavlink_param_value_t) objectds;

                                if (ASCIIEncoding.ASCII.GetString(param.param_id).Contains("INS_PRODUCT_ID"))
                                {
                                    if (param.param_value == 0 || param.param_value == 5)
                                    {
                                        board = true;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

                            if (objectds is MAVLink.mavlink_raw_imu_t)
                            {
                                MAVLink.mavlink_raw_imu_t rawimu = (MAVLink.mavlink_raw_imu_t) objectds;

                                if (board && Math.Abs(rawimu.xacc) > 2000 && Math.Abs(rawimu.yacc) > 2000 &&
                                    Math.Abs(rawimu.zacc) > 2000)
                                {
                                    //CustomMessageBox.Show("Bad Log " + file);
                                    badfiles.Add(file);
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }
                }
            }

            if (badfiles.Count > 0)
            {
                FileStream fs = File.Open(Settings.Instance.LogDir + Path.DirectorySeparatorChar + "SearchResults.zip",
                    FileMode.Create);
                ZipOutputStream zipStream = new ZipOutputStream(fs);
                zipStream.SetLevel(9); //0-9, 9 being the highest level of compression
                zipStream.UseZip64 = UseZip64.Off; // older zipfile


                foreach (var file in badfiles)
                {
                    // entry 2
                    string entryName = ZipEntry.CleanName(Path.GetFileName(file));
                        // Removes drive from name and fixes slash direction
                    ZipEntry newEntry = new ZipEntry(entryName);
                    newEntry.DateTime = DateTime.Now;

                    zipStream.PutNextEntry(newEntry);

                    // Zip the file in buffered chunks
                    // the "using" will close the stream even if an exception occurs
                    byte[] buffer = new byte[4096];
                    using (FileStream streamReader = File.OpenRead(file))
                    {
                        StreamUtils.Copy(streamReader, zipStream, buffer);
                    }
                    zipStream.CloseEntry();
                }

                zipStream.IsStreamOwner = true; // Makes the Close also Close the underlying stream
                zipStream.Close();

                CustomMessageBox.Show("Added " + badfiles.Count + " logs to " + Settings.Instance.LogDir +
                                      Path.DirectorySeparatorChar +
                                      "SearchResults.zip\n Please send this file to Craig Elder <craig@3drobotics.com>");
            }
            else
            {
                CustomMessageBox.Show("No Bad Logs Found");
            }
        }
    }
}