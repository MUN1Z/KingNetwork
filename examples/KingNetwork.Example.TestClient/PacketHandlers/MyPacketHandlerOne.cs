using KingNetwork.Client;
using System;

namespace KingNetwork.Example.Client.PacketHandlers
{
    /// <summary>
    /// This interface is responsible for represents the imeplementation of MyPacketHandlerOne from client packet handler.
    /// </summary>
    public class MyPacketHandlerOne : PacketHandler
    {
        /// <summary>
        /// This method is responsible for receive the message from client packet handler.
        /// </summary>
        /// <param name="data">The bytes data of message.</param>
        public override void HandleMessageData(byte[] data)
        {
            Console.WriteLine("Received message in MyPacketHandlerOne");
        }
    }
}
