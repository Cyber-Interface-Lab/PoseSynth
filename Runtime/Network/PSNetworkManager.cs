using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Events;

namespace CyberInterfaceLab.PoseSynth.Network
{
    /// <summary>
    /// Network manager for PoseSynth
    /// </summary>
    public class PSNetworkManager : Singleton<PSNetworkManager>
    {
        [SerializeField]
        private LocalCameraRig m_localCameraRig;
        public LocalCameraRig LocalCameraRig => m_localCameraRig;
        [SerializeField]
        private UnityTransport m_networkTransport;
        public UnityTransport NetworkTransport => m_networkTransport;

        #region GUI
        private int m_windowId = Utilities.GetWindowId();
        private Rect m_windowRect = new Rect(0, 0, 200, 100);
        private void OnGUI()
        {
            m_windowRect = GUI.Window(m_windowId, m_windowRect, (id) =>
            {
                if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
                {
                    if (GUILayout.Button("Shutdown"))
                    {
                        NetworkManager.Singleton.Shutdown();
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Server IP:");
                    m_networkTransport.ConnectionData.Address = GUILayout.TextArea(m_networkTransport.ConnectionData.Address);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button($"Host"))
                    {
                        NetworkManager.Singleton.StartHost();
                    }
                    if (GUILayout.Button($"Client"))
                    {
                        NetworkManager.Singleton.StartClient();
                    }
                    GUILayout.EndHorizontal();
                }

                GUI.DragWindow();
            }, "NetCode GUI");
        }
        #endregion
    }
}