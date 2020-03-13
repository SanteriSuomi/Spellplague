using Spellplague.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace Shooter.Utility
{
    #pragma warning disable S4275 // Property uses pool field through it's methods.
    public class ObjectPool<T> : Singleton<ObjectPool<T>> where T : Component
    {
        private Queue<T> pool;
        private Transform parent;
        [SerializeField]
        private T prefabToPool = default;
        [SerializeField]
        private int poolSize = 25;

        protected override void Awake()
        {
            base.Awake();
            InitializePool();
        }

        private void InitializePool()
        {
            pool = new Queue<T>(poolSize);
            parent = new GameObject($"{typeof(T).Name} Pool Objects").transform;
            for (int i = 0; i < poolSize; i++)
            {
                AddNewObjectToPool();
            }
        }

        /// <summary>
        /// Get = Return an object from the pool.
        /// Set = Return an object to the pool.
        /// </summary>
        public T Pool
        {
            get => PoolGet();
            set => PoolSet(value);
        }

        private T PoolGet()
        {
            T peekedObject = pool.Peek();
            if (peekedObject is null || peekedObject.gameObject.activeSelf)
            {
                AddNewObjectToPool();
            }

            T poppedObject = pool.Dequeue();
            SetObjectActiveState(poppedObject, true);
            return poppedObject;
        }

        private void PoolSet(T value)
        {
            SetObjectActiveState(value, false);
            pool.Enqueue(value);
        }

        private void AddNewObjectToPool()
        {
            T newObject = Instantiate(prefabToPool);
            SetObjectActiveState(newObject, false);
            newObject.transform.SetParent(parent);
            pool.Enqueue(newObject);
        }

        private void SetObjectActiveState(T poolObject, bool active)
            => poolObject.gameObject.SetActive(active);
    }
}