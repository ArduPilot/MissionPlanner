using Newtonsoft.Json;
using System;
using System.Text;

public static class Extension
{
    public static string ToJSON(this MAVLink.MAVLinkMessage msg)
    {
        return JsonConvert.SerializeObject(msg);
    }

    public static MAVLink.MAVLinkMessage FromJSON(this string msg)
    {
        return JsonConvert.DeserializeObject<MAVLink.MAVLinkMessage>(msg);
    }

    public static byte[] MakeSize(this byte[] buffer, int length)
    {
        if (buffer.Length == length)
            return buffer;
        Array.Resize(ref buffer, length);
        return buffer;
    }

    public static byte[] MakeBytesSize(this string item, int length)
    {
        var buffer = ASCIIEncoding.ASCII.GetBytes(item);
        if (buffer.Length == length)
            return buffer;
        Array.Resize(ref buffer, length);
        return buffer;
    }
}
