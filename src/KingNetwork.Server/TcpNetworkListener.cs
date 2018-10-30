using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for managing the network tcp listener.
    /// </summary>
    public class TcpNetworkListener : NetworkListener
    {
        #region private members 	

        /// <summary>
		/// The callback of client connected handler implementation.
		/// </summary>
        private readonly ClientConnectedHandler _clientConnectedHandler;

        private TcpListener _tcpListener;

        #endregion
        
        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="TcpNetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        public TcpNetworkListener(ushort port, ClientConnectedHandler clientConnectedHandler) : base(port, clientConnectedHandler)
        {
            try
            {
                _clientConnectedHandler = clientConnectedHandler;
                _tcpListener = new TcpListener(IPAddress.Any, port);

                _tcpListener.Server.NoDelay = true;
                _tcpListener.Start();
                _tcpListener.BeginAcceptSocket(OnAccept, this);

                Console.WriteLine($"Starting the server network listener on port: {port}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region private methods imlementation

        /// <summary> 	
        /// The callback from accept client connection. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from socket accepted in connection.</param>
        private void OnAccept(IAsyncResult asyncResult)
        {
            _clientConnectedHandler(((TcpListener)asyncResult.AsyncState).EndAcceptTcpClient(asyncResult));
            _tcpListener.BeginAcceptSocket(OnAccept, this);
        }
        public override void Stop()
        {
            _tcpListener.Stop();
        }

        #endregion
    }
}
