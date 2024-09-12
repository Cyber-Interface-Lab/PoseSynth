using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Mapper that maps Pose from ICameraRig.
    /// </summary>
    [RequireComponent(typeof(Pose))]
    public abstract class PoseMapper : MonoBehaviour, ISynthesizer
    {
        [SerializeField]
        private Component _cameraRig;
        protected ICameraRig m_cameraRig;
        public virtual ICameraRig CameraRig
        {
            get => m_cameraRig;
            set => m_cameraRig = value;
        }
        [SerializeField]
        protected Pose m_pose;
        public Pose Pose
        {
            get => m_pose;
            set => m_pose = value;
        }
        [SerializeField]
        protected bool m_isValid = true;
        public virtual bool IsValid
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

        /// <summary>
        /// Map transforms of ICameraRig to Pose in every fixed frame.
        /// </summary>
        /// <param name="cameraRig"></param>
        protected abstract void MapOnUpdate();

        protected virtual void Awake()
        {
            if (_cameraRig == null)
            {
                CameraRig = null;
            }
            else if (_cameraRig.TryGetComponent<ICameraRig>(out var c))
            {
                CameraRig = c;
            }
        }
        protected virtual void OnValidate()
        {
            m_pose = GetComponent<Pose>();

            // get m_cameraRig
            if (_cameraRig == null)
            {
                m_cameraRig = null;
            }
            else if (_cameraRig.TryGetComponent<ICameraRig>(out var c))
            {
                m_cameraRig = c;
            }
            else
            {
                Debug.LogError("PoseMapper: _cameraRig must be ICameraRig!");
                _cameraRig = null;
                m_cameraRig = null;
            }
        }
        public void FixedUpdate()
        {
            if (IsValid)
            {
                MapOnUpdate();
            }
        }
    }
}