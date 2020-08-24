using KingNetwork.Shared;
using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for represents the tcp client connection.
    /// </summary>
    public class KingUdpClient : KingBaseClient
    {
        #region properties

        /// <summary>
        /// The is connected flag.;
        /// </summary>
        public override bool IsConnected => _udpListener.Socket.Connected;

        #endregion

        #region private members

        /// <summary>
        /// The  udp network listener instance;
        /// </summary>
        private UdpNetworkListener _udpListener;

        /// <summary>
        /// The sremote end point;
        /// </summary>
        private EndPoint _remoteEndPoint;

        #endregion

        #region constructor

        /// <summary>
        /// Creates a new instance of a <see cref="KingTcpClient"/>.
        /// </summary>
        /// <param name="id">The identifier number of connected client.</param>
        /// <param name="socketClient">The tcp client from connected client.</param>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer.</param>
        public KingUdpClient(ushort id, UdpNetworkListener socketClient, EndPoint remoteEndPoint, MessageReceivedHandler messageReceivedHandler, ClientDisconnectedHandler clientDisconnectedHandler, ushort maxMessageBuffer)
        {
            try
            {
                _udpListener = socketClient;
                _remoteEndPoint = remoteEndPoint;

                _messageReceivedHandler = messageReceivedHandler;
                _clientDisconnectedHandler = clientDisconnectedHandler;

                _buffer = new byte[maxMessageBuffer];

                Id = id;
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
                if (_udpListener.Socket != null)
                    _udpListener.Socket.BeginSendTo(kingBuffer.BufferData, 0, kingBuffer.BufferData.Length, SocketFlags.None, _remoteEndPoint, UdpSendCompleted, new Action<SocketError>(UdpSendCompleted));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region private methods implementation

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
                _udpListener.Socket.EndSendTo(result);
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
        public void ReceiveDataCallback(byte[] data)
        {
            try
            {
                _messageReceivedHandler?.Invoke(this, KingBufferReader.Create(data, 0, data.Length));
            }
            catch (Exception ex)
            {
                _clientDisconnectedHandler(this);
                Console.WriteLine($"Client '{IpAddress}' Disconnected.");
            }
        }

        #endregion
    }
}
