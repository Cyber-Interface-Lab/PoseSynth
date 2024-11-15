using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Netcode;
using System;

namespace CyberInterfaceLab.PoseSynth.Network.UserInterfaces
{
    public class NetworkGUIStateMachine : Singleton<NetworkGUIStateMachine>
    {
        [Serializable]
        public class State
        {
            public UnityEvent OnEnter = new();
            public UnityEvent OnUpdate = new();
            public UnityEvent OnExit = new();
        }

        #region public variable
        #endregion

        #region private variable
        private State m_currentState;
        private NetworkManager m_networkManager;

        [Header("States")]
        [SerializeField] private State m_stateOffline;
        [SerializeField] private State m_stateClient;
        [SerializeField] private State m_stateServer;

        // pool of buttons
        [SerializeField] ButtonPoolManager m_buttonPool;
        #endregion

        #region public method
        public void Transit(State nextState)
        {
            if (m_currentState == nextState) return;

            m_currentState?.OnExit.Invoke();
            m_currentState = nextState;
            m_currentState?.OnEnter.Invoke();
        }

        public PooledButton AddButton()
        {
            if (m_buttonPool.TryGet(out var button))
            {
                return button;
            }
            return null;
        }
        public void RemoveButton(Button button)
        {
            if (button.TryGetComponent<PooledButton>(out var pb) && pb != null)
            {
                pb.Deactivate();
            }
        }
        #endregion

        #region private method
        #endregion

        #region event
        void Start()
        {
            m_networkManager = NetworkManager.Singleton;
        }
        void Update()
        {
            // transition
            if (m_networkManager.IsClient) // client and host
            {
                Transit(m_stateClient);
            }
            else if (m_networkManager.IsServer)
            {
                Transit(m_stateServer);
            }
            else
            {
                Transit(m_stateOffline);
            }

            m_currentState.OnUpdate.Invoke();
        }
        #endregion

        #region GUI
        #endregion
    }
}
