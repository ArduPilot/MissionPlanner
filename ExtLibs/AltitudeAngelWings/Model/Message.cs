using System;

namespace AltitudeAngelWings.Model
{
    public class Message
    {
        private Action _onClick;
        private const int DefaultExpirySeconds = 3;
        private const int ClickExpirySeconds = 60;

        public static Message ForInfo(string content, TimeSpan timeToLive = default) => ForInfo(null, content, timeToLive);

        public static Message ForInfo(string key, string content, TimeSpan timeToLive = default)
        {
            var message = new Message(content)
            {
                Key = key
            };
            if (timeToLive != default)
            {
                message.HasExpired = () => message.Clicked || DateTimeOffset.UtcNow.Subtract(message.Created) >= timeToLive;
            }
            return message;
        }

        public static Message ForError(string content, Exception exception) => ForError(null, content, exception);

        public static Message ForError(string key, string content, Exception exception)
            => new Message(content)
            {
                Key = key,
                Type = MessageType.Error,
                OnClick = () => ServiceLocator.GetService<IMissionPlanner>().ShowMessageBox(exception.ToDisplayedException(), "Exception")
            };

        public static Message ForAction(string content, Action action, Func<bool> condition = null) => ForAction(null, content, action, condition);

        public static Message ForAction(string key, string content, Action action, Func<bool> condition = null)
        {
            var message = new Message(content)
            {
                Key = key,
                OnClick = action
            };
            if (condition == null)
            {
                message.HasExpired = () => message.Clicked;
            }
            else
            {
                message.HasExpired = () => message.Clicked || condition();
            }
            return message;
        }

        private Message(string content)
        {
            Content = content;
            Created = DateTimeOffset.UtcNow;
            HasExpired = () => Clicked || DateTimeOffset.UtcNow.Subtract(Created) >= TimeSpan.FromSeconds(DefaultExpirySeconds);
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
