using KingNetwork.Server;
using System;

namespace KingNetwork.Example.Server.PacketHandlers
{
    /// <summary>
    /// This interface is responsible for represents the imeplementation of MyPacketHandlerOne from server packet handler.
    /// </summary>
    public class MyPacketHandlerOne : PacketHandler
    {
        /// <summary>
        /// This method is responsible for receive the message from server packet handler.
        /// </summary>
        /// <param name="index">The index of connected client.</param>
        /// <param name="data">The bytes data of message.</param>
        public override void HandleMessageData(ushort index, byte[] data)
        {
            Console.WriteLine("Received message in MyPacketHandlerOne");
        }
    }
}
