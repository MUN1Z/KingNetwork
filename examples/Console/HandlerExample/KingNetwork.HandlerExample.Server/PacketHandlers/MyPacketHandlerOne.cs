using System;
using KingNetwork.Server;
using KingNetwork.Server.Interfaces;
using KingNetwork.Shared;

namespace KingNetwork.HandlerExample.Server.PacketHandlers
{
    /// <summary>
    /// This interface is responsible for represents the implementation of MyPacketHandlerOne from server packet handler.
    /// </summary>
    public class MyPacketHandlerOne : PacketHandler
    {
        /// <summary>
        /// This method is responsible for receive the message from server packet handler.
        /// </summary>
        /// <param name="client">The connected client.</param>
        /// <param name="kingBuffer">The king buffer received from message.</param>
        public override void HandleMessageData(IClient client, KingBuffer kingBuffer)
        {
            Console.WriteLine($"OnMessageReceived PacketOne from {client.Key}");
        }
    }
}
