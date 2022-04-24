using KingNetwork.Client.Interfaces;
using KingNetwork.Shared;
using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

namespace KingNetwork.Client.Listeners
{
    /// <summary>
    /// This class is responsible for representation of abstract network listener.
    /// </summary>
    public abstract class NetworkListener : INetworkListener, IDisposable
    {
        #region private members 	

        /// <summary>
        /// The callback of message received handler implementation.
        /// </summary>
        protected readonly MessageReceivedHandler _messageReceivedHandler;

        /// <summary>
        /// The callback of client disconnected handler implementation.
        /// </summary>
        protected readonly DisconnectedHandler _disconnectedHandler;

        /// <summary>
        /// The tcp buffer of client connection.
        /// </summary>
        protected byte[] _tcpBuffer;

        /// <summary>
        /// The udp buffer of client connection.
        /// </summary>
        protected byte[] _udpBuffer;

        /// <summary>
        /// The stream of listener client.
        /// </summary>
        protected NetworkStream _stream;

        /// <summary>
        /// The listener for tcp connection.
        /// </summary>
        protected Socket _tcpListener;

        /// <summary>
        /// The listener for udp connection.
        /// </summary>
        protected Socket _udpListener;

        /// <summary>
        /// The listener for web socket connection.
        /// </summary>
        protected ClientWebSocket _webSocketListener;

        /// <summary>
		/// The value for dispose object.
		/// </summary>
        private bool _disposedValue;

        /// <summary>
		/// The value for tcp remote end point.
		/// </summary>
        protected EndPoint _tcpRemoteEndPoint;

        /// <summary>
        /// The value for udp remote end point.
        /// </summary>
        protected EndPoint _udpRemoteEndPoint;

        #endregion

        #region Properties

        /// <inheritdoc/>
		public abstract bool IsConnected { get; }

        #endregion

        #region delegates 

        /// <summary>
        /// The delegate of message received handler from server connection.
        /// </summary>
        /// <param name="reader">The king buffer reader of received message.</param>
        public delegate void MessageReceivedHandler(KingBufferReader reader);

        /// <summary>
        /// The delegate of client disconnected handler connection.
        /// </summary>
        public delegate void DisconnectedHandler();

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="NetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        public NetworkListener(MessageReceivedHandler messageReceivedHandler, DisconnectedHandler clientDisconnectedHandler)
        {
            _messageReceivedHandler = messageReceivedHandler;
            _disconnectedHandler = clientDisconnectedHandler;
        }

        #endregion

        #region public methods implementation

        /// <inheritdoc/>
        public virtual void StartClient(string ip, int port, ushort maxMessageBuffer) { }

        /// <inheritdoc/>
        public virtual void SendMessage(KingBufferWriter writer) { }

        /// <inheritdoc/>
        public void Dispose() => Dispose(true);

        /// <inheritdoc/>
        public virtual void Stop()
        {
            Dispose(true);
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
                {
                    _tcpListener?.Close();
                    _udpListener?.Close();
                    _webSocketListener?.Dispose();
                }

                _disposedValue = true;
            }
        }

        #endregion
    }
}
