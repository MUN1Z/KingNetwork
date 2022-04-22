using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
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
            try
            {
                _udpRemoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                _udpListener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                _udpListener.ReceiveBufferSize = maxMessageBuffer;
                _udpListener.SendBufferSize = maxMessageBuffer;

                _udpBuffer = new byte[maxMessageBuffer];

                _udpListener.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
                _udpListener.Connect(_udpRemoteEndPoint);

                byte[] array = new byte[9];

                _udpListener.Send(array);

                 array = new byte[16];

                _udpListener.ReceiveTimeout = 5000;
                _udpListener.Receive(array);
                _udpListener.ReceiveTimeout = 0;

                if (array[0] != 1)
                {
                    throw new SocketException(10053);
                }

                _udpListener.BeginReceiveFrom(_udpBuffer, 0, _udpListener.ReceiveBufferSize, SocketFlags.None, ref _udpRemoteEndPoint, new AsyncCallback(ReceiveDataCallback), _udpBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <inheritdoc/>
        public override void SendMessage(IKingBufferWriter writer)
        {
            try
            {
                _udpListener.SendTo(writer.BufferData, _udpRemoteEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
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
                if (_udpListener.Connected)
                { 
                    int bytesRead = _udpListener.EndReceive(asyncResult);

                    if (bytesRead > 0)
                    {
                        var kingBufferReader = KingBufferReader.Create((byte[])asyncResult.AsyncState, 0, bytesRead);

                        _messageReceivedHandler.Invoke(kingBufferReader);

                        _udpListener.BeginReceiveFrom(_udpBuffer, 0, _udpBuffer.Length, SocketFlags.None, ref _udpRemoteEndPoint, new AsyncCallback(this.ReceiveDataCallback), (byte[])asyncResult.AsyncState);
                    }
                }
                else
                {
                    _disconnectedHandler();
                }
            }
            catch (Exception ex)
            {
                _disconnectedHandler();
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
