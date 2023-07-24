using AltitudeAngelWings.Model;

namespace AltitudeAngelWings.Service.Messaging
{
    public class MultipleMessageDisplay : IMessageDisplay
    {
        private readonly IMessageDisplay[] _displays;

        public MultipleMessageDisplay(params IMessageDisplay[] displays)
        {
            _displays = displays;
        }

        public void AddMessage(Message message)
        {
            foreach (var display in _displays)
            {
                display.AddMessage(message);
            }
        }

        public void RemoveMessage(Message message)
        {
            foreach (var display in _displays)
            {
                display.RemoveMessage(message);
            }
        }
    }
}