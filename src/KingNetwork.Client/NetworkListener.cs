using KingNetwork.Shared;
using System;
using System.Net.Sockets;

namespace KingNetwork.Client
{
    public class NetworkListener : TcpClient
	{
		public bool IsConnected { get; private set; }
        
        /// <summary>
        /// The stream of client.
        /// </summary>
        public NetworkStream Stream => GetStream();

        private byte[] _buffer;

        public NetworkListener()
		{
            _buffer = new byte[ConnectionSettings.MAX_MESSAGE_BUFFER];
		}

		public void StartClient(string ip, int port)
		{
            Client.NoDelay = true;
            Connect(ip, port);

            Console.WriteLine("Connected to server!");
		}

        public void SendMessage()
        {
            _buffer[0] = 1;
            Stream.BeginWrite(_buffer, 0, _buffer.Length, null, null);
        }

        public void StartListening()
        {
            _buffer = new byte[ConnectionSettings.MAX_MESSAGE_BUFFER];

            ReceiveBufferSize = ConnectionSettings.MAX_MESSAGE_BUFFER;
            SendBufferSize = ConnectionSettings.MAX_MESSAGE_BUFFER;
            Stream.BeginRead(_buffer, 0, ReceiveBufferSize, new AsyncCallback(ReceiveDataCallback), null);
        }

        private void ReceiveDataCallback(IAsyncResult asyncResult)
        {
            try
            {
                int endRead = Stream.EndRead(asyncResult);

                if (endRead != 0)
                {
                    byte[] numArray = new byte[endRead];
                    Buffer.BlockCopy(_buffer, 0, numArray, 0, endRead);

                    Stream.BeginRead(_buffer, 0, ReceiveBufferSize, new AsyncCallback(ReceiveDataCallback), null);

                    Console.WriteLine($"Received message from server.");

                   // _messageReceived(this, _buffer);
                }
                else
                {
                    Close();
                    Console.WriteLine($"Client disconnected from server.");
                }
            }
            catch (Exception ex)
            {
                Close();
                Console.WriteLine($"Client disconnected from server.");
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }
    }
}
