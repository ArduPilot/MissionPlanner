namespace Rtsp.Sdp
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Globalization;

    public class ConnectionIP6 : Connection
    {
        internal new static ConnectionIP6 Parse(string ipAddress)
        {
            string[] parts = ipAddress.Split('/');

            if (parts.Length > 2)
                throw new FormatException("Too much address subpart in " + ipAddress);

            ConnectionIP6 result = new ConnectionIP6();

            result.Host = parts[0];

            int numberOfAddress;
            if (parts.Length > 1)
            {
                if (!int.TryParse(parts[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out numberOfAddress))
                    throw new FormatException("Invalid number of address : " + parts[1]);
                result.NumberOfAddress = numberOfAddress;
            }

            return result;

        }
    }
}
