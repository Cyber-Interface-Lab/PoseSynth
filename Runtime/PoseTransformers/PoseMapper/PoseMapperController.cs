using CyberInterfaceLab.PoseSynth.Network;
using CyberInterfaceLab.PoseSynth.Network.UserInterfaces;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Controller of <see cref="PoseMapper"/>.
    /// Add a button to embody the avatar (<see cref="PoseMapper.Pose"/>).
    /// </summary>
    [RequireComponent(typeof(PoseMapper))]
    public class PoseMapperController : MonoBehaviour
    {
        #region public variable
        #endregion

        #region private variable
        //[SerializeField]
        private PoseMapper[] m_mappers;
        /// <summary>
        /// <see cref="PooledButton"/> that is pooled by <see cref="ButtonPoolManager"/>.
        /// At Start(), it adds a button to <see cref="NetworkGUIStateMachine"/>'s GUI.
        /// </summary>
        private PooledButton m_button;

        [SerializeField] private bool m_addButtonToGUI = true;
        #endregion

        #region public method
        public void SetMapperCameraRig(ICameraRig cameraRig)
        {
            // set active
            gameObject.SetActive(cameraRig is object);

            // set cameraRigs
            m_mappers = GetComponents<PoseMapper>();
            foreach (var mapper in m_mappers)
            {
                mapper.CameraRig = cameraRig;
            }
        }
        public void ResetMapperCameraRig() => SetMapperCameraRig(null);
        #endregion

        #region private method
        /// <summary>
        /// One of two functions on button clicked.
        /// aaply all the <see cref="PoseMapper"/> to <see cref="ICameraRig"/> of the client who clicked it.
        /// </summary>
        private void ApplyPoseMapper()
        {
            // search server camera rig of this local player
            ICameraRig cameraRig = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().GetComponent<ServerCameraRig>();
            // embody to the pose via PoseMapper
            SetMapperCameraRig(cameraRig);

            // remove this listener
            m_button.Button.onClick.RemoveAllListeners();
            // add another function
            m_button.Button.onClick.AddListener(QuitPoseMapper);
            m_button.Text.text = $"Quit {name}";
        }
        /// <summary>
        /// One of two functions on button clicked.
        /// </summary>
        private void QuitPoseMapper()
        {
            ResetMapperCameraRig();

            // remove this listener
            m_button.Button.onClick.RemoveAllListeners();
            // add another function
            m_button.Button.onClick.AddListener(ApplyPoseMapper);
            m_button.Text.text = $"Embody {name}";
        }
        #endregion

        #region event
        private void Start()
        {
            if (m_addButtonToGUI)
            {
                var networkGUI = NetworkGUIStateMachine.Instance;
                if (networkGUI != null)
                {
                    // add button
                    m_button = NetworkGUIStateMachine.Instance.AddButton();
                    m_button.Button.onClick.AddListener(ApplyPoseMapper);
                    m_button.Text.text = $"Embody {name}";
                }
            }
        }
        private void OnDestroy()
        {
            if (m_button == null) { return; }
            m_button.Deactivate();
        }
        #endregion
    }
}