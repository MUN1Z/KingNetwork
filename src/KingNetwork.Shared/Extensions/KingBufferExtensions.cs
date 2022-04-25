namespace KingNetwork.Shared.Extensions
{
    public static class KingBufferExtensions
    {
        #region Extensions

        public static KingBufferReader ToKingBufferReader(this KingBufferWriter writer)
        {
            return KingBufferReader.Create(writer.BufferData, 0, writer.Length);
        }

        public static KingBufferWriter ToKingBufferWriter(this KingBufferReader reader)
        {
            return KingBufferWriter.Create(reader.Length, reader.BufferData);
        }

        #endregion
    }
}
