using KingNetwork.Server.PacketHandlers.Interfaces;

namespace KingNetwork.Server.PacketHandlers {
    public abstract class PacketHandler : IPacketHandler {
        public abstract void HandleMessageData(ushort index, byte[] data);
    }
}
