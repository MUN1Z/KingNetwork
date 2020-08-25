using System;
using System.Text;

namespace KingNetwork.Shared.Interfaces
{
    /// <summary>
    /// This interface is responsible for represents the king buffer reader interface of application.
    /// </summary>
    public interface IKingBufferReader
    {
        #region properties

        /// <summary>
        /// The encoding value to strings.
        /// </summary>
        Encoding Encoding { get; }

        /// <summary>
        /// The length value of buffer.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// The position value of buffer.
        /// </summary>
        int Position { get; }

        #endregion

        #region methods declaration

        /// <summary>
        /// Method responsible for read a byte value from buffer.
        /// </summary>
        /// <returns>The byte value from buffer.</returns>
        byte ReadByte();

        /// <summary>
        /// Method responsible for read a char value from buffer.
        /// </summary>
        /// <returns>The char value from buffer.</returns>
        char ReadChar();

        /// <summary>
        /// Method responsible for read a bool value from buffer.
        /// </summary>
        /// <returns>The bool value from buffer.</returns>
        bool ReadBoolean();

        /// <summary>
        /// Method responsible for read a double value from buffer.
        /// </summary>
        /// <returns>The double value from buffer.</returns>
        double ReadDouble();

        /// <summary>
        /// Method responsible for read a short value from buffer.
        /// </summary>
        /// <returns>The short value from buffer.</returns>
        short ReadInt16();

        /// <summary>
        /// Method responsible for read a int value from buffer.
        /// </summary>
        /// <returns>The int value from buffer.</returns>
        int ReadInt32();

        /// <summary>
        /// Method responsible for read a long value from buffer.
        /// </summary>
        /// <returns>The long value from buffer.</returns>
        long ReadInt64();

        /// <summary>
        /// Method responsible for sbyte a char value from buffer.
        /// </summary>
        /// <returns>The sbyte value from buffer.</returns>
        sbyte ReadSByte();

        /// <summary>
        /// Method responsible for read a float value from buffer.
        /// </summary>
        /// <returns>The float value from buffer.</returns>
        float ReadSingle();

        /// <summary>
        /// Method responsible for read a string value from buffer.
        /// </summary>
        /// <returns>The string value from buffer.</returns>
        string ReadString();

        /// <summary>
        /// Method responsible for read a string value from buffer.
        /// </summary>
        /// <param name="encoding">The encoding value to read string in the buffer.</param>
        /// <returns>The string value from buffer.</returns>
        string ReadString(Encoding encoding);

        /// <summary>
        /// Method responsible for read a ushort value from buffer.
        /// </summary>
        /// <returns>The ushort value from buffer.</returns>
        ushort ReadUInt16();

        /// <summary>
        /// Method responsible for read a uint value from buffer.
        /// </summary>
        /// <returns>The uint value from buffer.</returns>
        uint ReadUInt32();

        /// <summary>
        /// Method responsible for read a ulong value from buffer.
        /// </summary>
        /// <returns>The ulong value from buffer.</returns>
        ulong ReadUInt64();

        /// <summary>
        /// Method responsible for read a message packet value from buffer using generics.
        /// </summary>
        /// <returns>Returns a generic representation of message packet value from buffer.</returns>
        TPacket ReadMessagePacket<TPacket>() where TPacket : IConvertible;

        /// <summary>
        /// Method responsible for read a byte array value from buffer.
        /// </summary>
        /// <returns>The byte array value from buffer.</returns>
        byte[] ReadBytes();

        /// <summary>
        /// Method responsible for read a char array value from buffer.
        /// </summary>
        /// <returns>The char array value from buffer.</returns>
        char[] ReadChars();

        /// <summary>
        /// Method responsible for read a char array value from buffer.
        /// </summary>
        /// <param name="encoding">The encoding value to read string in the buffer.</param>
        /// <returns>The char array value from buffer.</returns>
        char[] ReadChars(Encoding encoding);

        /// <summary>
        /// Method responsible for read a bool array value from buffer.
        /// </summary>
        /// <returns>The bool array value from buffer.</returns>
        bool[] ReadBooleans();

        /// <summary>
        /// Method responsible for read a double array value from buffer.
        /// </summary>
        /// <returns>The double array value from buffer.</returns>
        double[] ReadDoubles();

        /// <summary>
        /// Method responsible for read a short array value from buffer.
        /// </summary>
        /// <returns>The short array value from buffer.</returns>
        short[] ReadInt16s();

        /// <summary>
        /// Method responsible for read a int array value from buffer.
        /// </summary>
        /// <returns>The int array value from buffer.</returns>
        int[] ReadInt32s();

        /// <summary>
        /// Method responsible for read a long array value from buffer.
        /// </summary>
        /// <returns>The long array value from buffer.</returns>
        long[] ReadInt64s();

        /// <summary>
        /// Method responsible for read a sbyte array value from buffer.
        /// </summary>
        /// <returns>The sbyte array value from buffer.</returns>
        sbyte[] ReadSBytes();

        /// <summary>
        /// Method responsible for read a float array value from buffer.
        /// </summary>
        /// <returns>The float array value from buffer.</returns>
        float[] ReadSingles();

        /// <summary>
        /// Method responsible for read a string array value from buffer.
        /// </summary>
        /// <returns>The string array value from buffer.</returns>
        string[] ReadStrings();

        /// <summary>
        /// Method responsible for read a ushort array value from buffer.
        /// </summary>
        /// <returns>The ushort array value from buffer.</returns>
        ushort[] ReadUInt16s();

        /// <summary>
        /// Method responsible for read a uint array value from buffer.
        /// </summary>
        /// <returns>The uint array value from buffer.</returns>
        uint[] ReadUInt32s();

        /// <summary>
        /// Method responsible for read a ulong array value from buffer.
        /// </summary>
        /// <returns>The ulong array value from buffer.</returns>
        ulong[] ReadUInt64s();

        /// <summary>
        /// Method responsible for read a raw data value from buffer.
        /// </summary>
        /// <param name="length">The length of raw data to read from buffer.</param>
        /// <returns>The byte array value from buffer.</returns>
        byte[] ReadRaw(int length);

        /// <summary>
        /// Method responsible for read a raw data into data buffer.
        /// </summary>
        /// <param name="buffer">The buffer array value.</param>
        /// <param name="offset">The offset of byte array data.</param>
        /// <param name="length">The length of byte array data.</param>
        void ReadRawInto(byte[] buffer, int offset, int length);

        /// <summary>
        /// Method responsible for reset the buffer.
        /// </summary>
        void Reset();

        /// <summary>
        /// Method responsible for dispose the instance.
        /// </summary>
        void Dispose();

        #endregion
    }
}
