using System;

namespace KingNetwork.Shared
{
    /// <summary>
    /// This class is responsible for represents the king pool of application.
    /// </summary>
    internal class KingPool<T>
    {
        /// <summary>
        /// The generic objects generate function.
        /// </summary>
        private readonly Func<T> _generate;

        /// <summary>
        /// The generic pool.
        /// </summary>
        private readonly T[] _pool;

        /// <summary>
        /// The value of max objects in pool.
        /// </summary>
        public int MaxObjects { get; }

        /// <summary>
        /// The count of objects on pool.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Creates a new instance of a <see cref="KingBufferWriter"/>.
        /// </summary>
        /// <param name="maxObjects">The max objects in pool.</param>
        /// <param name="generate">The function to gerenate the objects.</param>
        public KingPool(int maxObjects, Func<T> generate)
        {
            MaxObjects = maxObjects;
            _generate = generate;
            _pool = new T[maxObjects];
        }

        /// <summary>
        /// This method is reponsible for get the generic object instance of pool.
        /// </summary>
        /// <returns>The generic object instance.</returns>
        public T GetInstance()
        {
            if (Count > 0)
                return _pool[--Count];

            return _generate();
        }

        /// <summary>
        /// This method is reponsible for return the generic object instance of pool.
        /// </summary>
        /// <param name="t">The generic type of object.</param>
        /// <returns>The boolean value of returned instance.</returns>
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
