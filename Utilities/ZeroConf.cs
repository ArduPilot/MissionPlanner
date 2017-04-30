using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Zeroconf;

namespace MissionPlanner.Utilities
{
    public class ZeroConf
    {
        public static async Task ProbeForRTSP()
        {
            //while (Application.OpenForms.Count > 0)
            {
                IReadOnlyList<IZeroconfHost> results = await ZeroconfResolver.ResolveAsync("rtsp._udp");
            }
        }

        public static async Task EnumerateAllServicesFromAllHosts()
        {
            ILookup<string, string> domains = await ZeroconfResolver.BrowseDomainsAsync();
            var responses = await ZeroconfResolver.ResolveAsync(domains.Select(g => g.Key));
            foreach (var resp in responses)
                Console.WriteLine(resp);
        }
    }
}
