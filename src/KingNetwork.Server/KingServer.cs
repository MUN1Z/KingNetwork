using KingNetwork.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using KingNetwork.Shared;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for management of king server.
    /// </summary>
    public class KingServer
    {
        #region private members 	

        /// <summary> 	
        /// The network tcp listener instance. 	
        /// </summary> 	
        private NetworkTcpListener _networkListener;

        /// <summary> 	
        /// The network dictionary list of server packet handlers. 	
        /// </summary> 	
        private readonly Dictionary<byte, ServerPacketHandler> _serverPacketHandlers;

        /// <summary> 	
        /// The network dictionary of clients. 	
        /// </summary> 	
        private readonly Dictionary<ushort, IClient> _clients;

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

        /// <summary> 	
        /// The counter for generation of client id. 	
        /// </summary> 	
        private int _counter = 0;

        #endregion

        #region properties

        /// <summary>
        /// The callback of message received handler implementation.
        /// </summary>
        public Client.MessageReceivedHandler MessageReceivedHandler { get; set; }

        #endregion

        #region delegates 	

        /// <summary> 	
        /// The server packet handler delegate. 	
        /// </summary> 	
        /// <param name="client">The connected client.</param>
        /// <param name="buffer">The king buffer received from message.</param>
        public delegate void ServerPacketHandler(IClient client, KingBuffer buffer);

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
                _clients = new Dictionary<ushort, IClient>();
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
        /// Method responsible for generation of identifier to new connected client. 	
        /// </summary> 	
        private ushort GetNewClientIdentifier() => (ushort)Interlocked.Increment(ref _counter);

        /// <summary>
        /// Method responsible for execute the callback of message received from client in server.
        /// </summary>
        /// <param name="client">The connected client.</param>
        /// <param name="kingBuffer">The king buffer received from message.</param>
        private void OnMessageReceived(IClient client, KingBuffer kingBuffer)
        {
            try
            {
                if (_serverPacketHandlers.TryGetValue(kingBuffer.ReadMessagePacket(), out var serverHandler))
                    serverHandler(client, kingBuffer);
                else
                    MessageReceivedHandler(client, kingBuffer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Method responsible for execute the callback of client connected in server.
        /// </summary>
        /// <param name="tcpClient">The tcp client object from connected client.</param>
        private void OnClientConnected(TcpClient tcpClient)
        {
            try
            {
                if (_clients.Count <= _maxClientConnections)
                {
                    var client = new Client(GetNewClientIdentifier(), tcpClient, OnMessageReceived, OnClientDisconnected, _maxMessageBuffer);
                    _clients.Add(client.Id, client);
                }
                else
                {
                    tcpClient.Dispose();
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
        /// <param name="key">The key of disconnected client.</param>
        private void OnClientDisconnected(ushort key)
        {
            try
            {
                if (_clients.ContainsKey(key))
                    _clients.Remove(key);
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
        private async Task StartListenerAsync(CancellationToken cancellationToken)
        {
            try
            {
                _networkListener = new NetworkTcpListener(_port, OnClientConnected);

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
        public IClient GetClient(ushort id) => _clients[id];

        /// <summary>
        /// Method responsible for return all connected clients.
        /// </summary>
        public IList<IClient> GetAllClients() => _clients.Values.ToList();

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
        /// Method responsible for start the server.
        /// </summary>
        public void Start()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                var listeningTask = StartListenerAsync(cancellationTokenSource.Token);

                listeningTask.Wait(cancellationTokenSource.Token);
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
        public void SendMessage(IClient client, KingBuffer kingBuffer)
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
        /// Method responsible for send message to all connected client.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public void SendMessageToAll(KingBuffer kingBuffer)
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
        /// Method responsible for send message to all connected client minus one specific client.
        /// </summary>
        /// <param name="client">The client instance.</param>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public void SendMessageToAllMinus(IClient client, KingBuffer kingBuffer)
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

        #endregion
    }
}
