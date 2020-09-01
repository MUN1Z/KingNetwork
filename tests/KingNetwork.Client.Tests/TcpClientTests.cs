using KingNetwork.Client.Tests.Managers;
using KingNetwork.Server;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System.Threading;
using Xunit;
using Xunit.Extensions.Ordering;

namespace KingNetwork.Client.Tests
{
	[Order(1)]
	public class TcpClientTests
	{

		#region private members

		KingClient _kingClient;
		KingServer _kingServer;
		string _ip;

		#endregion

		#region constructors

		public TcpClientTests()
		{
			_ip = "127.0.0.1";

			_kingServer = ServerManager.GetInstance(NetworkListenerType.TCP).KingServer;
			_kingClient = ClientManager.GetInstance().KingClient;
		}

		#endregion

		#region testes implementations

		[Fact, Order(1)]
		public void Verify_KingClientConnection_ShouldReturnTrue()
		{
			var connectionResult = _kingClient.Connect(_ip);
			Assert.True(connectionResult);
		}

		[Fact, Order(2)]
		public void Verify_KingClientMessageReceivedHandler_ShouldHasReceivedMessage()
		{
			bool hasMessageReceived = false;

			_kingClient.MessageReceivedHandler += (IKingBufferReader reader) => 
			{
				hasMessageReceived = true; 
			};

			var writer = KingBufferWriter.Create();
			writer.Write("Test Message");

			_kingServer.SendMessageToAll(writer);

			Thread.Sleep(15);

			Assert.True(hasMessageReceived);
		}

		[Fact, Order(3)]
		public void Verify_KingClientDisconnection_ShouldHasConnectedFalse()
		{
			_kingClient.Disconnect();

			Assert.False(_kingClient.HasConnected);
		}

		#endregion
	}
}