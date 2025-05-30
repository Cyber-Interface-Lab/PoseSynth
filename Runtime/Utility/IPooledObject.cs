using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// ObjectPoolパターンを実装するためのインタフェースです。
    /// Interface to implement the ObjectPool pattern.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPooledObject<T> where T : class
    {
        /// <summary>
        /// このオブジェクトが所属するObjectPoolを設定します（書込専用）。
        /// Set the ObjectPool to which this object belongs (write-only).
        /// </summary>
        IObjectPool<T> ObjectPool { set; }
        void Initialize();
        void Deactivate();
    }
}
