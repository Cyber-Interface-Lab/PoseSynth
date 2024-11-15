using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// <see cref="PoseRemapper"/> for network.
    /// Attach it to the gameObject with <T> component.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class NetworkPoseRemapper<T> : PSNetworkBehaviour, IObserver<PoseRemapper> where T : PoseRemapper
    {
        #region public variable
        public T Remapper => m_remapper;
        #endregion

        #region private variable
        private T m_remapper;
        #endregion

        #region public method
        public void SetRefPose(Pose refPose)
        {
            if (refPose == null)
            {
                SetRefPoseToNullServerRpc(); // -> SetRefPoseToNullClientRpc()
                return;
            }
            if (refPose.TryGetComponent<NetworkObject>(out var no) && no.IsSpawned)
            {
                SetRefPoseServerRpc(no.NetworkObjectId); // -> SetRefPoseClientRpc(no.NetworkObjectId)
                return;
            }
            else
            {
                Debug.LogError($"The Pose ({refPose.name}) is not a network object!");
                return;
            }
        }
        public void OnNotified(PoseRemapper remapper)
        {
            SetRefPose(remapper.RefPose);
        }
        #endregion

        #region private method
        [ServerRpc(RequireOwnership = false)]
        private void SetRefPoseServerRpc(ulong networkObjectId)
        {
            SetRefPoseClientRpc(networkObjectId);
        }
        [ServerRpc(RequireOwnership = false)]
        private void SetRefPoseToNullServerRpc()
        {
            SetRefPoseToNullClientRpc();
        }
        [ClientRpc]
        private void SetRefPoseClientRpc(ulong networkObjectId)
        {
            // search the network object whose id is equal to the argument.
            var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            if (obj.TryGetComponent<Pose>(out var pose))
            {
                m_remapper.RefPose = pose;
                return;
            }

            Debug.LogError($"The network object (id: {networkObjectId}) does not have a Pose!");
        }
        [ClientRpc]
        private void SetRefPoseToNullClientRpc()
        {
            m_remapper.RefPose = null;
        }
        #endregion

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_remapper = GetComponent<T>();
            m_remapper.AddObserver(this);

            if (m_remapper == null)
            {
                Debug.LogWarning($"Could not get PoseRemapper!");
                enabled = false;
                return;
            }
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            m_remapper?.RemoveObserver(this);
        }
    }
}
