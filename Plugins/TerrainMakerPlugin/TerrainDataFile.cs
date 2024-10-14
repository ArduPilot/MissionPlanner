using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TerrainMakerPlugin
{

    public class TerrainDataFile
    {

        //Constants

        // MAVLink sends 4x4 grids
        public const int TERRAIN_GRID_MAVLINK_SIZE = 4;

        // a 2k grid_block on disk contains 8x7 of the mavlink grids.  Each
        // grid block overlaps by one with its neighbour. This ensures that
        // the altitude at any point can be calculated from a single grid
        // block
        public const int TERRAIN_GRID_BLOCK_MUL_X = 7;
        public const int TERRAIN_GRID_BLOCK_MUL_Y = 8;

        // this is the spacing between 32x28 grid blocks, in grid_spacing units
        public const int TERRAIN_GRID_BLOCK_SPACING_X = ((TERRAIN_GRID_BLOCK_MUL_X - 1) * TERRAIN_GRID_MAVLINK_SIZE);
        public const int TERRAIN_GRID_BLOCK_SPACING_Y = ((TERRAIN_GRID_BLOCK_MUL_Y - 1) * TERRAIN_GRID_MAVLINK_SIZE);

        //  giving a total grid size of a disk grid_block of 32x28
        public const int TERRAIN_GRID_BLOCK_SIZE_X = (TERRAIN_GRID_MAVLINK_SIZE * TERRAIN_GRID_BLOCK_MUL_X);
        public const int TERRAIN_GRID_BLOCK_SIZE_Y = (TERRAIN_GRID_MAVLINK_SIZE * TERRAIN_GRID_BLOCK_MUL_Y);

        // format of grid on disk
        public const int TERRAIN_GRID_FORMAT_VERSION = 1;

        public const int IO_BLOCK_SIZE = 2048;
        public const int IO_BLOCK_DATA_SIZE = 1821;
        public const int IO_BLOCK_TRAILER_SIZE = IO_BLOCK_SIZE - IO_BLOCK_DATA_SIZE;

        public static int GRID_SPACING = 100;



        public static Int32 east_blocks(Location loc)
        {
            Location loc2 = loc.add_offset_meters(0, 2 * GRID_SPACING * TERRAIN_GRID_BLOCK_SIZE_Y);
            loc2.lng = loc2.lng + 10 * 1000 * 1000;

            Vector2d offset = loc.get_distance_NE(loc2);

            return (Int32)(Math.Round(offset.y) / (GRID_SPACING * TERRAIN_GRID_BLOCK_SPACING_Y));

        }


        public static Location pos_from_file_offset(Int32 file_base_lat, Int32 file_base_lon, int file_offset)
        {
            Location ref_loc = new Location(file_base_lat * 10 * 1000 * 1000, file_base_lon * 10 * 1000 * 1000);

            Int32 stride = east_blocks(ref_loc);

            Int32 blocks = (Int32)Math.Floor((double)(file_offset / IO_BLOCK_SIZE));

            Int32 grid_idx_x = (Int32)Math.Floor((double)(blocks / stride));

            Int32 grid_idx_y = blocks % stride;



            Int32 idx_x = grid_idx_x * TERRAIN_GRID_BLOCK_SPACING_X;
            Int32 idx_y = grid_idx_y * TERRAIN_GRID_BLOCK_SPACING_Y;

            Location loc = ref_loc.add_offset_meters(idx_x * GRID_SPACING, idx_y * GRID_SPACING);

            grid_idx_x = (Int32)(idx_x / TERRAIN_GRID_BLOCK_SPACING_X);
            grid_idx_y = (Int32)(idx_y / TERRAIN_GRID_BLOCK_SPACING_Y);

            loc = ref_loc.add_offset_meters(grid_idx_x * TERRAIN_GRID_BLOCK_SPACING_X * GRID_SPACING, grid_idx_y * TERRAIN_GRID_BLOCK_SPACING_Y * GRID_SPACING);

            return loc;

        }

        public class GridBlock
        {
            // bitmap of 4x4 grids filled in from GCS (56 bits are used)
            public UInt64 Bitmap;

            // south west corner of block in degrees*10^7
            public int Lat;
            public int Lon;
            public Location blockTopLeft;

            // crc of whole block, taken with crc=0
            public ushort Crc;

            // format version number
            public ushort Version;

            // grid spacing in meters
            public ushort Spacing;

            // heights in meters over a 32*28 grid
            public short[] Height; // This should be initialized to a new short[TERRAIN_GRID_BLOCK_SIZE_X * TERRAIN_GRID_BLOCK_SIZE_Y]

            // indices info 32x28 grids for this degree reference
            public ushort GridIdxX;
            public ushort GridIdxY;

            // rounded latitude/longitude in degrees, of the whole dat file (like 41,11)
            public short LonDegrees;
            public sbyte LatDegrees;


            //public long ref_lat, ref_lon;


            //Constructor, pass the datafile location and location of the block left upper corner, plus spacing
            public GridBlock(sbyte lat_int, short lon_int, Location blockLocation, ushort spacing)
            {
                Spacing = (ushort)GRID_SPACING;
                Height = new short[TERRAIN_GRID_BLOCK_SIZE_X * TERRAIN_GRID_BLOCK_SIZE_Y];

                Version = TERRAIN_GRID_FORMAT_VERSION;

                LonDegrees = lon_int;
                LatDegrees = lat_int;


                Location ref_loc = new Location(lat_int * 10 * 1000 * 1000, lon_int * 10 * 1000 * 1000);
                Vector2d offset = ref_loc.get_distance_NE(blockLocation);

                long idx_x = (long)(Math.Round(offset.x) / GRID_SPACING);
                long idx_y = (long)(Math.Round(offset.y) / GRID_SPACING);

                GridIdxX = (ushort)Math.Floor(((double)idx_x / TERRAIN_GRID_BLOCK_SPACING_X));
                GridIdxY = (ushort)Math.Floor(((double)idx_y / TERRAIN_GRID_BLOCK_SPACING_Y));

                Location loc = ref_loc.add_offset_meters(GridIdxX * TERRAIN_GRID_BLOCK_SPACING_X * GRID_SPACING, GridIdxY * TERRAIN_GRID_BLOCK_SPACING_Y * GRID_SPACING);

                Lat = loc.lat;
                Lon = loc.lng;

                blockTopLeft = new Location (Lat , Lon);
            }


            public long blocknum()
            {
                long stride = east_blocks(new Location(LatDegrees * 10 * 1000 * 1000, LonDegrees * 10 * 1000 * 1000));
                return stride * GridIdxX + GridIdxY;
            }


            public bool isvalid(int x, int y)
            {
                // Check if a 4x4 block has any zero in it
                bool valid = true;

                if (GetHeight(x * 4, y * 4) == 0) valid = false;
                if (GetHeight(x * 4 + 1, y * 4) == 0) valid = false;
                if (GetHeight(x * 4 + 2, y * 4) == 0) valid = false;
                if (GetHeight(x * 4 + 3, y * 4) == 0) valid = false;
                if (GetHeight(x * 4, y * 4 + 1) == 0) valid = false;
                if (GetHeight(x * 4 + 1, y * 4 + 1) == 0) valid = false;
                if (GetHeight(x * 4 + 2, y * 4 + 1) == 0) valid = false;
                if (GetHeight(x * 4 + 3, y * 4 + 1) == 0) valid = false;

                if (GetHeight(x * 4, y * 4 + 2) == 0) valid = false;
                if (GetHeight(x * 4 + 1, y * 4 + 2) == 0) valid = false;
                if (GetHeight(x * 4 + 2, y * 4 + 2) == 0) valid = false;
                if (GetHeight(x * 4 + 3, y * 4 + 2) == 0) valid = false;

                if (GetHeight(x * 4, y * 4 + 3) == 0) valid = false;
                if (GetHeight(x * 4 + 1, y * 4 + 3) == 0) valid = false;
                if (GetHeight(x * 4 + 2, y * 4 + 3) == 0) valid = false;
                if (GetHeight(x * 4 + 3, y * 4 + 3) == 0) valid = false;

                return valid;
            }

            public void setBit(byte bitnumber)
            {
                if (bitnumber > 55) throw new Exception("Bit number out of range");
                UInt64 mask = ((UInt64)1 << bitnumber);
                Bitmap |= mask;

                if (Bitmap > 0xffffffffffffff00)
                {
                    Console.WriteLine("Bitmap Overflow {0}", bitnumber);
                    throw new Exception("Bitmap overflow");
                }
            }

            public void clearBit(byte bitnumber)
            {
                Bitmap &= (UInt64)~(1 << bitnumber);
            }

            public bool getBit(byte bitnumber)
            {
                return (Bitmap & (ulong)(1 << bitnumber)) != 0;
            }

            public short GetHeight(int x, int y)
            {
                return Height[y * TERRAIN_GRID_BLOCK_SIZE_X + x];
            }

            public void SetHeight(int x, int y, short value)
            {
                Height[y * TERRAIN_GRID_BLOCK_SIZE_X + x] = value;
            }


            private byte[] Pack()
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (BinaryWriter writer = new BinaryWriter(ms))
                    {
                        writer.Write(BitConverter.GetBytes(Bitmap));
                        writer.Write(BitConverter.GetBytes(Lat));
                        writer.Write(BitConverter.GetBytes(Lon));
                        writer.Write(BitConverter.GetBytes(Crc));
                        writer.Write(BitConverter.GetBytes(Version));
                        writer.Write(BitConverter.GetBytes(Spacing));


                        for (int gx = 0; gx < TerrainDataFile.TERRAIN_GRID_BLOCK_SIZE_X; gx++)
                        {
                            for (int gy = 0; gy < TerrainDataFile.TERRAIN_GRID_BLOCK_SIZE_Y; gy++)
                            {
                                writer.Write(BitConverter.GetBytes(GetHeight(gx, gy)));

                            }
                        }


                            //    foreach (var h in Height)
                            //{
                            //    writer.Write(BitConverter.GetBytes(h));
                            //}


                        writer.Write(BitConverter.GetBytes(GridIdxX));
                        writer.Write(BitConverter.GetBytes(GridIdxY));
                        writer.Write(BitConverter.GetBytes(LonDegrees));
                        writer.Write(BitConverter.GetBytes(LatDegrees));

                        for (int i = 1; i < IO_BLOCK_TRAILER_SIZE; i++)
                        {
                            writer.Write((byte)0);
                        }
                    }
                    return ms.ToArray();
                }
            }

            public byte[] getPackedBytes()
            {
                Crc = 0;
                byte[] packed = Pack();
                Crc = crc16(packed, IO_BLOCK_DATA_SIZE);
                byte[] packedWithCRC = Pack();
                return packedWithCRC;
            }

        }


        /* CRC16 implementation according to CCITT standards */
        static ushort[] crc16tab = new ushort[256] {
                0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50A5, 0x60C6, 0x70E7,
                0x8108, 0x9129, 0xA14A, 0xB16B, 0xC18C, 0xD1AD, 0xE1CE, 0xF1EF,
                0x1231, 0x0210, 0x3273, 0x2252, 0x52B5, 0x4294, 0x72F7, 0x62D6,
                0x9339, 0x8318, 0xB37B, 0xA35A, 0xD3BD, 0xC39C, 0xF3FF, 0xE3DE,
                0x2462, 0x3443, 0x0420, 0x1401, 0x64E6, 0x74C7, 0x44A4, 0x5485,
                0xA56A, 0xB54B, 0x8528, 0x9509, 0xE5EE, 0xF5CF, 0xC5AC, 0xD58D,
                0x3653, 0x2672, 0x1611, 0x0630, 0x76D7, 0x66F6, 0x5695, 0x46B4,
                0xB75B, 0xA77A, 0x9719, 0x8738, 0xF7DF, 0xE7FE, 0xD79D, 0xC7BC,
                0x48C4, 0x58E5, 0x6886, 0x78A7, 0x0840, 0x1861, 0x2802, 0x3823,
                0xC9CC, 0xD9ED, 0xE98E, 0xF9AF, 0x8948, 0x9969, 0xA90A, 0xB92B,
                0x5AF5, 0x4AD4, 0x7AB7, 0x6A96, 0x1A71, 0x0A50, 0x3A33, 0x2A12,
                0xDBFD, 0xCBDC, 0xFBBF, 0xEB9E, 0x9B79, 0x8B58, 0xBB3B, 0xAB1A,
                0x6CA6, 0x7C87, 0x4CE4, 0x5CC5, 0x2C22, 0x3C03, 0x0C60, 0x1C41,
                0xEDAE, 0xFD8F, 0xCDEC, 0xDDCD, 0xAD2A, 0xBD0B, 0x8D68, 0x9D49,
                0x7E97, 0x6EB6, 0x5ED5, 0x4EF4, 0x3E13, 0x2E32, 0x1E51, 0x0E70,
                0xFF9F, 0xEFBE, 0xDFDD, 0xCFFC, 0xBF1B, 0xAF3A, 0x9F59, 0x8F78,
                0x9188, 0x81A9, 0xB1CA, 0xA1EB, 0xD10C, 0xC12D, 0xF14E, 0xE16F,
                0x1080, 0x00A1, 0x30C2, 0x20E3, 0x5004, 0x4025, 0x7046, 0x6067,
                0x83B9, 0x9398, 0xA3FB, 0xB3DA, 0xC33D, 0xD31C, 0xE37F, 0xF35E,
                0x02B1, 0x1290, 0x22F3, 0x32D2, 0x4235, 0x5214, 0x6277, 0x7256,
                0xB5EA, 0xA5CB, 0x95A8, 0x8589, 0xF56E, 0xE54F, 0xD52C, 0xC50D,
                0x34E2, 0x24C3, 0x14A0, 0x0481, 0x7466, 0x6447, 0x5424, 0x4405,
                0xA7DB, 0xB7FA, 0x8799, 0x97B8, 0xE75F, 0xF77E, 0xC71D, 0xD73C,
                0x26D3, 0x36F2, 0x0691, 0x16B0, 0x6657, 0x7676, 0x4615, 0x5634,
                0xD94C, 0xC96D, 0xF90E, 0xE92F, 0x99C8, 0x89E9, 0xB98A, 0xA9AB,
                0x5844, 0x4865, 0x7806, 0x6827, 0x18C0, 0x08E1, 0x3882, 0x28A3,
                0xCB7D, 0xDB5C, 0xEB3F, 0xFB1E, 0x8BF9, 0x9BD8, 0xABBB, 0xBB9A,
                0x4A75, 0x5A54, 0x6A37, 0x7A16, 0x0AF1, 0x1AD0, 0x2AB3, 0x3A92,
                0xFD2E, 0xED0F, 0xDD6C, 0xCD4D, 0xBDAA, 0xAD8B, 0x9DE8, 0x8DC9,
                0x7C26, 0x6C07, 0x5C64, 0x4C45, 0x3CA2, 0x2C83, 0x1CE0, 0x0CC1,
                0xEF1F, 0xFF3E, 0xCF5D, 0xDF7C, 0xAF9B, 0xBFBA, 0x8FD9, 0x9FF8,
                0x6E17, 0x7E36, 0x4E55, 0x5E74, 0x2E93, 0x3EB2, 0x0ED1, 0x1EF0
        };



        public static ushort crc16(byte[] data, int len)
        {
            ushort crc = 0;
            for (int i = 0; i < len; i++)
            {
                crc = (ushort)((crc << 8) ^ crc16tab[((crc >> 8) ^ data[i]) & 0x00FF]);
            }
            return crc;
        }


    }



}
