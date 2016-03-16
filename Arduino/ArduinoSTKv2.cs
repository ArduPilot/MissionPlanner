using System;
using System.Reflection;
using System.Threading;
using log4net;
using MissionPlanner.Comms;

// Written by Michael Oborne

namespace MissionPlanner.Arduino
{
    public class ArduinoSTKv2 : SerialPort, IArduinoComms
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public event ProgressEventHandler Progress;

        public new void Open()
        {
            // default dtr status is false

            //from http://svn.savannah.nongnu.org/viewvc/RELEASE_5_11_0/arduino.c?root=avrdude&view=markup
            base.Open();

            DtrEnable = false;
            RtsEnable = false;

            Thread.Sleep(50);

            DtrEnable = true;
            RtsEnable = true;

            Thread.Sleep(50);
        }


        /// <summary>
        ///     Used to start initial connecting after serialport.open
        /// </summary>
        /// <returns>true = passed, false = failed</returns>
        public bool connectAP()
        {
            if (!IsOpen)
            {
                return false;
            }

            Thread.Sleep(100);

            var a = 0;
            while (a < 5)
            {
                byte[] temp = {0x6, 0, 0, 0, 0};
                temp = genstkv2packet(temp);
                a++;
                Thread.Sleep(50);

                try
                {
                    if (temp[0] == 6 && temp[1] == 0 && temp.Length == 2)
                    {
                        return true;
                    }
                }
                catch
                {
                }
            }
            return false;
        }

        /// <summary>
        ///     Used to keep alive the connection
        /// </summary>
        /// <returns>true = passed, false = lost connection</returns>
        public bool keepalive()
        {
            return connectAP();
        }

        /// <summary>
        ///     Syncs after a private command has been sent
        /// </summary>
        /// <returns>true = passed, false = failed</returns>
        public bool sync()
        {
            if (!IsOpen)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Downloads the eeprom with the given length - set Address first
        /// </summary>
        /// <param name="length">eeprom length</param>
        /// <returns>downloaded data</returns>
        public byte[] download(short length)
        {
            if (!IsOpen)
            {
                throw new Exception();
            }
            var data = new byte[length];

            byte[] temp = {0x16, (byte) ((length >> 8) & 0xff), (byte) ((length >> 0) & 0xff)};
            temp = genstkv2packet(temp);

            Array.Copy(temp, 2, data, 0, length);

            return data;
        }

        public byte[] downloadflash(short length)
        {
            if (!IsOpen)
            {
                throw new Exception("Port Closed");
            }
            var data = new byte[length];

            byte[] temp = {0x14, (byte) ((length >> 8) & 0xff), (byte) ((length >> 0) & 0xff)};
            temp = genstkv2packet(temp);

            Array.Copy(temp, 2, data, 0, length);

            return data;
        }

        public bool uploadflash(byte[] data, int startfrom, int length, int startaddress)
        {
            if (!IsOpen)
            {
                return false;
            }
            var loops = length/0x100;
            var totalleft = length;
            var sending = 0;

            for (var a = 0; a <= loops; a++)
            {
                if (totalleft > 0x100)
                {
                    sending = 0x100;
                }
                else
                {
                    sending = totalleft;
                }

                //startaddress = 256;
                if (sending == 0)
                    return true;

                setaddress(startaddress);
                startaddress += sending;

                // 0x13          

                byte[] command = {0x13, (byte) (sending >> 8), (byte) (sending & 0xff)};

                log.InfoFormat(startfrom + (length - totalleft) + " - " + sending);

                Array.Resize(ref command, sending + 10); // sending + head

                Array.Copy(data, startfrom + (length - totalleft), command, 10, sending);

                command = genstkv2packet(command);

                totalleft -= sending;


                if (Progress != null)
                    Progress((int) (startaddress/(float) length*100), "Uploading Firmware");

                if (command[1] != 0)
                {
                    log.InfoFormat("No Sync");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Sets the eeprom start read or write address
        /// </summary>
        /// <param name="address">address, must be eaven number</param>
        /// <returns>true = passed, false = failed</returns>
        public bool setaddress(int address)
        {
            if (!IsOpen)
            {
                return false;
            }

            if (address%2 == 1)
            {
                throw new Exception("Address must be an even number");
            }

            log.InfoFormat("Sending address   " + address/2);

            var tempstart = address/2; // words
            byte[] temp =
            {
                0x6, (byte) ((tempstart >> 24) & 0xff), (byte) ((tempstart >> 16) & 0xff),
                (byte) ((tempstart >> 8) & 0xff),
                (byte) ((tempstart >> 0) & 0xff)
            };
            temp = genstkv2packet(temp);

            if (temp[1] == 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Upload data at preset address
        /// </summary>
        /// <param name="data">array to read from</param>
        /// <param name="startfrom">start array index</param>
        /// <param name="length">length to send</param>
        /// <param name="startaddress">sets eeprom start programing address</param>
        /// <returns>true = passed, false = failed</returns>
        public bool upload(byte[] data, short startfrom, short length, short startaddress)
        {
            if (!IsOpen)
            {
                return false;
            }
            var loops = length/0x100;
            int totalleft = length;
            var sending = 0;

            for (var a = 0; a <= loops; a++)
            {
                if (totalleft > 0x100)
                {
                    sending = 0x100;
                }
                else
                {
                    sending = totalleft;
                }

                //startaddress = 256;
                if (sending == 0)
                    return true;

                setaddress(startaddress);
                startaddress += (short) sending;

                // 0x13          

                byte[] command = {0x15, (byte) (sending >> 8), (byte) (sending & 0xff)};

                log.InfoFormat(startfrom + (length - totalleft) + " - " + sending);

                Array.Resize(ref command, sending + 10); // sending + head

                Array.Copy(data, startfrom + (length - totalleft), command, 10, sending);

                command = genstkv2packet(command);

                totalleft -= sending;


                if (Progress != null)
                    Progress((int) (startaddress/(float) length*100), "");

                if (command[1] != 0)
                {
                    log.InfoFormat("No Sync");
                    return false;
                }
            }
            return true;
        }

        public Chip getChipType()
        {
            byte sig1 = 0x00;
            byte sig2 = 0x00;
            byte sig3 = 0x00;

            byte[] command = {0x1b, 0, 0, 0, 0};
            command = genstkv2packet(command);

            sig1 = command[2];

            command = new byte[] {0x1b, 0, 0, 0, 1};
            command = genstkv2packet(command);

            sig2 = command[2];

            command = new byte[] {0x1b, 0, 0, 0, 2};
            command = genstkv2packet(command);

            sig3 = command[2];

            foreach (var item in Chip.chips)
            {
                if (item.Equals(new Chip("", sig1, sig2, sig3, 0)))
                {
                    log.Debug("Match " + item);
                    return item;
                }
            }

            return null;
        }

        public byte[] genstkv2packet(byte[] message)
        {
            var data = new byte[300];
            byte ck = 0;

            data[0] = 0x1b;
            ck ^= data[0];
            data[1] = 0x1;
            ck ^= data[1];
            data[2] = (byte) ((message.Length >> 8) & 0xff);
            ck ^= data[2];
            data[3] = (byte) (message.Length & 0xff);
            ck ^= data[3];
            data[4] = 0xe;
            ck ^= data[4];

            var a = 5;
            foreach (var let in message)
            {
                data[a] = let;
                ck ^= let;
                a++;
            }
            data[a] = ck;
            a++;

            Write(data, 0, a);
            //Console.WriteLine("about to read packet");

            var ret = readpacket();

            //if (ret[1] == 0x0)
            {
                //Console.WriteLine("received OK");
            }

            return ret;
        }

        private byte[] readpacket()
        {
            var temp = new byte[4000];
            var mes = new byte[2] {0x0, 0xC0}; // fail
            var a = 7;
            var count = 0;

            ReadTimeout = 1000;

            while (count < a && a < temp.Length)
            {
                //Console.WriteLine("count {0} a {1} mes leng {2}",count,a,mes.Length);
                try
                {
                    temp[count] = (byte) ReadByte();
                }
                catch
                {
                    break;
                }


                //Console.Write("{1}", temp[0], (char)temp[0]);

                if (temp[0] != 0x1b)
                {
                    count = 0;
                    continue;
                }

                if (count == 3)
                {
                    a = (temp[2] << 8) + temp[3];
                    mes = new byte[a];
                    a += 5;
                }

                if (count >= 5)
                {
                    mes[count - 5] = temp[count];
                }

                count++;
            }

            //Console.WriteLine("read ck");
            try
            {
                temp[count] = (byte) ReadByte();
            }
            catch
            {
            }

            count++;

            Array.Resize(ref temp, count);

            //Console.WriteLine(this.BytesToRead);

            return mes;
        }

        public new bool Close()
        {
            try
            {
                byte[] command = {0x11};
                genstkv2packet(command);
            }
            catch
            {
            }

            try
            {
                if (IsOpen)
                    base.Close();
            }
            catch
            {
            }

            DtrEnable = false;
            RtsEnable = false;
            return true;
        }
    }
}