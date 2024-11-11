using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using System.Security.Cryptography;
using Org.BouncyCastle.Security;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Threading;
using System.Xml;
using MissionPlanner.Comms;

namespace px4uploader
{
    public class Uploader: IDisposable
    {
        public delegate void LogEventHandler(string message, int level = 0);

        public delegate void ProgressEventHandler(double completed);

        public event LogEventHandler LogEvent;
        public event ProgressEventHandler ProgressEvent;

        public ICommsSerial port;
        Uploader self;

        public bool skipotp = false;

        public int board_type;
        public int board_rev;
        public int fw_maxsize;
        public int chip;
        public string chip_desc;
        public int bl_rev;
        public bool libre = false;
        public byte[] sn = new byte[0];

        public enum Code : byte
        {
            // response codes
            NOP = 0x00,
            OK = 0x10,
            FAILED = 0x11,
            INSYNC = 0x12,
            INVALID = 0x13,//	# rev3+

            BAD_SILICON_REV = 0x14,

            // protocol commands
            EOC = 0x20,
            GET_SYNC = 0x21,
            GET_DEVICE = 0x22,	// returns DEVICE_ID and FREQ bytes
            CHIP_ERASE = 0x23,
            CHIP_VERIFY = 0x24,//# rev2 only
            PROG_MULTI = 0x27,
            READ_MULTI = 0x28,//# rev2 only
            GET_CRC = 0x29,//	# rev3+
            GET_OTP = 0x2a, // read a byte from OTP at the given address 
            GET_SN = 0x2b,    // read a word from UDID area ( Serial)  at the given address 
            GET_CHIP = 0x2c, // read chip version (MCU IDCODE)
            SET_DELAY = 0x2d, // set minimum boot delay
            GET_CHIP_DES = 0x2e,
            REBOOT = 0x30,
            DEBUG = 0x31,
            SET_BAUD = 0x33,

            EXTF_ERASE = 0x34,    // Erase sectors from external flash
            EXTF_PROG_MULTI = 0x35,   // write bytes at external flash program address and increment
            EXTF_READ_MULTI = 0x36,   // read bytes at address and increment
            EXTF_GET_CRC = 0x37,	// compute & return a CRC of data in external flash
        }

        public enum Info {
            BL_REV = 1,//	# bootloader protocol revision
            BOARD_ID = 2,//	# board type
            BOARD_REV = 3,//	# board revision
            FLASH_SIZE = 4,//	# max firmware size in bytes
            VEC_AREA = 5,
            EXTF_SIZE = 6
        }

        public const byte BL_REV_MIN = 2;//	# minimum supported bootloader protocol 
        public const byte BL_REV_MAX = 20;//	# maximum supported bootloader protocol
        public const byte PROG_MULTI_MAX = 252;//		# protocol max is 255, must be multiple of 4
        public const byte READ_MULTI_MAX = 252;//		# protocol max is 255, something overflows with >= 64


        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct otp
        {
            public byte h1;
            public byte h2;
            public byte h3;
            public byte h4;
            public byte id_type;
            public uint vid;
            public uint pid;
            // 19 bytes
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 19)]
            public byte[] reserved;
            // 128 bytes
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public byte[] signature;
        }

        public int ReadTimeout { get { return port.ReadTimeout; } set { port.ReadTimeout = value; } }

        static Uploader()
        {
            readcerts();
        }

        public Uploader(ICommsSerial port)
        {
            self = this;

            this.port = port;
            this.port.ReadTimeout = 50;
            this.port.WriteTimeout = 50;

            try
            {
                this.port.Open();
            }
            catch (Exception ex)
            {
                try
                {
                    this.port.Close();
                }
                catch { }
                try
                {
                    this.Dispose();
                    GC.Collect();
                }
                catch { }
                throw ex;
            }
        }

        public Uploader(string port, int baudrate): this(new SerialPort(port, baudrate))
        {
        }

        public Uploader(string port) : this(new SerialPort(port))
        {
        }

        public void close()
        {
            try
            {
                port.BaseStream.Flush();
                port.Close();
            }
            catch { }
        }

        public static void ByteArrayToStructure(byte[] bytearray, ref object obj)
        {
            int len = Marshal.SizeOf(obj);
            IntPtr i = Marshal.AllocHGlobal(len);
            // create structure from ptr
            obj = Marshal.PtrToStructure(i, obj.GetType());
            // copy byte array to ptr
            Marshal.Copy(bytearray, 0, i, len);
            obj = Marshal.PtrToStructure(i, obj.GetType());
            Marshal.FreeHGlobal(i);
        }

        static List<KeyValuePair<string, string>> certs = new List<KeyValuePair<string, string>>();
        public int extf_maxsize;

        public static void readcerts()
        {
            string vendor = "";
            string publickey = "";

            var file = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) +
                       Path.DirectorySeparatorChar + @"validcertificates.xml";

            if (!File.Exists(file))
            {
                return;
            }

            using (XmlTextReader xmlreader = new XmlTextReader(file))
            {
                while (xmlreader.Read())
                {
                    if (xmlreader.ReadToFollowing("CERTIFICATE"))
                    {
                        var xmlreader2 = xmlreader.ReadSubtree();

                        while (xmlreader2.Read())
                        {
                            switch (xmlreader2.Name)
                            {
                                case "VENDOR":
                                    vendor = xmlreader2.ReadString();
                                    break;
                                case "PUBLICKEY":
                                    publickey = xmlreader2.ReadString();
                                    break;
                            }
                        }

                        if (vendor != "")
                        {
                            certs.Add(new KeyValuePair<string, string>(vendor, publickey));

                            vendor = "";
                            publickey = "";
                        }
                    }
                }
            }
        }

        bool ByteArrayCompare(byte[] a1, byte[] a2)
        {
            if (a1.Length != a2.Length)
                return false;

            for (int i = 0; i < a1.Length; i++)
                if (a1[i] != a2[i])
                    return false;

            return true;
        }

        public bool verifyotp()
        {
            if (skipotp)
                return true;
            // check if is a fmuv2 and bootloader >= 4 else fail;
            // 9 = fmuv2
            // 5 = px4 1.x
            if (board_type == 9) // &&up.bl_rev >= 4
            {
                try
                {
                    // get the device sn
                    byte[] sn = __get_sn();

                    string line = "";

                    line="SN: ";
                    for (int s = 0; s < sn.Length; s += 1)
                    {
                        line += sn[s].ToString("X2");
                    }
                    print(line);

                    // 20 bytes - sha1
                    Array.Resize(ref sn, 20);

                    if (ByteArrayCompare(sn, new byte[] { 0x00, 0x23, 0x00, 0x30, 0x35, 0x32, 0x47, 0x18, 0x36, 0x34, 0x30, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }))
                    {
                        print("Libre bootloader");
                        libre = true;
                        print("Forged Key");
                        throw new InvalidKeyException("Invalid Board");
                    }

                    if (ByteArrayCompare(sn, new byte[] { 0x00, 0x38, 0x00, 0x1F, 0x34, 0x32, 0x47, 0x0D, 0x31, 0x32, 0x35, 0x33, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }))
                    { // pixhawk lite
                        // please sign your board via the proper process.
                        // nuttx has an auth command. use it.
                        print("Forged Key");
                        throw new InvalidKeyException("Invalid Board");
                    }

                    if (ByteArrayCompare(sn, new byte[] { 0x00, 0x38, 0x00, 0x21, 0x31, 0x34, 0x51, 0x17, 0x33, 0x36, 0x38, 0x34, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 }))
                    { // pixfalcon
                        print("Forged Key");
                        throw new InvalidKeyException("Invalid Board");
                    }

                    object obj = new otp();
                    byte[] test = __read_otp();

                    ByteArrayToStructure(test, ref obj);

                    otp otp = (otp)obj;

                    print("id: " + otp.id_type.ToString("X"));
                    print("vid: " + otp.vid.ToString("X"));
                    print("pid: " + otp.pid.ToString("X"));

                    if (otp.h1 == 'P' &&
                        otp.h2 == 'X' &&
                        otp.h3 == '4' &&
                        otp.h4 == '\0')
                    {
                        // no vendor checks yet
                        byte[] sig = otp.signature;

                        line = "";

                        for (int s = 0; s < 512; s += 1)
                        {
                            line += test[s].ToString("X2");
                            if (s % 16 == 15)
                            {
                                print(line);
                                line = "";
                            }
                        }

                        /*
                                                            byte[] PEMbuffer = Convert.FromBase64String(@"");
                                                            */
                        //   RSACryptoServiceProvider rsa = DecodeRsaPrivateKey(PEMbuffer);

                        //   RSAParameters rsapublic = rsa.ExportParameters(false);

                        foreach (var cert in certs)
                        {
                            byte[] pubpem = Convert.FromBase64String(cert.Value);

                            AsymmetricKeyParameter asymmetricKeyParameter = PublicKeyFactory.CreateKey(pubpem);
                            RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)asymmetricKeyParameter;
                            RSAParameters rsaParameters = new RSAParameters();
                            rsaParameters.Modulus = rsaKeyParameters.Modulus.ToByteArrayUnsigned();
                            rsaParameters.Exponent = rsaKeyParameters.Exponent.ToByteArrayUnsigned();
                            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
                            rsa.ImportParameters(rsaParameters);

                            bool valid = rsa.VerifyHash(sn, CryptoConfig.MapNameToOID("SHA1"), otp.signature);

                            if (valid)
                            {
                                print("Valid Key");
                                return true;
                            }
                        }

                        print("Invalid Key");
                        throw new InvalidKeyException("Invalid Board");
                    }
                    else
                    {
                        print("Failed Header Check");
                        throw new FormatException("Failed Header Check");
                    }

                }
                catch 
                {
                    print("Failed to read Certificate of Authenticity");
                    throw;
                }
            }

            // not board type 9
            return true;
        }

        public byte[] __read_otp()
        {
            byte[] otp = new byte[512];
            int addr = 0;
            while (addr < 512)
            {
                __send(new byte[] { (byte)Code.GET_OTP });
                __send(BitConverter.GetBytes(addr));
                __send(new byte[] { (byte)Code.EOC });
                byte[] ans = __recv(4);
                __getSync();
                Array.Copy(ans, 0, otp, addr, 4);
                addr += 4;
            }
            return otp;
        }

        public byte[] __get_sn()
        {
            byte[] sn = new byte[3*4];

            for (int a = 0; a < 12; a+=4)
            {
                __send(new byte[] { (byte)Code.GET_SN });
                __send(BitConverter.GetBytes(a));
                __send(new byte[] { (byte)Code.EOC });
                byte[] ans = __recv(4);
                __getSync();
                //ans = ans.Reverse().ToArray();
                Array.Copy(ans, 0, sn, a, 4);
            }

            return sn;
        }

        public int __getCHIP()
        {
            __send(new byte[] { (byte)Code.GET_CHIP, (byte)Code.EOC });
            int info = __recv_int();
            __getSync();
            return info;
        }

        public string __getCHIPDES()
        {
            __send(new byte[] { (byte)Code.GET_CHIP_DES, (byte)Code.EOC });
            int len = __recv_int();
            if (len > 0)
            {
                var bytes = __recv(len);
                __getSync();
                return ASCIIEncoding.ASCII.GetString(bytes);
            } else
            {
                __getSync();
                return "";
            }
        }

        public void __send(byte c)
        {
            //Console.WriteLine("__send " + c.ToString("X"));
            port.Write(new byte[] { c }, 0, 1);
        }

        public void __send(byte[] c)
        {
            //foreach (var i in c)
            //    Console.WriteLine("__send " + i.ToString("X"));
            port.Write(c, 0, c.Length);
        }

        public byte[] __recv(int count = 1)
        {
            // this will auto timeout
            // Console.WriteLine("recv "+count);
            byte[] c = new byte[count];
            int pos = 0;
            while (pos < count)
            {
                pos += port.Read(c, pos, count - pos);
                if (count >= 4 && c[0] == (byte)Code.INSYNC && c[1] == (byte)Code.INVALID)
                    throw new Exception("Bad Request INSYNC INVALID");
            }

            //foreach (var i in c)
            //    Console.WriteLine("__recv " + i.ToString("X"));

            return c;
        }

        public int __recv_int()
        {
            byte[] raw = __recv(4);
            int val = BitConverter.ToInt32(raw, 0);
            return val;
        }

        public void __getSync()
        {
            port.BaseStream.Flush();

            var deadline = DateTime.Now.AddMilliseconds(ReadTimeout);
            while(port.BytesToRead == 0)
            {
                if (DateTime.Now > deadline)
                    throw new TimeoutException("timeout waiting for response");
                Thread.Yield();
            }

            byte c = __recv()[0];
            if (c != (byte)Code.INSYNC)
                throw new Exception(string.Format("unexpected {0:X} instead of INSYNC", (byte)c));
            c = __recv()[0];
            if (c == (byte)Code.INVALID)
                throw new Exception(string.Format("bootloader reports INVALID OPERATION", (byte)c));
            if (c == (byte)Code.FAILED)
                throw new Exception(string.Format("bootloader reports OPERATION FAILED", (byte)c));
            if (c != (byte)Code.OK)
                throw new Exception(string.Format("unexpected {0:X} instead of OK", (byte)c));
        }

        public void __sync()
        {
            port.BaseStream.Flush();
            __send(new byte[] { (byte)Code.GET_SYNC, (byte)Code.EOC });
            __getSync();
        }

        public bool __syncAttempt()
        {
            port.BaseStream.Flush();
            __send(new byte[] { (byte)Code.GET_SYNC, (byte)Code.EOC });
            return __trySync();
        }

        public bool __trySync()
        {
            port.BaseStream.Flush();
            byte c = __recv()[0];
            if (c != (byte)Code.INSYNC)
                return false;

            c = __recv()[0];
            if (c != (byte)Code.OK)
                return false;

            return true;
        }

        public int __getInfo(Info param)
        {
            __send(new byte[] { (byte)Code.GET_DEVICE, (byte)param, (byte)Code.EOC });
            int info = __recv_int();
            __getSync();
            return info;
        }

        public void __erase()
        {
            __sync();

            // fix for bootloader bug - must see a sync and a get_device
            __getInfo(Info.BL_REV);

            __send(new byte[] { (byte)Code.CHIP_ERASE, (byte)Code.EOC });
            __wait_for_bytes(1, 20);

            __getSync();
        }

        public void __erase_extf(Firmware fw)
        {
            __sync();

            // fix for bootloader bug - must see a sync and a get_device
            __getInfo(Info.BL_REV);

            byte[] size_bytes = BitConverter.GetBytes(fw.extf_image_size);

            __send(new byte[] {(byte)Code.EXTF_ERASE,
                    size_bytes[0], size_bytes[1], size_bytes[2], size_bytes[3],
                    (byte) Code.EOC});

            __getSync();

            int last_pct = 0;
            while (true)
            {
                if (last_pct < 95)
                {
                    int pct = __recv()[0];
                    if (last_pct != pct)
                    {
                        last_pct = pct;

                        if (ProgressEvent != null)
                            ProgressEvent(pct);
                    }
                }
                else if (__trySync())
                {
                    if (ProgressEvent != null)
                        ProgressEvent(100);

                    break;
                }
            }
        }

        public void __program_multi_extf(byte[] data)
        {
            var length = (byte)data.Length;

            __send((byte)Code.EXTF_PROG_MULTI);
            __send(length);
            __send(data);
            __send((byte)Code.EOC);
            __getSync();
        }

        public void __program_multi(byte[] data)
        {
            __send(new byte[] { (byte)Code.PROG_MULTI, (byte)data.Length });
            __send(data);
            __send((byte)Code.EOC);
            __getSync();
        }

        public bool __verify_multi(byte[] data)
        {
            self.__send(new byte[] { (byte)Code.READ_MULTI, chr(len(data)), (byte)Code.EOC });
            byte[] programmed = self.__recv(len(data));
            if (!isMatch(programmed,data))
          {
                print("got    " + hexlify(programmed));
                print("expect " + hexlify(data));
                return false;
            }
            self.__getSync();
            return true;
        }

        bool __wait_for_bytes(int num_bytes, int timeout_secs)
        {
            DateTime deadline = DateTime.Now.AddSeconds(timeout_secs);
            while (DateTime.Now < deadline)
            {
                port.BaseStream.Flush();
                if (port.BytesToRead >= num_bytes)
                {
                    return true;
                }
                Thread.Yield();
            }
            return false;
        }

        bool isMatch(byte[] d1, byte[] d2)
        {
            if (d1.Length != d2.Length)
                return false;

            int a = 0;
            foreach (byte by in d1)
            {
                if (by != d2[a]) {
                    return false;
                }
                    a++;
            }
            return true;
        }

        string hexlify(byte[] data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte by in data) {
                sb.Append(string.Format("{0,2:X}",by));
            }
            return sb.ToString();
        }

        public void __reboot()
        {
            // silent fail
            try
            {
                self.__send(new byte[] { (byte)Code.REBOOT, (byte)Code.EOC });
                self.port.DiscardInBuffer();
            }
            catch { }
        }

        public List<Byte[]> __split_len(byte[] seq, int length)
        {
            List<Byte[]> answer = new List<byte[]>();
            int size = length;
            for (int a = 0; a < seq.Length;)
            {
                byte[] ba = new byte[size];
               // Console.WriteLine(a + " " + seq.Length +" " + size);
                Array.Copy(seq, a, ba, 0, size);
                answer.Add(ba);
                a += size;
                if ((seq.Length - a) < size)
                    size = seq.Length - a;
            }
            return answer;
        }

        public void __program(Firmware fw)
        {
            byte[] code = fw.imagebyte;
            List<byte[]> groups = self.__split_len(code, PROG_MULTI_MAX);
            int a = 1;
            foreach (Byte[] bytes in groups)
            {
                self.__program_multi(bytes);

                System.Diagnostics.Debug.WriteLine("Program {0}/{1}", a, groups.Count);

                a++;
                if (ProgressEvent != null)
                    ProgressEvent((a / (float)groups.Count) * 100.0);
            }
        }

        public void __program_extf(Firmware fw)
        {
            byte[] code = fw.extf_imagebyte;
            List<byte[]> groups = self.__split_len(code, PROG_MULTI_MAX);
            int a = 1;
            foreach (Byte[] bytes in groups)
            {
                self.__program_multi_extf(bytes);

                System.Diagnostics.Debug.WriteLine("Program extf {0}/{1}", a, groups.Count);

                a++;
                if (ProgressEvent != null)
                    ProgressEvent((a / (float)groups.Count) * 100.0);
            }
        }

        public void __verify_v2(Firmware fw)
        {
            self.__send(new byte[] {(byte)Code.CHIP_VERIFY
				, (byte)Code.EOC});
            self.__getSync();
            byte[] code = fw.imagebyte;
            List<byte[]> groups = self.__split_len(code, READ_MULTI_MAX);
            int a = 1;
            foreach (byte[] bytes in groups)
            {
                if (!self.__verify_multi(bytes))
                {
                    throw new Exception("Verification failed");
                }

                System.Diagnostics.Debug.WriteLine("Verify {0}/{1}", a, groups.Count);

                a++;
                if (ProgressEvent != null)
                    ProgressEvent((a / (float)groups.Count) * 100.0);
            }
        }

        public void __verify_v3(Firmware fw)
        {

            //Expected 0x33E22A51
            //Got      0x8117524C
            //System.Exception: Program CRC failed


            //python
            //Expected 0x4c521781
            //Got      0x4c521781

            int expect_crc = fw.crc(self.fw_maxsize);
            __send(new byte[] {(byte)Code.GET_CRC,
				(byte) Code.EOC});
            int report_crc = __recv_int();
            __getSync();

            print("Expected 0x" + hexlify(BitConverter.GetBytes(expect_crc)) + " " + expect_crc);
            print("Got      0x" + hexlify(BitConverter.GetBytes(report_crc)) + " " + report_crc);
            if (report_crc != expect_crc)
            {
                throw new Exception("Program CRC failed");
            }
        }

        public void __verify_extf(Firmware fw)
        {
            int expect_crc = fw.extf_crc(fw.extf_image_size);

            byte[] size_bytes = BitConverter.GetBytes(fw.extf_image_size);

            __send(new byte[] {(byte)Code.EXTF_GET_CRC,
                    size_bytes[0], size_bytes[1], size_bytes[2], size_bytes[3],
                    (byte) Code.EOC});

            DateTime deadline = DateTime.Now.AddSeconds(10);
            while (DateTime.Now < deadline)
            {
                port.BaseStream.Flush();
                if (port.BytesToRead >= 4)
                {
                    if (ProgressEvent != null)
                        ProgressEvent(100);
                    break;
                }
                if (ProgressEvent != null)
                {
                    TimeSpan ts = new TimeSpan(deadline.Ticks - DateTime.Now.Ticks);
                    ProgressEvent(((10.0 - ts.TotalSeconds) / 10.0) * 100.0);
                }
                Thread.Yield();
            }

            __wait_for_bytes(4, 20);

            int report_crc = __recv_int();
            __getSync();

            print("extf Expected 0x" + hexlify(BitConverter.GetBytes(expect_crc)) + " " + expect_crc);
            print("extf Got      0x" + hexlify(BitConverter.GetBytes(report_crc)) + " " + report_crc);
            if (report_crc != expect_crc)
            {
                throw new Exception("Program extf CRC failed");
            }
        }

        
        public void currentChecksum(Firmware fw)
        {
            if (self.bl_rev < 3)
                return;

            __sync();

            this.port.ReadTimeout = 1000; // 1 sec

            bool sameflash = true;
            bool sameexternalflash = true;

            if (self.fw_maxsize > 0)
            {
                int expect_crc = fw.crc(self.fw_maxsize);
                __send(new byte[] {(byte)Code.GET_CRC,
                    (byte) Code.EOC});
                int report_crc = __recv_int();
                self.__getSync();

                print("FW File 0x" + hexlify(BitConverter.GetBytes(expect_crc)) + " " + expect_crc);
                print("Current 0x" + hexlify(BitConverter.GetBytes(report_crc)) + " " + report_crc);

                if (expect_crc != report_crc)
                    sameflash = false;                    
            }

            if (self.extf_maxsize > 0)
            {
                int expect_crc = fw.extf_crc(fw.extf_image_size);

                byte[] size_bytes = BitConverter.GetBytes(fw.extf_image_size);

                __send(new byte[] {(byte)Code.EXTF_GET_CRC,
                    size_bytes[0], size_bytes[1], size_bytes[2], size_bytes[3],
                    (byte) Code.EOC});

                // crc can be slow, give it 10s
                __wait_for_bytes(4, 30);

                int report_crc = __recv_int();
                self.__getSync();

                print("Ext FW File 0x" + hexlify(BitConverter.GetBytes(expect_crc)) + " " + expect_crc);
                print("Current 0x" + hexlify(BitConverter.GetBytes(report_crc)) + " " + report_crc);

                if (expect_crc != report_crc)
                    sameexternalflash = false;
            }

            if (sameflash && sameexternalflash)
                throw new Exception("Same Firmware. Not uploading");
        }

        public void identify()
        {
            port.DiscardInBuffer();

            //Console.WriteLine("0 " + DateTime.Now.Millisecond);
            // make sure we are in sync before starting
            self.__sync();

            //Console.WriteLine("1 "+DateTime.Now.Millisecond);

            //get the bootloader protocol ID first
            self.bl_rev = self.__getInfo(Info.BL_REV);

           // Console.WriteLine("2 " + DateTime.Now.Millisecond);
            if ((bl_rev < (int)BL_REV_MIN) || (bl_rev > (int)BL_REV_MAX))
            {
                throw new Exception("Bootloader protocol mismatch");
            }

            print("Got BL Info - changing timeout");

            // revert to default write timeout
            port.WriteTimeout = 500;

            self.board_type = self.__getInfo(Info.BOARD_ID);
            self.board_rev = self.__getInfo(Info.BOARD_REV);
            self.fw_maxsize = self.__getInfo(Info.FLASH_SIZE);

            if (bl_rev >= 5)
            {
                try
                {
                    self.chip = self.__getCHIP();
                    self.chip_desc = self.__getCHIPDES();
                } catch { __sync(); }
            }

            if (bl_rev >= 5)
            {
                try
                {
                    self.sn = self.__get_sn();
                }
                catch { __sync(); }
            }

            if (bl_rev >= 5)
            {
                try
                {
                    self.extf_maxsize = __getInfo(Info.EXTF_SIZE);
                }
                catch { __sync(); }
            }

            var line = string.Format(
                "Found board type {0} brdrev {1} blrev {2} fwmax {3} extf {7} chip {5:X} chipdes {6} SN {8} on {4}",
                board_type,
                board_rev, bl_rev, fw_maxsize, port, chip, chip_desc, extf_maxsize,
                BitConverter.ToString(self.sn).Replace("-", string.Empty));

            print(line);
        }

        public void upload(Firmware fw)
        {
            this.port.ReadTimeout = 1000; // 1 sec

            //Make sure we are doing the right thing
            if (self.board_type != fw.board_id)
            {
                if (!(self.board_type == 33 && fw.board_id == 9))
                    throw new Exception("Firmware not suitable for this board fw:" + fw.board_id + " - board:" +
                                        self.board_type);
            }

            if (self.fw_maxsize < fw.image_size && self.fw_maxsize != 0)
                throw new Exception("Firmware image is too large for this board");

            if (self.extf_maxsize < fw.extf_image_size && self.extf_maxsize != 0)
                throw new Exception("extf image is too large for this board");

            // erasing the internal flash will prevent a case where a unplug during external flashing will cause bootloader mode on reboot as there i no valid internal flash. even if the external was valid.
            if (fw.image_size > 0)
            {
                print("erase...");
                self.__erase();
            }

            // external first - second internal
            if (fw.extf_image_size > 0)
            {
                print("erase extf...");
                self.__erase_extf(fw);
                print("program extf...");
                self.__program_extf(fw);
                print("verify extf...");
                self.__verify_extf(fw);
            }

            if (fw.image_size > 0)
            {
                print("program...");
                self.__program(fw);

                print("verify...");
                if (self.bl_rev == 2)
                    self.__verify_v2(fw);
                else
                    self.__verify_v3(fw);
            }

            print("done, rebooting.");
            self.__reboot();
            try
            {
                self.port.Close();
            }
            catch { }
        }

        public int len(byte[] data)
        {
            return data.Length;
        }

        public byte chr(int chr)
        {
            return (byte)chr;
        }

        public void print(string str)
        {
            Console.WriteLine(str);
            if (LogEvent != null)
                LogEvent(str,0);
        }

        public void Dispose()
        {
            try
            {
                this.port.Close();
            }
            catch { }

            try
            {
                this.port.Dispose();
            }
            catch { }            

            this.port = null;
        }
    }
}
