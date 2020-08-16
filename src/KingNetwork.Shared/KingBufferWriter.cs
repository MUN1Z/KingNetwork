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

        /// <summary>
        /// The encoding value to strings.
        /// </summary>
        public Encoding Encoding { get; set; }

        /// <summary>
        /// The byte array buffer.
        /// </summary>
        public byte[] BufferData => _buffer;

        /// <summary>
        /// The length value of buffer.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The capacity value of buffer.
        /// </summary>
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
            var writer = KingPoolManager.GetInstance().KingBufferWriter;

            if (writer._buffer == null || writer.Capacity != initialCapacity)
                writer._buffer = new byte[initialCapacity];

            writer.Length = 0;
            writer.Encoding = encoding;

            return writer;
        }

        #endregion

        #region public methods implementation   

        /// <summary>
        /// Method responsible for write a byte value in the buffer.
        /// </summary>
        /// <param name="value">The byte value to write in the buffer.</param>
        public void Write(byte value)
        {
            EnsureCapacity(1);
            _buffer[Length++] = value;
        }

        /// <summary>
        /// Method responsible for write a char value in the buffer.
        /// </summary>
        /// <param name="value">The char value to write in the buffer.</param>
        public void Write(char value) => Write(new char[1] { value });

        /// <summary>
        /// Method responsible for write a bool value in the buffer.
        /// </summary>
        /// <param name="value">The bool value to write in the buffer.</param>
        public void Write(bool value)
        {
            EnsureCapacity(1);
            Write((byte)(value ? 1 : 0));
        }

        /// <summary>
        /// Method responsible for write a double value in the buffer.
        /// </summary>
        /// <param name="value">The double value to write in the buffer.</param>
        public void Write(double value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            EnsureCapacity(8);
            Buffer.BlockCopy(bytes, 0, _buffer, Length, 8);
            Length += 8;
        }

        /// <summary>
        /// Method responsible for write a short value in the buffer.
        /// </summary>
        /// <param name="value">The short value to write in the buffer.</param>
        public void Write(short value)
        {
            EnsureCapacity(2);
            WriteBytes(_buffer, Length, value);
            Length += 2;
        }

        /// <summary>
        /// Method responsible for write a int value in the buffer.
        /// </summary>
        /// <param name="value">The int value to write in the buffer.</param>
        public void Write(int value)
        {
            EnsureCapacity(4);
            WriteBytes(_buffer, Length, value);
            Length += 4;
        }

        /// <summary>
        /// Method responsible for write a long value in the buffer.
        /// </summary>
        /// <param name="value">The long value to write in the buffer.</param>
        public void Write(long value)
        {
            EnsureCapacity(8);
            WriteBytes(_buffer, Length, value);
            Length += 8;
        }

        /// <summary>
        /// Method responsible for write a sbyte value in the buffer.
        /// </summary>
        /// <param name="value">The sbyte value to write in the buffer.</param>
        public void Write(sbyte value)
        {
            Write((byte)value);
        }

        /// <summary>
        /// Method responsible for write a float value in the buffer.
        /// </summary>
        /// <param name="value">The float value to write in the buffer.</param>
        public void Write(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            EnsureCapacity(4);
            Buffer.BlockCopy(bytes, 0, _buffer, Length, 4);
            Length += 4;
        }

        /// <summary>
        /// Method responsible for write a ushort value in the buffer.
        /// </summary>
        /// <param name="value">The ushort value to write in the buffer.</param>
        public void Write(ushort value)
        {
            EnsureCapacity(2);
            WriteBytes(_buffer, Length, value);
            Length += 2;
        }

        /// <summary>
        /// Method responsible for write a uint value in the buffer.
        /// </summary>
        /// <param name="value">The uint value to write in the buffer.</param>
        public void Write(uint value)
        {
            EnsureCapacity(4);
            WriteBytes(_buffer, Length, value);
            Length += 4;
        }

        /// <summary>
        /// Method responsible for write a ulong value in the buffer.
        /// </summary>
        /// <param name="value">The ulong value to write in the buffer.</param>
        public void Write(ulong value)
        {
            EnsureCapacity(8);
            WriteBytes(_buffer, Length, value);
            Length += 8;
        }

        /// <summary>
        /// Method responsible for write a string value in the buffer.
        /// </summary>
        /// <param name="value">The string value to write in the buffer.</param>
        public void Write(string value) => Write(value, Encoding);

        /// <summary>
        /// Method responsible for write a byte value in the buffer.
        /// </summary>
        /// <param name="value">The byte value to write in the buffer.</param>
        /// <param name="encoding">The encoding value to write char array in the buffer.</param>
        public void Write(string value, Encoding encoding) => Write(encoding.GetBytes(value));

        /// <summary>
        /// Method responsible for write a generic packet value in the buffer.
        /// </summary>
        /// <param name="packet">The generic packet value to write in the buffer.</param>
        public void Write<TPacket>(TPacket packet) where TPacket : IConvertible
        {
            if (Enum.IsDefined(typeof(TPacket), packet))
                Write((byte)(IConvertible)packet);
        }

        /// <summary>
        /// Method responsible for write a byte array values in the buffer.
        /// </summary>
        /// <param name="value">The byte array values to write in the buffer.</param>
        public void Write(byte[] value)
        {
            EnsureCapacity(4 + value.Length);

            WriteBytes(_buffer, Length, value.Length);
            Buffer.BlockCopy(value, 0, _buffer, Length + 4, value.Length);

            Length += 4 + value.Length;
        }

        /// <summary>
        /// Method responsible for write a char array values in the buffer.
        /// </summary>
        /// <param name="value">The char array values to write in the buffer.</param>
        public void Write(char[] value) => Write(value, Encoding);

        /// <summary>
        /// Method responsible for write a char array values in the buffer.
        /// </summary>
        /// <param name="value">The char array values to write in the buffer.</param>
        /// <param name="encoding">The encoding value to write char array in the buffer.</param>
        public void Write(char[] value, Encoding encoding) => Write(encoding.GetBytes(value));

        /// <summary>
        /// Method responsible for write a bool array values in the buffer.
        /// </summary>
        /// <param name="value">The bool array values to write in the buffer.</param>
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

        /// <summary>
        /// Method responsible for write a double array values in the buffer.
        /// </summary>
        /// <param name="value">The double array values to write in the buffer.</param>
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

        /// <summary>
        /// Method responsible for write a short array values in the buffer.
        /// </summary>
        /// <param name="value">The short array values to write in the buffer.</param>
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

        /// <summary>
        /// Method responsible for write a int array values in the buffer.
        /// </summary>
        /// <param name="value">The int array values to write in the buffer.</param>
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

        /// <summary>
        /// Method responsible for write a long array values in the buffer.
        /// </summary>
        /// <param name="value">The long array values to write in the buffer.</param>
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

        /// <summary>
        /// Method responsible for write a sbyte array values in the buffer.
        /// </summary>
        /// <param name="value">The sbyte array values to write in the buffer.</param>
        public void Write(sbyte[] value)
        {
            EnsureCapacity(4 + value.Length);

            WriteBytes(_buffer, Length, value.Length);
            Buffer.BlockCopy(value, 0, _buffer, Length + 4, value.Length);

            Length += 4 + value.Length;
        }

        /// <summary>
        /// Method responsible for write a float array values in the buffer.
        /// </summary>
        /// <param name="value">The float array value to write in the buffer.</param>
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

        /// <summary>
        /// Method responsible for write a string array values in the buffer.
        /// </summary>
        /// <param name="value">The string array values to write in the buffer.</param>
        public void Write(string[] value)
        {
            EnsureCapacity(4);

            WriteBytes(_buffer, Length, value.Length);
            Length += 4;

            foreach (string value2 in value)
                Write(value2);
        }

        /// <summary>
        /// Method responsible for write a ushort array values in the buffer.
        /// </summary>
        /// <param name="value">The ushort array values to write in the buffer.</param>
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

        /// <summary>
        /// Method responsible for write a uint array value in the buffer.
        /// </summary>
        /// <param name="value">The uint array values to write in the buffer.</param>
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

        /// <summary>
        /// Method responsible for write a ulong array values in the buffer.
        /// </summary>
        /// <param name="value">The ulong array values to write in the buffer.</param>
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

        /// <summary>
        /// Method responsible for write a byte array raw values in the buffer.
        /// </summary>
        /// <param name="value">The byte array raw values to write in the buffer.</param>
        /// <param name="offset">The offset value to write raw in the buffer.</param>
        /// <param name="length">The length value to write raw in the buffer.</param>
        public void WriteRaw(byte[] bytes, int offset, int length)
        {
            EnsureCapacity(length);
            Buffer.BlockCopy(bytes, offset, _buffer, Length, length);
            Length += length;
        }

        /// <summary>
        /// Method responsible for reset the buffer.
        /// </summary>
        public void Reset()
        {
            _buffer = new byte[16];

            Length = 0;
            Encoding = Encoding.UTF8;
        }

        /// <summary>
        /// Method responsible for dispose the instance.
        /// </summary>
        public void Dispose()
        {
            KingPoolManager.GetInstance().DisposeKingBufferWriter(this);
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
