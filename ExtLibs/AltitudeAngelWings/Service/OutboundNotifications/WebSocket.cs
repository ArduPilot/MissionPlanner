using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace AltitudeAngelWings.Service.OutboundNotifications
{
    public class WebSocket
    {
        private const int ReceiveBlockSize = 1024;
        private const int SendBlockSize = 1024;

        public async Task SendMessageAsync(byte[] message, CancellationToken cancellationToken = default, WebSocketMessageType type = WebSocketMessageType.Text)
        {
            var messageCount = (int)Math.Ceiling((double)message.Length / SendBlockSize);

            var bytesRemaining = message.Length;
            for (var i = 0; i < messageCount; i++)
            {
                var offset = SendBlockSize * i;
                var lastMessage = i == messageCount - 1;

                var count = bytesRemaining < SendBlockSize ? bytesRemaining : SendBlockSize;

                await Socket.SendAsync(new ArraySegment<byte>(message, offset, count), type, lastMessage, cancellationToken);
            }
        }

        protected async Task Listen(CancellationToken cancellationToken = default)
        {
            try
            {
                var buffer = new byte[ReceiveBlockSize];
                while (Socket.State == WebSocketState.Open)
                {
                    using (var message = new MemoryStream())
                    {
                        WebSocketReceiveResult result;
                        do
                        {
                            result = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await Socket.CloseAsync(
                                    WebSocketCloseStatus.NormalClosure,
                                    string.Empty,
                                    cancellationToken);
                                await OnDisconnected(cancellationToken);
                                return;
                            }

                            await message.WriteAsync(buffer, 0, result.Count, cancellationToken);
                        } while (!result.EndOfMessage);

                        await OnMessage(message.GetBuffer(), cancellationToken);
                    }
                }
            }
            catch (Exception e)
            {
                await OnError(e, cancellationToken);
            }
        }

        public Func<byte[], CancellationToken, Task> OnMessage { get; set; }

        public Func<CancellationToken, Task> OnDisconnected { get; set; }

        public Func<Exception, CancellationToken, Task> OnError { get; set; }

        protected System.Net.WebSockets.WebSocket Socket { get; set; }
    }
}
