using System;
using System.Net;
using System.Net.Sockets;
using static KingNetwork.Server.BaseClient;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for managing the udp network udp listener.
    /// </summary>
    public class UdpNetworkListener : NetworkListener
    {
        #region private members

        /// <summary>
        /// The endpoint value to received data.
        /// </summary>
        private EndPoint _endPointFrom;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="TcpNetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        /// <param name="maxMessageBuffer">The number max of connected clients, the default value is 1000.</param>
        public UdpNetworkListener(ushort port, ClientConnectedHandler clientConnectedHandler,
            MessageReceivedHandler messageReceivedHandler,
            ClientDisconnectedHandler clientDisconnectedHandler,
            ushort maxMessageBuffer) : base(port, clientConnectedHandler, messageReceivedHandler, clientDisconnectedHandler, maxMessageBuffer)
        {
            try
            {
                _listener = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
                _listener.Bind(new IPEndPoint(IPAddress.Any, port));
                _endPointFrom = new IPEndPoint(IPAddress.Any, 0);

                _listener.BeginAccept(new AsyncCallback(OnAccept), null);

                Console.WriteLine($"Starting the server network listener on port: {port}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region private methods implementation

        /// <summary> 	
        /// The callback from accept client connection. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from socket accepted in connection.</param>
        private void OnAccept(IAsyncResult asyncResult)
        {
            try
            {
                //_clientConnectedHandler(_listener.EndAccept(asyncResult));

                //Use UDPCLIENT

                var client = new TcpClient(0, _listener.EndAccept(asyncResult), _messageReceivedHandler, _clientDisconnectedHandler, _maxMessageBuffer);
                _clientConnectedHandler(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
            finally
            {
                _listener.BeginAccept(new AsyncCallback(OnAccept), null);
            }
        }

        #endregion
    }
}
