using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Unity.Netcode;
using System;

namespace CyberInterfaceLab.PoseSynth.Network.UserInterfaces
{
    //public class NetworkGUIStateMachine : Singleton<NetworkGUIStateMachine>
    public class NetworkGUIStateMachine : MonoBehaviour
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

        // pool of toggles
        //[SerializeField] ButtonPoolManager m_buttonPool;
        [SerializeField] private TogglePoolManager m_pool;
        private List<PooledToggle> m_toggles = new(64);

        [Header("Poses controlled by users")]
        [SerializeField] private PoseMapperController[] m_poseMapperControllers;
        #endregion

        #region public method
        public void Transit(State nextState)
        {
            if (m_currentState == nextState) return;

            m_currentState?.OnExit.Invoke();
            m_currentState = nextState;
            m_currentState?.OnEnter.Invoke();
        }
        /*

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
        */
        #endregion

        #region private method
        private void SpawnToggle(PoseMapperController poseMapperController)
        {
            if (m_pool.TryGet(out var toggle))
            {
                toggle.Initialize();
                toggle.Toggle.onValueChanged.AddListener((isOn) =>
                {
                    if (isOn)
                    {
                        // search server camera rig of this local player
                        // then embody to the pose via PoseMapper
                        var serverCameraRig = m_networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<ServerCameraRig>();
                        poseMapperController.SetMapperCameraRig(serverCameraRig);
                    }
                    else
                    {
                        poseMapperController.ResetMapperCameraRig();
                    }
                });

                // set labels (on/off)
                toggle.TextOn.text = $"{poseMapperController.name}";
                toggle.TextOff.text = $"{poseMapperController.name}";

                m_toggles.Add(toggle);
            }
        }
        private void SpawnToggles(params PoseMapperController[] poseMapperControllers)
        {
            foreach (var poseMapperController in poseMapperControllers)
            {
                SpawnToggle(poseMapperController);
            }
        }
        #endregion

        #region event
        void Start()
        {
            m_networkManager = NetworkManager.Singleton;

            // add toggles for each pose mapper controller
            SpawnToggles(m_poseMapperControllers);
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

            // turn off all toggles when the client is disconnected
            if (m_networkManager.IsClient && !m_networkManager.IsConnectedClient)
            {
                foreach (var toggle in m_toggles)
                {
                    toggle.Toggle.isOn = false;
                }
            }
        }
        #endregion

        #region GUI
        #endregion
    }
}
