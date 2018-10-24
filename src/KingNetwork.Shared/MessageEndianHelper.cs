using System;

namespace KingNetwork.Shared
{
    public static class MessageEndianHelper
    {
        public static void PutBytes(byte[] source, int offset, short value)
        {
            try
            {
                source[offset] = (byte)(value >> 8);
                source[offset + 1] = (byte)value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void PutBytes(byte[] source, int offset, ushort value)
        {
            try
            {
                source[offset] = (byte)(value >> 8);
                source[offset + 1] = (byte)value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void PutBytes(byte[] source, int offset, int value)
        {
            try
            {
                source[offset] = (byte)(value >> 24);
                source[offset + 1] = (byte)(value >> 16);
                source[offset + 2] = (byte)(value >> 8);
                source[offset + 3] = (byte)value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void PutBytes(byte[] source, int offset, uint value)
        {
            try
            {
                source[offset] = (byte)(value >> 24);
                source[offset + 1] = (byte)(value >> 16);
                source[offset + 2] = (byte)(value >> 8);
                source[offset + 3] = (byte)value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void PutBytes(byte[] source, int offset, long value)
        {
            try
            {
                source[offset] = (byte)(value >> 56);
                source[offset + 1] = (byte)(value >> 48);
                source[offset + 2] = (byte)(value >> 40);
                source[offset + 3] = (byte)(value >> 32);
                source[offset + 4] = (byte)(value >> 24);
                source[offset + 5] = (byte)(value >> 16);
                source[offset + 6] = (byte)(value >> 8);
                source[offset + 7] = (byte)value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static void PutBytes(byte[] source, int offset, ulong value)
        {
            try
            {
                source[offset] = (byte)(value >> 56);
                source[offset + 1] = (byte)(value >> 48);
                source[offset + 2] = (byte)(value >> 40);
                source[offset + 3] = (byte)(value >> 32);
                source[offset + 4] = (byte)(value >> 24);
                source[offset + 5] = (byte)(value >> 16);
                source[offset + 6] = (byte)(value >> 8);
                source[offset + 7] = (byte)value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public static short GetInt16(byte[] source, int offset)
        {
            short value = 0;

            try
            {
                value = (short)(source[offset] << 8 | source[offset + 1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return 0;
        }

        public static ushort GetUInt16(byte[] source, int offset)
        {
            ushort value = 0;

            try
            {
                value =  (ushort)(source[offset] << 8 | source[offset + 1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return value;
        }

        public static int GetInt32(byte[] source, int offset)
        {
            int value = 0;

            try
            {
                value = source[offset] << 24 | source[offset + 1] << 16 | source[offset + 2] << 8 | source[offset + 3];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return value;
        }

        public static uint GetUInt32(byte[] source, int offset)
        {
            uint value = 0;

            try
            {
                value = (uint)(source[offset] << 24 | source[offset + 1] << 16 | source[offset + 2] << 8 | source[offset + 3]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return value;
        }

        public static long GetInt64(byte[] source, int offset)
        {
            long value = 0;

            try
            {
                value = (long)((ulong)source[offset] << 56 | (ulong)source[offset + 1] << 48 | (ulong)source[offset + 2] << 40 | (ulong)source[offset + 3] << 32 | (ulong)source[offset + 4] << 24 | (ulong)source[offset + 5] << 16 | (ulong)source[offset + 6] << 8 | source[offset + 7]);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return value;
        }

        public static ulong GetUInt64(byte[] source, int offset)
        {
            ulong value = 0;

            try
            {
                value = (ulong)source[offset] << 56 | (ulong)source[offset + 1] << 48 | (ulong)source[offset + 2] << 40 | (ulong)source[offset + 3] << 32 | (ulong)source[offset + 4] << 24 | (ulong)source[offset + 5] << 16 | (ulong)source[offset + 6] << 8 | source[offset + 7];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return value;
        }
    }
}
