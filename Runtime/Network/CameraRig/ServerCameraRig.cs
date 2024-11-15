using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// Observer of CameraRigWrapper for each VR plugins.
    /// Can be used as CameraRig in the server.
    /// </summary>
    public class ServerCameraRig : PSNetworkBehaviour, ICameraRig
    {
        public virtual CameraRigType Type { get; }
        /// <summary>
        /// Transform of trackers in network.
        /// </summary>
        [Header("Trackers")]
        [SerializeField, UDictionary.Split(30, 70)]
        protected TrackerDictionary m_trackerTransform;

        public UnityEvent<ServerCameraRig> EventInitialize { get; set; } = new();
        public UnityEvent<ServerCameraRig> EventDespawn { get; set; } = new();

        /// <summary>
        /// All ServerCameraRigs in this scene.
        /// Keys refer to client ID (0, 1, ...)
        /// </summary>
        private static Dictionary<ulong, ServerCameraRig> m_serverCameraRigs = new(32);
        /// <summary>
        /// Try getting ServerCameraRig whose client ID is the same as the arg.
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="result">Successfully get the ServerCameraRig instance?</param>
        /// <returns></returns>
        public static bool TryGetServerCameraRig(ulong clientId, out ServerCameraRig result) => m_serverCameraRigs.TryGetValue(clientId, out result);

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

        #region event
        public override void OnNetworkSpawn()
        {
            // Observe local CameraRig.
            if (IsOwner)
            {
                var manager = PSNetworkManager.Instance;
                var localCameraRig = manager.LocalCameraRig;
                // observe local CameraRig to update tracker transforms from it.
                localCameraRig.AddObserver(this);
            }

            m_serverCameraRigs.Add(this.NetworkObject.OwnerClientId, this);

            // search NetworkPlayerSpawner and call initializing event.
            /*
            var spawner = PSNetworkManager.Instance.NetworkPlayerSpawner;
            if (spawner != null)
            {
                spawner.SetServerCameraRigEvent(this);
            }
            */

            EventInitialize.Invoke(this);
        }
        public override void OnNetworkDespawn()
        {
            var manager = PSNetworkManager.Instance;
            manager.LocalCameraRig.RemoveObserver(this);

            EventDespawn.Invoke(this);
            m_serverCameraRigs.Remove(this.NetworkObject.OwnerClientId);
        }
        #endregion
    }
}