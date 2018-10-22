using System.Net;

namespace KingNetwork.Server.Interfaces
{
    /// <summary>
    /// This interface is responsible for manager the clients.
    /// </summary>
    public interface IClientManager
    {
        IPAddress IPAddress { get; }

        IClient this[ushort id] { get; }

        IClient[] GetAllClients();

        IClient GetClient(ushort id);

        void Start();
    }
}