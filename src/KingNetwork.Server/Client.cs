using KingNetwork.Server.Interfaces;
using System;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for represents the client.
    /// </summary>
    public class Client : IClient
    {
        #region private members

        /// <summary>
        /// The id of client.
        /// </summary>
        public Socket _socketClient { get; private set; }


        #endregion

        #region properties

        /// <summary>
        /// The id of client.
        /// </summary>
        public ushort ID { get; set; }

        /// <summary>
        /// The stream of client.
        /// </summary>
        public NetworkStream Stream { get; private set; }

        #endregion

        #region constructors

        /// <summary>
		/// Creates a new instance of a <see cref="Client"/>.
		/// </summary>
        public Client(ushort id, Socket socketClient)
        {
            try
            {
                ID = id;
                Stream = new NetworkStream(socketClient);
                _socketClient = socketClient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
