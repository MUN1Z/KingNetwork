using System;
using System.Collections.Generic;
using System.Text;

namespace KingNetwork.Shared
{

    /// <summary>
    /// This class is responsible for represents the buffer of application.
    /// </summary>
    public class KingBuffer : IDisposable
    {
        #region private members 	

        /// <summary>
        /// The list of bytes from buffer.
        /// </summary>
        private readonly List<byte> _buffer;

        /// <summary>
        /// The array of bytes from read buffer.
        /// </summary>
        private byte[] _readBuffer;

        /// <summary>
        /// The read position of buffer.
        /// </summary>
        private int _readPos;

        /// <summary>
        /// The buffer updated flag.
        /// </summary>
        private bool _bufferUpdated;

        /// <summary>
        /// The buffer disposed flag.
        /// </summary>
        private bool _disposedValue;

        #endregion

        #region constructors   

        /// <summary>
        /// Creates a new instance of a <see cref="KingBuffer"/>.
        /// </summary>
        public KingBuffer()
        {
            _buffer = new List<byte>();
            _readPos = 0;
        }

        /// <summary>
        /// Creates a new instance of a <see cref="KingBuffer"/>.
        /// </summary>
        /// <param name="data">The byte array from data received in message.</param>
        public KingBuffer(IEnumerable<byte> data)
        {
            _buffer = new List<byte>(data);
            _readPos = 0;
        }

        #endregion

        #region public methods implementation

        /// <summary>
        /// Method responsible for clear the buffer.
        /// </summary>
        public void Clear()
        {
            _buffer.Clear();
            _readPos = 0;
        }

        /// <summary>
        /// Method responsible for returns the count from buffer.
        /// </summary>
        /// <returns>Returns a count of buffer.</returns>
        public int Count()
        {
            return _buffer.Count;
        }

        /// <summary>
        /// Method responsible for dispose the instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Method responsible for returns the read position from buffer.
        /// </summary>
        /// <returns>Returns a read position.</returns>
        public long GetReadPos()
        {
            return _readPos;
        }

        /// <summary>
        /// Method responsible for returns the length from buffer.
        /// </summary>
        /// <returns>Returns a length of buffer.</returns>
        public int Length()
        {
            return Count() - _readPos;
        }

        /// <summary>
        /// Method responsible for returns the array of bytes from buffer.
        /// </summary>
        /// <returns>Returns a byte array from buffer.</returns>
        public byte[] ToArray()
        {
            return _buffer.ToArray();
        }

        #endregion

        #region protected methods implementation

        /// <summary>
        /// Method responsible for dispose the instance.
        /// </summary>
        /// <param name="disposing">The flag for dispose object.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                    _buffer.Clear();

                _readPos = 0;
            }
            _disposedValue = true;
        }

        #endregion

        #region readers methods implementation 

        /// <summary>
        /// Method responsible for read a bytes array from buffer.
        /// </summary>
        /// <param name="length">The length of bytes.</param>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a byte array value from buffer.</returns>
        public byte[] ReadBytes(int length, bool peek = true)
        {
            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            var array = _buffer.GetRange(_readPos, length).ToArray();

            if (peek)
                _readPos += length;

            return array;
        }

        /// <summary>
        /// Method responsible for read a float value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a float value from buffer.</returns>
        public float ReadFloat(bool peek = true)
        {
            if (_buffer.Count <= _readPos)
                throw new Exception("Byte Buffer Past Limit!");

            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            var single = BitConverter.ToSingle(_readBuffer, _readPos);
            if (peek & _buffer.Count > _readPos)
                _readPos += 4;

            return single;
        }
        
        /// <summary>
        /// Method responsible for read a integer value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a integer value from buffer.</returns>
        public int ReadInteger(bool peek = true)
        {
            if (_buffer.Count <= _readPos)
                throw new Exception("Byte Buffer Past Limit!");

            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            var value = BitConverter.ToInt32(_readBuffer, _readPos);
            if (peek & _buffer.Count > _readPos)
                _readPos += 4;

            return value;
        }

        /// <summary>
        /// Method responsible for read a boolean value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a boolean value from buffer.</returns>
        public bool ReadBoolean(bool peek = true)
        {
            if (_buffer.Count <= _readPos)
                throw new Exception("Byte Buffer Past Limit!");

            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            var value = BitConverter.ToBoolean(_readBuffer, _readPos);
            if (peek & _buffer.Count > _readPos)
                _readPos += 4;

            return value;
        }

        /// <summary>
        /// Method responsible for read a long value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a long value from buffer.</returns>
        public long ReadLong(bool peek = true)
        {
            if (_buffer.Count < _readPos)
                throw new Exception("Byte Buffer Past Limit!");

            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            var value = BitConverter.ToInt64(_readBuffer, _readPos);
            if (peek & _buffer.Count > _readPos)
                _readPos += 8;

            return value;
        }

        /// <summary>
        /// Method responsible for read a short value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a short value from buffer.</returns>
        public short ReadShort(bool peek = true)
        {
            if (_buffer.Count <= _readPos)
                throw new Exception("Byte Buffer Past Limit!");

            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            var value = BitConverter.ToInt16(_readBuffer, _readPos);
            if (peek & _buffer.Count > _readPos)
                _readPos += 2;

            return value;
        }

        /// <summary>
        /// Method responsible for read a string value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a string value from buffer.</returns>
        public string ReadString(bool peek = true)
        {
            var count = ReadInteger();
            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            var value = Encoding.ASCII.GetString(_readBuffer, _readPos, count);
            if (peek & _buffer.Count > _readPos)
                if (value.Length > 0)
                    _readPos += count;

            return value;
        }

        /// <summary>
        /// Method responsible for read a message packet value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a byte representation of message packet value from buffer.</returns>
        public byte ReadMessagePacket(bool peek = true)
        {
            if (_readPos == 0)
                _readPos += 1;

            return _buffer[0];
        }

        /// <summary>
        /// Method responsible for read a message packet value from buffer using generics.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a generic representation of message packet value from buffer.</returns>
        public TPacket ReadMessagePacket<TPacket>(bool peek = true) where TPacket : IConvertible
        {
            return (TPacket)(IConvertible)ReadMessagePacket();
        }

        #endregion

        #region writers methods implementation

        /// <summary>
        /// Method responsible for write a byte array data value in the buffer.
        /// </summary>
        /// <param name="data">The bytes array data value to write in the buffer.</param>
        public void WriteBytes(byte[] data)
        {
            _buffer.AddRange(data);
            _bufferUpdated = true;
        }

        /// <summary>
        /// Method responsible for write a float value in the buffer.
        /// </summary>
        /// <param name="value">The float value to write in the buffer.</param>
        public void WriteFloat(float value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
            _bufferUpdated = true;
        }

        /// <summary>
        /// Method responsible for write a integer value in the buffer.
        /// </summary>
        /// <param name="value">The integer value to write in the buffer.</param>
        public void WriteInteger(int value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
            _bufferUpdated = true;
        }

        /// <summary>
        /// Method responsible for write a boolean value in the buffer.
        /// </summary>
        /// <param name="value">The boolean value to write in the buffer.</param>
        public void WriteBoolean(bool value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
            _bufferUpdated = true;
        }

        /// <summary>
        /// Method responsible for write a long value in the buffer.
        /// </summary>
        /// <param name="value">The long value to write in the buffer.</param>
        public void WriteLong(long value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
            _bufferUpdated = true;
        }

        /// <summary>
        /// Method responsible for write a short value in the buffer.
        /// </summary>
        /// <param name="value">The short value to write in the buffer.</param>
        public void WriteShort(short value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
            _bufferUpdated = true;
        }

        /// <summary>
        /// Method responsible for write a string value in the buffer.
        /// </summary>
        /// <param name="value">The string value to write in the buffer.</param>
        public void WriteString(string value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value.Length));
            _buffer.AddRange(Encoding.ASCII.GetBytes(value));
            _bufferUpdated = true;
        }

        /// <summary>
        /// Method responsible for write a message packet value in the buffer.
        /// </summary>
        /// <param name="messagePacket">The message packet value to write in the buffer.</param>
        public void WriteMessagePacket(byte messagePacket)
        {
            _buffer.Add(messagePacket);
            _bufferUpdated = true;
        }

        /// <summary>
        /// Method responsible for write a generic message packet value in the buffer.
        /// </summary>
        /// <param name="messagePacket">The generic message packet value to write in the buffer.</param>
        public void WriteMessagePacket<TPacket>(TPacket messagePacket) where TPacket : IConvertible
        {
            if (Enum.IsDefined(typeof(TPacket), messagePacket))
                WriteMessagePacket((byte)(IConvertible)messagePacket);
        }

        #endregion
    }
}
