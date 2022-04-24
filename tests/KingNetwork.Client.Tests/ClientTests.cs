using KingNetwork.Server;
using KingNetwork.Shared;
using KingNetwork.Shared.Encryptation;
using KingNetwork.Shared.Interfaces;
using Xunit;
using XUnitPriorityOrderer;

namespace KingNetwork.Client.Tests
{
    [Order(1)]
	public class ClientTests
	{
		#region private members

		private string _ip = "127.0.0.1";
		private ushort _port = 7171;

		#endregion

		#region tests implementations

		[Theory, Order(1)]
		[InlineData(NetworkListenerType.TCP)]
		[InlineData(NetworkListenerType.UDP)]
		[InlineData(NetworkListenerType.RUDP)]
		[InlineData(NetworkListenerType.WSBinary)]
		[InlineData(NetworkListenerType.WSText)]
		public void Verify_KingClientConnection_ShouldReturnTrue(NetworkListenerType type)
		{
			//Arrange
			var kingServer = new KingServer(type, _port);

			kingServer.StartAsync(out var cancellationToken);

			Thread.Sleep(15);

			var kingClient = new KingClient(type);

			//Act
			kingClient.Connect(_ip, _port);

			Thread.Sleep(15);

			//Assert
			Assert.True(kingClient.HasConnected);

			//Dispose
			kingClient.Disconnect();
			cancellationToken.Cancel();
			kingServer?.Stop();
		}

		[Theory, Order(2)]
		[InlineData(NetworkListenerType.TCP)]
		[InlineData(NetworkListenerType.UDP)]
		[InlineData(NetworkListenerType.RUDP)]
		[InlineData(NetworkListenerType.WSBinary)]
		[InlineData(NetworkListenerType.WSText)]
		public void Verify_KingClientDisconnection_ShouldHasConnectedFalse(NetworkListenerType type)
		{
			//Arrange
			var kingServer = new KingServer(type, _port);

			kingServer.StartAsync(out var cancellationToken);

			Thread.Sleep(15);

			var kingClient = new KingClient(type);

			//Act
			kingClient.Connect(_ip, _port);

			Thread.Sleep(15);

			kingClient.Disconnect();

			Thread.Sleep(15);

			//Assert
			Assert.False(kingClient.HasConnected);

			//Dispose
			cancellationToken.Cancel();
			kingServer?.Stop();
		}

		[Theory, Order(3)]
		[InlineData(NetworkListenerType.TCP)]
		[InlineData(NetworkListenerType.UDP)]
		[InlineData(NetworkListenerType.RUDP)]
		[InlineData(NetworkListenerType.WSBinary)]
		[InlineData(NetworkListenerType.WSText)]
		public void Verify_KingClientMessageReceivedHandler_ShouldHasReceivedMessage(NetworkListenerType type)
		{
			//Arrange
			KingServer kingServer = new KingServer(type, _port);
			kingServer.StartAsync(out var cancellationToken);

			Thread.Sleep(15);

			var kingClient = new KingClient(type);

			bool hasMessageReceived = false;

			//Act
			kingClient.MessageReceivedHandler += (KingBufferReader reader) =>
			{
				hasMessageReceived = true;
			};

			kingClient.Connect(_ip, _port);

			Thread.Sleep(15);

			var writer = KingBufferWriter.Create();
			writer.Write("Test Message");

			kingServer?.SendMessageToAll(writer);

			Thread.Sleep(15);

			//Assert
			Assert.True(hasMessageReceived);

			//Dispose
			kingClient.Disconnect();
			cancellationToken.Cancel();
			kingServer?.Stop();
		}

		[Theory, Order(4)]
		[InlineData(NetworkListenerType.TCP)]
		[InlineData(NetworkListenerType.UDP)]
		[InlineData(NetworkListenerType.RUDP)]
		[InlineData(NetworkListenerType.WSBinary)]
		public void Verify_KingClientXteaCryptoBinaryMessageReceivedHandler_ShouldHasReceivedXteaCryptoMessage(NetworkListenerType type)
		{
			//Arrange
			KingServer kingServer = new KingServer(type, _port);
			kingServer.StartAsync(out var cancellationToken);

			Thread.Sleep(15);

			var kingClient = new KingClient(type);

			bool hasMessageReceived = false;

			var key = new uint[4] { 10, 15, 20, 25 };

			var messageToSend = "Test Message";
			var messageReceived = string.Empty;

			//Act
			kingClient.MessageReceivedHandler += (KingBufferReader reader) =>
			{
				hasMessageReceived = true;

				var decryptedMessage = XteaEncryptation.Decrypt(reader, 0, key);

				messageReceived = decryptedMessage.ReadString();
			};

			kingClient.Connect(_ip, _port);

			Thread.Sleep(15);

			var writer = KingBufferWriter.Create();
			writer.Write(messageToSend);

			var encriptedMessage = XteaEncryptation.Encrypt(writer, key);

			kingServer?.SendMessageToAll(encriptedMessage);

			Thread.Sleep(15);

			//Assert
			Assert.True(messageToSend.Equals(messageReceived));

			//Dispose
			kingClient.Disconnect();
			cancellationToken.Cancel();
			kingServer?.Stop();
		}

		#endregion
	}
}