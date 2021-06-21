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
        public static List<IZeroconfHost> RtspHosts = new List<IZeroconfHost>();

        public delegate void ZeroConfHost(IZeroconfHost zeroconfHost);

        public static event ZeroConfHost StartUDPMavlink;

        public static void ProbeForRTSP()
        {
            resolverAsync();
        }

        private static async Task resolverAsync()
        {
            await Task.Delay(6000).ConfigureAwait(false);
            while (true)
            {
                try
                {
                    var results = await ZeroconfResolver.ResolveAsync("_rtsp._udp.local.").ConfigureAwait(false);

                    if (results != null)
                    {
                        foreach (var zeroconfHost in results)
                        {
                            Console.WriteLine("Stream " + zeroconfHost);
                            if (!RtspHosts.Contains(zeroconfHost))
                                RtspHosts.Add(zeroconfHost);
                        }
                    }
                }
                catch
                {
                    
                }
                
                await Task.Delay(30000).ConfigureAwait(false);
            }
        }

        public static async Task EnumerateAllServicesFromAllHosts()
        {
            ILookup<string, string> domains = await ZeroconfResolver.BrowseDomainsAsync();
            var responses = await ZeroconfResolver.ResolveAsync(domains.Select(g => g.Key));
            foreach (var resp in responses)
                Console.WriteLine(resp);
        }

        public static void ProbeForMavlink()
        {
            resolverMavlinkAsync();
        }

        private static async Task resolverMavlinkAsync()
        {
            while (true)
            {
                try
                {
                    var results = await ZeroconfResolver.ResolveAsync("_mavlink._udp.local.").ConfigureAwait(false);

                    if (results != null)
                    {
                        foreach (var zeroconfHost in results)
                        {
                            Console.WriteLine("Mavlink " + zeroconfHost);
                            var service = zeroconfHost.Services.Where(a => a.Key == "_mavlink._udp.local.");
                            if (service.Any())
                            {
                                StartUDPMavlink?.Invoke(zeroconfHost);
                            }
                        }
                    }
                }
                catch
                {

                }

                await Task.Delay(30000).ConfigureAwait(false);
            }
        }
    }
}
