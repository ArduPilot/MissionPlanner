using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Heijden.DNS;

namespace Zeroconf
{
    /// <summary>
    ///     Looks for ZeroConf devices
    /// </summary>
    public static partial class ZeroconfResolver
    {
        static readonly AsyncLock ResolverLock = new AsyncLock();

        static readonly INetworkInterface NetworkInterface = LoadPlatformNetworkInterface();

        static INetworkInterface LoadPlatformNetworkInterface()
        {
#if NETSTANDARD1_0
            throw new NotSupportedException("This PCL assembly must not be used at runtime. Make sure to add the Zeroconf Nuget reference to your main project.");
#else
            return new NetworkInterface();
#endif
        }





        static IEnumerable<string> BrowseResponseParser(Response response)
        {
            return response.RecordsPTR.Select(ptr => ptr.PTRDNAME);
        }


        static async Task<IDictionary<string, Response>> ResolveInternal(ZeroconfOptions options,
                                                                         Action<string, Response> callback,
                                                                         CancellationToken cancellationToken)
        {
            var requestBytes = GetRequestBytes(options);
            using (await ResolverLock.LockAsync())
            {
                cancellationToken.ThrowIfCancellationRequested();
                var dict = new Dictionary<string, Response>();

                Action<string, byte[]> converter =
                    (address, buffer) =>
                    {
                        var resp = new Response(buffer);
                        Debug.WriteLine($"IP: {address}, Bytes: {buffer.Length}, IsResponse: {resp.header.QR}");

                        if (resp.header.QR)
                        {
                            lock (dict)
                            {
                                dict[address] = resp;
                            }

                            callback?.Invoke(address, resp);
                        }
                    };

                Debug.WriteLine($"Looking for {string.Join(", ", options.Protocols)} with scantime {options.ScanTime}");

                await NetworkInterface.NetworkRequestAsync(requestBytes,
                                                           options.ScanTime,
                                                           options.Retries,
                                                           (int)options.RetryDelay.TotalMilliseconds,
                                                           converter,                                                           
                                                           cancellationToken)
                                      .ConfigureAwait(false);

                return dict;
            }
        }

        static byte[] GetRequestBytes(ZeroconfOptions options)
        {
            var req = new Request();
            var queryType = options.ScanQueryType == ScanQueryType.Ptr ? QType.PTR : QType.ANY;

            foreach (var protocol in options.Protocols)
            {
                var question = new Question(protocol, queryType, QClass.ANY);

                req.AddQuestion(question);
            }

            return req.Data;
        }

        static ZeroconfHost ResponseToZeroconf(Response response, string remoteAddress)
        {
            var z = new ZeroconfHost();

            // Get the Id and IP address from the A record
            var aRecord = response.Answers
                                  .Select(r => r.RECORD)
                                  .OfType<RecordA>()
                                  .FirstOrDefault();

            if (aRecord != null)
            {
                z.Id = aRecord.RR.NAME.Split('.')[0];
                z.IPAddress = aRecord.Address;
            }
            else
            {
                // Is this valid?
                z.Id = remoteAddress;
                z.IPAddress = remoteAddress;
            }

            var dispNameSet = false;
           
            foreach (var ptrRec in response.RecordsPTR)
            {
                // set the display name if needed
                if (!dispNameSet)
                {
                    z.DisplayName = ptrRec.PTRDNAME.Split('.')[0];
                    dispNameSet = true;
                }

                // Get the matching service records
                var responseRecords = response.RecordsRR
                                             .Where(r => r.NAME == ptrRec.PTRDNAME)
                                             .Select(r => r.RECORD)
                                             .ToList();

                var srvRec = responseRecords.OfType<RecordSRV>().FirstOrDefault();
                if (srvRec == null)
                    continue; // Missing the SRV record, not valid

                var svc = new Service
                {
                    Name = ptrRec.RR.NAME,
                    Port = srvRec.PORT,
                    Ttl = (int)srvRec.RR.TTL,
                    PTR = ptrRec.PTRDNAME.Split('.')[0]
                };

                // There may be 0 or more text records - property sets
                foreach (var txtRec in responseRecords.OfType<RecordTXT>())
                {
                    var set = new Dictionary<string, string>();
                    foreach (var txt in txtRec.TXT)
                    {
                        var split = txt.Split(new[] {'='}, 2);
                        if (split.Length == 1)
                        {
                            if (!string.IsNullOrWhiteSpace(split[0]))
                                set[split[0]] = null;
                        }
                        else
                        {
                            set[split[0]] = split[1];
                        }
                    }
                    svc.AddPropertySet(set);
                }

                z.AddService(svc);
            }

            return z;
        }


    }
}
