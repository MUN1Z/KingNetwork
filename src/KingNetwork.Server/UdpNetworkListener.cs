namespace KingNetwork.Server
{
    public class UdpNetworkListener : NetworkListener
    {
        public UdpNetworkListener(ushort port, ClientConnectedHandler clientConnectedHandler) : base(port, clientConnectedHandler)
        {
        }

        public override void Stop()
        {
            
        }
    }
}
