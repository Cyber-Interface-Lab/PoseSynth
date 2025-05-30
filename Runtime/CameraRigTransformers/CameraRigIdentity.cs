using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// <see cref="ICameraRig"/>のトラッカの位置・姿勢を別の<see cref="ICameraRig"/>にコピーするクラスです。
    /// <see cref="CameraRigRemapper{T}"/> that copies the reference to the target <see cref="ICameraRig"/> completely.
    /// </summary>
    /// <remarks>
    /// このクラスは<see cref="CameraRigRemapperMultipleReferences{T}"/>の入力に用いる<see cref="ICameraRig"/>を変更する際に使用します。
    /// <see cref="CameraRigRemapperMultipleReferences{T}"/>の入力はネットワーク間で同期されないため、通常は変更が推奨されていませんが、
    /// このクラスと別の<see cref="ICameraRig"/>を以下のように中継することで、<see cref="CameraRigRemapperMultipleReferences{T}"/>の入力を変更することができます。
    /// A's server cameraRig --- CameraRigIdentity --> 中継用simple local cameraRig --- CameraRigMixer --> mixed cameraRig
    /// 
    /// This class is used to change the <see cref="ICameraRig"/> used as input for <see cref="CameraRigRemapperMultipleReferences{T}"/>.
    /// It is not recommended to change the input of <see cref="CameraRigRemapperMultipleReferences{T}"/> as it is not synchronized across the network.
    /// But you can change the input of <see cref="CameraRigRemapperMultipleReferences{T}"/> by relaying it through this class and another <see cref="ICameraRig"/>:
    /// A's server cameraRig --- CameraRigIdentity --> relay simple local cameraRig --- CameraRigMixer --> mixed cameraRig
    /// </remarks>
    public class CameraRigIdentity : CameraRigRemapper, IObservable<CameraRigIdentity>
    {
        /// <summary>
        /// コピーの方法を指定します。
        /// Enum to specify the copy method.
        /// </summary>
        public enum TransformType
        {
            /// <summary>
            /// Copy nothing.
            /// </summary>
            None,
            /// <summary>
            /// Copy the global position/rotation.
            /// </summary>
            Global,
            /// <summary>
            /// Copy the local position/rotation.
            /// </summary>
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
        /// <inheritdoc/>
        public void AddObserver(IObserver<CameraRigIdentity> observer) => m_observers.Add(observer);
        /// <inheritdoc/>
        public void RemoveObserver(IObserver<CameraRigIdentity> observer) => m_observers.Remove(observer);
        /// <inheritdoc/>
        public override void Notify()
        {
            foreach (var observer in m_observers)
            {
                observer.OnNotified(this);
            }
        }
        #endregion

        #region private method
        /// <inheritdoc/>
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
