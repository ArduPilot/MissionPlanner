using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace AltitudeAngelWings.Extra.OutboundNotifsWebSocket
{
    public class AaClientWebSocket : AaWebSocket
    {
        private Task _listening;

        public AaClientWebSocket()
        {
            Socket = new ClientWebSocket();
        }

        /// <summary>
        /// Open a socket connection
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="requestHeaders">Optional headers to send with the request</param>
        /// <returns></returns>
        public async Task OpenAsync(Uri uri, Dictionary<string, string> requestHeaders = null)
        {
            await ClientSocket.ConnectAsync(uri, CancellationToken.None);

            if (ClientSocket.State != WebSocketState.Open)
            {
                throw new Exception("Failed to open connection");
            }

            await OnConnected();

            _listening = Task.Run(Listen);
        }

        /// <summary>
        /// Closes the connection and disposes the underlying socket
        /// </summary>
        /// <returns></returns>
        public async Task CloseAsync()
        {
            if (ClientSocket == null)
            {
                throw new InvalidOperationException("Socket not open");
            }

            await ClientSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
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

        public Func<Task> OnConnected { get; set; }

        protected ClientWebSocket ClientSocket =>
            Socket as ClientWebSocket ?? throw new InvalidOperationException("Invalid socket type");
    }
}
