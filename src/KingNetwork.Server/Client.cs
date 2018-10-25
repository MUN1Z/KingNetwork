using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using System;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for represents the client.
    /// </summary>
    public class Client : IClient
    {

        #region private members

        /// <summary>
        /// The tcp client instance from client.
        /// </summary>
        private TcpClient _tcpClient { get; set; }

        /// <summary>
        /// The buffer of client connection.
        /// </summary>
        private byte[] _buffer;

        /// <summary>
		/// The message received method callback.
		/// </summary>
        private MessageReceived _messageReceived { get; }

        #endregion

        #region properties

        /// <summary>
        /// The id of client.
        /// </summary>
        public ushort ID { get; set; }

        /// <summary>
        /// The ip of connected client.
        /// </summary>
        public string IP { get; }

        /// <summary>
        /// The stream of client.
        /// </summary>
        public NetworkStream Stream => _tcpClient.GetStream();

        /// <summary>
		/// The flag of client connection.
		/// </summary>
		public bool HasConnected => _tcpClient.Connected;

        #endregion

        #region delegates

        /// <summary>
		/// The delegate of message reveiced from client connection.
		/// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="data">The bytes data from message.</param>
        public delegate void MessageReceived(IClient client, byte[] data);

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="Client"/>.
        /// </summary>
        /// <param name="id">The id of connected client.</param>
        /// <param name="tcpClient">The tcp client from connected client.</param>
        /// <param name="messageReceived">The message received callback implementation.</param>
        public Client(ushort id, TcpClient tcpClient, MessageReceived messageReceived)
        {
            try
            {
                _tcpClient = tcpClient;
                _messageReceived = messageReceived;

                _tcpClient.ReceiveBufferSize = ConnectionSettings.MAX_MESSAGE_BUFFER;
                _tcpClient.SendBufferSize = ConnectionSettings.MAX_MESSAGE_BUFFER;
                _buffer = new byte[ConnectionSettings.MAX_MESSAGE_BUFFER];

                ID = id;
                IP = _tcpClient.Client.RemoteEndPoint.ToString();

                Stream.BeginRead(_buffer, 0, _tcpClient.ReceiveBufferSize, new AsyncCallback(ReceiveDataCallback), null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary> 	
        /// The callback from received message from connected client. 	
        /// </summary> 	
        /// <param name="asyncResult">The async result from a received message from connected client.</param>
        private void ReceiveDataCallback(IAsyncResult asyncResult)
        {
            try
            {
                int endRead = Stream.EndRead(asyncResult);

                if (endRead != 0)
                {
                    byte[] numArray = new byte[endRead];
                    Buffer.BlockCopy(_buffer, 0, numArray, 0, endRead);

                    Stream.BeginRead(_buffer, 0, _tcpClient.ReceiveBufferSize, new AsyncCallback(ReceiveDataCallback), null);

                    Console.WriteLine($"Received message from client '{IP}'.");

                    _messageReceived(this, _buffer);
                }
                else
                {
                    _tcpClient.Close();
                    Console.WriteLine($"Client '{IP}' Disconnected.");
                }
            }
            catch (Exception ex)
            {
                _tcpClient.Close();
                Console.WriteLine($"Client '{IP}' Disconnected.");
            }
        }

        #endregion
    }
}
