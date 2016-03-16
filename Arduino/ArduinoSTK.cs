using System;
using System.Reflection;
using System.Threading;
using log4net;
using MissionPlanner.Comms;

// Written by Michael Oborne

namespace MissionPlanner.Arduino
{
    public class ArduinoSTK : SerialPort, IArduinoComms
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
            var a = 0;
            while (a < 50) // 50 tries at 50 ms = 2.5sec
            {
                DiscardInBuffer();
                Write(new[] {(byte) '0', (byte) ' '}, 0, 2);
                a++;
                Thread.Sleep(50);

                log.InfoFormat("connectap btr {0}", BytesToRead);
                if (BytesToRead >= 2)
                {
                    var b1 = (byte) ReadByte();
                    var b2 = (byte) ReadByte();
                    if (b1 == 0x14 && b2 == 0x10)
                    {
                        return true;
                    }
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
            ReadTimeout = 1000;
            var f = 0;
            while (BytesToRead < 1)
            {
                f++;
                Thread.Sleep(1);
                if (f > 1000)
                    return false;
            }
            var a = 0;
            while (a < 10)
            {
                if (BytesToRead >= 2)
                {
                    var b1 = (byte) ReadByte();
                    var b2 = (byte) ReadByte();
                    log.DebugFormat("bytes {0:X} {1:X}", b1, b2);

                    if (b1 == 0x14 && b2 == 0x10)
                    {
                        return true;
                    }
                }
                log.DebugFormat("btr {0}", BytesToRead);
                Thread.Sleep(10);
                a++;
            }
            return false;
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

            byte[] command = {(byte) 't', (byte) (length >> 8), (byte) (length & 0xff), (byte) 'E', (byte) ' '};
            Write(command, 0, command.Length);

            if (ReadByte() == 0x14)
            {
                // 0x14

                var step = 0;
                while (step < length)
                {
                    var chr = (byte) ReadByte();
                    data[step] = chr;
                    step++;
                }

                if (ReadByte() != 0x10) // 0x10
                    throw new Exception("Lost Sync 0x10");
            }
            else
            {
                throw new Exception("Lost Sync 0x14");
            }
            return data;
        }

        public byte[] downloadflash(short length)
        {
            if (!IsOpen)
            {
                throw new Exception("Port Not Open");
            }
            var data = new byte[length];

            ReadTimeout = 1000;

            byte[] command = {(byte) 't', (byte) (length >> 8), (byte) (length & 0xff), (byte) 'F', (byte) ' '};
            Write(command, 0, command.Length);

            if (ReadByte() == 0x14)
            {
                // 0x14

                int read = length;
                while (read > 0)
                {
                    //Console.WriteLine("offset {0} read {1}", length - read, read);
                    read -= Read(data, length - read, read);
                    //System.Threading.Thread.Sleep(1);
                }

                if (ReadByte() != 0x10) // 0x10
                    throw new Exception("Lost Sync 0x10");
            }
            else
            {
                throw new Exception("Lost Sync 0x14");
            }
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

                byte[] command = {(byte) 'd', (byte) (sending >> 8), (byte) (sending & 0xff), (byte) 'F'};
                Write(command, 0, command.Length);
                log.Info(startfrom + (length - totalleft) + " - " + sending);
                Write(data, startfrom + (length - totalleft), sending);
                command = new[] {(byte) ' '};
                Write(command, 0, command.Length);

                totalleft -= sending;


                if (Progress != null)
                    Progress((int) (startaddress/(float) length*100), "");

                if (!sync())
                {
                    log.Info("No Sync");
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

            log.Info("Sending address   " + (ushort) (address/2));

            address /= 2;
            address = (ushort) address;

            byte[] command = {(byte) 'U', (byte) (address & 0xff), (byte) (address >> 8), (byte) ' '};
            Write(command, 0, command.Length);

            return sync();
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

                if (sending == 0)
                    return true;

                setaddress(startaddress);
                startaddress += (short) sending;

                byte[] command = {(byte) 'd', (byte) (sending >> 8), (byte) (sending & 0xff), (byte) 'E'};
                Write(command, 0, command.Length);
                log.Info(startfrom + (length - totalleft) + " - " + sending);
                Write(data, startfrom + (length - totalleft), sending);
                command = new[] {(byte) ' '};
                Write(command, 0, command.Length);

                totalleft -= sending;

                if (!sync())
                {
                    log.Info("No Sync");
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

            byte[] command = {(byte) 'u', (byte) ' '};
            Write(command, 0, command.Length);

            Thread.Sleep(20);

            var chr = new byte[5];

            var count = Read(chr, 0, 5);
            log.Debug("getChipType read " + count);

            if (chr[0] == 0x14 && chr[4] == 0x10)
            {
                sig1 = chr[1];
                sig2 = chr[2];
                sig3 = chr[3];
            }

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

        public new bool Close()
        {
            try
            {
                byte[] command = {(byte) 'Q', (byte) ' '};
                Write(command, 0, command.Length);
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