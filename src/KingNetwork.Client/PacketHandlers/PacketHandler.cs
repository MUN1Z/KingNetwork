using KingNetwork.Client.PacketHandlers.Interfaces;

namespace KingNetwork.Client.PacketHandlers {
    public abstract class PacketHandler : IPacketHandler {
        public abstract void HandleMessageData(byte[] data);
    }
}
