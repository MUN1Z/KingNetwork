namespace KingNetwork.Shared
{
    /// <summary>
    /// This class is responsible for represents the connections settings.
    /// </summary>
    public class ConnectionSettings
    {
        /// <summary>
        /// The max length of message buffer.
        /// </summary>
        public static readonly ushort MAX_MESSAGE_BUFFER = 4096;

        // <summary>
        /// The number max of connected clients.
        /// </summary>
        public static readonly ushort MAX_CLIENT_CONNECTIONS = 10000;
    }
}
