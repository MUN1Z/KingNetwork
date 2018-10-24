using System.Net;
using System.Net.Sockets;

namespace KingNetwork.Server.Interfaces
{
    /// <summary>
    /// This interface is responsible for represents the client.
    /// </summary>
    public interface IClient
    {
        ushort ID { get; }

	    string IP { get; }

		NetworkStream Stream { get; }

	    void StartListening();
    }
}