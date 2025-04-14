using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    public class CameraRigIdentity : CameraRigRemapper<CameraRigIdentity>
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
        #endregion

        #region public method
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
