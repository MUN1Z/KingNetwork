namespace KingNetwork.Server
{
    public class KingServer
    {

        #region private members 	
        
        /// <summary> 	
        /// Server address. 	
        /// </summary> 	
        public string Address { get; private set; }

        /// <summary> 	
        /// Server port. 	
        /// </summary> 	
        public ushort Port { get; private set; }

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
        }

        /// <summary>
        /// Method responsible for start the server.
        /// </summary>
        public void Start()
        {

        }
    }
}
