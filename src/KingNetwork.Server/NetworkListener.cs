using KingNetwork.Server.Interfaces;
using System.Net.Sockets;

namespace KingNetwork.Server
{
    public abstract class NetworkListener : INetworkListener
    {

        #region delegates 

        /// <summary> 	
        /// The handler from callback of client connection. 	
        /// </summary> 	
        /// <param name="client">The tcp client from connected client.</param>
        public delegate void ClientConnectedHandler(TcpClient client);

        #endregion

        /// <summary>
        /// Creates a new instance of a <see cref="TcpNetworkListener"/>.
        /// </summary>
        /// <param name="port">The port of server.</param>
        /// <param name="clientConnectedHandler">The client connected handler callback implementation.</param>
        public NetworkListener(ushort port, ClientConnectedHandler clientConnectedHandler)
        {

        }
        
        public virtual void Stop()
        {
            
        }
    }
}
