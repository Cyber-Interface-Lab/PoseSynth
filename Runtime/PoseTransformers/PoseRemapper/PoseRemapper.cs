using System.Collections.Generic;
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

    public abstract class PoseRemapper : MonoBehaviour, IPoseTransformer, IObservable<PoseRemapper>
    {
        /// <summary>
        /// The reference pose.
        /// </summary>
        [SerializeField]
        protected Pose m_refPose;
        public Pose RefPose
        {
            get => m_refPose;
            set
            {
                //m_refPose = value;
                SetRefPoseWithoutNotice(value);
                Notify();
            }
        }
        /// <summary>
        /// The result pose.
        /// </summary>
        [SerializeField]
        protected Pose m_pose;
        public Pose Pose
        {
            get => m_refPose;
            set
            {
                m_refPose = value;
                //Notify();
            }
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

        #region observable
        private HashSet<IObserver<PoseRemapper>> m_observers = new(64);
        public void AddObserver(IObserver<PoseRemapper> observer) => m_observers.Add(observer);
        public void RemoveObserver(IObserver<PoseRemapper> observer) => m_observers.Remove(observer);
        /// <summary>
        /// Call it when the inner values have changed.
        /// </summary>
        /// <param name="observer"></param>
        public void Notify()
        {
            foreach (var observer in m_observers)
            {
                observer.OnNotified(this);
            }
        }
        public void SetRefPoseWithoutNotice(Pose pose)
        {
            m_refPose = pose;
        }
        #endregion

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