using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zlib;
using Newtonsoft.Json;

namespace MissionPlanner.Utilities
{
    public class apj_tool
    {
        public static void Process(string filename, string paramfile)
        {
            var firmware = File.ReadAllText(filename);
            var param = File.ReadAllText(paramfile).Replace("\r", "");

            var fw_json = JsonConvert.DeserializeObject<Hashtable>(firmware);
            var fw_base64 = fw_json["image"].ToString();
            var fw_binary = new MemoryStream();

            ZlibStream decompressionStream = new ZlibStream(new MemoryStream(Convert.FromBase64String(fw_base64)),
                CompressionMode.Decompress);

            decompressionStream.CopyTo(fw_binary);

            var magic_str = "PARMDEF";
            var param_magic = new byte[] {0x55, 0x37, 0xf4, 0xa0, 0x38, 0x5d, 0x48, 0x5b};

            fw_binary.Position = 0;
            var offset = ReadOneSrch(fw_binary, magic_str.Select(a => (byte) a).ToArray());
            if (offset == -1)
            {
                throw new Exception("No param area found");
            }

            var magicoffset = ReadOneSrch(fw_binary, param_magic);

            if (magicoffset != -1)
            {
                var foffset = fw_binary.Position;
                var br = new BinaryReader(fw_binary);

                var max_len = br.ReadInt16();
                var length = br.ReadInt16();
                Console.WriteLine("Found param defaults max_length={0} length={1}", max_len, length);

                if (param.Length > max_len)
                {
                    throw new Exception(String.Format("Error: Length {0} larger than maximum {1}", length, max_len));
                }

                var paramdata = new byte[length];
                br.Read(paramdata, 0, length);

                var paramstring = ASCIIEncoding.ASCII.GetString(paramdata);

                var new_fwms = new MemoryStream(fw_binary.ToArray());
                var new_fw = new BinaryWriter(new_fwms);

                new_fw.Seek((int)foffset + 2, SeekOrigin.Begin);
                new_fw.Write((short) param.Length);
                new_fw.Write(ASCIIEncoding.ASCII.GetBytes(param), 0, param.Length);

                fw_json["image"] = Convert.ToBase64String(ZlibStream.CompressBuffer(new_fwms.ToArray()));

                //File.WriteAllBytes(filename + "orig.bin", fw_binary.ToArray());
                //File.WriteAllBytes(filename + "new.bin", new_fwms.ToArray());
                File.WriteAllText(filename + "new.apj", JsonConvert.SerializeObject(fw_json, Formatting.Indented));

                return;
            }

            throw new Exception("Error: Param defaults support not found in firmware");
        }

        public static long ReadOneSrch(Stream haystack, byte[] needle)
        {
            int b;
            long i = 0;
            while ((b = haystack.ReadByte()) != -1)
            {
                if (b == needle[i++])
                {
                    if (i == needle.Length)
                        return haystack.Position - needle.Length;
                }
                else
                    i = b == needle[0] ? 1 : 0;
            }

            return -1;
        }
    }
}
