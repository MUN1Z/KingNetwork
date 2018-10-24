using KingNetwork.Shared.PacketHandlers.Interfaces;

namespace KingNetwork.Shared.PacketHandlers {
    public abstract class PacketHandler : IPacketHandler {
        public abstract void HandleMessageData(ushort index, byte[] data);
    }
}
