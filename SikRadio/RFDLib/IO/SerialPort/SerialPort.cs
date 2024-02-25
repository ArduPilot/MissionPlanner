using System;

namespace RFDLib.IO
{
    public abstract class TSerialPort
    {
        public abstract void Write(string Text);
        public abstract string ReadExisting();
        public abstract int ReadTimeout { get; set; }
        public abstract int ReadChar();
        public abstract string ReadLine();
        public abstract void DiscardInBuffer();

        /// <summary>
        /// Query a serial port.
        /// </summary>
        /// <param name="Port">The port.  Must not be null.</param>
        /// <param name="Query">The query to send, without the terminator.  Must not be null.</param>
        /// <param name="Terminator">The terminator.  Can be null if none.</param>
        /// <param name="MaxWait">The maximum period to wait in milliseconds.</param>
        /// <returns>The data returned.  Never null.</returns>
        public static string Query(TSerialPort Port,
            string Query, string Terminator, int MaxWait)
        {
            Port.DiscardInBuffer();
            Query = Query + (Terminator == null ? "" : Terminator);
            Port.Write(Query);
            string Reply;
            WaitForToken(Port, Terminator, MaxWait, out Reply);
            return Reply;
        }

        /// <summary>
        /// Wait for the given token or timeout on the given serial port.
        /// </summary>
        /// <param name="Port">The port.  Must not be null.</param>
        /// <param name="Token">The token to wait for.  Can be null if not waiting for a token.</param>
        /// <param name="MaxWait">The maximum amount of time to wait for a response, in milliseconds.</param>
        /// <returns>true if token was received, false if not.</returns>
        public static bool WaitForToken(TSerialPort Port,
            string Token, int MaxWait)
        {
            string Temp;
            return WaitForToken(Port, Token, MaxWait, out Temp);
        }

        /// <summary>
        /// Wait for the given token or timeout on the given serial port
        /// </summary>
        /// <param name="Port">The port.  Must not be null.</param>
        /// <param name="Token">The token to wait for.  Can be null if not waiting for a token.</param>
        /// <param name="MaxWait">The maximum amount of time to wait for a response, in milliseconds.</param>
        /// <param name="Result">The data returned.  Never null.</param>
        /// <returns>true if token was received, false if not.</returns>
        public static bool WaitForToken(TSerialPort Port,
            string Token, int MaxWait, out string Result)
        {
            System.Diagnostics.Stopwatch SW = new System.Diagnostics.Stopwatch();
            string Temp = "";
            int Timeout = Port.ReadTimeout;
            Port.ReadTimeout = MaxWait;
            SW.Start();

            while (SW.ElapsedMilliseconds < MaxWait)
            {
                //System.Diagnostics.Stopwatch StWa = new System.Diagnostics.Stopwatch();
                //SW.Start();
                string x = Port.ReadExisting();
                //if (StWa.ElapsedMilliseconds > 10)
                //{
                //    System.Diagnostics.Debug.WriteLine("Read existing slow\n");
                //}
                Temp += x;

                if ((Token != null) && Temp.Contains(Token))
                {
                    Port.ReadTimeout = Timeout;
                    Result = Temp;
                    return true;
                }

                System.Threading.Thread.Sleep(50);
            }
            Port.ReadTimeout = Timeout;
            Result = Temp;
            return false;
        }
    }

    public class TSystemIOPortsSerialPort : TSerialPort
    {
        System.IO.Ports.SerialPort _Port;

        public TSystemIOPortsSerialPort(System.IO.Ports.SerialPort Port)
        {
            _Port = Port;
        }

        public override void DiscardInBuffer()
        {
            _Port.DiscardInBuffer();
        }

        public override int ReadChar()
        {
            return _Port.ReadChar();
        }

        public override void Write(string Text)
        {
            _Port.Write(Text);
        }

        public override string ReadLine()
        {
            return _Port.ReadLine();
        }

        public override string ReadExisting()
        {
            return _Port.ReadExisting();
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