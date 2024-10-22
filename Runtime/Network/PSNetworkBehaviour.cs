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
        }
        #endregion
        #region original events
        public virtual void OwnerAwake() { }
        public virtual void ServerAwake() { }
        public virtual void OwnerStart() { }
        public virtual void ServerStart() { }
        public virtual void OwnerUpdate() { }
        public virtual void ServerUpdate() { }
        public virtual void OwnerFixedUpdate() { }
        public virtual void ServerFixedUpdate() { }
        public virtual void OwnerLateUpdate() { }
        public virtual void ServerLateUpdate() { }
        #endregion
    }

}