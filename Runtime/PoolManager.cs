using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// PoolManager
    /// https://qiita.com/KeichiMizutani/items/ca46a40de02e87b3d8a8
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PoolManager<T> : MonoBehaviour where T : MonoBehaviour, IPooledObject<T>
    {
        [SerializeField] private T pooledPrefab;
        protected IObjectPool<T> _objectPool;

        [SerializeField] private bool _collectionCheck = true;

        [SerializeField] private int _defaultCapacity = 32;
        [SerializeField] private int _maxSize = 100;

        public virtual void Initialize()
        {
            _objectPool = new ObjectPool<T>(Create, OnGetFromPool, OnReleaseToPool, OnDestroyPooledObject,
                _collectionCheck, _defaultCapacity, _maxSize);
        }
        /// <summary>
        /// Try get new pooled object.
        /// </summary>
        /// <param name="pooledObject"></param>
        /// <returns></returns>
        public bool TryGet(out T pooledObject)
        {
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
