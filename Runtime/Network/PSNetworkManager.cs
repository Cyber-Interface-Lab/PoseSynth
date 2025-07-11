using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// PoseSynth用のネットワークマネージャです。
    /// Network manager for PoseSynth.
    /// </summary>
    /// <remarks>
    /// IMGUIを描画してネットワーク接続の管理を行うとともに、このクライアントの<see cref="LocalCameraRig"/>を保持します。
    /// Manage network connections and draw IMGUI, and hold the <see cref="LocalCameraRig"/> of this client.
    /// </remarks>
    public class PSNetworkManager : Singleton<PSNetworkManager>
    {
        private NetworkManager m_networkManager;
        [SerializeField]
        [Tooltip("The LocalCameraRig of this client.")]
        private LocalCameraRig m_localCameraRig;
        /// <summary>
        /// このクライアントの<see cref="LocalCameraRig"/>を取得します（読込専用）。
        /// Get the <see cref="LocalCameraRig"/> of this client (read-only).
        /// </summary>
        public LocalCameraRig LocalCameraRig => m_localCameraRig;
        [SerializeField]
        [Tooltip("NetworkTransport is used to draw the address and port in IMGUI.")]
        private UnityTransport m_networkTransport;
        //public UnityTransport NetworkTransport => m_networkTransport;

        [SerializeField]
        [Tooltip("The NetworkPlayerSpawner of this scene.")]
        private NetworkPlayerSpawner m_networkPlayerSpawner;
        /// <summary>
        /// このシーンの<see cref="NetworkPlayerSpawner"/>を取得します（読込専用）。
        /// Get the <see cref="NetworkPlayerSpawner"/> of this scene (read-only).
        /// </summary>
        public NetworkPlayerSpawner NetworkPlayerSpawner => m_networkPlayerSpawner;

        public void StartServer()
        {
            m_networkManager.StartServer();
        }
        public void StartClient()
        {
            m_networkManager.StartClient();
        }
        public void StartHost()
        {
            m_networkManager.StartHost();
        }
        public void StopServer()
        {
            m_networkManager.Shutdown();
        }
        public void StopClient()
        {
            m_networkManager.Shutdown();
        }
        public void StopHost()
        {
            m_networkManager.Shutdown();
        }

        private void Start()
        {
            m_networkManager = NetworkManager.Singleton;
        }

        #region GUI
        private int m_windowId = Utilities.GetWindowId();
        private Rect m_windowRect = new Rect(0, 0, 250, 100);
        private void OnGUI()
        {
            m_windowRect = GUI.Window(m_windowId, m_windowRect, (id) =>
            {
                if (m_networkManager.IsHost)
                {
                    if (GUILayout.Button("Shutdown"))
                    {
                        StopHost();
                    }
                    if (m_networkTransport != null)
                    {
                        GUILayout.Label($"Server address: {m_networkTransport.ConnectionData.Address}:{m_networkTransport.ConnectionData.Port}");
                    }
                }
                else if (m_networkManager.IsServer)
                {
                    if (GUILayout.Button("Shutdown"))
                    {
                        StopServer();
                    }
                    if (m_networkTransport != null)
                    {
                        GUILayout.Label($"Server address: {m_networkTransport.ConnectionData.Address}:{m_networkTransport.ConnectionData.Port}");
                    }
                }
                else if (m_networkManager.IsClient)
                {
                    if (GUILayout.Button("Shutdown"))
                    {
                        StopClient();
                    }
                    if (m_networkTransport != null)
                    {
                        GUILayout.Label($"Server address: {m_networkTransport.ConnectionData.Address}:{m_networkTransport.ConnectionData.Port}");
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Server IP:");
                    m_networkTransport.ConnectionData.Address = GUILayout.TextArea(m_networkTransport.ConnectionData.Address);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button($"Server"))
                    {
                        StartServer();
                    }
                    if (GUILayout.Button($"Host"))
                    {
                        StartHost();
                    }
                    if (GUILayout.Button($"Client"))
                    {
                        StartClient();
                    }
                    GUILayout.EndHorizontal();
                }

                GUI.DragWindow();
            }, "NetCode GUI (" + (
                m_networkManager.IsHost ? "Host" : 
                m_networkManager.IsServer ? "Server" : 
                m_networkManager.IsClient ? "Client" : 
                "Offline"
            ) + ")");
        }
        #endregion
    }
}