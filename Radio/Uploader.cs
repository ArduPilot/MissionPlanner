using System;
using System.Threading;
using MissionPlanner;
using MissionPlanner.Comms;

namespace uploader
{
    public class Uploader
    {
        public enum Board : byte
        {
            // device IDs XXX should come with the firmware image...
            DEVICE_ID_RF50 = 0x4d,
            DEVICE_ID_HM_TRP = 0x4e,
            DEVICE_ID_RFD900 = 0X42,
            DEVICE_ID_RFD900A = 0X43,

            DEVICE_ID_RFD900U = 0X80 | 0x01,
            DEVICE_ID_RFD900P = 0x80 | 0x02,

            FAILED = 0x11
        }

        public enum Code : byte
        {
            // response codes
            OK = 0x10,
            FAILED = 0x11,
            INSYNC = 0x12,

            // protocol commands
            EOC = 0x20,
            GET_SYNC = 0x21,
            GET_DEVICE = 0x22, // returns DEVICE_ID and FREQ bytes
            CHIP_ERASE = 0x23,
            LOAD_ADDRESS = 0x24,
            PROG_FLASH = 0x25,
            READ_FLASH = 0x26,
            PROG_MULTI = 0x27,
            READ_MULTI = 0x28,
            REBOOT = 0x30

            // protocol constants
        }

        public enum Frequency : byte
        {
            // frequency code bytes XXX should come with the firmware image...
            FREQ_NONE = 0xf0,
            FREQ_433 = 0x43,
            FREQ_470 = 0x47,
            FREQ_868 = 0x86,
            FREQ_915 = 0x91,

            FAILED = 0x11
        }

        private bool banking;
        private int bytes_processed;

        private int bytes_to_process;
        private Frequency freq = Frequency.FAILED;
        private Board id = Board.FAILED;
        public ICommsSerial port;

        public int PROG_MULTI_MAX = 32; // maximum number of bytes in a PROG_MULTI command
        public int READ_MULTI_MAX = 255; // largest read that can be requested


        public event Sikradio.LogEventHandler LogEvent;
        public event Sikradio.ProgressEventHandler ProgressEvent;


        /// <summary>
        ///     Upload the specified image_data.
        /// </summary>
        /// <param name='image_data'>
        ///     Image_data to be uploaded.
        /// </param>
        public void upload(ICommsSerial on_port, IHex image_data, bool use_mavlink = false)
        {
            progress(0);

            port = on_port;

            try
            {
                connect_and_sync();
                upload_and_verify(image_data);
                cmdReboot();
            }
            catch
            {
                if (port.IsOpen)
                    port.Close();
                throw;
            }
        }

        public void connect_and_sync()
        {
            // configure the port
            port.ReadTimeout = 2000; // must be longer than full flash erase time (~1s)

            // synchronise with the bootloader
            //
            // The second sync attempt here is mostly laziness, though it does verify that we 
            // can send more than one packet.
            //
            for (var i = 0; i < 3; i++)
            {
                if (cmdSync())
                    break;
                log(string.Format("sync({0}) failed\n", i), 1);
            }
            if (!cmdSync())
            {
                log("FAIL: could not synchronise with the bootloader");
                throw new Exception("SYNC FAIL");
            }

            checkDevice();

            log("connected to bootloader\n");
        }

        private void upload_and_verify(IHex image_data)
        {
            if (image_data.bankingDetected && ((byte) id & 0x80) != 0x80)
            {
                log("This Firmware requires banking support");
                throw new Exception("This Firmware requires banking support");
            }

            if (((byte) id & 0x80) == 0x80)
            {
                banking = true;
                log("Using 24bit addresses");
            }

            // erase the program area first
            log("erasing program flash\n");
            cmdErase();

            // progress fractions
            bytes_to_process = 0;
            foreach (var bytes in image_data.Values)
            {
                bytes_to_process += bytes.Length;
            }
            bytes_to_process *= 2; // once to program, once to verify
            bytes_processed = 0;

            // program the flash blocks
            log("programming\n");
            foreach (var kvp in image_data)
            {
                // move the program pointer to the base of this block
                cmdSetAddress(kvp.Key);
                log(string.Format("prog 0x{0:X}/{1}\n", kvp.Key, kvp.Value.Length), 1);

                upload_block_multi(kvp.Value);
            }

            // and read them back to verify that they were programmed
            log("verifying\n");
            foreach (var kvp in image_data)
            {
                // move the program pointer to the base of this block
                cmdSetAddress(kvp.Key);
                log(string.Format("verf 0x{0:X}/{1}\n", kvp.Key, kvp.Value.Length), 1);

                verify_block_multi(kvp.Value);
                bytes_processed += kvp.Value.GetLength(0);
                progress((double) bytes_processed/bytes_to_process);
            }
            log("Success\n");
        }

        private void upload_block(byte[] data)
        {
            foreach (var b in data)
            {
                cmdProgram_Single(b);
                progress((double) ++bytes_processed/bytes_to_process);
            }
        }

        private void upload_block_multi(byte[] data)
        {
            var offset = 0;
            int to_send;
            var length = data.GetLength(0);

            // Chunk the block in units of no more than what the bootloader
            // will program.
            while (offset < length)
            {
                to_send = length - offset;
                if (to_send > PROG_MULTI_MAX)
                    to_send = PROG_MULTI_MAX;

                log(string.Format("multi {0}/{1}\n", offset, to_send), 1);
                cmdProgramMulti(data, offset, to_send);
                offset += to_send;

                bytes_processed += to_send;
                progress((double) bytes_processed/bytes_to_process);
            }
        }

        private void verify_block_multi(byte[] data)
        {
            var offset = 0;
            int to_verf;
            var length = data.GetLength(0);

            // Chunk the block in units of no more than what the bootloader
            // will read.
            while (offset < length)
            {
                to_verf = length - offset;
                if (to_verf > READ_MULTI_MAX)
                    to_verf = READ_MULTI_MAX;

                log(string.Format("multi {0}/{1}\n", offset, to_verf), 1);
                cmdVerifyMulti(data, offset, to_verf);
                offset += to_verf;

                bytes_processed += to_verf;
                progress((double) bytes_processed/bytes_to_process);
            }
        }

        /// <summary>
        ///     Requests a sync reply.
        /// </summary>
        /// <returns>
        ///     True if in sync, false otherwise.
        /// </returns>
        private bool cmdSync()
        {
            port.DiscardInBuffer();

            send(Code.GET_SYNC);
            send(Code.EOC);

            try
            {
                getSync();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Erases the device.
        /// </summary>
        private void cmdErase()
        {
            send(Code.CHIP_ERASE);
            send(Code.EOC);

            // sleep for 2 second - erase seems to take about 2 seconds
            Thread.Sleep(2000);

            getSync();
        }

        /// <summary>
        ///     Set the address for the next program or read operation.
        /// </summary>
        /// <param name='address'>
        ///     Address to be set.
        /// </param>
        private void cmdSetAddress(uint address)
        {
            if (banking)
            {
                send(Code.LOAD_ADDRESS);
                send((byte) (address & 0xff));
                send((byte) ((address >> 8) & 0xff));
                send((byte) ((address >> 16) & 0xff));
                send(Code.EOC);

                log("Bank Programming address " + (address >> 16));
            }
            else
            {
                send(Code.LOAD_ADDRESS);
                send((ushort) address);
                send(Code.EOC);
            }
            getSync();
        }

        /// <summary>
        ///     Programs a byte and advances the program address by one.
        /// </summary>
        /// <param name='data'>
        ///     Data to program.
        /// </param>
        private void cmdProgram_Single(byte data)
        {
            send(Code.PROG_FLASH);
            send(data);
            send(Code.EOC);

            getSync();
        }

        private void cmdProgramMulti(byte[] data, int offset, int length)
        {
            send(Code.PROG_MULTI);
            send((byte) length);
            //for (int i = 0; i < length; i++)
            //send (data [offset + i]);
            send(data, offset, length);
            send(Code.EOC);

            getSync();
        }

        /// <summary>
        ///     Verifies the byte at the current program address.
        /// </summary>
        /// <param name='data'>
        ///     Data expected to be found.
        /// </param>
        /// <exception cref='VerifyFail'>
        ///     Is thrown when the verify fail.
        /// </exception>
        private void cmdVerify(byte data)
        {
            send(Code.READ_FLASH);
            send(Code.EOC);

            if (recv() != data)
                throw new Exception("flash verification failed");

            getSync();
        }

        private void cmdVerifyMulti(byte[] data, int offset, int length)
        {
            send(Code.READ_MULTI);
            send((byte) length);
            send(Code.EOC);

            for (var i = 0; i < length; i++)
            {
                if (recv() != data[offset + i])
                {
                    log("flash verification failed\n");
                    throw new Exception("VERIFY FAIL");
                }
            }

            getSync();
        }

        private void cmdReboot()
        {
            send(Code.REBOOT);
        }

        private void checkDevice()
        {
            send(Code.GET_DEVICE);
            send(Code.EOC);

            id = (Board) recv();
            freq = (Frequency) recv();

            log("Connected to board " + id + " freq " + freq);

            // XXX should be getting valid board/frequency data from firmware file
            if ((id != Board.DEVICE_ID_HM_TRP) && (id != Board.DEVICE_ID_RF50) && (id != Board.DEVICE_ID_RFD900) &&
                (id != Board.DEVICE_ID_RFD900A))
                throw new Exception("bootloader device ID mismatch - device:" + id);

            getSync();
        }

        public void getDevice(ref Board device, ref Frequency freq)
        {
            send(Code.GET_DEVICE);
            send(Code.EOC);

            device = (Board) recv();
            freq = (Frequency) recv();

            getSync();
        }

        /// <summary>
        ///     Expect the two-byte synchronisation codes within the read timeout.
        /// </summary>
        /// <exception cref='NoSync'>
        ///     Is thrown if the wrong bytes are read.
        ///     <exception cref='TimeoutException'>
        ///         Is thrown if the read timeout expires.
        ///     </exception>
        private void getSync()
        {
            try
            {
                Code c;

                c = (Code) recv();
                if (c != Code.INSYNC)
                {
                    log(string.Format("got {0:X} when expecting {1:X}\n", (int) c, (int) Code.INSYNC), 2);
                    throw new Exception("BAD SYNC");
                }
                c = (Code) recv();
                if (c != Code.OK)
                {
                    log(string.Format("got {0:X} when expecting {1:X}\n", (int) c, (int) Code.EOC), 2);
                    throw new Exception("BAD STATUS");
                }
            }
            catch
            {
                log("FAIL: lost synchronisation with the bootloader\n");
                throw new Exception("SYNC LOST");
            }
            log("in sync\n", 5);
        }

        /// <summary>
        ///     Send the specified code to the bootloader.
        /// </summary>
        /// <param name='code'>
        ///     Code to send.
        /// </param>
        private void send(Code code)
        {
            byte[] b = {(byte) code};

            log("send ", 5);
            foreach (var x in b)
            {
                log(string.Format(" {0:X}", x), 5);
            }
            log("\n", 5);

            port.Write(b, 0, 1);
        }

        /// <summary>
        ///     Send the specified byte to the bootloader.
        /// </summary>
        /// <param name='data'>
        ///     Data byte to send.
        /// </param>
        private void send(byte data)
        {
            byte[] b = {data};

            log("send ", 5);
            foreach (var x in b)
            {
                log(string.Format(" {0:X}", x), 5);
            }
            log("\n", 5);

            while (port.BytesToWrite > 50)
            {
                var fred = 1;
                fred++;
                Console.WriteLine("slowdown");
            }
            port.Write(b, 0, 1);
        }

        private void send(byte[] data, int offset, int length)
        {
            while (port.BytesToWrite > 50)
            {
                var fred = 1;
                fred++;
                Console.WriteLine("slowdown");
            }
            port.Write(data, offset, length);
        }

        /// <summary>
        ///     Send the specified 16-bit value, LSB first.
        /// </summary>
        /// <param name='data'>
        ///     Data value to send.
        /// </param>
        private void send(ushort data)
        {
            var b = new byte[2] {(byte) (data & 0xff), (byte) (data >> 8)};

            log("send ", 5);
            foreach (var x in b)
            {
                log(string.Format(" {0:X}", x), 5);
            }
            log("\n", 5);

            port.Write(b, 0, 2);
        }

        /// <summary>
        ///     Receive a byte.
        /// </summary>
        private byte recv()
        {
            byte b;

            var Deadline = DateTime.Now.AddMilliseconds(port.ReadTimeout);

            while (DateTime.Now < Deadline && port.BytesToRead == 0)
            {
            }
            if (port.BytesToRead == 0)
                throw new Exception("Timeout");

            b = (byte) port.ReadByte();

            log(string.Format("recv {0:X}\n", b), 5);

            return b;
        }

        private void log(string message, int level = 0)
        {
            if (LogEvent != null)
                LogEvent(message, level);
        }

        private void progress(double completed)
        {
            if (ProgressEvent != null)
                ProgressEvent(completed);
        }
    }
}