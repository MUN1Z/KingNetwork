namespace KingNetwork.Server.PacketHandlers.Interfaces {
	internal interface IPacketHandler {
		void HandleMessageData(ushort index, byte[] data);
	}
}
