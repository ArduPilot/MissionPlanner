using System;

namespace AltitudeAngelWings.Models
{
    public class Message
    {
        private Action _onClick;
        private const int DefaultExpirySeconds = 3;
        private const int ClickExpirySeconds = 60;

        public Message(string content)
            : this(content, new object[] {})
        {
        }

        public Message(string content, params object[] data)
        {
            Content = string.Format(content, data);
            Created = DateTimeOffset.UtcNow;
            HasExpired = () => Clicked || DateTimeOffset.UtcNow.Subtract(Created) >= TimeSpan.FromSeconds(DefaultExpirySeconds);
        }

        public static implicit operator Message(string content)
        {
            return new Message(content);
        }

        /// <summary>
        /// When the message was created.
        /// </summary>
        public DateTimeOffset Created { get; }

        /// <summary>
        /// Sets a key associated with the message. If another message with the same key
        /// already exists on a message display then it will be removed. A null or empty
        /// key denotes an ad-hoc message.
        /// </summary>
        public string Key { get; set; } = "";

        /// <summary>
        /// The type of message which affects how it is displayed.
        /// </summary>
        public MessageType Type { get; set; } = MessageType.Information;

        /// <summary>
        /// The content of the message.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Set the time to live on the message.
        /// </summary>
        public TimeSpan TimeToLive
        {
            set
            {
                HasExpired = () => Clicked || DateTimeOffset.UtcNow.Subtract(Created) >= value;
            }
        }

        /// <summary>
        /// Sets the function to see if the message has expired or not.
        /// </summary>
        public Func<bool> HasExpired { get; set; }

        /// <summary>
        ///  Sets an action to be performed when the message is clicked.
        /// </summary>
        public Action OnClick
        {
            get => _onClick;
            set
            {
                _onClick = () =>
                {
                    value();
                    Clicked = true;
                };
                HasExpired = () => Clicked || DateTimeOffset.UtcNow.Subtract(Created) >= TimeSpan.FromSeconds(ClickExpirySeconds);
            }
        }

        public bool Clicked { get; private set; }

        public override string ToString()
        {
            return Content;
        }
    }
}
