using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Remap the <see cref="Pose"/> into another <see cref="Pose"/>.
    /// </summary>
    [RequireComponent(typeof(Pose))]

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    public abstract class PoseRemapper : MonoBehaviour, IPoseTransformer
    {
        #region public variable
        [SerializeField]
        protected bool m_isValid = true;
        public bool IsValid
        {
            get
            {
                if (!m_target)
                {
                    m_isValid = false;
                }
                return m_isValid;
            }
            set
            {
                if (!m_target)
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
            set
            {
                m_target = value;
            }
        }
        public Pose Reference
        {
            get => m_reference;
            set
            {
                //m_refPose = value;
                SetRefPoseWithoutNotice(value);
                Notify();
            }
        }
        #endregion
        #region protected variable
        /// <summary>
        /// The result pose.
        /// </summary>
        [SerializeField]
        protected Pose m_target;
        /// <summary>
        /// The reference pose.
        /// </summary>
        [SerializeField]
        protected Pose m_reference;
        #endregion

        #region observable
        /// <summary>
        /// Call it when the inner values have changed.
        /// </summary>
        /// <param name="observer"></param>
        public abstract void Notify();
        protected bool m_hasModified = false;
        #endregion
        #region public method
        public void SetRefPoseWithoutNotice(Pose pose)
        {
            m_reference = pose;
        }
        #endregion
        #region protected method
        protected abstract void RemapOnUpdate();
        #endregion

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

            if (IsValid)
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
    }
}