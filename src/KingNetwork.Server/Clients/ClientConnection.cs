using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for represents the client connection.
    /// </summary>
    public abstract class ClientConnection : IClientConnection
    {
        #region private members

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

        /// <inheritdoc/>
        public ushort Id { get; set; }

        /// <inheritdoc/>
        public abstract string IpAddress { get; }

        /// <inheritdoc/>
		public abstract bool IsConnected { get; }

        #endregion

        #region delegates

        /// <summary>
        /// The delegate of message received handler from client connection.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="reader">The king buffer reader of received message.</param>
        public delegate void MessageReceivedHandler(ClientConnection client, KingBufferReader reader);

        /// <summary>
		/// The delegate of client disconnected handler connection.
		/// </summary>
        /// <param name="client">The instance of disconnected client.</param>
        public delegate void ClientDisconnectedHandler(ClientConnection client);

        #endregion

        /// <inheritdoc/>
        public abstract void SendMessage(KingBufferWriter writer);

        /// <inheritdoc/>
        public abstract void Disconnect();
    }
}
