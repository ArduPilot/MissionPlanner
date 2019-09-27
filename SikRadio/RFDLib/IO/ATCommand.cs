using System;

namespace RFDLib.IO.ATCommand
{
    /// <summary>
    /// An AT-command interface client.
    /// </summary>
    public class TClient
    {
        public IO.TSerialPort _Port;
        /// <summary>
        /// The line terminator.  null if nothing.
        /// </summary>
        public string Terminator = "\r\n";
        /// <summary>
        /// true if the server echoes commands, and the echoes need to be ignored.
        /// </summary>
        public bool Echoes = true;
        /// <summary>
        /// The maximum time to wait for a response.
        /// </summary>
        public int Timeout = 2000;

        /// <summary>
        /// Create a new TClient
        /// </summary>
        /// <param name="Port">The serial port to use.  Must not be null.</param>
        public TClient(IO.TSerialPort Port)
        {
            _Port = Port;
        }

        /// <summary>
        /// Returns the terminator to use.
        /// </summary>
        /// <returns>The terminator to use, never null.</returns>
        string GetTerminator()
        {
            if (Terminator == null)
            {
                return "";
            }
            else
            {
                return Terminator;
            }
        }

        /// <summary>
        /// Get next char from serial port.  Wait up to the read timeout.  
        /// </summary>
        /// <param name="x">the char read</param>
        /// <returns>true if got char before timeout, false if not.</returns>
        bool GetNextChar(out char x)
        {
            try
            {
                x = (char)_Port.ReadChar();
                return true;
            }
            catch
            {
                x = '\0';
                return false;
            }
        }

        /// <summary>
        /// Eliminate the command echo.
        /// </summary>
        /// <param name="CompleteCommand">The complete command including terminator.  Must not be null.</param>
        /// <returns>true if got echo OK, false if not.</returns>
        bool EliminateEcho(string CompleteCommand)
        {
            if (Echoes)
            {
                _Port.ReadTimeout = Timeout;
                int n = 0;
                char Temp;
                while ((n < CompleteCommand.Length) && GetNextChar(out Temp))
                {
                    if (CompleteCommand[n] == Temp)
                    {
                        n++;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Do a command and wait for a reply.  Returns true if "OK"
        /// </summary>
        /// <param name="Command"></param>
        /// <returns></returns>
        public bool DoCommand(string Command)
        {
            string Result = DoQuery(Command, true);
            return Result.Contains("OK");
        }

        /// <summary>
        /// Returns a received line from the interface.
        /// </summary>
        /// <returns></returns>
        public string GetLine()
        {
            return _Port.ReadLine();
        }

        /// <summary>
        /// Do the given command.  If wait for terminator, returns the result minus
        /// the terminator.
        /// </summary>
        /// <param name="Command">The command.  Must not be null.</param>
        /// <param name="WaitForTerminator">true to wait for terminator.</param>
        /// <returns>The reply, except for the echo.</returns>
        public virtual string DoQuery(string Command, bool WaitForTerminator)
        {
            string CompleteCommand = Command + GetTerminator();
            _Port.DiscardInBuffer();
            _Port.Write(CompleteCommand);
            if (EliminateEcho(CompleteCommand))
            {
                string Result;
                if (WaitForTerminator)
                {
                    if (IO.TSerialPort.WaitForToken(_Port, Terminator, Timeout, out Result))
                    {
                        //Get the terminator out of it.
                        return Result.Split(new string[] { Terminator }, StringSplitOptions.None)[0];
                    }
                    else
                    {
                        return Result;
                    }
                }
                else
                {
                    IO.TSerialPort.WaitForToken(_Port, null, Timeout, out Result);
                    return Result;
                }
            }
            else
            {
                return "";
            }
        }

        public string DoQueryWithMultiLineResponse(string Command)
        {
            string CompleteCommand = Command + GetTerminator();
            _Port.DiscardInBuffer();
            _Port.Write(CompleteCommand);

            if (EliminateEcho(CompleteCommand))
            {
                string Result = "";
                string Temp;
                int Timeout = this.Timeout;

                while (IO.TSerialPort.WaitForToken(_Port, Terminator, Timeout, out Temp))
                {
                    Result += Temp;
                    Timeout = 500;
                }

                return Result;
            }
            else
            {
                return "";
            }
        }
    }

    public class TServer
    {
        TSerialPort _Port;
        public event Action<TCommand> GotCommand;

        public TServer(TSerialPort Port)
        {
            _Port = Port;
        }

        public abstract class TCommand
        {
        }

        public class TQuery : TCommand
        {
            public char FirstCharacter;
            public string Parameter;
        }

        public class TSetValue : TCommand
        {
            public char FirstCharacter;
            public string Parameter;
            public string Value;
        }

    }
}