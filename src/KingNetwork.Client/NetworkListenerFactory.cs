using KingNetwork.Client;
using KingNetwork.Shared;
using static KingNetwork.Client.NetworkListener;

namespace KingNetwork.client
{
    public static class NetworkListenerFactory
    {
        public static NetworkListener CreateForType(NetworkListenerType listenerType, MessageReceivedHandler messageReceivedHandler, ClientDisconnectedHandler clientDisconnectedHandler)
        {
            if (listenerType == NetworkListenerType.TCP)
                return new TcpNetworkListener(messageReceivedHandler, clientDisconnectedHandler);
            else if (listenerType == NetworkListenerType.WSBinary || listenerType == NetworkListenerType.WSText)
                return new WSNetworkListener(listenerType, messageReceivedHandler, clientDisconnectedHandler);

            return new UdpNetworkListener(messageReceivedHandler, clientDisconnectedHandler);
        }
    }
}
