using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;
using static KingNetwork.Server.NetworkListener;

namespace KingNetwork.Server
{
    public static class NetworkListenerFactory
    {
        public static INetworkListener CreateForType(NetworkListenerType listenerType, ushort port, ClientConnectedHandler clientConnectedHandler)
        {
            if (listenerType == NetworkListenerType.TCP)
                return new TcpNetworkListener(port, clientConnectedHandler);
            else if (listenerType == NetworkListenerType.UDP)
                return new UdpNetworkListener(port, clientConnectedHandler);
            else if (listenerType == NetworkListenerType.WEBSOCKET)
                return new WebSocketNetworkListener(port, clientConnectedHandler);

            return new TcpNetworkListener(port, clientConnectedHandler);
        }
    }
}
