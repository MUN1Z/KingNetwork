using System;
using System.Collections.Generic;
using System.Threading;
using KingNetwork.Shared;

namespace KingNetwork.Client
{
    /// <summary>
    /// This class is responsible for management of king client.
    /// </summary>
    public class KingClient
    {
        #region private members 	

        /// <summary> 	
        /// The network dictionary list of server packet handlers. 	
        /// </summary> 	
        private readonly Dictionary<byte, ClientPacketHandler> _clientPacketHandlers;

        /// <summary> 	
        /// The network tcp listener instance. 	
        /// </summary> 	
        private NetworkTcpListener _networkListener;

        /// <summary> 	
        /// The thread for start the network listener. 	
        /// </summary> 	
        private Thread _clientThread;

        #endregion

        #region properties

        /// <summary>
        /// The flag of client connection.
        /// </summary>
        public bool HasConnected => _networkListener.Connected;

        /// <summary>
        /// The callback of message received handler implementation.
        /// </summary>
        public NetworkTcpListener.MessageReceivedHandler MessageReceivedHandler { get; set; }

        #endregion

        #region delegates 	

        /// <summary> 	
        /// The client packet handler delegate. 	
        /// </summary> 	
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public delegate void ClientPacketHandler(KingBuffer kingBuffer);

        #endregion

        #region constructors

        /// <summary>
		/// Creates a new instance of a <see cref="KingClient"/>.
		/// </summary>
        public KingClient()
        {
            _clientPacketHandlers = new Dictionary<byte, ClientPacketHandler>();
        }

        #endregion

        #region public methods implementation

        /// <summary>
        /// Method responsible for put packet handler in the list of packet handlers.
        /// </summary>
        /// <param name="packet">The value of packet handler.</param>
        public void PutHandler<TPacketHandler, TPacket>(TPacket packet) where TPacketHandler : PacketHandler, new() where TPacket : IConvertible
        {
            try
            {
                if (Enum.IsDefined(typeof(TPacket), packet))
                {
                    if (_clientPacketHandlers.ContainsKey((byte)(IConvertible)packet))
                        _clientPacketHandlers.Remove((byte)(IConvertible)packet);

                    var handler = new TPacketHandler();

                    _clientPacketHandlers.Add((byte)(IConvertible)packet, handler.HandleMessageData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible to connect client in server.
        /// </summary>
        /// <param name="ip">The ip address from server.</param>
        /// <param name="port">The port number from server, the default value us 7171</param>
        /// <param name="maxMessageBuffer">The max length of message buffer, the default value is 4096.</param>
        public void Connect(string ip, ushort port = 7171, ushort maxMessageBuffer = 4096)
        {
            try
            {
                _clientThread = new Thread(() =>
                {
                    _networkListener = new NetworkTcpListener(OnMessageReceived, OnClientDisconnected);
                    _networkListener.StartClient(ip, port, maxMessageBuffer);
                });

                _clientThread.IsBackground = true;
                _clientThread.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for send message to connected server.
        /// </summary>
        /// <param name="kingBuffer">The king buffer to send message.</param>
        public void SendMessage(KingBuffer kingBuffer)
        {
            try
            {
                _networkListener.SendMessage(kingBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region private methods implementation 

        /// <summary>
        /// Method responsible for execute the callback of message received from client in server.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        private void OnMessageReceived(KingBuffer kingBuffer)
        {
            try
            {
                if (_clientPacketHandlers.TryGetValue(kingBuffer.ReadMessagePacket(), out var clientPacketHandler))
                    clientPacketHandler(kingBuffer);
                else
                    MessageReceivedHandler(kingBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for execute the callback of client disconnected from server.
        /// </summary>
        private void OnClientDisconnected()
        {
            try
            {
                _networkListener.Dispose();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
