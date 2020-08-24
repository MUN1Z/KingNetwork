using KingNetwork.Shared;
using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Client
{
    /// <summary>
    /// This class is responsible for managing the network udp listener.
    /// </summary>
    public class UdpNetworkListener : NetworkListener
    {
        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="UdpNetworkListener"/>.
        /// </summary>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        public UdpNetworkListener(MessageReceivedHandler messageReceivedHandler, ClientDisconnectedHandler clientDisconnectedHandler)
            : base(messageReceivedHandler, clientDisconnectedHandler) { }

        #endregion

        #region public methods implementation

        /// <summary>
        /// Method responsible for start the client network udp listener.
        /// </summary>
        /// <param name="ip">The ip address of server.</param>
        /// <param name="port">The port of server.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer.</param>
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

        /// <summary>
        /// Method responsible for send message to connected server.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public override void SendMessage(KingBufferWriter kingBuffer)
        {
            try
            {
                _listener.SendTo(kingBuffer.BufferData, _remoteEndPoint);
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
                    _clientDisconnectedHandler();
                }
            }
            catch (Exception ex)
            {
                _clientDisconnectedHandler();
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
