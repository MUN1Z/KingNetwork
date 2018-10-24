using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for manipulation of server.
    /// </summary>
    public class KingServer
    {

        #region private members 	
        
        /// <summary> 	
        /// The network listener instance. 	
        /// </summary> 	
        public NetworkListener _networkListener { get; private set; }

        private static Dictionary<ushort, ServerHandler> _serverHandlers;

        #endregion

        #region properties 	

        /// <summary> 	
        /// The Server port. 	
        /// </summary> 	
        public ushort Port { get; private set; }

        public List<IClient> Clients { get; private set; }
        
        #endregion

        #region delegates 	

        public delegate void ServerHandler(ushort index, byte[] data);

        #endregion
        
        int _counter = 0;
        private ushort GetNextClientId() => (ushort)Interlocked.Increment(ref _counter);

        /// <summary>
		/// Creates a new instance of a <see cref="KingServer"/>.
		/// </summary>
        /// <param name="port">The server port.</param>
        public KingServer(ushort port)
        {
            try
            {
                Port = port;
                Clients = new List<IClient>();

                _networkListener = new NetworkListener(this, OnClientConnected);

                _serverHandlers = new Dictionary<ushort, ServerHandler>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        public void PutHandler<T>(ushort type) where T : PacketHandler, new()
        {
            if (_serverHandlers.ContainsKey(type))
                _serverHandlers.Remove(type);

            var handler = new T();

            _serverHandlers.Add(type, handler.HandleMessageData);
        }

        /// <summary>
        /// Method responsible for start the server.
        /// </summary>
        public void Start()
        {
            try
            {
                var cancellationTokenSource = new CancellationTokenSource();

                var listeningTask = RunAsync(cancellationTokenSource.Token);

                listeningTask.Wait(cancellationTokenSource.Token);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        public void SendMessage(ushort clientId, byte[] data)
        {
            try
            {
                GetClient(clientId).Stream.Write(data, 0, data.Length);
                GetClient(clientId).Stream.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        private void OnClientConnected(Socket tcpClient)
        {
            Clients.Add(new Client(GetNextClientId(), tcpClient));

            Console.WriteLine("OnClientConnected");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            _networkListener.StartListener();

            while (!cancellationToken.IsCancellationRequested)
                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        }

        #region clients manager

        /// <summary>
        /// Method responsible for return one connected client by id.
        /// </summary>
        public IClient GetClient(ushort id) => Clients.FirstOrDefault(c => c.ID == id);

        #endregion
    }
}
