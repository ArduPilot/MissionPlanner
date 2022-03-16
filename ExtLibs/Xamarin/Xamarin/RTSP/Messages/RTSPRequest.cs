using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Rtsp.Messages
{
    /// <summary>
    /// An Rtsp Request
    /// </summary>
    public class RtspRequest : RtspMessage
    {

        /// <summary>
        /// Request type.
        /// </summary>
        public enum RequestType
        {
            UNKNOWN,
            DESCRIBE,
            ANNOUNCE,
            GET_PARAMETER,
            OPTIONS,
            PAUSE,
            PLAY,
            RECORD,
            REDIRECT,
            SETUP,
            SET_PARAMETER,
            TEARDOWN,
        }

        /// <summary>
        /// Parses the request command.
        /// </summary>
        /// <param name="aStringRequest">A string request command.</param>
        /// <returns>The typed request.</returns>
        internal static RequestType ParseRequest(string aStringRequest)
        {
            RequestType returnValue;
            if (!Enum.TryParse<RequestType>(aStringRequest, true, out returnValue))
                returnValue = RequestType.UNKNOWN;
            return returnValue;
        }

        /// <summary>
        /// Gets the Rtsp request.
        /// </summary>
        /// <param name="aRequestParts">A request parts.</param>
        /// <returns>the parsed request</returns>
        internal static RtspMessage GetRtspRequest(string[] aRequestParts)
        {
            // <pex>
            Debug.Assert(aRequestParts != (string[])null, "aRequestParts");
            Debug.Assert(aRequestParts.Length != 0, "aRequestParts.Length == 0");
            // </pex>
            // we already know this is a Request
            RtspRequest returnValue;
            switch (ParseRequest(aRequestParts[0]))
            {
                case RequestType.OPTIONS:
                    returnValue = new RtspRequestOptions();
                    break;
                case RequestType.DESCRIBE:
                    returnValue = new RtspRequestDescribe();
                    break;
                case RequestType.SETUP:
                    returnValue = new RtspRequestSetup();
                    break;
                case RequestType.PLAY:
                    returnValue = new RtspRequestPlay();
                    break;
                case RequestType.PAUSE:
                    returnValue = new RtspRequestPause();
                    break;
                case RequestType.TEARDOWN:
                    returnValue = new RtspRequestTeardown();
                    break;
                case RequestType.GET_PARAMETER:
                    returnValue = new RtspRequestGetParameter();
                    break;
                case RequestType.ANNOUNCE:
                    returnValue = new RtspRequestAnnounce();
                    break;
                case RequestType.RECORD:
                    returnValue = new RtspRequestRecord();
                    break;
                    /*
                case RequestType.REDIRECT:
                    break;
                
                case RequestType.SET_PARAMETER:
                    break;
                     */
                case RequestType.UNKNOWN:
                default:
                    returnValue = new RtspRequest();
                    break;
            } 


             
            return returnValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RtspRequest"/> class.
        /// </summary>
        public RtspRequest()
        {
            Command = "OPTIONS * RTSP/1.0";
        }

        /// <summary>
        /// Gets the request.
        /// </summary>
        /// <value>The request in string format.</value>
        public string Request
        {
            get
            {
                return commandArray[0];
            }
        }

        /// <summary>
        /// Gets the request.
        /// <remarks>The return value is typed with <see cref="Rtsp.RequestType"/> if the value is not
        /// reconise the value is sent. The string value can be get by <see cref="Request"/></remarks>
        /// </summary>
        /// <value>The request.</value>
        public RequestType RequestTyped
        {
            get
            {
                return ParseRequest(commandArray[0]);
            }
            set
            {
                if (Enum.IsDefined(typeof(RequestType), value))
                    commandArray[0] = value.ToString();
                else
                    commandArray[0] = RequestType.UNKNOWN.ToString();
            }
        }

        private Uri _RtspUri;
        /// <summary>
        /// Gets or sets the Rtsp asked URI.
        /// </summary>
        /// <value>The Rtsp asked URI.</value>
        /// <remarks>The request with uri * is return with null URI</remarks>
        public Uri RtspUri
        {
            get
            {
                if (commandArray.Length < 2 || commandArray[1]=="*")
                    return null;
                if (_RtspUri == null)
                    Uri.TryCreate(commandArray[1], UriKind.Absolute, out _RtspUri);
                return _RtspUri;
            }
            set
            {
                _RtspUri = value;
                if (commandArray.Length < 2)
                {
                    Array.Resize(ref commandArray, 3);
                }
                commandArray[1] = (value != null ? value.ToString().TrimEnd('/') : "*");
            }
        }

        /// <summary>
        /// Gets the assiociate OK response with the request.
        /// </summary>
        /// <returns>an Rtsp response correcponding to request.</returns>
        public virtual RtspResponse CreateResponse()
        {
            RtspResponse returnValue = new RtspResponse();
            returnValue.ReturnCode = 200;
            returnValue.CSeq = this.CSeq;
            if (this.Headers.ContainsKey(RtspHeaderNames.Session))
            {
                returnValue.Headers[RtspHeaderNames.Session] = this.Headers[RtspHeaderNames.Session]; 
            }

            return returnValue;
        }

        public Object ContextData { get; set; }
    }
}
