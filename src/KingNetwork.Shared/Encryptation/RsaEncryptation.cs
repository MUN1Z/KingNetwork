using KingNetwork.Shared.Extensions;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace KingNetwork.Shared.Encryptation
{
    public class RsaEncryptation
    {
        #region private constants

        private const string _public = "PUBLIC";
        private const string _private = "PRIVATE";

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
            string keyBase64;

            if (keyData.Contains(_public))
            {
                regex = new Regex(@"-----(BEGIN|END) (RSA|OPENSSH|ENCRYPTED) PUBLIC KEY-----[\W]*");
                keyBase64 = regex.Replace(keyData, "");

                if (keyBase64.Contains(_public))
                {
                    var lines = keyBase64.Split('\n');
                    keyBase64 = keyBase64.Replace(lines.FirstOrDefault(), "");
                    keyBase64 = keyBase64.Replace(lines.LastOrDefault(), "");
                    keyBase64 = keyBase64.Trim();
                }
            }
            else
            {
                regex = new Regex(@"-----(BEGIN|END) (RSA|OPENSSH|ENCRYPTED) PRIVATE KEY-----[\W]*");
                keyBase64 = regex.Replace(keyData, "");

                if (keyBase64.Contains(_private))
                {
                    var lines = keyBase64.Split('\n');
                    keyBase64 = keyBase64.Replace(lines.FirstOrDefault(), "");
                    keyBase64 = keyBase64.Replace(lines.LastOrDefault(), "");
                    keyBase64 = keyBase64.Trim();
                }
            }

            var keyBytes = Convert.FromBase64String(keyBase64);
            var provider = new RSACryptoServiceProvider();

            try
            {
                //The ImportRSAPublicKey is not working correctl, bug in dotnet6 imlementation?
                if (keyData.Contains(_public))
                    provider.ImportRSAPublicKey(new ReadOnlySpan<byte>(keyBytes), out var bytesRead);
                else
                    provider.ImportRSAPrivateKey(new ReadOnlySpan<byte>(keyBytes), out var bytesRead);
            }
            catch
            {
                //custom implementation for possible buf of ImportRSAPublicKey in dotnet6 implementation
                if (keyData.Contains(_public))
                    return GetRSAProviderFromParameters(GetPublicParametersFromKeyBytes(keyBytes));
                //Create a private custom generate with parameters
            }

            return provider;
        }

        private static byte[] Combine(byte[] first, byte[] second)
        {
            var bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }

        private static RsaEncryptationParameters GetPublicParametersFromKeyBytes(byte[] keyBytes)
        {
            // encoded OID sequence for  PKCS #1 rsaEncryption szOID_RSA_RSA = "1.2.840.113549.1.1.1"
            byte[] seqOid = { 0x30, 0x0D, 0x06, 0x09, 0x2A, 0x86, 0x48, 0x86, 0xF7, 0x0D, 0x01, 0x01, 0x01, 0x05, 0x00 };
            // ---------  Set up stream to read the asn.1 encoded SubjectPublicKeyInfo blob  ------
            using (var mem = new MemoryStream(keyBytes))
            {
                using (var binr = new BinaryReader(mem))    //wrap Memory Stream with BinaryReader for easy reading
                {
                    var twobytes = binr.ReadUInt16();
                    switch (twobytes)
                    {
                        case 0x8130:
                            binr.ReadByte();    //advance 1 byte
                            break;
                        case 0x8230:
                            binr.ReadInt16();   //advance 2 bytes
                            break;
                        default:
                            return null;
                    }

                    var seq = binr.ReadBytes(15);

                    if (!CompareByteArrays(seq, seqOid))  //make sure Sequence for OID is correct
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8103) //data read as little endian order (actual data order for Bit String is 03 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8203)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    var bt = binr.ReadByte();
                    if (bt != 0x00)     //expect null byte next
                        return null;

                    twobytes = binr.ReadUInt16();
                    if (twobytes == 0x8130) //data read as little endian order (actual data order for Sequence is 30 81)
                        binr.ReadByte();    //advance 1 byte
                    else if (twobytes == 0x8230)
                        binr.ReadInt16();   //advance 2 bytes
                    else
                        return null;

                    twobytes = binr.ReadUInt16();
                    byte lowbyte = 0x00;
                    byte highbyte = 0x00;

                    if (twobytes == 0x8102) //data read as little endian order (actual data order for Integer is 02 81)
                        lowbyte = binr.ReadByte();  // read next bytes which is bytes in modulus
                    else if (twobytes == 0x8202)
                    {
                        highbyte = binr.ReadByte(); //advance 2 bytes
                        lowbyte = binr.ReadByte();
                    }
                    else
                        return null;

                    byte[] modint = { lowbyte, highbyte, 0x00, 0x00 };   //reverse byte order since asn.1 key uses big endian order
                    int modsize = BitConverter.ToInt32(modint, 0);

                    byte firstbyte = binr.ReadByte();
                    binr.BaseStream.Seek(-1, SeekOrigin.Current);

                    if (firstbyte == 0x00)
                    {   //if first byte (highest order) of modulus is zero, don't include it
                        binr.ReadByte();    //skip this null byte
                        modsize -= 1;   //reduce modulus buffer size by 1
                    }

                    byte[] modulus = binr.ReadBytes(modsize); //read the modulus bytes

                    if (binr.ReadByte() != 0x02)            //expect an Integer for the exponent data
                        return null;

                    int expbytes = binr.ReadByte();        // should only need one byte for actual exponent data (for all useful values)
                    byte[] exponent = binr.ReadBytes(expbytes);

                    var rsaKeyInfo = new RsaEncryptationParameters
                    {
                        Modulus = modulus,
                        Exponent = exponent
                    };

                    return rsaKeyInfo;
                }
            }
        }

        private static bool CompareByteArrays(byte[] a, byte[] b)
        {
            if (a.Length != b.Length)
                return false;

            int i = 0;

            foreach (byte c in a)
            {
                if (c != b[i])
                    return false;
                i++;
            }

            return true;
        }

        #endregion
    }
}
