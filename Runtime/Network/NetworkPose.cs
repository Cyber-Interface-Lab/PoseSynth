using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// Wrapper of Pose for network.
    /// </summary>
    [RequireComponent(typeof(Pose))]
    public class NetworkPose : PSNetworkBehaviour
    {
        [SerializeField]
        private Pose m_pose;
        public Pose Pose => m_pose;
        /// <summary>
        /// IEditors attached to this GameObject.
        /// They attach this m_pose as Pose.
        /// They should be invalid in client environment.
        /// </summary>
        private HashSet<ISynthesizer> m_poseEditors;
        /// <summary>
        /// Implicit operator for Pose.
        /// </summary>
        /// <param name="pose"></param>
        public static implicit operator Pose(NetworkPose pose)
        {
            return pose.m_pose;
        }

        public override void OnNetworkSpawn()
        {
            m_poseEditors = GetComponents<ISynthesizer>().ToHashSet();
            foreach (var editor in m_poseEditors)
            {
                editor.IsValid = IsHost || IsServer;
            }
        }
        private void OnValidate()
        {
            if (m_pose == null)
            {
                m_pose = GetComponent<Pose>();
            }
        }
    }
}