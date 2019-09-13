using System;
using System.Net.Sockets;

namespace KingNetwork.Shared
{
    public class KingPoolManager
    {
        private static KingPoolManager _instance;

        [ThreadStatic]
        private static KingPool<KingBufferWriter> _kingBufferWriterPool;

        [ThreadStatic]
        private static KingPool<KingBufferReader> _kingBufferReaderPool;
        
        [ThreadStatic]
        private static KingPool<SocketAsyncEventArgs> _socketAsyncEventArgsPool;
        
        public KingPoolManager()
        {
            _kingBufferWriterPool = new KingPool<KingBufferWriter>(2, () => new KingBufferWriter());
            _kingBufferReaderPool = new KingPool<KingBufferReader>(2, () => new KingBufferReader());
            _socketAsyncEventArgsPool = new KingPool<SocketAsyncEventArgs>(32, () => new SocketAsyncEventArgs());
        }   

        public static KingPoolManager GetInstance()
        {
            if (_instance == null)
                _instance = new KingPoolManager();

            return _instance;
        }

        public KingBufferWriter KingBufferWriter => _kingBufferWriterPool.GetInstance();
        public KingBufferReader KingBufferReader => _kingBufferReaderPool.GetInstance();
        public SocketAsyncEventArgs SocketAsyncEventArgs => _socketAsyncEventArgsPool.GetInstance();

        public void DisposeKingBufferWriter(KingBufferWriter kingBufferWriter)
        {
            if (!_kingBufferWriterPool.ReturnInstance(kingBufferWriter))
                kingBufferWriter.Dispose();
        }

        public void DisposeKingBufferReader(KingBufferReader kingBufferReader)
        {
            if (!_kingBufferReaderPool.ReturnInstance(kingBufferReader))
                kingBufferReader.Dispose();
        }

        public void DisposeSocketAsyncEventArgs(SocketAsyncEventArgs socketAsyncEventArgs)
        {
            if (!_socketAsyncEventArgsPool.ReturnInstance(socketAsyncEventArgs))
                socketAsyncEventArgs.Dispose();
        }
    }
}
