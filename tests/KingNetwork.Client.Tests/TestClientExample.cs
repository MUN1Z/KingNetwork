using KingNetwork.Server;
using KingNetwork.Shared;
using KingNetwork.Shared.Interfaces;
using System.Threading;
using Xunit;
using Xunit.Extensions.Ordering;

namespace KingNetwork.Client.Tests
{
	public class ServerManager
	{
		private static ServerManager _instance;
		private static Thread _serverTread;

		public ServerManager()
		{
			_serverTread = new Thread(() =>
			{
				KingServer = new KingServer();
				KingServer.Start();
			});

			_serverTread.Start();
		}

		public static ServerManager GetInstance()
		{
			if (_instance == null)
				_instance = new ServerManager();

			return _instance;
		}

		public KingServer KingServer { get; set; }
	}

	public class ClientManager
	{
		private static ClientManager _instance;

		public ClientManager()
		{
			KingClient = new KingClient();
		}

		public static ClientManager GetInstance()
		{
			if (_instance == null)
				_instance = new ClientManager();

			return _instance;
		}

		public KingClient KingClient { get; set; }
	}

	public class TestClientExample
	{

		#region private members

		KingClient _kingClient;
		KingServer _kingServer;
		string _ip;

		#endregion

		public TestClientExample()
		{
			_ip = "127.0.0.1";

			_kingServer = ServerManager.GetInstance().KingServer;
			_kingClient = ClientManager.GetInstance().KingClient;
		}

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
	}
}
