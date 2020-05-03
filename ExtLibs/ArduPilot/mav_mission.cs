using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using MissionPlanner.Utilities;

namespace MissionPlanner.ArduPilot
{
    public class mav_mission
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static async Task<List<Locationwp>> download(MAVLinkInterface port, byte sysid, byte compid, MAVLink.MAV_MISSION_TYPE type, Action<int, string> progress = null)
        {
            List<Locationwp> commandlist = new List<Locationwp>();

            try
            {
                if (!port.BaseStream.IsOpen)
                {
                    throw new Exception("Please Connect First!");
                }

                bool use_supportfence = (port.MAVlist[sysid, compid].cs.capabilities & (uint)MAVLink.MAV_PROTOCOL_CAPABILITY.MISSION_FENCE) > 0;

                bool use_int = (port.MAVlist[sysid, compid].cs.capabilities & (uint)MAVLink.MAV_PROTOCOL_CAPABILITY.MISSION_INT) > 0;

                if (!use_supportfence && !port.MAVlist[sysid, compid].mavlinkv2 && type != MAVLink.MAV_MISSION_TYPE.MISSION)
                {
                    throw new Exception("Mission type only supported under mavlink2");
                }

                progress?.Invoke(0, "Getting WP count");

                log.Info("Getting WP # " + type);

                int cmdcount = await port.getWPCountAsync(sysid, compid, type).ConfigureAwait(false);

                for (ushort a = 0; a < cmdcount; a++)
                {
                    log.Info("Getting WP" + a);
                    progress?.Invoke((a * 100) / cmdcount, "Getting WP " + a);
                    commandlist.Add(await port.getWPAsync(sysid, compid, a, type).ConfigureAwait(false));
                }

                port.setWPACK(sysid, compid, type);

                progress?.Invoke(100, "Done");

                log.Info("Done " + type);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }

            return commandlist;
        }

        public static async Task upload(MAVLinkInterface port, byte sysid, byte compid, MAVLink.MAV_MISSION_TYPE type, List<Locationwp> commandlist, Action<int,string> progress = null)
        {
            try
            {
                if (!port.BaseStream.IsOpen)
                {
                    throw new Exception("Please connect first!");
                }

                bool use_supportfence = (port.MAVlist[sysid, compid].cs.capabilities & (uint)MAVLink.MAV_PROTOCOL_CAPABILITY.MISSION_FENCE) > 0;

                bool use_int = (port.MAVlist[sysid, compid].cs.capabilities & (uint)MAVLink.MAV_PROTOCOL_CAPABILITY.MISSION_INT) > 0;

                if (!use_supportfence && !port.MAVlist[sysid, compid].mavlinkv2 && type != MAVLink.MAV_MISSION_TYPE.MISSION)
                {
                    throw new Exception("Mission type only supported under mavlink2");
                }

                int a;

                await port.setWPTotalAsync(sysid, compid, (ushort) commandlist.Count, type).ConfigureAwait(false);

                // process commandlist to the mav
                for (a = 0; a < commandlist.Count; a++)
                {
                    var temp = commandlist[a];

                    // handle current wp upload number
                    int uploadwpno = a;

                    progress?.Invoke(((a*100) / commandlist.Count), "Uploading WP " + a);

                    // try send the wp
                    MAVLink.MAV_MISSION_RESULT ans = await port.setWPAsync(sysid, compid, temp, (ushort)(uploadwpno), (MAVLink.MAV_FRAME)temp.frame, 0, 1, use_int, type).ConfigureAwait(false);

                    // we timed out while uploading wps/ command wasnt replaced/ command wasnt added
                    if (ans == MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ERROR)
                    {
                        // resend for partial upload
                        await port.setWPPartialUpdateAsync(sysid, compid, (ushort)(uploadwpno), (ushort)commandlist.Count, type).ConfigureAwait(false);
                        // reupload this point.
                        ans = await port.setWPAsync(sysid, compid, temp, (ushort) (uploadwpno),
                            (MAVLink.MAV_FRAME) temp.frame, 0, 1, use_int, type).ConfigureAwait(false);
                    }

                    if (ans == MAVLink.MAV_MISSION_RESULT.MAV_MISSION_NO_SPACE)
                    {
                        throw new Exception("Upload failed, please reduce the number of wp's");
                    }
                    if (ans == MAVLink.MAV_MISSION_RESULT.MAV_MISSION_INVALID)
                    {
                        throw new Exception(
                            "Upload failed, mission was rejected by the Mav,\n item had a bad option wp# " + a + " " +
                            ans);
                    }
                    if (ans == MAVLink.MAV_MISSION_RESULT.MAV_MISSION_INVALID_SEQUENCE)
                    {
                        // invalid sequence can only occur if we failed to see a response from the apm when we sent the request.
                        // or there is io lag and we send 2 mission_items and get 2 responces, one valid, one a ack of the second send

                        // the ans is received via mission_ack, so we dont know for certain what our current request is for. as we may have lost the mission_request

                        // get requested wp no - 1;
                        try
                        {
                            a = await port.getRequestedWPNoAsync(sysid, compid).ConfigureAwait(false) - 1;
                        }
                        catch
                        {
                            // resend for partial upload
                            await port.setWPPartialUpdateAsync(sysid, compid, (ushort)(uploadwpno), (ushort)commandlist.Count, type).ConfigureAwait(false);
                            // reupload this point.
                        }

                        continue;
                    }
                    if (ans != MAVLink.MAV_MISSION_RESULT.MAV_MISSION_ACCEPTED)
                    {
                        throw new Exception("Upload "+ type.ToString() + " failed " + Enum.Parse(typeof(MAVLink.MAV_CMD), temp.id.ToString()) +
                                            " " + Enum.Parse(typeof(MAVLink.MAV_MISSION_RESULT), ans.ToString()));
                    }
                }

                port.setWPACK(sysid, compid, type);

            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }
    }
}