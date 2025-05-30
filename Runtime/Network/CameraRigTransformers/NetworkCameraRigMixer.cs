using CyberInterfaceLab.PoseSynth.Network;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// <see cref="CameraRigMixer"/>用の<see cref="NetworkBehaviour"/>です。
    /// <see cref="CameraRigMixer"/>の各トラッカの重みをネットワーク間で同期します。
    /// <see cref="NetworkBehaviour"/> for <see cref="CameraRigMixer"/>.
    /// This class is used to synchronize the weights of <see cref="CameraRigMixer"/> across the network.
    /// </summary>
    /// <remarks>
    /// 参照している<see cref="Pose"/>の追加/削除には対応していません。疑似的な追加/削除は重みを0に変更することで行えます。
    /// This class cannot add/remove the reference <see cref="Pose"/>s. For such purpose, set the weight from/to 0.
    /// </remarks>
    [RequireComponent(typeof(CameraRigMixer))]
    public class NetworkCameraRigMixer : PSNetworkBehaviour, IObserver<CameraRigMixer>
    {
        #region public variable
        /// <summary>
        /// このクラスが参照している<see cref="CameraRigMixer"/>を取得します（読込専用）。
        /// Get the <see cref="CameraRigMixer"/> that this class refers to (read-only).
        /// </summary>
        public CameraRigMixer CameraRigMixer => m_cameraRigMixer;
        #endregion

        #region private variable
        private CameraRigMixer m_cameraRigMixer;
        #endregion

        #region public method
        #endregion

        #region private method
        /// <summary>
        /// 特定の<see cref="ICameraRig"/>のトラッカの重みを変更します。
        /// Update the weight of a specific <see cref="ICameraRig"/>'s tracker.
        /// </summary>
        /// <param name="index">Index of the <see cref="ICameraRig"/>.</param>
        /// <param name="type"><see cref="TrackerType"/> of the target tracker.</param>
        /// <param name="weight">Weight in [0, 1].</param>
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
            if (!IsServer)
            {
                m_cameraRigMixer.IsDrawingGUI = false;
            }

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
