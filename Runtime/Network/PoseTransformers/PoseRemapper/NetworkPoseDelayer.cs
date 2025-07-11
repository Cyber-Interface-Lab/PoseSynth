using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    public class NetworkPoseDelayer : NetworkPoseRemapper<PoseDelayer>
    {
        #region enum
        #endregion

        #region const
        #endregion

        #region public variable
        #endregion

        #region private variable
        #endregion

        #region public method
        public override void OnNotified(PoseDelayer observable)
        {
            base.OnNotified(observable);
            // synchronize the delay time
            m_remapper.DelayFixedFrame = observable.DelayFixedFrame;
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
        protected override void Observe(PoseDelayer observable)
        {
            observable.AddObserver(this);
        }
        protected override void Unobserve(PoseDelayer observable)
        {
            observable.RemoveObserver(this);
        }
        #endregion

        #region event
        #endregion
    
        #region GUI
        #endregion
    }
}
