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
        public override bool IsConnected => _listener != null ? _listener.Connected : false;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="TcpNetworkListener"/>.
        /// </summary>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="disconnectedHandler">The callback of client disconnected handler implementation.</param>
        public TcpNetworkListener(MessageReceivedHandler messageReceivedHandler, DisconnectedHandler disconnectedHandler)
            : base (messageReceivedHandler, disconnectedHandler) {  }

        #endregion

        #region public methods implementation

        /// <inheritdoc/>
        public override void StartClient(string ip, int port, ushort maxMessageBuffer)
        {
            try
            {
                _remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);

                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _listener.ReceiveBufferSize = maxMessageBuffer;
                _listener.SendBufferSize = maxMessageBuffer;

                _listener.Connect(_remoteEndPoint);

                _buffer = new byte[maxMessageBuffer];
                _stream = new NetworkStream(_listener);
                
                _stream.BeginRead(_buffer, 0, _listener.ReceiveBufferSize, ReceiveDataCallback, null);
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
                _stream.BeginWrite(writer.BufferData, 0, writer.Length, null, null);
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

                        _stream.BeginRead(_buffer, 0, _listener.ReceiveBufferSize, ReceiveDataCallback, null);

                        var buffer = KingBufferReader.Create(numArray, 0, numArray.Length);

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
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
