using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for managing the network listener.
    /// </summary>
    public class NetworkListener
    {
        #region private members 	

        /// <summary> 	
        /// TCPListener to listen for incomming TCP connection requests. 	
        /// </summary> 	
        private Socket _tcpListener;

        /// <summary> 	
        /// Server to tcp socket conection.
        /// </summary> 	
        private KingServer _server;

        #endregion

        #region constructors

        /// <summary>
		/// Creates a new instance of a <see cref="NetworkListener"/>.
		/// </summary>
        /// <param name="server">The instance of KingServer.</param>
        public NetworkListener(KingServer server)
        {
            try
            {
                _server = server;
                _tcpListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
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
                _tcpListener.Bind(new IPEndPoint(IPAddress.Parse(_server.Address), _server.Port));
                _tcpListener.Listen(100);

                Console.WriteLine("Starting the network listener.");

                SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();

                socketAsyncEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(TcpAcceptCompleted);
                if (!_tcpListener.AcceptAsync(socketAsyncEventArgs))
                    TcpAcceptCompleted(this, socketAsyncEventArgs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
           
        }

        /// <summary>
        /// Method called when the tcp connection is accepted is completed.
        /// </summary>
        /// <param name="sender">Object sender.</param>
        /// <param name="e">The event args from sasync socket.</param>
        private void TcpAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError != 0)
                    e.Completed -= new EventHandler<SocketAsyncEventArgs>(TcpAcceptCompleted);
                else
                {
                    try
                    {
                        HandleTcpConnection(e.AcceptSocket);
                    }
                    finally
                    {
                        e.AcceptSocket = null;
                        if (!_tcpListener.AcceptAsync(e))
                            TcpAcceptCompleted(this, e);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        /// <summary>
        /// Handle form accepted socket connection.
        /// </summary>
        /// <param name="acceptSocket">The accepted socket connection.</param>
        private void HandleTcpConnection(Socket acceptSocket)
        {
            try
            {
                Console.WriteLine($"Accepted TCP connection from {acceptSocket.RemoteEndPoint}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
        }

        #endregion
    }
}
