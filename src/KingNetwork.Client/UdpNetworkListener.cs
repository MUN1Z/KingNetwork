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
            : base (messageReceivedHandler, clientDisconnectedHandler) {  }

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
                var local = new IPEndPoint(IPAddress.Parse(ip), 7272);

                var ipVersion = IPVersion.IPv4;
                AddressFamily family = (AddressFamily)((ipVersion == IPVersion.IPv6) ? 23 : 2);

                _listener = new Socket(family, SocketType.Dgram, ProtocolType.Udp);
                _listener.ReceiveBufferSize = maxMessageBuffer;
                _listener.SendBufferSize = maxMessageBuffer;

               _listener.Bind(local);
                //_listener.Bind(new IPEndPoint(_remoteEndPoint.Address, 0));
                _listener.Connect(_remoteEndPoint);

                _buffer = new byte[maxMessageBuffer];
                //_stream = new NetworkStream(_listener);
                
                //_stream.BeginRead(_buffer, 0, _listener.ReceiveBufferSize, ReceiveDataCallback, null);
                _listener.BeginReceiveFrom(_buffer, 0, _listener.ReceiveBufferSize, SocketFlags.None, ref _remoteEndPoint, new AsyncCallback(ReceiveDataCallback), null);
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
        public override void SendMessage(IKingBuffer kingBuffer)
        {
            try
            {
                _listener.SendTo(kingBuffer.ToArray(), SocketFlags.None, _remoteEndPoint);
                //_stream.BeginWrite(kingBuffer.ToArray(), 0, kingBuffer.Length(), null, null);
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
                    var endRead = _stream.EndRead(asyncResult);

                    if (endRead != 0)
                    {
                        var numArray = new byte[endRead];
                        Buffer.BlockCopy(_buffer, 0, numArray, 0, endRead);

                        //_stream.BeginRead(_buffer, 0, _listener.ReceiveBufferSize, ReceiveDataCallback, null);
                        _listener.BeginReceiveFrom(_buffer, 0, _listener.ReceiveBufferSize, SocketFlags.None, ref _remoteEndPoint, new AsyncCallback(ReceiveDataCallback), null);
                        
                        _messageReceivedHandler(new KingBufferBase(numArray));

                        return;
                    }
                }

                //_stream.Close();
                _clientDisconnectedHandler();
            }
            catch (Exception ex)
            {
                //_stream.Close();
                _clientDisconnectedHandler();
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
