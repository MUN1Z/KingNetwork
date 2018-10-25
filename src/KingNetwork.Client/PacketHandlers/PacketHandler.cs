using KingNetwork.Client.PacketHandlers.Interfaces;

namespace KingNetwork.Client.PacketHandlers
{
    /// <summary>
    /// This class is responsible for representation of abstract client packet handler.
    /// </summary>
    public abstract class PacketHandler : IPacketHandler
    {
        /// <summary>
        /// This method is responsible for receive the message from client packet handler.
        /// </summary>
        /// <param name="data">The bytes data of message.</param>
        public abstract void HandleMessageData(byte[] data);
    }
}
