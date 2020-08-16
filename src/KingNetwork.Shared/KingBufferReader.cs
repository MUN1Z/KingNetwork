﻿using System;
using System.IO;
using System.Text;

namespace KingNetwork.Shared
{
    /// <summary>
    /// This class is responsible for represents the buffer of application.
    /// </summary>
    public class KingBufferReader : IDisposable
    {
        #region private members 	
        
        /// <summary>
        /// The buffer disposed flag.
        /// </summary>
        protected bool _disposedValue;

        /// <summary>
        /// The byte array data of buffer.
        /// </summary>
        private byte[] bytes;
        
        /// <summary>
        /// The byte array buffer.
        /// </summary>
        private int dataOffset;
        
        /// <summary>
        /// The byte array buffer.
        /// </summary>
        private byte[] _buffer = new byte[8];
        
        /// <summary>
        /// The encoding value to strings.
        /// </summary>
        public Encoding Encoding { get; set; }
        
        /// <summary>
        /// The length value of buffer.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// The position value of buffer.
        /// </summary>
        public int Position { get; private set; }

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="KingBufferReader"/>.
        /// </summary>
        internal KingBufferReader() { }

        #endregion

        #region internal static methods implementation   

        /// <summary>
        /// Method responsible for create a buffer instance.
        /// </summary>
        /// <param name="bytes">The byte array value.</param>
        /// <param name="dataOffset">The dataOffset of byte array data.</param>
        /// <param name="dataLength">The dataLength of byte array data.</param>
        public static KingBufferReader Create(byte[] bytes, int dataOffset, int dataLength)
        {
            var reader = new KingBufferReader();
            reader.Initialize(bytes, dataOffset, dataLength);
            return reader;
        }

        #endregion

        #region public methods implementation   

        /// <summary>
        /// Method responsible for read a byte value from buffer.
        /// </summary>
        /// <returns>The byte value from buffer.</returns>
        public byte ReadByte()
        {
            if (Position >= Length)
                throw new EndOfStreamException();

            return bytes[dataOffset + Position++];
        }

        /// <summary>
        /// Method responsible for read a char value from buffer.
        /// </summary>
        /// <returns>The char value from buffer.</returns>
        public char ReadChar() => ReadChars()[0];

        /// <summary>
        /// Method responsible for read a bool value from buffer.
        /// </summary>
        /// <returns>The bool value from buffer.</returns>
        public bool ReadBoolean() => ReadByte() == 1;

        /// <summary>
        /// Method responsible for read a double value from buffer.
        /// </summary>
        /// <returns>The double value from buffer.</returns>
        public double ReadDouble()
        {
            if (Position + 8 > Length)
                throw new EndOfStreamException();

            Buffer.BlockCopy(bytes, dataOffset + Position, _buffer, 0, 8);
            Position += 8;

            if (BitConverter.IsLittleEndian)
                Array.Reverse(_buffer, 0, 8);

            return BitConverter.ToDouble(_buffer, 0);
        }

        /// <summary>
        /// Method responsible for read a short value from buffer.
        /// </summary>
        /// <returns>The short value from buffer.</returns>
        public short ReadInt16()
        {
            if (Position + 2 > Length)
                throw new EndOfStreamException();

            short result = ReadInt16(bytes, dataOffset + Position);
            Position += 2;
            return result;
        }

        /// <summary>
        /// Method responsible for read a int value from buffer.
        /// </summary>
        /// <returns>The int value from buffer.</returns>
        public int ReadInt32()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException();

            int result = ReadInt32(bytes, dataOffset + Position);
            Position += 4;
            return result;
        }

        /// <summary>
        /// Method responsible for read a long value from buffer.
        /// </summary>
        /// <returns>The long value from buffer.</returns>
        public long ReadInt64()
        {
            if (Position + 8 > Length)
                throw new EndOfStreamException();

            long result = ReadInt64(bytes, dataOffset + Position);
            Position += 8;
            return result;
        }

        /// <summary>
        /// Method responsible for sbyte a char value from buffer.
        /// </summary>
        /// <returns>The sbyte value from buffer.</returns>
        public sbyte ReadSByte() => (sbyte)ReadByte();

        /// <summary>
        /// Method responsible for read a float value from buffer.
        /// </summary>
        /// <returns>The float value from buffer.</returns>
        public float ReadSingle()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException();

            Buffer.BlockCopy(bytes, dataOffset + Position, _buffer, 0, 4);
            Position += 4;

            if (BitConverter.IsLittleEndian)
                Array.Reverse(_buffer, 0, 4);

            return BitConverter.ToSingle(_buffer, 0);
        }

        /// <summary>
        /// Method responsible for read a string value from buffer.
        /// </summary>
        /// <returns>The string value from buffer.</returns>
        public string ReadString() => ReadString(Encoding);

        /// <summary>
        /// Method responsible for read a string value from buffer.
        /// </summary>
        /// <param name="encoding">The encoding value to read string in the buffer.</param>
        /// <returns>The string value from buffer.</returns>
        public string ReadString(Encoding encoding)
        {
            byte[] array = ReadBytes();
            return encoding.GetString(array);
        }

        /// <summary>
        /// Method responsible for read a ushort value from buffer.
        /// </summary>
        /// <returns>The ushort value from buffer.</returns>
        public ushort ReadUInt16()
        {
            if (Position + 2 > Length)
                throw new EndOfStreamException();

            ushort result = ReadUInt16(bytes, dataOffset + Position);
            Position += 2;
            return result;
        }

        /// <summary>
        /// Method responsible for read a uint value from buffer.
        /// </summary>
        /// <returns>The uint value from buffer.</returns>
        public uint ReadUInt32()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException();

            uint result = ReadUInt32(bytes, dataOffset + Position);
            Position += 4;
            return result;
        }

        /// <summary>
        /// Method responsible for read a ulong value from buffer.
        /// </summary>
        /// <returns>The ulong value from buffer.</returns>
        public ulong ReadUInt64()
        {
            if (Position + 8 > Length)
                throw new EndOfStreamException();

            ulong result = ReadUInt64(bytes, dataOffset + Position);
            Position += 8;
            return result;
        }

        /// <summary>
        /// Method responsible for read a message packet value from buffer using generics.
        /// </summary>
        /// <returns>Returns a generic representation of message packet value from buffer.</returns>
        public TPacket ReadMessagePacket<TPacket>() where TPacket : IConvertible => (TPacket)(IConvertible)ReadByte();

        /// <summary>
        /// Method responsible for read a byte array value from buffer.
        /// </summary>
        /// <returns>The byte array value from buffer.</returns>
        public byte[] ReadBytes()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException("Length parameter exceeded bounds");

            int num = ReadInt32(bytes, dataOffset + Position);

            if (Position + 4 + num > Length)
                throw new EndOfStreamException("Elements exceeded bounds");

            byte[] array = new byte[num];
            Buffer.BlockCopy(bytes, dataOffset + Position + 4, array, 0, num);
            Position += 4 + num;
            return array;
        }

        /// <summary>
        /// Method responsible for read a char array value from buffer.
        /// </summary>
        /// <returns>The char array value from buffer.</returns>
        public char[] ReadChars() => ReadChars(Encoding);

        /// <summary>
        /// Method responsible for read a char array value from buffer.
        /// </summary>
        /// <param name="encoding">The encoding value to read string in the buffer.</param>
        /// <returns>The char array value from buffer.</returns>
        public char[] ReadChars(Encoding encoding) => encoding.GetChars(ReadBytes());

        /// <summary>
        /// Method responsible for read a bool array value from buffer.
        /// </summary>
        /// <returns>The bool array value from buffer.</returns>
        public bool[] ReadBooleans()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException("Length parameter exceeded bounds");

            int num = ReadInt32(bytes, dataOffset + Position);
            int num2 = (int)Math.Ceiling((double)num / 8.0);

            if (Position + 4 + num2 > Length)
                throw new EndOfStreamException("Elements exceeded bounds");

            bool[] array = new bool[num];
            int num3 = 0;

            for (int i = 0; i < num2; i++)
            {
                byte b = bytes[dataOffset + Position + 4 + i];

                for (int j = 0; j < 8; j++)
                {
                    if (num3 >= num)
                        break;

                    b = (byte)(b >> 1 | b << 31);
                    array[num3++] = ((b & 1) == 1);
                }
            }

            Position += 4 + num2;
            return array;
        }

        /// <summary>
        /// Method responsible for read a double array value from buffer.
        /// </summary>
        /// <returns>The double array value from buffer.</returns>
        public double[] ReadDoubles()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException("Length parameter exceeded bounds");

            int num = ReadInt32(bytes, dataOffset + Position);

            if (Position + 4 + num * 8 > Length)
                throw new EndOfStreamException("Elements exceeded bounds");

            double[] array = new double[num];
            int num2 = 0;
            int num3 = dataOffset + Position + 4;

            while (num2 < num)
            {
                Buffer.BlockCopy(bytes, num3, _buffer, 0, 8);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(_buffer, 0, 8);

                array[num2] = BitConverter.ToDouble(_buffer, 0);
                num2++;
                num3 += 8;
            }

            Position += 4 + num * 8;
            return array;
        }

        /// <summary>
        /// Method responsible for read a short array value from buffer.
        /// </summary>
        /// <returns>The short array value from buffer.</returns>
        public short[] ReadInt16s()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException("Length parameter exceeded bounds");

            int num = ReadInt32(bytes, dataOffset + Position);

            if (Position + 4 + num * 2 > Length)
                throw new EndOfStreamException("Elements exceeded bounds");

            short[] array = new short[num];
            int num2 = 0;
            int num3 = dataOffset + Position + 4;

            while (num2 < num)
            {
                array[num2] = ReadInt16(bytes, num3);
                num2++;
                num3 += 2;
            }

            Position += 4 + num * 2;
            return array;
        }

        /// <summary>
        /// Method responsible for read a int array value from buffer.
        /// </summary>
        /// <returns>The int array value from buffer.</returns>
        public int[] ReadInt32s()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException("Length parameter exceeded bounds");

            int num = ReadInt32(bytes, dataOffset + Position);

            if (Position + 4 + num * 4 > Length)
                throw new EndOfStreamException("Elements exceeded bounds");

            int[] array = new int[num];
            int num2 = 0;
            int num3 = dataOffset + Position + 4;

            while (num2 < num)
            {
                array[num2] = ReadInt32(bytes, num3);
                num2++;
                num3 += 4;
            }

            Position += 4 + num * 4;
            return array;
        }

        /// <summary>
        /// Method responsible for read a long array value from buffer.
        /// </summary>
        /// <returns>The long array value from buffer.</returns>
        public long[] ReadInt64s()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException("Length parameter exceeded bounds");

            int num = ReadInt32(bytes, dataOffset + Position);

            if (Position + 4 + num * 8 > Length)
                throw new EndOfStreamException("Elements exceeded bounds");

            long[] array = new long[num];
            int num2 = 0;
            int num3 = dataOffset + Position + 4;

            while (num2 < num)
            {
                array[num2] = ReadInt64(bytes, num3);
                num2++;
                num3 += 8;
            }

            Position += 4 + num * 8;
            return array;
        }

        /// <summary>
        /// Method responsible for read a sbyte array value from buffer.
        /// </summary>
        /// <returns>The sbyte array value from buffer.</returns>
        public sbyte[] ReadSBytes()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException("Length parameter exceeded bounds");

            int num = ReadInt32(bytes, dataOffset + Position);

            if (Position + 4 + num > Length)
                throw new EndOfStreamException("Elements exceeded bounds");

            sbyte[] array = new sbyte[num];
            Buffer.BlockCopy(bytes, dataOffset + Position + 4, array, 0, num);
            Position += 4 + num;

            return array;
        }

        /// <summary>
        /// Method responsible for read a float array value from buffer.
        /// </summary>
        /// <returns>The float array value from buffer.</returns>
        public float[] ReadSingles()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException("Length parameter exceeded bounds");

            int num = ReadInt32(bytes, dataOffset + Position);

            if (Position + 4 + num * 4 > Length)
                throw new EndOfStreamException("Elements exceeded bounds");

            float[] array = new float[num];
            int num2 = 0;
            int num3 = dataOffset + Position + 4;

            while (num2 < num)
            {
                Buffer.BlockCopy(bytes, num3, _buffer, 0, 4);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(_buffer, 0, 4);

                array[num2] = BitConverter.ToSingle(_buffer, 0);
                num2++;
                num3 += 4;
            }

            Position += 4 + num * 4;
            return array;
        }

        /// <summary>
        /// Method responsible for read a string array value from buffer.
        /// </summary>
        /// <returns>The string array value from buffer.</returns>
        public string[] ReadStrings()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException("Length parameter exceeded bounds");

            int num = ReadInt32(bytes, dataOffset + Position);
            Position += 4;
            string[] array = new string[num];

            for (int i = 0; i < num; i++)
                array[i] = ReadString();

            return array;
        }

        /// <summary>
        /// Method responsible for read a ushort array value from buffer.
        /// </summary>
        /// <returns>The ushort array value from buffer.</returns>
        public ushort[] ReadUInt16s()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException("Length parameter exceeded bounds");

            int num = ReadInt32(bytes, dataOffset + Position);

            if (Position + 4 + num * 2 > Length)
                throw new EndOfStreamException("Elements exceeded bounds");

            ushort[] array = new ushort[num];
            int num2 = 0;
            int num3 = dataOffset + Position + 4;

            while (num2 < num)
            {
                array[num2] = ReadUInt16(bytes, num3);
                num2++;
                num3 += 2;
            }

            Position += 4 + num * 2;
            return array;
        }

        /// <summary>
        /// Method responsible for read a uint array value from buffer.
        /// </summary>
        /// <returns>The uint array value from buffer.</returns>
        public uint[] ReadUInt32s()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException("Length parameter exceeded bounds");

            int num = ReadInt32(bytes, dataOffset + Position);

            if (Position + 4 + num * 4 > Length)
                throw new EndOfStreamException("Elements exceeded bounds");

            uint[] array = new uint[num];
            int num2 = 0;
            int num3 = dataOffset + Position + 4;

            while (num2 < num)
            {
                array[num2] = ReadUInt32(bytes, num3);
                num2++;
                num3 += 4;
            }

            Position += 4 + num * 4;
            return array;
        }

        /// <summary>
        /// Method responsible for read a ulong array value from buffer.
        /// </summary>
        /// <returns>The ulong array value from buffer.</returns>
        public ulong[] ReadUInt64s()
        {
            if (Position + 4 > Length)
                throw new EndOfStreamException("Length parameter exceeded bounds");

            int num = ReadInt32(bytes, dataOffset + Position);

            if (Position + 4 + num * 8 > Length)
                throw new EndOfStreamException("Elements exceeded bounds");

            ulong[] array = new ulong[num];
            int num2 = 0;
            int num3 = dataOffset + Position + 4;

            while (num2 < num)
            {
                array[num2] = ReadUInt64(bytes, num3);
                num2++;
                num3 += 8;
            }

            Position += 4 + num * 8;
            return array;
        }

        /// <summary>
        /// Method responsible for read a raw data value from buffer.
        /// </summary>
        /// <param name="length">The length of raw data to read from buffer.</param>
        /// <returns>The byte array value from buffer.</returns>
        public byte[] ReadRaw(int length)
        {
            if (Position + length > Length)
                throw new EndOfStreamException();

            byte[] result = new byte[length];
            ReadRawInto(result, 0, length);
            return result;
        }

        /// <summary>
        /// Method responsible for read a raw data into data buffer.
        /// </summary>
        /// <param name="buffer">The buffer array value.</param>
        /// <param name="offset">The offset of byte array data.</param>
        /// <param name="length">The length of byte array data.</param>
        public void ReadRawInto(byte[] buffer, int offset, int length)
        {
            if (Position + length > Length)
                throw new EndOfStreamException();

            Buffer.BlockCopy(bytes, dataOffset + Position, buffer, offset, length);
            Position += length;
        }

        /// <summary>
        /// Method responsible for reset the buffer.
        /// </summary>
        public void Reset()
        {
            _buffer = new byte[8];

            Position = 0;
            Length = 0;
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// Method responsible for dispose the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        #region protected methods
        
        /// <summary>
        /// Method responsible for dispose the instance.
        /// </summary>
        /// <param name="disposing">The flag for dispose object.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
                if (disposing)
                    Reset();

            _disposedValue = true;
        }

        #endregion

        #region private methods implementations

        /// <summary>
        /// Method responsible for initialize the buffer data.
        /// </summary>
        /// <param name="bytes">The byte array value.</param>
        /// <param name="dataOffset">The dataOffset of byte array data.</param>
        /// <param name="dataLength">The dataLength of byte array data.</param>
        private void Initialize(byte[] bytes, int dataOffset, int dataLength)
        {
            Encoding = Encoding.UTF8;
            this.bytes = bytes;
            this.dataOffset = dataOffset;
            Length = dataLength;
            Position = 0;
        }

        /// <summary>
        /// Method responsible for read the short value from source byte array.
        /// </summary>
        /// <param name="source">The byte array source value.</param>
        /// <param name="offset">The offset of byte array source data.</param>
        /// <returns>The short value from source buffer.</returns>
        private short ReadInt16(byte[] source, int offset)
        {
            return (short)(source[offset] << 8 | source[offset + 1]);
        }

        /// <summary>
        /// Method responsible for read the ushort value from source byte array.
        /// </summary>
        /// <param name="source">The byte array source value.</param>
        /// <param name="offset">The offset of byte array source data.</param>
        /// <returns>The ushort value from source buffer.</returns>
        private ushort ReadUInt16(byte[] source, int offset)
        {
            return (ushort)(source[offset] << 8 | source[offset + 1]);
        }

        /// <summary>
        /// Method responsible for read the int value from source byte array.
        /// </summary>
        /// <param name="source">The byte array source value.</param>
        /// <param name="offset">The offset of byte array source data.</param>
        /// <returns>The int value from source buffer.</returns>
        private int ReadInt32(byte[] source, int offset)
        {
            return source[offset] << 24 | source[offset + 1] << 16 | source[offset + 2] << 8 | source[offset + 3];
        }

        /// <summary>
        /// Method responsible for read the uint value from source byte array.
        /// </summary>
        /// <param name="source">The byte array source value.</param>
        /// <param name="offset">The offset of byte array source data.</param>
        /// <returns>The uint value from source buffer.</returns>
        private uint ReadUInt32(byte[] source, int offset)
        {
            return (uint)(source[offset] << 24 | source[offset + 1] << 16 | source[offset + 2] << 8 | source[offset + 3]);
        }

        /// <summary>
        /// Method responsible for read the long value from source byte array.
        /// </summary>
        /// <param name="source">The byte array source value.</param>
        /// <param name="offset">The offset of byte array source data.</param>
        /// <returns>The long value from source buffer.</returns>
        private long ReadInt64(byte[] source, int offset)
        {
            return (long)((ulong)source[offset] << 56 | (ulong)source[offset + 1] << 48 | (ulong)source[offset + 2] << 40 | (ulong)source[offset + 3] << 32 | (ulong)source[offset + 4] << 24 | (ulong)source[offset + 5] << 16 | (ulong)source[offset + 6] << 8 | source[offset + 7]);
        }

        /// <summary>
        /// Method responsible for read the ulong value from source byte array.
        /// </summary>
        /// <param name="source">The byte array source value.</param>
        /// <param name="offset">The offset of byte array source data.</param>
        /// <returns>The ulong value from source buffer.</returns>
        private ulong ReadUInt64(byte[] source, int offset)
        {
            return (ulong)source[offset] << 56 | (ulong)source[offset + 1] << 48 | (ulong)source[offset + 2] << 40 | (ulong)source[offset + 3] << 32 | (ulong)source[offset + 4] << 24 | (ulong)source[offset + 5] << 16 | (ulong)source[offset + 6] << 8 | source[offset + 7];
        }

        #endregion
    }
}
