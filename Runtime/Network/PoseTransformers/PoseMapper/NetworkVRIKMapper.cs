using CyberInterfaceLab.PoseSynth.IK;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    public class NetworkVRIKMapper : NetworkPoseMapper<VRIKMapper>
    {
        #region public variable
        #endregion

        #region private variable
        #endregion

        #region public method
        #endregion

        #region private method
        [ServerRpc(RequireOwnership = false)]
        protected override void SetCameraRigServerRpc(ulong networkObjectId)
        {
            var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            if (obj.TryGetComponent<ICameraRig>(out var cr))
            {
                m_mapper.SetCameraRigWithoutNotice(cr);
            }
            SetCameraRigClientRpc(networkObjectId);
        }
        [ServerRpc(RequireOwnership = false)]
        protected override void SetCameraRigToNullServerRpc()
        {
            m_mapper.SetCameraRigWithoutNotice(null);
            SetCameraRigToNullClientRpc();
        }
        [ClientRpc]
        protected override void SetCameraRigClientRpc(ulong networkObjectId)
        {
            // search the network object whose id is equal to the argument.
            var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            if (obj.TryGetComponent<ICameraRig>(out var cr))
            {
                m_mapper.SetCameraRigWithoutNotice(cr);
                return;
            }

            Debug.LogError($"The network object (id: {networkObjectId}) does not have an ICameraRig!");
        }
        [ClientRpc]
        protected override void SetCameraRigToNullClientRpc()
        {
            m_mapper.SetCameraRigWithoutNotice(null);
        }
        protected override void Observe(VRIKMapper observable)
        {
            observable.AddObserver(this);
        }
        protected override void Unobserve(VRIKMapper observable)
        {
            observable.RemoveObserver(this);
        }
        #endregion
    }
}
