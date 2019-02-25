﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Reflection;
using uploader;

namespace RFD.RFD900
{
    /// <summary>
    /// For detecting which mode the modem is in, and putting it in to desired modes.
    /// </summary>
    public class TSession : IDisposable
    {
        TMode _Mode = TMode.INIT;
        MissionPlanner.Comms.ICommsSerial _Port;
        RFDLib.IO.ATCommand.TClient _ATCClient;
        RFD900 _ModemObject;
        public uploader.Uploader.Board Board = uploader.Uploader.Board.FAILED;
        int _MainFirmwareBaud;
        const int BOOTLOADER_BAUD = 115200;

        public TSession(MissionPlanner.Comms.ICommsSerial Port, int MainFirmwareBaud)
        {
            _Port = Port;
            _ATCClient = new RFDLib.IO.ATCommand.TClient(new TMissionPlannerSerialPort(Port));
            _ATCClient.Echoes = true;
            _ATCClient.Terminator = "\r\n";
            _ATCClient.Timeout = 1000;
            _MainFirmwareBaud = MainFirmwareBaud;
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

        public void Dispose()
        {
            //_Port.Close();
        }

        public bool WaitForToken(string Token, int MaxWait)
        {
            return WaitForAnyOfTheseTokens(new string[] { Token }, MaxWait) >= 0;
        }

        /// <summary>
        /// Wait for any of these tokens to be received on the port.  
        /// Wait up to MaxWait milliseconds.  If a token was received, return the array index of
        /// the token.  If no tokens received, return -1.
        /// </summary>
        /// <param name="Tokens">The tokens to wait for.  Must not be null.</param>
        /// <param name="MaxWait">The max wait time in milliseconds.</param>
        /// <returns>The token array index of the token received, or -1 is none of the tokens received.</returns>
        public int WaitForAnyOfTheseTokens(string[] Tokens, int MaxWait)
        {
            System.Diagnostics.Stopwatch SW = new System.Diagnostics.Stopwatch();
            string Temp = "";
            int Timeout = _Port.ReadTimeout;
            _Port.ReadTimeout = MaxWait;
            SW.Start();

            while (SW.ElapsedMilliseconds < MaxWait)
            {
                try
                {
                    int Byte;
                    while ((Byte = _Port.ReadByte()) != -1)
                    {
                        //string x = _Port.ReadExisting();
                        Temp += ((char)Byte);

                        for (int n = 0; n < Tokens.Length; n++)
                        {
                            if (Temp.Contains(Tokens[n]))
                            {
                                _Port.ReadTimeout = Timeout;
                                return n;
                            }
                        }
                    }

                    Thread.Sleep(50);
                }
                catch
                {
                    return -1;
                }
            }

            //Console.WriteLine("Failed to get token, got this instead:  " + Temp);

            _Port.ReadTimeout = Timeout;
            return -1;
        }

        void WriteBootloaderCode(uploader.Uploader.Code Code)
        {
            byte[] ToSend = new byte[] { (byte)Code };
            _Port.Write(ToSend, 0, 1);
        }

        bool IsInBootloaderMode()
        {
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
                _Port.BaudRate = _MainFirmwareBaud;
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
            //Console.WriteLine("Waiting up to 1.5s for OK");
            if (WaitForToken("OK\r\n", 1500))
            {
                return TMode.AT_COMMAND;
            }
            //Check if already in AT command mode.
            _Port.DiscardInBuffer();
            _Port.Write("\r\n");
            Thread.Sleep(100);
            _Port.Write("ATI\r\n");
            if (WaitForToken("RFD", 400))
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

        public RFDLib.IO.ATCommand.TClient ATCClient
        {
            get
            {
                return _ATCClient;
            }
        }

        public MissionPlanner.Comms.ICommsSerial Port
        {
            get
            {
                return _Port;
            }
        }

        /// <summary>
        /// Get the modem object, assuming it can be put into AT command mode, or already is in AT command mode.
        /// Returns null if could not get modem object.
        /// </summary>
        /// <returns></returns>
        public RFD900 GetModemObject()
        {
            if (_ModemObject == null)
            {
                switch (GetMode())
                {
                    case TMode.TRANSPARENT:
                        if (PutIntoATCommandMode() != TMode.AT_COMMAND)
                        {
                            return null;
                        }
                        goto case TMode.AT_COMMAND;
                    case TMode.AT_COMMAND:
                        string Result = _ATCClient.DoQuery("ATI2", true);
                        try
                        {
                            int Code = int.Parse(Result);
                            uploader.Uploader.Board Board = (uploader.Uploader.Board)Code;
                            switch (Board)
                            {
                                case Uploader.Board.DEVICE_ID_RFD900A:
                                    _ModemObject = new RFD900a(this);
                                    break;
                                case Uploader.Board.DEVICE_ID_RFD900P:
                                    _ModemObject = new RFD900p(this);
                                    break;
                                case Uploader.Board.DEVICE_ID_RFD900U:
                                    _ModemObject = new RFD900u(this);
                                    break;
                                case Uploader.Board.DEVICE_ID_RFD900UX:
                                    _ModemObject = new RFD900ux(this);
                                    break;
                                case Uploader.Board.DEVICE_ID_RFD900X:
                                    _ModemObject = new RFD900x(this);
                                    break;
                            }
                        }
                        catch
                        {
                            
                        }
                        break;
                    case TMode.BOOTLOADER:
                        _ModemObject = RFD900APU.GetObjectForModem(this);
                        break;
                    case TMode.BOOTLOADER_X:
                        _ModemObject = RFD900xux.GetObjectForModem(this);
                        break;
                }
            }
            return _ModemObject;
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
            switch (CurrentMode)
            {
                case TMode.BOOTLOADER:
                case TMode.BOOTLOADER_X:
                    break;
                default:
                    CurrentMode = PutIntoATCommandMode();
                    if (CurrentMode == TMode.AT_COMMAND)
                    {
                        _Port.Write("\r\n");
                        Thread.Sleep(100);
                        _Port.Write("AT&UPDATE\r\n");
                        Thread.Sleep(100);
                        CheckIfInBootloaderMode();
                    }
                    break;
            }

            return GetMode();
        }

        public TMode PutIntoATCommandMode()
        {
            TMode Current = GetMode();
            switch (Current)
            {
                case TMode.TRANSPARENT:
                    return PutIntoATCommandModeAssumingInTransparentMode();
                case TMode.BOOTLOADER:
                    _Port.BaudRate = BOOTLOADER_BAUD;
                    WriteBootloaderCode(uploader.Uploader.Code.REBOOT);
                    _Port.BaudRate = _MainFirmwareBaud;
                    _Mode = TMode.INIT;
                    break;
                case TMode.BOOTLOADER_X:
                    // boot
                    Thread.Sleep(100);
                    _Port.Write("\r\n");
                    Thread.Sleep(100);
                    _Port.Write("RESET\r\n");
                    Thread.Sleep(100);
                    _Mode = TMode.INIT;
                    break;
            }
            return GetMode();
        }

        /// <summary>
        /// Assuming in transparent mode, try to put it into AT command mode.
        /// If fails, determine mode and then try again.
        /// </summary>
        public TMode PutIntoATCommandModeAssumingInTransparentMode()
        {
            _Port.ReadTimeout = 2000;
            //Console.WriteLine("Waiting 1500ms");
            Thread.Sleep(1500);
            _Port.DiscardInBuffer();
            //Console.WriteLine("Sending +++");
            _Port.Write("+++");
            //Console.WriteLine("Waiting up to 3s for OK");
            if (WaitForToken("OK\r\n", 3000))
            {
                _Mode = TMode.AT_COMMAND;
                return TMode.AT_COMMAND;
            }

            _Mode = TMode.INIT;
            return PutIntoATCommandMode();
        }

        /// <summary>
        /// Put into transparent mode.
        /// </summary>
        /// <returns></returns>
        public TMode PutIntoTransparentMode()
        {
            if (GetMode() == TMode.TRANSPARENT)
            {
                System.Diagnostics.Debug.WriteLine("Already in transparent mode");
                return TMode.TRANSPARENT;
            }
            else
            {
                if (PutIntoATCommandMode() == TMode.AT_COMMAND)
                {
                    _Port.Write("\r\n");
                    Thread.Sleep(100);
                    _Port.Write("ATO\r\n");
                    Thread.Sleep(100);
                    if (WaitForToken("ATO\r\n", 100))
                    {
                        System.Diagnostics.Debug.WriteLine("Put into transparent mode OK");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to put into transparent mode");
                    }
                    _Mode = TMode.TRANSPARENT;
                    return TMode.TRANSPARENT;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Failed to put into AT cmd mode");
                    return TMode.UNKNOWN;
                }
            }
        }

        public void AssumeMode(TMode Mode)
        {
            if (Mode == TMode.BOOTLOADER_X)
            {
                _Port.BaudRate = BOOTLOADER_BAUD;
            }
            else
            {
                _Port.BaudRate = _MainFirmwareBaud;
            }
            _Mode = Mode;
        }

        /// <summary>
        /// Parse the setting designator from the given Line and character positon.
        /// </summary>
        /// <param name="Line">The ATI5? line.  Must not be null.</param>
        /// <param name="n">The current character position.</param>
        /// <returns></returns>
        string ParseDesignator(string Line, ref int n)
        {
            n = 0;
            string Designator = "";

            if (RFDLib.Text.CheckIsLetter(Line[0]))
            {
                Designator += Line[0];
            }
            else
            {
                return null;
            }

            n = 1;

            while (Line[n] != ':')
            {
                if (RFDLib.Text.CheckIsNumeral(Line[n]))
                {
                    Designator += Line[n];
                }
                else
                {
                    return null;
                }
                n++;
            }

            return Designator;
        }

        string ParseName(string Line, ref int n)
        {
            string Name = "";
            while (true)
            {
                switch (Line[n])
                {
                    case '(':
                    case '=':
                        return Name;
                }

                Name += Line[n];
                n++;
            }
        }

        bool ParseType(string Line, ref int n)
        {
            for (; Line[n] != ')'; n++)
            {
            }
            n++;
            return true;
        }

        bool ParseIntUntil(string Line, ref int n, string Delimiter, out int Value)
        {
            string Temp = "";
            while ((n < Line.Length) && RFDLib.Text.CheckIsNumeral(Line[n]))
            {
                Temp += Line[n];
                n++;
            }
            Value = int.Parse(Temp);
            if (Delimiter == null)
            {
                return true;
            }
            else
            {
                if (Line.IndexOf(Delimiter, n) == n)
                {
                    n += Delimiter.Length;
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        TSetting.TRange ParseRange(string Line, ref int n)
        {
            if (Line[n] != '[')
            {
                return null;
            }
            n++;
            int Min;
            if (!ParseIntUntil(Line, ref n, "..", out Min))
            {
                return null;
            }
            int Max;
            if (!ParseIntUntil(Line, ref n, "]", out Max))
            {
                return null;
            }
            return new TSetting.TRange(Min, Max);
        }

        bool ParseValue(string Line, ref int n, out int Value)
        {
            return ParseIntUntil(Line, ref n, null, out Value);
        }

        bool CharArrayContains(char[] Array, char x)
        {
            foreach (var c in Array)
            {
                if (c == x)
                {
                    return true;
                }
            }
            return false;
        }

        string GetStringUntil(string Line, ref int n, char[] Tokens)
        {
            string Result = "";
            while (!CharArrayContains(Tokens, Line[n]))
            {
                Result += Line[n];
                n++;
            }
            return Result;
        }

        string[] ParseOptions(string Line, ref int n)
        {
            if (Line[n] != '{')
            {
                return null;
            }
            n++;
            List<string> Options = new List<string>();
            while (true)
            {
                string Temp = GetStringUntil(Line, ref n, new char[] { ',', '}' });
                if (Temp != null && Temp.Length > 0)
                {
                    Options.Add(Temp);
                }
                if (Line[n] == '}')
                {
                    break;
                }
                else
                {
                    n++;
                }
            }

            return Options.ToArray();
        }

        int[] CheckIfAllIntegers(string[] Options)
        {
            int[] Result = new int[Options.Length];

            for (int n = 0; n < Options.Length; n++)
            {
                if (!int.TryParse(Options[n], out Result[n]))
                {
                    return null;
                }
            }
            return Result;
        }

        float GetScaleFactor(string Name)
        {
            switch (Name)
            {
                case "SERIAL_SPEED":
                    return 0.001F;
            }
            return 1;
        }

        TSetting.TOption[] CreateOptions(TSetting.TRange Range, string[] Options,
            string Name)
        {
            if (Range == null || Options == null)
            {
                return null;
            }

            int[] Ints = CheckIfAllIntegers(Options);
            if (Ints != null)
            {
                TSetting.TOption[] Result = new TSetting.TOption[Ints.Length];
                float SF = GetScaleFactor(Name);
                for (int n = 0; n < Result.Length; n++)
                {
                    Result[n] = new TSetting.TOption((int)(SF * Ints[n]), Options[n]);
                }
                return Result;
            }

            //Not all integers.
            if (Range == null)
            {
                return null;
            }
            if (Options.Length == 2)
            {
                //Match up with min and max.
                TSetting.TOption[] Result = new TSetting.TOption[2];
                Result[0] = new TSetting.TOption(Range.Min, Options[0]);
                Result[1] = new TSetting.TOption(Range.Max, Options[1]);
                return Result;
            }
            else
            {
                return null;
            }
        }

        int GetIncrement(string Name)
        {
            switch (Name)
            {
                case "MIN_FREQ":
                case "MAX_FREQ":
                    return 500;
                case "MAX_WINDOW":
                    return 20;
                default:
                    return 1;
            }
        }

        /// <summary>
        /// A valid line looks like one of the following formats:
        /// Designator:Name:(Type)[Range]=Value{Options}\r\n
        ///     S2:AIR_SPEED(L)[4..1000]=125{4,64,125,250,500,1000,}<\r><\n>
        /// Designator:Name=Value\r\n
        ///     S2:AIR_SPEED=64<\r><\n>
        /// </summary>
        /// <param name="Line">The line.  Must not be null.</param>
        /// <returns>The setting parsed, or null if could not parse setting.</returns>
        TSetting ParseATI5QueryResponseLine(string Line)
        {
            try
            {
                int n = 0;
                string Designator = ParseDesignator(Line, ref n);
                if (Designator == null)
                {
                    return null;
                }

                n++;
                string Name = ParseName(Line, ref n);
                if (Name == null || Name.Length == 0)
                {
                    return null;
                }
                TSetting.TRange Range = null;
                if (Line[n] == '(')
                {
                    if (!ParseType(Line, ref n))
                    {
                        return null;
                    }
                    Range = ParseRange(Line, ref n);
                    if (Range == null)
                    {
                        return null;
                    }
                    
                }
                if (Line[n] != '=')
                {
                    return null;
                }
                n++;
                
                int Value;
                if (!ParseValue(Line, ref n, out Value))
                {
                    return null;
                }
                string[] Options = null;

                if (n < Line.Length)
                {
                    switch (Line[n])
                    {
                        case '\r':
                            break;
                        case '{':
                            Options = ParseOptions(Line, ref n);
                            break;
                        default:
                            return null;
                    }
                }

                return new TSetting(Designator, Name, Range, Value, 
                    CreateOptions(Range, Options, Name), GetIncrement(Name));
            }
            catch
            {
                return null;
            }
        }

        TSetting.TOption[] GetBaudRateOptionsGivenRawBaudRates(int[] Rates)
        {
            List<TSetting.TOption> Options = new List<TSetting.TOption>();
            foreach (var Rate in Rates)
            {
                Options.Add(new TSetting.TOption(Rate / 1000, Rate.ToString()));
            }
            return Options.ToArray();
        }

        TSetting.TOption[] GetDefaultBaudRateSettingForBoard(uploader.Uploader.Board Board)
        {
            switch (Board)
            {
                case uploader.Uploader.Board.DEVICE_ID_RFD900X:
                    return GetBaudRateOptionsGivenRawBaudRates(
                        new int[] { 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200, 230400, 460800 });
                default:
                    return GetBaudRateOptionsGivenRawBaudRates(
                        new int[] { 1200, 2400, 4800, 9600, 19200, 38400, 57600, 115200});
            }
        }

        /// <summary>
        /// Parse an ATI5 query response, into a list of settings.  Settings may contain the options
        /// available.
        /// </summary>
        /// <param name="ATI5Response">The full ATI5 query response.  Must not be null.</param>
        /// <returns>A dictionary of settings found, with name as key.  Never null.</returns>
        Dictionary<string, TSetting> ParseATI5QueryResponse(string ATI5Response,
            Dictionary<string, TSetting> Dict)
        {
            foreach (var Line in ATI5Response.Split('\n', '\r'))
            {
                var S = ParseATI5QueryResponseLine(Line);
                if (S != null)
                {
                    Dict[S.Name] = S;
                }
            }

            return Dict;
        }

        public Dictionary<string, TSetting> GetSettings(string ATI5Response,
            uploader.Uploader.Board Board)
        {
            Dictionary<string, TSetting> Result = new Dictionary<string, TSetting>();
            ParseATI5QueryResponse(ATI5Response, Result);
            string SerialName = "SERIAL_SPEED";
            if (Result.ContainsKey(SerialName))
            {
                if (Result[SerialName].Options == null)
                {
                    Result[SerialName].Options = GetDefaultBaudRateSettingForBoard(Board);
                }
            }
            return Result;
        }
    }

    public class TSetting
    {
        public string Designator;
        public string Name;
        /// <summary>
        /// null if range unknown
        /// </summary>
        public TRange Range;
        public int Value;
        /// <summary>
        /// null if options unknown
        /// </summary>
        public TOption[] Options;
        public int Increment;

        public TSetting(string Designator, string Name, TRange Range, int Value, TOption[] Options,
            int Increment)
        {
            this.Designator = Designator;
            this.Name = Name;
            this.Range = Range;
            this.Value = Value;
            this.Options = Options;
            this.Increment = Increment;
        }

        public string[] GetOptionNames()
        {
            if (Options == null)
            {
                return null;
            }
            else
            {
                return RFDLib.Array.CherryPickArray(Options, (x) => x.OptionName);
            }
        }

        public string GetOptionNameForValue(string Value)
        {
            if (Options != null)
            {
                foreach (var O in Options)
                {
                    if (O.Value.ToString() == Value)
                    {
                        return O.OptionName;
                    }
                }
            }
            return null;
        }

        public class TRange
        {
            public readonly int Min;
            public readonly int Max;

            public TRange(int Min, int Max)
            {
                this.Min = Min;
                this.Max = Max;
            }
        }

        public class TOption
        {
            public readonly int Value;
            public readonly string OptionName;

            public TOption(int Value, string OptionName)
            {
                this.Value = Value;
                this.OptionName = OptionName;
            }
        }
    }

    public abstract class RFD900
    {
        protected TSession _Session;

        public RFD900(TSession Session)
        {
            _Session = Session;
        }

        /// <summary>
        /// Program firmware into modem, firstly doing necessary checks.
        /// </summary>
        /// <param name="FilePath">The path of the firmware.  Must not be null.</param>
        /// <param name="Progress">A callback to report progress.</param>
        /// <returns>true if succeeded, false if failed.</returns>
        public virtual bool ProgramFirmware(string FilePath, Action<string, double> Progress)
        {
            if (CheckFirmwareOK(FilePath))
            {
                Progress("Putting into bootloader mode.", double.NaN);
                var Mode = _Session.PutIntoBootloaderMode();
                switch (Mode)
                {
                    case TSession.TMode.BOOTLOADER:
                    case TSession.TMode.BOOTLOADER_X:
                        Progress("Programming selected firmware into modem.", double.NaN);
                        return DoFirmwareProgramming(FilePath, Progress);
                    default:
                        Progress("Failed to put into bootloader mode.", double.NaN);
                        return false;
                }
            }
            else
            {
                Progress("Incorrect firmware selected.", double.NaN);
                return false;
            }
        }

        /// <summary>
        /// The function which actually programs the firmware, doesn't check the firmware first, etc.
        /// </summary>
        /// <param name="FilePath">The firmware file path.  Must not be null.</param>
        /// <param name="Progress">The programming progress callback.  Must not be null.</param>
        /// <returns>true if successful, false if failed.</returns>
        protected abstract bool DoFirmwareProgramming(string FilePath, Action<string, double> Progress);
        protected abstract bool CheckFirmwareOK(string FilePath);

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
        /// Search the given binary file for the given tokens
        /// </summary>
        /// <param name="BinFilePath">The binary file path.  Must not be null.</param>
        /// <param name="Tokens">The tokens.  Must not be null.</param>
        /// <returns>true if at least one token found, false if not.</returns>
        protected static bool SearchBinary(string BinFilePath, string[] Tokens)
        {
            foreach (var Token in Tokens)
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
            }

            return false;
        }

        /// <summary>
        /// Search the given hex file for the given tokens.
        /// </summary>
        /// <param name="File">The file to search.  Must not be null.</param>
        /// <param name="Tokens">The tokens to search for.  Must not be null.</param>
        /// <returns>true if at least one found, false if not found.</returns>
        protected static bool SearchHex(uploader.IHex File, string[] Tokens)
        {
            foreach (var Token in Tokens)
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
            }
            return false;
        }

        protected abstract string[] GetFirmwareSearchTokens();

        protected void ShowWrongFirmwareMessageBox()
        {
            string S = "File doesn't appear to be valid for this radio.  Could not find ";
            var Tokens = GetFirmwareSearchTokens();
            for (int n = 0; n < Tokens.Length; n++)
            {
                if (n != 0)
                {
                    if (n >= (Tokens.Length - 1))
                    {
                        S += " or ";
                    }
                    else
                    {
                        S += ", ";
                    }
                }
                S += "\"" + Tokens[n] + "\"";
            }

            S += " in File.";

            System.Windows.Forms.MessageBox.Show(S);
        }

        public abstract uploader.Uploader.Board Board { get; }
    }

    public abstract class RFD900APU : RFD900
    {
        public RFD900APU(TSession Session)
            : base(Session)
        {

        }

        /// <summary>
        /// Check whether the firmware in the given path is suitable for this modem.
        /// </summary>
        /// <param name="FilePath">The path to the firmware.  Must not be null.</param>
        /// <returns>true if OK, false if not.</returns>
        protected override bool CheckFirmwareOK(string FilePath)
        {
            try
            {
                uploader.IHex Hex = new uploader.IHex();
                Hex.load(FilePath);

                if (SearchHex(Hex, GetFirmwareSearchTokens()))
                {
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
        /// Do the firmware programming without doing any checks.
        /// </summary>
        /// <param name="FilePath">The path of the firmware file to program.  Must not be null.</param>
        /// <param name="Progress">A callback for progress.</param>
        /// <returns>true if succeeded, false if failed.</returns>
        protected override bool DoFirmwareProgramming(string FilePath, Action<string, double> Progress)
        {
            try
            {
                uploader.IHex Hex = new uploader.IHex();
                Hex.load(FilePath);

                uploader.Uploader UL = new uploader.Uploader();
                UL.ProgressEvent += (d) => Progress(null, d);
                UL.upload(_Session.Port, Hex);
                _Session.AssumeMode(TSession.TMode.TRANSPARENT);
                return true;
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

        protected override string[] GetFirmwareSearchTokens()
        {
            return new string[] { "RFD900A" };
        }

        public override Uploader.Board Board
        {
            get
            {
                return Uploader.Board.DEVICE_ID_RFD900A;
            }
        }
    }

    public class RFD900p : RFD900APU
    {
        public RFD900p(TSession Session)
            : base(Session)
        {

        }

        protected override string[] GetFirmwareSearchTokens()
        {
            return new string[] { "RFD900P" };
        }

        public override Uploader.Board Board
        {
            get
            {
                return Uploader.Board.DEVICE_ID_RFD900P;
            }
        }
    }

    public class RFD900u : RFD900APU
    {
        public RFD900u(TSession Session)
            : base(Session)
        {

        }

        protected override string[] GetFirmwareSearchTokens()
        {
            return new string[] { "RFD900U" };
        }

        public override Uploader.Board Board
        {
            get
            {
                return Uploader.Board.DEVICE_ID_RFD900U;
            }
        }
    }

    public abstract class RFD900xux : RFD900
    {
        public RFD900xux(TSession Session)
            : base(Session)
        {

        }

        /// <summary>
        /// Get the correct object for the modem which is in bootloader mode.
        /// </summary>
        /// <param name="Session">The session.  Must not be null.</param>
        /// <returns>null if could not generate modem object</returns>
        public static RFD900xux GetObjectForModem(TSession Session)
        {
            Session.Port.Write("\r\n");
            Thread.Sleep(200);
            Session.Port.DiscardInBuffer();
            Session.Port.Write("CHIPID\r\n");
            int TokenIndex = Session.WaitForAnyOfTheseTokens(new string[] { "RFD900x", "RFD900ux" }, 200);
            switch (TokenIndex)
            {
                case 0:
                    return new RFD900x(Session);
                case 1:
                    return new RFD900ux(Session);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns whether the file of the given path is "certified".  i.e. Whether it
        /// will return whether it is locked to a country using the ATI command.
        /// </summary>
        /// <param name="FilePath">The file path.  Must not be null.</param>
        /// <returns>true if certified, false if not.</returns>
        bool IsFirmwareCertified(string FilePath)
        {
            return SearchBinary(FilePath, new string[] { "HastaLaVistaBaby" });
        }

        /// <summary>
        /// Unpack the reference firmware embedded into this exe into a temp file and return
        /// the path to that file.
        /// </summary>
        /// <returns>The path to the temp file containing the ref file.  Never null.</returns>
        string GetRefFirmwarePath()
        {
            string Temp = System.IO.Path.GetTempFileName();
            var assembly = Assembly.GetExecutingAssembly();
            //var resourceName = "RFD900Tools.Properties.Resources.resources.RFDSiK_V3.00_rfd900x.bin";
            var resourceName = "RFD900Tools.Resources.RFDSiK V3.00 rfd900x.bin";

            var Names = assembly.GetManifestResourceNames();
            Console.WriteLine(Names.ToString());

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (Stream FileStream = System.IO.File.OpenWrite(Temp))
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(FileStream))
            {
                stream.Seek(0, SeekOrigin.Begin);
                stream.CopyTo(FileStream);
            }

            return Temp;
        }

        /// <summary>
        /// Assumes in AT command mode.
        /// </summary>
        /// <returns></returns>
        bool GetIsThisLockedToCountry()
        {
            string Reply = _Session.ATCClient.DoQuery("ATI", true);
            foreach (var ST in this.GetFirmwareSearchTokens())
            {
                if (Reply.Contains(ST+"-"))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Program the given firmware.  Do necesary checks first.
        /// </summary>
        /// <param name="FilePath">The path to the firmware file.  Must not be null.</param>
        /// <param name="Progress">The function to call back to provide progress updates.</param>
        /// <returns>true if successful, false if failed.</returns>
        public override bool ProgramFirmware(string FilePath, Action<string, double> Progress)
        {
            if (IsFirmwareCertified(FilePath))
            {
                return base.ProgramFirmware(FilePath, Progress);
            }
            else
            {
                Progress("Trying to put into AT command mode.", double.NaN);
                if (_Session.PutIntoATCommandMode() != TSession.TMode.AT_COMMAND)
                {
                    Progress("Trying to put into bootloader mode.", double.NaN);
                    if (_Session.PutIntoBootloaderMode() == TSession.TMode.BOOTLOADER_X)
                    {
                        Progress("Programming temporary firmware into modem to check certified.", double.NaN);
                        string RefFirmwarePath = GetRefFirmwarePath();
                        try
                        {
                            if (!DoFirmwareProgramming(RefFirmwarePath, Progress))
                            {
                                return false;
                            }
                        }
                        finally
                        {
                            try
                            {
                                System.IO.File.Delete(RefFirmwarePath);
                            }
                            catch
                            {

                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }

                //Can it go into AT command mode?
                Progress("Trying to put into AT command mode.", double.NaN);
                if (_Session.PutIntoATCommandMode() == TSession.TMode.AT_COMMAND)
                {
                    Progress("Checking if modem certified.", double.NaN);
                    if (!GetIsThisLockedToCountry())
                    {
                        //They can program whatever they want into it.
                        return base.ProgramFirmware(FilePath, Progress);
                    }
                    else
                    {
                        //They can only program certified firmware into it.
                        if (IsFirmwareCertified(FilePath))
                        {
                            return base.ProgramFirmware(FilePath, Progress);
                        }
                        else
                        {
                            System.Windows.Forms.MessageBox.Show("The selected firmware is not certified to run on this modem.  Aborting.");
                            return false;
                        }
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Program the given firmware without doing any checks.
        /// </summary>
        /// <param name="FilePath">The path of the firmware file.  Must not be null.</param>
        /// <param name="Progress">A function to use to provide real-time feedback about the
        /// programming progress.</param>
        /// <returns>true if successful, false if failed.</returns>
        protected override bool DoFirmwareProgramming(string FilePath, Action<string, double> Progress)
        {
            _Session.Port.Write("\r");
            Thread.Sleep(100);
            _Session.Port.DiscardInBuffer();
            _Session.Port.Write("UPLOAD\r");
            if (_Session.WaitForToken("Ready\r\n", 5000) && _Session.WaitForToken("C", 5000))
            {
                try
                {
                    MissionPlanner.Radio.XModem.ProgressEvent += (d) => Progress(null, d);
                    MissionPlanner.Radio.XModem.Upload(FilePath, _Session.Port);
                    _Session.AssumeMode(TSession.TMode.INIT);
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

        /// <summary>
        /// Check that the firmware is the right type for this modem.
        /// </summary>
        /// <param name="FilePath">The path to the firmware.  Must not be null.</param>
        /// <returns>true if correct, false if not.</returns>
        protected override bool CheckFirmwareOK(string FilePath)
        {
            if (SearchBinary(FilePath, GetFirmwareSearchTokens()))
            {
                return true;
            }
            else
            {
                ShowWrongFirmwareMessageBox();
                return false;
            }
        }
    }

    public class RFD900x : RFD900xux
    {
        public RFD900x(TSession Session)
            : base(Session)
        {

        }

        protected override string[] GetFirmwareSearchTokens()
        {
            return new string[] { "RFD900x", "RFD900X" };
        }

        public override Uploader.Board Board
        {
            get
            {
                return Uploader.Board.DEVICE_ID_RFD900X;
            }
        }
    }

    public class RFD900ux : RFD900xux
    {
        public RFD900ux(TSession Session)
            : base(Session)
        {

        }

        public override Uploader.Board Board
        {
            get
            {
                return Uploader.Board.DEVICE_ID_RFD900UX;
            }
        }

        protected override string[] GetFirmwareSearchTokens()
        {
            return new string[] { "RFD900ux", "RFD900UX" };
        }
    }

    /// <summary>
    /// A RFDLib.IO.TSerialPort wrapper for TMissionPlannerSerialPort
    /// </summary>
    public class TMissionPlannerSerialPort : RFDLib.IO.TSerialPort
    {
        MissionPlanner.Comms.ICommsSerial _Port;

        public TMissionPlannerSerialPort(MissionPlanner.Comms.ICommsSerial Port)
        {
            _Port = Port;
        }

        public override void Write(string Text)
        {
            _Port.Write(Text);
        }

        public override void DiscardInBuffer()
        {
            _Port.DiscardInBuffer();
        }

        public override int ReadChar()
        {
            return _Port.ReadChar();
        }

        public override string ReadExisting()
        {
            return _Port.ReadExisting();
        }

        public override string ReadLine()
        {
            return _Port.ReadLine();
        }

        public override int ReadTimeout
        {
            get
            {
                return _Port.ReadTimeout;
            }
            set
            {
                _Port.ReadTimeout = value;
            }
        }
    }
}