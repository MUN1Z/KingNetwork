using KingNetwork.Client.Tests.Managers;
using KingNetwork.Server;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System.Threading;
using Xunit;
using Xunit.Extensions.Ordering;

namespace KingNetwork.Client.Tests
{
	[Order(3)]
	public class WebSocketClientTests
	{

		#region private members

		KingClient _kingClient;
		KingServer _kingServer;
		string _ip;

		#endregion

		#region constructors

		public WebSocketClientTests()
		{
			_ip = "127.0.0.1";

			_kingServer = ServerManager.GetInstance(NetworkListenerType.WSBinary).KingServer;
			_kingClient = ClientManager.GetInstance().KingClient;
		}

		#endregion

		#region tests implementations

		[Fact, Order(1)]
		public void Verify_KingWSClientConnection_ShouldReturnTrue()
		{
			_kingClient.Connect(_ip, listenerType: NetworkListenerType.WSBinary);

			Thread.Sleep(50);

			Assert.True(_kingClient.HasConnected);
		}

		[Fact, Order(2)]
		public void Verify_KingWSClientMessageReceivedHandler_ShouldHasReceivedMessage()
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
		public void Verify_KingWSClientDisconnection_ShouldHasConnectedFalse()
		{
			_kingClient.Disconnect();

			Assert.False(_kingClient.HasConnected);
		}

		#endregion
	}
}