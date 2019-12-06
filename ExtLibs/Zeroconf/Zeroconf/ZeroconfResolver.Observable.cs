using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;

namespace Zeroconf
{
    static partial class ZeroconfResolver
    {
        /// <summary>
        ///     Resolves available ZeroConf services
        /// </summary>
        /// <param name="scanTime">Default is 2 seconds</param>
        /// <param name="protocol"></param>
        /// <param name="retries">If the socket is busy, the number of times the resolver should retry</param>
        /// <param name="retryDelayMilliseconds">The delay time between retries</param>
        /// <returns></returns>
        public static IObservable<IZeroconfHost> Resolve(string protocol,
                                                         TimeSpan scanTime = default(TimeSpan),
                                                         int retries = 2,
                                                         int retryDelayMilliseconds = 2000)
        {
            if (string.IsNullOrWhiteSpace(protocol))
                throw new ArgumentNullException(nameof(protocol));

            return Resolve(new[] { protocol }, scanTime, retries, retryDelayMilliseconds);
        }

        /// <summary>
        ///     Resolves available ZeroConf services
        /// </summary>
        /// <param name="scanTime">Default is 2 seconds</param>
        /// <param name="protocols"></param>
        /// <param name="retries">If the socket is busy, the number of times the resolver should retry</param>
        /// <param name="retryDelayMilliseconds">The delay time between retries</param>
        /// <returns></returns>
        public static IObservable<IZeroconfHost> Resolve(IEnumerable<string> protocols,
                                                         TimeSpan scanTime = default(TimeSpan),
                                                         int retries = 2,
                                                         int retryDelayMilliseconds = 2000)
        {
            if (retries <= 0) throw new ArgumentOutOfRangeException(nameof(retries));
            if (retryDelayMilliseconds <= 0) throw new ArgumentOutOfRangeException(nameof(retryDelayMilliseconds));
            if (scanTime == default(TimeSpan))
                scanTime = TimeSpan.FromSeconds(2);

            var options = new ResolveOptions(protocols)
            {
                Retries = retries,
                RetryDelay = TimeSpan.FromMilliseconds(retryDelayMilliseconds),
                ScanTime = scanTime
            };

            return Resolve(options); 
        }

        /// <summary>
        ///     Resolves available ZeroConf services
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IObservable<IZeroconfHost> Resolve(ResolveOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));


            return Observable.Create<IZeroconfHost>(
                async (obs, cxl) =>
                {
                    try
                    {
                        Action<IZeroconfHost> cb = obs.OnNext;
                        await ResolveAsync(options, cb, cxl);
                    }
                    catch (OperationCanceledException)
                    {
                        // Nothing to do here, eat it and mark completed
                    }
                    catch (Exception e)
                    {
                        obs.OnError(e);
                    }
                    finally
                    {
                        obs.OnCompleted();
                    }

                });
        }

        public static IObservable<DomainService> BrowseDomains(TimeSpan scanTime = default(TimeSpan),
                                                               int retries = 2,
                                                               int retryDelayMilliseconds = 2000)
        {
            if (retries <= 0) throw new ArgumentOutOfRangeException(nameof(retries));
            if (retryDelayMilliseconds <= 0) throw new ArgumentOutOfRangeException(nameof(retryDelayMilliseconds));
            if (scanTime == default(TimeSpan))
                scanTime = TimeSpan.FromSeconds(2);

            var options = new BrowseDomainsOptions
            {
                Retries = retries,
                RetryDelay = TimeSpan.FromMilliseconds(retryDelayMilliseconds),
                ScanTime = scanTime
            };

            return BrowseDomains(options);
        }

        public static IObservable<DomainService> BrowseDomains(BrowseDomainsOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));

            return Observable.Create<DomainService>(
                async (obs, cxl) =>
                {
                    try
                    {
                        Action<string, string> cb = (d, s) => obs.OnNext(new DomainService(d, s));
                        await BrowseDomainsAsync(options, cb, cxl);
                    }
                    catch (OperationCanceledException)
                    { }
                    catch (Exception e)
                    {
                        obs.OnError(e);
                    }
                    finally
                    {
                        obs.OnCompleted();
                    }
                });
        }


        /// <summary>
        /// Listens for mDNS Service Announcements
        /// </summary>
        /// <returns></returns>
        public static IObservable<ServiceAnnouncement> ListenForAnnouncementsAsync()
        {
            return Observable.Create<ServiceAnnouncement>(
                async (obs, cxl) =>
                {
                    try
                    {
                        await ListenForAnnouncementsAsync(obs.OnNext, cxl);
                    }
                    catch (OperationCanceledException)
                    { }
                    catch (Exception e)
                    {
                        obs.OnError(e);
                    }
                    finally
                    {
                        obs.OnCompleted();
                    }
                });
        }


        /// <summary>
        ///     Resolves available ZeroConf services continuously until disposed
        /// </summary>
        /// <param name="scanTime">Default is 2 seconds</param>
        /// <param name="protocols"></param>
        /// <param name="retries">If the socket is busy, the number of times the resolver should retry</param>
        /// <param name="retryDelayMilliseconds">The delay time between retries</param>
        /// <returns></returns>
        public static IObservable<IZeroconfHost> ResolveContinuous(IEnumerable<string> protocols,
                                                         TimeSpan scanTime = default(TimeSpan),
                                                         int retries = 2,
                                                         int retryDelayMilliseconds = 2000)
        {



            var inner = Resolve(protocols, scanTime, retries, retryDelayMilliseconds);


            return inner.Repeat().Distinct();
            
        }

        /// <summary>
        ///     Resolves available ZeroConf services continuously until disposed
        /// </summary>
        /// <param name="scanTime">Default is 2 seconds</param>
        /// <param name="protocol"></param>
        /// <param name="retries">If the socket is busy, the number of times the resolver should retry</param>
        /// <param name="retryDelayMilliseconds">The delay time between retries</param>
        public static IObservable<IZeroconfHost> ResolveContinuous(string protocol,
                                                                   TimeSpan scanTime = default(TimeSpan),
                                                                   int retries = 2,
                                                                   int retryDelayMilliseconds = 2000)
        {
            return ResolveContinuous(new[] { protocol }, scanTime, retries, retryDelayMilliseconds);
        }

        public static IObservable<DomainService> BrowseDomainsContinuous(TimeSpan scanTime = default(TimeSpan),
                                                                             int retries = 2,
                                                                             int retryDelayMilliseconds = 2000)
        {
            return BrowseDomains(scanTime, retries, retryDelayMilliseconds)
                   .Repeat()
                   .Distinct();
        }
    }
}
