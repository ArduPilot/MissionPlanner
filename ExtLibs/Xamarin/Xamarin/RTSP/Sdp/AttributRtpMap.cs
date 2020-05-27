using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rtsp.Sdp
{
    public class AttributRtpMap : Attribut
    {
        // Format
        //   rtpmap:<payload type> <encoding name>/<clock rate> [/<encoding parameters>] 
        // Examples
        //   rtpmap:96 H264/90000
        //   rtpmap:8 PCMA/8000

        public const string NAME = "rtpmap";

        public AttributRtpMap()
        {
        }

        public override string Key
        {
            get
            {
                return NAME;
            }
        }

        public override string Value
        {
            get
            {
                if(string.IsNullOrEmpty(EncodingParameters))
                {
                    return string.Format("{0} {1}/{2}", PayloadNumber, EncodingName, ClockRate);
                } else {
                    return string.Format("{0} {1}/{2}/{3}", PayloadNumber, EncodingName, ClockRate, EncodingParameters);
                }
            }
            protected set
            {
                ParseValue(value);
            }
        }

        public int PayloadNumber { get; set; }
        public String EncodingName { get; set; }
        public String ClockRate { get; set; }
        public String EncodingParameters { get; set; }

        protected override void ParseValue(string value)
        {
            var parts = value.Split(new char[] { ' ', '/' });

            if (parts.Length >= 1) {
                int tmp_payloadNumber;
                if (int.TryParse(parts[0], out tmp_payloadNumber))
                {
                    PayloadNumber = tmp_payloadNumber;
                }
            }
            if (parts.Length >= 2)
            {
                EncodingName = parts[1];
            }
            if (parts.Length >= 3)
            {
                ClockRate = parts[2];
            }
            if (parts.Length >= 4)
            {
                EncodingParameters = parts[3];
            }


        }
    }
}
