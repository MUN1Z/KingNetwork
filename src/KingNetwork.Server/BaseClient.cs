using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for represents the client connection.
    /// </summary>
    public abstract class BaseClient : IClient
    {
        #region private members

        /// <summary>
        /// The tcp client instance from client.
        /// </summary>
        protected Socket _socketClient;

        /// <summary>
        /// The buffer of client connection.
        /// </summary>
        protected byte[] _buffer;

        /// <summary>
        /// The stream of tcp client.
        /// </summary>
        protected NetworkStream _stream;

        /// <summary>
        /// The callback of message received handler implementation.
        /// </summary>
        protected MessageReceivedHandler _messageReceivedHandler;

        /// <summary>
        /// The callback of client disconnected handler implementation.
        /// </summary>
        protected ClientDisconnectedHandler _clientDisconnectedHandler;

        #endregion

        #region properties

        /// <summary>
        /// The identifier number of client.
        /// </summary>
        public ushort Id { get; set; }

        /// <summary>
        /// The ip address of connected client.
        /// </summary>
        public string IpAddress { get; }

        /// <summary>
		/// The flag of client connection.
		/// </summary>
		public bool IsConnected => _socketClient.Connected;

        #endregion

        #region delegates

        /// <summary>
        /// The delegate of message received handler from client connection.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public delegate void MessageReceivedHandler(IClient client, KingBufferReader kingBuffer);

        /// <summary>
		/// The delegate of client disconnected handler connection.
		/// </summary>
        /// <param name="client">The instance of disconnected client.</param>
        public delegate void ClientDisconnectedHandler(IClient client);

        #endregion

        /// <summary>
        /// Method responsible for send message to connected server.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public abstract void SendMessage(KingBufferWriter kingBuffer);
    }
}
