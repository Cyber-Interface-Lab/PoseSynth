using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// Observer of CameraRigWrapper for each VR plugins.
    /// Can be used as CameraRig in the server.
    /// </summary>
    public class ServerCameraRig : PSNetworkBehaviour, ICameraRig
    {
        /// <summary>
        /// Transform of trackers in network.
        /// </summary>
        [SerializeField, UDictionary.Split(30, 70)]
        protected TrackerDictionary m_trackerTransform;

        /// <summary>
        /// Update each tracker's transform in network.
        /// </summary>
        /// <param name="wrapper"></param>
        public void UpdateTrackerTransforms(LocalCameraRig wrapper)
        {
            foreach (var type in m_trackerTransform.Keys)
            {
                if (wrapper.TryGetTransform(type, out var refTransform))
                {
                    var transform = m_trackerTransform[type];
                    transform.localPosition = refTransform.localPosition;
                    transform.localRotation = refTransform.localRotation;
                }
                else
                {
                    Debug.LogWarning($"ServerCameraRig: LocalCameraRig does not have Transform of {type}!");
                }
            }
        }
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

        #region event
        public override void OnNetworkSpawn()
        {
            var manager = PSNetworkManager.Instance;

            // Observe local CameraRig.
            if (IsOwner)
            {
                manager.LocalCameraRig.AddObserver(this);
            }
        }
        public override void OnNetworkDespawn()
        {
            var manager = PSNetworkManager.Instance;
            manager.LocalCameraRig.RemoveObserver(this);
        }
        #endregion
    }
}