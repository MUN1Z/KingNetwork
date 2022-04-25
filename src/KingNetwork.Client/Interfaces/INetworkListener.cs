using KingNetwork.Shared;

namespace KingNetwork.Client.Interfaces
{
    /// <summary>
    /// This class is responsible for representation of network listener.
    /// </summary>
    internal interface INetworkListener
    {
        #region properties

        /// <summary>
        /// This property is responsible for returns if listener has connected.
        /// </summary>
        bool HasConnected { get; }

        #endregion

        #region methods

        /// <summary>
        /// Method responsible for start the client network tcp listener.
        /// </summary>
        /// <param name="ip">The ip address of server.</param>
        /// <param name="port">The port of server.</param>
        /// <param name="maxMessageBuffer">The max length of message buffer.</param>
        void StartClient(string ip, int port, ushort maxMessageBuffer);

        /// <summary>
        /// Method responsible for send message to connected server.
        /// </summary>
        /// <param name="writer">The king buffer writer of received message.</param>
        void SendMessage(KingBufferWriter writer);

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
