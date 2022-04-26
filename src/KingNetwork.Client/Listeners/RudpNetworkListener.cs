using KingNetwork.Shared.Enums;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Client.Listeners
{
    /// <summary>
    /// This class is responsible for managing the network realiable udp listener.
    /// </summary>
    public class RudpNetworkListener : NetworkListener
    {
        #region properties

        /// <inheritdoc/>
        public override bool HasConnected => _udpListener?.Connected == true || _tcpListener?.Connected == true ? true : false;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="UdpNetworkListener"/>.
        /// </summary>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="disconnectedHandler">The callback of client disconnected handler implementation.</param>
        public RudpNetworkListener(MessageReceivedHandler messageReceivedHandler, DisconnectedHandler disconnectedHandler)
            : base(messageReceivedHandler, disconnectedHandler) { }

        #endregion

        #region public methods implementation

        /// <inheritdoc/>
        public override void StartClient(string ip, int port, ushort maxMessageBuffer)
        {
            //tcp
            _tcpRemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            _tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _tcpListener.ReceiveBufferSize = maxMessageBuffer;
            _tcpListener.SendBufferSize = maxMessageBuffer;

            try
            {
                _tcpListener.Connect(_tcpRemoteEndPoint);
            }
            catch (SocketException e)
            {
                throw new Exception("Unable to establish TCP connection to remote server.", e);
            }


            _tcpBuffer = new byte[maxMessageBuffer];
            _stream = new NetworkStream(_tcpListener);

            _stream.BeginRead(_tcpBuffer, 0, _tcpListener.ReceiveBufferSize, ReceiveTcpDataCallback, null);

            //udp
            _udpRemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            _udpListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            _udpListener.ReceiveBufferSize = maxMessageBuffer;
            _udpListener.SendBufferSize = maxMessageBuffer;

            _udpBuffer = new byte[maxMessageBuffer];

            try
            {
                _udpListener.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
                _udpListener.Connect(_udpRemoteEndPoint);
            }
            catch (SocketException e)
            {
                throw new Exception("Unable to bind UDP ports.", e);
            }

            byte[] array = new byte[9];

            _udpListener.Send(array);

            array = new byte[16];

            _udpListener.ReceiveTimeout = 5000;
            int receivedUdp = _udpListener.Receive(array);
            _udpListener.ReceiveTimeout = 0;

            if (receivedUdp != 16 || array[0] != 1)
            {
                throw new Exception("Timeout waiting for UDP acknowledgement from server.");
            }

            SocketAsyncEventArgs udpArgs = new SocketAsyncEventArgs();

            udpArgs.BufferList = null;
            udpArgs.SetBuffer(_udpBuffer, 0, maxMessageBuffer);

            udpArgs.Completed += ReceiveDataCompletedCallback;

            bool udpCompletingAsync = _udpListener.ReceiveAsync(udpArgs);
            if (!udpCompletingAsync)
                ReceiveDataCompletedCallback(this, udpArgs);
        }

        /// <inheritdoc/>
        public override void SendMessage(KingBufferWriter writer)
        {
            SendMessage(writer, RudpMessageType.Reliable);
        }

        /// <inheritdoc/>
        public void SendMessage(KingBufferWriter writer, RudpMessageType type)
        {
            if (type == RudpMessageType.Reliable)
                _stream.BeginWrite(writer.BufferData, 0, writer.Length, null, null);
            else
                _udpListener.SendTo(writer.BufferData, _udpRemoteEndPoint);
        }

        #endregion

        #region private methods implementation

        /// <summary> 	
        /// The callback from received message from connected server. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from a received message from connected server.</param>
        private void ReceiveTcpDataCallback(IAsyncResult asyncResult)
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

                        _stream.BeginRead(_tcpBuffer, 0, _tcpListener.ReceiveBufferSize, ReceiveTcpDataCallback, null);

                        var buffer = KingBufferReader.Create(tempArray);

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

        /// <summary> 	
        /// The callback from received message from connected server. 	
        /// </summary> 	
        /// <param name="sender">The sender from a received message from connected server.</param>
        /// <param name="e">The event args from a received message from connected server.</param>
        private void ReceiveDataCompletedCallback(object sender, SocketAsyncEventArgs e)
        {
            bool receiveDataStatus;
            do
            {
                if (e.SocketError == SocketError.Success)
                {
                    var tempArray = new byte[e.BytesTransferred];
                    Buffer.BlockCopy(e.Buffer, 0, tempArray, 0, e.BytesTransferred);

                    using (var kingBufferReader = KingBufferReader.Create(tempArray))
                    {
                        receiveDataStatus = _udpListener.ReceiveAsync(e);

                        if (kingBufferReader.Length != 0)
                            _messageReceivedHandler.Invoke(kingBufferReader);
                    }
                }
                else if (e.SocketError == SocketError.ConnectionReset)
                {
                    receiveDataStatus = _udpListener.ReceiveAsync(e);
                }
                else
                {
                    _disconnectedHandler();
                    e.Completed -= ReceiveDataCompletedCallback;
                    return;
                }

            } while (!receiveDataStatus);
        }

        #endregion
    }
}
