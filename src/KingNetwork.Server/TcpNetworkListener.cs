using KingNetwork.Shared;
using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for managing the tcp network tcp listener.
    /// </summary>
    public class TcpNetworkListener : NetworkListener, IDisposable
    {
        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="TcpNetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        public TcpNetworkListener(ushort port, ClientConnectedHandler clientConnectedHandler) : base(port, clientConnectedHandler)
        {
            try
            {
                _clientConnectedHandler = clientConnectedHandler;

                _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _listener.Bind(new IPEndPoint(IPAddress.Any, port));
                _listener.Listen(100);
                _listener.NoDelay = true;
                
                var socketAsyncEventArgs = KingPoolManager.GetInstance().SocketAsyncEventArgs;
                socketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnAccept);

                if (!_listener.AcceptAsync(socketAsyncEventArgs))
                    OnAccept(this, socketAsyncEventArgs);

                Console.WriteLine($"Starting the server network listener on port: {port}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion

        #region private methods imlementation
        
        /// <summary> 	
        /// The callback from accept client connection. 	
        /// </summary> 	
        /// <param name="sender">The object sender result from socket accepted in connection.</param>
        /// <param name="e">The SocketAsyncEventArgs result from socket accepted in connection.</param>
        private void OnAccept(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != 0)
            {
                e.Completed -= new EventHandler<SocketAsyncEventArgs>(OnAccept);
                KingPoolManager.GetInstance().DisposeSocketAsyncEventArgs(e);
            }
            else
            {
                try
                {
                    _clientConnectedHandler(e.AcceptSocket);
                }
                finally
                {
                    e.AcceptSocket = null;

                    if (!_listener.AcceptAsync(e))
                        OnAccept(this, e);
                }
            }
        }

        #endregion
    }
}
