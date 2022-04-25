using KingNetwork.Shared.Encryptation;
using KingNetwork.Shared.Extensions;
using System.Security.Cryptography;
using Xunit;
using XUnitPriorityOrderer;

namespace KingNetwork.Shared.Tests
{
    [Order(3)]
	public class SharedTests
	{
		#region constructors

		public SharedTests()
		{

		}

		#endregion

		#region tests implementations

		[Fact, Order(1)]
		public void Verify_XteaEncryptation_ShouldReturnTrue()
		{
			//Arrange
			var key = XteaEncryptation.GenerateKey();

			var messageValue = "Test";

			var messageWriter = KingBufferWriter.Create();
			messageWriter.Write(messageValue);

			//Act
			var encryptedMessageWriter = XteaEncryptation.Encrypt(messageWriter, key);

			var encryptedMessageReader = encryptedMessageWriter.ToKingBufferReader();

			var decryptedMessageReader = XteaEncryptation.Decrypt(encryptedMessageReader, key);

			var decryptedMessageValue = decryptedMessageReader.ReadString();

			//Assert
			Assert.True(decryptedMessageValue.Equals(messageValue));
		}

		[Fact, Order(2)]
		public void Verify_RsaEncryptation_ShouldReturnTrue()
		{
			//Arrange
			var provider = new RSACryptoServiceProvider(1024);

			var pubKey = provider.ExportParameters(false).ToRsaEncryptationParameters();
			var privKey = provider.ExportParameters(true).ToRsaEncryptationParameters();

			var messageValue = "Test";

			var messageWriter = KingBufferWriter.Create();
			messageWriter.Write(messageValue);

			//Act
			var encryptedMessageWriter = RsaEncryptation.Encrypt(messageWriter, pubKey);

			var encryptedMessageReader = encryptedMessageWriter.ToKingBufferReader();

			var decryptedMessageReader = RsaEncryptation.Decrypt(encryptedMessageReader, privKey);

			var decryptedMessageValue = decryptedMessageReader.ReadString();

			//Assert
			Assert.True(decryptedMessageValue.Equals(messageValue));
		}

        #endregion
    }
}
