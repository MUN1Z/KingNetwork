namespace KingNetwork.Server.Interfaces
{
    /// <summary>
    /// This interface is responsible for represents the server packet handler.
    /// </summary>
	internal interface IPacketHandler
    {
        /// <summary>
        /// This method is responsible for receive the message from server packet handler.
        /// </summary>
        /// <param name="index">The index of connected client.</param>
        /// <param name="data">The bytes data of message.</param>
        void HandleMessageData(ushort index, byte[] data);
    }
}
