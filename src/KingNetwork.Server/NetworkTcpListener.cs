using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for managing the network tcp listener.
    /// </summary>
    public class NetworkTcpListener : TcpListener
    {
        #region private members 	

        /// <summary> 	
        /// The callback of connected handler. 	
        /// </summary> 	
        public ConnectedHandler _connected;

        #endregion

        #region delegates 

        /// <summary> 	
        /// The handler from callback of client connection. 	
        /// </summary> 	
        /// <param name="client">The tcp client from connected client.</param>
        public delegate void ConnectedHandler(TcpClient client);

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="NetworkTcpListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="connectedHandler">The coonected handler callback implementation.</param>
        public NetworkTcpListener(ushort port, ConnectedHandler connectedHandler) : base(IPAddress.Any, port)
        {
            try
            {
                _connected = connectedHandler;

                Server.NoDelay = true;
                Start();
                BeginAcceptSocket(OnAccept, this);

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
            _connected(((TcpListener)asyncResult.AsyncState).EndAcceptTcpClient(asyncResult));
            BeginAcceptSocket(OnAccept, this);
        }

        #endregion
    }
}
