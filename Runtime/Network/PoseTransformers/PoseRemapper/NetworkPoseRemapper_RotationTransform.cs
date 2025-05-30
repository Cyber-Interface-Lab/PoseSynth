using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    public class NetworkPoseRemapper_RotationTransformation : NetworkPoseRemapper<PoseRemapper_RotationTransformation>
    {
        #region public variable
        #endregion

        #region private variable
        #endregion

        #region public method
        #endregion

        #region private method
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
        protected override void Observe(PoseRemapper_RotationTransformation observable)
        {
            observable.AddObserver(this);
        }
        protected override void Unobserve(PoseRemapper_RotationTransformation observable)
        {
            observable.RemoveObserver(this);
        }
        #endregion
    }
}
