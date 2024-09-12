using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CyberInterfaceLab.PoseSynth.Network;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Wrapper pattern for each camera rig.
    /// Subject of ServerCameraRig (as Observer).
    /// </summary>
    public abstract class LocalCameraRig : MonoBehaviour, ICameraRig
    {
        /// <summary>
        /// NetworkCameraRigs observing this.
        /// </summary>
        protected HashSet<ServerCameraRig> m_observers = new(64);
        public void AddObserver(ServerCameraRig observer)
        {
            m_observers.Add(observer);
        }
        public void RemoveObserver(ServerCameraRig observer)
        {
            m_observers.Remove(observer);
        }
        /// <summary>
        /// Send notification to observers.
        /// </summary>
        public void NotifyObserver()
        {
            foreach (var observer in m_observers)
            {
                observer.UpdateTrackerTransforms(this);
            }
        }

        /// <summary>
        /// Trackers of camera rig.
        /// </summary>
        [SerializeField, UDictionary.Split(30, 70)]
        protected TrackerDictionary m_trackerTransform;

        /// <summary>
        /// Try to get transform of the tracker.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="transform"></param>
        /// <returns></returns>
        public bool TryGetTransform(TrackerType type, out Transform transform)
        {
            return m_trackerTransform.TryGetValue(type, out transform);
        }

        private void Update()
        {
            NotifyObserver();
        }
    }
}