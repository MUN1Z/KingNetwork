using System.Net.Sockets;

namespace KingNetwork.Server.Interfaces
{
    /// <summary>
    /// This interface is responsible for represents the client.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// The id of client.
        /// </summary>
        ushort ID { get; }

        /// <summary>
        /// The ip of connected client.
        /// </summary>
        string IP { get; }

        /// <summary>
        /// The stream of client.
        /// </summary>
        NetworkStream Stream { get; }

        /// <summary>
		/// The flag of client connection.
		/// </summary>
        bool HasConnected { get; }
    }
}