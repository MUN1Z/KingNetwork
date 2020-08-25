using KingNetwork.Shared.Interfaces;
using System;
using System.Text;

namespace KingNetwork.Shared
{
    /// <summary>
    /// This class is responsible for represents the buffer of application.
    /// </summary>
    public class KingBufferWriter : IKingBufferWriter, IDisposable
    {
        #region private members 	
        
        /// <summary>
        /// The buffer disposed flag.
        /// </summary>
        protected bool _disposedValue;

        /// <summary>
        /// The byte array buffer.
        /// </summary>
        private byte[] _buffer;

        #endregion

        #region properties
        
        /// <inheritdoc/>
        public byte[] BufferData => _buffer;

        /// <inheritdoc/>
        public Encoding Encoding { get; private set; }

        /// <inheritdoc/>
        public int Length { get; private set; }

        /// <inheritdoc/>
        public int Capacity => _buffer.Length;

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="KingBufferWriter"/>.
        /// </summary>
        internal KingBufferWriter() { }

        #endregion

        #region public static methods implementation   

        /// <summary>
        /// Method responsible for create a buffer instance.
        /// </summary>
        /// <param name="capacity">The initial capacity value of buffer.</param>
        public static KingBufferWriter Create()
        {
            return Create(16, Encoding.UTF8);
        }

        /// <summary>
        /// Method responsible for create a buffer instance.
        /// </summary>
        /// <param name="encoding">The encoding value to write char array in the buffer.</param>
        public static KingBufferWriter Create(Encoding encoding)
        {
            return Create(16, encoding);
        }

        /// <summary>
        /// Method responsible for create a buffer instance.
        /// </summary>
        /// <param name="capacity">The initial capacity value of buffer.</param>
        public static KingBufferWriter Create(int initialCapacity)
        {
            return Create(initialCapacity, Encoding.UTF8);
        }

        /// <summary>
        /// Method responsible for create a buffer instance.
        /// </summary>
        /// <param name="capacity">The initial capacity value of buffer.</param>
        /// <param name="encoding">The encoding value to write char array in the buffer.</param>
        public static KingBufferWriter Create(int initialCapacity, Encoding encoding)
        {
            var writer = KingPoolManager.KingBufferWriter;

            if (writer._buffer == null || writer.Capacity != initialCapacity)
                writer._buffer = new byte[initialCapacity];

            writer.Length = 0;
            writer.Encoding = encoding;

            return writer;
        }

        #endregion

        #region public methods implementation   

        /// <inheritdoc/>
        public void Write(byte value)
        {
            EnsureCapacity(1);
            _buffer[Length++] = value;
        }

        /// <inheritdoc/>
        public void Write(char value) => Write(new char[1] { value });

        /// <inheritdoc/>
        public void Write(bool value)
        {
            EnsureCapacity(1);
            Write((byte)(value ? 1 : 0));
        }

        /// <inheritdoc/>
        public void Write(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            EnsureCapacity(8);
            Buffer.BlockCopy(bytes, 0, _buffer, Length, 8);
            Length += 8;
        }

        /// <inheritdoc/>
        public void Write(short value)
        {
            EnsureCapacity(2);
            WriteBytes(_buffer, Length, value);
            Length += 2;
        }

        /// <inheritdoc/>
        public void Write(int value)
        {
            EnsureCapacity(4);
            WriteBytes(_buffer, Length, value);
            Length += 4;
        }

        /// <inheritdoc/>
        public void Write(long value)
        {
            EnsureCapacity(8);
            WriteBytes(_buffer, Length, value);
            Length += 8;
        }

        /// <inheritdoc/>
        public void Write(sbyte value)
        {
            Write((byte)value);
        }

        /// <inheritdoc/>
        public void Write(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            EnsureCapacity(4);
            Buffer.BlockCopy(bytes, 0, _buffer, Length, 4);
            Length += 4;
        }

        /// <inheritdoc/>
        public void Write(ushort value)
        {
            EnsureCapacity(2);
            WriteBytes(_buffer, Length, value);
            Length += 2;
        }

        /// <inheritdoc/>
        public void Write(uint value)
        {
            EnsureCapacity(4);
            WriteBytes(_buffer, Length, value);
            Length += 4;
        }

        /// <inheritdoc/>
        public void Write(ulong value)
        {
            EnsureCapacity(8);
            WriteBytes(_buffer, Length, value);
            Length += 8;
        }

        /// <inheritdoc/>
        public void Write(string value) => Write(value, Encoding);

        /// <inheritdoc/>
        public void Write(string value, Encoding encoding) => Write(encoding.GetBytes(value));

        /// <inheritdoc/>
        public void Write<TPacket>(TPacket packet) where TPacket : IConvertible
        {
            if (Enum.IsDefined(typeof(TPacket), packet))
                Write((byte)(IConvertible)packet);
        }

        /// <inheritdoc/>
        public void Write(byte[] value)
        {
            EnsureCapacity(4 + value.Length);

            WriteBytes(_buffer, Length, value.Length);
            Buffer.BlockCopy(value, 0, _buffer, Length + 4, value.Length);

            Length += 4 + value.Length;
        }

        /// <inheritdoc/>
        public void Write(char[] value) => Write(value, Encoding);

        /// <inheritdoc/>
        public void Write(char[] value, Encoding encoding) => Write(encoding.GetBytes(value));

        /// <inheritdoc/>
        public void Write(bool[] value)
        {
            int num = (int)Math.Ceiling((double)value.Length / 8.0);
            EnsureCapacity(4 + num);

            WriteBytes(_buffer, Length, value.Length);

            byte b = 0;
            int num2 = 0;

            for (int i = 0; i < num; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (num2 >= value.Length)
                        break;

                    b = (byte)(b | (value[num2] ? 1 : 0));
                    b = (byte)(b << 1 | b >> 31);
                    num2++;
                }

                _buffer[Length + 4 + i] = b;
            }

            Length += 4 + num;
        }

        /// <inheritdoc/>
        public void Write(double[] value)
        {
            EnsureCapacity(4 + value.Length * 8);

            WriteBytes(_buffer, Length, value.Length);

            int num = 0;
            int num2 = Length + 4;

            while (num < value.Length)
            {
                byte[] bytes = BitConverter.GetBytes(value[num]);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(bytes);

                Buffer.BlockCopy(bytes, 0, _buffer, num2, 8);
                num++;
                num2 += 8;
            }

            Length += 4 + value.Length * 8;
        }

        /// <inheritdoc/>
        public void Write(short[] value)
        {
            EnsureCapacity(4 + value.Length * 2);

            WriteBytes(_buffer, Length, value.Length);
            int num = 0;
            int num2 = Length + 4;

            while (num < value.Length)
            {
                WriteBytes(_buffer, num2, value[num]);
                num++;
                num2 += 2;
            }

            Length += 4 + value.Length * 2;
        }

        /// <inheritdoc/>
        public void Write(int[] value)
        {
            EnsureCapacity(4 + value.Length * 4);

            WriteBytes(_buffer, Length, value.Length);

            int num = 0;
            int num2 = Length + 4;

            while (num < value.Length)
            {
                WriteBytes(_buffer, num2, value[num]);
                num++;
                num2 += 4;
            }

            Length += 4 + value.Length * 4;
        }

        /// <inheritdoc/>
        public void Write(long[] value)
        {
            EnsureCapacity(4 + value.Length * 8);

            WriteBytes(_buffer, Length, value.Length);

            int num = 0;
            int num2 = Length + 4;

            while (num < value.Length)
            {
                WriteBytes(_buffer, num2, value[num]);
                num++;
                num2 += 8;
            }

            Length += 4 + value.Length * 8;
        }

        /// <inheritdoc/>
        public void Write(sbyte[] value)
        {
            EnsureCapacity(4 + value.Length);

            WriteBytes(_buffer, Length, value.Length);
            Buffer.BlockCopy(value, 0, _buffer, Length + 4, value.Length);

            Length += 4 + value.Length;
        }

        /// <inheritdoc/>
        public void Write(float[] value)
        {
            EnsureCapacity(4 + value.Length * 4);

            WriteBytes(_buffer, Length, value.Length);

            int num = 0;
            int num2 = Length + 4;

            while (num < value.Length)
            {
                byte[] bytes = BitConverter.GetBytes(value[num]);

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(bytes);

                Buffer.BlockCopy(bytes, 0, _buffer, num2, 4);
                num++;
                num2 += 4;
            }

            Length += 4 + value.Length * 4;
        }

        /// <inheritdoc/>
        public void Write(string[] value)
        {
            EnsureCapacity(4);

            WriteBytes(_buffer, Length, value.Length);
            Length += 4;

            foreach (string value2 in value)
                Write(value2);
        }

        /// <inheritdoc/>
        public void Write(ushort[] value)
        {
            EnsureCapacity(4 + value.Length * 2);

            WriteBytes(_buffer, Length, value.Length);

            int num = 0;
            int num2 = Length + 4;

            while (num < value.Length)
            {
                WriteBytes(_buffer, num2, value[num]);
                num++;
                num2 += 2;
            }

            Length += 4 + value.Length * 2;
        }

        /// <inheritdoc/>
        public void Write(uint[] value)
        {
            EnsureCapacity(4 + value.Length * 4);

            WriteBytes(_buffer, Length, value.Length);

            int num = 0;
            int num2 = Length + 4;

            while (num < value.Length)
            {
                WriteBytes(_buffer, num2, value[num]);
                num++;
                num2 += 4;
            }

            Length += 4 + value.Length * 4;
        }

        /// <inheritdoc/>
        public void Write(ulong[] value)
        {
            EnsureCapacity(4 + value.Length * 8);

            WriteBytes(_buffer, Length, value.Length);
            int num = 0;
            int num2 = Length + 4;

            while (num < value.Length)
            {
                WriteBytes(_buffer, num2, value[num]);
                num++;
                num2 += 8;
            }

            Length += 4 + value.Length * 8;
        }

        /// <inheritdoc/>
        public void WriteRaw(byte[] bytes, int offset, int length)
        {
            EnsureCapacity(length);
            Buffer.BlockCopy(bytes, offset, _buffer, Length, length);
            Length += length;
        }

        /// <inheritdoc/>
        public void Reset()
        {
            _buffer = new byte[16];

            Length = 0;
            Encoding = Encoding.UTF8;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            KingPoolManager.DisposeKingBufferWriter(this);
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
        /// Method responsible for ensure the capacity of buffer.
        /// </summary>
        /// <param name="capacity">The capacity value of buffer.</param>
        private void EnsureCapacity(int capacity)
        {
            int num = Length + capacity;
            if (num > _buffer.Length)
            {
                int num2;
                for (num2 = _buffer.Length; num2 < num; num2 <<= 1) { }
                Array.Resize(ref _buffer, num2);
            }
        }

        /// <summary>
        /// Method responsible for write offset values in destination byte array value.
        /// </summary>
        /// <param name="destination">The destination byte array value.</param>
        /// <param name="offset">The offset value.</param>
        /// <param name="value">The ulong value to offset.</param>
        private void WriteBytes(byte[] destination, int offset, ulong value)
        {
            destination[offset] = (byte)(value >> 56);
            destination[offset + 1] = (byte)(value >> 48);
            destination[offset + 2] = (byte)(value >> 40);
            destination[offset + 3] = (byte)(value >> 32);
            destination[offset + 4] = (byte)(value >> 24);
            destination[offset + 5] = (byte)(value >> 16);
            destination[offset + 6] = (byte)(value >> 8);
            destination[offset + 7] = (byte)value;
        }

        /// <summary>
        /// Method responsible for write offset values in destination byte array value.
        /// </summary>
        /// <param name="destination">The destination byte array value.</param>
        /// <param name="offset">The offset value.</param>
        /// <param name="value">The short value to offset.</param>
        private void WriteBytes(byte[] destination, int offset, short value)
        {
            destination[offset] = (byte)(value >> 8);
            destination[offset + 1] = (byte)value;
        }

        /// <summary>
        /// Method responsible for write offset values in destination byte array value.
        /// </summary>
        /// <param name="destination">The destination byte array value.</param>
        /// <param name="offset">The offset value.</param>
        /// <param name="value">The ushort value to offset.</param>
        private void WriteBytes(byte[] destination, int offset, ushort value)
        {
            destination[offset] = (byte)(value >> 8);
            destination[offset + 1] = (byte)value;
        }

        /// <summary>
        /// Method responsible for write offset values in destination byte array value.
        /// </summary>
        /// <param name="destination">The destination byte array value.</param>
        /// <param name="offset">The offset value.</param>
        /// <param name="value">The int value to offset.</param>
        private void WriteBytes(byte[] destination, int offset, int value)
        {
            destination[offset] = (byte)(value >> 24);
            destination[offset + 1] = (byte)(value >> 16);
            destination[offset + 2] = (byte)(value >> 8);
            destination[offset + 3] = (byte)value;
        }

        /// <summary>
        /// Method responsible for write offset values in destination byte array value.
        /// </summary>
        /// <param name="destination">The destination byte array value.</param>
        /// <param name="offset">The offset value.</param>
        /// <param name="value">The uint value to offset.</param>
        private void WriteBytes(byte[] destination, int offset, uint value)
        {
            destination[offset] = (byte)(value >> 24);
            destination[offset + 1] = (byte)(value >> 16);
            destination[offset + 2] = (byte)(value >> 8);
            destination[offset + 3] = (byte)value;
        }

        /// <summary>
        /// Method responsible for write offset values in destination byte array value.
        /// </summary>
        /// <param name="destination">The destination byte array value.</param>
        /// <param name="offset">The offset value.</param>
        /// <param name="value">The long value to offset.</param>
        private void WriteBytes(byte[] destination, int offset, long value)
        {
            destination[offset] = (byte)(value >> 56);
            destination[offset + 1] = (byte)(value >> 48);
            destination[offset + 2] = (byte)(value >> 40);
            destination[offset + 3] = (byte)(value >> 32);
            destination[offset + 4] = (byte)(value >> 24);
            destination[offset + 5] = (byte)(value >> 16);
            destination[offset + 6] = (byte)(value >> 8);
            destination[offset + 7] = (byte)value;
        }

        #endregion
    }
}
