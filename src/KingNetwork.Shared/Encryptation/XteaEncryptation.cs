using System;

namespace KingNetwork.Shared.Encryptation
{
    public class XteaEncryptation
    {
        #region public methods implementations

        public static KingBufferWriter Encrypt(KingBufferWriter msg, uint[] key)
        {
            if (key == null)
                throw new ArgumentException("Key invalid");

            var lengthInBytes = BitConverter.GetBytes((ushort)(msg.BufferData.Length));
            var bufferData = Combine(lengthInBytes, msg.BufferData);

            var pad = 8 - (bufferData.Length % 8);

            if (pad > 0)
            {
                var paddingData = new byte[pad];

                for (int i = 0; i < pad; i++)
                    paddingData[i] = (byte)0x33;

                bufferData = Combine(bufferData, paddingData);
            }

            var words = Split(bufferData.AsSpan(0, bufferData.Length));

            for (var pos = 0; pos < bufferData.Length / 4; pos += 2)
            {
                uint x_sum = 0, x_delta = 0x9e3779b9, x_count = 32;

                while (x_count-- > 0)
                {
                    words[pos] += (((words[pos + 1] << 4) ^ (words[pos + 1] >> 5)) + words[pos + 1]) ^ (x_sum
                        + key[x_sum & 3]);
                    x_sum += x_delta;
                    words[pos + 1] += (((words[pos] << 4) ^ (words[pos] >> 5)) + words[pos]) ^ (x_sum
                        + key[(x_sum >> 11) & 3]);
                }
            }

            var newBytes = ConvertToBytes(words);

            return KingBufferWriter.Create(newBytes);
        }

        public static bool Encrypt(ref KingBufferWriter msg, uint[] key)
        {
            msg = Encrypt(msg, key);
            return true;
        }

        public static unsafe KingBufferReader Decrypt(KingBufferReader msg, int index, uint[] key)
        {
            var length = msg.BufferData.Length;
            var buffer = msg.BufferData;

            if (length <= index || (length - index) % 8 > 0 || key == null) return null;

            fixed (byte* bufferPtr = buffer)
            {
                var words = (uint*)(bufferPtr + index);
                var msgSize = length - index;

                for (var pos = 0; pos < msgSize / 4; pos += 2)
                {
                    uint xCount = 32, xSum = 0xC6EF3720, xDelta = 0x9E3779B9;

                    while (xCount-- > 0)
                    {
                        words[pos + 1] -= (((words[pos] << 4) ^ (words[pos] >> 5)) + words[pos]) ^ (xSum
                            + key[(xSum >> 11) & 3]);
                        xSum -= xDelta;
                        words[pos] -= (((words[pos + 1] << 4) ^ (words[pos + 1] >> 5)) + words[pos + 1]) ^ (xSum
                            + key[xSum & 3]);
                    }
                }
            }

            var messageSizeLength = sizeof(ushort);

            var bufferData = buffer[messageSizeLength..];
            var bufferDataLength = buffer[..messageSizeLength];

            var lengthOfMessage = BitConverter.ToInt16(bufferDataLength);
            bufferData = bufferData[..lengthOfMessage];

            return KingBufferReader.Create(bufferData);
        }

        public static unsafe KingBufferReader Decrypt(KingBufferReader msg, uint[] key)
        {
            return Decrypt(msg, 0, key);
        }

        public static unsafe bool Decrypt(ref KingBufferReader msg, int index, uint[] key)
        {
            msg = Decrypt(msg, index, key);
            return true;
        }

        public static unsafe bool Decrypt(ref KingBufferReader msg, uint[] key)
        {
            msg = Decrypt(msg, key);
            return true;
        }

        public static uint[] GenerateKey()
        {
            var key = new uint[4];

            var random = new Random();

            key[0] = (uint)random.Next(int.MinValue, int.MaxValue);
            key[1] = (uint)random.Next(int.MinValue, int.MaxValue);
            key[2] = (uint)random.Next(int.MinValue, int.MaxValue);
            key[3] = (uint)random.Next(int.MinValue, int.MaxValue);

            return key;
        }

        #endregion

        #region private methods implementations

        private static byte[] ConvertToBytes(Span<uint> array)
        {
            var bytes = new byte[array.Length * 4];
            var index = 0;
            for (var i = 0; i < array.Length; i++)
            {
                var newBytes = BitConverter.GetBytes(array[i]);

                bytes[index] = newBytes[0];
                bytes[index + 1] = newBytes[1];
                bytes[index + 2] = newBytes[2];
                bytes[index + 3] = newBytes[3];

                index += 4;
            }

            return bytes;
        }

        private static Span<uint> Split(ReadOnlySpan<byte> array)
        {
            var newArray = new uint[array.Length / 4];

            var index = 0;
            for (var i = 0; i < array.Length; i += sizeof(uint))
            {
                newArray[index] = BitConverter.ToUInt32(array.Slice(i, 4));
                index++;
            }

            return newArray.AsSpan();
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
