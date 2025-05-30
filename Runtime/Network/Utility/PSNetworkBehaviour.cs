using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// PoseSynth用に独自に拡張された<see cref="NetworkBehaviour"/>です。
    /// NetworkBehaviourについては、NetCodeの公式ドキュメントを参照してください。
    /// Orignal <see cref="NetworkBehaviour"/> for PoseSynth.
    /// For more information about NetworkBehaviour, see the official documentation of NetCode.
    /// </summary>
    /// <remarks>
    /// Serverと名の付くイベントは、サーバでのみ呼び出されます。
    /// Clientと名の付くイベントは、クライアントでのみ呼び出されます。
    /// Ownerと名の付くイベントは、この<see cref="NetworkObject"/>の所有者のクライアントでのみ呼び出されます。
    /// ほぼ全てのNetworkObjectのOwnerはサーバ（ホスト）ですが、PlayerObjectのOwnerはクライアントになります。
    /// Events with "Server" are called only on the server.
    /// Events with "Client" are called only on the client.
    /// Events with "Owner" are called only on the client that owns this <see cref="NetworkObject"/>.
    /// Almost all NetworkObjects are owned by the server (host), but PlayerObjects are owned by the client.
    /// </remarks>
    public abstract class PSNetworkBehaviour : NetworkBehaviour
    {
        //protected NetworkObject m_networkObject;
        protected Transform m_transform;

        /// <summary>
        /// <see cref="NetworkObject"/>としてこのゲームオブジェクトをスポーンさせます。
        /// 必ずサーバで呼び出してください。
        /// Spawn this gameObject as a NetworkObject. Call this method on the server.
        /// </summary>
        public void Spawn()
        {
            //m_networkObject.Spawn();
            NetworkObject.Spawn();
        }
        /// <summary>
        /// <see cref="NetworkObject"/>をデスポーンさせ、全てのクライアントからこのゲームオブジェクトを削除します。
        /// 必ずサーバで呼び出してください。
        /// Despawn the NetworkObject and remove this game object from all clients.
        /// Call this method on the server.
        /// </summary>
        public void Despawn()
        {
            //m_networkObject.Despawn();
            NetworkObject.Despawn();
        }

        #region MonoBehaviour events
        // Do not use in the inherit class
        private void Update()
        {
            if (IsOwner)
            {
                OwnerUpdate();
            }
            if (IsServer)
            {
                ServerUpdate();
            }
            if (IsClient)
            {
                ClientUpdate();
            }
        }
        private void FixedUpdate()
        {
            if (IsOwner)
            {
                OwnerFixedUpdate();
            }
            if (IsServer)
            {
                ServerFixedUpdate();
            }
            if (IsClient)
            {
                ClientFixedUpdate();
            }
        }
        private void LateUpdate()
        {
            if (IsOwner)
            {
                OwnerLateUpdate();
            }
            if (IsServer)
            {
                ServerLateUpdate();
            }
            if (IsClient)
            {
                ClientUpdate();
            }
        }
        #endregion

        #region original events
        public virtual void OwnerUpdate() { }
        public virtual void ServerUpdate() { }
        public virtual void ClientUpdate() { }
        public virtual void OwnerFixedUpdate() { }
        public virtual void ServerFixedUpdate() { }
        public virtual void ClientFixedUpdate() { }
        public virtual void OwnerLateUpdate() { }
        public virtual void ServerLateUpdate() { }
        public virtual void ClientLateUpdate() { }
        #endregion
    }

}