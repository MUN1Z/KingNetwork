using KingNetwork.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using KingNetwork.Shared.Enums;
using System.Diagnostics;

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
        /// The Server network listener type. 	
        /// </summary> 	
        private readonly NetworkListenerType _listenerType;

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
        /// <param name="listenerType">The listener type to creation of listener, the default value is NetworkListenerType.TCP.</param>
        /// <param name="port">The server port, the default value is 7171.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer, the default value is 4096.</param>
        /// <param name="maxClientConnections">The number max of connected clients, the default value is 1000.</param>
        public KingServer(NetworkListenerType listenerType = NetworkListenerType.TCP, ushort port = 7171, ushort maxMessageBuffer = 4096, ushort maxClientConnections = 1000)
        {
            _listenerType = listenerType;
            _port = port;
            _maxMessageBuffer = maxMessageBuffer;
            _maxClientConnections = maxClientConnections;
            _clients = new Dictionary<ushort, IClientConnection>();
            _serverPacketHandlers = new Dictionary<byte, ServerPacketHandler>();

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledException);
        }

        #endregion

        #region private methods implementation

        /// <summary>
        /// Provisory method to log all exceptions of server.
        /// </summary>
        /// <param name="sender">The sender of exception.</param>
        /// <param name="e">The exception event args.</param>
        private void UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var message = $"Exception: {(e.ExceptionObject as Exception).Message}.";
            Debug.WriteLine(message);
            Console.WriteLine(message);
        }

        /// <summary>
        /// Method responsible for execute the callback of message received from client in server.
        /// </summary>
        /// <param name="client">The connected client.</param>
        /// <param name="kingBuffer">The king buffer received from message.</param>
        private void OnMessageReceived(ClientConnection client, KingBufferReader reader)
        {
            if (reader.Length > 0 && _serverPacketHandlers.Count > 0 && _serverPacketHandlers.TryGetValue(reader.ReadByte(), out var serverHandler))
                serverHandler(client, reader);
            else
                OnMessageReceivedHandler?.Invoke(client, reader);
        }

        /// <summary>
        /// Method responsible for execute the callback of client connected in server.
        /// </summary>
        /// <param name="client">The socket client object from connected client.</param>
        private void OnClientConnected(ClientConnection client)
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

        /// <summary>
        /// Method responsible for execute the callback of client disconnected in server.
        /// </summary>
        /// <param name="client">The instance of disconnected client.</param>
        private void OnClientDisconnected(ClientConnection client)
        {
            if (_clients.ContainsKey(client.Id))
                _clients.Remove(client.Id);

            OnClientDisconnectedHandler?.Invoke(client);
        }

        /// <summary>
        /// Method responsible for start the network listener.
        /// </summary>
        private void StartListener()
        {
            if (_listenerType == NetworkListenerType.TCP)
                _networkListener = new TcpNetworkListener(_port, OnClientConnected, OnMessageReceived, OnClientDisconnected, _maxMessageBuffer);
            else if (_listenerType == NetworkListenerType.UDP)
                _networkListener = new UdpNetworkListener(_port, OnClientConnected, OnMessageReceived, OnClientDisconnected, _maxMessageBuffer);
            else if (_listenerType == NetworkListenerType.RUDP)
                _networkListener = new RudpNetworkListener(_port, OnClientConnected, OnMessageReceived, OnClientDisconnected, _maxMessageBuffer);
            else if (_listenerType == NetworkListenerType.WSBinary || _listenerType == NetworkListenerType.WSText)
                _networkListener = new WebSocketNetworkListener(_listenerType, _port, OnClientConnected, OnMessageReceived, OnClientDisconnected, _maxMessageBuffer);

            OnServerStartedHandler?.Invoke();
        }

        /// <summary>
        /// Method responsible for start the async network listener.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token for the task execution.</param>
        /// <param name="serverFrameRate">The server frame rate per secounds.</param>
        private void StartListenerAsync(CancellationToken cancellationToken, ushort serverFrameRate)
        {
            Task.Run(async () =>
            {
                StartListener();

                while (!cancellationToken.IsCancellationRequested)
                    await Task.Delay(TimeSpan.FromSeconds(1 / serverFrameRate), cancellationToken);

            }, cancellationToken);
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
            if (Enum.IsDefined(typeof(TPacket), packet))
            {
                if (_serverPacketHandlers.ContainsKey((byte)(IConvertible)packet))
                    _serverPacketHandlers.Remove((byte)(IConvertible)packet);

                var handler = new TPacketHandler();

                _serverPacketHandlers.Add((byte)(IConvertible)packet, handler.HandleMessageData);
            }
        }

        /// <summary>
        /// Method responsible for put packet handler in the list of packet handlers.
        /// </summary>
        /// <param name="packet">The value of packet handler.</param>
        /// <param name="serverPacketHandler">The server packet handler callback implementation.</param>
        public void PutHandler<TPacket>(TPacket packet, ServerPacketHandler serverPacketHandler) where TPacket : IConvertible
        {
            if (Enum.IsDefined(typeof(TPacket), packet))
            {
                if (_serverPacketHandlers.ContainsKey((byte)(IConvertible)packet))
                    _serverPacketHandlers.Remove((byte)(IConvertible)packet);
                    
                _serverPacketHandlers.Add((byte)(IConvertible)packet, serverPacketHandler);
            }
        }

        /// <summary>
        /// Method responsible for start the server.
        /// </summary>
        public void Start() => StartListener();

        /// <summary>
        /// Method responsible for start the server.
        /// </summary>
        /// <param name="listenerType">The listener type to creation of listener.</param>
        /// <param name="serverFrameRate">The server frame rate per secounds, default 30 secounds.</param>
        public void StartAsync(out CancellationTokenSource cancellationTokenSource, ushort serverFrameRate = 30)
        {
            cancellationTokenSource = new CancellationTokenSource();
            StartListenerAsync(cancellationTokenSource.Token, serverFrameRate);
        }

        /// <summary>
        /// Method responsible for stop the server.
        /// </summary>
        public void Stop() => _networkListener.Stop();
        
        /// <summary>
        /// Method responsible for send message to specific connected client.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public void SendMessage(IClientConnection client, KingBufferWriter kingBuffer)
        {
            client.SendMessage(kingBuffer);
        }

        /// <summary>
        /// Method responsible for send message to specific connected client.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        /// <param name="messageType">The message type to send message listener.</param>
        public void SendMessage(IClientConnection client, KingBufferWriter kingBuffer, RudpMessageType messageType)
        {
            if (client is RudpClientConnection rudpClientConnection)
                rudpClientConnection.SendMessage(kingBuffer, messageType);
            else
                client.SendMessage(kingBuffer);
        }

        /// <summary>
        /// Method responsible for send message to all connected client.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public void SendMessageToAll(KingBufferWriter kingBuffer)
        {
            foreach (var client in GetAllClients().Where(c => c.IsConnected))
                SendMessage(client, kingBuffer);
        }

        /// <summary>
        /// Method responsible for send message to all connected client.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        /// <param name="messageType">The message type to send message listener.</param>
        public void SendMessageToAll(KingBufferWriter kingBuffer, RudpMessageType messageType)
        {
            foreach (var client in GetAllClients().Where(c => c.IsConnected))
                if (client is RudpClientConnection rudpClientConnection)
                    rudpClientConnection.SendMessage(kingBuffer, messageType);
                else
                    client.SendMessage(kingBuffer);
        }

        /// <summary>
        /// Method responsible for send message to all connected client minus one specific client.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public void SendMessageToAllMinus(IClientConnection client, KingBufferWriter kingBuffer)
        {
            foreach (var clientToSend in GetAllClients().Where(c => c.IsConnected && c.Id != client.Id))
                SendMessage(clientToSend, kingBuffer);
        }

        /// <summary>
        /// Method responsible for send message to all connected client minus one specific client.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        /// <param name="messageType">The message type to send message listener.</param>
        public void SendMessageToAllMinus(IClientConnection client, KingBufferWriter kingBuffer, RudpMessageType messageType)
        {
            foreach (var clientToSend in GetAllClients().Where(c => c.IsConnected && c.Id != client.Id))
                if (clientToSend is RudpClientConnection rudpClientConnection)
                    rudpClientConnection.SendMessage(kingBuffer, messageType);
                else
                    clientToSend.SendMessage(kingBuffer);
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
            _clients[id].Disconnect();
            _clients.Remove(id);
        }

        #endregion
    }
}
