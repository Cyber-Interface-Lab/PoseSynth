using CyberInterfaceLab.PoseSynth.Network;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// <see cref="PoseMapper"/>用の<see cref="NetworkBehaviour"/>です。
    /// <see cref="PoseMapper.Reference"/>をネットワークで同期します。
    /// <see cref="NetworkBehaviour"/> for <see cref="PoseMapper"/>.
    /// This class is used to synchronize <see cref="PoseMapper.Reference"/> across the network.
    /// </summary>
    public abstract class NetworkPoseMapper<T> : PSNetworkBehaviour, IObserver<T> where T: PoseMapper
    {
        #region public variable
        //public PoseMapper[] Mappers => m_mappers;
        public T Mapper => m_mapper;
        #endregion

        #region private variable
        //private PoseMapper[] m_mappers;
        protected T m_mapper;
        #endregion

        #region public method
        /// <summary>
        /// <see cref="PoseMapper.Reference"/>を設定します。
        /// Set <see cref="PoseMapper.Reference"/>.
        /// </summary>
        /// <remarks>
        /// この関数は<see cref="OnNotified(T)"/>内で呼び出されます。
        /// This method is called in <see cref="OnNotified(T)"/>.
        /// </remarks>
        /// <param name="cameraRig"></param>
        public void SetReference(ICameraRig cameraRig)
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

            //Debug.Log("SetCameraRig");
            if (cameraRig == null)
            {
                //SetCameraRigToNullServerRpc();
                //return;
                if (IsClient)
                    SetCameraRigToNullServerRpc(); // -> SetCameraRigToNullClientRpc()
                else if (IsServer)
                {
                    /*
                    foreach (var mapper in m_mappers)
                    {
                        mapper.SetCameraRigWithoutNotice(null);
                    }
                    */
                    m_mapper.SetCameraRigWithoutNotice(null);
                    // notify to clients
                    SetCameraRigToNullClientRpc();
                }
            }
            else if (cameraRig is MonoBehaviour mb && mb.TryGetComponent<NetworkObject>(out var no) && no.IsSpawned)
            {
                //SetCameraRigServerRpc(no.NetworkObjectId);
                //return;
                if (IsClient)
                    SetCameraRigServerRpc(no.NetworkObjectId); // -> SetCameraRigClientRpc(no.NetworkObjectId)
                else if (IsServer)
                {
                    /*
                    foreach (var mapper in m_mappers)
                    {
                        mapper.SetCameraRigWithoutNotice(cameraRig);
                    }
                    */
                    m_mapper.SetCameraRigWithoutNotice(cameraRig);
                    // notify to clients
                    SetCameraRigClientRpc(no.NetworkObjectId);
                }
            }
            else
            {
                Debug.LogError($"The CameraRig is not a network object!");
            }
        }
        /// <inheritdoc/>
        public void OnNotified(T mapper)
        {
            SetReference(mapper.Reference);
        }
        /// <summary>
        /// 特定の<see cref="T"/>を観測します。
        /// 内部で<see cref="IObservable{T}.AddObserver(IObserver{T})"/>を呼び出します。
        /// Observe a specific <see cref="T"/>.
        /// This method calls <see cref="IObservable{T}.AddObserver(IObserver{T})"/> internally.
        /// </summary>
        /// <param name="observable"></param>
        protected abstract void Observe(T observable);
        /// <summary>
        /// 特定の<see cref="T"/>の観測を解除します。
        /// 内部で<see cref="IObservable{T}.RemoveObserver(IObserver{T})"/>を呼び出します。
        /// Unobserve a specific <see cref="T"/>.
        /// This method calls <see cref="IObservable{T}.RemoveObserver(IObserver{T})"/> internally.
        /// </summary>
        /// <param name="observable"></param>
        protected abstract void Unobserve(T observable);
        #endregion

        #region private method
        [ServerRpc(RequireOwnership = false)]
        protected virtual void SetCameraRigServerRpc(ulong networkObjectId)
        {
            //Debug.Log("SetCameraRigServerRpc: " + networkObjectId);
            var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            if (obj.TryGetComponent<ICameraRig>(out var cr))
            {
                /*
                foreach (var mapper in m_mappers)
                {
                    mapper.SetCameraRigWithoutNotice(cr);
                }
                */
                m_mapper.SetCameraRigWithoutNotice(cr);
            }
            SetCameraRigClientRpc(networkObjectId);
        }
        [ServerRpc(RequireOwnership = false)]
        protected virtual  void SetCameraRigToNullServerRpc()
        {
            /*
            foreach (var mapper in m_mappers)
            {
                mapper.SetCameraRigWithoutNotice(null);
            }
            */
            m_mapper.SetCameraRigWithoutNotice(null);
            SetCameraRigToNullClientRpc();
        }
        [ClientRpc]
        protected virtual void SetCameraRigClientRpc(ulong networkObjectId)
        {
            //Debug.Log("SetCameraRigClientRpc: " + networkObjectId);
            // search the network object whose id is equal to the argument.
            var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[networkObjectId];
            if (obj.TryGetComponent<ICameraRig>(out var cr))
            {
                /*
                foreach (var mapper in m_mappers)
                {
                    mapper.SetCameraRigWithoutNotice(cr);
                }
                */
                m_mapper.SetCameraRigWithoutNotice(cr);
                return;
            }

            Debug.LogError($"The network object (id: {networkObjectId}) does not have an ICameraRig!");
        }
        [ClientRpc]
        protected virtual void SetCameraRigToNullClientRpc()
        {
            /*
            foreach (var mapper in m_mappers)
            {
                mapper.SetCameraRigWithoutNotice(null);
            }
            */
            m_mapper.SetCameraRigWithoutNotice(null);
        }
        #endregion

        #region event
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            //m_mappers = GetComponents<PoseMapper>();
            m_mapper = GetComponent<T>();

            /*
            for (int i = 0; i < m_mappers.Length; i++)
            {
                m_mappers[i].AddObserver(this);
            }
            */
            Observe(m_mapper);
        }
        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            /*
            if (m_mappers == null) { return; }
            for (int i = 0; i < m_mappers.Length; i++)
            {
                m_mappers[i]?.RemoveObserver(this);
            }
            */
            Unobserve(m_mapper);
        }
        #endregion
    }
}
