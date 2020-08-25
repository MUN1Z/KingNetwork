using System;
using System.Net.Sockets;
using System.Threading;

namespace KingNetwork.Shared
{
    /// <summary>
    /// This class is responsible for represents the king pool  manager of application.
    /// </summary>
    public static class KingPoolManager
    {
        /// <summary>
        /// The initialized value flag.
        /// </summary>
        [ThreadStatic]
        private static bool initialized;

        /// <summary>
        /// The king buffer writer objects pool instance..
        /// </summary>
        [ThreadStatic]
        private static KingPool<KingBufferWriter> _kingBufferWriterPool;

        /// <summary>
        /// The king buffer reader objects pool instance..
        /// </summary>
        [ThreadStatic]
        private static KingPool<KingBufferReader> _kingBufferReaderPool;

        /// <summary>
        /// The socket async event args objects pool instance..
        /// </summary>
        [ThreadStatic]
        private static KingPool<SocketAsyncEventArgs> _socketAsyncEventArgsPool;

        /// <summary>
        /// The initialize object lock.
        /// </summary>
        private static readonly object initializeLock = new object();

        /// <summary>
        /// This method is responsible for initialize the thread instance of objects.
        /// </summary>
        private static void ThreadInitialize()
        {
            var obj = initializeLock;

            Monitor.Enter(obj);

            try
            {
                _kingBufferWriterPool = new KingPool<KingBufferWriter>(2, () => new KingBufferWriter());
                _kingBufferReaderPool = new KingPool<KingBufferReader>(2, () => new KingBufferReader());
                _socketAsyncEventArgsPool = new KingPool<SocketAsyncEventArgs>(32, () => new SocketAsyncEventArgs());
            }
            finally
            {
                Monitor.Exit(obj);
            }

            initialized = true;
        }

        /// <summary>
        /// The king buffer writer object instance.
        /// </summary>
        public static KingBufferWriter KingBufferWriter
        {
            get
            {
                if (!initialized)
                    ThreadInitialize();

                return _kingBufferWriterPool.GetInstance();
            }
        }
        
        /// <summary>
        /// The king buffer reader object instance.
        /// </summary>
        public static KingBufferReader KingBufferReader
        {
            get
            {
                if (!initialized)
                    ThreadInitialize();

                return _kingBufferReaderPool.GetInstance();
            }
        }

        /// <summary>
        /// The socket async event args object instance.
        /// </summary>
        public static SocketAsyncEventArgs SocketAsyncEventArgs
        {
            get
            {
                if (!initialized)
                    ThreadInitialize();

                return _socketAsyncEventArgsPool.GetInstance();
            }
        }

        /// <summary>
        /// This method is reponsible for dispose the king buffer writer object instance.
        /// </summary>
        /// <param name="kingBufferWriter">The king buffer writer object instance</param>
        public static void DisposeKingBufferWriter(KingBufferWriter kingBufferWriter)
        {
            if (!initialized)
                ThreadInitialize();

            _kingBufferWriterPool.ReturnInstance(kingBufferWriter);
        }

        /// <summary>
        /// This method is reponsible for dispose the king reader writer object instance.
        /// </summary>
        /// <param name="kingBufferReader">The king buffer reader object instance</param>
        public static void DisposeKingBufferReader(KingBufferReader kingBufferReader)
        {
            if (!initialized)
                ThreadInitialize();

            _kingBufferReaderPool.ReturnInstance(kingBufferReader);
        }

        /// <summary>
        /// This method is reponsible for dispose the socket async events args object instance.
        /// </summary>
        /// <param name="socketAsyncEventArgs">The socket async events args object instance</param>
        public static void DisposeSocketAsyncEventArgs(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (!initialized)
                ThreadInitialize();

            _socketAsyncEventArgsPool.ReturnInstance(socketAsyncEventArgs);
        }
    }
}
