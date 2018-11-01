using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;

namespace KingNetwork.Server
{
    /// <summary>
    /// This class is responsible for representation of abstract server packet handler.
    /// </summary>
    public abstract class PacketHandler : IPacketHandler
    {
        /// <summary>
        /// This method is responsible for receive the message from server packet handler.
        /// </summary>
        /// <param name="client">The connected client.</param>
        /// <param name="kingBuffer">The king buffer received from message.</param>
        public abstract void HandleMessageData(IClient client, IKingBuffer kingBuffer);
    }
}
