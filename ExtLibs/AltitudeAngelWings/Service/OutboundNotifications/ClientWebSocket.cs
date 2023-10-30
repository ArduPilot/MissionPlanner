using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace AltitudeAngelWings.Service.OutboundNotifications
{
    public class ClientWebSocket : WebSocket
    {
        private Task _listening;

        public ClientWebSocket()
        {
            Socket = new System.Net.WebSockets.ClientWebSocket();
        }

        /// <summary>
        /// Open a socket connection
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="requestHeaders">Optional headers to send with the request</param>
        /// <returns></returns>
        public async Task OpenAsync(Uri uri, CancellationToken cancellationToken,
            Dictionary<string, string> requestHeaders = null)
        {
            await ClientSocket.ConnectAsync(uri, CancellationToken.None);

            if (ClientSocket.State != WebSocketState.Open)
            {
                throw new Exception("Failed to open connection");
            }

            await OnConnected(cancellationToken);

            _listening = Task.Run(() => Listen(cancellationToken), cancellationToken);
        }

        /// <summary>
        /// Closes the connection and disposes the underlying socket
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task CloseAsync(CancellationToken cancellationToken)
        {
            if (ClientSocket == null)
            {
                throw new InvalidOperationException("Socket not open");
            }

            await ClientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken);
            await _listening;

            var socket = Socket;
            Socket = null;
            socket.Dispose();
        }

        public void SetSocketHeaders(Dictionary<string, string> headers)
        {
            if (headers == null)
            {
                return;
            }

            foreach (var header in headers)
            {
                ClientSocket.Options.SetRequestHeader(header.Key, header.Value);
            }
        }

        public Func<CancellationToken, Task> OnConnected { get; set; }

        protected System.Net.WebSockets.ClientWebSocket ClientSocket =>
            Socket as System.Net.WebSockets.ClientWebSocket ?? throw new InvalidOperationException("Invalid socket type");
    }
}
