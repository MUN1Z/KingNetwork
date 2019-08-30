using System;
using System.Text;

namespace KingNetwork.Shared
{
    /// <summary>
    /// This interface is responsible for represents the king buffer writer interface of application.
    /// </summary>
    public interface IKingBufferWriter
    {
        #region methods declaration

        /// <summary>
        /// Method responsible for write a byte value in the buffer.
        /// </summary>
        /// <param name="value">The byte value to write in the buffer.</param>
        void Write(byte value);

        /// <summary>
        /// Method responsible for write a char value in the buffer.
        /// </summary>
        /// <param name="value">The char value to write in the buffer.</param>
        void Write(char value);

        /// <summary>
        /// Method responsible for write a bool value in the buffer.
        /// </summary>
        /// <param name="value">The bool value to write in the buffer.</param>
        void Write(bool value);

        /// <summary>
        /// Method responsible for write a double value in the buffer.
        /// </summary>
        /// <param name="value">The double value to write in the buffer.</param>
        void Write(double value);

        /// <summary>
        /// Method responsible for write a short value in the buffer.
        /// </summary>
        /// <param name="value">The short value to write in the buffer.</param>
        void Write(short value);

        /// <summary>
        /// Method responsible for write a int value in the buffer.
        /// </summary>
        /// <param name="value">The int value to write in the buffer.</param>
        void Write(int value);

        /// <summary>
        /// Method responsible for write a long value in the buffer.
        /// </summary>
        /// <param name="value">The long value to write in the buffer.</param>
        void Write(long value);

        /// <summary>
        /// Method responsible for write a sbyte value in the buffer.
        /// </summary>
        /// <param name="value">The sbyte value to write in the buffer.</param>
        void Write(sbyte value);

        /// <summary>
        /// Method responsible for write a float value in the buffer.
        /// </summary>
        /// <param name="value">The float value to write in the buffer.</param>
        void Write(float value);

        /// <summary>
        /// Method responsible for write a ushort value in the buffer.
        /// </summary>
        /// <param name="value">The ushort value to write in the buffer.</param>
        void Write(ushort value);

        /// <summary>
        /// Method responsible for write a uint value in the buffer.
        /// </summary>
        /// <param name="value">The uint value to write in the buffer.</param>
        void Write(uint value);

        /// <summary>
        /// Method responsible for write a ulong value in the buffer.
        /// </summary>
        /// <param name="value">The ulong value to write in the buffer.</param>
        void Write(ulong value);

        /// <summary>
        /// Method responsible for write a string value in the buffer.
        /// </summary>
        /// <param name="value">The string value to write in the buffer.</param>
        void Write(string value);

        /// <summary>
        /// Method responsible for write a byte value in the buffer.
        /// </summary>
        /// <param name="value">The byte value to write in the buffer.</param>
        /// <param name="encoding">The encoding value to write char array in the buffer.</param>
        void Write(string value, Encoding encoding);

        /// <summary>
        /// Method responsible for write a generic packet value in the buffer.
        /// </summary>
        /// <param name="packet">The generic packet value to write in the buffer.</param>
        void Write<TPacket>(TPacket packet) where TPacket : IConvertible;

        /// <summary>
        /// Method responsible for write a byte array values in the buffer.
        /// </summary>
        /// <param name="value">The byte array values to write in the buffer.</param>
        void Write(byte[] value);

        /// <summary>
        /// Method responsible for write a char array values in the buffer.
        /// </summary>
        /// <param name="value">The char array values to write in the buffer.</param>
        void Write(char[] value);

        /// <summary>
        /// Method responsible for write a char array values in the buffer.
        /// </summary>
        /// <param name="value">The char array values to write in the buffer.</param>
        /// <param name="encoding">The encoding value to write char array in the buffer.</param>
        void Write(char[] value, Encoding encoding);

        /// <summary>
        /// Method responsible for write a bool array values in the buffer.
        /// </summary>
        /// <param name="value">The bool array values to write in the buffer.</param>
        void Write(bool[] value);

        /// <summary>
        /// Method responsible for write a double array values in the buffer.
        /// </summary>
        /// <param name="value">The double array values to write in the buffer.</param>
        void Write(double[] value);

        /// <summary>
        /// Method responsible for write a short array values in the buffer.
        /// </summary>
        /// <param name="value">The short array values to write in the buffer.</param>
        void Write(short[] value);

        /// <summary>
        /// Method responsible for write a int array values in the buffer.
        /// </summary>
        /// <param name="value">The int array values to write in the buffer.</param>
        void Write(int[] value);

        /// <summary>
        /// Method responsible for write a long array values in the buffer.
        /// </summary>
        /// <param name="value">The long array values to write in the buffer.</param>
        void Write(long[] value);

        /// <summary>
        /// Method responsible for write a sbyte array values in the buffer.
        /// </summary>
        /// <param name="value">The sbyte array values to write in the buffer.</param>
        void Write(sbyte[] value);

        /// <summary>
        /// Method responsible for write a float array values in the buffer.
        /// </summary>
        /// <param name="value">The float array value to write in the buffer.</param>
        void Write(float[] value);

        /// <summary>
        /// Method responsible for write a string array values in the buffer.
        /// </summary>
        /// <param name="value">The string array values to write in the buffer.</param>
        void Write(string[] value);

        /// <summary>
        /// Method responsible for write a ushort array values in the buffer.
        /// </summary>
        /// <param name="value">The ushort array values to write in the buffer.</param>
        void Write(ushort[] value);

        /// <summary>
        /// Method responsible for write a uint array value in the buffer.
        /// </summary>
        /// <param name="value">The uint array values to write in the buffer.</param>
        void Write(uint[] value);

        /// <summary>
        /// Method responsible for write a ulong array values in the buffer.
        /// </summary>
        /// <param name="value">The ulong array values to write in the buffer.</param>
        void Write(ulong[] value);

        /// <summary>
        /// Method responsible for write a byte array raw values in the buffer.
        /// </summary>
        /// <param name="value">The byte array raw values to write in the buffer.</param>
        /// <param name="offset">The offset value to write raw in the buffer.</param>
        /// <param name="length">The length value to write raw in the buffer.</param>
        void WriteRaw(byte[] bytes, int offset, int length);

        /// <summary>
        /// Method responsible for reset the buffer.
        /// </summary>
        void Reset();

        #endregion
    }
}
