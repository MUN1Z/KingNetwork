using KingNetwork.Server;
using KingNetwork.Shared;
using KingNetwork.Shared.Encryptation;
using KingNetwork.Shared.Extensions;
using System.Security.Cryptography;
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
        public async Task Verify_KingClientConnection_ShouldReturnTrueAsync(NetworkListenerType type)
        {
            //Arrange
            var kingServer = new KingServer(type, _port);

            var cancellationToken = new CancellationTokenSource();
            await kingServer.StartAsync(cancellationToken);

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

        [Theory, Order(1)]
        [InlineData(NetworkListenerType.TCP)]
        [InlineData(NetworkListenerType.UDP)]
        [InlineData(NetworkListenerType.RUDP)]
        [InlineData(NetworkListenerType.WSBinary)]
        [InlineData(NetworkListenerType.WSText)]
        public async Task Verify_KingClientConnectionSuccessHandler_ShouldReturnTrueAsync(NetworkListenerType type)
        {
            //Arrange
            var kingServer = new KingServer(type, _port);

            var cancellationToken = new CancellationTokenSource();
            await kingServer.StartAsync(cancellationToken);

            Thread.Sleep(15);

            var kingClient = new KingClient(type);

            var hasConnected = false;

            kingClient.OnConnectionSuccessHandler += () => { hasConnected = true; };
            kingClient.OnConnectionFailHandler += () => { hasConnected = false; };

            //Act
            kingClient.Connect(_ip, _port);

            Thread.Sleep(15);

            //Assert
            Assert.True(hasConnected);

            //Dispose
            kingClient.Disconnect();
            cancellationToken.Cancel();
            kingServer?.Stop();
        }

        [Theory, Order(1)]
        [InlineData(NetworkListenerType.TCP)]
        [InlineData(NetworkListenerType.UDP)]
        [InlineData(NetworkListenerType.RUDP)]
        [InlineData(NetworkListenerType.WSBinary)]
        [InlineData(NetworkListenerType.WSText)]
        public async Task Verify_KingClientConnectionFailHandler_ShouldReturnTrueAsync(NetworkListenerType type)
        {
            //Arrange
            var kingServer = new KingServer(type, _port);

            var cancellationToken = new CancellationTokenSource();
            await kingServer.StartAsync(cancellationToken);

            Thread.Sleep(15);

            var kingClient = new KingClient(type);

            var hasConnected = true;

            kingClient.OnConnectionSuccessHandler += () => { hasConnected = true; };
            kingClient.OnConnectionFailHandler += () => { hasConnected = false; };

            //Act
            kingClient.Connect(_ip, 7272);

            Thread.Sleep(15);

            //Assert
            Assert.False(hasConnected);

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
        public async Task Verify_KingClientDisconnection_ShouldHasConnectedFalseAsync(NetworkListenerType type)
        {
            //Arrange
            var kingServer = new KingServer(type, _port);

            var cancellationToken = new CancellationTokenSource();
            await kingServer.StartAsync(cancellationToken);

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
        public async Task Verify_KingClientMessageReceivedHandler_ShouldHasReceivedMessageAsync(NetworkListenerType type)
        {
            //Arrange
            KingServer kingServer = new KingServer(type, _port);

            var cancellationToken = new CancellationTokenSource();
            await kingServer.StartAsync(cancellationToken);

            Thread.Sleep(15);

            var kingClient = new KingClient(type);

            bool hasMessageReceived = false;

            //Act
            kingClient.OnMessageReceivedHandler += (KingBufferReader reader) =>
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
        public async Task Verify_KingClientXteaCryptoBinaryMessageReceivedHandler_ShouldHasReceivedXteaCryptoMessageAsync(NetworkListenerType type)
        {
            //Arrange
            KingServer kingServer = new KingServer(type, _port);

            var cancellationToken = new CancellationTokenSource();
            await kingServer.StartAsync(cancellationToken);

            Thread.Sleep(15);

            var kingClient = new KingClient(type);

            var key = XteaEncryptation.GenerateKey();

            var messageToSend = "Test Message";
            var messageReceived = string.Empty;

            //Act
            kingClient.OnMessageReceivedHandler += (KingBufferReader reader) =>
            {
                var decryptedMessage = XteaEncryptation.Decrypt(reader, key);

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

        [Theory, Order(5)]
        [InlineData(NetworkListenerType.TCP)]
        [InlineData(NetworkListenerType.UDP)]
        [InlineData(NetworkListenerType.RUDP)]
        [InlineData(NetworkListenerType.WSBinary)]
        public async Task Verify_KingClientRsaCryptoBinaryMessageReceivedHandler_ShouldHasReceivedRsaCryptoMessageAsync(NetworkListenerType type)
        {
            //Arrange
            KingServer kingServer = new KingServer(type, _port);
            var cancellationToken = new CancellationTokenSource();
            await kingServer.StartAsync(cancellationToken);

            Thread.Sleep(15);

            var kingClient = new KingClient(type);

            //Arrange
            var provider = new RSACryptoServiceProvider(1024);

            var pubKey = provider.ExportParameters(false).ToRsaEncryptationParameters();
            var privKey = provider.ExportParameters(true).ToRsaEncryptationParameters();

            var messageToSend = "Test Message";
            var messageReceived = string.Empty;

            //Act
            kingClient.OnMessageReceivedHandler += (KingBufferReader reader) =>
            {
                var decryptedMessage = RsaEncryptation.Decrypt(reader, privKey);

                messageReceived = decryptedMessage.ReadString();
            };

            kingClient.Connect(_ip, _port);

            Thread.Sleep(15);

            var writer = KingBufferWriter.Create();
            writer.Write(messageToSend);

            var encriptedMessage = RsaEncryptation.Encrypt(writer, pubKey);

            kingServer?.SendMessageToAll(encriptedMessage);

            Thread.Sleep(15);

            //Assert
            Assert.True(messageToSend.Equals(messageReceived));

            //Dispose
            kingClient.Disconnect();
            cancellationToken.Cancel();
            kingServer?.Stop();
        }

        [Theory, Order(5)]
        [InlineData(NetworkListenerType.TCP)]
        [InlineData(NetworkListenerType.UDP)]
        [InlineData(NetworkListenerType.RUDP)]
        [InlineData(NetworkListenerType.WSBinary)]
        public async Task Verify_KingClientLoginWithRsaAndXteaSecurityBinary_ShouldHasSucessAsync(NetworkListenerType type)
        {
            //Arrange
            KingServer kingServer = new KingServer(type, _port);
            var cancellationToken = new CancellationTokenSource();
            await kingServer.StartAsync(cancellationToken);

            Thread.Sleep(15);

            var kingClient = new KingClient(type);

            var provider = new RSACryptoServiceProvider(1024);

            var pubKey = provider.ExportParameters(false).ToRsaEncryptationParameters();
            var privKey = provider.ExportParameters(true).ToRsaEncryptationParameters();

            var xteaKey = XteaEncryptation.GenerateKey();

            var clientsAndXteakeys = new Dictionary<ClientConnection, uint[]>();

            var loginRequestPacket = (byte)0;
            var loginResponsePacket = (byte)1;

            var loginEmail = "test@mail.com";
            var loginPass = "1234";
            var loginSucess = "Login Sucess!";

            var messageReceivedAfterLogin = string.Empty;

            //Act
            kingServer.OnMessageReceivedHandler += (ClientConnection client, KingBufferReader reader) =>
            {
                var packet = reader.ReadMessagePacket<byte>();

                if (packet == loginRequestPacket)
                {
                    RsaEncryptation.Decrypt(ref reader, privKey, 1);

                    var email = reader.ReadString();
                    var password = reader.ReadString();

                    var xteaKeyFromClient = new uint[4];
                    xteaKeyFromClient[0] = reader.ReadUInt32();
                    xteaKeyFromClient[1] = reader.ReadUInt32();
                    xteaKeyFromClient[2] = reader.ReadUInt32();
                    xteaKeyFromClient[3] = reader.ReadUInt32();

                    if (email.Equals(loginEmail) && password.Equals(loginPass))
                    {
                        clientsAndXteakeys.Add(client, xteaKeyFromClient);

                        using var writer = KingBufferWriter.Create();
                        writer.Write(loginResponsePacket);
                        writer.Write(loginSucess);

                        //Use only xtea encrypt in client and server now
                        var writterEncryptedXtea = XteaEncryptation.Encrypt(writer, xteaKeyFromClient);

                        client.SendMessage(writterEncryptedXtea);
                    }
                }
            };

            kingClient.OnMessageReceivedHandler += (KingBufferReader reader) =>
            {
                XteaEncryptation.Decrypt(ref reader, xteaKey);

                var packet = reader.ReadMessagePacket<byte>();

                if (packet == loginResponsePacket)
                {
                    messageReceivedAfterLogin = reader.ReadString();
                }

                //Use only xtea encrypt in client and server now
            };

            kingClient.Connect(_ip, _port);

            Thread.Sleep(15);

            var writer = KingBufferWriter.Create();

            writer.Write(loginRequestPacket);
            writer.Write(loginEmail);
            writer.Write(loginPass);

            writer.Write(xteaKey[0]);
            writer.Write(xteaKey[1]);
            writer.Write(xteaKey[2]);
            writer.Write(xteaKey[3]);

            //On Login has crypted message with rsa encryptation, after this, will be used the xtea encryptation betwen server and client to use max speed of processings and networks.
            RsaEncryptation.Encrypt(ref writer, pubKey, 1);

            kingClient.SendMessage(writer);

            Thread.Sleep(15);

            //Assert
            Assert.True(messageReceivedAfterLogin.Equals(loginSucess));

            //Dispose
            kingClient.Disconnect();
            cancellationToken.Cancel();
            kingServer?.Stop();
        }

        #endregion
    }
}