namespace KingNetwork.Shared.PacketHandlers.Interfaces {
	internal interface IPacketHandler {
		void HandleMessageData(ushort index, byte[] data);
	}
}
