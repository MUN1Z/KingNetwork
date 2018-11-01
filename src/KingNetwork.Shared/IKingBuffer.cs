using System;

namespace KingNetwork.Shared
{
    /// <summary>
    /// This interface is responsible for represents the buffer of application.
    /// </summary>
    public interface  IKingBuffer
    {
        #region public methods implementation

        /// <summary>
        /// Method responsible for clear the buffer.
        /// </summary>
        void Clear();

        /// <summary>
        /// Method responsible for returns the count from buffer.
        /// </summary>
        /// <returns>Returns a count of buffer.</returns>
        int Count();

        /// <summary>
        /// Method responsible for returns the read position from buffer.
        /// </summary>
        /// <returns>Returns a read position.</returns>
        long GetReadPos();

        /// <summary>
        /// Method responsible for returns the length from buffer.
        /// </summary>
        /// <returns>Returns a length of buffer.</returns>
        int Length();

        /// <summary>
        /// Method responsible for returns the array of bytes from buffer.
        /// </summary>
        /// <returns>Returns a byte array from buffer.</returns>
        byte[] ToArray();

        #endregion

        #region readers methods implementation 

        /// <summary>
        /// Method responsible for read a bytes array from buffer.
        /// </summary>
        /// <param name="length">The length of bytes.</param>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a byte array value from buffer.</returns>
        byte[] ReadBytes(int length, bool peek = true);

        /// <summary>
        /// Method responsible for read a float value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a float value from buffer.</returns>
        float ReadFloat(bool peek = true);

        /// <summary>
        /// Method responsible for read a integer value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a integer value from buffer.</returns>
        int ReadInteger(bool peek = true);

        /// <summary>
        /// Method responsible for read a boolean value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a boolean value from buffer.</returns>
        bool ReadBoolean(bool peek = true);

        /// <summary>
        /// Method responsible for read a long value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a long value from buffer.</returns>
        long ReadLong(bool peek = true);

        /// <summary>
        /// Method responsible for read a short value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a short value from buffer.</returns>
        short ReadShort(bool peek = true);

        /// <summary>
        /// Method responsible for read a string value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a string value from buffer.</returns>
        string ReadString(bool peek = true);

        /// <summary>
        /// Method responsible for read a message packet value from buffer.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a byte representation of message packet value from buffer.</returns>
        byte ReadMessagePacket(bool peek = true);

        /// <summary>
        /// Method responsible for read a message packet value from buffer using generics.
        /// </summary>
        /// <param name="peek">The flag for peek position.</param>
        /// <returns>Returns a generic representation of message packet value from buffer.</returns>
        TPacket ReadMessagePacket<TPacket>(bool peek = true) where TPacket : IConvertible;

        #endregion

        #region writers methods implementation

        /// <summary>
        /// Method responsible for write a byte array data value in the buffer.
        /// </summary>
        /// <param name="data">The bytes array data value to write in the buffer.</param>
        void WriteBytes(byte[] data);

        /// <summary>
        /// Method responsible for write a float value in the buffer.
        /// </summary>
        /// <param name="value">The float value to write in the buffer.</param>
        void WriteFloat(float value);

        /// <summary>
        /// Method responsible for write a integer value in the buffer.
        /// </summary>
        /// <param name="value">The integer value to write in the buffer.</param>
        void WriteInteger(int value);

        /// <summary>
        /// Method responsible for write a boolean value in the buffer.
        /// </summary>
        /// <param name="value">The boolean value to write in the buffer.</param>
        void WriteBoolean(bool value);

        /// <summary>
        /// Method responsible for write a long value in the buffer.
        /// </summary>
        /// <param name="value">The long value to write in the buffer.</param>
        void WriteLong(long value);

        /// <summary>
        /// Method responsible for write a short value in the buffer.
        /// </summary>
        /// <param name="value">The short value to write in the buffer.</param>
        void WriteShort(short value);

        /// <summary>
        /// Method responsible for write a string value in the buffer.
        /// </summary>
        /// <param name="value">The string value to write in the buffer.</param>
        void WriteString(string value);

        /// <summary>
        /// Method responsible for write a message packet value in the buffer.
        /// </summary>
        /// <param name="messagePacket">The message packet value to write in the buffer.</param>
        void WriteMessagePacket(byte messagePacket);

        /// <summary>
        /// Method responsible for write a generic message packet value in the buffer.
        /// </summary>
        /// <param name="messagePacket">The generic message packet value to write in the buffer.</param>
        void WriteMessagePacket<TPacket>(TPacket messagePacket) where TPacket : IConvertible;

        #endregion
    }
}
