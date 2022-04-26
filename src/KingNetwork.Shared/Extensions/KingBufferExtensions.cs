namespace KingNetwork.Shared.Extensions
{
    public static class KingBufferExtensions
    {
        #region Extensions

        public static KingBufferReader ToKingBufferReader(this KingBufferWriter writer)
        {
            return KingBufferReader.Create(writer.BufferData);
        }

        public static KingBufferWriter ToKingBufferWriter(this KingBufferReader reader)
        {
            return KingBufferWriter.Create(reader.BufferData);
        }

        #endregion
    }
}
