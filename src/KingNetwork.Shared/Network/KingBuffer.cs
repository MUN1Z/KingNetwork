using System;
using System.Collections.Generic;
using System.Text;

namespace KingNetwork.Shared.Network
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
        private List<byte> _buffer;

        /// <summary>
        /// The array of bytes from read buffer.
        /// </summary>
        private byte[] _readBuffer;

        /// <summary>
        /// The read position of buffer.
        /// </summary>
        private int _readpos;

        /// <summary>
        /// The buffer updated flag.
        /// </summary>
        private bool _bufferUpdated = false;

        /// <summary>
        /// The buffer disposed flag.
        /// </summary>
        private bool _disposedValue = false;

        #endregion

        #region constructors   

        /// <summary>
        /// Creates a new instance of a <see cref="KingBuffer"/>.
        /// </summary>
        public KingBuffer()
        {
            _buffer = new List<byte>();
            _readpos = 0;
        }

        #endregion

        #region public methods implementation

        /// <summary>
        /// Method responsible for clear the buffer.
        /// </summary>
        public void Clear()
        {
            _buffer.Clear();
            _readpos = 0;
        }

        /// <summary>
        /// Method responsible for returns the count from buffer.
        /// </summary>
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
        public long GetReadPos()
        {
            return _readpos;
        }

        /// <summary>
        /// Method responsible for returns the length from buffer.
        /// </summary>
        public int Length()
        {
            return Count() - _readpos;
        }

        /// <summary>
        /// Method responsible for returns the array of bytes from buffer.
        /// </summary>
        public byte[] ToArray()
        {
            return _buffer.ToArray();
        }
        
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                    _buffer.Clear();

                _readpos = 0;
            }
            _disposedValue = true;
        }
        
        #endregion

        #region serializers and deserializers implementation

        public static byte[] Serializer<TMessage>(TMessage message) where TMessage : KingMessage
        {
            message.Serialize();
            return message.KingBuffer.ToArray();
        }

        public static TMessage DeSerializer<TMessage>(byte[] bytes) where TMessage : KingMessage, new()
        {
            var message = new TMessage();
            message.DeSerialize(bytes);

            return message;
        }

        #endregion

        #region readers methods implementation 

        public byte[] ReadBytes(int Length, bool Peek = true)
        {
            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            byte[] array = _buffer.GetRange(_readpos, Length).ToArray();

            if (Peek)
                _readpos += Length;

            return array;
        }

        public float ReadFloat(bool peek = true)
        {
            if (_buffer.Count <= _readpos)
                throw new Exception("Byte Buffer Past Limit!");

            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            float single = BitConverter.ToSingle(_readBuffer, _readpos);
            if (peek & _buffer.Count > _readpos)
                _readpos += 4;

            return single;
        }

        public int ReadMessagePacketType()
        {
            return ReadInteger(true);
        }

        public int ReadInteger(bool peek = true)
        {
            if (_buffer.Count <= _readpos)
                throw new Exception("Byte Buffer Past Limit!");

            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            var value = BitConverter.ToInt32(_readBuffer, _readpos);
            if (peek & _buffer.Count > _readpos)
                _readpos += 4;

            return value;
        }

        public bool ReadBoolean(bool peek = true)
        {
            if (_buffer.Count <= _readpos)
                throw new Exception("Byte Buffer Past Limit!");

            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            var value = BitConverter.ToBoolean(_readBuffer, _readpos);
            if (peek & _buffer.Count > _readpos)
                _readpos += 4;

            return value;
        }

        public long ReadLong(bool peek = true)
        {
            if (_buffer.Count < _readpos)
                throw new Exception("Byte Buffer Past Limit!");

            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            var value = BitConverter.ToInt64(_readBuffer, _readpos);
            if (peek & _buffer.Count > _readpos)
                _readpos += 8;

            return value;
        }

        public short ReadShort(bool peek = true)
        {
            if (_buffer.Count <= _readpos)
                throw new Exception("Byte Buffer Past Limit!");

            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            var value = BitConverter.ToInt16(_readBuffer, _readpos);
            if (peek & _buffer.Count > _readpos)
                _readpos += 2;

            return value;
        }

        public string ReadString(bool Peek = true)
        {
            int count = ReadInteger(true);
            if (_bufferUpdated)
            {
                _readBuffer = _buffer.ToArray();
                _bufferUpdated = false;
            }

            var value = Encoding.ASCII.GetString(_readBuffer, _readpos, count);
            if (Peek & _buffer.Count > _readpos)
                if (value.Length > 0)
                    _readpos += count;

            return value;
        }

        #endregion

        #region writers methods implementation

        public void WriteBytes(byte[] data)
        {
            _buffer.AddRange(data);
            _bufferUpdated = true;
        }

        public void WriteFloat(float value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
            _bufferUpdated = true;
        }

        public void WriteMessagePacketType(int messagePacketType)
        {
            WriteInteger(messagePacketType);
        }

        public void WriteInteger(int value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
            _bufferUpdated = true;
        }

        public void WriteBoolean(bool value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
            _bufferUpdated = true;
        }

        public void WriteLong(long value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
            _bufferUpdated = true;
        }

        public void WriteShort(short value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value));
            _bufferUpdated = true;
        }

        public void WriteString(string value)
        {
            _buffer.AddRange(BitConverter.GetBytes(value.Length));
            _buffer.AddRange(Encoding.ASCII.GetBytes(value));
            _bufferUpdated = true;
        }

        #endregion
    }
}
