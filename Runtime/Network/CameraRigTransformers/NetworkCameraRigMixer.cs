using CyberInterfaceLab.PoseSynth.Network;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// NetworkBehaviour for <see cref="CameraRigMixer"/>.
    /// </summary>
    [RequireComponent(typeof(CameraRigMixer))]
    public class NetworkCameraRigMixer : PSNetworkBehaviour, IObserver<CameraRigMixer>
    {
        #region public variable
        public CameraRigMixer CameraRigMixer => m_cameraRigMixer;
        #endregion

        #region private variable
        private CameraRigMixer m_cameraRigMixer;
        #endregion

        #region public method
        #endregion

        #region private method
        [ClientRpc]
        private void UpdateWeightClientRpc(int index, TrackerType type, float weight)
        {
            m_cameraRigMixer.SetWeight(m_cameraRigMixer.References[index], type, weight);
        }
        #endregion

        #region event
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_cameraRigMixer = GetComponent<CameraRigMixer>();

            // enable the GUI of CameraRigMixer only in the server.
            m_cameraRigMixer.IsDrawingGUI = IsServer;

            // subscribe CameraRigMixer
            m_cameraRigMixer.AddObserver(this);
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            m_cameraRigMixer.IsDrawingGUI = true;
            m_cameraRigMixer.RemoveObserver(this);
        }
        public void OnNotified(CameraRigMixer observable)
        {
            if (!IsServer)
            {
                return;
            }
            // synchronize weights
            for (int j = 0; j < observable.MixedTrackerGroups.Count; j++)
            {
                var group = observable.MixedTrackerGroups[j];
                for (int i = 0; i < observable.References.Count; i++)
                {
                    var cameraRig = observable.References[i];

                    if (observable.TryGetWeight(cameraRig, group.TrackerType, out float weight))
                    {
                        UpdateWeightClientRpc(i, group.TrackerType, weight);
                    }
                }
            }
        }
        #endregion
    }
}
