using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace CyberInterfaceLab.PoseSynth.Network.UserInterfaces
{
    /// <summary>
    /// Network GUI for client.
    /// </summary>
    public class NetworkGUIClient : MonoBehaviour
    {
        #region public variable
        #endregion

        #region private variable
        private NetworkManager m_networkManager;
        private UnityTransport m_transport;
        [SerializeField]
        private TMPro.TextMeshProUGUI m_addressText;

        [Header("Buttons")]
        [SerializeField] private Button m_buttonQuit;
        #endregion

        #region public method
        #endregion

        #region private method
        private void Shutdown()
        {
            m_networkManager.Shutdown();
        }
        #endregion

        #region event
        void Awake()
        {
            m_buttonQuit.onClick.AddListener(Shutdown);
        }
        private void Start()
        {
            // get singletons
            m_networkManager = NetworkManager.Singleton;
            try
            {
                m_transport = m_networkManager.NetworkConfig.NetworkTransport as UnityTransport;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                enabled = false;
            }
        }
        void FixedUpdate()
        {
            if (m_networkManager.IsClient)
            {
                m_addressText.text = m_transport.ConnectionData.Address;
            }
        }
        #endregion
    }
}
