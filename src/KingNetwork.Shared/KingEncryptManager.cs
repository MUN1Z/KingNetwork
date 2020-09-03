using KingNetwork.Shared.Interfaces;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace KingNetwork.Shared
{
    /// <summary>
    /// This class is responsible for represents the king pool  manager of application.
    /// </summary>
    public class KingEncryptManager
    {
        #region private members

        private static KingEncryptManager _instance;

        private string _rsaPublicKey;
        private string _rsaPrivateKey;

        private byte[] _xorKey;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="KingEncryptManager"/>.
        /// </summary>
        public KingEncryptManager()
        {

        }

        #endregion

        /// <summary>
        /// This method is responsible for ccreates and gets the instance of KingEncryptManager.
        /// </summary>
        /// <returns>a instance of KingEncryptManager.</returns>
        public static KingEncryptManager GetInstance()
        {
            if (_instance == null)
                _instance = new KingEncryptManager();

            return _instance;
        }

        #region rsa methods imlementation

        public void SetRSAPublicKey(string path) => _rsaPublicKey = File.ReadAllText(path);
        public void SetRSAPrivateKey(string path) => _rsaPrivateKey = File.ReadAllText(path);

        public IKingBufferWriter EncryptRSA(IKingBufferWriter writer)
        {
            byte[] encrypted;

            using (var rsa = new RSACryptoServiceProvider(4096))
            {
                rsa.PersistKeyInCsp = false;
                rsa.FromXmlString(_rsaPublicKey);
                encrypted = rsa.Encrypt(writer.BufferData, true);
            }

            writer = KingBufferWriter.Create();
            writer.WriteRaw(encrypted, 0, encrypted.Length);

            return writer;
        }

        public IKingBufferReader DecryptRSA(IKingBufferReader reader)
        {
            using (var rsa = new RSACryptoServiceProvider(4096))
            {
                rsa.PersistKeyInCsp = false;
                rsa.FromXmlString(_rsaPrivateKey);

                var decryptedData = rsa.Decrypt(reader.ReadRaw(reader.Length), true);

                return KingBufferReader.Create(decryptedData, 0, decryptedData.Length);
            }
        }

        #endregion

        #region xor encryptation

        public void SetXORKey(string key) => SetXORKey(Encoding.UTF8.GetBytes(key));

        public void SetXORKey(byte[] key) => _xorKey = key;

        public IKingBufferReader DecryptXOR(IKingBufferReader reader)
        {
            var rawData = reader.ReadRaw(reader.Length);

            for (var i = 0; i < reader.Length; i++)
            {
                var offset = i % _xorKey.Length;
                rawData[i] = (byte)(rawData[i] ^ _xorKey[offset]);
            }

            return KingBufferReader.Create(rawData, 0, rawData.Length);
        }

        public IKingBufferWriter EncryptXOR(IKingBufferWriter writer)
        {
            var rawData = writer.BufferData;
            for (var i = 0; i < writer.Length; i++)
            {
                var offset = i % _xorKey.Length;
                rawData[i] = (byte)(rawData[i] ^ _xorKey[offset]);
            }

            writer = KingBufferWriter.Create();
            writer.WriteRaw(rawData, 0, rawData.Length);

            return writer;
        }

        #endregion
    }
}
