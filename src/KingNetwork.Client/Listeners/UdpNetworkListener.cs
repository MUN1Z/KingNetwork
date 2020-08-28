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
        public override bool IsConnected => _listener != null ? _listener.Connected : false;

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
                _remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                _listener.ReceiveBufferSize = maxMessageBuffer;
                _listener.SendBufferSize = maxMessageBuffer;

                _buffer = new byte[maxMessageBuffer];

                _listener.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
                _listener.Connect(_remoteEndPoint);

                byte[] array = new byte[9];

                _listener.Send(array);

                 array = new byte[16];

                _listener.ReceiveTimeout = 5000;
                _listener.Receive(array);
                _listener.ReceiveTimeout = 0;

                if (array[0] != 1)
                {
                    throw new SocketException(10053);
                }

                _listener.BeginReceiveFrom(_buffer, 0, _listener.ReceiveBufferSize, SocketFlags.None, ref _remoteEndPoint, new AsyncCallback(ReceiveDataCallback), _buffer);
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
                _listener.SendTo(writer.BufferData, _remoteEndPoint);
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
                if (_listener.Connected)
                { 
                    int bytesRead = _listener.EndReceive(asyncResult);

                    if (bytesRead > 0)
                    {
                        var kingBufferReader = KingBufferReader.Create((byte[])asyncResult.AsyncState, 0, bytesRead);

                        _messageReceivedHandler.Invoke(kingBufferReader);

                        _listener.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref _remoteEndPoint, new AsyncCallback(this.ReceiveDataCallback), (byte[])asyncResult.AsyncState);
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
