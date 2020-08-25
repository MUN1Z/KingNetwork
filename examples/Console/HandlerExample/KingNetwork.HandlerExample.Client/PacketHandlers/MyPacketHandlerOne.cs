using System;
using KingNetwork.Client;
using KingNetwork.Shared.Interfaces;

namespace KingNetwork.HandlerExample.Client.PacketHandlers
{
    /// <summary>
    /// This interface is responsible for represents the implementation of MyPacketHandlerOne from client packet handler.
    /// </summary>
    public class MyPacketHandlerOne : PacketHandler
    {
        /// <inheritdoc/>
        public override void HandleMessageData(IKingBufferReader reader)
        {
            Console.WriteLine("Received message in MyPacketHandlerOne");
        }
    }
}
