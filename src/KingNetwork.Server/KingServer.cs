using KingNetwork.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using KingNetwork.Shared.Enums;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for management of king server.
    /// </summary>
    public class KingServer
    {
        #region private members 	

        /// <summary> 	
        /// The network listener instance. 	
        /// </summary> 	
        private INetworkListener _networkListener;

        /// <summary> 	
        /// The network dictionary list of server packet handlers. 	
        /// </summary> 	
        private readonly Dictionary<byte, ServerPacketHandler> _serverPacketHandlers;

        /// <summary> 	
        /// The network dictionary of clients. 	
        /// </summary> 	
        private readonly Dictionary<ushort, IClientConnection> _clients;

        /// <summary> 	
        /// The Server port. 	
        /// </summary> 	
        private readonly ushort _port;

        /// <summary>
        /// The max length of message buffer.
        /// </summary>
        private readonly ushort _maxMessageBuffer;

        /// <summary>
        /// The number max of connected clients.
        /// </summary>
        private readonly ushort _maxClientConnections;

        #endregion

        #region properties

        /// <summary>
        /// The callback of message received handler implementation.
        /// </summary>
        public ClientConnection.MessageReceivedHandler OnMessageReceivedHandler { get; set; }

        /// <summary>
        /// The callback of client connnected handler implementation.
        /// </summary>
        public ClientConnectedHandler OnClientConnectedHandler { get; set; }

        /// <summary>
        /// The callback of client disconnected handler implementation.
        /// </summary>
        public ClientConnection.ClientDisconnectedHandler OnClientDisconnectedHandler { get; set; }

        /// <summary>
        /// The callback of started server handler implementation.
        /// </summary>
        public ServerStartedHandler OnServerStartedHandler { get; set; }


        #endregion

        #region delegates 	

        /// <summary> 	
        /// The server packet handler delegate. 	
        /// </summary> 	
        /// <param name="client">The connected client.</param>
        /// <param name="reader">The king buffer reader from received message.</param>
        public delegate void ServerPacketHandler(IClientConnection client, IKingBufferReader reader);

        /// <summary> 	
        /// The handler from callback of client connection. 	
        /// </summary> 	
        /// <param name="client">The connected client.</param>
        public delegate void ClientConnectedHandler(IClientConnection client);

        /// <summary> 	
        /// The handler from callback of started server. 	
        /// </summary> 	
        public delegate void ServerStartedHandler();

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="KingServer"/>.
        /// </summary>
        /// <param name="port">The server port, the default value is 7171.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer, the default value is 4096.</param>
        /// <param name="maxClientConnections">The number max of connected clients, the default value is 1000.</param>
        public KingServer(ushort port = 7171, ushort maxMessageBuffer = 4096, ushort maxClientConnections = 1000)
        {
            try
            {
                _port = port;
                _maxMessageBuffer = maxMessageBuffer;
                _maxClientConnections = maxClientConnections;
                _clients = new Dictionary<ushort, IClientConnection>();
                _serverPacketHandlers = new Dictionary<byte, ServerPacketHandler>();
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
        /// <param name="client">The connected client.</param>
        /// <param name="kingBuffer">The king buffer received from message.</param>
        private void OnMessageReceived(IClientConnection client, IKingBufferReader reader)
        {
            try
            {
                if (reader.Length > 0 && _serverPacketHandlers.Count > 0 && _serverPacketHandlers.TryGetValue(reader.ReadByte(), out var serverHandler))
                    serverHandler(client, reader);
                else
                    OnMessageReceivedHandler?.Invoke(client, reader);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for execute the callback of client connected in server.
        /// </summary>
        /// <param name="client">The socket client object from connected client.</param>
        private void OnClientConnected(IClientConnection client)
        {
            try
            {
                if (_clients.Count <= _maxClientConnections)
                {
                    _clients.Add(client.Id, client);
                    OnClientConnectedHandler?.Invoke(client);
                }
                else
                {
                    //Implements Dispose
                    //client.Dispose();

                    Console.WriteLine($"Max client connections {_maxClientConnections}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for execute the callback of client disconnected in server.
        /// </summary>
        /// <param name="client">The instance of disconnected client.</param>
        private void OnClientDisconnected(IClientConnection client)
        {
            try
            {
                if (_clients.ContainsKey(client.Id))
                    _clients.Remove(client.Id);

                OnClientDisconnectedHandler?.Invoke(client);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }
        
        /// <summary>
        /// Method responsible for start the async network listener.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for the task execution.</param>
        /// <param name="listenerType">The listener type to creation of listener, the default value is NetworkListenerType.TCP.</param>
        private async Task StartListenerAsync(CancellationToken cancellationToken, NetworkListenerType listenerType = NetworkListenerType.TCP)
        {
            try
            {
                if (listenerType == NetworkListenerType.TCP)
                    _networkListener = new TcpNetworkListener(_port, OnClientConnected, OnMessageReceived, OnClientDisconnected, _maxMessageBuffer);
                else if (listenerType == NetworkListenerType.UDP)
                    _networkListener = new UdpNetworkListener(_port, OnClientConnected, OnMessageReceived, OnClientDisconnected, _maxMessageBuffer);
                else if (listenerType == NetworkListenerType.RUDP)
                    _networkListener = new RudpNetworkListener(_port, OnClientConnected, OnMessageReceived, OnClientDisconnected, _maxMessageBuffer);
                else if (listenerType == NetworkListenerType.WSBinary || listenerType == NetworkListenerType.WSText)
                    _networkListener = new WebSocketNetworkListener(listenerType, _port, OnClientConnected, OnMessageReceived, OnClientDisconnected, _maxMessageBuffer);
                
                OnServerStartedHandler?.Invoke();

                while (!cancellationToken.IsCancellationRequested)
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region public methods implementation

        /// <summary>
        /// Method responsible for return one connected client by id.
        /// </summary>
        /// <param name="id">The id of connected client.</param>
        public IClientConnection GetClient(ushort id) => _clients[id];

        /// <summary>
        /// Method responsible for return all connected clients.
        /// </summary>
        public IList<IClientConnection> GetAllClients() => _clients.Values.ToList();

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
                    if (_serverPacketHandlers.ContainsKey((byte)(IConvertible)packet))
                        _serverPacketHandlers.Remove((byte)(IConvertible)packet);

                    var handler = new TPacketHandler();

                    _serverPacketHandlers.Add((byte)(IConvertible)packet, handler.HandleMessageData);
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
        /// <param name="serverPacketHandler">The server packet handler callback implementation.</param>
        public void PutHandler<TPacket>(TPacket packet, ServerPacketHandler serverPacketHandler) where TPacket : IConvertible
        {
            try
            {
                if (Enum.IsDefined(typeof(TPacket), packet))
                {
                    if (_serverPacketHandlers.ContainsKey((byte)(IConvertible)packet))
                        _serverPacketHandlers.Remove((byte)(IConvertible)packet);
                    
                    _serverPacketHandlers.Add((byte)(IConvertible)packet, serverPacketHandler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for start the server.
        /// </summary>
        /// <param name="listenerType">The listener type to creation of listener.</param>
        public void Start(NetworkListenerType listenerType = NetworkListenerType.TCP)
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                var listeningTask = StartListenerAsync(cancellationTokenSource.Token, listenerType);

                listeningTask.Wait(cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for stop the server.
        /// </summary>
        public void Stop()
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
        /// Method responsible for send message to specific connected client.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public void SendMessage(IClientConnection client, KingBufferWriter kingBuffer)
        {
            try
            {
                client.SendMessage(kingBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for send message to specific connected client.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        /// <param name="messageType">The message type to send message listener.</param>
        public void SendMessage(IClientConnection client, KingBufferWriter kingBuffer, RudpMessageType messageType)
        {
            try
            {
                if (client is RudpClientConnection rudpClientConnection)
                    rudpClientConnection.SendMessage(kingBuffer, messageType);
                else
                    client.SendMessage(kingBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for send message to all connected client.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public void SendMessageToAll(KingBufferWriter kingBuffer)
        {
            try
            {
                foreach (var client in GetAllClients().Where(c => c.IsConnected))
                    SendMessage(client, kingBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for send message to all connected client.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        /// <param name="messageType">The message type to send message listener.</param>
        public void SendMessageToAll(KingBufferWriter kingBuffer, RudpMessageType messageType)
        {
            try
            {
                foreach (var client in GetAllClients().Where(c => c.IsConnected))
                    if (client is RudpClientConnection rudpClientConnection)
                        rudpClientConnection.SendMessage(kingBuffer, messageType);
                    else
                        client.SendMessage(kingBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for send message to all connected client minus one specific client.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public void SendMessageToAllMinus(IClientConnection client, KingBufferWriter kingBuffer)
        {
            try
            {
                foreach (var clientToSend in GetAllClients().Where(c => c.IsConnected && c.Id != client.Id))
                    SendMessage(clientToSend, kingBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for send message to all connected client minus one specific client.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        /// <param name="messageType">The message type to send message listener.</param>
        public void SendMessageToAllMinus(IClientConnection client, KingBufferWriter kingBuffer, RudpMessageType messageType)
        {
            try
            {
                foreach (var clientToSend in GetAllClients().Where(c => c.IsConnected && c.Id != client.Id))
                    if (clientToSend is RudpClientConnection rudpClientConnection)
                        rudpClientConnection.SendMessage(kingBuffer, messageType);
                    else
                        clientToSend.SendMessage(kingBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for disconnect a specific client from server.
        /// </summary>
        /// <param name="client">The client instance.</param>
        public void DisconnectClient(IClientConnection client) => DisconnectClient(client.Id);

        /// <summary>
        /// Method responsible for disconnect a specific client from server.
        /// </summary>
        /// <param name="id">The client id value.</param>
        public void DisconnectClient(ushort id)
        {
            try
            {
                _clients[id].Disconnect();
                _clients.Remove(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
