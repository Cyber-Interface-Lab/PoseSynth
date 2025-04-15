using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Mapper that maps Pose from ICameraRig.
    /// </summary>
    [RequireComponent(typeof(Pose))]
    public abstract class PoseMapper : MonoBehaviour, IPoseTransformer
    {
        #region public variable
        public virtual bool IsValid
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
            set => m_target = value;
        }
        public virtual ICameraRig Reference
        {
            get => m_reference;
            set
            {
                SetCameraRigWithoutNotice(value);
                Notify();
            }
        }
        #endregion

        #region protected variable
        [SerializeField]
        protected Pose m_target;
        [SerializeField]
        private Component m_referenceObject;
        protected ICameraRig m_reference;
        [SerializeField]
        protected bool m_isValid = true;
        #endregion

        #region observable
        public abstract void Notify();
        protected bool m_hasModified = false;
        #endregion

        #region public method
        public virtual void SetCameraRigWithoutNotice(ICameraRig value)
        {
            m_reference = value;
        }
        #endregion

        #region protected method
        /// <summary>
        /// Map transforms of ICameraRig to Pose in every fixed frame.
        /// </summary>
        /// <param name="cameraRig"></param>
        protected abstract void MapOnUpdate();
        #endregion



        protected virtual void Awake()
        {
            if (m_referenceObject == null)
            {
                Reference = null;
            }
            else if (m_referenceObject.TryGetComponent<ICameraRig>(out var c))
            {
                Reference = c;
            }
        }
        protected virtual void OnValidate()
        {
            m_target = GetComponent<Pose>();

            // get m_cameraRig from Component _cameraRig
            if (m_referenceObject == null)
            {
                m_reference = null;
            }
            else if (m_referenceObject.TryGetComponent<ICameraRig>(out var c))
            {
                m_reference = c;
            }
            else
            {
                Debug.LogError("PoseMapper: m_referenceObject must be ICameraRig!");
                m_referenceObject = null;
                m_reference = null;
            }
        }
        protected virtual void Update()
        {
            m_hasModified = false;

            if (IsValid)
            {
                MapOnUpdate();
            }
        }
    }
}