namespace KingNetwork.Server.Interfaces
{
    /// <summary>
    /// This interface is responsible for represents the client.
    /// </summary>
    public interface INetworkListener
    {
        #region public methods implementation

        /// <summary>
        /// This property is responsible for returns if listener has started.
        /// </summary>
        bool HasStarted { get; }

        /// <summary>
        /// This method is responsible for call the dispose implementation method.
        /// </summary>
        void Dispose();

        /// <summary>
        /// Method responsible for stop the tcp network listener.
        /// </summary>
        void Stop();

        #endregion
    }
}