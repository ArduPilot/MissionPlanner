using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace Rtsp.Sdp
{
    public class EncriptionKey
    {
        public EncriptionKey(string p)
        {
        }

        public static EncriptionKey ParseInvariant(string value)
        {
            if (value == null)
                throw new ArgumentNullException("value");

            Contract.EndContractBlock();

            throw new NotImplementedException();
        }
    }
}
