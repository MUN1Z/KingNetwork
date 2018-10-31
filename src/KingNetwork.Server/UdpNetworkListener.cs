using System;
using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for managing the udp network udp listener.
    /// </summary>
    public class UdpNetworkListener : NetworkListener
    {
        #region private members
        
        /// <summary>
		/// The endpoint value to received data.
		/// </summary>
        private EndPoint _endPointFrom;

        /// <summary>
		/// The buffer data for udp connection.
		/// </summary>
        private byte[] _buffer;
        
        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="TcpNetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        public UdpNetworkListener(ushort port, ClientConnectedHandler clientConnectedHandler) : base(port, clientConnectedHandler)
        {
            _clientConnectedHandler = clientConnectedHandler;
            _listener = new Socket(AddressFamily.InterNetworkV6, SocketType.Dgram, ProtocolType.Udp);
            _listener.Bind(new IPEndPoint(IPAddress.Any, port));
            _endPointFrom = new IPEndPoint(IPAddress.Any, 0);
            _buffer = new byte[4096];

            _listener.BeginReceiveFrom(_buffer, 0, _buffer.Length, SocketFlags.None, ref _endPointFrom, new AsyncCallback(OnAccept), _buffer);

            Console.WriteLine($"Starting the server network listener on port: {port}.");
        }

        #endregion
        
        #region private methods implementation

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
            catch (Exception ex)
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
