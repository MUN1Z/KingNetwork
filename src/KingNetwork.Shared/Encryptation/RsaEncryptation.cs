using KingNetwork.Shared.Extensions;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace KingNetwork.Shared.Encryptation
{
    public class RsaEncryptation
    {
        #region private constants

        private const string _public = "PUBLIC";

        #endregion

        #region public methods implementations

        public static KingBufferWriter Encrypt(KingBufferWriter msg, RSACryptoServiceProvider provider)
        {
            var encryptedBuffer = provider.Encrypt(msg.BufferData, true);
            return KingBufferWriter.Create(encryptedBuffer.Length, encryptedBuffer);
        }

        public static KingBufferWriter Encrypt(KingBufferWriter msg, RsaEncryptationParameters parameters)
        {
            var provider = GetRSAProviderFromParameters(parameters);
            return Encrypt(msg, provider);
        }

        public static KingBufferWriter Encrypt(KingBufferWriter msg, string pemFilePath)
        {
            var provider = GetRSAProviderFromPemFile(pemFilePath);
            return Encrypt(msg, provider);
        }

        public static KingBufferReader Decrypt(KingBufferReader msg, RSACryptoServiceProvider provider)
        {
            var decryptedBuffer = provider.Decrypt(msg.BufferData, true);
            return KingBufferReader.Create(decryptedBuffer, 0, decryptedBuffer.Length);
        }

        public static KingBufferReader Decrypt(KingBufferReader msg, RsaEncryptationParameters parameters)
        {
            var provider = GetRSAProviderFromParameters(parameters);
            return Decrypt(msg, provider);
        }

        public static KingBufferReader Decrypt(KingBufferReader msg, string pemFilePath)
        {
            var provider = GetRSAProviderFromPemFile(pemFilePath);
            return Decrypt(msg, provider);
        }

        #endregion

        #region private methods implementations

        private static RSACryptoServiceProvider GetRSAProviderFromParameters(RsaEncryptationParameters parameters)
        {
            var provider = new RSACryptoServiceProvider();
            provider.ImportParameters(parameters.ToRSAParameters());
            return provider;
        }

        private static RSACryptoServiceProvider GetRSAProviderFromPemFile(string pemFilePath)
        {
            var keyData = File.ReadAllText(pemFilePath).Trim();

            Regex regex;

            if (keyData.Contains(_public))
                regex = new Regex(@"-----(BEGIN|END) (RSA|OPENSSH|ENCRYPTED) PUBLIC KEY-----[\W]*");
            else
                regex = new Regex(@"-----(BEGIN|END) (RSA|OPENSSH|ENCRYPTED) PRIVATE KEY-----[\W]*");

            var keyBase64 = regex.Replace(keyData, "");
            var keyBytes = Convert.FromBase64String(keyBase64);
            var provider = new RSACryptoServiceProvider();

            if (keyData.Contains(_public))
                provider.ImportRSAPublicKey(new ReadOnlySpan<byte>(keyBytes), out var bytesRead);
            else
                provider.ImportRSAPublicKey(new ReadOnlySpan<byte>(keyBytes), out var bytesRead);

            return provider;
        }

        #endregion
    }
}
