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
    }
}
