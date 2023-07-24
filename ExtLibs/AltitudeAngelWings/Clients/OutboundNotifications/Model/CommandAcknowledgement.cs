namespace AltitudeAngelWings.Clients.OutboundNotifications.Model
{
    /// <summary>
    /// Acknowledges a command
    /// </summary>
    public class CommandAcknowledgement
    {
        /// <summary>
        /// Id of command being acknowledged
        /// </summary>
        public string Id
        {
            get;
            set;
        }
    }
}