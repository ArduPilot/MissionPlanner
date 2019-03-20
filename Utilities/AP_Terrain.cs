using System;
using System.IO;
using System.Runtime.InteropServices;
using uint8_t = System.Byte;
using int8_t = System.SByte;
using uint32_t = System.UInt32;
using int32_t = System.Int32;
using int16_t = System.Int16;
using uint16_t = System.UInt16;
using uint64_t = System.UInt64;
using int64_t = System.Int64;

namespace MissionPlanner.Utilities
{
    public class AP_Terrain : Utils
    {
        private bool enable = true;
        private int32_t grid_spacing = 100;
        /*
        calculate lookahead rise in terrain. This returns extra altitude
        needed to clear upcoming terrain in meters
        */

        private float lookahead(Locationwp loc, float bearing, float distance, float climb_ratio)
        {
            if (!enable || grid_spacing <= 0)
            {
                return 0;
            }

            //Locationwp loc;
            //if (!ahrs.get_position(loc))
            {
                // we don't know where we are
                //return 0;
            }
            float base_height = 0;
            if (!height_amsl(loc, ref base_height))
            {
                // we don't know our current terrain height
                return 0;
            }

            float climb = 0;
            float lookahead_estimate = 0;

            // check for terrain at grid spacing intervals
            while (distance > 0)
            {
                location_update(loc, bearing, grid_spacing);
                climb += climb_ratio*grid_spacing;
                distance -= grid_spacing;
                float height = 0;
                if (height_amsl(loc, ref height))
                {
                    float rise = (height - base_height) - climb;
                    if (rise > lookahead_estimate)
                    {
                        lookahead_estimate = rise;
                    }
                }
            }

            return lookahead_estimate;
        }


        /*
        return terrain height in meters above average sea level (WGS84) for
        a given position

        This is the base function that other height calculations are derived
        from. The functions below are more convenient for most uses

        This function costs about 20 microseconds on Pixhawk
        */

        private bool height_amsl(Locationwp loc, ref float height)
        {
            height = (float) srtm.getAltitude(loc.lat, loc.lng).alt;
            return true;
        }

        /*
        *  extrapolate latitude/longitude given bearing and distance
        * Note that this function is accurate to about 1mm at a distance of 
        * 100m. This function has the advantage that it works in relative
        * positions, so it keeps the accuracy even when dealing with small
        * distances and floating point numbers
        */

        private static void location_update(Locationwp loc, float bearing, float distance)
        {
            float ofs_north = cosf(radians(bearing))*distance;
            float ofs_east = sinf(radians(bearing))*distance;
            location_offset(loc, ofs_north, ofs_east);
        }

        /*
        *  extrapolate latitude/longitude given distances north and east
        *  This function costs about 80 usec on an AVR2560
        */

        private static void location_offset(Locationwp loc, float ofs_north, float ofs_east)
        {
            if (!is_zero(ofs_north) || !is_zero(ofs_east))
            {
                int32_t dlat = (int32_t) (ofs_north*LOCATION_SCALING_FACTOR_INV);
                int32_t dlng = (int32_t) ((ofs_east*LOCATION_SCALING_FACTOR_INV)/longitude_scale(loc));
                loc.lat += dlat;
                loc.lng += dlng;
            }
        }

        private static Vector3 location_diff(Locationwp loc1, Locationwp loc2)
        {
            return new Vector3((loc2.lat - loc1.lat)*LOCATION_SCALING_FACTOR,
                               (loc2.lng - loc1.lng)*LOCATION_SCALING_FACTOR*longitude_scale(loc1));
        }


        private static float longitude_scale(Locationwp loc)
        {
            float scale = cosf(loc.lat * 1.0e-7f * DEG_TO_RAD);
            return constrain_float(scale, 0.01f, 1.0f);
        }

        public class IO
        {
            private const int TERRAIN_GRID_MAVLINK_SIZE = 4;
            private const int TERRAIN_GRID_BLOCK_MUL_X = 7;
            private const int TERRAIN_GRID_BLOCK_MUL_Y = 8;
            private const int TERRAIN_GRID_BLOCK_SPACING_X =((TERRAIN_GRID_BLOCK_MUL_X-1)*TERRAIN_GRID_MAVLINK_SIZE);
            private const int TERRAIN_GRID_BLOCK_SPACING_Y =((TERRAIN_GRID_BLOCK_MUL_Y-1)*TERRAIN_GRID_MAVLINK_SIZE);
            private const int TERRAIN_GRID_BLOCK_SIZE_X = (TERRAIN_GRID_MAVLINK_SIZE*TERRAIN_GRID_BLOCK_MUL_X);
            private const int TERRAIN_GRID_BLOCK_SIZE_Y = (TERRAIN_GRID_MAVLINK_SIZE*TERRAIN_GRID_BLOCK_MUL_Y);
            private const int TERRAIN_GRID_FORMAT_VERSION = 1;

            [StructLayoutAttribute(LayoutKind.Sequential, Pack = 1,Size = 2048)]
            public struct grid_block
            {
                // bitmap of 4x4 grids filled in from GCS (56 bits are used)
                public uint64_t bitmap;

                // south west corner of block in degrees*10^7
                public int32_t lat;
                public int32_t lon;

                // crc of whole block, taken with crc=0
                public uint16_t crc;

                // format version number
                public uint16_t version;

                // grid spacing in meters
                public uint16_t spacing;

                // heights in meters over a 32*28 grid
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = TERRAIN_GRID_BLOCK_SIZE_X * TERRAIN_GRID_BLOCK_SIZE_Y)]
                public int16_t[] height;

                // indices info 32x28 grids for this degree reference
                public uint16_t grid_idx_x;
                public uint16_t grid_idx_y;

                // rounded latitude/longitude in degrees. 
                public int16_t lon_degrees;
                public int8_t lat_degrees;
            }

            [StructLayoutAttribute(LayoutKind.Explicit, Pack = 1)]
            public struct grid_io_block
            {
                [FieldOffset(0)] public grid_block block;
                //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2048)]
                //[FieldOffset(0)] public uint8_t[] buffer;
                    // 2048
            }

            public struct grid_info
            {
                // rounded latitude/longitude in degrees. 
                public int8_t lat_degrees;
                public int16_t lon_degrees;

                // lat and lon of SW corner of this 32*28 grid, *10^7 degrees
                public int32_t grid_lat;
                public int32_t grid_lon;

                // indices info 32x28 grids for this degree reference
                public uint16_t grid_idx_x;
                public uint16_t grid_idx_y;

                // indexes into 32x28 grid
                public uint8_t idx_x; // north (0..27)
                public uint8_t idx_y; // east  (0..31)

                // fraction within the grid square
                public float frac_x; // north (0..1)
                public float frac_y; // east  (0..1)

                // file offset of this grid
                public uint32_t file_offset;
            };

            private grid_io_block disk_block = new grid_io_block();

            public ushort grid_spacing = 100;

            public void test()
            {
                var fs = File.Open(@"C:\Users\michael\Documents\Mission Planner\sitl\d0\terrain\S36E149.DAT", FileMode.Open);
                var br = new BinaryReader(fs);

                var buffer = new byte[2048];

                while (br.BaseStream.Position < br.BaseStream.Length)
                {
                    grid_info info = new grid_info();
                    var loc = new Locationwp() {lat = -35.363261*1.0e7, lng = 149.165230*1.0e7};
                    calculate_grid_info(loc, ref info);

                    disk_block.block.lat = info.grid_lat;
                    disk_block.block.lon = info.grid_lon;
                    disk_block.block.spacing = grid_spacing;
                    disk_block.block.grid_idx_x = info.grid_idx_x;
                    disk_block.block.grid_idx_y = info.grid_idx_y;
                    disk_block.block.lat_degrees = info.lat_degrees;
                    disk_block.block.lon_degrees = info.lon_degrees;
                    disk_block.block.version = TERRAIN_GRID_FORMAT_VERSION;

                    // 29,5 = 1792000
                    //read_block(fs);

                    //continue;

                    var off = br.BaseStream.Position;

                    br.Read(buffer, 0, buffer.Length);

                    disk_block = buffer.ByteArrayToStructure<grid_io_block>(0);

                    if (disk_block.block.bitmap > 0 && disk_block.block.lat != 0 && disk_block.block.lon != 0)
                    {
                        Console.WriteLine("Got {0} - {1} at idx {2},{3} off {4}", disk_block.block.lat, disk_block.block.lon, disk_block.block.grid_idx_x, disk_block.block.grid_idx_y, off);
                    }
                }

            }

            private void calculate_grid_info(Locationwp loc,ref grid_info info)
            {
                // grids start on integer degrees. This makes storing terrain data
                // on the SD card a bit easier
                info.lat_degrees = (int8_t) ((loc.lat < 0 ? (loc.lat - 9999999L) : loc.lat)/(10*1000*1000L));
                info.lon_degrees = (int16_t) ((loc.lng < 0 ? (loc.lng - 9999999L) : loc.lng)/(10*1000*1000L));

                // create reference position for this rounded degree position
                Locationwp ref1 = new Locationwp();
                ref1.lat = info.lat_degrees*10*1000*1000L;
                ref1.lng = info.lon_degrees*10*1000*1000L;

                // find offset from reference
                Vector3 offset = location_diff(ref1, loc);

                // get indices in terms of grid_spacing elements
                uint32_t idx_x = (uint32_t) (offset.x/grid_spacing);
                uint32_t idx_y = (uint32_t) (offset.y/grid_spacing);

                // find indexes into 32*28 grids for this degree reference. Note
                // the use of TERRAIN_GRID_BLOCK_SPACING_{X,Y} which gives a one square
                // overlap between grids
                info.grid_idx_x = (ushort) (idx_x/TERRAIN_GRID_BLOCK_SPACING_X);
                info.grid_idx_y = (ushort) (idx_y/TERRAIN_GRID_BLOCK_SPACING_Y);

                // find the indices within the 32*28 grid
                info.idx_x = (byte) (idx_x%TERRAIN_GRID_BLOCK_SPACING_X);
                info.idx_y = (byte) (idx_y%TERRAIN_GRID_BLOCK_SPACING_Y);

                // find the fraction (0..1) within the square
                info.frac_x = (float) ((offset.x - idx_x*grid_spacing)/grid_spacing);
                info.frac_y = (float) ((offset.y - idx_y*grid_spacing)/grid_spacing);

                // calculate lat/lon of SW corner of 32*28 grid_block
                location_offset(ref1,
                    info.grid_idx_x*TERRAIN_GRID_BLOCK_SPACING_X*(float) grid_spacing,
                    info.grid_idx_y*TERRAIN_GRID_BLOCK_SPACING_Y*(float) grid_spacing);
                info.grid_lat = (int32_t) ref1.lat;
                info.grid_lon = (int32_t) ref1.lng;

                //ASSERT_RANGE(info.idx_x,0, TERRAIN_GRID_BLOCK_SPACING_X-1);
                //ASSERT_RANGE(info.idx_y,0, TERRAIN_GRID_BLOCK_SPACING_Y-1);
                //ASSERT_RANGE(info.frac_x,0,1);
                //ASSERT_RANGE(info.frac_y,0,1);
            }

            private void seek_offset(Stream st)
            {
                grid_block block = disk_block.block;
                // work out how many longitude blocks there are at this latitude
                Locationwp loc1 = new Locationwp();
                Locationwp loc2 = new Locationwp();
                loc1.lat = block.lat_degrees*10*1000*1000L;
                loc1.lng = block.lon_degrees*10*1000*1000L;
                loc2.lat = block.lat_degrees*10*1000*1000L;
                loc2.lng = (block.lon_degrees + 1)*10*1000*1000L;

                // shift another two blocks east to ensure room is available
                location_offset(loc2, 0, 2*grid_spacing*TERRAIN_GRID_BLOCK_SIZE_Y);
                Vector3 offset = location_diff(loc1, loc2);
                uint16_t east_blocks = (uint16_t) (offset.y/(grid_spacing*TERRAIN_GRID_BLOCK_SIZE_Y));
                uint32_t file_offset = (uint32_t) ((east_blocks*block.grid_idx_x +
                                                    block.grid_idx_y)*2048);

                st.Seek(file_offset, SeekOrigin.Begin);
            }

            private void read_block(Stream fd)
            {
                seek_offset(fd);

                int32_t lat = disk_block.block.lat;
                int32_t lon = disk_block.block.lon;

                var ret = read(fd, ref disk_block, 2048);
                if (ret != 2048 ||
                    disk_block.block.lat != lat ||
                    disk_block.block.lon != lon ||
                    disk_block.block.bitmap == 0 ||
                    disk_block.block.spacing != grid_spacing ||
                    disk_block.block.version != TERRAIN_GRID_FORMAT_VERSION //||
                    //disk_block.block.crc != get_block_crc(disk_block.block)
                    )
                {

                    printf("read empty block at %ld %ld ret=%d\n",
                        (long) lat,
                        (long) lon,
                        (int) ret);

                    // a short read or bad data is not an IO failure, just a
                    // missing block on disk
                    memset(ref disk_block, 0, Marshal.SizeOf(disk_block));
                    disk_block.block.lat = lat;
                    disk_block.block.lon = lon;
                    disk_block.block.bitmap = 0;
                }
                else
                {

                    printf("read block at %ld %ld ret=%d mask=%07llx\n",
                        (long) lat,
                        (long) lon,
                        (int) ret,
                        (uint64_t) disk_block.block.bitmap);
                }
                //disk_io_state = DiskIoDoneRead;
            }

            private int read(Stream fd, ref grid_io_block diskBlock, int sizeOf)
            {
                var buf = new byte[sizeOf];

                var read = fd.Read(buf, 0, sizeOf);

                diskBlock = buf.ByteArrayToStructure<grid_io_block>(0);

                return read;
            }

            private void printf(string input, params object[] args)
            {
                AT.MIN.Tools.printf(input, args);
            }

            private void memset(ref grid_io_block diskBlock, int i, int sizeOf)
            {
        
            }
            
        }
    }
}