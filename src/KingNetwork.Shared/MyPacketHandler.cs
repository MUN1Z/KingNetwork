using System;

namespace KingNetwork.Shared
{
    public class MyPacketHandler : PacketHandler
    {
        public override void HandleMessageData(ushort index, byte[] data)
        {
            Console.WriteLine("Received message in MyPacketHandler");
        }
    }
}
