using KingNetwork.Shared;

namespace KingNetwork.Client.Interfaces
{
    /// <summary>
    /// This interface is responsible for represents the client packet handler.
    /// </summary>
    internal interface IPacketHandler
    {
        /// <summary>
        /// This method is responsible for receive the message from client packet handler.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        void HandleMessageData(IKingBuffer kingBuffer);
    }
}
