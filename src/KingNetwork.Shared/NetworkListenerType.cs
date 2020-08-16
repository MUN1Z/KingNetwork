namespace KingNetwork.Shared
{
    /// <summary>
    /// This enum is responsible for represents the type of network listener connection.
    /// </summary>
    public enum NetworkListenerType
    {
        TCP,
        UDP,
        WSBinary,
        WSText
    }

    public enum IPVersion
    {
        IPv4,
        IPv6
    }
}
