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
        /// Dictionary of connected clients.
        /// </summary>
        private Dictionary<ushort, Client> _clients;

        #endregion

        #region public properties

        /// <summary>
        /// The ip adress.
        /// </summary>
        public IPAddress IPAddress { get; }

        /// <summary>
        /// The connected client.
        /// </summary>
        public IClient this[ushort id] => GetClient(id);

        #endregion

        #region constructors

        /// <summary>
		/// Creates a new instance of a <see cref="ClientManager"/>.
		/// </summary>
        public ClientManager()
        {
            _clients = new Dictionary<ushort, Client>();
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

        }

        #endregion
    }
}
