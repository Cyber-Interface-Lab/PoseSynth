using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace CyberInterfaceLab.PoseSynth
{
    public interface IPooledObject<T> where T : class
    {
        IObjectPool<T> ObjectPool { set; }
        void Initialize();
        void Deactivate();
    }
}
