using CyberInterfaceLab.PoseSynth.Network;
using CyberInterfaceLab.PoseSynth.Network.UserInterfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace CyberInterfaceLab.PoseSynth
{
    /// <summary>
    /// Pooled avatar.
    /// </summary>
    [Obsolete]
    public class PooledAvatar : MonoBehaviour, IPooledObject<PooledAvatar>
    {
        #region public variable
        public IObjectPool<PooledAvatar> ObjectPool { set => m_pool = value; }
        public Pose Pose => m_pose;
        #endregion

        #region private variable
        private IObjectPool<PooledAvatar> m_pool;
        /// <summary>
        /// <see cref="Pose"/> attached to it.
        /// </summary>
        [SerializeField] private Pose m_pose;
        private PooledButton m_button;
        #endregion

        #region public method
        public void Initialize()
        {
            //var networkGUI = NetworkGUIStateMachine.Instance;
            //if (networkGUI == null) { return; }

            // add button
            //m_button = NetworkGUIStateMachine.Instance.AddButton();
            //InitializePooledButton(m_button);
        }
        public void Deactivate()
        {
            m_pool.Release(this);
        }
        #endregion
    }
}
