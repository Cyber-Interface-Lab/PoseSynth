using CyberInterfaceLab.PoseSynth.Network;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace CyberInterfaceLab.PoseSynth
{
    public class NetworkPoseRootBoneRedirector : NetworkPoseRemapper<PoseRootBoneRedirector>
    {

        [ServerRpc(RequireOwnership = false)]
        protected override void SetRefPoseServerRpc(ulong networkObjectId)
        {
            base.SetRefPoseServerRpc(networkObjectId);
        }
        [ServerRpc(RequireOwnership = false)]
        protected override void SetRefPoseToNullServerRpc()
        {
            base.SetRefPoseToNullServerRpc();
        }
        [ClientRpc]
        protected override void SetRefPoseClientRpc(ulong networkObjectId)
        {
            base.SetRefPoseClientRpc(networkObjectId);
        }
        [ClientRpc]
        protected override void SetRefPoseToNullClientRpc()
        {
            base.SetRefPoseToNullClientRpc();
        }
    }
}
