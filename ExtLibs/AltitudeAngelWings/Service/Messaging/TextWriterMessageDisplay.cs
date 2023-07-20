using System.IO;
using AltitudeAngelWings.Models;

namespace AltitudeAngelWings.Service.Messaging
{
    public class TextWriterMessageDisplay : IMessageDisplay
    {
        private readonly TextWriter _writer;

        public TextWriterMessageDisplay(TextWriter writer)
        {
            _writer = writer;
        }

        public void AddMessage(Message message)
        {
            _writer.WriteLine(message.Content);
        }

        public void RemoveMessage(Message message)
        {
            // Do nothing
        }
    }
}