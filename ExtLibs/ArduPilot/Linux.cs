using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MissionPlanner.Utilities
{
    public class Linux
    {
        public static List<ArduPilot.DeviceInfo> GetAllCOMPorts()
        {
            List<ArduPilot.DeviceInfo> ans = new List<ArduPilot.DeviceInfo>();
            //var proc = System.Diagnostics.Process.Start("bash", @"-c ""lsusb -tv > /tmp/lsusb.list""");
            //proc.WaitForExit();


            try
            {  
                var proc = System.Diagnostics.Process.Start("/usr/bin/bash", @"-c ""/usr/bin/find /sys/bus/usb/devices/usb*/ -name dev | /usr/bin/grep tty > /tmp/usb.list""");
                proc.WaitForExit();              

                var data = File.ReadAllLines("/tmp/usb.list");
                Console.WriteLine(data);
                foreach (var device in data)
                {
                    try
                    {
                        var pth = Path.Combine(Path.GetDirectoryName(device), "../../../");
                        var product = File.ReadAllText(pth + "product").TrimEnd();

                        ans.Add(new ArduPilot.DeviceInfo()
                        {
                            board = product,
                            description = product,
                            name = product,
                            hardwareid = $"USB\\VID_{File.ReadAllText(pth + "idVendor").TrimEnd()}&PID_{File.ReadAllText(pth + "idProduct").TrimEnd()}"
                        });
                    } catch (Exception ex) { Console.WriteLine(ex.ToString()); }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            return ans;
        }
    }
}
