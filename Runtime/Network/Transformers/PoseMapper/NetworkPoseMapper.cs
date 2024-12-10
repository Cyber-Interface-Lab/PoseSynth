using CyberInterfaceLab.PoseSynth.Network;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// <see cref="PoseMapper"/> for network.
    /// Attach it to the gameObject with PoseMapper.
    /// </summary>
    public class NetworkPoseMapper : PSNetworkBehaviour, IObserver<PoseMapper>
    {
        #region public variable
        public PoseMapper[] Mappers => m_mappers;
        #endregion

        #region private variable
        private PoseMapper[] m_mappers;
        #endregion

        #region public method
        public void SetCameraRig(ICameraRig cameraRig)
        {
            //Debug.Log("SetCameraRig");
            if (cameraRig == null)
            {
                SetCameraRigToNullServerRpc();
                return;
            }
            if (cameraRig is MonoBehaviour mb && mb.TryGetComponent<NetworkObject>(out var no) && no.IsSpawned)
            {
                SetCameraRigServerRpc(no.NetworkObjectId);
                return;
            }
            else
            {
                Debug.LogError($"The CameraRig is not a network object!");
                return;
            }
        }
        public void OnNotified(PoseMapper mapper)
        {
            SetCameraRig(mapper.CameraRig);
        }
        #endregion

        #region private method
        [ServerRpc(RequireOwnership = false)]
        private void SetCameraRigServerRpc(ulong networkObjectId)
        {
            //Debug.Log("SetCameraRigServerRpc: " + networkObjectId);
            var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            if (obj.TryGetComponent<ICameraRig>(out var cr))
            {
                foreach (var mapper in m_mappers)
                {
                    mapper.SetCameraRigWithoutNotice(cr);
                }
            }
            SetCameraRigClientRpc(networkObjectId);
        }
        [ServerRpc(RequireOwnership = false)]
        private void SetCameraRigToNullServerRpc()
        {
            foreach (var mapper in m_mappers)
            {
                mapper.SetCameraRigWithoutNotice(null);
            }
            SetCameraRigToNullClientRpc();
        }
        [ClientRpc]
        private void SetCameraRigClientRpc(ulong networkObjectId)
        {
            //Debug.Log("SetCameraRigClientRpc: " + networkObjectId);
            // search the network object whose id is equal to the argument.
            var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            if (obj.TryGetComponent<ICameraRig>(out var cr))
            {
                foreach (var mapper in m_mappers)
                {
                    mapper.SetCameraRigWithoutNotice(cr);
                }
                return;
            }

            Debug.LogError($"The network object (id: {networkObjectId}) does not have an ICameraRig!");
        }
        [ClientRpc]
        private void SetCameraRigToNullClientRpc()
        {
            foreach (var mapper in m_mappers)
            {
                mapper.SetCameraRigWithoutNotice(null);
            }
        }
        #endregion

        #region event
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_mappers = GetComponents<PoseMapper>();

            for (int i = 0; i < m_mappers.Length; i++)
            {
                m_mappers[i].AddObserver(this);
            }
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            if (m_mappers == null) { return; }
            for (int i = 0; i < m_mappers.Length; i++)
            {
                m_mappers[i]?.RemoveObserver(this);
            }
        }
        #endregion
    }
}
