using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using KingNetwork.Server.Interfaces;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for manager the clients.
    /// </summary>
    public class ClientManager : IClientManager
    {
        #region private members

        /// <summary>
        /// The dictionary of connected clients.
        /// </summary>
        private Dictionary<ushort, Client> _clients;

        /// <summary>
        /// The network listener.
        /// </summary>
        private NetworkListener _networkListener;

        /// <summary>
        /// The king server instance.
        /// </summary>
        private KingServer _server;

        #endregion

        #region public properties

        /// <summary>
        /// The ip adress.
        /// </summary>
        public IPAddress IPAddress { get; }

        /// <summary>
        /// The port number.
        /// </summary>
        public ushort Port { get; }

        /// <summary>
        /// The connected client.
        /// </summary>
        public IClient this[ushort Id] => GetClient(Id);

        #endregion

        #region constructors

        /// <summary>
		/// Creates a new instance of a <see cref="ClientManager"/>.
		/// </summary>
        /// <param name="server">The instance of KingServer.</param>
        public ClientManager(KingServer server)
        {
            try
            {
                Port = server.Port;
                IPAddress = IPAddress.Parse(server.Address);

                _server = server;

                _clients = new Dictionary<ushort, Client>();
                _networkListener = new NetworkListener(_server);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion


        #region methods implementation

        /// <summary>
        /// Method responsible for return all connected clients.
        /// </summary>
        public IClient[] GetAllClients() => _clients.Values.ToArray();

        /// <summary>
        /// Method responsible for return one connected client by id.
        /// </summary>
        public IClient GetClient(ushort id) => _clients[id];

        /// <summary>
        /// Method responsible for start the client manager.
        /// </summary>
        public void Start()
        {
            try
            {
                _networkListener.StartListener();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
