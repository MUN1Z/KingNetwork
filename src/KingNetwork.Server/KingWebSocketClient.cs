using KingNetwork.Shared;
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
    public class KingWebSocketClient : KingBaseClient
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

        public override bool IsConnected => _webSocket != null;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="KingWebSocketClient"/>.
        /// </summary>
        /// <param name="listenerType">The listener type of client connection.</param>
        /// <param name="ws">The websocket connection.</param>
        /// <param name="listenerContext">The websocket http listener context.</param>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer.</param>
        public KingWebSocketClient(NetworkListenerType listenerType, WebSocket ws, HttpListenerContext listenerContext, MessageReceivedHandler messageReceivedHandler, ClientDisconnectedHandler clientDisconnectedHandler, ushort maxMessageBuffer)
        {
            try
            {
                _webSocket = ws;
                _listenerContext = listenerContext;
                _listenerType = listenerType;
               
                _buff = new ArraySegment<byte>(new byte[maxMessageBuffer]);

                _messageReceivedHandler = messageReceivedHandler;
                _clientDisconnectedHandler = clientDisconnectedHandler;

                WaitConnection();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region public methods implementation

        /// <summary>
        /// Method responsible for send message to client.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public override void SendMessage(KingBufferWriter kingBuffer)
        {
            try
            {
                if (_listenerType == NetworkListenerType.WSText)
                {
                    var data = new ArraySegment<byte>(kingBuffer.BufferData, 4, kingBuffer.BufferData.Length - 4);
                    _webSocket.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else if (_listenerType == NetworkListenerType.WSBinary)
                {
                    var data = new ArraySegment<byte>(kingBuffer.BufferData);
                    _webSocket.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region private methods implementations


        public async void WaitConnection()
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

                            var writter = KingBufferWriter.Create();
                            writter.Write(data);

                            var buffer = KingBufferReader.Create(writter.BufferData, 0, writter.Length);

                            _messageReceivedHandler(this, buffer);
                        }
                    }
                    else if (ret.MessageType == WebSocketMessageType.Binary)
                    {
                        if (_listenerContext.Request.RemoteEndPoint != null)
                        {
                            KingBufferReader buffer = KingBufferReader.Create(_buff.Take(ret.Count).ToArray(), 0, ret.Count);
                            _messageReceivedHandler(this, buffer);
                        }
                    }
                    else if (ret.MessageType == WebSocketMessageType.Close) 
                        break;
                }
                catch (Exception ex)
                {
                    if (_webSocket.State != WebSocketState.Open)
                        _clientDisconnectedHandler(this);
                    else
                        Console.WriteLine($"Error: {ex.Message}.");
                    break;
                }
            }
        }

        #endregion
    }
}
