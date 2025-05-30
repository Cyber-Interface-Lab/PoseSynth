using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    public class NetworkPoseOffsetter : NetworkPoseRemapper<PoseOffsetter>
    {
        #region public variable
        #endregion

        #region private variable
        #endregion

        #region public method
        public override void OnNotified(PoseOffsetter observable)
        {
            base.OnNotified(observable);
            // synchronize the offset types
            m_remapper.Position = observable.Position;
            m_remapper.Rotation = observable.Rotation;
        }
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
        protected override void Observe(PoseOffsetter observable)
        {
            observable.AddObserver(this);
        }
        protected override void Unobserve(PoseOffsetter observable)
        {
            observable.RemoveObserver(this);
        }
        #endregion
    }
}
