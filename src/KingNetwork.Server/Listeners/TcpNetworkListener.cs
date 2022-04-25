using System;
using System.Net;
using System.Net.Sockets;
using static KingNetwork.Server.ClientConnection;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for managing the tcp network tcp listener.
    /// </summary>
    public class TcpNetworkListener : NetworkListener, IDisposable
    {
        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="TcpNetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        /// <param name="maxMessageBuffer">The number max of connected clients, the default value is 1000.</param>
        public TcpNetworkListener(
            ushort port, 
            ClientConnectedHandler clientConnectedHandler, 
            MessageReceivedHandler messageReceivedHandler,
            ClientDisconnectedHandler clientDisconnectedHandler,
            ushort maxMessageBuffer) : base(clientConnectedHandler, messageReceivedHandler, clientDisconnectedHandler, maxMessageBuffer)
        {
            _tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _tcpListener.Bind(new IPEndPoint(IPAddress.Any, port));
            _tcpListener.Listen(100);
            _tcpListener.BeginAccept(new AsyncCallback(OnAccept), null);

            Console.WriteLine($"Starting the TCP network listener on port: {port}.");

            HasStarted = true;
        }

        #endregion
        
        #region private methods imlementation

        /// <summary> 	
        /// The callback from accept client connection. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from socket accepted in connection.</param>
        private void OnAccept(IAsyncResult asyncResult)
        {
            var clientId = GetNewClientIdentifier();
            var client = new TcpClientConnection(clientId, _tcpListener.EndAccept(asyncResult), _messageReceivedHandler, _clientDisconnectedHandler, _maxMessageBuffer);
            _clientConnectedHandler(client);
            _tcpListener.BeginAccept(new AsyncCallback(OnAccept), null);
        }

        #endregion
    }
}
