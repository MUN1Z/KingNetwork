using KingNetwork.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for managing the network listener.
    /// </summary>
    public class NetworkListener: TcpListener
    {
        #region private members 	
        
        #endregion

        #region constructors

        public delegate void ConnectedHandler(TcpClient client);

        public ConnectedHandler Connected { get; set; }

        /// <summary>
		/// Creates a new instance of a <see cref="NetworkListener"/>.
		/// </summary>
        /// <param name="port">The port of server.</param>
        public NetworkListener(ushort port, ConnectedHandler connectedHandler) : base(IPAddress.Any, port)
        {
            try
            {
                Connected = connectedHandler;

                Server.NoDelay = true;
                Start();
                BeginAcceptSocket(OnAccept, this);

                Console.WriteLine($"Starting the server network listener on port: {port}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region methods
        
        public void OnAccept(IAsyncResult ar)
        {
            var client = ((TcpListener)ar.AsyncState).EndAcceptTcpClient(ar);

            Connected(client);

            BeginAcceptSocket(OnAccept, this);
        }

        #endregion
    }
}
