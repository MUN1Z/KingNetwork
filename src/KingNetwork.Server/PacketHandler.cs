using KingNetwork.Server.Interfaces;
using KingNetwork.Shared.Interfaces;

namespace KingNetwork.Server
{
    /// <inheritdoc/>
    public abstract class PacketHandler : IPacketHandler
    {
        /// <inheritdoc/>
        public abstract void HandleMessageData(IClientConnection client, IKingBufferReader reader);
    }
}
