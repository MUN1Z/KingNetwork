using System;

namespace KingNetwork.Shared
{
    internal class KingPool<T>
    {
        private readonly Func<T> _generate;
        private readonly T[] _pool;

        public int MaxObjects { get; }

        public int Count { get; set; }

        public KingPool(int maxObjects, Func<T> generate)
        {
            MaxObjects = maxObjects;
            _generate = generate;
            _pool = new T[maxObjects];
        }

        public T GetInstance()
        {
            if (Count > 0)
                return _pool[--Count];

            return _generate();
        }

        public bool ReturnInstance(T t)
        {
            if (Count < MaxObjects)
            {
                _pool[Count++] = t;
                return true;
            }

            return false;
        }
    }
}
