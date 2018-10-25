using System.Net.Sockets;

namespace KingNetwork.Server.Interfaces
{
    /// <summary>
    /// This interface is responsible for represents the client.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// The key number of client.
        /// </summary>
        ushort Key { get; }

        /// <summary>
        /// The ip address of connected client.
        /// </summary>
        string IpAddress { get; }

        /// <summary>
        /// The stream of client.
        /// </summary>
        NetworkStream Stream { get; }

        /// <summary>
		/// The flag of client connection.
		/// </summary>
        bool IsConnected { get; }
    }
}