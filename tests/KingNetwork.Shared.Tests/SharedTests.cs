using Xunit;
using Xunit.Extensions.Ordering;

namespace KingNetwork.Shared.Tests
{
    [Order(3)]
	public class SharedTests
	{
		[Fact, Order(1)]
		public void XOREncrypt_ShouldEncyptAndDecyptMessage()
		{
			var messageIn = "Test Message";

			var writerIn = KingBufferWriter.Create();
			writerIn.Write(messageIn);

			KingEncryptManager.GetInstance().SetXORKey("my_key");

			var writerOut = KingEncryptManager.GetInstance().EncryptXOR(writerIn);

			var readerIn = KingBufferReader.Create(writerOut.BufferData, 0, writerOut.BufferData.Length);

			var readerOut = KingEncryptManager.GetInstance().DecryptXOR(readerIn);

			var messageOut = readerOut.ReadString();

			Assert.Equal(messageIn, messageOut);
		}
	}
}
