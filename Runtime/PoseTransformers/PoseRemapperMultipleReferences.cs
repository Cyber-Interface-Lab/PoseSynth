using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public abstract class PoseRemapperMultipleReferences : MonoBehaviour, IPoseTransformer
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
        public Pose Target
        {
            get => m_target;
            set => m_target = value;
        }
        public List<Pose> References
        {
            get => m_references;
            set => m_references = value;
        }
        #endregion

        #region private variable
        [SerializeField] protected Pose m_target;
        [SerializeField] protected List<Pose> m_references = new(64);
        [SerializeField] protected bool m_isValid = true;
        #endregion

        #region IObservable
        public abstract void Notify();
        /// <summary>
        /// Flag to check whether the object has been modified.
        /// If true, <see cref="Notify"/> will be called in <see cref="LateUpdate"/> to synchronize all the parameters to clients.
        /// </summary>
        protected bool m_hasModified = false;
        #endregion

        #region public method
        public virtual void AddPose(Pose pose)
        {
            if (pose == null)
            {
                Debug.LogError("Pose is null");
                return;
            }
            if (!m_references.Contains(pose))
                m_references.Add(pose);
        }
        public virtual void RemovePose(Pose pose)
        {
            if (pose == null)
            {
                Debug.LogError("Pose is null");
                return;
            }
            if (m_references.Contains(pose))
                m_references.Remove(pose);
        }
        #endregion

        #region private method
        protected abstract void RemapOnUpdate();
        #endregion

        #region event
        protected virtual void OnValidate()
        {
            m_target = GetComponent<Pose>();
        }
        protected virtual void Awake()
        {
            m_target = GetComponent<Pose>();
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
