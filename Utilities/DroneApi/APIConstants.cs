using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MissionPlanner.Utilities.DroneApi
{
    class APIConstants
    {
        /**
 * The default world wide drone broker
 */
        public static String DEFAULT_SERVER = "api.3drobotics.com";


        public static String URL_BASE = "https://" + DEFAULT_SERVER;


        /**
         * If using a raw TCP link to the server, use this port number
         */
        public static int DEFAULT_TCP_PORT = 5555;


        public static String ZMQ_URL = "tcp://" + DEFAULT_SERVER + ":5556";


        public static String TLOG_MIME_TYPE = "application/vnd.mavlink.tlog";

        // Do not use this key in your own applications - please register your own.
        // https://developer.3drobotics.com/
        public static String apiKey = "614ca8bd.4d084b822a53c6eccb642271db04c937";
    }
}