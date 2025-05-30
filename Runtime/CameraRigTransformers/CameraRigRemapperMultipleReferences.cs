using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// 複数の<see cref="ICameraRig"/>の情報を元に、別の<see cref="ICameraRig"/>を変換するクラスです。
    /// This class remapds the target <see cref="ICameraRig"/> from other multiple <see cref="ICameraRig"/>s.
    /// </summary>
    /// <remarks>
    /// When you want to use this in the multiplayer scene, please use <see cref="CameraRigIdentity"/> as the input references,
    /// Because PoseSynth does not synchronize the reference among clients. 
    /// </remarks>
    [RequireComponent(typeof(ICameraRig))]
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public abstract class CameraRigRemapperMultipleReferences<T> : MonoBehaviour, ICameraRigTransformer, IObservable<T>
        where T: CameraRigRemapperMultipleReferences<T>
    {
        #region public variable
        /// <inheritdoc/>
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
        /// <inheritdoc/>
        public ICameraRig Target
        {
            get => m_target;
            set => m_target = value;
        }
        /// <summary>
        /// <see cref="Target"/>の変換のために入力として使用する<see cref="ICameraRig"/>です。
        /// <see cref="ICameraRig"/> used as input for the transformation of <see cref="Target"/>.
        /// </summary>
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
        private HashSet<IObserver<T>> m_observers = new(64);
        /// <inheritdoc/>
        public void AddObserver(IObserver<T> observer) => m_observers.Add(observer);
        /// <inheritdoc/>
        public void RemoveObserver(IObserver<T> observer) => m_observers.Remove(observer);
        /// <inheritdoc/>
        public void Notify()
        {
            foreach (var observer in m_observers)
            {
                observer.OnNotified(this as T);
            }
        }
        /// <summary>
        /// Flag to check whether any parameter has been modified.
        /// If true, <see cref="Notify"/> will be called in <see cref="LateUpdate"/> to synchronize all the parameters to clients.
        /// </summary>
        protected bool m_hasModified = false;
        #endregion

        #region public method
        /// <summary>
        /// <see cref="References"/>に<see cref="ICameraRig"/>を追加します。
        /// Add <see cref="ICameraRig"/> to <see cref="References"/>.
        /// </summary>
        /// <param name="reference"></param>
        public virtual void AddReference(ICameraRig reference)
        {
            // return if the list already has the input
            if (m_references.Contains(reference as MonoBehaviour))
            {
                return;
            }

            m_references.Add((MonoBehaviour)reference);
            m_references.RemoveAll(x => x == null);
        }
        /// <summary>
        /// <see cref="References"/>から<see cref="ICameraRig"/>を削除します。
        /// Remove <see cref="ICameraRig"/> from <see cref="References"/>.
        /// </summary>
        /// <param name="reference"></param>
        public virtual void RemoveReference(ICameraRig reference)
        {
            if (!m_references.Contains(reference as MonoBehaviour))
            {
                return;
            }
            m_references.Remove((MonoBehaviour)reference);
            m_references.RemoveAll(x => x == null);
        }
        #endregion

        #region protected method
        /// <summary>
        /// <see cref="IsValid"/>がtrueの場合に、毎ループ呼び出される変換用の関数です。
        /// Called every loop when <see cref="IsValid"/> is true.
        /// </summary>
        protected abstract void RemapOnUpdate();
        #endregion

        #region event
        protected virtual void OnValidate()
        {
            m_target = GetComponent<ICameraRig>();
            // remove contents of references who does not inherit ICameraRig
            //m_references.RemoveAll(x => x is not ICameraRig && x is not null);
        }
        protected virtual void Awake()
        {
            m_target = GetComponent<ICameraRig>();
            // remove contents of references who does not inherit ICameraRig
            //m_references.RemoveAll(x => x is not ICameraRig && x is not null);
        }
        protected virtual void Update()
        {
            m_hasModified = false;
            if (m_isValid && m_references.Count > 0)
            {
                RemapOnUpdate();
            }
        }
        void LateUpdate()
        {
            if (m_hasModified)
            {
                Notify();
            }
        }
        #endregion
    }
}
