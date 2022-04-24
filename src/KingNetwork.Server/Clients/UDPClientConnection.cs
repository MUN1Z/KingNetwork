using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for represents the udp client connection.
    /// </summary>
    public class UdpClientConnection : ClientConnection
    {
        #region properties

        /// <inheritdoc/>
        public override bool IsConnected => _udpListener != null;

        /// <inheritdoc/>
        public override string IpAddress => _remoteEndPoint.ToString();

        #endregion

        #region private members

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
        /// Creates a new instance of a <see cref="UDPClientConnection"/>.
        /// </summary>
        /// <param name="id">The identifier number of connected client.</param>
        /// <param name="socketClient">The tcp client from connected client.</param>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer.</param>
        public UdpClientConnection(ushort id, Socket socketClient, EndPoint remoteEndPoint, MessageReceivedHandler messageReceivedHandler, ClientDisconnectedHandler clientDisconnectedHandler, ushort maxMessageBuffer)
        {
            try
            {
                _udpListener = socketClient;
                _remoteEndPoint = remoteEndPoint;

                _messageReceivedHandler = messageReceivedHandler;
                _clientDisconnectedHandler = clientDisconnectedHandler;

                Id = id;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region public methods implementation

        /// <inheritdoc/>
        public override void SendMessage(KingBufferWriter writer)
        {
            try
            {
                if (_udpListener != null)
                    _udpListener.BeginSendTo(writer.BufferData, 0, writer.BufferData.Length, SocketFlags.None, _remoteEndPoint, UdpSendCompleted, new Action<SocketError>(UdpSendCompleted));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <inheritdoc/>
        public override void Disconnect()
        {
            try
            {
                _udpListener.Close();
                _udpListener.Dispose();

                _clientDisconnectedHandler(this);
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
