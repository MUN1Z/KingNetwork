using KingNetwork.Server.Interfaces;
using System;
using System.Net.Sockets;
using System.Threading;
using static KingNetwork.Server.Client;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for representation of abstract network listener.
    /// </summary>
    public abstract class NetworkListener : INetworkListener, IDisposable
    {
        #region private members

        /// <summary>
        /// The callback of client connected handler implementation.
        /// </summary>
        protected ClientConnectedHandler _clientConnectedHandler;

        /// <summary>
        /// The callback of message received handler implementation.
        /// </summary>
        protected MessageReceivedHandler _messageReceivedHandler;

        /// <summary>
        /// The callback of client disconnected handler implementation.
        /// </summary>
        protected ClientDisconnectedHandler _clientDisconnectedHandler;

        // <summary>
        /// The max length of message buffer.
        /// </summary>
        protected ushort _maxMessageBuffer;

        /// <summary>
		/// The listener for tcp connection.
		/// </summary>
        protected Socket _listener;

        /// <summary>
		/// The value for dispose object.
		/// </summary>
        private bool _disposedValue;

        /// <summary> 	
        /// The counter for generation of client id. 	
        /// </summary> 	
        private int _counter = 0;

        #endregion

        #region delegates 

        /// <summary> 	
        /// The handler from callback of client connection. 	
        /// </summary> 	
        /// <param name="socketClient">The socket client from connected client.</param>
        public delegate void ClientConnectedHandler(Client socketClient);

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="NetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        /// <param name="maxMessageBuffer">The number max of connected clients, the default value is 1000.</param>
        public NetworkListener(ushort port, ClientConnectedHandler clientConnectedHandler, 
            MessageReceivedHandler messageReceivedHandler,
            ClientDisconnectedHandler clientDisconnectedHandler,
            ushort maxMessageBuffer)
        {
            _clientConnectedHandler = clientConnectedHandler;
            _messageReceivedHandler = messageReceivedHandler;
            _clientDisconnectedHandler = clientDisconnectedHandler;
            _maxMessageBuffer = maxMessageBuffer;
        }

        #endregion

        #region public methods implementation

        /// <inheritdoc/>
        public void Dispose() => Dispose(true);

        /// <inheritdoc/>
        public void Stop()
        {
            try
            {
                Dispose(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region protected methods implementation

        /// <summary>
        /// Method responsible for dispose the instance.
        /// </summary>
        /// <param name="disposing">The flag for dispose object.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                    _listener.Close();

                _disposedValue = true;
            }
        }

        /// <summary> 	
        /// Method responsible for generation of identifier to new connected client. 	
        /// </summary> 	
        protected ushort GetNewClientIdentifier() => (ushort)Interlocked.Increment(ref _counter);

        #endregion
    }
}
