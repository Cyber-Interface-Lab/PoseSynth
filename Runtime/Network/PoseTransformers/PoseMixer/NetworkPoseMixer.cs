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
    public class NetworkPoseMixer : PSNetworkBehaviour, IObserver<PoseMixer>
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
        //public override void ServerFixedUpdate()
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