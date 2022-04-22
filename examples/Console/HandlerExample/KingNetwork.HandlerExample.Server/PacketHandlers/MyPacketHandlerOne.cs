using System;
using KingNetwork.Server;
using KingNetwork.Server.Interfaces;
using KingNetwork.Shared.Interfaces;

namespace KingNetwork.HandlerExample.Server.PacketHandlers
{
    /// <summary>
    /// This interface is responsible for represents the implementation of MyPacketHandlerOne from server packet handler.
    /// </summary>
    public class MyPacketHandlerOne : PacketHandler
    {
        /// <inheritdoc/>
        public override void HandleMessageData(IClientConnection client, IKingBufferReader reader)
        {
            Console.WriteLine($"OnMessageReceived PacketOne from {client.Id}");
        }
    }
}
