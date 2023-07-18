using System;

namespace AltitudeAngelWings.Models
{
    public class Message
    {
        public Message(string content)
        {
            Content = content;
        }

        public Message(string content, params object[] data)
        {
            Content = string.Format(content, data);
        }

        public static implicit operator Message(string content)
        {
            return new Message(content);
        }

        public string Content { get; set; }

        /// <summary>
        ///  Sets the Time To Live for a message that does not have an
        ///  <see cref="OnClick"/> registered.
        /// </summary>
        public TimeSpan TimeToLive { get; set; } = TimeSpan.FromSeconds(3);

        /// <summary>
        ///  Sets an action to be performed when the message is clicked.
        ///  Messages with this set will only disappear after clicked, not after <see cref="TimeToLive"/>.
        /// </summary>
        public Action OnClick { get; set; }
    }
}
