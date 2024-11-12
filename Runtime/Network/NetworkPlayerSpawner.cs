using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Events;
using System;
using System.Linq;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// Network object that controls a scene after starting a server.
    /// </summary>
    public class NetworkPlayerSpawner : PSNetworkBehaviour
    {
        /*
        [Serializable]
        public class ClientEvent
        {
            public ulong ClientID => m_clientId;
            [SerializeField]
            private ulong m_clientId = 0;
            public UnityEvent<ServerCameraRig> EventInitialize => m_eventInitialize;
            [SerializeField]
            private UnityEvent<ServerCameraRig> m_eventInitialize = new();
            public UnityEvent<ServerCameraRig> EventDespawn => m_eventDespawn;
            [SerializeField]
            private UnityEvent<ServerCameraRig> m_eventDespawn = new();
        }
        */
        /// <summary>
        /// This class is used to select a prefab of <see cref="ServerCameraRig"> from the enum <see cref="CameraRigType"/>.
        /// </summary>
        [Serializable]
        public class ServerCameraRigType
        {
            [SerializeField]
            private CameraRigType m_type;
            [SerializeField]
            private ServerCameraRig m_prefab;

            public CameraRigType Type => m_type;
            public ServerCameraRig Prefab => m_prefab;
        }
        [SerializeField]
        private List<ServerCameraRigType> m_cameraRigPrefabs;
        /// <summary>
        /// <see cref="CameraRigType"/> for each client.
        /// </summary>
        private Dictionary<ulong, CameraRigType> m_clientCameraRigType = new(64);

        /*
        public List<ClientEvent> ClientEvents => m_clientEvents;
        [SerializeField]
        private List<ClientEvent> m_clientEvents = new(32);
        public bool TryGetClientEvent(ulong clientID, out ClientEvent result)
        {
            foreach (var clientEvent in ClientEvents)
            {
                if (clientEvent.ClientID == clientID)
                {
                    result = clientEvent;
                    return true;
                }
            }
            result = null;
            return false;
        }
        */

        /*
        public UnityEvent<ServerCameraRig> EventInitializeByDefault => m_eventInitializeByDefault;
        [Header("Default Events")]
        [SerializeField]
        [Tooltip("This event is applied for client that Client Events does not contains.")]
        private UnityEvent<ServerCameraRig> m_eventInitializeByDefault;
        public UnityEvent<ServerCameraRig> EventDespawnByDefault => m_eventDespawnByDefault;
        [SerializeField]
        [Tooltip("This event is applied for client that Client Events does not contains.")]
        private UnityEvent<ServerCameraRig> m_eventDespawnByDefault;
        */

        /*
        /// <summary>
        /// Set event for the client.
        /// The events are called when the player object is initialized and despawned.
        /// </summary>
        /// <param name="clientId"></param>
        [Obsolete]
        public void SetEvent(ulong clientId)
        {
            if (ServerCameraRig.TryGetServerCameraRig(clientId, out var serverCameraRig))
            {
                SetServerCameraRigEvent(serverCameraRig);
            }
            else
            {
                Debug.LogWarning($"NetworkPlayerSpawner.SetEvent(): Client {clientId} does not have ServerCameraRig!");
            }
        }
        public void SetServerCameraRigEvent(ServerCameraRig serverCameraRig)
        {
            if (TryGetClientEvent(serverCameraRig.NetworkObject.OwnerClientId, out var _event))
            {
                serverCameraRig.EventInitialize = _event.EventInitialize;
                serverCameraRig.EventDespawn = _event.EventDespawn;
            }
            else
            {
                Debug.LogWarning($"NetworkPlayerSpawner.SetServerCameraRigEvent(): This scene does not have any valid events for Client {serverCameraRig.name}! Use default events.");
                serverCameraRig.EventInitialize = m_eventInitializeByDefault;
                serverCameraRig.EventDespawn = m_eventDespawnByDefault;
            }
        }
        */
        [ServerRpc(RequireOwnership = false)]
        private void OnNetworkSpawnServerRpc(ulong clientId, CameraRigType type)
        {
            Debug.Log($"NetworkPlayerSpawner.OnNetworkSpawnServerRpc(): Client {clientId.ToString()} has connected the server with CameraRig {type.ToString()}");

            // add the client to the dictionary.
            m_clientCameraRigType.Add(clientId, type);

            // select server camera rig prefab for the type of camera rig of client
            var prefab = m_cameraRigPrefabs.FirstOrDefault(x => x.Type == type)?.Prefab;
            if (prefab == null)
            {
                Debug.LogError($"NetworkPlayerSpawner.OnNetworkSpawnServerRpc(): The server does not have any prefabs for {type}!");
                return;
            }

            var serverCameraRig = Instantiate(prefab);
            serverCameraRig.name = serverCameraRig.name.Replace("(Clone)", $"(Client: {clientId.ToString()})");
            serverCameraRig.NetworkObject.SpawnAsPlayerObject(clientId);
        }
        // called when a client enters the scene or connects a server.
        public override void OnNetworkSpawn()
        {
            // if this is the spawned client, call the server rpc to spawn this server camera rig.
            if (IsClient)
            {
                //SpawnNetworkCameraRigServerRpc(NetworkManager.Singleton.LocalClientId, PSNetworkManager.Instance.LocalCameraRig.Type);
                OnNetworkSpawnServerRpc(NetworkManager.Singleton.LocalClientId, PSNetworkManager.Instance.LocalCameraRig.Type);
            }
        }
        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                m_clientCameraRigType.Clear();
            }
        }
    }
}