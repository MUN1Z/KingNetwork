using KingNetwork.Server.Interfaces;
using System;

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
        private readonly KingServer _server;

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
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
