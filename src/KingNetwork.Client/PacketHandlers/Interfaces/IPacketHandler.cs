namespace KingNetwork.Client.PacketHandlers.Interfaces
{
    /// <summary>
    /// This interface is responsible for represents the client packet handler.
    /// </summary>
    internal interface IPacketHandler
    {
        /// <summary>
        /// This method is responsible for receive the message from client packet handler.
        /// </summary>
        /// <param name="data">The bytes data of message.</param>
        void HandleMessageData(byte[] data);
    }
}
