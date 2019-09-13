using KingNetwork.Shared;
using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Client
{
    /// <summary>
    /// This class is responsible for representation of abstract network listener.
    /// </summary>
    public abstract class NetworkListener : IDisposable
    {
        #region private members 	

        /// <summary>
        /// The callback of message received handler implementation.
        /// </summary>
        protected readonly MessageReceivedHandler _messageReceivedHandler;

        /// <summary>
        /// The callback of client disconnected handler implementation.
        /// </summary>
        protected readonly ClientDisconnectedHandler _clientDisconnectedHandler;

        /// <summary>
        /// The buffer of client connection.
        /// </summary>
        protected byte[] _buffer;

        /// <summary>
        /// The stream of listener client.
        /// </summary>
        protected NetworkStream _stream;
        
        /// <summary>
        /// The listener for tcp connection.
        /// </summary>
        protected Socket _listener;

        /// <summary>
		/// The value for dispose object.
		/// </summary>
        private bool _disposedValue;

        /// <summary>
		/// The value for remote end point.
		/// </summary>
        public EndPoint _remoteEndPoint;

        #endregion

        #region delegates 

        /// <summary>
        /// The delegate of message received handler from server connection.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public delegate void MessageReceivedHandler(IKingBuffer kingBuffer);

        /// <summary>
        /// The delegate of client disconnected handler connection.
        /// </summary>
        public delegate void ClientDisconnectedHandler();

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="NetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        public NetworkListener(MessageReceivedHandler messageReceivedHandler, ClientDisconnectedHandler clientDisconnectedHandler)
        {
            try
            {
                _messageReceivedHandler = messageReceivedHandler;
                _clientDisconnectedHandler = clientDisconnectedHandler;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region public methods implementation

        /// <summary>
        /// Method responsible for start the client network tcp listener.
        /// </summary>
        /// <param name="ip">The ip address of server.</param>
        /// <param name="port">The port of server.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer.</param>
        public virtual void StartClient(string ip, int port, ushort maxMessageBuffer) { }

        /// <summary>
        /// Method responsible for send message to connected server.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public virtual void SendMessage(IKingBuffer kingBuffer) { }

        /// <summary>
        /// This method is responsible for call the dispose implementation method.
        /// </summary>
        public void Dispose() => Dispose(true);

        /// <summary>
        /// This method is responsible for verify if listener has connected.
        /// </summary>
        public bool Connected() => _listener.Connected;

        /// <summary>
        /// Method responsible for stop the tcp network listener.
        /// </summary>
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

        #endregion
    }
}
