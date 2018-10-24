namespace KingNetwork.Client.PacketHandlers.Interfaces {
	internal interface IPacketHandler {
		void HandleMessageData(byte[] data);
	}
}
