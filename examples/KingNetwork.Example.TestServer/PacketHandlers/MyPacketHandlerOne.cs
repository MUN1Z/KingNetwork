using KingNetwork.Server.PacketHandlers;
using System;

namespace KingNetwork.Example.Server.PacketHandlers
{
    public class MyPacketHandlerOne : PacketHandler {
		public override void HandleMessageData(ushort index, byte[] data) {
			Console.WriteLine("Received message in MyPacketHandlerOne");
		}
	}
}
