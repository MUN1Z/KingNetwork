using System;
using System.Collections.Generic;
using System.Threading;
using KingNetwork.Client.Interfaces;
using KingNetwork.Client.Listeners;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;

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
        /// The network listener instance. 	
        /// </summary> 	
        private INetworkListener _networkListener;

        /// <summary> 	
        /// The thread for start the network listener. 	
        /// </summary> 	
        private Thread _clientThread;

        #endregion

        #region properties

        /// <summary>
        /// The flag of client connection.
        /// </summary>
        public bool HasConnected => _networkListener.IsConnected;

        /// <summary>
        /// The callback of message received handler implementation.
        /// </summary>
        public NetworkListener.MessageReceivedHandler MessageReceivedHandler { get; set; }

        #endregion

        #region delegates 	

        /// <summary> 	
        /// The client packet handler delegate. 	
        /// </summary> 	
        /// <param name="reader">The king buffer reader of received message.</param>
        public delegate void ClientPacketHandler(IKingBufferReader reader);

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
        /// Method responsible for put packet handler in the list of packet handlers.
        /// </summary>
        /// <param name="packet">The value of packet handler.</param>
        /// <param name="clientPacketHandler">The client packet handler callback implementation.</param>
        public void PutHandler<TPacket>(TPacket packet, ClientPacketHandler clientPacketHandler) where TPacket : IConvertible
        {
            try
            {
                if (Enum.IsDefined(typeof(TPacket), packet))
                {
                    if (_clientPacketHandlers.ContainsKey((byte)(IConvertible)packet))
                        _clientPacketHandlers.Remove((byte)(IConvertible)packet);
                    
                    _clientPacketHandlers.Add((byte)(IConvertible)packet, clientPacketHandler);
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
        /// <param name="listenerType">The listener type to creation of listener, the default value is NetworkListenerType.TCP.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer, the default value is 4096.</param>
        public void Connect(string ip, ushort port = 7171, NetworkListenerType listenerType = NetworkListenerType.TCP, ushort maxMessageBuffer = 4096)
        {
            try
            {
                if (listenerType == NetworkListenerType.TCP)
                    _networkListener = new TcpNetworkListener(OnMessageReceived, OnClientDisconnected);
                else if (listenerType == NetworkListenerType.UDP)
                    _networkListener = new UdpNetworkListener(OnMessageReceived, OnClientDisconnected);
                else if (listenerType == NetworkListenerType.WSBinary || listenerType == NetworkListenerType.WSText)
                    _networkListener = new WSNetworkListener(listenerType, OnMessageReceived, OnClientDisconnected);

                _clientThread = new Thread(() =>
                {
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
        /// Method responsible for disconnect client from server.
        /// </summary>
        public void Disconnect()
        {
            try
            {
                _networkListener.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for send message to connected server.
        /// </summary>
        /// <param name="writer">The king buffer writer to send message.</param>
        public void SendMessage(IKingBufferWriter writer)
        {
            try
            {
                _networkListener.SendMessage(writer);
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
        /// <param name="reader">The king buffer reader of received message.</param>
        private void OnMessageReceived(IKingBufferReader reader)
        {
            try
            {
                if (reader.Length > 0 && _clientPacketHandlers.Count > 0 && _clientPacketHandlers.TryGetValue(reader.ReadByte(), out var clientPacketHandler))
                    clientPacketHandler(reader);
                else
                    MessageReceivedHandler(reader);
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
                _networkListener.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
