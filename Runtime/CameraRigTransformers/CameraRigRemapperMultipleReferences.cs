using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Remap the target <see cref="ICameraRig"/> from other multiple <see cref="ICameraRig"/>s.
    /// </summary>
    [RequireComponent(typeof(ICameraRig))]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public abstract class CameraRigRemapperMultipleReferences : MonoBehaviour, ICameraRigTransformer, IObservable<CameraRigRemapperMultipleReferences>
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
            set => m_target = value;
        }
        public List<ICameraRig> References
        {
            get
            {
                var result = new List<ICameraRig>(64);
                foreach (var component in m_references)
                {
                    if (component != null && component.TryGetComponent<ICameraRig>(out var cameraRig))
                    {
                        result.Add(cameraRig);
                    }
                }
                return result;
            }
        }
        #endregion

        #region protected variable
        //protected ICameraRig m_target;
        //protected List<ICameraRig> m_references = new(64);
        protected ICameraRig m_target;
        [SerializeField] protected List<MonoBehaviour> m_references = new(64);
        [SerializeField] protected bool m_isValid = true;
        #endregion

        #region IObservable
        private HashSet<IObserver<CameraRigRemapperMultipleReferences>> m_observers = new(64);
        public void AddObserver(IObserver<CameraRigRemapperMultipleReferences> observer) => m_observers.Add(observer);
        public void RemoveObserver(IObserver<CameraRigRemapperMultipleReferences> observer) => m_observers.Remove(observer);
        public void Notify()
        {
            foreach (var observer in m_observers)
            {
                observer.OnNotified(this);
            }
        }
        #endregion

        #region public method
        public virtual void AddReference(ICameraRig reference, bool withNotice=true)
        {
            // return if the list already has the input
            if (m_references.Contains(reference as MonoBehaviour))
            {
                return;
            }

            m_references.Add((MonoBehaviour)reference);
            m_references.RemoveAll(x => x == null);
            if (withNotice)
            {
                Notify();
            }
        }
        public virtual void RemoveReference(ICameraRig reference, bool withNotice=true)
        {
            if (!m_references.Contains(reference as MonoBehaviour))
            {
                return;
            }
            m_references.Remove((MonoBehaviour)reference);
            m_references.RemoveAll(x => x == null);
            if (withNotice)
            {
                Notify();
            }
        }
        #endregion

        #region protected method
        protected abstract void RemapOnUpdate();
        #endregion

        #region event
        protected virtual void OnValidate()
        {
            m_target = GetComponent<ICameraRig>();

            // remove contents of references who does not inherit ICameraRig
            m_references.RemoveAll(x => x is not ICameraRig && x is not null);
        }
        void FixedUpdate()
        {
            if (m_isValid && m_references.Count > 0)
            {
                RemapOnUpdate();
            }
        }
        #endregion
    }
}
