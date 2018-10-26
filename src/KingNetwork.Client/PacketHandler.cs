using KingNetwork.Client.Interfaces;
using KingNetwork.Shared;

namespace KingNetwork.Client
{
    /// <summary>
    /// This class is responsible for representation of abstract client packet handler.
    /// </summary>
    public abstract class PacketHandler : IPacketHandler
    {
        /// <summary>
        /// This method is responsible for receive the message from client packet handler.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public abstract void HandleMessageData(KingBuffer kingBuffer);
    }
}
