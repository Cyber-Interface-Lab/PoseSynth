using UnityEngine;
using System;

namespace CyberInterfaceLab.PoseSynth
{

    /// <summary>
    /// Singleton pattern
    /// </summary>
    /// <typeparam name="SingletonType"></typeparam>
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {

        private static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    Type t = typeof(T);

                    instance = (T)FindObjectOfType(t);
                    if (instance == null)
                    {
                        Debug.LogError($"There's no GameObject that attouches {t}");
                    }
                }

                return instance;
            }
        }

        virtual protected void Awake()
        {
            // Check there is an instance of this class in the scene.
            CheckInstance();
        }
        /// <summary>
        /// Check if there is another instance of this class in the scene.
        /// </summary>
        /// <returns>This instance is the only instance.</returns>
        protected bool CheckInstance()
        {
            if (instance == null)
            {
                instance = this as T;
                return true;
            }
            else if (Instance == this)
            {
                return true;
            }
            Destroy(this);
            return false;
        }
    }
}