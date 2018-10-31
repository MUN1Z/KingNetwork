using KingNetwork.Shared;
using static KingNetwork.Server.NetworkListener;

namespace KingNetwork.Server
{
    public static class NetworkListenerFactory
    {
        public static NetworkListener CreateForType(NetworkListenerType listenerType, ushort port, ClientConnectedHandler clientConnectedHandler)
        {
            if (listenerType == NetworkListenerType.TCP)
                return new TcpNetworkListener(port, clientConnectedHandler);

            return new UdpNetworkListener(port, clientConnectedHandler);
        }
    }
}
