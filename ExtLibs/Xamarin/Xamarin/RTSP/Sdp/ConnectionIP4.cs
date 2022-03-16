namespace Rtsp.Sdp
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Net;

    public class ConnectionIP4 : Connection
    {

        public int Ttl { get; set; }

        internal new static ConnectionIP4 Parse(string ipAddress)
        {
            string[] parts = ipAddress.Split('/');

            if (parts.Length > 3)
                throw new FormatException("Too much address subpart in " + ipAddress);

            ConnectionIP4 result = new ConnectionIP4();

            result.Host = parts[0];

            int ttl;
            if (parts.Length > 1)
            {
                if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out ttl))
                    throw new FormatException("Invalid TTL format : " + parts[1]);
                result.Ttl = ttl;
            }
            int numberOfAddress;
            if (parts.Length > 2)
            {
                if (!int.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out numberOfAddress))
                    throw new FormatException("Invalid number of address : " + parts[2]);
                result.NumberOfAddress = numberOfAddress;
            }

            return result;
        }
    }
}
