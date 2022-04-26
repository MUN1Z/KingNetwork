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

        public static KingBufferWriter Encrypt(KingBufferWriter msg, RSACryptoServiceProvider provider, int padding = 0)
        {
            if (padding == 0)
                return KingBufferWriter.Create(provider.Encrypt(msg.BufferData, true));

            var bufferToNotEcrypt = msg.BufferData[..padding];
            var bufferToEcrypt = msg.BufferData[padding..];
            var encryptedBuffer = provider.Encrypt(bufferToEcrypt, true);

            var finalBuffer = Combine(bufferToNotEcrypt, encryptedBuffer);

            return KingBufferWriter.Create(finalBuffer);
        }

        public static KingBufferWriter Encrypt(KingBufferWriter msg, RsaEncryptationParameters parameters, int padding = 0)
        {
            var provider = GetRSAProviderFromParameters(parameters);
            return Encrypt(msg, provider, padding);
        }

        public static KingBufferWriter Encrypt(KingBufferWriter msg, string pemFilePath, int padding = 0)
        {
            var provider = GetRSAProviderFromPemFile(pemFilePath);
            return Encrypt(msg, provider, padding);
        }

        public static void Encrypt(ref KingBufferWriter msg, RSACryptoServiceProvider provider, int padding = 0)
        {
            msg = Encrypt(msg, provider, padding);
        }

        public static void Encrypt(ref KingBufferWriter msg, RsaEncryptationParameters parameters, int padding = 0)
        {
            msg = Encrypt(msg, parameters, padding);
        }

        public static void Encrypt(ref KingBufferWriter msg, string pemFilePath, int padding = 0)
        {
            msg = Encrypt(msg, pemFilePath, padding);
        }

        public static KingBufferReader Decrypt(KingBufferReader msg, RSACryptoServiceProvider provider, int padding = 0)
        {
            if (padding == 0)
                return KingBufferReader.Create(provider.Decrypt(msg.BufferData, true));

            var bufferToNotDecrypt = msg.BufferData[..padding];
            var bufferToDecrypt = msg.BufferData[padding..];
            var decryptedBuffer = provider.Decrypt(bufferToDecrypt, true);

            var finalBuffer = Combine(bufferToNotDecrypt, decryptedBuffer);

            return KingBufferReader.Create(finalBuffer, padding);
        }

        public static KingBufferReader Decrypt(KingBufferReader msg, RsaEncryptationParameters parameters, int padding = 0)
        {
            var provider = GetRSAProviderFromParameters(parameters);
            return Decrypt(msg, provider, padding);
        }

        public static KingBufferReader Decrypt(KingBufferReader msg, string pemFilePath, int padding = 0)
        {
            var provider = GetRSAProviderFromPemFile(pemFilePath);
            return Decrypt(msg, provider, padding);
        }

        public static void Decrypt(ref KingBufferReader msg, RSACryptoServiceProvider provider, int padding = 0)
        {
            msg = Decrypt(msg, provider, padding);
        }

        public static void Decrypt(ref KingBufferReader msg, RsaEncryptationParameters parameters, int padding = 0)
        {
            msg = Decrypt(msg, parameters, padding);
        }

        public static void Decrypt(ref KingBufferReader msg, string pemFilePath, int padding = 0)
        {
            msg = Decrypt(msg, pemFilePath, padding);
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

        private static byte[] Combine(byte[] first, byte[] second)
        {
            var bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }

        #endregion
    }
}
