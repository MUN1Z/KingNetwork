using KingNetwork.Shared.Network;
using System;

namespace KingNetwork.Shared
{
    /// <summary>
    /// This class is responsible for represents the message that will be received and sent by the server and client.
    /// </summary>
    public class KingMessage : IDisposable
    {
        #region properties

        /// <summary>
        /// The type of packet from message.
        /// </summary>
        public int MessagePacketType { get; private set; }

        /// <summary>
        /// The king buffer instance.
        /// </summary>
        public KingBuffer KingBuffer { get; set; }

        #endregion

        #region constructors

        /// <summary>
        /// Creates a new instance of a <see cref="KingMessage"/>.
        /// </summary>
        /// <param name="messagePacketType">The type of packet from message.</param>
        public KingMessage(int messagePacketType)
        {
            MessagePacketType = messagePacketType;
            KingBuffer = new KingBuffer();
        }

        #endregion

        #region internal methods implementation

        /// <summary>
        /// Method responsible for serialize the message.
        /// </summary>
        internal virtual void Serialize()
        {
            KingBuffer.WriteMessagePacketType(MessagePacketType);
        }

        /// <summary>
        /// Method responsible for deserialize the message.
        /// </summary>
        internal virtual void DeSerialize(byte[] data)
        {
            KingBuffer.WriteBytes(data);
            MessagePacketType = KingBuffer.ReadMessagePacketType();
        }

        #endregion

        #region public methods implementation

        /// <summary>
        /// Method responsible for dispose the message.
        /// </summary>
        public void Dispose()
        {
            KingBuffer.Dispose();
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
