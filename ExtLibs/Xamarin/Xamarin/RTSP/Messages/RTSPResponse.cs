using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Rtsp.Messages
{
    public class RtspResponse : RtspMessage
    {
        public const int DEFAULT_TIMEOUT = 60;

        /// <summary>
        /// Gets the default error message for an error code.
        /// </summary>
        /// <param name="aErrorCode">An error code.</param>
        /// <returns>The default error message associate</returns>
        private static string GetDefaultError(int aErrorCode)
        {
            switch (aErrorCode)
            {

                case 100: return "Continue";

                case 200: return "OK";
                case 201: return "Created";
                case 250: return "Low on Storage Space";

                case 300: return "Multiple Choices";
                case 301: return "Moved Permanently";
                case 302: return "Moved Temporarily";
                case 303: return "See Other";
                case 305: return "Use Proxy";

                case 400: return "Bad Request";
                case 401: return "Unauthorized";
                case 402: return "Payment Required";
                case 403: return "Forbidden";
                case 404: return "Not Found";
                case 405: return "Method Not Allowed";
                case 406: return "Not Acceptable";
                case 407: return "Proxy Authentication Required";
                case 408: return "Request Timeout";
                case 410: return "Gone";
                case 411: return "Length Required";
                case 412: return "Precondition Failed";
                case 413: return "Request Entity Too Large";
                case 414: return "Request-URI Too Long";
                case 415: return "Unsupported Media Type";
                case 451: return "Invalid parameter";
                case 452: return "Illegal Conference Identifier";
                case 453: return "Not Enough Bandwidth";
                case 454: return "Session Not Found";
                case 455: return "Method Not Valid In This State";
                case 456: return "Header Field Not Valid";
                case 457: return "Invalid Range";
                case 458: return "Parameter Is Read-Only";
                case 459: return "Aggregate Operation Not Allowed";
                case 460: return "Only Aggregate Operation Allowed";
                case 461: return "Unsupported Transport";
                case 462: return "Destination Unreachable";

                case 500: return "Internal Server Error";
                case 501: return "Not Implemented";
                case 502: return "Bad Gateway";
                case 503: return "Service Unavailable";
                case 504: return "Gateway Timeout";
                case 505: return "RTSP Version Not Supported";
                case 551: return "Option not support";
                default:
                    return "Return: " + aErrorCode.ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RtspResponse"/> class.
        /// </summary>
        public RtspResponse()
            : base()
        {
            // Initialise with a default result code.
            Command = "RTSP/1.0 200 OK";
        }

        private int _returnCode;
        /// <summary>
        /// Gets or sets the return code of the response.
        /// </summary>
        /// <value>The return code.</value>
        /// <remarks>On change the error message is set to the default one associate with the code</remarks>
        public int ReturnCode
        {
            get
            {
                if (_returnCode == 0 && commandArray.Length >= 2)
                {
                    int.TryParse(commandArray[1], out _returnCode);
                }

                return _returnCode;
            }
            set
            {
                if (ReturnCode != value)
                {
                    _returnCode = value;
                    // make sure we have the room
                    if (commandArray.Length < 3)
                    {
                        Array.Resize(ref commandArray, 3);
                    }
                    commandArray[1] = value.ToString(CultureInfo.InvariantCulture);
                    commandArray[2] = GetDefaultError(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the error/return message.
        /// </summary>
        /// <value>The return message.</value>
        public string ReturnMessage
        {
            get
            {
                if (commandArray.Length < 3)
                    return String.Empty;
                return commandArray[2];
            }
            set
            {
                // Make sure we have the room
                if (commandArray.Length < 3)
                {
                    Array.Resize(ref commandArray, 3);
                }
                commandArray[2] = value;

            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance correspond to an OK response.
        /// </summary>
        /// <value><c>true</c> if this instance is OK; otherwise, <c>false</c>.</value>
        public bool IsOk
        {
            get
            {
                if (ReturnCode > 0 && ReturnCode < 400)
                    return true;
                return false;
            }
        }

        /// <summary>
        /// Gets the timeout in second.
        /// <remarks>The default timeout is 60.</remarks>
        /// </summary>
        /// <value>The timeout.</value>
        public int Timeout
        {
            get
            {
                int returnValue = DEFAULT_TIMEOUT;
                if (Headers.ContainsKey(RtspHeaderNames.Session))
                {
                    string[] parts = Headers[RtspHeaderNames.Session].Split(';');
                    if (parts.Length > 1)
                    {
                        string[] subParts = parts[1].Split('=');
                        if (subParts.Length > 1 &&
                            subParts[0].ToUpperInvariant() == "TIMEOUT")
                            if (!int.TryParse(subParts[1], out returnValue))
                                returnValue = DEFAULT_TIMEOUT;
                    }
                }
                return returnValue;
            }
            set
            {
                if(Headers.ContainsKey(RtspHeaderNames.Session))
                    if (value != DEFAULT_TIMEOUT)
                    {
                        
                        Headers[RtspHeaderNames.Session] = Headers[RtspHeaderNames.Session].Split(';').First() 
                            + ";timeout=" + value.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        //remove timeout part
                        Headers[RtspHeaderNames.Session] = Headers[RtspHeaderNames.Session].Split(';').First();
                    }
            }
        }

        /// <summary>
        /// Gets the session ID.
        /// </summary>
        /// <value>The session ID.</value>
        public override string Session
        {
            get
            {
                if (!Headers.ContainsKey(RtspHeaderNames.Session))
                    return null;

                return Headers[RtspHeaderNames.Session].Split(';')[0];
            }
            set
            {
                if(Timeout != DEFAULT_TIMEOUT)
                {
                    Headers[RtspHeaderNames.Session] = value + ";timeout=" + Timeout.ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    Headers[RtspHeaderNames.Session] = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the original request associate with the response.
        /// </summary>
        /// <value>The original request.</value>
        public RtspRequest OriginalRequest
        { get; set; }
    }
}
