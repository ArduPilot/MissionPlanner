using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zeroconf;

namespace MissionPlanner.Utilities
{
    public class ZeroConf
    {
        public static List<IZeroconfHost> Hosts = new List<IZeroconfHost>();

        public static void ProbeForRTSP()
        {
            Thread th = new Thread(resolverAsync);
            th.IsBackground = true;
            th.Start();
        }

        private static void resolverAsync()
        {
            while (true)
            {
                try
                {
                    var results = ZeroconfResolver.ResolveAsync("_rtsp._udp.local.");

                    if (results != null)
                    {
                        foreach (var zeroconfHost in results.Result)
                        {
                            Console.WriteLine("Stream " + zeroconfHost);
                            if (!Hosts.Contains(zeroconfHost))
                                Hosts.Add(zeroconfHost);
                        }
                    }
                }
                catch
                {
                    
                }

                Thread.Sleep(4000);
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
