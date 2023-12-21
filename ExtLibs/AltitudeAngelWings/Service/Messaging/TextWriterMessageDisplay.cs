using System.IO;
using AltitudeAngelWings.Model;

namespace AltitudeAngelWings.Service.Messaging
{
    public class TextWriterMessageDisplay : IMessageDisplay
    {
        private const string DefaultPrefix = "[AA] ";

        private readonly TextWriter _writer;
        private readonly string _prefix;

        public TextWriterMessageDisplay(TextWriter writer, string prefix = DefaultPrefix)
        {
            _writer = writer;
            _prefix = prefix;
        }

        public void AddMessage(Message message)
        {
            _writer.Write(_prefix);
            _writer.WriteLine(message.Content);
        }

        public void RemoveMessage(Message message)
        {
            // Do nothing
        }
    }
}