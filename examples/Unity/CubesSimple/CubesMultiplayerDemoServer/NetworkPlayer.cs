using KingNetwork.Server.Interfaces;

namespace CubesMultiplayerDemoServer
{
    public class NetworkPlayer
    {
        public IClientConnection IClient { get; private set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public bool Moved { get; set; }

        public NetworkPlayer(IClientConnection client)
        {
            IClient = client;

            X = 0.0f;
            Y = 0.0f;
            Z = 0.0f;

            Moved = false;
        }
    }
}
