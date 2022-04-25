using KingNetwork.Shared;
using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Client.Listeners
{
    /// <summary>
    /// This class is responsible for managing the network udp listener.
    /// </summary>
    public class UdpNetworkListener : NetworkListener
    {
        #region properties

        /// <inheritdoc/>
        public override bool IsConnected => _udpListener != null ? _udpListener.Connected : false;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="UdpNetworkListener"/>.
        /// </summary>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="disconnectedHandler">The callback of client disconnected handler implementation.</param>
        public UdpNetworkListener(MessageReceivedHandler messageReceivedHandler, DisconnectedHandler disconnectedHandler)
            : base(messageReceivedHandler, disconnectedHandler) { }

        #endregion

        #region public methods implementation

        /// <inheritdoc/>
        public override void StartClient(string ip, int port, ushort maxMessageBuffer)
        {
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
                throw new Exception("Unable to bind UDP ports.");
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
            _udpListener.SendTo(writer.BufferData, _udpRemoteEndPoint);
        }

        #endregion

        #region private methods implementation

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

                    using (var kingBufferReader = KingBufferReader.Create(tempArray, 0, e.BytesTransferred))
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