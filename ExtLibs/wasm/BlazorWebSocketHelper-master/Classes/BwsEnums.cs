using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebSocketHelper.Classes
{
    public class BwsEnums
    {
        public enum BwsMessageType
        {
            send,
            received,
        }


        public enum BwsTransportType
        {
            Text,
            ArrayBuffer,
            Blob,
         //   ArrayBufferView,
        }

        public enum BwsState
        {
            Open,
            Closing,
            Close,
            Error,
            Undefined,
            Connecting
        }
    }


    
}
