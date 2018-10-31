namespace KingNetwork.Server.Interfaces
{
    /// <summary>
    /// This interface is responsible for representation of network listener.
    /// </summary>
    public interface INetworkListener
    {
        /// <summary>
        /// Method responsible for stop the tcp network listener.
        /// </summary>
        void Stop();
    }
}
