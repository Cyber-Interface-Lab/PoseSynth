using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// <see cref="PoseMixer"/> for network.
    /// Attach it to the gameObject with PoseMixer.
    /// </summary>
    [RequireComponent(typeof(PoseMixer))]
    public class NetworkPoseMixer : PSNetworkBehaviour
    {
        #region public variable
        public PoseMixer PoseMixer => m_poseMixer;
        #endregion

        #region private variable
        private PoseMixer m_poseMixer;
        #endregion

        #region private method
        /// <summary>
        /// Update weights of client <see cref="PoseMixer"/>.
        /// </summary>
        [ClientRpc]
        private void UpdateWeightsClientRpc(int index, string label, float weight)
        {
            m_poseMixer.SetWeightOf(m_poseMixer.Poses[index], label, weight);
        }
        #endregion

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_poseMixer = GetComponent<PoseMixer>();

            // enable the GUI of PoseMixer
            m_poseMixer.IsDrawingGUI = IsServer;
        }
        public override void ServerFixedUpdate()
        {
            // synchronize its weights
            for (int j = 0; j < m_poseMixer.MixedBoneGroups.Count; j++)
            {
                var group = m_poseMixer.MixedBoneGroups[j];
                for (int i = 0; i < m_poseMixer.Poses.Count; i++)
                {
                    var pose = m_poseMixer.Poses[i];

                    if (m_poseMixer.SearchWeightOf(pose, group.Label, out float weight))
                    {
                        UpdateWeightsClientRpc(i, group.Label, weight);
                    }
                }
            }
        }
    }
}