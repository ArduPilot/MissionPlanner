namespace Rtsp.Messages
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Text.RegularExpressions;

    public class RtspMessage : RtspChunk
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The regex to validate the Rtsp message.
        /// </summary>
        private static readonly Regex _rtspVersionTest = new Regex(@"^RTSP/\d\.\d", RegexOptions.Compiled);
        /// <summary>
        /// Create the good type of Rtsp Message from the header.
        /// </summary>
        /// <param name="aRequestLine">A request line.</param>
        /// <returns>An Rtsp message</returns>
        public static RtspMessage GetRtspMessage(string aRequestLine)
        {
            // We can't determine the message 
            if (string.IsNullOrEmpty(aRequestLine))
                return new RtspMessage();
            string[] requestParts = aRequestLine.Split(new char[] { ' ' }, 3);
            RtspMessage returnValue;
            if (requestParts.Length == 3)
            {
                // A request is : Method SP Request-URI SP RTSP-Version
                // A response is : RTSP-Version SP Status-Code SP Reason-Phrase
                // RTSP-Version = "RTSP" "/" 1*DIGIT "." 1*DIGIT
                if (_rtspVersionTest.IsMatch(requestParts[2]))
                    returnValue = RtspRequest.GetRtspRequest(requestParts);
                else if (_rtspVersionTest.IsMatch(requestParts[0]))
                    returnValue = new RtspResponse();
                else
                {
                    _logger.Warn(CultureInfo.InvariantCulture, "Got a strange message {0}", aRequestLine);
                    returnValue = new RtspMessage();
                }
            }
            else
            {
                _logger.Warn(CultureInfo.InvariantCulture, "Got a strange message {0}", aRequestLine);
                returnValue = new RtspMessage();
            }
            returnValue.Command = aRequestLine;
            return returnValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RtspMessage"/> class.
        /// </summary>
        public RtspMessage()
        {
            Data = new byte[0];
            Creation = DateTime.Now;
        }

        private Dictionary<string, string> _headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        internal protected string[] commandArray;

        /// <summary>
        /// Gets or sets the creation time.
        /// </summary>
        /// <value>The creation time.</value>
        public DateTime Creation { get; private set; }

        /// <summary>
        /// Gets or sets the command of the message (first line).
        /// </summary>
        /// <value>The command.</value>
        public string Command
        {
            get
            {
                if (commandArray == null)
                    return string.Empty;
                return string.Join(" ", commandArray);
            }
            set
            {
                if (value == null)
                    commandArray = new string[] { String.Empty };
                else
                    commandArray = value.Split(new char[] {' '}, 3);
            }
        }


		/// <summary>
        /// Gets the Method of the message (eg OPTIONS, DESCRIBE, SETUP, PLAY).
        /// </summary>
        /// <value>The Method</value>
        public string Method
		{
			get
			{
				if (commandArray == null)
					return string.Empty;
				return commandArray[0];
			}
		}


        /// <summary>
        /// Gets the headers of the message.
        /// </summary>
        /// <value>The headers.</value>
        public Dictionary<string, string> Headers
        {
            get
            {
                return _headers;
            }
        }

        /// <summary>
        /// Adds one header from a string.
        /// </summary>
        /// <param name="line">The string containing header of format Header: Value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="line"/> is null</exception>
        public void AddHeader(string line)
        {
            if (line == (string)null)
                throw new ArgumentNullException("line");

            //spliter
            string[] elements = line.Split(new char[] { ':' }, 2);
            if (elements.Length == 2)
            {
                _headers[elements[0].Trim()] = elements[1].TrimStart();
            }
            else
            {
                _logger.Warn(CultureInfo.InvariantCulture, "Invalid Header received : -{0}-", line);
            }
        }

        /// <summary>
        /// Gets or sets the Ccommande Seqquence number.
        /// <remarks>If the header is not define or not a valid number it return 0</remarks>
        /// </summary>
        /// <value>The sequence number.</value>
        public int CSeq
        {
            get
            {
                string returnStringValue;
                int returnValue;
                if (!(_headers.TryGetValue("CSeq", out returnStringValue) &&
                    int.TryParse(returnStringValue, out returnValue)))
                    returnValue = 0;

                return returnValue;
            }
            set
            {
                _headers["CSeq"] = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Gets the session ID.
        /// </summary>
        /// <value>The session ID.</value>
        public virtual string Session
        {
            get
            {
                if (!_headers.ContainsKey("Session"))
                    return null;

                return _headers["Session"];
            }
            set
            {
                _headers["Session"] = value;
            }
        }
        
        /// <summary>
        /// Initialises the length of the data byte array from content lenth header.
        /// </summary>
        public void InitialiseDataFromContentLength()
        {
            int dataLength;
            if (!(_headers.ContainsKey("Content-Length")
                && int.TryParse(_headers["Content-Length"], out dataLength)))
            {
                dataLength = 0;
            }
            this.Data = new byte[dataLength];
        }

        /// <summary>
        /// Adjusts the content length header.
        /// </summary>
        public void AdjustContentLength()
        {
            if (Data.Length > 0)
            {
                _headers["Content-Length"] = Data.Length.ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                _headers.Remove("Content-Length");
            }
        }

        /// <summary>
        /// Sends to the message to a stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <exception cref="ArgumentNullException"><paramref name="stream"/> is empty</exception>
        /// <exception cref="ArgumentException"><paramref name="stream"/> can't be written.</exception>
        public void SendTo(Stream stream)
        {
            // <pex>
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanWrite)
                throw
                  new ArgumentException("Stream CanWrite == false, can't send message to it", "stream");
            // </pex>
            Contract.EndContractBlock();

            Encoding encoder = ASCIIEncoding.UTF8;
            StringBuilder outputString = new StringBuilder();

            AdjustContentLength();

            // output header
            outputString.Append(Command);
            outputString.Append("\r\n");
            foreach (KeyValuePair<string, string> item in _headers)
            {
                outputString.AppendFormat("{0}: {1}\r\n", item.Key, item.Value);
            }
            outputString.Append("\r\n");
            byte[] buffer = encoder.GetBytes(outputString.ToString());
            lock(stream) {
                stream.Write(buffer, 0, buffer.Length);

                // Output data
                if (Data.Length > 0)
                    stream.Write(Data, 0, Data.Length);

                }
            stream.Flush();
        }



        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="aLevel">A log level.</param>
        public override void LogMessage(NLog.LogLevel aLevel)
        {
            // Default value to debug
            if (aLevel == null)
                aLevel = NLog.LogLevel.Debug;
            // if the level is not logged directly return
            if (!_logger.IsEnabled(aLevel))
                return;

            _logger.Log(aLevel, "Commande : {0}", Command);
            foreach (KeyValuePair<string, string> item in _headers)
            {
                _logger.Log(aLevel, "Header : {0}: {1}", item.Key, item.Value);
            }

            if (Data.Length > 0)
            {
                _logger.Log(aLevel, "Data :-{0}-", ASCIIEncoding.ASCII.GetString(Data));
            }
        }

        /// <summary>
        /// Crée un nouvel objet qui est une copie de l'instance en cours.
        /// </summary>
        /// <returns>
        /// Nouvel objet qui est une copie de cette instance.
        /// </returns>
        public override object Clone()
        {
            RtspMessage returnValue = GetRtspMessage(this.Command);

            foreach (var item in this.Headers)
            {
                if (item.Value == null)
                    returnValue.Headers.Add(item.Key.Clone() as string, null);
                else
                    returnValue.Headers.Add(item.Key.Clone() as string, item.Value.Clone() as string);
            }
            returnValue.Data = this.Data.Clone() as byte[];
            returnValue.SourcePort = this.SourcePort;

            return returnValue;
        }

    }
}
