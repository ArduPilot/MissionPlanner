using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MissionPlanner.Utilities
{
    public class TerrainFollow
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        bool issending = false;

        MAVLink.mavlink_terrain_request_t lastrequest;

        KeyValuePair<MAVLink.MAVLINK_MSG_ID, Func<byte[], bool>> subscription;

        private MAVLinkInterface _interface;

        public TerrainFollow(MAVLinkInterface inInterface)
        {
            _interface = inInterface;

            log.Info("Subscribe to packets");
            subscription = _interface.SubscribeToPacketType(MAVLink.MAVLINK_MSG_ID.TERRAIN_REQUEST, ReceviedPacket);
        }

        ~TerrainFollow()
        {
            log.Info("unSubscribe to packets");
            _interface.UnSubscribeToPacketType(subscription);
        }

        bool ReceviedPacket(byte[] rawpacket)
        {
            if (rawpacket[5] == (byte) MAVLink.MAVLINK_MSG_ID.TERRAIN_REQUEST)
            {
                MAVLink.mavlink_terrain_request_t packet =
                    rawpacket.ByteArrayToStructure<MAVLink.mavlink_terrain_request_t>();

                if (issending)
                    return false;

                lastrequest = packet;

                log.Info("received TERRAIN_REQUEST " + packet.lat/1e7 + " " + packet.lon/1e7 + " space " +
                         packet.grid_spacing + " " + Convert.ToString((long) packet.mask, 2));

                System.Threading.ThreadPool.QueueUserWorkItem(QueueSendGrid);
            }
            else if (rawpacket[5] == (byte) MAVLink.MAVLINK_MSG_ID.TERRAIN_REPORT)
            {
                MAVLink.mavlink_terrain_report_t packet =
                    rawpacket.ByteArrayToStructure<MAVLink.mavlink_terrain_report_t>();
                log.Info("received TERRAIN_REPORT " + packet.lat/1e7 + " " + packet.lon/1e7 + " " + packet.loaded + " " +
                         packet.pending);
            }
            return false;
        }

        void QueueSendGrid(object nothing)
        {
            issending = true;
            try
            {
                // 8 across - 7 down
                // cycle though the bitmask to check what we need to send (8*7)
                for (byte i = 0; i < 56; i++)
                {
                    // check to see if the ap requested this box.
                    if ((lastrequest.mask & ((ulong) 1 << i)) > 0)
                    {
                        // get the requested lat and lon
                        double lat = lastrequest.lat/1e7;
                        double lon = lastrequest.lon/1e7;

                        // get the distance between grids
                        int bitgridspacing = lastrequest.grid_spacing*4;

                        // get the new point, based on our current bit.
                        var newplla = new PointLatLngAlt(lat, lon).gps_offset(bitgridspacing*(i%8),
                            bitgridspacing*(int) Math.Floor(i/8.0));

                        // send a 4*4 grid, based on the lat lon of the bitmask
                        SendGrid(newplla.Lat, newplla.Lng, lastrequest.grid_spacing, i);

                        // 12hz = (43+6) * 12 = 588 bps
                        System.Threading.Thread.Sleep(1000/12);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            finally
            {
                issending = false;
            }
        }

        void SendGrid(double lat, double lon, ushort grid_spacing, byte bit)
        {
            log.Info("SendGrid " + lat + " " + lon + " space " + grid_spacing + " bit " + bit);

            MAVLink.mavlink_terrain_data_t resp = new MAVLink.mavlink_terrain_data_t();
            resp.grid_spacing = grid_spacing;
            resp.lat = lastrequest.lat;
            resp.lon = lastrequest.lon;
            resp.gridbit = bit;
            resp.data = new short[16];

            for (int i = 0; i < (4*4); i++)
            {
                int x = i%4;
                int y = i/4;

                PointLatLngAlt plla = new PointLatLngAlt(lat, lon).gps_offset(x*grid_spacing, y*grid_spacing);

                var alt = srtm.getAltitude(plla.Lat, plla.Lng);

                // check where the alt returned came from.
                if (alt.currenttype == srtm.tiletype.invalid)
                    return;

                resp.data[i] = (short) alt.alt;
            }

            _interface.sendPacket(resp);
        }

        public void checkTerrain(double lat, double lon)
        {
            MAVLink.mavlink_terrain_check_t packet = new MAVLink.mavlink_terrain_check_t();

            packet.lat = (int) (lat*1e7);
            packet.lon = (int) (lon*1e7);

            _interface.sendPacket(packet);
        }
    }
}