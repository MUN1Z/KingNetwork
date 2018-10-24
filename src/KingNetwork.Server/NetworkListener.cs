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
        
        /// <summary> 	
        /// The Server to tcp socket conection.
        /// </summary> 	
        private KingServer _server;
        
        #endregion

        #region constructors

        public delegate void ConnectedHandler(Socket client);

        public ConnectedHandler Connected { get; set; }

        /// <summary>
		/// Creates a new instance of a <see cref="NetworkListener"/>.
		/// </summary>
        /// <param name="server">The instance of KingServer.</param>
        public NetworkListener(KingServer server, ConnectedHandler connectedHandler) : base(IPAddress.Any, server.Port)
        {
            try
            {
                _server = server;

                Connected = connectedHandler;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region methods

        /// <summary>
        /// Method responsible for start the network listener.
        /// </summary>
        public void StartListener()
        {
            try
            {
                Server.NoDelay = true;
                Start();
                BeginAcceptSocket(OnAccept, this);
                
                Console.WriteLine($"Starting the server network listener on port: {_server.Port}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
           
        }
        
        public void OnAccept(IAsyncResult ar)
        {
            var socket = ((TcpListener)ar.AsyncState).EndAcceptSocket(ar);

            Console.WriteLine($"Accepted TCP connection from {socket.RemoteEndPoint}.");
            
            Connected(socket);

            //var connection = new NetworkServerConnection();

            //connection.OnCloseEvent += OnConnectionClose;
            //connection.OnProcessEvent += Protocol.ProcessMessage;
            //connection.OnPostProcessEvent += Protocol.PostProcessMessage;

            //_connections.Add(connection);
            //Protocol.OnAcceptNewConnection(connection, ar);

            BeginAcceptSocket(OnAccept, this);
        }

        //private void OnConnectionClose(NetworkServerConnection connection)
        //{
        //    // De-subscribe to this event first.
        //    //connection.OnCloseEvent -= OnConnectionClose;
        //    //connection.OnProcessEvent -= Protocol.ProcessMessage;
        //    //connection.OnPostProcessEvent -= Protocol.PostProcessMessage;

        //    _connections.Remove(connection);
        //}

        #endregion
    }
}
