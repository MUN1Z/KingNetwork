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
                _listener.BeginAccept(new AsyncCallback(OnAccept), null);

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
        /// <param name="asyncResult">The async result from socket accepted in connection.</param>
        private void OnAccept(IAsyncResult asyncResult)
        {
            try
            {
                _clientConnectedHandler(_listener.EndAccept(asyncResult));
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}.");
            }
            finally
            {
                _listener.BeginAccept(new AsyncCallback(OnAccept), null);
            }
        }

        #endregion
        
    }
}
