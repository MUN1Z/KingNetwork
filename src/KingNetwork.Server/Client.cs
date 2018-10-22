using KingNetwork.Server.Interfaces;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for represents the client.
    /// </summary>
    public class Client : IClient
    {
        #region private members

        /// <summary>
        /// The king server instance.
        /// </summary>
        private KingServer _server;

        #endregion

        #region public properties

        /// <summary>
        /// The id of client.
        /// </summary>
        public ushort ID { get; }

        #endregion

        #region constructors

        /// <summary>
		/// Creates a new instance of a <see cref="Client"/>.
		/// </summary>
        public Client()
        {

        }

        #endregion
    }
}
