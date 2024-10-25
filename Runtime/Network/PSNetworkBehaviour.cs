using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// Orignal NetworkBehaviour for PoseSynth.
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public abstract class PSNetworkBehaviour : NetworkBehaviour
    {
        protected NetworkObject m_networkObject;
        protected Transform m_transform;

        /// <summary>
        /// Spawn a NetworkObject.
        /// </summary>
        public void Spawn()
        {
            m_networkObject.Spawn();
        }
        public void Despawn()
        {
            m_networkObject.Despawn();
        }

        #region MonoBehaviour events
        // Do not use in the inherit class
        private void Awake()
        {
            m_transform = transform;
            m_networkObject = GetComponent<NetworkObject>();
            if (IsOwner)
            {
                OwnerAwake();
            }
            if (IsServer)
            {
                ServerAwake();
            }
            if (IsClient)
            {
                ClientAwake();
            }
        }
        private void Start()
        {
            if (IsOwner)
            {
                OwnerStart();
            }
            if (IsServer)
            {
                ServerStart();
            }
            if (IsClient)
            {
                ClientStart();
            }
        }
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
        [Obsolete("Use OnNetworkSpawn() instead.")]
        public virtual void OwnerAwake() { }
        [Obsolete("Use OnNetworkSpawn() instead.")]
        public virtual void ServerAwake() { }
        [Obsolete("Use OnNetworkSpawn() instead.")]
        public virtual void ClientAwake() { }
        [Obsolete("Use OnNetworkSpawn() instead.")]
        public virtual void OwnerStart() { }
        [Obsolete("Use OnNetworkSpawn() instead.")]
        public virtual void ServerStart() { }
        [Obsolete("Use OnNetworkSpawn() instead.")]
        public virtual void ClientStart() { }
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