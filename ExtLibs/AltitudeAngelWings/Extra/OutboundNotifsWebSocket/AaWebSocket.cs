using System;
using System.IO;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace AltitudeAngelWings.Extra.OutboundNotifsWebSocket
{
    public class AaWebSocket
    {
        private const int ReceiveBlockSize = 1024;
        private const int SendBlockSize = 1024;

        public async Task SendMessageAsync(byte[] message, WebSocketMessageType type = WebSocketMessageType.Text)
        {
            var messageCount = (int)Math.Ceiling((double)message.Length / SendBlockSize);

            var bytesRemaining = message.Length;
            for (var i = 0; i < messageCount; i++)
            {
                var offset = SendBlockSize * i;
                var lastMessage = i == messageCount - 1;

                var count = bytesRemaining < SendBlockSize ? bytesRemaining : SendBlockSize;

                await Socket.SendAsync(new ArraySegment<byte>(message, offset, count), type, lastMessage, CancellationToken.None);
            }
        }

        protected async Task Listen()
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
                            result = await Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                            if (result.MessageType == WebSocketMessageType.Close)
                            {
                                await Socket.CloseAsync(
                                    WebSocketCloseStatus.NormalClosure,
                                    string.Empty,
                                    CancellationToken.None);
                                await OnDisconnected();
                                return;
                            }

                            await message.WriteAsync(buffer, 0, result.Count);
                        } while (!result.EndOfMessage);

                        await OnMessage(message.GetBuffer());
                    }
                }
            }
            catch (Exception e)
            {
                await OnError(e);
            }
        }

        public Func<byte[], Task> OnMessage { get; set; }

        public Func<Task> OnDisconnected { get; set; }

        public Func<Exception, Task> OnError { get; set; }

        protected WebSocket Socket { get; set; }
    }
}
