using KingNetwork.Server.PacketHandlers.Interfaces;

namespace KingNetwork.Server.PacketHandlers
{
    /// <summary>
    /// This class is responsible for representation of abstract server packet handler.
    /// </summary>
    public abstract class PacketHandler : IPacketHandler
    {
        /// <summary>
        /// This method is responsible for receive the message from server packet handler.
        /// </summary>
        /// <param name="index">The index of connected client.</param>
        /// <param name="data">The bytes data of message.</param>
        public abstract void HandleMessageData(ushort index, byte[] data);
    }
}
