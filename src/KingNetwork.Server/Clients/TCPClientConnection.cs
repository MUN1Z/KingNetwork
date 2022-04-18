using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for represents the tcp client connection.
    /// </summary>
    public class TCPClientConnection : ClientConnection
    {
        #region private members

        /// <summary>
        /// The tcp client instance from client.
        /// </summary>
        protected Socket _socketClient;

        /// <summary>
        /// The stream of tcp client.
        /// </summary>
        protected NetworkStream _stream;

        /// <summary>
        /// The buffer of client connection.
        /// </summary>
        protected byte[] _buffer;

        #endregion

        #region properties

        /// <inheritdoc/>
        public override bool IsConnected => _socketClient.Connected;

        /// <inheritdoc/>
        public override string IpAddress { get; }

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="TCPClientConnection"/>.
        /// </summary>
        /// <param name="id">The identifier number of connected client.</param>
        /// <param name="socketClient">The tcp client from connected client.</param>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer.</param>
        public TCPClientConnection(ushort id, Socket socketClient, MessageReceivedHandler messageReceivedHandler, ClientDisconnectedHandler clientDisconnectedHandler, ushort maxMessageBuffer)
        {
            try
            {
                _socketClient = socketClient;
                _messageReceivedHandler = messageReceivedHandler;
                _clientDisconnectedHandler = clientDisconnectedHandler;

                _socketClient.ReceiveBufferSize = maxMessageBuffer;
                _socketClient.SendBufferSize = maxMessageBuffer;
                _buffer = new byte[maxMessageBuffer];
                _stream = new NetworkStream(_socketClient);

                Id = id;
                IpAddress = _socketClient.RemoteEndPoint.ToString();

                _stream.BeginRead(_buffer, 0, _socketClient.ReceiveBufferSize, ReceiveDataCallback, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region public methods implementation

        /// <inheritdoc/>
        public override void SendMessage(IKingBufferWriter writer)
        {
            try
            {
                if (IsConnected)
                {
                    _stream.Write(writer.BufferData, 0, writer.Length);
                    _stream.Flush();
                }
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
                _socketClient.Close();
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
        /// The callback from received message from connected client. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from a received message from connected client.</param>
        private void ReceiveDataCallback(IAsyncResult asyncResult)
        {
            try
            {
                if (_socketClient.Connected)
                {
                    var endRead = _stream.EndRead(asyncResult);

                    var numArray = new byte[endRead];

                    if (endRead != 0)
                    {
                        Buffer.BlockCopy(_buffer, 0, numArray, 0, endRead);

                        _stream.BeginRead(_buffer, 0, _socketClient.ReceiveBufferSize, ReceiveDataCallback, null);
                        
                        var buffer = KingBufferReader.Create(numArray, 0, numArray.Length);

                        _messageReceivedHandler(this, buffer);

                        return;
                    }
                }

                _socketClient.Close();
                _clientDisconnectedHandler(this);
            }
            catch (Exception ex)
            {
                _socketClient.Dispose();
                _clientDisconnectedHandler(this);
            }
            
            Console.WriteLine($"Client '{IpAddress}' Disconnected.");
        }

        #endregion
    }
}
