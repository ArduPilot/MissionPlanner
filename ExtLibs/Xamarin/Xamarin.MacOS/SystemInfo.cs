using System.Linq;

namespace Xamarin.MacOS
{
    internal class SystemInfo : ISystemInfo
    {
        public string GetSystemTag()
        {
            return "";
        }

        public void StartProcess(string[] cmd)
        {
            System.Diagnostics.Process.Start(cmd[0], cmd.Skip(1).Aggregate("", (a, b) => a + " " + b));
        }
    }
}