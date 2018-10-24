using KingNetwork.Server.Interfaces;
using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for represents the client.
    /// </summary>
    public class Client : IClient
    {
        #region private members

        /// <summary>
        /// The id of client.
        /// </summary>
        public TcpClient _tcpClient { get; private set; }


        #endregion

        #region properties

        /// <summary>
        /// The id of client.
        /// </summary>
        public ushort ID { get; set; }

	    /// <summary>
	    /// The ip of connected client.
	    /// </summary>
		public string IP => _tcpClient?.Client.RemoteEndPoint.ToString();

		/// <summary>
		/// The stream of client.
		/// </summary>
		public NetworkStream Stream => _tcpClient?.GetStream();

		#endregion

		#region constructors

		/// <summary>
		/// Creates a new instance of a <see cref="Client"/>.
		/// </summary>
		public Client(ushort id, TcpClient tcpClient)
        {
            try
            {
                ID = id;
                _tcpClient = tcpClient;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
