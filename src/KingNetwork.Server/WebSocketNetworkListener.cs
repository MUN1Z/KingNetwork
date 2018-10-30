namespace KingNetwork.Server
{
    public class WebSocketNetworkListener : NetworkListener
    {
        public WebSocketNetworkListener(ushort port, ClientConnectedHandler clientConnectedHandler) : base (port, clientConnectedHandler)
        {

        }

        public override void Stop()
        {

        }
    }
}