using KingNetwork.Shared.Network;

namespace KingNetwork.Example.Shared.PacketHandlers
{
    /// <summary>
    /// This class is responsible for represents the packets of messages to client and server handlers.
    /// </summary>
    public class MyPackets : Packets
    {
        /// <summary>
        /// This MyTestPacketOne.
        /// </summary>
        public static ushort MyTestPacketOne => 1;
    }
}
