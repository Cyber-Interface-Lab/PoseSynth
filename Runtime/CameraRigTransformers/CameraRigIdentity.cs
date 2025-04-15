using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// <see cref="CameraRigRemapper{T}"/> that copies the reference to the target <see cref="ICameraRig"/> completely.
    /// </summary>
    public class CameraRigIdentity : CameraRigRemapper, IObservable<CameraRigIdentity>
    {
        public enum TransformType
        {
            None,
            Global,
            Local,
        }

        #region public variable
        #endregion

        #region private variable
        /// <summary>
        /// TrackerTypes to identity.
        /// </summary>
        [SerializeField]
        private List<TrackerType> m_trackers = new(64);
        [SerializeField] TransformType m_position;
        [SerializeField] TransformType m_rotation;


        HashSet<IObserver<CameraRigIdentity>> m_observers = new(64);
        #endregion

        #region public method
        public void AddObserver(IObserver<CameraRigIdentity> observer) => m_observers.Add(observer);
        public void RemoveObserver(IObserver<CameraRigIdentity> observer) => m_observers.Remove(observer);
        public override void Notify()
        {
            foreach (var observer in m_observers)
            {
                observer.OnNotified(this);
            }
        }
        #endregion

        #region private method
        protected override void RemapOnUpdate()
        {
            if (m_reference == null)
            {
                Debug.LogWarning($"no ref!");
                return;
            }
            if (m_target == null) m_target = GetComponent<ICameraRig>();

            // refからtargetに座標を単純コピーする
            // Simply copy the position from reference to target
            for (int i = 0; i < m_trackers.Count; i++)
            {
                if (m_target.TryGetTransform(m_trackers[i], out Transform targetTransform) && targetTransform != null &&
                    m_reference.TryGetTransform(m_trackers[i], out Transform referenceTransform) && referenceTransform != null)
                {
                    switch (m_position)
                    {
                        case TransformType.Global:
                            targetTransform.position = referenceTransform.position;
                            break;
                        case TransformType.Local:
                            targetTransform.localPosition = referenceTransform.localPosition;
                            break;
                        case TransformType.None:
                        default:
                            break;
                    }

                    switch (m_rotation)
                    {
                        case TransformType.Global:
                            targetTransform.rotation = referenceTransform.rotation;
                            break;
                        case TransformType.Local:
                            targetTransform.localRotation = referenceTransform.localRotation;
                            break;
                        case TransformType.None:
                        default:
                            break;
                    }
                }
            }
        }
        #endregion

        #region event
        #endregion
    }
}
