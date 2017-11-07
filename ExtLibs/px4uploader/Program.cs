using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace px4uploader
{
    /// <summary>
    /// otp signing process
    /// create yourself a 1024 bit private key - do not distribute and keep safe.
    /// put that key in pem format on the px4 sd card, in base64 encoding.
    /// goto the nuttx shell
    /// ensure private key is on sd card, and is readable by auth.
    /// run auth, and follow instructions. "auth -e -t -l" or "auth -v -l"
    /// 
    /// remove the sd card before distribution.
    /// 
    /// </summary>
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(@"Usage: px4uploader.exe C:\px4\Firmware\Images\px4fmu.px4");
                return;
            }

            Uploader(args[0]);

            Console.WriteLine("Press Any Key");
            Console.ReadLine();
        }

        public static bool Uploader(string fn) 
        {
            Firmware fw;
            Uploader up;

            fw = Firmware.ProcessFirmware(fn);

            Console.WriteLine("Loaded firmware for {0},{1} waiting for the bootloader...", fw.board_id, fw.board_revision);

            while (true)
            {
                string[] ports = GetPortNames();

                //ports = new string[] { "COM9"};

                foreach (string port in ports)
                {

                    if (!port.StartsWith("COM") && !port.Contains("APM") && !port.Contains("ACM") && !port.Contains("usb"))
                        continue;

                    Console.WriteLine(DateTime.Now.Millisecond +  " Trying Port "+port);

                    try
                    {
                        up = new Uploader(port, 115200);
                    }
                    catch ( Exception ex)
                    {
                        //System.Threading.Thread.Sleep(50);
                        Console.WriteLine(DateTime.Now.Millisecond  + " " +ex.Message);
                        continue;
                    }

                    try
                    {
                        up.identify();
                        Console.WriteLine("Found board type {0} boardrev {1} bl rev {2} fwmax {3} on {4}", up.board_type, up.board_rev,up.bl_rev,up.fw_maxsize, port);
                    }
                    catch ( Exception)
                    {
                        try
                        {
                            //System.Threading.ThreadPool.QueueUserWorkItem(up.__mavlinkreboot);
                        }
                        catch { 
                            //up.close();
                        }

                        Console.WriteLine(DateTime.Now.Millisecond + " " + "Not There..");
                        //Console.WriteLine(ex.Message);
                        try
                        {
                            up.close();
                        }
                        catch { }
                        continue;
                    }

                    try
                    {
                        up.currentChecksum(fw);
                    }
                    catch (Exception ex) { Console.WriteLine("No need to upload. already on the board" + ex.ToString()); up.close(); return true; }

                    try
                    {
                        up.upload(fw);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                    up.close();

                    return true;
                }
            }
        }

        public static string[] GetPortNames()
        {
            List<string> allPorts = new List<string>();

            if (Directory.Exists("/dev/"))
            {
                // cleanup now
                GC.Collect();
                // mono is failing in here on linux "too many open files"
                try
                {
                    if (Directory.Exists("/dev/serial/by-id/"))
                        allPorts.AddRange(Directory.GetFiles("/dev/serial/by-id/", "*"));
                }
                catch { }
                try
                {
                    allPorts.AddRange(Directory.GetFiles("/dev/", "ttyACM*"));
                }
                catch { }
                try
                {
                    allPorts.AddRange(Directory.GetFiles("/dev/", "ttyUSB*"));
                }
                catch { }
                try
                {
                    allPorts.AddRange(Directory.GetFiles("/dev/", "rfcomm*"));
                }
                catch { }
                try
                {
                    allPorts.AddRange(Directory.GetFiles("/dev/", "*usb*"));
                }
                catch { }
            }


            string[] ports = System.IO.Ports.SerialPort.GetPortNames();

            ports = ports.Select(p => trimcomportname(p.TrimEnd())).ToArray();

            allPorts.AddRange(ports);

            return allPorts.ToArray();
        }

        static string trimcomportname(string input)
        {
            var match = Regex.Match(input.ToUpper(), "(COM[0-9]+)");

            if (match.Success)
            {
                return match.Groups[0].Value;
            }

            return input;
        }

        /// <summary>
        /// This helper function parses an RSA private key using the ASN.1 format
        /// </summary>
        /// <param name="privateKeyBytes">Byte array containing PEM string of private key.</param>
        /// <returns>An instance of <see cref="RSACryptoServiceProvider"/> rapresenting the requested private key.
        /// Null if method fails on retriving the key.</returns>
        public static RSACryptoServiceProvider DecodeRsaPrivateKey(byte[] privateKeyBytes)
        {
            MemoryStream ms = new MemoryStream(privateKeyBytes);
            BinaryReader rd = new BinaryReader(ms);

            try
            {
                byte byteValue;
                ushort shortValue;

                shortValue = rd.ReadUInt16();

                switch (shortValue)
                {
                    case 0x8130:
                        // If true, data is little endian since the proper logical seq is 0x30 0x81
                        rd.ReadByte(); //advance 1 byte
                        break;
                    case 0x8230:
                        rd.ReadInt16();  //advance 2 bytes
                        break;
                    default:
                        Debug.Assert(false);     // Improper ASN.1 format
                        return null;
                }

                shortValue = rd.ReadUInt16();
                if (shortValue != 0x0102) // (version number)
                {
                    Debug.Assert(false);     // Improper ASN.1 format, unexpected version number
                    return null;
                }

                byteValue = rd.ReadByte();
                if (byteValue != 0x00)
                {
                    Debug.Assert(false);     // Improper ASN.1 format
                    return null;
                }

                // The data following the version will be the ASN.1 data itself, which in our case
                // are a sequence of integers.

                // In order to solve a problem with instancing RSACryptoServiceProvider
                // via default constructor on .net 4.0 this is a hack
                CspParameters parms = new CspParameters();
                parms.Flags = CspProviderFlags.NoFlags;
                parms.KeyContainerName = Guid.NewGuid().ToString().ToUpperInvariant();
                parms.ProviderType = ((Environment.OSVersion.Version.Major > 5) || ((Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor >= 1))) ? 0x18 : 1;

                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(parms);
                RSAParameters rsAparams = new RSAParameters();

                rsAparams.Modulus = rd.ReadBytes(Helpers.DecodeIntegerSize(rd));

                // Argh, this is a pain.  From emperical testing it appears to be that RSAParameters doesn't like byte buffers that
                // have their leading zeros removed.  The RFC doesn't address this area that I can see, so it's hard to say that this
                // is a bug, but it sure would be helpful if it allowed that. So, there's some extra code here that knows what the
                // sizes of the various components are supposed to be.  Using these sizes we can ensure the buffer sizes are exactly
                // what the RSAParameters expect.  Thanks, Microsoft.
                RSAParameterTraits traits = new RSAParameterTraits(rsAparams.Modulus.Length * 8);

                rsAparams.Modulus = Helpers.AlignBytes(rsAparams.Modulus, traits.size_Mod);
                rsAparams.Exponent = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_Exp);
                rsAparams.D = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_D);
                rsAparams.P = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_P);
                rsAparams.Q = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_Q);
                rsAparams.DP = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_DP);
                rsAparams.DQ = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_DQ);
                rsAparams.InverseQ = Helpers.AlignBytes(rd.ReadBytes(Helpers.DecodeIntegerSize(rd)), traits.size_InvQ);

                rsa.ImportParameters(rsAparams);
                return rsa;
            }
            catch (Exception)
            {
                Debug.Assert(false);
                return null;
            }
            finally
            {
                rd.Close();
            }
        }

        public class Helpers
        {
            public enum PemStringType
            {
                Certificate,
                RsaPrivateKey
            }

            /// <summary>
            /// This helper function parses an integer size from the reader using the ASN.1 format
            /// </summary>
            /// <param name="rd"></param>
            /// <returns></returns>
            public static int DecodeIntegerSize(System.IO.BinaryReader rd)
            {
                byte byteValue;
                int count;

                byteValue = rd.ReadByte();
                if (byteValue != 0x02)        // indicates an ASN.1 integer value follows
                    return 0;

                byteValue = rd.ReadByte();
                if (byteValue == 0x81)
                {
                    count = rd.ReadByte();    // data size is the following byte
                }
                else if (byteValue == 0x82)
                {
                    byte hi = rd.ReadByte();  // data size in next 2 bytes
                    byte lo = rd.ReadByte();
                    count = BitConverter.ToUInt16(new[] { lo, hi }, 0);
                }
                else
                {
                    count = byteValue;        // we already have the data size
                }

                //remove high order zeros in data
                while (rd.ReadByte() == 0x00)
                {
                    count -= 1;
                }
                rd.BaseStream.Seek(-1, System.IO.SeekOrigin.Current);

                return count;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pemString"></param>
            /// <param name="type"></param>
            /// <returns></returns>
            public static byte[] GetBytesFromPEM(string pemString, PemStringType type)
            {
                string header; string footer;

                switch (type)
                {
                    case PemStringType.Certificate:
                        header = "-----BEGIN CERTIFICATE-----";
                        footer = "-----END CERTIFICATE-----";
                        break;
                    case PemStringType.RsaPrivateKey:
                        header = "-----BEGIN RSA PRIVATE KEY-----";
                        footer = "-----END RSA PRIVATE KEY-----";
                        break;
                    default:
                        return null;
                }

                int start = pemString.IndexOf(header) + header.Length;
                int end = pemString.IndexOf(footer, start) - start;
                return Convert.FromBase64String(pemString.Substring(start, end));
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="inputBytes"></param>
            /// <param name="alignSize"></param>
            /// <returns></returns>
            public static byte[] AlignBytes(byte[] inputBytes, int alignSize)
            {
                int inputBytesSize = inputBytes.Length;

                if ((alignSize != -1) && (inputBytesSize < alignSize))
                {
                    byte[] buf = new byte[alignSize];
                    for (int i = 0; i < inputBytesSize; ++i)
                    {
                        buf[i + (alignSize - inputBytesSize)] = inputBytes[i];
                    }
                    return buf;
                }
                else
                {
                    return inputBytes;      // Already aligned, or doesn't need alignment
                }
            }
        }

        internal class RSAParameterTraits
        {
            public RSAParameterTraits(int modulusLengthInBits)
            {
                // The modulus length is supposed to be one of the common lengths, which is the commonly referred to strength of the key,
                // like 1024 bit, 2048 bit, etc.  It might be a few bits off though, since if the modulus has leading zeros it could show
                // up as 1016 bits or something like that.
                int assumedLength = -1;
                double logbase = Math.Log(modulusLengthInBits, 2);
                if (logbase == (int)logbase)
                {
                    // It's already an even power of 2
                    assumedLength = modulusLengthInBits;
                }
                else
                {
                    // It's not an even power of 2, so round it up to the nearest power of 2.
                    assumedLength = (int)(logbase + 1.0);
                    assumedLength = (int)(Math.Pow(2, assumedLength));
                    System.Diagnostics.Debug.Assert(false);  // Can this really happen in the field?  I've never seen it, so if it happens
                    // you should verify that this really does the 'right' thing!
                }

                switch (assumedLength)
                {
                    case 1024:
                        this.size_Mod = 0x80;
                        this.size_Exp = -1;
                        this.size_D = 0x80;
                        this.size_P = 0x40;
                        this.size_Q = 0x40;
                        this.size_DP = 0x40;
                        this.size_DQ = 0x40;
                        this.size_InvQ = 0x40;
                        break;
                    case 2048:
                        this.size_Mod = 0x100;
                        this.size_Exp = -1;
                        this.size_D = 0x100;
                        this.size_P = 0x80;
                        this.size_Q = 0x80;
                        this.size_DP = 0x80;
                        this.size_DQ = 0x80;
                        this.size_InvQ = 0x80;
                        break;
                    case 4096:
                        this.size_Mod = 0x200;
                        this.size_Exp = -1;
                        this.size_D = 0x200;
                        this.size_P = 0x100;
                        this.size_Q = 0x100;
                        this.size_DP = 0x100;
                        this.size_DQ = 0x100;
                        this.size_InvQ = 0x100;
                        break;
                    default:
                        System.Diagnostics.Debug.Assert(false); // Unknown key size?
                        break;
                }
            }

            public int size_Mod = -1;
            public int size_Exp = -1;
            public int size_D = -1;
            public int size_P = -1;
            public int size_Q = -1;
            public int size_DP = -1;
            public int size_DQ = -1;
            public int size_InvQ = -1;
        }
    }
}
