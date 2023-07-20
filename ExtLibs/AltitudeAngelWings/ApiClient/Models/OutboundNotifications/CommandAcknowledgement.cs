namespace AltitudeAngelWings.ApiClient.Models.OutboundNotifications
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