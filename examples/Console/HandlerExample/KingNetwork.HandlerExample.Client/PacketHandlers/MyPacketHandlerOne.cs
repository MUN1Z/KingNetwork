using System;
using KingNetwork.Client;
using KingNetwork.Shared;

namespace KingNetwork.HandlerExample.Client.PacketHandlers
{
    /// <summary>
    /// This interface is responsible for represents the implementation of MyPacketHandlerOne from client packet handler.
    /// </summary>
    public class MyPacketHandlerOne : PacketHandler
    {
        /// <summary>
        /// This method is responsible for receive the message from client packet handler.
        /// </summary>
        /// <param name="kingBuffer">The king buffer of received message.</param>
        public override void HandleMessageData(IKingBuffer kingBuffer)
        {
            Console.WriteLine("Received message in MyPacketHandlerOne");
        }
    }
}
