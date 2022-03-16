using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Zeroconf
{
    class NetworkInterface : INetworkInterface
    {
        public async Task NetworkRequestAsync(byte[] requestBytes,
                                              TimeSpan scanTime,
                                              int retries,
                                              int retryDelayMilliseconds,
                                              Action<string, byte[]> onResponse,
                                              CancellationToken cancellationToken)
        {
            using (var socket = new DatagramSocket())
            {
                // setup delegate to detach from later
                TypedEventHandler<DatagramSocket, DatagramSocketMessageReceivedEventArgs> handler =
                    (sock, args) =>
                    {
                        var dr = args.GetDataReader();
                        var buffer = dr.ReadBuffer(dr.UnconsumedBufferLength).ToArray();

                        onResponse(args.RemoteAddress.CanonicalName.ToString(), buffer);
                    };

                socket.MessageReceived += handler;
                var socketBound = false;

                for (var i = 0; i < retries; i++)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        await BindToSocketAndWriteQuery(socket,
                                                        requestBytes,
                                                        cancellationToken).ConfigureAwait(false);
                        socketBound = true;
                    }
                    catch (Exception e)
                    {
                        socketBound = false;

                        // If we were canceled, don't retry
                        if (e is OperationCanceledException)
                            throw;

                        Debug.WriteLine("Exception trying to Bind:\n{0}", e);

                        // Most likely a fatal error
                        if (SocketError.GetStatus(e.HResult) == SocketErrorStatus.Unknown)
                            throw;

                        // If we're not connected on the last retry, rethrow the underlying exception
                        if (i + 1 >= retries)
                            throw;
                    }

                    if (socketBound)
                        break;

                    Debug.WriteLine("Retrying in {0} ms", retryDelayMilliseconds);
                    // Not found, wait to try again
                    await Task.Delay(retryDelayMilliseconds, cancellationToken).ConfigureAwait(false);
                }

                if (socketBound)
                {
                    // wait for responses
                    await Task.Delay(scanTime, cancellationToken).ConfigureAwait(false);
                    Debug.WriteLine("Done Scanning");
                }
#if WINDOWS_UWP
                socket.MessageReceived -= handler;
#endif
            }
        }

        static async Task BindToSocketAndWriteQuery(DatagramSocket socket, byte[] bytes, CancellationToken cancellationToken)
        {
#if WINDOWS_UWP
            // Set control option for multicast. This enables re-use of the port, which is always in use under Windows 10 otherwise.
            socket.Control.MulticastOnly = true;
#endif
            await socket.BindServiceNameAsync("5353") // binds to the local IP addresses of all network interfaces on the local computer if no adapter is specified
                        .AsTask(cancellationToken)
                        .ConfigureAwait(false);

            socket.JoinMulticastGroup(new HostName("224.0.0.251"));
            var os = await socket.GetOutputStreamAsync(new HostName("224.0.0.251"), "5353")
                                 .AsTask(cancellationToken)
                                 .ConfigureAwait(false);

            using (var writer = new DataWriter(os))
            {
                writer.WriteBytes(bytes);
                await writer.StoreAsync()
                            .AsTask(cancellationToken)
                            .ConfigureAwait(false);

                Debug.WriteLine("Sent mDNS query");

                writer.DetachStream();
            }
        }

        public Task ListenForAnnouncementsAsync(Action<AdapterInformation, string, byte[]> callback, CancellationToken cancellationToken)
        {
#if WINDOWS_UWP
             return ListenForAnnouncementsAsync(NetworkInformation.GetInternetConnectionProfile()?.NetworkAdapter, callback, cancellationToken);
#else
            throw new NotImplementedException("Windows RT does not support socket address reuse, which makes listening virtually impossible, as most users have browsers and Apple Bonjour running which already listens to 5353");
#endif
        }

#if WINDOWS_UWP
        // listen for announcements on a given adapter
        Task ListenForAnnouncementsAsync(NetworkAdapter adapter, Action<AdapterInformation, string, byte[]> callback, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(async () =>
            {
                using (var socket = new DatagramSocket())
                {
                    // setup delegate to detach from later
                    TypedEventHandler<DatagramSocket, DatagramSocketMessageReceivedEventArgs> handler =
                        (sock, args) =>
                        {
                            var dr = args.GetDataReader();
                            var buffer = dr.ReadBuffer(dr.UnconsumedBufferLength).ToArray();

                            callback(new AdapterInformation(args.LocalAddress.CanonicalName.ToString(), args.LocalAddress.RawName.ToString()), args.RemoteAddress.RawName.ToString(), buffer);
                        };

                    socket.MessageReceived += handler;

                    // Set control option for multicast. This enables re-use of the port, which is always in use under Windows 10 otherwise.
                    socket.Control.MulticastOnly = true;

                    if (adapter != null)
                    {
                        await socket.BindServiceNameAsync("5353", adapter)
                               .AsTask(cancellationToken)
                               .ConfigureAwait(false);
                    }
                    else
                    {
                        await socket.BindServiceNameAsync("5353")
                                    .AsTask(cancellationToken)
                                    .ConfigureAwait(false);
                    }

                    socket.JoinMulticastGroup(new HostName("224.0.0.251"));

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(5000, cancellationToken).ConfigureAwait(false);
                    }
                    socket.MessageReceived -= handler;
                    socket.Dispose();
                }

            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default).Unwrap();
        }
#endif

    }
}

