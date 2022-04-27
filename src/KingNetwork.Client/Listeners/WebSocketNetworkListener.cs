using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;

namespace KingNetwork.Client.Listeners
{
    /// <summary>
    /// This class is responsible for managing the network websocket listener.
    /// </summary>
    public class WebSocketNetworkListener : NetworkListener
    {
        #region private members

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

        #region properties

        /// <inheritdoc/>
        public override bool HasConnected => _webSocketListener != null ? _webSocketListener.State == WebSocketState.Open : false;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="WebSocketNetworkListener"/>.
        /// </summary>
        /// <param name="listenerType">The listener type of client connection.</param>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="disconnectedHandler">The callback of client disconnected handler implementation.</param>
        public WebSocketNetworkListener(
            NetworkListenerType listenerType,
            MessageReceivedHandler messageReceivedHandler,
            DisconnectedHandler disconnectedHandler)
            : base(messageReceivedHandler, disconnectedHandler)
        {
            _listenerType = listenerType;
        }

        #endregion

        #region public methods implementation

        /// <inheritdoc/>
        public override void StartClient(string ip, int port, ushort maxMessageBuffer)
        {
            _messageBufferSize = maxMessageBuffer;
            _buff = new ArraySegment<byte>(new byte[_messageBufferSize]);
            _webSocketListener = new ClientWebSocket();

            _webSocketListener.ConnectAsync(new Uri($"ws://{ip}:{port}"), CancellationToken.None).Wait();

            StartConnection();
        }

        /// <inheritdoc/>
        public override void SendMessage(KingBufferWriter writer)
        {
            if (HasConnected)
            {
                if (_listenerType == NetworkListenerType.WSText)
                {
                    var data = new ArraySegment<byte>(writer.BufferData, 4, writer.BufferData.Length - 4);
                    _webSocketListener.SendAsync(data, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else if (_listenerType == NetworkListenerType.WSBinary)
                {
                    var data = new ArraySegment<byte>(writer.BufferData);
                    _webSocketListener.SendAsync(data, WebSocketMessageType.Binary, true, CancellationToken.None);
                }
            }
        }

        /// <inheritdoc/>
        public override void Stop()
        {
            _webSocketListener.Abort();
            _webSocketListener.Dispose();

            base.Stop();
        }

        #endregion

        #region private methods implementation

        /// <summary>
        /// Method responsible for starts the web socket connection.
        /// </summary>
        /// <param name="uri">The uri to start the connection</param>
        public async void StartConnection()
        {
            try
            {
                //if (_webSocketListener.State == WebSocketState.Open)
                //    return;

                //await _webSocketListener.ConnectAsync(uri, CancellationToken.None);

                while (HasConnected)
                {
                    var ret = await _webSocketListener.ReceiveAsync(_buff, CancellationToken.None);

                    if (ret.MessageType == WebSocketMessageType.Text)
                    {
                        var data = _buff.Take(ret.Count).ToArray();

                        var writer = KingBufferWriter.Create();
                        writer.Write(data);

                        var reader = KingBufferReader.Create(writer.BufferData);

                        _messageReceivedHandler(reader);
                    }
                    else if (ret.MessageType == WebSocketMessageType.Binary)
                    {
                        var reader = KingBufferReader.Create(_buff.Take(ret.Count).ToArray());
                        _messageReceivedHandler(reader);
                    }
                }
            }
            catch (Exception ex)
            {
                _webSocketListener.Dispose();

                if (HasConnected)
                    _disconnectedHandler();

                throw ex;
            }
        }

        #endregion
    }
}
