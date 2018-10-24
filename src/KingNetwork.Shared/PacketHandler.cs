namespace KingNetwork.Shared
{
    public abstract class PacketHandler
    {
        public abstract void HandleMessageData(ushort index, byte[] data);
    }
}
