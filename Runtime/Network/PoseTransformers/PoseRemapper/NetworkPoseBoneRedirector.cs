using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    public class NetworkPoseBoneRedirector : NetworkPoseRemapper<PoseBoneRedirector>
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
