using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Net;

namespace Rtsp.Sdp
{
    public abstract class Connection
    {
        public Connection()
        {
            //Default value from spec
            NumberOfAddress = 1;
        }

        public string Host { get; set; }

        /// <summary>
        /// Gets or sets the number of address specifed in connection.
        /// </summary>
        /// <value>The number of address.</value>
        //TODO handle it a different way (list of adress ?)
        public int NumberOfAddress { get; set; }

        public static Connection Parse(string value)
        {
            if(value ==null)
                throw new ArgumentNullException("value");

            string[] parts = value.Split(' ');

            if (parts.Length != 3)
                throw new FormatException("Value do not contain 3 parts as needed.");

            if (parts[0] != "IN")
                throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Net type {0} not suported", parts[0]));

            switch (parts[1])
            {
                case "IP4":
                    return ConnectionIP4.Parse(parts[2]);
                case "IP6":
                    return ConnectionIP6.Parse(parts[2]);
                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Address type {0} not suported", parts[1]));
            }
            
        }
    }
}
