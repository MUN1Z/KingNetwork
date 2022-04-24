using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Threading;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for represents the websocket client connection.
    /// </summary>
    public class WebSocketClientConnection : ClientConnection
    {
        #region private members

        /// <summary>
        /// The client websocket connection.
        /// </summary>
        private WebSocket _webSocket;

        /// <summary>
        /// The http listener connection.
        /// </summary
        private HttpListenerContext _listenerContext;

        /// <summary>
        /// The listener type of connection.
        /// </summary>
        private NetworkListenerType _listenerType;

        /// <summary>
        /// The buffer of websocket connection.
        /// </summary>
        private ArraySegment<byte> _buff;

        #endregion

        #region properties

        /// <inheritdoc/>
        public override bool IsConnected => _webSocket != null;

        /// <inheritdoc/>
        public override string IpAddress { get; }

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="WebSocketClientConnection"/>.
        /// </summary>
        /// <param name="id">The identifier number of connected client.</param>
        /// <param name="id">The identifier number of connected client.</param>
        /// <param name="listenerType">The listener type of client connection.</param>
        /// <param name="ws">The websocket connection.</param>
        /// <param name="listenerContext">The websocket http listener context.</param>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer.</param>
        public WebSocketClientConnection(ushort id, string remoteEndPoint, NetworkListenerType listenerType, WebSocket ws, HttpListenerContext listenerContext, MessageReceivedHandler messageReceivedHandler, ClientDisconnectedHandler clientDisconnectedHandler, ushort maxMessageBuffer)
        {
            IpAddress = remoteEndPoint;

            _webSocket = ws;
            _listenerContext = listenerContext;
            _listenerType = listenerType;

            _buff = new ArraySegment<byte>(new byte[maxMessageBuffer]);

            _messageReceivedHandler = messageReceivedHandler;
            _clientDisconnectedHandler = clientDisconnectedHandler;

            Id = id;

            WaitConnection();
        }

        #endregion

        #region public methods implementation

        /// <inheritdoc/>
        public override void SendMessage(KingBufferWriter writer)
        {
            if (_listenerType == NetworkListenerType.WSText)
            {
                var data = new ArraySegment<byte>(writer.BufferData, 4, writer.BufferData.Length - 4);
                _webSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else if (_listenerType == NetworkListenerType.WSBinary)
            {
                var data = new ArraySegment<byte>(writer.BufferData);
                _webSocket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        }

        /// <inheritdoc/>
        public override void Disconnect()
        {
            _webSocket.Abort();
            _clientDisconnectedHandler(this);
        }

        #endregion

        #region private methods implementations

        /// <summary>
        /// This method is reponsible for wait for connections.
        /// </summary>
        private async void WaitConnection()
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    var ret = await _webSocket.ReceiveAsync(_buff, CancellationToken.None);

                    if (ret.MessageType == WebSocketMessageType.Text)
                    {
                        if (_listenerContext.Request.RemoteEndPoint != null)
                        {
                            var data = _buff.Take(ret.Count).ToArray();

                            var writer = KingBufferWriter.Create();
                            writer.Write(data);

                            var reader = KingBufferReader.Create(writer.BufferData, 0, writer.Length);

                            _messageReceivedHandler(this, reader);
                        }
                    }
                    else if (ret.MessageType == WebSocketMessageType.Binary)
                    {
                        if (_listenerContext.Request.RemoteEndPoint != null)
                        {
                            var reader = KingBufferReader.Create(_buff.Take(ret.Count).ToArray(), 0, ret.Count);
                            _messageReceivedHandler(this, reader);
                        }
                    }
                    else if (ret.MessageType == WebSocketMessageType.Close)
                        break;
                }
                catch (Exception ex)
                {
                    if (_webSocket.State != WebSocketState.Open)
                    {
                        _clientDisconnectedHandler(this);
                    }
                    else
                        throw ex;

                    break;
                }
            }
        }

        #endregion
    }
}
