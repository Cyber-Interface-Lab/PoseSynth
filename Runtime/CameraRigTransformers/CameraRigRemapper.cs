using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Remap the target <see cref="ICameraRig"/> from another <see cref="ICameraRig"/>.
    /// </summary>
    [RequireComponent(typeof(ICameraRig))]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public abstract class CameraRigRemapper : MonoBehaviour, ICameraRigTransformer, IObservable<CameraRigRemapper>
    {
        #region public variable
        public bool IsValid
        {
            get
            {
                if (m_target == null)
                {
                    m_isValid = false;
                }
                return m_isValid;
            }
            set
            {
                if (m_target == null)
                {
                    m_isValid = false;
                    return;
                }
                m_isValid = value;
            }
        }
        public ICameraRig Target
        {
            get => m_target;
            set
            {
                m_target = value;
            }
        }
        public ICameraRig Reference
        {
            get => m_reference;
            set
            {
                SetReferenceWithoutNotice(value);
                Notify();
            }
        }
        #endregion

        #region protected variable
        protected ICameraRig m_target;
        protected ICameraRig m_reference;
        [SerializeField] protected bool m_isValid = true;
        #endregion

        #region IObservable
        private HashSet<IObserver<CameraRigRemapper>> m_observers = new(64);
        public void AddObserver(IObserver<CameraRigRemapper> observer) => m_observers.Add(observer);
        public void RemoveObserver(IObserver<CameraRigRemapper> observer) => m_observers.Remove(observer);
        public void Notify()
        {
            foreach (var observer in m_observers)
            {
                observer.OnNotified(this);
            }
        }
        #endregion

        #region public method
        public void SetReferenceWithoutNotice(ICameraRig cameraRig)
        {
            m_target = cameraRig;
        }
        #endregion

        #region protected method
        protected abstract void RemapOnUpdate();
        #endregion

        #region event
        protected virtual void OnValidate()
        {
            m_target = GetComponent<ICameraRig>();
        }
        void FixedUpdate()
        {
            if (IsValid && m_reference != null)
            {
                RemapOnUpdate();
            }
        }
        #endregion
    }
}
