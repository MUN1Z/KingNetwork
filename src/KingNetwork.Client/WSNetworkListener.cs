using KingNetwork.Shared;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;

namespace KingNetwork.Client
{
    /// <summary>
    /// This class is responsible for managing the network websocket listener.
    /// </summary>
    public class WSNetworkListener : NetworkListener
    {
        #region private members

        /// <summary>
        /// The client websocket connection.
        /// </summary>
        private ClientWebSocket _clientWebSocket;

        /// <summary>
        /// The buffer size of websocket connection.
        /// </summary>
        private int _messageBufferSize;

        /// <summary>
        /// The listener type of connection.
        /// </summary>
        private NetworkListenerType _listenerType;

        /// <summary>
        /// The buffer of websocket connection.
        /// </summary>
        private ArraySegment<byte> _buff;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="WSNetworkListener"/>.
        /// </summary>
        /// <param name="listenerType">The listener type of client connection.</param>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        public WSNetworkListener(NetworkListenerType listenerType, MessageReceivedHandler messageReceivedHandler, ClientDisconnectedHandler clientDisconnectedHandler)
            : base(messageReceivedHandler, clientDisconnectedHandler)
        {
            _listenerType = listenerType;
        }

        #endregion

        #region public methods implementation

        /// <summary>
        /// Method responsible for start the client network websocket listener.
        /// </summary>
        /// <param name="ip">The ip address of server.</param>
        /// <param name="port">The port of server.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer.</param>
        public override void StartClient(string ip, int port, ushort maxMessageBuffer)
        {
            try
            {
                _messageBufferSize = maxMessageBuffer;
                _buff = new ArraySegment<byte>(new byte[_messageBufferSize]);
                _clientWebSocket = new ClientWebSocket();

                StartConnection(new Uri($"ws://{ip}:{port}"));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for send message to connected server.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public override void SendMessage(KingBufferWriter kingBuffer)
        {
            try
            {
                if (_clientWebSocket != null && _clientWebSocket.State == WebSocketState.Open)
                {
                    if (_listenerType == NetworkListenerType.WSText)
                    {
                        var data = new ArraySegment<byte>(kingBuffer.BufferData, 4, kingBuffer.BufferData.Length - 4);
                        _clientWebSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else if (_listenerType == NetworkListenerType.WSBinary)
                    {
                        var data = new ArraySegment<byte>(kingBuffer.BufferData);
                        _clientWebSocket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region private methods implementation

        /// <summary>
        /// Method responsible for starts the web socket connection.
        /// </summary>
        /// <param name="uri">The uri to start the connection</param>
        public async void StartConnection(Uri uri)
        {
            try
            {
                if (_clientWebSocket.State == WebSocketState.Open) return;
                await _clientWebSocket.ConnectAsync(uri, CancellationToken.None);

                while (_clientWebSocket.State == WebSocketState.Open)
                {
                    var ret = await _clientWebSocket.ReceiveAsync(_buff, CancellationToken.None);

                    if (ret.MessageType == WebSocketMessageType.Text)
                    {
                        var data = _buff.Take(ret.Count).ToArray();

                        var writter = KingBufferWriter.Create();
                        writter.Write((byte)0);
                        writter.Write(data);

                        var buffer = KingBufferReader.Create(writter.BufferData, 0, writter.Length);

                        _messageReceivedHandler(buffer);
                    }
                    else if (ret.MessageType == WebSocketMessageType.Binary)
                    {

                        var kingBuffer = KingBufferReader.Create(_buff.Take(ret.Count).ToArray(), 0, ret.Count);
                        _messageReceivedHandler(kingBuffer);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_clientWebSocket.State != WebSocketState.Open)
                    _clientDisconnectedHandler();
                else
                    Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
