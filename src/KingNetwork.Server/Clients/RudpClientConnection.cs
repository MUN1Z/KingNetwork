using KingNetwork.Shared;
using KingNetwork.Shared.Enums;
using KingNetwork.Shared.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for represents the udp client connection.
    /// </summary>
    public class RudpClientConnection : ClientConnection
    {
        #region properties

        /// <inheritdoc/>
        public override bool IsConnected => _udpListener != null;

        /// <inheritdoc/>
        public override string IpAddress { get; }

        #endregion

        #region private members

        /// <summary>
        /// The  udp network listener instance;
        /// </summary>
        private Socket _tcpListener;

        /// <summary>
        /// The stream of tcp client.
        /// </summary>
        protected NetworkStream _tcpStream;

        /// <summary>
        /// The buffer of client connection.
        /// </summary>
        protected byte[] _tcpBuffer;

        /// <summary>
        /// The  udp network listener instance;
        /// </summary>
        private Socket _udpListener;

        /// <summary>
        /// The sremote end point;
        /// </summary>
        private EndPoint _remoteEndPoint;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new instance of a <see cref="UdpClientConnection"/>.
        /// </summary>
        /// <param name="id">The identifier number of connected client.</param>
        /// <param name="tcpSocketClient">The tcp client from socket connection.</param>
        /// <param name="udpSocketClient">The udp client from socket connection.</param>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer.</param>
        public RudpClientConnection(ushort id, Socket tcpSocketClient, Socket udpSocketClient, EndPoint remoteEndPoint, MessageReceivedHandler messageReceivedHandler, ClientDisconnectedHandler clientDisconnectedHandler, ushort maxMessageBuffer)
        {
            _messageReceivedHandler = messageReceivedHandler;
            _clientDisconnectedHandler = clientDisconnectedHandler;
            Id = id;

            //Udp
            _udpListener = udpSocketClient;
            _remoteEndPoint = remoteEndPoint;

            //Tcp
            _tcpListener = tcpSocketClient;
            _tcpListener.ReceiveBufferSize = maxMessageBuffer;
            _tcpListener.SendBufferSize = maxMessageBuffer;
            _tcpBuffer = new byte[maxMessageBuffer];
            _tcpStream = new NetworkStream(_tcpListener);

            Id = id;
            IpAddress = _tcpListener.RemoteEndPoint.ToString();

            _tcpStream.BeginRead(_tcpBuffer, 0, _tcpListener.ReceiveBufferSize, ReceiveTcpDataCallback, null);
        }

        #endregion

        #region public methods implementation

        /// <inheritdoc/>
        public override void SendMessage(KingBufferWriter writer)
        {
            SendMessage(writer, RudpMessageType.Reliable);
        }

        /// <inheritdoc/>
        public void SendMessage(KingBufferWriter writer, RudpMessageType messageType)
        {
            if (messageType == RudpMessageType.Reliable)
                _udpListener?.BeginSendTo(writer.BufferData, 0, writer.BufferData.Length, SocketFlags.None, _remoteEndPoint, UdpSendCompleted, new Action<SocketError>(UdpSendCompleted));
            else
                _udpListener?.BeginSendTo(writer.BufferData, 0, writer.BufferData.Length, SocketFlags.None, _remoteEndPoint, UdpSendCompleted, new Action<SocketError>(UdpSendCompleted));
        }

        /// <inheritdoc/>
        public override void Disconnect()
        {
            _tcpListener?.Close();
            _tcpListener?.Dispose();

            _udpListener?.Close();
            _udpListener?.Dispose();

            _clientDisconnectedHandler(this);
        }

        #endregion

        #region private methods implementation

        /// <summary> 	
        /// The callback from received message from connected client. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from a received message from connected client.</param>
        private void ReceiveTcpDataCallback(IAsyncResult asyncResult)
        {
            try
            {
                if (_tcpListener.Connected)
                {
                    var endRead = _tcpStream.EndRead(asyncResult);

                    var numArray = new byte[endRead];

                    if (endRead != 0)
                    {
                        Buffer.BlockCopy(_tcpBuffer, 0, numArray, 0, endRead);

                        _tcpStream.BeginRead(_tcpBuffer, 0, _tcpListener.ReceiveBufferSize, ReceiveTcpDataCallback, null);

                        var buffer = KingBufferReader.Create(numArray);

                        _messageReceivedHandler(this, buffer);

                        return;
                    }
                }

                _tcpListener.Close();
                _clientDisconnectedHandler(this);
            }
            catch (Exception ex)
            {
                _tcpListener.Dispose();
                _clientDisconnectedHandler(this);
                throw ex;
            }
        }

        /// <summary>
        /// The udp send completed message callback.
        /// </summary>
        /// <param name="e">The socket error from send message.</param>
        private void UdpSendCompleted(SocketError e)
        {
            if (e != 0)
                _clientDisconnectedHandler.Invoke(this);
        }

        /// <summary>
        /// The udp send completed message callback.
        /// </summary>
        /// <param name="asyncResult">The async result from a received message from connected client.</param>
        private void UdpSendCompleted(IAsyncResult result)
        {
            Action<SocketError> action = result.AsyncState as Action<SocketError>;
            try
            {
                _udpListener.EndSendTo(result);
            }
            catch (SocketException ex)
            {
                action(ex.SocketErrorCode);
                return;
            }
            action(SocketError.Success);
        }

        /// <summary> 	
        /// The callback from received message from connected client. 	
        /// </summary> 	
        /// <param name="data">The data of message received.</param>
        public void ReceiveUdpDataCallback(byte[] data)
        {
            try
            {
                _messageReceivedHandler?.Invoke(this, KingBufferReader.Create(data));
            }
            catch (Exception ex)
            {
                _clientDisconnectedHandler(this);
                throw ex;
            }
        }

        #endregion
    }
}
