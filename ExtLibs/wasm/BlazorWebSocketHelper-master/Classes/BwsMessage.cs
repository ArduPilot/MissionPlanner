using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BlazorWebSocketHelper.Classes.BwsEnums;

namespace BlazorWebSocketHelper.Classes
{
    public class BwsMessage
    {
        public int ID { get; set; }
        public Guid GUID { get; set; }  = Guid.NewGuid();
        public DateTime Date { get; set; }
        public string Message { get; set; }
        public byte[] MessageBinary { get; set; }
        public string MessageBinaryVisual { get; set; }
        public BwsMessageType MessageType { get; set; }
        public BwsTransportType TransportType { get; set; }
    }
}
