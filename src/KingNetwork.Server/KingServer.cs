using KingNetwork.Server.Interfaces;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for manipulation of server.
    /// </summary>
    public class KingServer
    {
        #region private members 	
        
        /// <summary> 	
        /// The Server address. 	
        /// </summary> 	
        public string Address { get; private set; }

        /// <summary> 	
        /// The Server port. 	
        /// </summary> 	
        public ushort Port { get; private set; }

        /// <summary> 	
        /// The client manager instance. 	
        /// </summary> 	
        public IClientManager ClientManager { get; private set; }

        #endregion

        /// <summary>
		/// Creates a new instance of a <see cref="KingServer"/>.
		/// </summary>
        /// <param name="address">The server adress.</param>
        /// <param name="port">The server port.</param>
        public KingServer(string address, ushort port)
        {
            Address = address;
            Port = port;

            ClientManager = new ClientManager();
        }

        /// <summary>
        /// Method responsible for start the server.
        /// </summary>
        public void Start()
        {
            ClientManager.Start();
        }
    }
}
