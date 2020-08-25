using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;

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
        /// <param name="reader">The king buffer reader of received message.</param>
        void HandleMessageData(IKingBufferReader reader);
    }
}
