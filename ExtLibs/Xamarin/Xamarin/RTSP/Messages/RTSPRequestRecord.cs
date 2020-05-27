namespace Rtsp.Messages
{
    public class RtspRequestRecord : RtspRequest
    {
        public RtspRequestRecord()
        {
            Command = "RECORD * RTSP/1.0";
        }
    }
}