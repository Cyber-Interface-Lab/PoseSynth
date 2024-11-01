using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.UI;

namespace CyberInterfaceLab.PoseSynth.Network.UserInterfaces
{
    /// <summary>
    /// Network GUI for offline user.
    /// Input IP address to connect to server.
    /// </summary>
    public class NetworkGUIOffline : MonoBehaviour
    {
        #region public variable
        #endregion

        #region private variable
        private NetworkManager m_networkManager;
        private UnityTransport m_transport;
        [SerializeField]
        private TMPro.TextMeshProUGUI m_addressText;

        // buttons
        [Header("Buttons")]
        [SerializeField] private Button m_button0;
        [SerializeField] private Button m_buttonDot;
        [SerializeField] private Button m_button1;
        [SerializeField] private Button m_button2;
        [SerializeField] private Button m_button3;
        [SerializeField] private Button m_button4;
        [SerializeField] private Button m_button5;
        [SerializeField] private Button m_button6;
        [SerializeField] private Button m_button7;
        [SerializeField] private Button m_button8;
        [SerializeField] private Button m_button9;
        [SerializeField] private Button m_buttonClear;
        [SerializeField] private Button m_buttonBackspace;
        [SerializeField] private Button m_buttonJoin;
        #endregion

        #region public method
        #endregion

        #region private method
        private void AddStringToAddress(string str)
        {
            if (m_networkManager.IsClient || m_networkManager.IsServer)
                return;

            m_transport.ConnectionData.Address += str;
        }
        private void RemoveLastStringOfAddress()
        {
            if (m_networkManager.IsClient || m_networkManager.IsServer)
                return;

            string address = m_transport.ConnectionData.Address;
            address = address[..^1];
            m_transport.ConnectionData.Address = address;
        }
        private void ResetAddress()
        {
            if (m_networkManager.IsClient || m_networkManager.IsServer)
                return;

            m_transport.ConnectionData.Address = "";
        }
        private void ConnectToServer()
        {
            m_networkManager.StartClient();
        }
        #endregion

        #region event
        void Awake()
        {
            // set button event
            m_button0.onClick.AddListener(() => AddStringToAddress("0"));
            m_buttonDot.onClick.AddListener(() => AddStringToAddress("."));
            m_button1.onClick.AddListener(() => AddStringToAddress("1"));
            m_button2.onClick.AddListener(() => AddStringToAddress("2"));
            m_button3.onClick.AddListener(() => AddStringToAddress("3"));
            m_button4.onClick.AddListener(() => AddStringToAddress("4"));
            m_button5.onClick.AddListener(() => AddStringToAddress("5"));
            m_button6.onClick.AddListener(() => AddStringToAddress("6"));
            m_button7.onClick.AddListener(() => AddStringToAddress("7"));
            m_button8.onClick.AddListener(() => AddStringToAddress("8"));
            m_button9.onClick.AddListener(() => AddStringToAddress("9"));
            m_buttonClear.onClick.AddListener(ResetAddress);
            m_buttonBackspace.onClick.AddListener(RemoveLastStringOfAddress);
            m_buttonJoin.onClick.AddListener(ConnectToServer);
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
            // update tmpro text
            m_addressText.text = m_transport.ConnectionData.Address;
        }
        #endregion
    }
}
