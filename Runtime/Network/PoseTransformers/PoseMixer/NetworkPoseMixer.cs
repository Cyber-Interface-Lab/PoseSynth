using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// <see cref="PoseMixer"/>用の<see cref="NetworkBehaviour"/>です。
    /// <see cref="PoseMixer"/>の各関節グループの重みをネットワークで同期します。
    /// <see cref="NetworkBehaviour"/> for <see cref="PoseMixer"/>.
    /// This class is used to synchronize the weights of <see cref="PoseMixer"/> across the network.
    /// </summary>
    [RequireComponent(typeof(PoseMixer))]
    public class NetworkPoseMixer : PSNetworkBehaviour, IObserver<PoseMixer>
    {
        #region public variable
        /// <summary>
        /// このクラスが参照している<see cref="PoseMixer"/>を取得します（読込専用）。
        /// Get the <see cref="PoseMixer"/> that this class refers to (read-only).
        /// </summary>
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
            m_poseMixer.SetWeight(m_poseMixer.References[index], label, weight);
        }
        #endregion

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_poseMixer = GetComponent<PoseMixer>();

            // enable the GUI of PoseMixer
            m_poseMixer.IsDrawingGUI = IsServer;

            // subscribe PoseMixer
            m_poseMixer.AddObserver(this);
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            m_poseMixer.IsDrawingGUI = true;
            m_poseMixer.RemoveObserver(this);
        }
        public void OnNotified(PoseMixer observable)
        {
            // synchronize its weights
            for (int j = 0; j < m_poseMixer.MixedJointGroups.Count; j++)
            {
                var group = m_poseMixer.MixedJointGroups[j];
                for (int i = 0; i < m_poseMixer.References.Count; i++)
                {
                    var pose = m_poseMixer.References[i];

                    if (m_poseMixer.TryGetWeight(pose, group.Label, out float weight))
                    {
                        UpdateWeightsClientRpc(i, group.Label, weight);
                    }
                }
            }
        }
    }
}