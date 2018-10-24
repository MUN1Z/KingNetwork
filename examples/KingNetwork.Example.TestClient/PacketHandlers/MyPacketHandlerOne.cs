using KingNetwork.Client.PacketHandlers;
using System;

namespace KingNetwork.Example.Client.PacketHandlers
{
    public class MyPacketHandlerOne : PacketHandler {
		public override void HandleMessageData(byte[] data) {
			Console.WriteLine("Received message in MyPacketHandlerOne");
		}
	}
}
