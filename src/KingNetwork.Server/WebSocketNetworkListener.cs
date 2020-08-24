using KingNetwork.Shared;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using static KingNetwork.Server.KingBaseClient;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for managing the tcp network tcp listener.
    /// </summary>
    public class WebSocketNetworkListener : NetworkListener, IDisposable
    {
        #region private members

        /// <summary>
        /// The http listener connection.
        /// </summary
        private HttpListener _httpListener;

        /// <summary>
        /// The listener type of connection.
        /// </summary
        private NetworkListenerType _listenerType;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="WebSocketNetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="listenerType">The listener type of client connection.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        /// <param name="maxMessageBuffer">The number max of connected clients, the default value is 1000.</param>
        public WebSocketNetworkListener(NetworkListenerType listenerType, ushort port, ClientConnectedHandler clientConnectedHandler,
            MessageReceivedHandler messageReceivedHandler,
            ClientDisconnectedHandler clientDisconnectedHandler,
            ushort maxMessageBuffer) : base(port, clientConnectedHandler, messageReceivedHandler, clientDisconnectedHandler, maxMessageBuffer)
        {
            try
            {
                //var host = Dns.GetHostEntry(Dns.GetHostName());
                //var hostIp = host.AddressList.FirstOrDefault(c => c.AddressFamily == AddressFamily.InterNetwork).ToString();

                _listenerType = listenerType;
                _httpListener = new HttpListener();
                _httpListener.Prefixes.Add($"http://localhost:{port}/");
                _httpListener.Prefixes.Add($"http://127.0.0.1:{port}/");
                //_httpListener.Prefixes.Add($"http://{hostIp}:{port}/");
                _httpListener.Start();

                Console.WriteLine($"Starting the websocket network listener on port: {port}.");

                WaitForConnections();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region private methods imlementation

        /// <summary>
        /// This method is reponsible for wait the client connections/>.
        /// </summary>
        private async void WaitForConnections()
        {
            while (true)
            {
                var listenerContext = await _httpListener.GetContextAsync();
                if (listenerContext.Request.IsWebSocketRequest)
                {
                    var webSocket = (await listenerContext.AcceptWebSocketAsync(null)).WebSocket;

                    var client = new KingWebSocketClient(_listenerType, webSocket, listenerContext, _messageReceivedHandler, _clientDisconnectedHandler, _maxMessageBuffer);
                    _clientConnectedHandler(client);
                }
                else
                {
                    listenerContext.Response.StatusCode = 400;
                    listenerContext.Response.Close();
                }
            }
        }

        #endregion
    }
}
