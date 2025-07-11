using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CyberInterfaceLab.PoseSynth.Network;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// VRプラグインごとに異なるカメラリグを統一して扱うためのWrapperパターンです。
    /// 対応する<see cref="ServerCameraRig"/>に通知を行い、ネットワーク間でトラッカの位置・姿勢を同期します。
    /// Wrapper pattern to unify different camera rigs for each VR plugin.
    /// Notifies the corresponding <see cref="ServerCameraRig"/> and synchronizes the position and rotation of the trackers accross the network.
    /// </summary>
    public abstract class LocalCameraRig : MonoBehaviour, ICameraRig
    {
        /// <summary>
        /// このクラスを観測している<see cref="ServerCameraRig"/>です。
        /// <see cref="ServerCameraRig"/>s observing this.
        /// </summary>
        protected HashSet<ServerCameraRig> m_observers = new(64);
        /// <summary>
        /// このクラスを観測する<see cref="ServerCameraRig"/>を追加します。
        /// Add observer to this class.
        /// </summary>
        /// <param name="observer"></param>
        public void AddObserver(ServerCameraRig observer)
        {
            m_observers.Add(observer);
        }
        /// <summary>
        /// このクラスを観測する<see cref="ServerCameraRig"/>を削除します。
        /// Remove observer from this class.
        /// </summary>
        /// <param name="observer"></param>
        public void RemoveObserver(ServerCameraRig observer)
        {
            m_observers.Remove(observer);
        }
        /// <summary>
        /// このクラスを観測している<see cref="ServerCameraRig"/>に通知します。
        /// この通知により、ネットワーク間で各トラッカの位置・姿勢を同期します。
        /// Send notification to observers.
        /// This notification synchronizes the position and rotation of each tracker across the network.
        /// </summary>
        public void NotifyObserver()
        {
            foreach (var observer in m_observers)
            {
                observer.UpdateTrackerTransforms(this);
            }
        }

        /// <summary>
        /// このカメラリグが保持しているトラッカの種類と<see cref="Transform"/>を保持する辞書です。
        /// <see cref="TrackerType"/> and <see cref="Transform"/> dictionary of this camera rig.
        /// </summary>
        [SerializeField, UDictionary.Split(30, 70)]
        protected TrackerDictionary m_trackerTransform;
        /// <summary>
        /// このカメラリグの種類を取得します。
        /// この種類はVRプラグインにより異なります。
        /// Get the type of this camera rig.
        /// This type varies depending on the VR plugin.
        /// </summary>
        public abstract CameraRigType Type { get; }

        /// <inheritdoc/>
        public bool TryGetTransform(TrackerType type, out Transform transform)
        {
            return m_trackerTransform.TryGetValue(type, out transform);
        }
        /// <inheritdoc/>
        public bool TryGetType(Transform transform, out TrackerType type)
        {
            foreach (var pair in m_trackerTransform)
            {
                if (pair.Value == transform)
                {
                    type = pair.Key;
                    return true;
                }
            }

            // fail to get type
            type = TrackerType.None; return false;
        }

        private void Update()
        {
            NotifyObserver();
        }
    }
}