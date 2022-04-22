using KingNetwork.Server.Tests.Managers;
using KingNetwork.Server;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System.Threading;
using Xunit;
using Xunit.Extensions.Ordering;
using KingNetwork.Server.Interfaces;
using System.Linq;

namespace KingNetwork.Client.Tests
{
	[Order(1)]
	public class TcpServertTests
	{

		#region private members

		KingClient _kingClient;
		KingServer _kingServer;
		string _ip;

		#endregion

		#region constructors

		public TcpServertTests()
		{
			_ip = "127.0.0.1";

			_kingServer = ServerManager.GetInstance(NetworkListenerType.TCP).KingServer;
			_kingClient = ClientManager.GetInstance().KingClient;
		}

		#endregion

		#region tests implementations

		[Fact, Order(1)]
		public void Verify_KingTCPServerConnection_ShouldReturnTrue()
		{
			bool hasClientConnected = false;

			_kingServer.OnClientConnectedHandler += (IClientConnection client) =>
			{
				hasClientConnected = true;
			};

			_kingClient.Connect(_ip);

			Thread.Sleep(15);

			Assert.True(hasClientConnected);
		}

		[Fact, Order(2)]
		public void Verify_KingTCPServerMessageReceivedHandler_ShouldHasReceivedMessage()
		{
			bool hasMessageReceived = false;

			_kingServer.OnMessageReceivedHandler += (IClientConnection client, IKingBufferReader reader) =>
			{
				hasMessageReceived = true;
			};

			var writer = KingBufferWriter.Create();
			writer.Write("Test Message");

			_kingClient.SendMessage(writer);

			Thread.Sleep(15);

			Assert.True(hasMessageReceived);
		}

		[Fact, Order(3)]
		public void Verify_KingTCPServerDisconnection_ShouldHasClientDisconnected()
		{
			bool hasClientDisconnected = false;

			_kingServer.OnClientDisconnectedHandler += (IClientConnection client) =>
			{
				hasClientDisconnected = true;
			};

			var clients = _kingServer.GetAllClients();
			_kingServer.DisconnectClient(clients.FirstOrDefault());

			Thread.Sleep(15);

			Assert.True(hasClientDisconnected);
		}

		#endregion
	}
}