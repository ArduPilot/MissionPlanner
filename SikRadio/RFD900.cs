using System;
using System.Threading;

namespace RFD.RFD900
{
    /// <summary>
    /// For detecting which mode the modem is in, and putting it in to desired modes.
    /// </summary>
    public class TSession
    {
        TMode _Mode = TMode.INIT;
        MissionPlanner.Comms.ICommsSerial _Port;
        const int BOOTLOADER_BAUD = 115200;

        public TSession(MissionPlanner.Comms.ICommsSerial Port)
        {
            _Port = Port;
        }

        public enum TMode
        {
            INIT,
            TRANSPARENT,
            AT_COMMAND,
            BOOTLOADER,     //For a,u,+
            BOOTLOADER_X,    //For x
            UNKNOWN
        }

        public bool WaitForToken(string Token, int MaxWait)
        {
            System.Diagnostics.Stopwatch SW = new System.Diagnostics.Stopwatch();
            SW.Start();
            string Temp = "";
            int Timeout = _Port.ReadTimeout;
            _Port.ReadTimeout = MaxWait;

            while (SW.ElapsedMilliseconds < MaxWait)
            {
                try
                {
                    Temp += (char)_Port.ReadByte();
                }
                catch
                {
                    _Port.ReadTimeout = Timeout;
                    return false;
                }
                if (Temp.Contains(Token))
                {
                    return true;
                }
                //Thread.Sleep(100);
            }
            return false;
        }

        void WriteBootloaderCode(uploader.Uploader.Code Code)
        {
            byte[] ToSend = new byte[] { (byte)Code };
            _Port.Write(ToSend, 0, 1);
        }

        bool IsInBootloaderMode()
        {
            int PrevBaud = _Port.BaudRate;
            _Port.BaudRate = BOOTLOADER_BAUD;
            Thread.Sleep(100);
            WriteBootloaderCode(uploader.Uploader.Code.EOC);
            Thread.Sleep(100);
            _Port.DiscardInBuffer();
            WriteBootloaderCode(uploader.Uploader.Code.GET_SYNC);
            WriteBootloaderCode(uploader.Uploader.Code.EOC);
            bool Result;
            try
            {
                Result = (_Port.ReadByte() == (int)uploader.Uploader.Code.INSYNC);
            }
            catch
            {
                Result = false;
            }
            if (!Result)
            {
                _Port.BaudRate = PrevBaud;
            }
            return Result;
        }

        /// <summary>
        /// TODO - determine whether bootloader is 0.0 or 0.1, as in Matheus's code.
        /// </summary>
        /// <returns></returns>
        bool IsInBootloaderXMode()
        {
            //for (int n = 0; n < 3; n++)
            {
                Thread.Sleep(200);
                _Port.Write("U");   //Send it a capital U to sync the baud rate
                Thread.Sleep(200);
                _Port.Write("\r\n");
                Thread.Sleep(200);
                _Port.DiscardInBuffer();
                _Port.Write("CHIPID\r\n");
                if (WaitForToken("RFD", 200))
                {
                    return true;
                }
            }
            return false;
        }

        TMode DetermineMode()
        {
            if (IsInBootloaderXMode())
            {
                return TMode.BOOTLOADER_X;
            }

            _Port.ReadTimeout = 2000;
            //Console.WriteLine("Waiting 1500ms");
            Thread.Sleep(1500);
            _Port.DiscardInBuffer();
            //Console.WriteLine("Sending +++");
            _Port.Write("+++");
            //Console.WriteLine("Waiting up to 3s for OK");
            if (WaitForToken("OK\r\n", 3000))
            {
                return TMode.AT_COMMAND;
            }
            //Check if already in AT command mode.
            _Port.Write("\r\nATI\r\n");
            if (WaitForToken("RFD", 200))
            {
                return TMode.AT_COMMAND;
            }
            //Not in transparent or at command mode.  Probably in bootloader mode.  Need to verify.
            if (IsInBootloaderXMode())
            {
                return TMode.BOOTLOADER_X;
            }
            if (IsInBootloaderMode())
            {
                return TMode.BOOTLOADER;
            }
            return TMode.UNKNOWN;
        }

        void CheckIfInBootloaderMode()
        {
            if (IsInBootloaderXMode())
            {
                _Mode = TMode.BOOTLOADER_X;
            }
            else if (IsInBootloaderMode())
            {
                _Mode = TMode.BOOTLOADER;
            }
        }

        public MissionPlanner.Comms.ICommsSerial Port
        {
            get
            {
                return _Port;
            }
        }

        public TMode GetMode()
        {
            if (_Mode == TMode.INIT)
            {
                _Mode = DetermineMode();
            }
            return _Mode;
        }

        public TMode PutIntoBootloaderMode()
        {
            var CurrentMode = GetMode();
            if (CurrentMode == TMode.AT_COMMAND)
            {
                _Port.Write("\r\n");
                Thread.Sleep(100);
                _Port.Write("AT&UPDATE\r\n");
                Thread.Sleep(100);
                CheckIfInBootloaderMode();
            }
            return GetMode();
        }

        public TMode PutIntoATCommandMode()
        {
            TMode Current = GetMode();
            switch (Current)
            {
                case TMode.BOOTLOADER:
                    int PrevBaud = _Port.BaudRate;
                    _Port.BaudRate = BOOTLOADER_BAUD;
                    WriteBootloaderCode(uploader.Uploader.Code.REBOOT);
                    _Port.BaudRate = PrevBaud;
                    _Mode = TMode.INIT;
                    break;
                case TMode.BOOTLOADER_X:
                    // boot
                    Thread.Sleep(100);
                    _Port.Write("\r\n");
                    Thread.Sleep(100);
                    _Port.Write("BOOTNEW\r\n");
                    Thread.Sleep(100);
                    _Mode = TMode.INIT;
                    break;
            }
            return GetMode();
        }
    }

    public abstract class RFD900
    {
        protected TSession _Session;

        public RFD900(TSession Session)
        {
            _Session = Session;
        }

        public abstract bool ProgramFirmware(string FilePath, Action<double> Progress);

        static bool SearchTokenUpdate(byte[] Token, ref int TokenIndex, byte NextByte)
        {
            if (NextByte == Token[TokenIndex])
            {
                if (++TokenIndex >= Token.Length)
                {
                    //Found it.
                    return true;
                }
            }
            else
            {
                TokenIndex = 0;
            }
            return false;
        }

        /// <summary>
        /// Search the given binary file for the given token
        /// </summary>
        /// <param name="BinFilePath">The binary file path.  Must not be null.</param>
        /// <param name="Token">The token.  Must not be null.</param>
        /// <returns>true if found, false if not.</returns>
        protected static bool SearchBinary(string BinFilePath, string Token)
        {
            byte[] BinToken = System.Text.ASCIIEncoding.ASCII.GetBytes(Token);

            using (System.IO.FileStream FS = new System.IO.FileStream(BinFilePath, System.IO.FileMode.Open))
            {
                int Byte;
                int TokenIndex = 0;
                while ((Byte = FS.ReadByte()) != -1)
                {
                    if (SearchTokenUpdate(BinToken, ref TokenIndex, (byte)Byte))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Search the given hex file for the given token.
        /// </summary>
        /// <param name="File">The file to search.  Must not be null.</param>
        /// <param name="Token">The token to search for.  Must not be null.</param>
        /// <returns>true if found, false if not found.</returns>
        protected static bool SearchHex(uploader.IHex File, string Token)
        {
            int TokenIndex = 0;
            byte[] BinToken = System.Text.ASCIIEncoding.ASCII.GetBytes(Token);

            foreach (var Part in File.Values)
            {
                foreach (byte b in Part)
                {
                    if (SearchTokenUpdate(BinToken, ref TokenIndex, b))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        protected abstract string GetFirmwareSearchToken();

        protected void ShowWrongFirmwareMessageBox()
        {
            System.Windows.Forms.MessageBox.Show("File doesn't appear to be valid for this radio.  Could not find \"" +
                    GetFirmwareSearchToken() + "\" in file.");
        }
    }

    public abstract class RFD900APU : RFD900
    {
        public RFD900APU(TSession Session)
            : base(Session)
        {

        }

        public override bool ProgramFirmware(string FilePath, Action<double> Progress)
        {
            try
            {
                uploader.IHex Hex = new uploader.IHex();
                Hex.load(FilePath);

                if (SearchHex(Hex, GetFirmwareSearchToken()))
                {
                    uploader.Uploader UL = new uploader.Uploader();
                    UL.ProgressEvent += new MissionPlanner.Sikradio.ProgressEventHandler(Progress);
                    UL.upload(_Session.Port, Hex);
                    return true;
                }
                else
                {
                    ShowWrongFirmwareMessageBox();
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Get the correct object for the modem which is in bootloader mode.
        /// </summary>
        /// <param name="Session">The session.  Must not be null.</param>
        /// <returns>null if could not generate modem object</returns>
        public static RFD900APU GetObjectForModem(TSession Session)
        {
            uploader.Uploader U = new uploader.Uploader();
            U.port = Session.Port;
            uploader.Uploader.Board Board = uploader.Uploader.Board.FAILED;
            uploader.Uploader.Frequency Freq = uploader.Uploader.Frequency.FAILED;

            U.getDevice(ref Board, ref Freq);
            switch (Board)
            {
                case uploader.Uploader.Board.DEVICE_ID_RFD900A:
                    return new RFD900a(Session);
                case uploader.Uploader.Board.DEVICE_ID_RFD900P:
                    return new RFD900p(Session);
                case uploader.Uploader.Board.DEVICE_ID_RFD900U:
                    return new RFD900u(Session);
                default:
                    return null;
            }
        }
    }

    public class RFD900a : RFD900APU
    {
        public RFD900a(TSession Session)
            : base(Session)
        {

        }

        protected override string GetFirmwareSearchToken()
        {
            return "RFD900A";
        }
    }

    public class RFD900p : RFD900APU
    {
        public RFD900p(TSession Session)
            : base(Session)
        {

        }

        protected override string GetFirmwareSearchToken()
        {
            return "RFD900P";
        }
    }

    public class RFD900u : RFD900APU
    {
        public RFD900u(TSession Session)
            : base(Session)
        {

        }

        protected override string GetFirmwareSearchToken()
        {
            return "RFD900U";
        }
    }

    public class RFD900x : RFD900
    {
        public RFD900x(TSession Session)
            : base(Session)
        {

        }

        public override bool ProgramFirmware(string FilePath, Action<double> Progress)
        {
            if (SearchBinary(FilePath, GetFirmwareSearchToken()))
            {
                _Session.Port.Write("\r");
                Thread.Sleep(100);
                _Session.Port.DiscardInBuffer();
                _Session.Port.Write("UPLOAD\r");
                if (_Session.WaitForToken("Ready\r\n", 5000) && _Session.WaitForToken("C", 5000))
                {
                    try
                    {
                        MissionPlanner.Radio.XModem.ProgressEvent += new MissionPlanner.Sikradio.ProgressEventHandler(Progress);
                        MissionPlanner.Radio.XModem.Upload(FilePath, _Session.Port);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                ShowWrongFirmwareMessageBox();
                return false;
            }
        }

        protected override string GetFirmwareSearchToken()
        {
            return "RFD900x";
        }
    }
}