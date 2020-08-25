using KingNetwork.Client.Interfaces;
using KingNetwork.Shared.Interfaces;

namespace KingNetwork.Client
{
    /// <inheritdoc/>
    public abstract class PacketHandler : IPacketHandler
    {
        /// <inheritdoc/>
        public abstract void HandleMessageData(IKingBufferReader reader);
    }
}
