using KingNetwork.Shared.PacketHandlers;
using System;

namespace KingNetwork.Example.Shared.PacketHandlers {
	public class MyPacketHandlerTwo : PacketHandler {
		public override void HandleMessageData(ushort index, byte[] data) {
			Console.WriteLine("Received message in MyPacketHandlerTwo");
		}
	}
}
