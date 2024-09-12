using UnityEditor;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Remap the Pose into another Pose.
    /// </summary>
    [RequireComponent(typeof(Pose))]

#if UNITY_EDITOR
    [InitializeOnLoad]
#endif

    public abstract class PoseRemapper : MonoBehaviour, ISynthesizer
    {
        /// <summary>
        /// The reference pose.
        /// </summary>
        [SerializeField]
        protected Pose m_refPose;
        public Pose RefPose
        {
            get => m_refPose;
            set => m_refPose = value;
        }
        /// <summary>
        /// The result pose.
        /// </summary>
        [SerializeField]
        protected Pose m_pose;
        public Pose Pose
        {
            get => m_refPose;
            set => m_refPose = value;
        }

        [SerializeField]
        protected bool m_isValid = true;
        public bool IsValid
        {
            get
            {
                if (!m_pose)
                {
                    m_isValid = false;
                }
                return m_isValid;
            }
            set
            {
                if (!m_pose)
                {
                    m_isValid = false;
                    return;
                }
                m_isValid = value;
            }
        }

        protected virtual void OnValidate()
        {
            m_pose = GetComponent<Pose>();
        }
        private void FixedUpdate()
        {
            if (IsValid)
            {
                RemapOnUpdate();
            }
        }
        protected abstract void RemapOnUpdate();
    }
}