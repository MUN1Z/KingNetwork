using KingNetwork.Shared;
using static KingNetwork.Server.BaseClient;
using static KingNetwork.Server.NetworkListener;

namespace KingNetwork.Server
{
    public static class NetworkListenerFactory
    {
        public static NetworkListener CreateForType(NetworkListenerType listenerType, ushort port, ClientConnectedHandler clientConnectedHandler, 
            MessageReceivedHandler messageReceivedHandler,
            ClientDisconnectedHandler clientDisconnectedHandler,
            ushort maxMessageBuffer)
        {
            if (listenerType == NetworkListenerType.TCP)
                return new TcpNetworkListener(port, clientConnectedHandler, messageReceivedHandler, clientDisconnectedHandler, maxMessageBuffer);
            else if (listenerType == NetworkListenerType.WSBinary || listenerType == NetworkListenerType.WSText)
                return new WSNetworkListener(listenerType, port, clientConnectedHandler, messageReceivedHandler, clientDisconnectedHandler, maxMessageBuffer);

            return new UdpNetworkListener(port, clientConnectedHandler, messageReceivedHandler, clientDisconnectedHandler, maxMessageBuffer);
        }
    }
}
