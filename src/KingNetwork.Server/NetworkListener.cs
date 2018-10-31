using System;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for representation of abstract network listener.
    /// </summary>
    public abstract class NetworkListener : IDisposable
    {
        #region private members

        /// <summary>
        /// The callback of client connected handler implementation.
        /// </summary>
        protected ClientConnectedHandler _clientConnectedHandler;

        /// <summary>
		/// The listener for tcp connection.
		/// </summary>
        protected Socket _listener;

        /// <summary>
		/// The value for dispose object.
		/// </summary>
        private bool _disposedValue;

        #endregion

        #region delegates 

        /// <summary> 	
        /// The handler from callback of client connection. 	
        /// </summary> 	
        /// <param name="socketClient">The socket client from connected client.</param>
        public delegate void ClientConnectedHandler(Socket socketClient);

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="NetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        public NetworkListener(ushort port, ClientConnectedHandler clientConnectedHandler) {}

        #endregion

        #region public methods implementation

        /// <summary>
        /// This method is responsible for call the dispose implementation method.
        /// </summary>
        public void Dispose() => Dispose(true);

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
