using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rtsp.Messages
{
    public class RtspRequestSetup : RtspRequest
    {

        // Constructor
        public RtspRequestSetup()
        {
            Command = "SETUP * RTSP/1.0";
        }


        /// <summary>
        /// Gets the transports associate with the request.
        /// </summary>
        /// <value>The transport.</value>
        public RtspTransport[] GetTransports()
        {

            if (!Headers.ContainsKey(RtspHeaderNames.Transport))
                return new RtspTransport[] { new RtspTransport() };

            string[] items = Headers[RtspHeaderNames.Transport].Split(',');
            return Array.ConvertAll<string, RtspTransport>(items,
                new Converter<string, RtspTransport>(RtspTransport.Parse));

        }

        public void AddTransport(RtspTransport newTransport)
        {
            string actualTransport = string.Empty;
            if(Headers.ContainsKey(RtspHeaderNames.Transport))
                actualTransport = Headers[RtspHeaderNames.Transport] + ",";
            Headers[RtspHeaderNames.Transport] = actualTransport + newTransport.ToString();



        }

    }
}
