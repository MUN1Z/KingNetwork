using KingNetwork.Shared;
using System;
using System.Net.Sockets;

namespace KingNetwork.Client
{
    /// <summary>
    /// This class is responsible for managing the network tcp listener.
    /// </summary>
    public class NetworkTcpListener : TcpClient
    {
        #region private members 	

        /// <summary>
        /// The callback of message received handler implementation.
        /// </summary>
        private readonly MessageReceivedHandler _messageReceivedHandler;

        /// <summary>
        /// The callback of client disconnected handler implementation.
        /// </summary>
        private readonly ClientDisconnectedHandler _clientDisconnectedHandler;
        
        /// <summary>
        /// The buffer of client connection.
        /// </summary>
        private byte[] _buffer;

        #endregion

        #region delegates 

        /// <summary>
        /// The delegate of message received handler from server connection.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public delegate void MessageReceivedHandler(KingBuffer kingBuffer);
        
        /// <summary>
		/// The delegate of client disconnected handler connection.
		/// </summary>
        public delegate void ClientDisconnectedHandler();

        #endregion

        #region properties

        /// <summary>
        /// The stream of tcp client.
        /// </summary>
        public NetworkStream Stream => GetStream();

        /// <summary>
		/// The flag of client connection.
		/// </summary>
		public bool IsConnected => this.IsConnected();

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="NetworkTcpListener"/>.
        /// </summary>
        /// <param name="messageReceivedHandler">The callback of message received handler implementation.</param>
        /// <param name="clientDisconnectedHandler">The callback of client disconnected handler implementation.</param>
        public NetworkTcpListener(MessageReceivedHandler messageReceivedHandler, ClientDisconnectedHandler clientDisconnectedHandler)
        {
            try
            {
                _messageReceivedHandler = messageReceivedHandler;
                _clientDisconnectedHandler = clientDisconnectedHandler;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region public methods implementation

        /// <summary>
        /// Method responsible for start the client network tcp listener.
        /// </summary>
        /// <param name="ip">The ip address of server.</param>
        /// <param name="port">The port of server.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer.</param>
        public void StartClient(string ip, int port, ushort maxMessageBuffer)
        {
            try

            {
                Client.NoDelay = true;
                Connect(ip, port);
                
                _buffer = new byte[maxMessageBuffer];

                ReceiveBufferSize = maxMessageBuffer;
                SendBufferSize = maxMessageBuffer;

                Stream.BeginRead(_buffer, 0, ReceiveBufferSize, new AsyncCallback(ReceiveDataCallback), null);

                Console.WriteLine("Connected to server!");
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
        public void SendMessage(KingBuffer kingBuffer)
        {
            try
            {
                Stream.BeginWrite(kingBuffer.ToArray(), 0, kingBuffer.Length(), null, null);
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
                if (IsConnected)
                {
                    var endRead = Stream.EndRead(asyncResult);

                    if (endRead != 0)
                    {
                        var numArray = new byte[endRead];
                        Buffer.BlockCopy(_buffer, 0, numArray, 0, endRead);

                        Stream.BeginRead(_buffer, 0, ReceiveBufferSize, new AsyncCallback(ReceiveDataCallback), null);

                        Console.WriteLine("Received message from server.");

                        _messageReceivedHandler(new KingBuffer(_buffer));
                    }
                }

                Close();

                Console.WriteLine("Client disconnected from server.");

                _clientDisconnectedHandler();
            }
            catch (Exception ex)
            {
                Close();
                Console.WriteLine("Client disconnected from server.");
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
