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
    public abstract class NetworkPoseRemapper<T> : PSNetworkBehaviour, IObserver<T> where T : PoseRemapper
    {
        #region public variable
        public T Remapper => m_remapper;
        #endregion

        #region private variable
        protected T m_remapper;
        #endregion

        #region public method
        /// <summary>
        /// Set the reference <see cref="Pose"/> to the <see cref="PoseRemapper"/>.
        /// </summary>
        /// <param name="refPose"></param>
        public void SetReference(Pose refPose)
        {
            /*
            処理順
            Clientの場合
                -> ServerにRPCを送信 -> ServerがReferenceを変更
                -> Serverが全ClientにRPCを送信 -> ClientがReferenceを変更
            Serverの場合
                -> Referenceを変更
                -> 全ClientにRPCを送信 -> ClientがReferenceを変更

            Order of processing
            Client
                -> Send RPC to Server -> Set Reference on Server
                -> Send RPC to all Clients -> Set Reference on Client
            Server
                -> Set Reference
                -> Send RPC to all Clients -> Set Reference on Client
             */

            if (refPose == null)
            {
                if (IsClient)
                    SetRefPoseToNullServerRpc(); // -> SetRefPoseToNullClientRpc()
                else if (IsServer)
                {
                    m_remapper.SetRefPoseWithoutNotice(null);
                    // notify to clients
                    SetRefPoseToNullClientRpc();
                }
            }
            else if (refPose.TryGetComponent<NetworkObject>(out var no) && no.IsSpawned)
            {
                if (IsClient)
                    SetRefPoseServerRpc(no.NetworkObjectId); // -> SetRefPoseClientRpc(no.NetworkObjectId)
                else if (IsServer)
                {
                    m_remapper.SetRefPoseWithoutNotice(refPose);
                    // notify to clients
                    SetRefPoseClientRpc(no.NetworkObjectId);
                }
            }
            else
            {
                Debug.LogError($"The Pose ({refPose.name}) is not a network object!");
            }
        }
        public virtual void OnNotified(T observable)
        {
            SetReference(observable.Reference);
        }
        protected abstract void Observe(T observable);
        protected abstract void Unobserve(T observable);
        #endregion

        #region private method
        [ServerRpc(RequireOwnership = false)]
        protected virtual void SetRefPoseServerRpc(ulong networkObjectId)
        {
            var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            if (obj.TryGetComponent<Pose>(out var pose))
            {
                //m_remapper.RefPose = pose;
                m_remapper.SetRefPoseWithoutNotice(pose);
            }
            SetRefPoseClientRpc(networkObjectId);
        }
        [ServerRpc(RequireOwnership = false)]
        protected virtual void SetRefPoseToNullServerRpc()
        {
            SetRefPoseToNullClientRpc();
        }
        [ClientRpc]
        protected virtual void SetRefPoseClientRpc(ulong networkObjectId)
        {
            // search the network object whose id is equal to the argument.
            var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            if (obj.TryGetComponent<Pose>(out var pose))
            {
                //m_remapper.RefPose = pose;
                m_remapper.SetRefPoseWithoutNotice(pose);
                return;
            }

            Debug.LogError($"The network object (id: {networkObjectId}) does not have a Pose!");
        }
        [ClientRpc]
        protected virtual void SetRefPoseToNullClientRpc()
        {
            m_remapper.SetRefPoseWithoutNotice(null);
        }
        #endregion

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            m_remapper = GetComponent<T>();
            //m_remapper.AddObserver(this as T);
            Observe(m_remapper);

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
            //m_remapper?.RemoveObserver(this);
            Unobserve(m_remapper);
        }
    }
}
