using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Client.Listeners
{
    /// <summary>
    /// This class is responsible for managing the network tcp listener.
    /// </summary>
    public class TcpNetworkListener : NetworkListener
    {
        #region properties

        /// <inheritdoc/>
        public override bool IsConnected => _tcpListener != null ? _tcpListener.Connected : false;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="TcpNetworkListener"/>.
        /// </summary>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="disconnectedHandler">The callback of client disconnected handler implementation.</param>
        public TcpNetworkListener(MessageReceivedHandler messageReceivedHandler, DisconnectedHandler disconnectedHandler)
            : base(messageReceivedHandler, disconnectedHandler) { }

        #endregion

        #region public methods implementation

        /// <inheritdoc/>
        public override void StartClient(string ip, int port, ushort maxMessageBuffer)
        {
            _tcpRemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            _tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _tcpListener.ReceiveBufferSize = maxMessageBuffer;
            _tcpListener.SendBufferSize = maxMessageBuffer;

            _tcpListener.Connect(_tcpRemoteEndPoint);

            _tcpBuffer = new byte[maxMessageBuffer];
            _stream = new NetworkStream(_tcpListener);

            _stream.BeginRead(_tcpBuffer, 0, _tcpListener.ReceiveBufferSize, ReceiveDataCallback, null);
        }

        /// <inheritdoc/>
        public override void SendMessage(KingBufferWriter writer)
        {
            _stream.BeginWrite(writer.BufferData, 0, writer.Length, null, null);
        }

        #endregion

        #region private methods implementation

        /// <summary> 	
        /// The callback from received message from connected server. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from a received message from connected server.</param>
        private void ReceiveDataCallback(IAsyncResult asyncResult)
        {
            try
            {
                if (_tcpListener.Connected)
                {
                    var endRead = _stream.EndRead(asyncResult);

                    if (endRead != 0)
                    {
                        var tempArray = new byte[endRead];
                        Buffer.BlockCopy(_tcpBuffer, 0, tempArray, 0, endRead);

                        _stream.BeginRead(_tcpBuffer, 0, _tcpListener.ReceiveBufferSize, ReceiveDataCallback, null);

                        var buffer = KingBufferReader.Create(tempArray, 0, endRead);

                        _messageReceivedHandler(buffer);

                        return;
                    }
                }

                _stream.Close();
                _disconnectedHandler();
            }
            catch (Exception ex)
            {
                _stream.Close();
                _disconnectedHandler();
                throw ex;
            }
        }

        #endregion
    }
}
