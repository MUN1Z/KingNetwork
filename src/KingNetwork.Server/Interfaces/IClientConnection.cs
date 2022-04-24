using KingNetwork.Shared;

namespace KingNetwork.Server.Interfaces
{
    /// <summary>
    /// This interface is responsible for represents the client connection.
    /// </summary>
    public interface IClientConnection
    {
        /// <summary>
        /// The identification number of client.
        /// </summary>
        ushort Id { get; }

        /// <summary>
        /// The ip address of connected client.
        /// </summary>
        string IpAddress { get; }

        /// <summary>
        /// The flag of client connection.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Method responsible for send message to specific connected client.
        /// </summary>
        /// <param name="writer">The king bufferwriter  of received message.</param>
        void SendMessage(KingBufferWriter writer);

        /// <summary>
        /// Method responsible for disconnect a client connection.
        /// </summary>
        void Disconnect();
    }
}