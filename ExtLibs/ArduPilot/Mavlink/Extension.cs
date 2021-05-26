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

    public static string ToHexString(this byte[] input)
    {
        StringBuilder hex = new StringBuilder(input.Length * 2);
        foreach (byte b in input)
            hex.AppendFormat("{0:x2}", b);
        return hex.ToString();
    }

    public static string WrapText(this string msg, int length, char[] spliton)
    {
        StringBuilder ans = new StringBuilder();
        int linecha = 0;
        for (int i = 0; i < msg.Length; i++)
        {
            bool splitline = false;
            if (linecha > length)
            {
                foreach (var cha in spliton)
                {
                    if (msg[i] == cha)
                    {
                        ans.Append(msg[i]);
                        ans.Append("\n");
                        splitline = true;
                        linecha = -1;
                        break;
                    }
                }
            }

            if (!splitline)
                ans.Append(msg[i]);

            linecha++;
        }

        return ans.ToString();
    }

}
