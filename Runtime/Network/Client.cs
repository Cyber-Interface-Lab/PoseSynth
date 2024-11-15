using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// Class of the client in the server.
    /// </summary>
    [Obsolete("Network architectures using this class are not recommended from v1.1.0.")]
    public class Client : PSNetworkBehaviour
    {
        /// <summary>
        /// ServerCameraRig of the client.
        /// </summary>
        [SerializeField]
        [Tooltip("ServerCameraRig of the client.")]
        private ServerCameraRig m_cameraRig;
        /// <summary>
        /// NetworkPose of the client as the avatar.
        /// Can get from PSNetworkManager.
        /// </summary>
        [SerializeField]
        [Tooltip("NetworkPose of the client as the avatar. Can get from m_pose.")]
        private Pose m_poseServer;
        /// <summary>
        /// Mappers from ServerCameraRig to NetworkPose.
        /// Can get from m_pose.GetComponentsInChildren<...>().
        /// </summary>
        private HashSet<PoseMapper> m_mappers;

        /// <summary>
        /// The avatar of the client in the client.
        /// This is the avatar that the client sees.
        /// The pose/motion is copied from m_poseServer.
        /// </summary>
        [SerializeField]
        [Tooltip("The avatar of the client in the client. This is the avatar that the client sees. The pose/motion is copied from m_poseServer.")]
        private NetworkPose m_pose;
        public NetworkPose Pose => m_pose;
        private PoseMixer m_mixer;
        private HashSet<PoseRemapper> m_boneRedirectors;

        // Prefabs
        /// <summary>
        /// Prefab of the user's avatar in the server.
        /// </summary>
        [Tooltip("Prefab of the user's avatar in the server.")]
        public Pose PoseServerPrefab;
        /// <summary>
        /// Prefab of the user's avatar sent to the client.
        /// </summary>
        [Tooltip("Prefab of the user's avatar sent to the client.")]
        public NetworkPose PoseClientPrefab;

        #region event
        public override void OnNetworkSpawn()
        {
            if (!IsServer) { return; }

            var mappedInstance = Instantiate(PoseServerPrefab);
            m_poseServer = mappedInstance;
            m_mappers = m_poseServer.GetComponents<PoseMapper>().ToHashSet();
            foreach (var mapper in m_mappers)
            {
                mapper.CameraRig = m_cameraRig;
            }
            mappedInstance.transform.parent = transform;

            // Identity mapping by PoseMixer and bone redirectors
            // in order to show the Pose in the client environment.
            var instance = Instantiate(PoseClientPrefab);
            m_pose = instance;
            if (m_pose.TryGetComponent<PoseMixer>(out var mixer))
            {
                m_mixer = mixer;
                m_mixer.Poses.Add(m_poseServer);
            }
            m_boneRedirectors = m_pose.GetComponents<PoseRemapper>().ToHashSet();
            foreach (var r in m_boneRedirectors)
            {
                r.RefPose = m_poseServer;
            }

            // spawn instance
            instance.Spawn();
            instance.transform.parent = transform;
        }
        public override void OnNetworkDespawn()
        {
            if (IsServer) { return; }

            m_pose?.Despawn();
        }
        #endregion
    }
}