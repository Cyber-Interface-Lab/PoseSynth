using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// ObjectPoolパターンを実装するためのPoolManager抽象クラスです。
    /// https://qiita.com/KeichiMizutani/items/ca46a40de02e87b3d8a8
    /// PoolManager abstract class to implement the ObjectPool pattern.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PoolManager<T> : MonoBehaviour where T : MonoBehaviour, IPooledObject<T>
    {
        [SerializeField] private T pooledPrefab;
        protected IObjectPool<T> _objectPool;
        bool m_isInitialized = false;

        [SerializeField] private bool _collectionCheck = true;

        [SerializeField] private int _defaultCapacity = 32;
        [SerializeField] private int _maxSize = 100;

        /// <summary>
        /// Poolを初期化します。
        /// Initialize the pool.
        /// </summary>
        public virtual void Initialize()
        {
            _objectPool = new ObjectPool<T>(Create, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject,
                _collectionCheck, _defaultCapacity, _maxSize);
            m_isInitialized = true;
        }
        /// <summary>
        /// 新しいPooledObjectを生成します。
        /// Try get new pooled object.
        /// </summary>
        /// <param name="pooledObject"></param>
        /// <returns>Successfully get the new object.</returns>
        public bool TryGet(out T pooledObject)
        {
            if (!m_isInitialized)
            {
                Initialize();
            }

            if (_objectPool == null)
            {
                pooledObject = null;
                return false;
            }
            pooledObject = _objectPool.Get();
            return pooledObject != null;
        }

        #region protected method
        protected virtual T Create()
        {
            T instance = Instantiate(pooledPrefab, transform.position, Quaternion.identity, transform);
            instance.ObjectPool = _objectPool;
            return instance;
        }
        protected virtual void OnReleaseToPool(T pooledObject)
        {
            pooledObject.gameObject.SetActive(false);
        }
        protected virtual void OnGetFromPool(T pooledObject)
        {
            pooledObject.gameObject.SetActive(true);
        }
        protected virtual void OnDestroyPooledObject(T pooledObject)
        {
            pooledObject.Deactivate();
            Destroy(pooledObject.gameObject);
        }
        #endregion
    }
}
