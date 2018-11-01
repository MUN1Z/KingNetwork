using KingNetwork.Shared;
using System.Collections.Generic;

namespace KingNetwork.Unity
{
    /// <summary>
    /// This class is responsible for represents the buffer of application.
    /// </summary>
    public class KingBuffer : KingBufferBase
    {
        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="KingBuffer"/>.
        /// </summary>
        public KingBuffer() : base() { }

        /// <summary>
        /// Creates a new instance of a <see cref="KingBuffer"/>.
        /// </summary>
        /// <param name="data">The byte array from data received in message.</param>
        public KingBuffer(IEnumerable<byte> data) : base(data) { }

        #endregion
    }
}
